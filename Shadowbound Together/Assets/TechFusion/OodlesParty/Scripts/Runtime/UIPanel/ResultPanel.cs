using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OodlesParty
{
    public class ResultPanel : UIPanel

    {
        private static ResultPanel _instance;
        public Image Winimg;
        public Image Loseimg;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _instance = this;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

        }

        public void SetWin(bool bwin)
        {
            if(bwin)
            {
                Winimg.enabled = true;
                Loseimg.enabled = false;

            }
            else
            {
                Winimg.enabled = false;
                Loseimg.enabled = true;

            }
        }

        public static ResultPanel Get()
        {
            return _instance;
        }

        public void ExitGame()
        {
            //OodlesPartyNetworkManager t = GameObject.Find("NetworkManager").GetComponent<OodlesPartyNetworkManager>();
            //t.StopClient();
        }
    }
}
