using System;
using UnityEngine;

namespace OodlesParty
{
    public struct OodlesCharacterState : IEquatable<OodlesCharacterState>, INetworkedClientState
    {
        public uint LastProcessedInputTick => lastProcessedInputTick;
        
        public uint lastProcessedInputTick;
        
        public Vector3[] positions;
        public Quaternion[] rotations;

        public OodlesCharacterState(Rigidbody physicsBody, ConfigurableJoint[] joints, uint lastProcessedInputTick)
        {
            this.lastProcessedInputTick = lastProcessedInputTick;

            this.positions = new Vector3[joints.Length + 1];
            this.rotations = new Quaternion[joints.Length + 1];

            //note! use pos and rot on root, not localpos and localrot
            this.positions[0] = physicsBody.transform.position;
            this.rotations[0] = physicsBody.transform.rotation;

            for (int i = 0; i < joints.Length; i++)
            {
                this.positions[i + 1] = Vector3.zero;
                this.rotations[i + 1] = Quaternion.identity;

                if (joints[i] != null)
                {
                    this.positions[i + 1] = joints[i].transform.localPosition;
                    this.rotations[i + 1] = joints[i].transform.localRotation;
                }
            }
            
        }

        public bool Equals(OodlesCharacterState other)
        {
            return false;
        }

        public bool Equals(INetworkedClientState other)
        {
            return other is OodlesCharacterState __other && Equals(__other);
        }

        public static OodlesCharacterState Interpolate(OodlesCharacterState from, OodlesCharacterState to, int clientTick)
        {
            float t = ((float)(clientTick - from.LastProcessedInputTick)) / (to.LastProcessedInputTick - from.LastProcessedInputTick);
            OodlesCharacterState res = new OodlesCharacterState();
            
            res.lastProcessedInputTick = 0;

            int length = from.positions.Length;

            res.positions = new Vector3[length];
            res.rotations = new Quaternion[length];
            
            for (int i = 0; i < length; i++)
            {
                res.positions[i] = Vector3.Lerp(from.positions[i], to.positions[i], t);
                res.rotations[i] = Quaternion.Slerp(from.rotations[i], to.rotations[i], t);
            }

            return res;
        }
    }
}