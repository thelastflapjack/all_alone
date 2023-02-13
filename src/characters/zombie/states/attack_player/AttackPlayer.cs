using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class AttackPlayer : ZombieState
    {
        private Timer _timerAttack;

        public override void _Ready()
        {
            _timerAttack = GetNode<Timer>("AttackTimer");
        }


        public override void Enter()
        {
            base.Enter();
            Attack();
            _timerAttack.Start();
            _timerAttack.Connect("timeout", this, nameof(OnAttackTimerTimeout));
        }

        public override void Exit()
        {
            base.Exit();
            _timerAttack.Stop();
            _timerAttack.Disconnect("timeout", this, nameof(OnAttackTimerTimeout));
        }

        private void Attack()
        {
            _blackboard.Character.PlayRandomAudioAttack();
            _blackboard.AnimStateMachine.Travel("attack_player");
            _blackboard.AttackHitBox.Activate();
        }

        private void OnAttackTimerTimeout()
        {
            if (_blackboard.AttackTrigger.IsPlayerInside)
            {
                Attack();
            }
            else
            {
                EmitSignal(nameof(ChangeStateRequest), "Chase");
            }
        }
    }
}

