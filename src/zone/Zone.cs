using Godot;
using System.Collections.Generic;


namespace ZombieHoardGame
{
    public class Zone : Spatial
    {
        [Signal]
        public delegate void Activated();

        /// Exported Fields ///
        [Export]
        private List<NodePath> _accessObstructionNodePaths = new List<NodePath>();
        [Export]
        private bool _activateOnReady = false;

        /// Properties - public, protected, private ///
        public List<BoardedWindow> ZoneWindows{
            get { return _windows; }
        }

        /// Fields - protected or private ///
        private List<BoardedWindow> _windows = new List<BoardedWindow>();
        private bool _isActivated = false;


        public override void _Ready()
        {
            GetNode<MeshInstance>("EditorMarker").QueueFree();

            ZoneEffectFortify zoneEffectFortify = null;
            foreach (Node child in GetChildren())
            {
                if (child is BoardedWindow)
                {
                    _windows.Add((BoardedWindow)child);
                }
                else if (child is ZoneEffectFortify)
                {
                    zoneEffectFortify = (ZoneEffectFortify)child;
                }
            }

            if (zoneEffectFortify != null)
            {
                zoneEffectFortify.SetZoneWindows(_windows);
            }

            if (_activateOnReady)
            {
                Activate();
            }
            else
            {
                foreach(NodePath path in _accessObstructionNodePaths)
                {
                    Obstruction obstruction = GetNode<Obstruction>(path);
                    obstruction.Connect(nameof(Obstruction.Cleared), this, nameof(OnObstructionCleared));
                }
            }
        }
        
        private void OnObstructionCleared()
        {
            if (!_isActivated)
            {
                Activate();
            }
        }

        private void Activate()
        {
            foreach(BoardedWindow window in _windows)
            {
                window.Activate();
            }
            _isActivated = true;
            EmitSignal(nameof(Activated));
        }
    }
}

