using Godot;
using System;
using System.Collections.Generic;
using Weapons;

namespace ZombieHoardGame
{
    public class GunWallBuy : Area, IInteractable
    {

        /// Properties - public, protected, private ///
        [Export]
        public PackedScene GunPackedScene { 
            get{ return _gunPackedScene; }
            private set { _gunPackedScene = value; }
        }

        /// Fields - protected or private ///
        private Sprite3D _iconSprite;
        private AnimationPlayer _animPlayer;
        private AudioStreamPlayer3D _audioUsed;
        private PackedScene _gunPackedScene;
        private int _buyGunCost;
        private int _buyAmmoCost;

        public override void _Ready()
        {
            _iconSprite = GetNode<Sprite3D>("Sprite3D");
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _audioUsed = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

            UpdateGunInfo();
        }

        public void Use(List<Gun> playerGuns, int playerPoints, out int useCost)
        {
            useCost = -1;

            foreach (Gun playerGun in playerGuns)
            {
                if (playerGun.Filename == _gunPackedScene.ResourcePath)
                {
                    if (playerPoints >= _buyAmmoCost)
                    {
                        useCost = _buyAmmoCost;
                        Flash(true);
                    }
                    else
                    {
                        Flash(false);
                    }
                    return;
                }
            }

            if (playerPoints >= _buyGunCost)
            {
                useCost = _buyGunCost;
                Flash(true);
            }
            else
            {
                Flash(false);
            }
        }


        public String GetInteractText()
        {
            return $"[BUY - {_buyGunCost}], [AMMO - {_buyAmmoCost}]";
        }

        private void Flash(bool flashGood)
        {
            if (flashGood)
            {
                _animPlayer.Play("flash_good");
                _audioUsed.Play();
            }
            else
            {
                _animPlayer.Play("flash_bad");
            }
        }

        private void UpdateGunInfo()
        {
            Gun gun = GunPackedScene.Instance<Gun>();
            _buyGunCost = gun.BuyCost;
            _buyAmmoCost = Mathf.RoundToInt(_buyGunCost * 0.5f);
            String iconFilePath = $"{GunPackedScene.ResourcePath.GetBaseDir()}/icon.png";
            _iconSprite.Texture = ResourceLoader.Load<Texture>(iconFilePath);
            gun.Dispose();
        }
    }
}

