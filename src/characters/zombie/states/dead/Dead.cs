using Godot;


namespace ZombieHoardGame.ZombieCharacter.States
{
    public class Dead : ZombieState
    {
        [Export]
        private int _despawnDelay = 5;

        public override async void Enter()
        {
            base.Enter();
            await ToSignal(GetTree().CreateTimer(_despawnDelay), "timeout");
            VisibilityNotifier vn = _blackboard.VisibilityNotifier;
            if (vn.IsOnScreen())
            {
                _blackboard.VisibilityNotifier.Connect("screen_exited", this, nameof(OnVisibilityNotifierScreenExited));
            }
            else
            {
                _blackboard.Character.QueueFree();
            }
        }

        protected void OnVisibilityNotifierScreenExited()
        {
            _blackboard.Character.QueueFree();
        }
    }
}

