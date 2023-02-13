using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class ClimbThroughWindow : ZombieState
    {
        [Export]
        private float climbAnimationLength = 1.25f; // Only reliable way to sync transitions with the animation tree
        private Timer _climbTimer;

        
        public override void _Ready()
        {
            base._Ready();
            _climbTimer = GetNode<Timer>("TimerClimb");
            _climbTimer.WaitTime = climbAnimationLength;
        }

        public override void PhysicsUpdate(float delta)
        {
            _blackboard.Character.Translate(-_blackboard.AnimTree.GetRootMotionTransform().origin);
        }

        public override void Enter()
        {
            base.Enter();
            _blackboard.Character.GlobalTransform = _blackboard.TargetBoardedWindow.ZombieAttackPointTransform;
            Climb();
        }

        public override void Exit()
        {
            base.Exit();
            _blackboard.IsInPlayerArea = true;
            _blackboard.Character.EmitSignal(nameof(Zombie.EnteredPlayerArea), _blackboard.Character);

            _blackboard.Character.SetCollisionMaskBit(2, true); // Turn on collision with other zombies
        }

        private async void Climb()
        {
            _blackboard.Character.SetCollisionMaskBit(1, false); // turn off environment collision

            _blackboard.AnimStateMachine.Travel("window_climb");

            _climbTimer.Start();
            await ToSignal(_climbTimer, "timeout");

            _blackboard.NavAgent.NavigationLayers = 2; // set to inside
            _blackboard.Character.SetCollisionMaskBit(1, true); // turn on environment collision
            EmitSignal(nameof(ChangeStateRequest), "Chase");
            
        }
    }
}

