using Godot;
using System;


namespace ZombieHoardGame
{
    public class Stopwatch : Node
    {
        public float ElapsedTime{ get{ return _timeSeconds; } }
        

        private bool _isStopped = true;
        private float _timeSeconds = 0;


        public override void _Process(float delta)
        {
            if (!_isStopped)
            {
                _timeSeconds += delta;
            }
        }

        public void Start()
        {
            _isStopped = false;
        }

        public void Stop()
        {
            _isStopped = true;
        }

        public String ElapsedTimeFormattedString()
        {
            int timeMinutes = Mathf.FloorToInt(_timeSeconds / 60);
            float timeRemainderSeconds = Mathf.Stepify(_timeSeconds - (timeMinutes * 60), 0.01f);
            return $"{timeMinutes}:{timeRemainderSeconds}";
        }
    }
}

