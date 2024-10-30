using UnityEngine;
using System.Collections.Generic;
using Mirror;

namespace OodlesParty
{
    public class NetworkedSingleTransform : NetworkedClient<SingleTransformInput, SingleTransformState>
    {
        [Header("Transform/References")]
        public Transform _transform = null;
        
        LinkedList<SingleTransformState> _observerBuffer = new LinkedList<SingleTransformState>();

        void Start()
        {
            if (_identity.isClient)
                AddState(new SingleTransformState(_transform.position, _transform.rotation, 0));
        }

        public override void SetState(SingleTransformState state)
        {
            AddState(state);

            _currentTick = state.LastProcessedInputTick;

            ProcessStateSmooth();
        }

        //no input
        public override void ProcessInput(SingleTransformInput input)
        {
        }

        protected override SingleTransformState RecordState(uint lastProcessedInputTick)
        {
            return new SingleTransformState(transform.position, transform.rotation, lastProcessedInputTick);
        }

        [Client]
        void AddState(SingleTransformState state)
        {
            if (_observerBuffer.Count > 0 && _observerBuffer.Last.Value.LastProcessedInputTick > state.LastProcessedInputTick)
            {
                Debug.Log("void AddState(SingleTransformState state) - tick index error!");
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

            if (_transform != null)
            {
                SingleTransformState state = toNode != null ?
                    SingleTransformState.Interpolate(fromNode.Value, toNode.Value, pastTick) : fromNode.Value;

                _transform.position = state.position;
                _transform.rotation = state.rotation;
            }
        }
    }
}