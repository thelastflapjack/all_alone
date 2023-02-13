using Godot;
using GameGeneral.FSM;
using ZombieHoardGame.ZombieCharacter.States;

namespace ZombieHoardGame.ZombieCharacter.FSMController
{
    public class Controller : StateMachine
    {

       public Blackboard Blackboard{
            set {
                _blackboard = value;
                foreach (ZombieState state in GetChildren())
                {
                    state.Blackboard = _blackboard;
                }
            }
       }

        private Blackboard _blackboard;

        public override void _Ready()
        {
            base._Ready();
        }

        public void Die()
        {
            // CONSIDER: Death code is in a few places, might want to consolidate
            
            AnimationNodeStateMachinePlayback animStateMachine = _blackboard.AnimStateMachine;
            if (animStateMachine.GetCurrentNode() == "window_climb")
            {
                float length = _blackboard.AnimStateMachine.GetCurrentLength();
                float played = _blackboard.AnimStateMachine.GetCurrentPlayPosition();
                if (played < length * 0.5)
                {
                    _blackboard.AnimStateMachine.Travel("window_death_out");
                }
                else
                {
                    _blackboard.AnimStateMachine.Travel("window_death_in");
                }
            }
            else
            {
                _blackboard.AnimStateMachine.Travel("dead");
            }

            TransitionTo(_states["Dead"]);
        }

    }
}

