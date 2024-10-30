using UnityEngine;
using System.Collections;
using Mirror;

namespace OodlesParty
{
    public class CharacterProperty : NetworkBehaviour
    {
        public event System.Action<int> OnPlayerTeamChanged;
        
        [HideInInspector] public int score;
        [HideInInspector] public string username;
        [HideInInspector] public string userid;

        [SyncVar(hook = nameof(PlayerTeamChanged))]
        public int playerteam = 0;


        void PlayerTeamChanged(int _, int newPlayerTeam)
        {
            OnPlayerTeamChanged?.Invoke(newPlayerTeam);

            OodlesCharacter pc = GetComponent<OodlesCharacter>();
            if (pc != null)
            {
                pc.ChangeSkin(newPlayerTeam);
    }
        }

        [Server]
        public override void OnStartServer()
        {
            base.OnStartServer();

            if (ServerGamePlay.GetInstance())
            {
                int team = 0;
                ServerGamePlay.GetInstance().AppointTeam(ref team);
                playerteam = team;
            }

            //RpcPlayerTeamChanged(playerteam);

            if (isOwned)
            {
                OodlesCharacter pc = GetComponent<OodlesCharacter>();
                if (pc != null)
                {
                    pc.ChangeSkin(playerteam);
                }
            }
        }

        [ClientRpc]
        void RpcPlayerTeamChanged(int newPlayerTeam)
        {
            OodlesCharacter pc = GetComponent<OodlesCharacter>();
            if (pc != null)
            {
                pc.ChangeSkin(playerteam);
            }
        }

        public override void OnStartClient()
        {
            OodlesCharacter pc = GetComponent<OodlesCharacter>();
            if (pc != null)
            {
                pc.ChangeSkin(playerteam);
            }
        }
    }
}