using Godot;
using System;
using GameGeneral.FSM;

namespace ZombieHoardGame.ZombieCharacter.States
{
    public class ZombieState : State
    {
        public Blackboard Blackboard{
            set { _blackboard = value; }
        }

        protected Blackboard _blackboard;
        private Timer _timerGroanInterval;
        private float _groanIntervalChanceGain = 0.2f;
        private float _groanChance = 0f;

        public override void _Ready()
        {
            base._Ready();
            _timerGroanInterval = GetNodeOrNull<Timer>("TimerGroanInterval");
            if (_timerGroanInterval != null)
            {
                _timerGroanInterval.Connect("timeout", this, nameof(OnTimerGroanIntervalTimeout));
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (_timerGroanInterval != null)
            {
                _timerGroanInterval.Start();
            }
        }

        public override void Exit()
        {
            if (_timerGroanInterval != null)
            {
                _timerGroanInterval.Stop();
            }
        }

        private void OnTimerGroanIntervalTimeout()
        {
            _groanChance += _groanIntervalChanceGain;
            Random r = new Random();
            if (r.NextDouble() <= _groanChance)
            {
                _blackboard.Character.PlayRandomAudioGroan();
                _timerGroanInterval.Start();
                _groanChance = 0;
            }
        }
    }
}

