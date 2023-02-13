using Godot;


namespace ZombieHoardGame
{
    public class RoundCounter : Node
    {
        [Signal]
        public delegate void Incremented(int newValue);
        
        public int RoundsStarted{
            private set;
            get;
        } = 0;

        public void Increment()
        {
            RoundsStarted++;
            EmitSignal(nameof(Incremented), RoundsStarted);
        }
    }
}

