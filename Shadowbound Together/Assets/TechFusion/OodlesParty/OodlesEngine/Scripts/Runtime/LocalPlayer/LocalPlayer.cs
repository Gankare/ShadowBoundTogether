using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{
    public class LocalPlayer : MonoBehaviour
    {
        OodlesCharacter characterController;
        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<OodlesCharacter>();
            CameraFollow.Get().player = characterController.GetPhysicsBody().transform;
            CameraFollow.Get().enable = true;
        }

        // Update is called once per frame
        void Update()
        {
            OodlesCharacterInput pci = new OodlesCharacterInput(
                    InputManager.Get().GetVertical(),//Input.GetAxisRaw("Vertical"),
                    InputManager.Get().GetHorizontal(),//Input.GetAxisRaw("Horizontal"),
                    InputManager.Get().GetJump(),//Input.GetAxisRaw("Jump"),
                    InputManager.Get().GetTouchMoveY(),//Input.GetAxisRaw("Mouse Y"),
                    InputManager.Get().GetLeftHandUse(),//Input.GetAxisRaw("Fire1"),
                    InputManager.Get().GetRightHandUse(),//Input.GetAxisRaw("Fire2"),
                    InputManager.Get().GetDoAction1(),
                    InputManager.Get().GetCameraLook(),//Camera.main.transform.forward,
                    Time.deltaTime, 0);

            characterController.ProcessInput(pci);
        }
    }
}
