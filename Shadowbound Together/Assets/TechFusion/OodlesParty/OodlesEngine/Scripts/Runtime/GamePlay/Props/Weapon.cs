using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public class Weapon : Prop
    {
        [HideInInspector]
        public OodlesCharacter owner;

        public float hitForce = 15000;

        void OnCollisionEnter(Collision col)
        {
            if (owner == null) return;

            //not me
            if (col.gameObject.transform.IsChildOf(owner.transform))
            {
                return;
            }

            //test event
            //if (ServerGamePlay.GetInstance() != null)
            //{
            //    if (owner.isAttacking)
            //    {
            //        ServerGamePlay.GetInstance().OnWeaponAttackSomething(owner, this, -col.GetContact(0).normal, col.gameObject);
            //    }
            //}

            if (owner.isAttacking)
            {
                EventBetter.Raise(new WeaponAttackMessage()
                {
                    pc = owner,
                    wp = this,
                    dir = -col.GetContact(0).normal,
                    obj = col.gameObject,
                });
            }
        }
    }
}