using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections;

namespace OodlesParty
{ 
    public class OodlesPartyNetworkManager : NetworkManager
    {
        public static OodlesPartyNetworkManager Instance = null;

        [SerializeField, Range(1, 60), Tooltip("In steps per second")]
        public int interpolationDelay = 12;

        public override void Awake()
        {
            base.Awake();

            if (Instance != null)
            {
                Debug.LogError("OodlesPartyNetworkManager should only one!");
            }

            Instance = this;

            Physics.autoSimulation = false;
        }

        private void FixedUpdate()
        {
            if (isNetworkActive)
            {
                TickManager.Tick();

                if (mode == NetworkManagerMode.Host ||
                    mode == NetworkManagerMode.ServerOnly)
                    SimulatePhysics(Time.fixedDeltaTime);
            }
        }

        [Server]
        void SimulatePhysics(float delta)
        {
            Physics.Simulate(delta);
        }
    }
}