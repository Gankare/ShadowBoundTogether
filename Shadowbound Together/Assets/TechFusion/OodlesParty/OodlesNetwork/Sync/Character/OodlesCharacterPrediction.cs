using UnityEngine;

namespace OodlesParty
{
    public class OodlesCharacterPrediction : ClientPrediction<OodlesCharacterInput, OodlesCharacterState>
    {
        protected override OodlesCharacterInput GetInput(float deltaTime, uint currentTick)
        {
            return new OodlesCharacterInput(
                InputManager.Get().GetVertical(),//Input.GetAxisRaw("Vertical"),
                InputManager.Get().GetHorizontal(),//Input.GetAxisRaw("Horizontal"),
                InputManager.Get().GetJump(),//Input.GetAxisRaw("Jump"),
                InputManager.Get().GetTouchMoveY(),//Input.GetAxisRaw("Mouse Y"),
                InputManager.Get().GetLeftHandUse(),//Input.GetAxisRaw("Fire1"),
                InputManager.Get().GetRightHandUse(),//Input.GetAxisRaw("Fire2"),
                InputManager.Get().GetDoAction1(),
                InputManager.Get().GetCameraLook(),//Camera.main.transform.forward,
                deltaTime, currentTick);
        }
    }
}