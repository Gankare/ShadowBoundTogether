using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

namespace OodlesParty
{ 
    public class ServerGamePlay : NetworkBehaviour
    {
        public TMP_Text textUITeam1;
        public TMP_Text textUITeam2;
        
        public static ServerGamePlay _instance;

        private int score1 = 0, score2 = 0;
        private int team1PlayerCount = 0;
        private int team2PlayerCount = 0;

        struct GangResult
        {
            public int winner; //0:deuce 1:red 2:blue
        }
        
        public static ServerGamePlay GetInstance()
        {
            return _instance;
        }

        public void Awake()
        {
            _instance = this;

            EventBetter.Listen<ServerGamePlay, SearchGrabTargetMessage>(this, OnSearchGrabTarget);
            EventBetter.Listen<ServerGamePlay, CheckGrabableMessage>(this, OnCheckGrabable);
            EventBetter.Listen<ServerGamePlay, GrabObjectMessage>(this, OnGrabObject);
            EventBetter.Listen<ServerGamePlay, ReleaseObjectMessage>(this, OnReleaseObject);
            EventBetter.Listen<ServerGamePlay, ThrowObjectMessage>(this, OnThrowObject);
            EventBetter.Listen<ServerGamePlay, HandAttackMessage>(this, OnHandAttack);
            EventBetter.Listen<ServerGamePlay, WeaponAttackMessage>(this, OnWeaponAttack);
        }

        public enum GameState
        {
            //Prepare,
            Playing,
            GameOver
        }

        protected GameState gameState = GameState.Playing;

        // Update is called once per frame
        void Update()
        {
            OnGameUpdate();
        }

        void OnGameUpdate()
        {
            if (isServer) UpdateServerLogic();
        }

        void UpdateServerLogic()
        {
            if (gameState == GameState.GameOver) return;
        }

        public void AppointTeam(ref int team)
        {
            if (team1PlayerCount <= team2PlayerCount)
            {
                team = 0;
                team1PlayerCount++;
            }
            else
            {
                team = 1;
                team2PlayerCount++;
            }
        }

        public void OnGameMessage(string name, string arg1 = null, string arg2 = null, string arg3 = null, string arg4 = null)
        {
            if (!isServer) return;

            if (gameState == GameState.GameOver) return;

            if (name == "Collect")
            {
                OnScore(int.Parse(arg1), int.Parse(arg2));
            }
        }

        void OnScore(int team, int score)
        {
            if (team == 1)
            {
                score1 += score;
            }
            else if (team == 2)
            {
                score2 += score;
            }

            RpcUpdateScoreBoard(score1, score2);
        }

        [ClientRpc]
        void RpcUpdateScoreBoard(int score1, int score2)
        {
            this.score1 = score1;
            this.score2 = score2;

            UpdateUI();
        }

        void UpdateUI()
        {
            if (textUITeam1) textUITeam1.text = score1.ToString();
            if (textUITeam2) textUITeam2.text = score2.ToString();
        }

        private void OnSearchGrabTarget(SearchGrabTargetMessage msg)
        {
            Collider[] hitColliders = Physics.OverlapSphere(msg.hc.transform.position, msg.radius);
            Transform target = null;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.attachedRigidbody != null &&
                    !hitCollider.attachedRigidbody.isKinematic &&
                    !hitCollider.transform.IsChildOf(msg.hc.oodlesCharacter.transform) && //it's me
                    (hitCollider.gameObject.tag == "CanBeGrabbed" || hitCollider.gameObject.layer == LayerMask.NameToLayer("Ragdoll")))
                    target = hitCollider.transform;
            }

            if (target != null)
            {
                msg.callback(target);
            }
        }

        private void OnCheckGrabable(CheckGrabableMessage msg)
        {
            bool res = false;

            if (msg.obj != null)
            {
                if (!(msg.obj.GetComponent<JointMatch>() != null ||
                msg.obj.GetComponent<WeaponHandler>() != null ||
                msg.obj.tag == "CanBeGrabbed"))
                {
                    res = false;
                }
            }

            res = true;

            msg.callback(res);
        }

        private void OnGrabObject(GrabObjectMessage msg)
        {
            if (msg.obj == null) return;

            WeaponHandler wh = msg.obj.GetComponent<WeaponHandler>();
            if (wh != null)
            {
                wh.SetOwner(msg.pc);
            }
        }

        private void OnReleaseObject(ReleaseObjectMessage msg)
        {
            if (msg.obj == null) return;

            WeaponHandler wh = msg.obj.GetComponent<WeaponHandler>();
            if (wh != null)
            {
                wh.SetOwner(null);
            }
        }

        private void OnThrowObject(ThrowObjectMessage msg)
        {
            Rigidbody rb = msg.obj.GetComponent<Rigidbody>();
            if (rb == null) return;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            float twoHandsForce = 0;
            float singleHandForce = 0;

            if (msg.obj.GetComponent<Toy>() != null)
            {
                twoHandsForce = 6400;
                singleHandForce = 4000;
            }
            else if (msg.obj.GetComponent<WeaponHandler>() != null)
            {

            }
            else if (msg.obj.GetComponent<JointMatch>() != null)
            {
                twoHandsForce = 34000;
                singleHandForce = 20000;
            }

            if (msg.twoHands)
            {
                rb.AddForce(msg.dir * twoHandsForce);
            }
            else
            {
                rb.AddForce(msg.dir * singleHandForce);
            }
        }

        private void OnHandAttack(HandAttackMessage msg)
        {
            JointMatch jointMatch = msg.col.gameObject.GetComponent<JointMatch>();
            if (jointMatch != null)//hit someone
            {
                jointMatch.oodlesCharacter.KnockDown();
            }

            if (msg.col.rigidbody != null)
                msg.col.rigidbody.AddForce(-msg.col.contacts[0].normal * 10000);
        }

        private void OnWeaponAttack(WeaponAttackMessage msg)
        {
            Rigidbody rb = msg.obj.GetComponent<Rigidbody>();

            if (rb == null || rb.isKinematic) return;

            if (rb != null)
            {
                rb.AddForce(msg.dir * msg.wp.hitForce);
            }

            string player = OodlesPartyManager.Instance.setting.PlayerLayerName;
            if (msg.obj.layer == LayerMask.NameToLayer(player))
            {
                OodlesCharacter targetPC = msg.obj.GetComponentInParent<OodlesCharacter>();
                if (targetPC != null)
                {
                    targetPC.KnockDown();
                }
            }
        }
    }
}