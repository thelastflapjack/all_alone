using Godot;


namespace UI
{
    public class ControlsPage : UI.MultiPage.Page
    {
        private void OnBackButtonPressed()
        {
            if(IsActive)
            {
                EmitSignal(nameof(ChangePageRequested), _backPageName);
            }
        }
    }
}

