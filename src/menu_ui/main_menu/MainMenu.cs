using Godot;
using System;
using GameGeneral;


namespace UI.MainMenu
{
    public class MainMenu : MultiPage.Manager
    {
        public override void _Ready()
        {
            base._Ready();

            Home homePage = (Home)_pages["Home"];
            homePage.Connect(nameof(Home.PlayLevelRequested), this, nameof(OnHomePlayLevelRequested));
        }


        private void OnHomePlayLevelRequested(String levelName)
        {
            Main.Instance.TransitionToLevel(levelName);
        }
    }
}

