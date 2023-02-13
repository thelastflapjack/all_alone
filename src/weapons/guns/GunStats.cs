using Godot;


namespace Weapons
{
    [Tool]
    public class GunStats : Resource
    {
        [Export]
        public int ProjectileDamage;
        [Export]
        public int FireRate; // rounds per minute
        [Export]
        public int MagazineSize;
        [Export]
        public float Spread;
        [Export]
        public float SpreadGainPerShot = 10f;
        [Export]
        public float Recoil;
        [Export]
        public int ShotProjectileCount = 1;
        [Export]
        public int ProjectileSpeed;
        [Export]
        public int MaxRange;
        [Export]
        public float ReloadTime;
        [Export]
        public int MaxAmmo;
        [Export]
        public bool IsAutomatic;
        [Export]
        public int BuyCost;
    }
}

