using Godot;
using System;

namespace ZombieHoardGame.PlayerCharacter.States
{
    public class Jumping : PlayerState
    {
        [Export]
        private float _heightTarget = 1;

        private float _impluseSpeed;

        public override void _Ready()
        {
            CalculateImpluseSpeed();
        }

        public override void PhysicsUpdate(float delta)
        {
            Vector3 newVelocity = _player.Velocity;
            newVelocity.y += _impluseSpeed;
            _player.Velocity = newVelocity;
            _snapVector = Vector3.Zero;
            PlayerMoveAndSlideWithSnap(delta);
            UpdateState();
        }

        private void CalculateImpluseSpeed()
        {
            float timeToPeak = Mathf.Sqrt((-2.0f * _heightTarget) / -_gravityAcceleration);
            _impluseSpeed = (2 * _heightTarget) / timeToPeak;
        }
    }
}
