using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public class WeaponHandler : MonoBehaviour
    {
        public Weapon wepon;

        public OodlesCharacter GetOwner()
        {
            if (wepon != null)
            {
                return wepon.owner;
            }

            return null;
        }
    
        public void SetOwner(OodlesCharacter pc)
        {
            if (wepon != null)
            {
                wepon.owner = pc;
            }
        }
    }
}