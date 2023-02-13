using Godot;


namespace ZombieHoardGame.PlayerCharacter.States
{
    public class Sprinting : MoveState
    {
        private AudioStreamPlayer _audioBreathing;
        private Timer _maxDurationTimer;
        private Timer _cooldownTimer;
        private float _cooldownTimeMax = 0;


        public override void _Ready()
        {
            base._Ready();
            _audioBreathing = GetNode<AudioStreamPlayer>("AudioBreathing");
            _maxDurationTimer = GetNode<Timer>("MaxDurationTimer");
            _cooldownTimer = GetNode<Timer>("CooldownTimer");
            _cooldownTimeMax = _cooldownTimer.WaitTime;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            if (!_cooldownTimer.IsStopped())
            {
                // Reduce volume of breathing effect
                float effectStrength = _cooldownTimer.TimeLeft / _cooldownTimeMax;
                SetBreathingAudioVolume(effectStrength);
            }
            else if (!_maxDurationTimer.IsStopped())
            {
                // Increase volume of breathing effect
                float effectStrength = 1 - (_maxDurationTimer.TimeLeft / _maxDurationTimer.WaitTime);
                SetBreathingAudioVolume(effectStrength);
            }
        }


        public override void Enter()
        {
            base.Enter();
            if (_cooldownTimer.IsStopped())  // TODO: Work cooldown feature into base state machine
            {
                _maxDurationTimer.Start();
                _maxDurationTimer.Connect("timeout", this, nameof(OnMaxDurationTimerTimeout));
            }
            else
            {
                EmitSignal(nameof(ChangeStateRequest), "Walking");
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (_maxDurationTimer.IsConnected("timeout", this, nameof(OnMaxDurationTimerTimeout)))
            {
                float cooldownProportion = (_maxDurationTimer.WaitTime - _maxDurationTimer.TimeLeft) / _maxDurationTimer.WaitTime;
                _cooldownTimer.Start(_cooldownTimeMax * cooldownProportion);
                _maxDurationTimer.Stop();
                _maxDurationTimer.Disconnect("timeout", this, nameof(OnMaxDurationTimerTimeout));
            }
        }

        private void OnMaxDurationTimerTimeout()
        {
            EmitSignal(nameof(ChangeStateRequest), "Walking");
        }

        private void SetBreathingAudioVolume(float volLinear)
        {
            _audioBreathing.VolumeDb = GD.Linear2Db(volLinear);
            if (volLinear == 0)
            {
                _audioBreathing.Stop();
            }
            else if (!_audioBreathing.Playing)
            {
                _audioBreathing.Play();
            }
        }

    }
}

