using Mirror;
using UnityEngine;

namespace OodlesParty
{
    public abstract class ClientPrediction<TClientInput, TClientState> : MonoBehaviour 
        where TClientInput : IInput
        where TClientState : INetworkedClientState
    {
        [Header("Prediction/References")]
        [SerializeField, HideInInspector] public NetworkIdentity _identity = null;
        [Header("Prediction/Settings")] 
        [SerializeField, Tooltip("The number of ticks to be stored in the input and state buffers")]
        uint _bufferSize = 1024;
        //[SerializeField] int _sendInputInterval = 2;
        float _sendDelta = 0;

        NetworkedClient<TClientInput, TClientState> _client = null;
        TClientInput[] _inputBuffer;
        TClientState _lastProcessedState = default;
        
        protected virtual void Awake()
        {
            _client = GetComponent<NetworkedClient<TClientInput, TClientState>>();
            
            if(_client == null)
                Debug.LogError($"Couldn't find client for {name}");

            _inputBuffer = new TClientInput[_bufferSize];
        }
 
        public void HandleTick(float deltaTime, uint currentTick, TClientState latestServerState)
        {
            if(!_identity.isServer && (latestServerState != null && (_lastProcessedState == null || !_lastProcessedState.Equals(latestServerState))))
                UpdatePrediction(currentTick, latestServerState);

            _sendDelta += deltaTime;
            
            if (_sendDelta >= 0.02f)
            {
                var __input = GetInput(_sendDelta, currentTick);

                _sendDelta = 0;

                var __bufferIndex = currentTick % _bufferSize;

                _inputBuffer[__bufferIndex] = __input;

                _client.SendClientInput(__input);
            }
        }

        protected abstract TClientInput GetInput(float deltaTime, uint currentTick);
        
        void UpdatePrediction(uint currentTick, TClientState latestServerState)
        {
            _lastProcessedState = latestServerState;

            //Debug.Log("latestServerState and client current : " + _lastProcessedState.LastProcessedInputTick.ToString() + "|" + currentTick.ToString());
            
            _client.SetState(_lastProcessedState);
        }
    }
}