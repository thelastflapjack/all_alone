using Godot;
using ZombieHoardGame.PlayerCharacter;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class Chase : MoveState
    {
        private Timer _navUpdateTimer;

        public override void _Ready()
        {
            base._Ready();
            _navUpdateTimer = GetNode<Timer>("TimerNavigationUpdate");
            _navUpdateTimer.Connect("timeout", this, nameof(OnNavUpdateTimerTimeout));
        }

        public override void Enter()
        {
            base.Enter();
            _blackboard.Character.EmitSignal(nameof(ZombieCharacter.Zombie.PlayerPositionUpdateRequest), _blackboard.Character);
            _navUpdateTimer.Start();
            
            if(_blackboard.AttackTrigger.IsPlayerInside)
            {
                EmitSignal(nameof(ChangeStateRequest), "AttackPlayer");
            }
            else
            {
                _blackboard.AttackTrigger.Connect(nameof(AttackTrigger.PlayerEntered), this, nameof(OnAttackTriggerPlayerEntered));
                _blackboard.AnimStateMachine.Travel("walk");
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (_blackboard.AttackTrigger.IsConnected(nameof(AttackTrigger.PlayerEntered), this, nameof(OnAttackTriggerPlayerEntered)))
            {
                _blackboard.AttackTrigger.Disconnect(nameof(AttackTrigger.PlayerEntered), this, nameof(OnAttackTriggerPlayerEntered));
            }
            _navUpdateTimer.Stop();
        }

        public override void PhysicsUpdate(float delta)
        {
            MoveTowardsPlayer(delta);
        }


        private void OnAttackTriggerPlayerEntered(Player player)
        {
            EmitSignal(nameof(ChangeStateRequest), "AttackPlayer");
        }

        private void OnNavUpdateTimerTimeout()
        {
            _blackboard.Character.EmitSignal(nameof(ZombieCharacter.Zombie.PlayerPositionUpdateRequest), _blackboard.Character);
            int LOD = _blackboard.LODSwitcher.CurrentLOD;
            if (LOD == 1)
            {
                _navUpdateTimer.WaitTime = 0.05f;
            }
            else if (LOD == 2)
            {
                _navUpdateTimer.WaitTime = 0.1f;
            }
            else if (LOD == 3)
            {
                _navUpdateTimer.WaitTime = 0.5f;
            }
        }


        private void MoveTowardsPlayer(float delta)
        {
            NavTarget = _blackboard.PlayerPosition;
            MoveTowardsNavTarget(delta);
        }
    }
}

