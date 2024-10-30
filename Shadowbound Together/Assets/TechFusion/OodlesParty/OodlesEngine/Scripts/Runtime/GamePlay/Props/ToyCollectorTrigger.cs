using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public class ToyCollectorTrigger : Prop
    {
        public int teamID;
    
        void OnTriggerEnter(Collider other)
        {
            Toy toy = other.gameObject.GetComponent<Toy>();
            if (toy != null && !toy.IsUsed())
            {
                toy.MarkUsed();
                ServerGamePlay.GetInstance().OnGameMessage("Collect", teamID.ToString(), toy.GetScore().ToString());
            }
        }
    }
}