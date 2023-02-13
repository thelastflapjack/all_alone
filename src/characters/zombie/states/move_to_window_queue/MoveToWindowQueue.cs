using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class MoveToWindowQueue : MoveState
    {

        public override void PhysicsUpdate(float delta)
        {
            MoveTowardsNavTarget(delta);
        }


        public override void Enter()
        {
            base.Enter();
            _blackboard.IsInPlayerArea = false;
            _blackboard.NavAgent.NavigationLayers = 1; // set to outside
            Position3D queuePoint = _blackboard.TargetBoardedWindow.RandomAvailableQueuePoint();
            _blackboard.TargetBoardedWindow.ZombieReserveQueuePoint(_blackboard.Character, queuePoint);

            _blackboard.Character.SetCollisionMaskBit(2, false); // Turn off collision with other zombies

            _blackboard.TargetBoardedWindow.Connect(nameof(BoardedWindow.NextZombieCalled), this, nameof(OnTargetWindowNextZombieCalled));
            NavTarget = queuePoint.GlobalTranslation;
            _blackboard.AnimStateMachine.Travel("walk");
        }

        public override void Exit()
        {
            base.Exit();
            _blackboard.TargetBoardedWindow.Disconnect(nameof(BoardedWindow.NextZombieCalled), this, nameof(OnTargetWindowNextZombieCalled));
        }

        protected override void OnNavAgentNavigationFinished()
        {
            if (_blackboard.TargetBoardedWindow.IsAttackPointReserved)
            {
                EmitSignal(nameof(ChangeStateRequest), "QueueAtWindow");
            }
            else
            {
                EmitSignal(nameof(ChangeStateRequest), "MoveToWindowAttack");
            }
        }

        private void OnTargetWindowNextZombieCalled(Zombie nextZombie)
        {
            if (nextZombie == _blackboard.Character)
            {
                EmitSignal(nameof(ChangeStateRequest), "MoveToWindowAttack");
            }
        }

    }
}

