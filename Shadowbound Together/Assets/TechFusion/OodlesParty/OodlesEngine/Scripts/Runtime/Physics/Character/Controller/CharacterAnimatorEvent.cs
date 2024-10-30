using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public class CharacterAnimatorEvent : MonoBehaviour
    {
        public OodlesCharacter controller;

        void HitB()
        {
            controller.isAttacking = true;
        }

        void HitE()
        {
            controller.isAttacking = false;
        }
    }
}