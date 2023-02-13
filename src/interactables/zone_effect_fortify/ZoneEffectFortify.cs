using Godot;
using System;
using System.Collections;
using System.Collections.Generic;


namespace ZombieHoardGame
{
    public class ZoneEffectFortify : Area, IInteractable
    {
        private const int CostPerWindow = 300;

        public int BuyCost{
            get { return _validWindows.Count * CostPerWindow; }
        }

        private HashSet<BoardedWindow> _validWindows = new HashSet<BoardedWindow>();
        private AnimationPlayer _animPlayer;

        
        public override void _Ready()
        {
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }
        

        public String GetInteractText()
        {
            if (_validWindows.Count == 0)
            {
                return "No windows to restore";
            }
            else
            {
                return $"Fortify zone [{BuyCost}]";
            }
        }

        public void SetZoneWindows(List<BoardedWindow> windows)
        {
            foreach (BoardedWindow window in windows)
            {
                window.Connect(nameof(BoardedWindow.BoardRemoved), this, nameof(OnZoneWindowBoardRemoved));
                window.Connect(nameof(BoardedWindow.FullyBoarded), this, nameof(OnZoneWindowFullyBoarded));
                if (!window.IsFullyBoarded)
                {
                    _validWindows.Add(window);
                }
            }
        }

        public void Use()
        {
            List<BoardedWindow> validWindowsList = new List<BoardedWindow>(_validWindows);
            foreach (BoardedWindow window in validWindowsList)
            {
                window.RestoreBoards();
            }
            _animPlayer.Play("flash_good");
        }


        private void OnZoneWindowFullyBoarded(BoardedWindow window)
        {
            _validWindows.Remove(window);
        }

        private void OnZoneWindowBoardRemoved(BoardedWindow window)
        {
            _validWindows.Add(window);
        }
    }
}

