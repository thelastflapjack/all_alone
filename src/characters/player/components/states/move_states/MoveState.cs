using Godot;

namespace ZombieHoardGame.PlayerCharacter.States
{
    public class MoveState : PlayerState
    {
        [Export]
        private float _targetSpeedXZ = 5;
        [Export]
        private float _accelerationXZ = 1;
        [Export]
        private float _frictionXZ = 1;
        [Export]
        private string _gunAnimStateName = "";

        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public override void PhysicsUpdate(float delta)
        {
            UpdateState();
            UpdatePlayerMovement(delta);
        }

        public override void Enter()
        {
            base.Enter();
            _player.UpdateGunAnimationState(_gunAnimStateName);
        }

        //////////////////////////////
        //     Protected Methods    //
        //////////////////////////////

        protected void UpdatePlayerMovement(float delta)
        {
            UpdateSnapVector();

            Vector3 inputDirectionXZ = _userInput.MoveDirectionXZ.Rotated(
                Vector3.Up, _player.GetCameraBasis().GetEuler().y
            );

            ApplyMovementInput(delta, inputDirectionXZ);
            ApplyGravity(delta);
            PlayerMoveAndSlideWithSnap(delta);
        }

        protected void ApplyMovementInput(float delta, Vector3 inputDirectionXZ)
        {
            Vector3 newVelocity = _player.Velocity;
            if (inputDirectionXZ != Vector3.Zero)
            {
                Vector3 velocityTargetXZ = inputDirectionXZ * _targetSpeedXZ;
                Vector3 velocityActualXZ = _player.Velocity.MoveToward(velocityTargetXZ, _accelerationXZ * delta);
                newVelocity.x = velocityActualXZ.x;
                newVelocity.z = velocityActualXZ.z;
            }
            else
            {
                Vector3 frictionedVelocityXZ = _player.Velocity.MoveToward(Vector3.Zero, _frictionXZ * delta);
                newVelocity.x = frictionedVelocityXZ.x;
                newVelocity.z = frictionedVelocityXZ.z;
            }
            _player.Velocity = newVelocity;
        }
    }
}
