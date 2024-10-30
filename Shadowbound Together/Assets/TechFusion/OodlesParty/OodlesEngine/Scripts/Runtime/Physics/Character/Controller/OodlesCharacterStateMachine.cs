using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{ 
    public partial class OodlesCharacter : MonoBehaviour
    {
        public enum State
        {
            Control,
            LostControl,
        }

        public StateMachine<OodlesCharacter> stateMachine { get; set; }

        private Dictionary<State, State<OodlesCharacter>> states = new Dictionary<State, State<OodlesCharacter>>();

        public State<OodlesCharacter> GetState(State state)
        {
            return states[state];
        }

        public void ChangeState(State state)
        {
            stateMachine.ChangeState(states[state]);
        }

        private void InitStateMachine()
        {
            states.Add(State.Control, new ControlState());
            states.Add(State.LostControl, new LostControlState());
            stateMachine = new StateMachine<OodlesCharacter>(this);

            ChangeState(State.Control);
        }

        private void TickState()
        {
            stateMachine.Update();
        }
    }
}