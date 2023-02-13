using Godot;
using System;


namespace UI.MultiPage
{
    public class Page : Control
    {
        [Signal]
        public delegate void ChangePageRequested(String pageName);

        [Export]
        public Vector2 Coordinates;
        [Export]
        protected String _backPageName;

        public bool IsActive { set; get; } = false;

        public override void _Ready()
        {
            Coordinates = Coordinates.Snapped(new Vector2(1,1));
        }
    }
}

