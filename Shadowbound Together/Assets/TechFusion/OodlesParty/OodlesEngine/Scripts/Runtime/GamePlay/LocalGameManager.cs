using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{
    public class LocalGameManager : SingletonMono<LocalGameManager>
    {
        override protected void Awake()
        {
            base.Awake();

            EventBetter.Listen<LocalGameManager, SearchGrabTargetMessage>(this, OnSearchGrabTarget);
            EventBetter.Listen<LocalGameManager, CheckGrabableMessage>(this, OnCheckGrabable);
            EventBetter.Listen<LocalGameManager, GrabObjectMessage>(this, OnGrabObject);
            EventBetter.Listen<LocalGameManager, ReleaseObjectMessage>(this, OnReleaseObject);
            EventBetter.Listen<LocalGameManager, ThrowObjectMessage>(this, OnThrowObject);
            EventBetter.Listen<LocalGameManager, HandAttackMessage>(this, OnHandAttack);
            EventBetter.Listen<LocalGameManager, WeaponAttackMessage>(this, OnWeaponAttack);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnSearchGrabTarget(SearchGrabTargetMessage msg)
        {
            Collider[] hitColliders = Physics.OverlapSphere(msg.hc.transform.position, msg.radius);
            Transform target = null;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.attachedRigidbody != null &&
                    !hitCollider.attachedRigidbody.isKinematic &&
                    !hitCollider.transform.IsChildOf(msg.hc.oodlesCharacter.transform) && //it's me
                    (hitCollider.gameObject.tag == "CanBeGrabbed" || hitCollider.gameObject.layer == LayerMask.NameToLayer("Ragdoll")))
                    target = hitCollider.transform;
            }

            if (target != null)
            {
                msg.callback(target);
            }
        }

        private void OnCheckGrabable(CheckGrabableMessage msg)
        {
            bool res = false;

            if (msg.obj != null)
            {
                if (!(msg.obj.GetComponent<JointMatch>() != null ||
                msg.obj.GetComponent<WeaponHandler>() != null ||
                msg.obj.tag == "CanBeGrabbed"))
                {
                    res = false;
                }
            }

            res = true;

            msg.callback(res);
        }

        private void OnGrabObject(GrabObjectMessage msg)
        {
            if (msg.obj == null) return;

            WeaponHandler wh = msg.obj.GetComponent<WeaponHandler>();
            if (wh != null)
            {
                wh.SetOwner(msg.pc);

                //attach weapon
                // Physics.IgnoreCollision(wh.GetComponent<Collider>(), msg.hf.GetComponent<Collider>(), false);
                //wh.GetComponent<Collider>().isTrigger = true;
                //wh.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //wh.GetComponent<Rigidbody>().Sleep();
                //wh.transform.localRotation = Quaternion.Euler(45, 45, 45);
                //wh.GetComponent<Rigidbody>().isKinematic = false;
                //wh.GetComponent<Collider>().isTrigger = false;
            }
        }

        private void OnReleaseObject(ReleaseObjectMessage msg)
        {
            if (msg.obj == null) return;

            WeaponHandler wh = msg.obj.GetComponent<WeaponHandler>();
            if (wh != null)
            {
                wh.SetOwner(null);
            }
        }

        private void OnThrowObject(ThrowObjectMessage msg)
        {
            Rigidbody rb = msg.obj.GetComponent<Rigidbody>();
            if (rb == null) return;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            float twoHandsForce = 0;
            float singleHandForce = 0;

            if (msg.obj.GetComponent<Toy>() != null)
            {
                twoHandsForce = 6400;
                singleHandForce = 4000;
            }
            else if (msg.obj.GetComponent<WeaponHandler>() != null)
            {

            }
            else if (msg.obj.GetComponent<JointMatch>() != null)
            {
                twoHandsForce = 34000;
                singleHandForce = 20000;
            }

            if (msg.twoHands)
            {
                rb.AddForce(msg.dir * twoHandsForce);
            }
            else
            {
                rb.AddForce(msg.dir * singleHandForce);
            }
        }

        private void OnHandAttack(HandAttackMessage msg)
        {
            JointMatch animMagnet = msg.col.gameObject.GetComponent<JointMatch>();
            if (animMagnet != null)//hit someone
            {
                animMagnet.oodlesCharacter.KnockDown();
            }

            if (msg.col.rigidbody != null)
                msg.col.rigidbody.AddForce(-msg.col.contacts[0].normal * 10000);
        }

        private void OnWeaponAttack(WeaponAttackMessage msg)
        {
            Rigidbody rb = msg.obj.GetComponent<Rigidbody>();

            if (rb == null || rb.isKinematic) return;

            if (rb != null)
            {
                rb.AddForce(msg.dir * msg.wp.hitForce);
            }

            string player = OodlesPartyManager.Instance.setting.PlayerLayerName;
            if (msg.obj.layer == LayerMask.NameToLayer(player))
            {
                OodlesCharacter targetPC = msg.obj.GetComponentInParent<OodlesCharacter>();
                if (targetPC != null)
                {
                    targetPC.KnockDown();
                }
            }
        }
    }
}

