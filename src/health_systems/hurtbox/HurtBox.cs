using Godot;


namespace HealthSystem
{
    public class HurtBox : Area
    {
        [Signal]
        public delegate void Hurt(HurtBox box, Node inflictor, int value);

        [Export]
        private bool _isCritical = false;
        [Export(PropertyHint.Range, "1,100,1")]
        private int _penetrationResistance = 40;
        [Export(PropertyHint.Flags, "player,zombie")]
        private int _damageLayers = 0;

        public bool IsCritical{
            get { return _isCritical; }
        }

        public int PenetrationResistance{
            get { return _penetrationResistance; }
        }

        public int DamageLayers{
            get { return _damageLayers; }
        }

        static private float criticalMultiplier = 1.5f;

        public void ApplyDamage(int damage, Node inflictor)
        {
            // Make sure HurtBoxes do not take damage from their owner
            if (inflictor != Owner)
            {   
                int damageResult = damage;
                if (_isCritical)
                {
                    damageResult = Mathf.FloorToInt(damage * criticalMultiplier);
                }
                EmitSignal(nameof(Hurt), this, inflictor, damageResult);
            }
        }
    }
}

