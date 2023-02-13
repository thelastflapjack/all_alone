using Godot;
using System.Diagnostics;
using ZombieHoardGame.PlayerCharacter;


namespace ZombieHoardGame.ZombieCharacter
{
    public class AttackTrigger : Area
    {
        [Signal]
        public delegate void PlayerEntered(Player player);

        public bool IsPlayerInside{
            private set;
            get;
        }

        public override void _Ready()
        {
            Connect("body_entered", this, nameof(OnBodyEntered));
            Connect("body_exited", this, nameof(OnBodyExited));
        }
        
        private void OnBodyEntered(PhysicsBody body)
        {
            Debug.Assert(body is Player, "AttackTrigger must only detect Player characters");
            IsPlayerInside = true;
            EmitSignal(nameof(PlayerEntered), (Player)body);
        }

        private void OnBodyExited(PhysicsBody body)
        {
            Debug.Assert(body is Player, "AttackTrigger must only detect Player characters");
            IsPlayerInside = false;
        }
    }
}

