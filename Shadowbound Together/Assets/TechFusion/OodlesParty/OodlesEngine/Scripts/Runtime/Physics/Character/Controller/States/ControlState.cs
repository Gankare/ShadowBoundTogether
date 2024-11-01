﻿using UnityEngine;

namespace OodlesParty
{ 
    public class ControlState : State<OodlesCharacter>
    {
        public override void EnterState(OodlesCharacter _owner)
        {
            _owner.JointIdleState();
        }

        public override void ExitState(OodlesCharacter _owner)
        {
            //Debug.Log("Exiting ControlState State");
        }

        public override void UpdateState(OodlesCharacter _owner)
        {
            _owner.UpdatePickUp();
            _owner.UpdateThrow();
            _owner.UpdateAttack();

            _owner.UpdateMovement();
            _owner.UpdateAnimations();
            _owner.SyncAnimator();
            _owner.UpdateIK();
            //_owner.UpdateAnimator();
            _owner.UpdateHandFunction();

            _owner.UpdateJointState();
        }
    }
}