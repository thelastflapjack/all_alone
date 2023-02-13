using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameGeneral.FSM
{
    public class State : Node
    {
        [Signal]
        public delegate void ChangeStateRequest(String newState);

        public virtual void Enter(){}

        public virtual void Exit(){}

        public virtual void Update(float delta){}

        public virtual void PhysicsUpdate(float delta){}
    }

    public class StateMachine : Node
    {
        [Export]
        private NodePath _initialStateNodePath = null;

        public State CurrentState
        {
            get { return _currentState; }
        }

        protected Dictionary<String, State> _states = new Dictionary<string, State>();
        protected State _currentState;


        public override void _Ready()
        {
            Setup();
        }
        
        public void Update(float delta)
        {
            _currentState.Update(delta);
        }

        public void PhysicsUpdate(float delta)
        {
            _currentState.PhysicsUpdate(delta);
        }

        protected void TransitionTo(State targetState)
        {
            if (_currentState != null)
            {
                _currentState.Disconnect(nameof(State.ChangeStateRequest), this, nameof(OnChangeStateRequest));
                _currentState.Exit();
            }

            _currentState = targetState;
            _currentState.Connect(nameof(State.ChangeStateRequest), this, nameof(OnChangeStateRequest));
            _currentState.Enter();
        }

        private async void Setup()
        {
            await ToSignal(Owner, "ready");
            FindStates();
            State initialState = GetNode<State>(_initialStateNodePath);
            TransitionTo(_states[initialState.Name]);
        }

        private void OnChangeStateRequest(String newStateName)
        {
            Debug.Assert(_states.ContainsKey(newStateName), $"StateMachine does not contain state with key {newStateName}");
            State targetState = _states[newStateName];
            TransitionTo(targetState);
        }

        private void FindStates()
        {
            foreach (Node child in GetChildren())
            {
                Debug.Assert(child is State, "All child nodes of a state machine must be of type State");
                State newState = (State)child;
                _states.Add(newState.Name, newState);
            }
        }
    }
}


