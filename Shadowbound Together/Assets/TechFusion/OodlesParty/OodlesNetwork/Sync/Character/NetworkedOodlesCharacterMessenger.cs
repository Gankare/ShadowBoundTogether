using System;
using Mirror;

namespace OodlesParty
{
    public class NetworkedOodlesCharacterMessenger : NetworkBehaviour, INetworkedClientMessenger<OodlesCharacterInput, OodlesCharacterState>
    {
        public event Action<OodlesCharacterInput> OnInputReceived;
        
        public OodlesCharacterState LatestServerState => _latestServerState;

        OodlesCharacterState _latestServerState;
        
        public void SendState(OodlesCharacterState state)
        {
            RpcSendState(state);
        }

        public void SendInput(OodlesCharacterInput input)
        {
            CmdSendInput(input);
        }

        [ClientRpc(channel = Channels.Unreliable)]
        void RpcSendState(OodlesCharacterState state)
        {
            _latestServerState = state;
        }
        
        [Command(channel = Channels.Unreliable)]
        void CmdSendInput(OodlesCharacterInput state)
        {
            OnInputReceived?.Invoke(state);
        }
    }
}