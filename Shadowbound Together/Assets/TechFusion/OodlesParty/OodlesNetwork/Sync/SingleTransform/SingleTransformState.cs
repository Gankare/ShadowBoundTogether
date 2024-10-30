using System;
using UnityEngine;

namespace OodlesParty
{
    public struct SingleTransformState : IEquatable<SingleTransformState>, INetworkedClientState
    {
        public uint LastProcessedInputTick => lastProcessedInputTick;
        
        public Vector3 position;
        public Quaternion rotation;
        public uint lastProcessedInputTick;

        public SingleTransformState(Vector3 position, Quaternion rotation, uint lastProcessedInputTick)
        {
            this.position = position;
            this.rotation = rotation;
            this.lastProcessedInputTick = lastProcessedInputTick;
        }

        public bool Equals(SingleTransformState other)
        {
            return position.Equals(other.position) && rotation.Equals(other.rotation);
        }

        public bool Equals(INetworkedClientState other)
        {
            return other is SingleTransformState __other && Equals(__other);
        }

        public override string ToString()
        {
            return
                $"Pos: {position.ToString()} | Rot: {rotation.ToString()} | LasProcessInput: {lastProcessedInputTick.ToString()}";
        }

        public static SingleTransformState Interpolate(SingleTransformState from, SingleTransformState to, int clientTick)
        {
            float t = ((float)(clientTick - from.LastProcessedInputTick)) / (to.LastProcessedInputTick - from.LastProcessedInputTick);
            SingleTransformState res = new SingleTransformState();
            
            res.lastProcessedInputTick = 0;

            res.position = Vector3.Lerp(from.position, to.position, t);
            res.rotation = Quaternion.Slerp(from.rotation, to.rotation, t);

            return res;
        }
    }
}