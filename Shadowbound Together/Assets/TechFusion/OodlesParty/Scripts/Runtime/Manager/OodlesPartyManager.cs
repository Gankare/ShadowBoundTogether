using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{
    public class OodlesPartyManager : MonoBehaviour
    {
        static public OodlesPartyManager Instance = null;

        public GlobalSetting setting;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
