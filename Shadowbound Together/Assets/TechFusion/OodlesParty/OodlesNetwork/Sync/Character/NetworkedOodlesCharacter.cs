using UnityEngine;
using System.Collections.Generic;
using Mirror;

namespace OodlesParty
{
    public class NetworkedOodlesCharacter : NetworkedClient<OodlesCharacterInput, OodlesCharacterState>
    {
        [Header("OodlesCharacter/References")]
        [SerializeField] public OodlesCharacter oodlesCharacter = null;

        LinkedList<OodlesCharacterState> _observerBuffer = new LinkedList<OodlesCharacterState>();

        void Start()
        {
            if (_identity.isClient)
                AddState(new OodlesCharacterState(oodlesCharacter.GetPhysicsBody(), oodlesCharacter.GetJoints(), 0));
        }

        [Client]
        public override void SetState(OodlesCharacterState state)
        {
            if (state.positions == null) return;

            AddState(state);

            _currentTick = state.LastProcessedInputTick;

            ProcessStateSmooth();
        }

        public override void ProcessInput(OodlesCharacterInput input)
        {
            if (oodlesCharacter != null)
            {
                oodlesCharacter.ProcessInput(input);
            }
        }

        protected override OodlesCharacterState RecordState(uint lastProcessedInputTick)
        {
            if (oodlesCharacter.GetPhysicsBody() == null)
            {
                Debug.Log("oodlesCharacter.GetPhysicsBody() is null!");
                return new OodlesCharacterState();
            }

            return new OodlesCharacterState(oodlesCharacter.GetPhysicsBody(), oodlesCharacter.GetJoints(),lastProcessedInputTick);
        }

        [Client]
        void AddState(OodlesCharacterState state)
        {
            if (_observerBuffer.Count > 0 && _observerBuffer.Last.Value.LastProcessedInputTick > state.LastProcessedInputTick)
            {
                Debug.Log("void AddState(OodlesCharacterState state) - tick index error!");
                return;
            }
            _observerBuffer.AddLast(state);
        }

        public void ProcessStateSmooth()
        {
            //server return
            if (_identity.isServer) return;

            int pastTick = (int)_currentTick - OodlesPartyNetworkManager.Instance.interpolationDelay;
            var fromNode = _observerBuffer.First;
            var toNode = fromNode.Next;
            while (toNode != null && toNode.Value.LastProcessedInputTick <= pastTick)
            {
                fromNode = toNode;
                toNode = fromNode.Next;
                _observerBuffer.RemoveFirst();
            }

            if (oodlesCharacter != null)
            {
                SetCharacterState(toNode != null ?
                    OodlesCharacterState.Interpolate(fromNode.Value, toNode.Value, pastTick) : fromNode.Value);
            }
        }

        void SetCharacterState(OodlesCharacterState state)
        {
            if (state.positions == null) return;

            //Use pos and rot on root instead of localpos and localrot
            oodlesCharacter.GetPhysicsBody().transform.position = state.positions[0];
            oodlesCharacter.GetPhysicsBody().transform.rotation = state.rotations[0];

            for (int i = 0; i < oodlesCharacter.joints.Length; i++)
            {
                if (oodlesCharacter.joints[i])
                {
                    oodlesCharacter.joints[i].transform.localPosition = state.positions[i + 1];
                    oodlesCharacter.joints[i].transform.localRotation = state.rotations[i + 1];
                }
            }
        }
    }
}