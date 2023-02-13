using Godot;


namespace UI.MainMenu
{
    public class BackgroundCamera : Camera
    {
        [Export]
        private OpenSimplexNoise _noise = null;
        [Export]
        private Vector3 _maxRotationDeg = new Vector3(10, 10, 2);
        [Export]
        private float _shakeSpeed = 1.5f;


        private Vector3 _initialRotationDeg;


        public override void _Ready()
        {
            _initialRotationDeg = RotationDegrees;
        }

        public override void _PhysicsProcess(float delta)
        {
            Vector3 newRotationDeg = _initialRotationDeg;
            newRotationDeg.x += _maxRotationDeg.x * RandNoiseValue(1);
            newRotationDeg.y += _maxRotationDeg.y * RandNoiseValue(2);
            newRotationDeg.z += _maxRotationDeg.z * RandNoiseValue(3);
            
            RotationDegrees = newRotationDeg;
        }

        private float RandNoiseValue(int seed)
        {
            _noise.Seed = seed;
            return _noise.GetNoise1d(OS.GetTicksMsec() * 0.001f * _shakeSpeed);
        }
    }
}

