using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace OodlesParty
{ 
    public class NetworkPlayer : NetworkBehaviour
    {
        public override void OnStartLocalPlayer()
        {
            OodlesCharacter controller = GetComponent<OodlesCharacter>();
            CameraFollow.Get().player = controller.GetPhysicsBody().transform;
        }
    }
}