using System;
using Mirror;

namespace OodlesParty
{
    public class NetworkedSingleTransformMessenger : NetworkBehaviour, INetworkedClientMessenger<SingleTransformInput, SingleTransformState>
    {
        public event Action<SingleTransformInput> OnInputReceived;
        
        public SingleTransformState LatestServerState => _latestServerState;

        SingleTransformState _latestServerState;
        
        public void SendState(SingleTransformState state)
        {
            RpcSendState(state);
        }

        //no input
        public void SendInput(SingleTransformInput input)
        {
            //CmdSendInput(input);
            OnInputReceived?.Invoke(input);
        }

        [ClientRpc(channel = Channels.Unreliable)]
        void RpcSendState(SingleTransformState state)
        {
            _latestServerState = state;
        }
        
    }
}