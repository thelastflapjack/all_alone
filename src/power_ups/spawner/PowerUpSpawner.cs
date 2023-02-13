using Godot;
using System;
using System.Collections.Generic;
using ZombieHoardGame;


namespace PowerUps
{
    public class PowerUpSpawner : Node
    {
        [Signal]
        public delegate void MaxAmmoCollected();

        [Export]
        List<PowerUpData> _powerUpsData = new List<PowerUpData>();

        public int TotalPowerupsCollected{ private set; get; } = 0;

        private PackedScene _powerUpPackedScene = ResourceLoader.Load<PackedScene>("res://src/power_ups/PowerUp.tscn");
        private Timer _timerDouleTap;
        private Timer _timerDoulePoints;
        private HBoxContainer _powerupIconsContainer;
        private Dictionary<PowerUp.Type, TextureRect> _activeIcons = new Dictionary<PowerUp.Type, TextureRect>();
        private Dictionary<PowerUp.Type, SceneTreeTween> _activeIconTweens = new Dictionary<PowerUp.Type, SceneTreeTween>();


        public override void _Ready()
        {
            _timerDouleTap = GetNode<Timer>("TimerDoubleTap");
            _timerDoulePoints = GetNode<Timer>("TimerDoublePoints");
            _powerupIconsContainer = GetNode<HBoxContainer>("CanvasLayer/UI/PowerupIcons");

            foreach (Node child in _powerupIconsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        public void SpawnRandom(Vector3 location)
        {
            Random r = new Random();
            int randIndex = r.Next(0, _powerUpsData.Count);
            PowerUpData newPowerUpData = _powerUpsData[randIndex];
            PowerUp newPowerUp = _powerUpPackedScene.Instance<PowerUp>();
            AddChild(newPowerUp);
            newPowerUp.GlobalTranslation = location;
            newPowerUp.ResourceData = newPowerUpData;

            newPowerUp.Connect(nameof(PowerUp.CollectedByPlayer), this, nameof(OnPowerUpCollectedByPlayer));
            newPowerUp.Connect(nameof(PowerUp.Timedout), this, nameof(OnPowerUpTimedout));
        }

        private void OnPowerUpCollectedByPlayer(PowerUp powerUp)
        {
            powerUp.Despawn();
            TotalPowerupsCollected++;

            switch (powerUp.PowerupType)
            {
                case PowerUp.Type.MaxAmmo:
                    ActivateMaxAmmo(powerUp.Icon);
                    break;
                case PowerUp.Type.DoubleTap:
                    ActivateDoubleTap(powerUp.Icon);
                    break;
                case PowerUp.Type.DoublePoints:
                    ActivateDoublePoints(powerUp.Icon);
                    break;
                default:
                    break;
            }
        }

        private void OnPowerUpTimedout(PowerUp powerUp)
        {
            powerUp.QueueFree();
        }

        private void ActivateMaxAmmo(Texture icon)
        {
            LevelServices.Instance.Player.PowerUpEffectEffectMaxAmmo();
            ShowPowerupIcon(PowerUp.Type.MaxAmmo, icon, 0);
        }

        private async void ActivateDoubleTap(Texture icon)
        {
            ShowPowerupIcon(PowerUp.Type.DoubleTap, icon, _timerDouleTap.WaitTime);
            LevelServices.Instance.BulletSpawner.DoubleProjectileSpawn = true;
            _timerDouleTap.Start();
            await ToSignal(_timerDouleTap, "timeout");

            LevelServices.Instance.BulletSpawner.DoubleProjectileSpawn = false;
        }

        private async void ActivateDoublePoints(Texture icon)
        {
            ShowPowerupIcon(PowerUp.Type.DoublePoints, icon, _timerDoulePoints.WaitTime);
            LevelServices.Instance.PointsAwarder.DoublePoints = true;
            _timerDoulePoints.Start();
            await ToSignal(_timerDoulePoints, "timeout");

            LevelServices.Instance.PointsAwarder.DoublePoints = false;
        }

        private void ClearActiveIcon(PowerUp.Type powerupType)
        {
            _activeIconTweens[powerupType].Kill();
            _activeIconTweens.Remove(powerupType);
            _activeIcons[powerupType].QueueFree();
            _activeIcons.Remove(powerupType);
        }

        private void ShowPowerupIcon(PowerUp.Type powerupType, Texture icon, float duration)
        {
            // CONSIDER: Emit a signal for player hud to listen to. Player hud shows icons not spawner
            // Set duration to 0 for powerups which have a one time instant effect like max ammo

            if (_activeIcons.ContainsKey(powerupType))
            {
                ClearActiveIcon(powerupType);
            }
            
            TextureRect iconRect = new TextureRect();
            iconRect.Expand = true;
            iconRect.RectMinSize = new Vector2(90, 0);
            iconRect.Texture = icon;
            _powerupIconsContainer.AddChild(iconRect);
            _activeIcons[powerupType] = iconRect;

            SceneTreeTween iconTween = GetTree().CreateTween();
            _activeIconTweens[powerupType] = iconTween;
            if (duration > 0)
            {
                int flashTime = 3;
                iconTween.TweenInterval(duration - flashTime);
                for (int i = 0; i < flashTime; i++)
                {
                    iconTween.TweenProperty(iconRect, "modulate", new Color(1,1,1,0), 0.5f).FromCurrent();
                    iconTween.TweenProperty(iconRect, "modulate", new Color(1,1,1,1), 0.5f).FromCurrent();
                }
            }
            else
            {
                iconTween.TweenInterval(3);
            }
            iconTween.TweenCallback(this, nameof(ClearActiveIcon), new Godot.Collections.Array(){powerupType});
            iconTween.Play();
        }
    }
}

