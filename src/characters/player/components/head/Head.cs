using Godot;

namespace ZombieHoardGame.PlayerCharacter
{
    public class Head : Spatial
    {

        public Basis CameraBasis
        {
            get { return _camera.GlobalTransform.basis; }
        }

        public float CameraFOV { get { return _camera.Fov; }}

        private Camera _camera;
        private Position3D _weaponAnchorPoint;

        public override void _Ready()
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;

            _camera = GetNode<Camera>("Camera");
            _weaponAnchorPoint = GetNode<Position3D>("Guns");
        }

    }
}

