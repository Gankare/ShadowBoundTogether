using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public partial class OodlesCharacter : MonoBehaviour
    {
        public struct DriveState
        {
            public float positionSpring;
            public float positionDamper;

            static public void ApplyDrive(ConfigurableJoint joint, DriveState state)
            {
                JointDrive jDrivex = joint.angularXDrive;
                JointDrive jDriveyz = joint.angularYZDrive;
                jDrivex.positionSpring = state.positionSpring;
                jDriveyz.positionSpring = state.positionSpring;
                jDrivex.positionDamper = state.positionDamper;
                jDriveyz.positionDamper = state.positionDamper;
                joint.angularXDrive = jDrivex;
                joint.angularYZDrive = jDriveyz;
            }
        }

        DriveState DriveOff, DriveLow, DriveMedium, DriveHigh, DriveControl, DriveFix;

        void InitJointDrives()
        {
            DriveOff = new DriveState();
            DriveOff.positionSpring = 2;
            DriveOff.positionDamper = 0;

            DriveLow = new DriveState();
            DriveLow.positionSpring = 100;
            DriveLow.positionDamper = 1;

            DriveMedium = new DriveState();
            DriveMedium.positionSpring = 200;
            DriveMedium.positionDamper = 2;

            DriveHigh = new DriveState();
            DriveHigh.positionSpring = 400;
            DriveHigh.positionDamper = 2;

            DriveControl = new DriveState();
            DriveControl.positionSpring = 1000;
            DriveControl.positionDamper = 1;

            DriveFix = new DriveState();
            DriveFix.positionSpring = 10000;
            DriveFix.positionDamper = 3;

            ////test
            //DriveLow.positionSpring = 8000;
            //DriveLow.positionDamper = 0;
            //DriveMedium.positionSpring = 8000;
            //DriveMedium.positionDamper = 0;
            //DriveHigh.positionSpring = 8000;
            //DriveHigh.positionDamper = 0;
            //DriveControl.positionSpring = 8000;
            //DriveControl.positionDamper = 0;
        }
    }
}