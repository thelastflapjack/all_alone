using Godot;
using System;

namespace ZombieHoardGame.PlayerCharacter.States
{
    public class Idling : PlayerState
    {
         
        [Export]
        private float _frictionXZ = 1;


        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////

        public override void PhysicsUpdate(float delta)
        {
            UpdateState();
            UpdateSnapVector();
            MoveVelocityXZTowardZero(delta);
            ApplyGravity(delta);
            PlayerMoveAndSlideWithSnap(delta);
        }

        public override void Enter()
        {
            base.Enter();
            _player.UpdateGunAnimationState("idle");
        }


        //////////////////////////////
        //     Protected Methods    //
        //////////////////////////////


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void MoveVelocityXZTowardZero(float delta)
        {
            Vector3 newVelocity = _player.Velocity;
            Vector3 frictionedVelocityXZ = _player.Velocity.MoveToward(Vector3.Zero, _frictionXZ * delta);
            newVelocity.x = frictionedVelocityXZ.x;
            newVelocity.z = frictionedVelocityXZ.z;
            _player.Velocity = newVelocity;
        }
    }
}
