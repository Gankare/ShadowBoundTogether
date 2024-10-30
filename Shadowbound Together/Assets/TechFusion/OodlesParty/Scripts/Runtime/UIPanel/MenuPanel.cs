using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace OodlesParty
{
    public class MenuPanel : MonoBehaviour
    {
        public TMP_InputField ipInput;
        //Button clientModeButton;
        //Button hostModeButton;
        //Button serverModeButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClientModeClick()
        {
            if (OodlesPartyNetworkManager.Instance)
            {
                if (ipInput && ipInput.text != "")
                {
                    BeforeEnterGame();
                    OodlesPartyNetworkManager.Instance.networkAddress = ipInput.text;
                    OodlesPartyNetworkManager.Instance.StartClient();
                }
            }
            else
            {
                Debug.Log("NetworkManager does not exist!");
            }
        }

        public void OnHostModeClick()
        {
            if (OodlesPartyNetworkManager.Instance)
            {
                BeforeEnterGame();
                OodlesPartyNetworkManager.Instance.StartHost();
            }
            else
            {
                Debug.Log("NetworkManager does not exist!");
            }
        }

        public void OnServerModeClick()
        {
            if (OodlesPartyNetworkManager.Instance)
            {
                BeforeEnterGame();
                OodlesPartyNetworkManager.Instance.StartServer();
            }
            else
            {
                Debug.Log("NetworkManager does not exist!");
            }
        }

        void BeforeEnterGame()
        {
            gameObject.SetActive(false);

            if (CameraFollow.Get())
                CameraFollow.Get().enable = true;

            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
