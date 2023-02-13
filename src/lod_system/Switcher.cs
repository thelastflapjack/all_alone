using Godot;
using System.Collections.Generic;


namespace GameGeneral.LOD
{
    public class Switcher : Spatial
    {
        [Export]
        private List<NodePath> _targetNodePaths = new List<NodePath>();
        [Export]
        private float _lodDistance2 = 5;
        [Export]
        private float _lodDistance3 = 15;
        [Export]
        private float _pollWaitTime = 0.1f;

        public int CurrentLOD{ get { return _currentLod; }}

        private Timer _pollTimer;
        private float _lodDistanceSquare2;
        private float _lodDistanceSquare3;
        private List<Target> _targets = new List<Target>();
        private int _currentLod = 1;


        public override void _Ready()
        {
            _pollTimer = GetNode<Timer>("PollTimer");
            _pollTimer.WaitTime = _pollWaitTime;
            _pollTimer.Connect("timeout", this, nameof(OnPollTimerTimeout));

            _lodDistanceSquare2 = Mathf.Pow(_lodDistance2, 2);
            _lodDistanceSquare3 = Mathf.Pow(_lodDistance3, 2);

            foreach(NodePath targetPath in _targetNodePaths)
            {
                _targets.Add(GetNode<Target>(targetPath));
            }

            Activate(true);
        }
        

        public void Activate(bool value)
        {
            if (value)
            {
                _pollTimer.Start();
            }
            else
            {
                _pollTimer.Stop();
            }
        }


        private void OnPollTimerTimeout()
        {
            Camera camera = GetViewport().GetCamera();
            float viewDistanceSquare = GlobalTranslation.DistanceSquaredTo(camera.GlobalTranslation);
            
            int lodLevelTarget = 1;
            if (viewDistanceSquare >= _lodDistanceSquare3)
            {
                lodLevelTarget = 3;
            }
            else if (viewDistanceSquare >= _lodDistanceSquare2)
            {
                lodLevelTarget = 2;
            }

            SetLOD(lodLevelTarget);
        }


        private void SetLOD(int level)
        {
            if (level != _currentLod)
            {
                foreach (Target target in _targets)
                {
                    target.SetLOD(level);
                }
                _currentLod = level;
            }
        }
    }
}

