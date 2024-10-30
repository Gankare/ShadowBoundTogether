﻿namespace OodlesParty
{
    public interface INetworkedClient
    {
        INetworkedClientState LatestServerState { get; }
        uint CurrentTick { get; }
    }
}