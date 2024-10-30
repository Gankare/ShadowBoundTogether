using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public partial class OodlesCharacter : MonoBehaviour
    {
        DriveState PrevArmDrive;
        bool LeftArmOnUse = false, RightArmOnUse = false;

        public void JointMoveState()
        {
            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveLow);

            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveFix);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveFix);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveFix);

            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveMedium);

            //physicsBodyJoint.slerpDrive = DriveControl;
            DriveState.ApplyDrive(physicsBodyJoint, DriveFix);

            PrevArmDrive = DriveLow;
        }

        public void JointInAirState()
        {
            //SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveLow);
            //SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveLow);

            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveMedium);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveMedium);

            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveHigh);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveHigh);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveHigh);

            //physicsBodyJoint.slerpDrive = DriveControl;
            DriveState.ApplyDrive(physicsBodyJoint, DriveControl);

            PrevArmDrive = DriveLow;
        }

        public void JointDizzyState()
        {
            //SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveLow);
            //SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveOff);
        
            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveOff);

            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveControl);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveControl);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveControl);

            //physicsBodyJoint.slerpDrive = DriveControl;
            DriveState.ApplyDrive(physicsBodyJoint, DriveControl);

            PrevArmDrive = DriveLow;
        }

        public void JointIdleState()
        {
            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveHigh);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveHigh);//low
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveHigh);//low
            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveHigh);

            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveLow);

            DriveState.ApplyDrive(physicsBodyJoint, DriveControl);

            PrevArmDrive = DriveLow;
        }

        public void JointPickState()
        {
            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveHigh);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveHigh);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveLow);

            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveLow);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveLow);

            DriveState.ApplyDrive(physicsBodyJoint, DriveHigh);

            PrevArmDrive = DriveLow;
        }

        public void JointLoseBalanceState()
        {
            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveOff);
            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveOff);

            DriveState.ApplyDrive(physicsBodyJoint, DriveOff);

            //not reasonable
            PrevArmDrive = DriveOff;
        }

        public void JointActionState()
        {
            SetJointDriveOnGroup(BodyPartGroup.BPG_Head, DriveControl);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveControl);
            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveControl);

            SetJointDriveOnGroup(BodyPartGroup.BPG_Spine, DriveControl);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegLeft, DriveControl);
            SetJointDriveOnGroup(BodyPartGroup.BPG_LegRight, DriveControl);

            SetJointDriveOnGroup(BodyPartGroup.BPG_Pelvis, DriveControl);

            DriveState.ApplyDrive(physicsBodyJoint, DriveControl);
        }

        public void JointUseLeftArm()
        {
            if (LeftArmOnUse) return;

            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, DriveControl);
        }

        public void JointUnUseLeftArm()
        {
            if (!LeftArmOnUse) return;

            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmLeft, PrevArmDrive);
        }

        public void JointUseRightArm()
        {
            if (RightArmOnUse) return;

            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, DriveControl);
        }

        public void JointUnUseRightArm()
        {
            if (!RightArmOnUse) return;

            SetJointDriveOnGroup(BodyPartGroup.BPG_ArmRight, PrevArmDrive);
        }
    }
}