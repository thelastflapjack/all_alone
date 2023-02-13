using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class QueueAtWindow : ZombieState
    {
        
        public override void Enter()
        {
            base.Enter();
            _blackboard.TargetBoardedWindow.Connect(nameof(BoardedWindow.NextZombieCalled), this, nameof(OnTargetWindowNextZombieCalled));
            _blackboard.AnimStateMachine.Travel("idle");
        }

        public override void Exit()
        {
            base.Exit();
            _blackboard.TargetBoardedWindow.Disconnect(nameof(BoardedWindow.NextZombieCalled), this, nameof(OnTargetWindowNextZombieCalled));
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

