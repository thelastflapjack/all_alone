using Godot;
using System;


namespace Weapons
{
    public class Gun : Spatial
    {

        /// Exported Fields ///
        [Export]
        private GunStats _statsRaw = null; 

        /// Properties - public, protected, private ///
        public int AmmoLoaded {
            private set {
                _ammoLoaded = (int)Mathf.Clamp(value, 0, _statsRaw.MagazineSize);
            }
            get { return _ammoLoaded; }
        }
        public int AmmoRemainder {
            private set {
                _ammoRemainder = (int)Mathf.Max(value, 0);
            }
            get { return _ammoRemainder; }
        }
        public bool IsLoaded {
            get { return AmmoLoaded != 0; }
        }
        public float ReloadTime{
            get { return _statsRaw.ReloadTime; }
        }
        public float SecondsBetweenShots{
            private set; get;
        }
        public int MagazineSize{
            get {return _statsRaw.MagazineSize; }
        }
        public float Recoil {
            get { return _statsRaw.Recoil; }
        }
        public bool IsAutomatic {
            get { return _statsRaw.IsAutomatic; }
        }
        public int BuyCost {
            get { return _statsRaw.BuyCost; }
        }

        public int ProjectileDamage { get{ return _statsRaw.ProjectileDamage; }}
        public int ShotProjectileCount { get{ return _statsRaw.ShotProjectileCount; }}
        public int ProjectileSpeed { get{ return _statsRaw.ProjectileSpeed; }}
        public int MaxRange { get{ return _statsRaw.MaxRange; }}
        public float Spread { get{ return _statsRaw.Spread + _fireSpreadGain; } }

        /// Fields - protected or private ///
        private int _ammoLoaded;
        private int _ammoRemainder;
        private Random _rng = new Random();
        private int _muzzleFlashFrameCount;
        private float _fireSpreadGain = 0;
        private float _fireSpreadRecoverySpeed = 10f; // per second
        private float _fireSpreadGainMax = 30;

        private AudioStreamPlayer3D _audioFire;
        private AudioStreamPlayer3D _audioEmpty;
        private AnimatedSprite3D _muzzleFlashSprite;
        private OmniLight _muzzleFlashLight;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            SecondsBetweenShots = (float)(1.0 / (_statsRaw.FireRate / 60.0));
            
            AmmoLoaded = _statsRaw.MagazineSize;
            MaxAmmo();

            _audioEmpty = GetNode<AudioStreamPlayer3D>("AudioEmpty");
            _audioFire = GetNode<AudioStreamPlayer3D>("AudioFire");
            _muzzleFlashSprite = GetNode<AnimatedSprite3D>("MuzzleFlash");
            _muzzleFlashLight = GetNode<OmniLight>("MuzzleFlash/OmniLight");

            _muzzleFlashSprite.Modulate = new Color(1,1,1,0);
            _muzzleFlashLight.Hide();

            _muzzleFlashFrameCount = _muzzleFlashSprite.Frames.GetFrameCount("default");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_fireSpreadGain > 0)
            {
                _fireSpreadGain = Mathf.MoveToward(_fireSpreadGain, 0, delta * _fireSpreadRecoverySpeed);
            }
        }
        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void MaxAmmo()
        {
            _ammoRemainder = _statsRaw.MaxAmmo;
        }
        
        public bool Fire()
        {
            if (AmmoLoaded > 0)
            {   
                AmmoLoaded--;
                _audioFire.Play();
                ShowMuzzleFlash();
                _fireSpreadGain = Mathf.Min(_fireSpreadGain + _statsRaw.SpreadGainPerShot, _fireSpreadGainMax);
                return true;
            }
            else
            {
                _audioEmpty.Play();
                return false;
            }

            //GD.Print(AmmoLoaded);
            
            // if ammo_loaded == 0:
            // _audio_efffect_empty_click.play()
            // elif not is_reloading:
            //     set_ammo_loaded(ammo_loaded - 1)
        	// 	_anim_player.stop()
        	// 	_anim_player.play("fire")
            //     _fire_effect_bang.play()
        	// 	_cam_trauma_causer.new_trauma()
            //     _show_muzzle_flash()
            //     return true
            // return false
        }

        public void ReloadStart()
        {
            _ammoLoaded = 0;
        }

        public void ReloadEnd()
        {
            if (_ammoRemainder < MagazineSize)
            {
                _ammoLoaded = _ammoRemainder;
                _ammoRemainder = 0;
            }
            else
            {
                _ammoLoaded = MagazineSize;
                _ammoRemainder -= MagazineSize;
            }
        }


        private void ShowMuzzleFlash()
        {
            float flashDuration = 0.2f;
            _muzzleFlashSprite.RotateZ(Mathf.Pi * (float)_rng.NextDouble());
            _muzzleFlashSprite.Frame = _rng.Next(0, _muzzleFlashFrameCount);

            SceneTreeTween flashTween = CreateTween();
            flashTween.TweenCallback(_muzzleFlashLight, "show");
            flashTween.SetParallel(true);
            flashTween.TweenProperty(_muzzleFlashLight, "light_energy", 0.0f, flashDuration).From(0.5f);
            flashTween.TweenProperty(_muzzleFlashSprite, "modulate", new Color(1,1,1,0), flashDuration).From(Colors.White);
            flashTween.SetParallel(false);
            flashTween.TweenCallback(_muzzleFlashLight, "hide");
        }
    }
}

