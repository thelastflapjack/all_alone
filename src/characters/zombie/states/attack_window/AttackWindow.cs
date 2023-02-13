using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class AttackWindow : ZombieState
    {
        private Timer _timerAttack;

        public override void _Ready()
        {
            _timerAttack = GetNode<Timer>("AttackTimer");
        }
        
        public override void Enter()
        {
            base.Enter();
            _blackboard.AnimStateMachine.Travel("idle");
            _blackboard.Character.GlobalTransform = _blackboard.TargetBoardedWindow.ZombieAttackPointTransform;
            _timerAttack.Start();
            _timerAttack.Connect("timeout", this, nameof(OnAttackTimerTimeout));
        }

        public override void Exit()
        {
            base.Exit();
            _timerAttack.Stop();
            _timerAttack.Disconnect("timeout", this, nameof(OnAttackTimerTimeout));
        }

        private void OnAttackTimerTimeout()
        {
            BoardedWindow window = _blackboard.TargetBoardedWindow;
            _blackboard.AnimStateMachine.Travel("attack_window");
            if (window.IsClear)
            {
                _timerAttack.Stop();
                EmitSignal(nameof(ChangeStateRequest), "ClimbThroughWindow");
            }
            else
            {
                _blackboard.AnimStateMachine.Travel("attack_window");
            }
        }
    }
}

