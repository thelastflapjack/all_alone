using Godot;
using System;


namespace ZombieHoardGame
{
    public class Obstruction : StaticBody, IInteractable
    {
        [Signal]
        public delegate void Cleared();
        
        [Export]
        private int _buyCost = 750;

        public int BuyCost { get{ return _buyCost; } }

        private Particles _clearEffect;
        private AudioStreamPlayer3D _audioClear;
        private NavigationMeshInstance _navMeshInst;

        public override void _Ready()
        {
            _clearEffect = GetNode<Particles>("ClearEffect");
            _audioClear = GetNode<AudioStreamPlayer3D>("AudioClear");
            _navMeshInst = GetNode<NavigationMeshInstance>("NavMeshInst");

            _navMeshInst.Enabled = false;
        }

        public String GetInteractText()
        {
            return $"[BUY - {_buyCost}]";
        }

        public void Clear(PlayerCharacter.Player player)
        {
            _clearEffect.Emitting = true;
            _audioClear.Play();
            foreach (Spatial child in GetChildren())
            {
                if (child is CollisionShape)
                {
                    ((CollisionShape)child).Disabled = true;
                }
                else if (child is MeshInstance)
                {
                    child.Hide();
                }
            }
            player.IncrementPoints(-BuyCost);
            EmitSignal(nameof(Cleared));
            _navMeshInst.Enabled = true;
        }
    }
}

