using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{
    public class EngineSetup : SingletonMono<EngineSetup>
    {
        public bool Networked { get { return networked; } }

        protected bool networked = false;

        override protected void Awake()
        {
            base.Awake();

            string player = OodlesSetting.Instance.PlayerLayerName;
            string ragdoll = OodlesSetting.Instance.RagdollLayerName;
            string ragdollHands = OodlesSetting.Instance.RagdollHandsLayerName;

            int layerPlayer = LayerMask.NameToLayer(player);
            int layerRagdoll = LayerMask.NameToLayer(ragdoll);
            int layerRagdollHands = LayerMask.NameToLayer(ragdollHands);

            Physics.IgnoreLayerCollision(layerPlayer, layerRagdoll);
            Physics.IgnoreLayerCollision(layerPlayer, layerRagdollHands);

            Physics.defaultSolverIterations = 40;
            Physics.defaultSolverVelocityIterations = 40;
        }
    }
}