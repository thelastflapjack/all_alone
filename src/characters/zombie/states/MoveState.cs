using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class MoveState : ZombieState
    {
        [Export]
        private float _speed = 3;

        protected Vector3 NavTarget{
            set {
                if (!value.IsEqualApprox(_navTarget))
                {
                    _navTarget = value;
                    _blackboard.NavAgent.SetTargetLocation(_navTarget);
                    _isNavigating = true;
                }
            }
        }

        protected Vector3 _navTarget;
        protected bool _isNavigating;


        public override void Enter()
        {
            base.Enter();
            _blackboard.NavAgent.Connect("navigation_finished", this, nameof(OnNavAgentNavigationFinished));
        }

        public override void Exit()
        {
            base.Exit();
            _blackboard.NavAgent.Disconnect("navigation_finished", this, nameof(OnNavAgentNavigationFinished));
        }


        protected void MoveTowardsNavTarget(float delta)
        {
            Vector3 targetPosition = _blackboard.NavAgent.GetNextLocation();
            Vector3 direction = _blackboard.Character.GlobalTranslation.DirectionTo(targetPosition);
            Vector3 velocity = direction * _speed;
            _blackboard.Character.MoveAndSlide(velocity, Vector3.Up);
            
            float rotationAcceleration = 4;
            float characterRotationY = Mathf.LerpAngle(
			    _blackboard.Character.Rotation.y, Mathf.Atan2(-direction.x, -direction.z), 
			    rotationAcceleration * delta
	        );
            _blackboard.Character.Rotation =  new Vector3(
                0,
                characterRotationY,
                0
            );
        }


        protected virtual void OnNavAgentNavigationFinished(){}

    }
}

