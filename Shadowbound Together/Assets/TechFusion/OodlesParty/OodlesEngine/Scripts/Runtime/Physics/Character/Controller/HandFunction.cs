using System.Collections;
using UnityEngine;

namespace OodlesParty
{ 
    public enum HandSide
    {
        HandLeft = 0,
        HandRight = 1,
    }

    public class HandFunction : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        public OodlesCharacter oodlesCharacter;

        public HandSide handSide;

        [HideInInspector]
        public bool hasJoint;
        [HideInInspector]
        public Rigidbody GrabbedObject;
        [HideInInspector]
        public Joint catchJoint;

        public void ProcessInput()
        {
            //if (oodlesCharacter.useControls)
            {
                if (hasJoint && catchJoint == null)
                {
                    hasJoint = false;
                    GrabbedObject = null;
                }
            }
        }

        public void ReleaseHand()
        {
            if (GrabbedObject != null)
            {
                EventBetter.Raise(new ReleaseObjectMessage()
                {
                    pc = oodlesCharacter,
                    obj = GrabbedObject.gameObject
                });
            }

            if (catchJoint != null)
            {
                catchJoint.breakForce = 0;
                catchJoint.connectedBody = null;
                Destroy(catchJoint);
            }
        
            hasJoint = false;
            GrabbedObject = null;
        }

        void SetFixedJoint(GameObject obj)
        {
            catchJoint = this.gameObject.AddComponent<FixedJoint>();
            catchJoint.breakForce = Mathf.Infinity;
            catchJoint.connectedBody = obj.GetComponent<Rigidbody>();
            catchJoint.connectedBody.velocity = Vector3.zero;
            catchJoint.connectedBody.angularVelocity = Vector3.zero;


            //ConfigurableJoint cJoint = this.gameObject.AddComponent<ConfigurableJoint>();
            //cJoint.xMotion = ConfigurableJointMotion.Locked;
            //cJoint.yMotion = ConfigurableJointMotion.Locked;
            //cJoint.zMotion = ConfigurableJointMotion.Locked;

            //catchJoint = cJoint;
            //catchJoint.breakForce = Mathf.Infinity;
            //catchJoint.connectedBody = obj.GetComponent<Rigidbody>();

            ////test init loc and rot
            //cJoint.angularXMotion = ConfigurableJointMotion.Locked;
            //cJoint.angularYMotion = ConfigurableJointMotion.Locked;
            //cJoint.angularZMotion = ConfigurableJointMotion.Locked;
            //catchJoint.anchor = Vector3.zero;
            //catchJoint.axis = Vector3.forward;
        }

        void SetConfigurableJoint(GameObject obj)
        {
            ConfigurableJoint cJoint = this.gameObject.AddComponent<ConfigurableJoint>();
            cJoint.xMotion = ConfigurableJointMotion.Locked;
            cJoint.yMotion = ConfigurableJointMotion.Locked;
            cJoint.zMotion = ConfigurableJointMotion.Locked;

            catchJoint = cJoint;
            catchJoint.breakForce =  Mathf.Infinity;
            catchJoint.connectedBody = obj.GetComponent<Rigidbody>();
        }

        void OnGrabSomething(GameObject obj)
        {
            GrabbedObject = obj.GetComponent<Rigidbody>();

            if (GrabbedObject == null)
            {
                //Debug.Log("HandFunction Grabbed Object is null!");
                return;
            }

            //static wall objects
            if (GrabbedObject.isKinematic ||
                GrabbedObject.gameObject.layer == LayerMask.NameToLayer("Player") ||
                GrabbedObject.gameObject.layer == LayerMask.NameToLayer("Ragdoll") ||
                GrabbedObject.gameObject.layer == LayerMask.NameToLayer("RagdollHands"))
            {
                SetConfigurableJoint(obj);
            }
            else
            {
                //SetConfigurableJoint(col);
                SetFixedJoint(obj);
            }

            hasJoint = true;

            EventBetter.Raise(new GrabObjectMessage()
            {
                pc = oodlesCharacter,
                hf = this,
                obj = obj
            });
        }

        void OnAttackSomething(Collision col)
        {
            EventBetter.Raise(new HandAttackMessage()
            {
                pc = oodlesCharacter,
                col = col
            });
        }

        //Grab on collision
        void OnCollisionEnter(Collision col)
        {
            if (oodlesCharacter == null) return;

            //not me
            if (col.gameObject.transform.IsChildOf(oodlesCharacter.transform))
            {
                return;
            }

            if ((handSide == HandSide.HandLeft ? oodlesCharacter.IsLeftArmWorking() : oodlesCharacter.IsRightArmWorking()) && !hasJoint)
            {
                OnGrabSomething(col.gameObject);
            }

            if (!hasJoint && oodlesCharacter.isAttacking)
            {
                OnAttackSomething(col);
            }
        }

        public void GrabSomething(GameObject go)
        {
            if (hasJoint) return;

            bool Grabable = false;

            EventBetter.Raise(new CheckGrabableMessage()
            {
                pc = null,//not used
                obj = go,
                callback = (bool b) => {
                    Grabable = b;
                }
            });

            if (Grabable)
            {
                OnGrabSomething(go);
            }
        }
    }
}