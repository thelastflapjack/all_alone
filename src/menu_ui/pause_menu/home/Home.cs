using Godot;


namespace UI.PauseMenu
{
    public class Home : MultiPage.Page
    {
        [Signal]
        public delegate void ResumeRequested();
        [Signal]
        public delegate void RestartRequested();
        [Signal]
        public delegate void QuitRequested();


        public override void _UnhandledInput(InputEvent @event)
        {
            if (IsActive && @event.IsActionPressed("ui_cancel"))
            {
                EmitSignal(nameof(ResumeRequested));
            }
        }
        
        private void OnBtnResumePressed()
        {
            if (IsActive)
            {
                EmitSignal(nameof(ResumeRequested));
            }
        }
        
        private void OnBtnRestartPressed()
        {
            if (IsActive)
            {
                EmitSignal(nameof(RestartRequested));
            }
        }
        
        private void OnBtnSettingsPressed()
        {
            if (IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), "SettingsPage");
            }
        }
        
        private void OnBtnControlsPressed()
        {
            if (IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), "ControlsPage");
            }
        }
        
        private void OnBtnQuitPressed()
        {
            if (IsActive)
            {
                EmitSignal(nameof(QuitRequested));
            }
        }
    }
}

