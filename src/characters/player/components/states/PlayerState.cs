using Godot;
using System;
using GameGeneral.FSM;

namespace ZombieHoardGame.PlayerCharacter.States
{
    public class PlayerState : State
    {
        [Export]
        private bool _canShoot = true;
        
        public UserInput UserInput
        {
            set { 
                _userInput = value;
                _userInput.Connect(nameof(UserInput.MouseMotionInputEvent), this, nameof(OnUserInputMouseMotionInputEvent));
            }
        }

        public Player Player
        {
            set { _player = value; }
        }

        public bool CanShoot {
            get { return _canShoot; }
        }

        protected Player _player;
        protected UserInput _userInput;

        protected static float _gravityAcceleration = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        protected Vector3 _snapVector = Vector3.Down;


        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public override void Enter()
        {
            //GD.Print($"Player Entered State {Name}");
        }


        //////////////////////////////
        //     Protected Methods    //
        //////////////////////////////
        protected virtual void UpdateState()
        {
            String targetState = Name;
            if (_player.IsOnFloor())
            {
                // if (_userInput.IsJumpPressed)
                // {
                //     targetState = "Jumping";
                // }
                if (_userInput.MoveDirectionXZ != Vector3.Zero)
                {
                    if (_userInput.IsSprintPressed && (_userInput.MoveDirectionXZ.z < 0))
                    {
                        targetState = "Sprinting";
                    }
                    else
                    {
                        targetState = "Walking";
                    }
                }
                else
                {
                    targetState = "Idling";
                }
            }
            else
            {
                targetState = "Falling";
            }

            if (targetState != Name)
            {
                EmitSignal(nameof(ChangeStateRequest), targetState);
            }
        }

        protected void UpdateSnapVector()
        {
            if (_player.IsOnFloor())
            {
                _snapVector = -_player.GetFloorNormal();
            }
            else
                _snapVector = Vector3.Zero;
        }

        protected void ApplyGravity(float delta)
        {
            if (!_player.IsOnFloor())
            {
                Vector3 _updatedVelocity = _player.Velocity;
                _updatedVelocity.y = Mathf.Max(
                    _player.Velocity.y - (_gravityAcceleration * delta),
                    _player.MaxFallSpeed
                );
                _player.Velocity = _updatedVelocity;
            }
        }

        protected void PlayerMoveAndSlideWithSnap(float delta)
        {
            _player.Velocity = _player.MoveAndSlideWithSnap(
                _player.Velocity, _snapVector, Vector3.Up, true,
                4, _player.MaxFloorAngle
            );
        }

        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        protected virtual void OnUserInputMouseMotionInputEvent(Vector2 rotationValues)
        {
            _player.RotateView(rotationValues);
        }

    }
}

