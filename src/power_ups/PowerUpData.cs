using Godot;


namespace PowerUps
{
    public class PowerUpData : Resource
    {
        [Export]
        public PowerUp.Type Type;
        [Export]
        public Mesh Mesh;
        [Export]
        public Texture Icon;
    }
}

