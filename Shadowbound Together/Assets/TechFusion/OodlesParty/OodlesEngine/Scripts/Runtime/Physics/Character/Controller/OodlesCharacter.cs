using System.Collections;
using UnityEngine;

namespace OodlesParty
{
    public partial class OodlesCharacter : MonoBehaviour
    {
        public Animator animatorPlayer;
        public GameObject ragdollPlayer;
        [HideInInspector]
        public ConfigurableJoint[] joints;

        [HideInInspector] public HandFunction handFunctionRight, handFunctionLeft;
        
        [SerializeField] private LayerMask ignoreGroundCheckOn;

        [Tooltip("Rotation range where the target direction influences arm movement.")]
        public float minTargetDirAngle = -30;
        public float maxTargetDirAngle = 60;

        [Space(10)]
        [Tooltip("The limits of the arms direction. How far down/up should they be able to point?")]
        public float minArmsAngle = -70;
        public float maxArmsAngle = 100;
        [Tooltip("The limits of the look direction. How far down/up should the character be able to look?")]
        public float minLookAngle = -50, maxLookAngle = 60;

        [Space(10)]
        [Tooltip("The vertical offset of the look direction in reference to the target direction.")]
        public float lookAngleOffset;
        [Tooltip("The vertical offset of the arms direction in reference to the target direction.")]
        public float armsAngleOffset;
        [Tooltip("Defines the orientation of the hands")]
        public float handsRotationOffset = 0;

        [Space(10)]
        [Tooltip("How far apart to place the arms")]
        public float armsHorizontalSeparation = 0.75f;

        //[Tooltip("The distance from the body to the hands in relation to how high/low they are. " +
        //         "Allows to create more realistic movement patterns.")]
        //public AnimationCurve armsDistance;

        public OodlesCharacterInput inputState;

        private bool ragdollMode, waitingGetUp = false, attackInput, pickUpInput;
        private bool isThrowing = false;
        [HideInInspector] public bool isAttacking = false;

        // IK
        private Vector3 armsDir, lookDir, iKDir, targetDir2D;
        private Transform animTorso, chest;
        private float targetDirVerticalPercent;

        private Rigidbody physicsBody;
        private ConfigurableJoint physicsBodyJoint;
        private AnimatorHelper animatorHelper;
        private GameObject stabilizerGameobject;
        private Rigidbody stabilizerRigidbody;
        private ConfigurableJoint stabilizerJoint;
        private CharacterMovement movement;
        private CharacterAnimatorEvent animatorEvent;

        void Awake()
        {
            physicsBody = ragdollPlayer.GetComponent<Rigidbody>();
            physicsBodyJoint = ragdollPlayer.GetComponent<ConfigurableJoint>();

            animatorPlayer.speed = animationSpeed;
            animatorHelper = animatorPlayer.gameObject.AddComponent<AnimatorHelper>();
            animatorEvent = animatorPlayer.gameObject.AddComponent<CharacterAnimatorEvent>();
            animatorEvent.controller = this;

            movement = ragdollPlayer.AddComponent<CharacterMovement>();
            movement.controller = this;

            string[] buildInIgnore = { OodlesSetting.Instance.PlayerLayerName, OodlesSetting.Instance.RagdollLayerName, OodlesSetting.Instance.RagdollHandsLayerName };
            LayerMask buildInIgnoreMask = LayerMask.GetMask(buildInIgnore);
            movement.whatIsGround = ~buildInIgnoreMask;
            movement.maxSpeed = 3.0f;
            movement.maxSlopeAngle = 50;
            movement.jumpForce = 25000;

            InitLayers();
            InitJointMatchs();
            //IgnoreInternalColliders();
            InitJointSprings();
            InitJointDrives();
            InitFeetMaterial();
            InitEffects();
            InitStateMachine();
        }

        //todo OodlesEngine
        //public override void OnStartLocalPlayer()
        //{
        //    CameraFollow.Get().player = physicsBody.transform;
        //}

        public void ChangeSkin(int skinColor)
        {
            CharacterSkin skin = ragdollPlayer.GetComponent<CharacterSkin>();
            if (skin)
            {
                skin.SetSkinMat((CharacterSkin.SkinColor)skinColor);
            }
        }

        public Rigidbody GetPhysicsBody()
        {
            return ragdollPlayer.GetComponent<Rigidbody>();
        }

        public ConfigurableJoint[] GetJoints()
        {
            return joints;
        }

        public ConfigurableJoint GetJoint(OodlesCharacter.BodyPart part)
        {
            return joints[(int)part];
        }

        void IgnoreInternalColliders()
        {
            Collider[] colliders = ragdollPlayer.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                for (int j = 0; j < colliders.Length; j++)
                {
                    if (i == j) continue;

                    Collider ci = colliders[i];
                    Collider cj = colliders[j];

                    if (ci != null && cj != null && ci.enabled && cj.enabled && ci.gameObject.activeInHierarchy && cj.gameObject.activeInHierarchy)
                    {
                        Physics.IgnoreCollision(ci, cj);
                    }
                }
            }
        }

        void IgnoreCollideWithMe(Collider c)
        {
            Collider[] colliders = ragdollPlayer.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider ci = colliders[i];
                if (ci != null && c != null && ci.enabled && c.enabled && ci.gameObject.activeInHierarchy && c.gameObject.activeInHierarchy)
                {
                    Physics.IgnoreCollision(ci, c);
                }
            }
        }

        void InitLayers()
        {
            string player = OodlesSetting.Instance.PlayerLayerName;    
            string ragdoll = OodlesSetting.Instance.RagdollLayerName;
            string ragdollHands = OodlesSetting.Instance.RagdollHandsLayerName;
            LayerUtility.SetLayerRecursively(transform, player);
            LayerUtility.SetLayerRecursively(GetJoint(OodlesCharacter.BodyPart.BP_Pelvis).transform, ragdoll);
            LayerUtility.SetLayerRecursively(handFunctionLeft.transform, ragdollHands);
            LayerUtility.SetLayerRecursively(handFunctionRight.transform, ragdollHands);
        }

        void InitJointMatchs()
        {
            JointMatch[] jointMatchs = ragdollPlayer.GetComponentsInChildren<JointMatch>();
            for (int i = 0; i < jointMatchs.Length; i++)
            {
                jointMatchs[i].oodlesCharacter = this;
            }
        }

        void InitFeetMaterial()
        {
            PhysicMaterial pm = new PhysicMaterial();
            pm.staticFriction = 0.0f;
            pm.dynamicFriction = 0.0f;
            pm.bounciness = 0.0f;
            pm.frictionCombine = PhysicMaterialCombine.Minimum;
            pm.bounceCombine = PhysicMaterialCombine.Average;

            if (physicsBody.GetComponent<CapsuleCollider>())
            {
                physicsBody.GetComponent<CapsuleCollider>().material = pm;
            }
        }

        void InitJointSprings()
        {

            for (int i = 0; i < joints.Length; i++)    //set joints to chosen values
            {
                SetJointParams(joints[i],
                    OodlesSetting.Instance.JointSpringsStrength,
                    OodlesSetting.Instance.JointSpringDamper);
            }
        }

        public void SetJointParams(ConfigurableJoint cj, float posSpring, float posDamper)
        {
            JointDrive jDrivex = cj.angularXDrive;
            JointDrive jDriveyz = cj.angularYZDrive;
            jDrivex.positionSpring = posSpring;
            jDriveyz.positionSpring = posSpring;
            jDrivex.positionDamper = posDamper;
            jDriveyz.positionDamper = posDamper;
            cj.angularXDrive = jDrivex;
            cj.angularYZDrive = jDriveyz;
        }

        public void ProcessInput(OodlesCharacterInput input)
        {
            inputState = input;

            //todo OodlesEngine
            //if (isServer)
            {
                TickState();
            }
        }

        public void UpdateJointState()
        {
            if (ragdollMode) return;

            if (movement.grounded)
            {
                if (inputState.forwardAxis != 0 || inputState.leftAxis != 0)
                {
                    JointMoveState();
                }
                else
                {
                    JointIdleState();
                }

                if (leftPicking)
                {
                    JointPickState();
                }

                if (rightPicking)
                {
                    JointPickState();
                }

                if (isAttacking)
                {
                    JointActionState();
                }
            }
            else
            {
                if (physicsBody.velocity.y < -10)
                {
                    ragdollMode = true;
                    JointLoseBalanceState();
                    ChangeState(State.LostControl);
                }
                else
                {
                    JointInAirState();
                }
            }
        }

        public void UpdateMovement()
        {
            movement.ProcessInput();
        }

        float getUpTime = 0;
        public void UpdateStandUp()
        {
            if (!ragdollMode) return;

            if (waitingGetUp)
            {
                getUpTime += Time.deltaTime;

                if (getUpTime >= 2.5f && getUpTime < 5f)
                {
                    JointDizzyState();
                }
                else if (getUpTime >= 5f)
                {
                    JointIdleState();

                    waitingGetUp = false;
                    getUpTime = 0;

                    ragdollMode = false;
                    ChangeState(State.Control);
                    return;
                }
            }

            if (physicsBody.velocity.magnitude < 0.2f)
            {
                waitingGetUp = true;
            }
        }

        public void UpdateAnimations()
        {
            if (ragdollMode) return;

            if (inputState.leftAxis != 0 || inputState.forwardAxis != 0)
            {
                if (animatorPlayer.GetFloat("BlendVertical") < 1)
                {
                    var vert = animatorPlayer.GetFloat("BlendVertical");
                    animatorPlayer.SetFloat("BlendVertical", vert += 0.1f);
                }
            }
            else
            {
                animatorPlayer.SetFloat("BlendVertical", 0);
            }

            if (movement.grounded)
            {
                animatorPlayer.SetBool("IsAir", false);

                if (!runningEffectVisible) runningEffectVisible = true;
            }
            else
            {
                animatorPlayer.SetBool("IsAir", true);

                if (runningEffectVisible) runningEffectVisible = false;
            }
        }

        public void KnockDown()
        {
            JointLoseBalanceState();

            ragdollMode = true;
            waitingGetUp = false;
            ChangeState(State.LostControl);
        }

        public void SyncAnimator()
        {
            animatorPlayer.transform.position = ragdollPlayer.transform.position;

            var lookPos = physicsBody.transform.forward;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            animatorPlayer.transform.rotation = rotation;
        }

        //public void UpdateAnimator()
        //{
        //    animatorPlayer.enabled = false;
        //    animatorPlayer.Update(Time.deltaTime);
        //}
        
        Vector3 GetFloorProjection(Vector3 vec)
        {
            return Vector3.ProjectOnPlane(vec, Vector3.up).normalized;
        }

        public void UpdateIK()
        {
            if (!leftPicking && !rightPicking)
            {
                animatorHelper.SimulateIK = false;
                UseLeftArm(0);
                UseRightArm(0);
                return;
            }

            UseLeftArm(leftPicking ? 1 : 0);
            UseRightArm(rightPicking ? 1 : 0);

            animatorHelper.LookIKWeight = 1;

            animTorso = GetAnimatorBodyPart(BodyPart.BP_Pelvis);
            chest = GetAnimatorBodyPart(BodyPart.BP_Spine);

            iKDir = physicsBody.transform.forward;
            targetDir2D = GetFloorProjection(iKDir);
            CalculateVerticalPercent();

            UpdateLookIK();
            UpdateArmsIK();
            animatorHelper.SimulateIK = true;
        }

        void CalculateVerticalPercent()
        {
            float directionAngle = curPickAngle;
            directionAngle -= 90;
            targetDirVerticalPercent = 1 - Mathf.Clamp01((directionAngle - minTargetDirAngle) / Mathf.Abs(maxTargetDirAngle - minTargetDirAngle));
        }

        void UpdateLookIK()
        {
            float lookVerticalAngle = targetDirVerticalPercent * Mathf.Abs(maxLookAngle - minLookAngle) + minLookAngle;
            lookVerticalAngle += lookAngleOffset;
            lookDir = Quaternion.AngleAxis(-lookVerticalAngle, animTorso.right) * targetDir2D;

            Vector3 lookPoint = GetAnimatorBodyPart(BodyPart.BP_Head).position + lookDir;
            animatorHelper.LookAtPoint(lookPoint);
        }

        void UpdateArmsIK()
        {
            float armsVerticalAngle = targetDirVerticalPercent * Mathf.Abs(maxArmsAngle - minArmsAngle) + minArmsAngle;
            armsVerticalAngle += armsAngleOffset;
            armsDir = Quaternion.AngleAxis(-armsVerticalAngle, animTorso.right) * targetDir2D;

            //todo,test
            //float currentArmsDistance = armsDistance.Evaluate(targetDirVerticalPercent);
            float currentArmsDistance = 0.5f;

            Vector3 armsMiddleTarget = chest.position + armsDir * currentArmsDistance;
            Vector3 upRef = Vector3.Cross(armsDir, animTorso.right).normalized;
            Vector3 armsHorizontalVec = Vector3.Cross(armsDir, upRef).normalized;
            Quaternion handsRot = armsDir != Vector3.zero ? Quaternion.LookRotation(armsDir, upRef)
                                                            : Quaternion.identity;

            if (pickTargetLeftHand != null)
            {
                animatorHelper.LeftHandTarget.position = pickTargetLeftHand.position;
            }
            else
            {
                animatorHelper.LeftHandTarget.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation / 2;
            }

            if (pickTargetRightHand != null)
            {
                animatorHelper.RightHandTarget.position = pickTargetRightHand.position;
            }
            else
            {
                animatorHelper.RightHandTarget.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation / 2;
            }

            animatorHelper.LeftHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, 90 - handsRotationOffset);
            animatorHelper.RightHandTarget.rotation = handsRot * Quaternion.Euler(0, 0, -90 + handsRotationOffset);

            var armsUpVec = Vector3.Cross(armsDir, animTorso.right).normalized;
            animatorHelper.LeftHandHint.position = armsMiddleTarget + armsHorizontalVec * armsHorizontalSeparation - armsUpVec;
            animatorHelper.RightHandHint.position = armsMiddleTarget - armsHorizontalVec * armsHorizontalSeparation - armsUpVec;
        }

        public void UpdateHandFunction()
        {
            handFunctionLeft.ProcessInput();
            handFunctionRight.ProcessInput();
        }

        public void UseLeftArm(float weight)
        {
            animatorHelper.LeftArmIKWeight = weight;

            if (weight > 0)
            {
                JointUseLeftArm();
            }
            else
            {
                JointUnUseLeftArm();
            }
        }

        public void UseRightArm(float weight)
        {
            animatorHelper.RightArmIKWeight = weight;

            if (weight > 0)
            {
                JointUseRightArm();
            }
            else
            {
                JointUnUseRightArm();
            }
        }

        public bool IsLeftArmWorking()
        {
            return leftPicking;
        }

        public bool IsRightArmWorking()
        {
            return rightPicking;
        }

        public Rigidbody LeftHandGrabObject()
        {
            if (handFunctionLeft == null) return null;

            return handFunctionLeft.GrabbedObject;
        }

        public Rigidbody RightHandGrabObject()
        {
            if (handFunctionRight == null) return null;

            return handFunctionRight.GrabbedObject;
        }

        public void UpdateThrow()
        {
            if (inputState.doAction1 == 1)
            {
                if (!isThrowing)
                {
                    Vector3 hor = physicsBody.transform.forward;
                    hor.y = 0;
                    hor.Normalize();

                    if (LeftHandGrabObject() && LeftHandGrabObject() == RightHandGrabObject())
                    {
                        Rigidbody rb = LeftHandGrabObject();

                        handFunctionLeft.ReleaseHand();
                        handFunctionRight.ReleaseHand();
                        leftPicking = false;
                        rightPicking = false;

                        EventBetter.Raise(new ThrowObjectMessage()
                        {
                            pc = this,
                            obj = rb.gameObject,
                            dir = (hor + Vector3.up * 2).normalized,
                            twoHands = true
                        });
                    }
                    else
                    {
                        if (LeftHandGrabObject())
                        {
                            Rigidbody rb = LeftHandGrabObject();

                            handFunctionLeft.ReleaseHand();
                            leftPicking = false;

                            EventBetter.Raise(new ThrowObjectMessage()
                            {
                                pc = this,
                                obj = rb.gameObject,
                                dir = (hor + Vector3.up * 2).normalized,
                                twoHands = false
                            });
                        }

                        if (RightHandGrabObject())
                        {
                            Rigidbody rb = RightHandGrabObject();

                            handFunctionRight.ReleaseHand();
                            rightPicking = false;

                            EventBetter.Raise(new ThrowObjectMessage()
                            {
                                pc = this,
                                obj = rb.gameObject,
                                dir = (hor + Vector3.up * 2).normalized,
                                twoHands = false
                            });
                        }
                    }
                }

                isThrowing = true;
            }
            else
            {
                isThrowing = false;
            }
        }

        //pick up process
        private Coroutine coroutinePickUp = null;
        private bool leftPicking = false, rightPicking = false;
        private float curPickAngle = 20;
        private Transform pickTargetLeftHand = null;
        private Transform pickTargetRightHand = null;

        void SearchPickTarget()
        {
            EventBetter.Raise(new SearchGrabTargetMessage()
            {
                hc = handFunctionLeft,
                radius = 1.0f,
                callback = (Transform t)=> { pickTargetLeftHand = t; }
            });

            EventBetter.Raise(new SearchGrabTargetMessage()
            {
                hc = handFunctionRight,
                radius = 1.0f,
                callback = (Transform t) => { pickTargetRightHand = t; }
            });
        }

        void ClearPickTarget()
        {
            pickTargetLeftHand = null;
            pickTargetRightHand = null;
        }

        private IEnumerator PickUpAction(float pickAngle)
        {
            curPickAngle = pickAngle;
            leftPicking = true;
            rightPicking = true;
            SearchPickTarget();
            JointActionState();
            UseLeftArm(1);
            UseRightArm(1);

            // for simple
            while (curPickAngle >= 20)
            {
                yield return new WaitForSeconds(0.05f);
                curPickAngle -= 15;
            }

            leftPicking = false;
            rightPicking = false;
            curPickAngle = 20;

            ClearPickTarget();
        }


        public void UpdatePickUp()
        {
            if (inputState.fire2Axis > 0)
            {
                if (!pickUpInput)
                {
                    if (coroutinePickUp != null) { StopCoroutine(coroutinePickUp); }
                    coroutinePickUp = StartCoroutine(PickUpAction(170));
                }

                pickUpInput = true;
            }
            else
            {
                pickUpInput = false;
            }

            if (leftPicking)
            {
                if (pickTargetLeftHand != null)
                {
                    handFunctionLeft.GetComponent<Rigidbody>().AddForce(
                        (pickTargetLeftHand.position - handFunctionLeft.GetComponent<Rigidbody>().position).normalized * 14,
                        ForceMode.VelocityChange);
                }
            }

            if (rightPicking)
            {
                if (pickTargetRightHand != null)
                {
                    handFunctionRight.GetComponent<Rigidbody>().AddForce(
                        (pickTargetRightHand.position - handFunctionRight.GetComponent<Rigidbody>().position).normalized * 14,
                        ForceMode.VelocityChange);
                }
            }
        }

        public void UpdateAttack()
        {
            if (inputState.fire1Axis > 0)
            {
                if (!attackInput)
                {
                    animatorPlayer.SetTrigger("Attack");
                }

                attackInput = true;
            }
            else
            {
                attackInput = false;
            }
        }
    }
}
