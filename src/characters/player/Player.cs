using Godot;
using System;
using System.Collections.Generic;
using GameGeneral.FSM;
using ZombieHoardGame.PlayerCharacter.States;
using Weapons;
using HealthSystem;


namespace ZombieHoardGame.PlayerCharacter
{
    public class Player : KinematicBody
    {
        /// Signals ///
        [Signal]
        public delegate void Died(Player player);

        /// Enums ///
        public enum GunAnimationState
        {
            Shooting, Idle, Walk, Run
        }

        /// Constants ///

        /// Exported Fields ///
        [Export]
        private PackedScene _starterGunPackedScene = null;

        /// Properties - public, protected, private ///
        public Vector3 Velocity{ set; get; }
        public float MaxFallSpeed {get;} = -30;
        public float MaxFloorAngle {get;} = Mathf.Deg2Rad(30);
        public bool CanShootInCurrentState { set; get; } = true;


        /// Fields - protected or private ///
        private UserInput _userInputComponent;
        private StateMachine _stateMachine;
        private Head _head;
        private Gun _currentGun;
        private int _currentGunIndex;
        private Position3D _gunsAnchor;
        private Timer _timerFireRate;
        private Timer _timerReload;
        private Timer _timerSwitchGun;
        private PlayerHUD _hud;
        private List<Gun> _guns = new List<Gun>();
        private int _points;
        private Health _health;
        private RayCast _interactRay;
        private Camera _gunCamera;
        private AnimationTree _gunAnimTree;
        private AnimationNodeStateMachinePlayback _gunAnimStateMachine;

        private float _motionSpread = 0.0f;
        private float _motionSpreadMax = 4.0f;
        private float _motionSpreadEffectCeilingSpeed = 5.0f; // The speed at which _motionSpread will be _motionSpreadMax
        private float _motionSpreadEffectCeilingSpeedSquared;
        
        private float _recoilTrauma;
        private float _recoilRecoveryRate = 1.8f; // per second;
        private float _recoilZDisplacementMax = 0.06f;
        private float _recoilRestZTranslation;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            GetNode<Viewport>("CanvasLayer/ViewportContainer/GunViewport").Size = OS.WindowSize;
            
            CacheNodeReferences();
            ConnectComponentSignals();
            SupplyReferencesForStates();

            EquipGun(_starterGunPackedScene);

            _hud.SetHealthComponent(_health);

            _recoilRestZTranslation = _gunsAnchor.Translation.z;

            _motionSpreadEffectCeilingSpeedSquared = _motionSpreadEffectCeilingSpeed * _motionSpreadEffectCeilingSpeed;

            _gunAnimStateMachine = (AnimationNodeStateMachinePlayback)_gunAnimTree.Get("parameters/playback");
        }

        public override void _Process(float delta)
        {
            _stateMachine.Update(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _stateMachine.PhysicsUpdate(delta);
            CheckInteractRay();
            ProcessRecoil(delta);
            _gunCamera.GlobalTransform = _head.GlobalTransform;

            float baseGunSpeadWeight = 1 - ((_motionSpreadEffectCeilingSpeedSquared - Velocity.LengthSquared()) / _motionSpreadEffectCeilingSpeedSquared);
            baseGunSpeadWeight = Mathf.Clamp(baseGunSpeadWeight, 0, 1);
            _motionSpread = Mathf.Lerp(0, _motionSpreadMax, baseGunSpeadWeight);

            _hud.UpdateCrosshair(_currentGun.Spread + _motionSpread, _head.CameraFOV);
        }

        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void RotateHeadX(float value)
        {
            _head.RotateX(value);
        }

        public Basis GetCameraBasis()
        {
            return _head.CameraBasis;
        }

        public void RotateView(Vector2 rotationValues)
        {
            RotateY(-rotationValues.x);
            RotateHeadX(-rotationValues.y);
            //_head.AddWeaponSway(rotationValues);
        }

        public void IncrementPoints(int increment)
        {
            _points += increment;
            _hud.UpdatePointsValue(_points);
            //GD.Print($"+{increment}");
        }

        public void RoundNumberChanged(int number)
        {
            _hud.UpdateRoundNumber(number);
        }

        public void PowerUpEffectEffectMaxAmmo()
        {
            foreach( Gun gun in _guns)
            {
                gun.MaxAmmo();
            }
            _hud.UpdateAmmoLabel(_currentGun.AmmoLoaded, _currentGun.AmmoRemainder);
        }

        public void UpdateGunAnimationState(String stateName)
        {
            if (stateName != "")
            {
                _gunAnimStateMachine.Travel(stateName);
            }
            
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnTimerFireRateTimeout()
        {
            PlayerState currentState = (PlayerState)_stateMachine.CurrentState;
            if (_userInputComponent.IsShootPressed && _currentGun.IsAutomatic && _timerSwitchGun.IsStopped() && currentState.CanShoot)
            {
                AttemptFireCurrentGun();
            }
        }

        private void OnHealthHurt(Node inflictor, int value, bool isCritical)
        {
            if (_health.Points <= 0)
            {
                Die();
            }
        }

        private void OnUserInputReload()
        {
            if (_currentGun.AmmoRemainder > 0 && _timerSwitchGun.IsStopped() && _timerReload.IsStopped())
            {
                ReloadCurrentGun();
            }
            else
            {
                GD.Print("Cannot reload");
            }
        }

        private void OnUserInputShoot()
        {
            PlayerState currentState = (PlayerState)_stateMachine.CurrentState;
            if(_timerSwitchGun.IsStopped() && currentState.CanShoot && _timerFireRate.IsStopped())
            {
                AttemptFireCurrentGun();
            }
        }

        private void OnUserInputSwitchGun()
        {
            if (_guns.Count > 1 && _timerSwitchGun.IsStopped() && _timerReload.IsStopped())
            {
                SwitchGun();
            }
        }

        private void OnUserInputInteract()
        {
            if (_interactRay.IsColliding())
            {
                Interact(_interactRay.GetCollider());
            }
        }

        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void EquipGun(PackedScene gunPackedScene)
        {
            if (_guns.Count < 2)
            {
                Gun newGun = gunPackedScene.Instance<Gun>();
                _guns.Add(newGun);
                _gunsAnchor.AddChild(newGun);
                if (_currentGun == null)
                {
                    SetCurrentGun(0);
                }
                else if (_timerSwitchGun.IsStopped() && _timerReload.IsStopped())
                {
                    SwitchGun();
                }
            }
            else
            {
                Gun newGun = gunPackedScene.Instance<Gun>();
                _guns[_currentGunIndex] = newGun;
                _currentGun.QueueFree();
                _gunsAnchor.AddChild(newGun);
                SetCurrentGun(_currentGunIndex);
            }
        }
        
        private void CacheNodeReferences()
        {
            _userInputComponent = GetNode<UserInput>("UserInput");
            _stateMachine = GetNode<StateMachine>("StateMachine");
            _head = GetNode<Head>("Head");
            _timerFireRate = GetNode<Timer>("TimerFireRate");
            _timerReload = GetNode<Timer>("TimerReload");
            _timerSwitchGun = GetNode<Timer>("TimerSwitchGun");
            _hud = GetNode<PlayerHUD>("CanvasLayer/PlayerHUD");
            _health = GetNode<Health>("Health");
            _interactRay = _head.GetNode<RayCast>("InteractRay");
            _gunsAnchor = _head.GetNode<Position3D>("Guns");
            _gunCamera = GetNode<Camera>("CanvasLayer/ViewportContainer/GunViewport/GunCamera");
            _gunAnimTree = GetNode<AnimationTree>("Head/GunsAnimationTree");
        }

        private void SupplyReferencesForStates()
        {
            foreach (PlayerState state in _stateMachine.GetChildren())
            {
                state.Player = this;
                state.UserInput = _userInputComponent;
            }
        }

        private void ConnectComponentSignals()
        {
            _timerFireRate.Connect("timeout", this, nameof(OnTimerFireRateTimeout));
            _health.Connect(nameof(Health.Hurt), this, nameof(OnHealthHurt));

            _userInputComponent.Connect(nameof(UserInput.Reload), this, nameof(OnUserInputReload));
            _userInputComponent.Connect(nameof(UserInput.Shoot), this, nameof(OnUserInputShoot));
            _userInputComponent.Connect(nameof(UserInput.SwitchGun), this, nameof(OnUserInputSwitchGun));
            _userInputComponent.Connect(nameof(UserInput.Interact), this, nameof(OnUserInputInteract));
        }

        private void SetCurrentGun(int slotIndex)
        {
            Gun newGun = _guns[slotIndex];
            if (_currentGun != null)
            {
                _currentGun.Hide();
                _timerFireRate.Stop();
            }

            _currentGun = newGun;
            _currentGunIndex = slotIndex;
            _currentGun.Show();
            _hud.UpdateAmmoLabel(_currentGun.AmmoLoaded, _currentGun.AmmoRemainder);
            _timerReload.WaitTime = _currentGun.ReloadTime;
        }

        private void AttemptFireCurrentGun()
        {
            // TODO: Move more of the gun code onto the gun object
            float spread = _currentGun.Spread + _motionSpread;
            //GD.Print(spread);
            if (_currentGun.Fire())
            {
                _hud.UpdateAmmoLabel(_currentGun.AmmoLoaded, _currentGun.AmmoRemainder);
                LevelServices.Instance.BulletSpawner.SpawnGunShot(
                    _head.GlobalTranslation, -GetCameraBasis().z, GetCameraBasis().x, 
                    spread, _currentGun, this
                );
                _timerFireRate.Start(_currentGun.SecondsBetweenShots);
                IncrementRecoilTrauma(_currentGun.Recoil);
            }
        }

        private async void ReloadCurrentGun()
        {
            _currentGun.ReloadStart();
            _timerReload.Start();
            _hud.UpdateAmmoLabel(_currentGun.AmmoLoaded, _currentGun.AmmoRemainder);
            _hud.ShowReloadBar(_timerReload.WaitTime);
            await ToSignal(_timerReload, "timeout");
            _currentGun.ReloadEnd();
            _hud.UpdateAmmoLabel(_currentGun.AmmoLoaded, _currentGun.AmmoRemainder);
        }

        private async void SwitchGun()
        {
            _currentGun.Hide();
            _timerSwitchGun.Start();
            await ToSignal(_timerSwitchGun, "timeout");

            int nextGunSlotIndex = _currentGunIndex + 1;
            if (nextGunSlotIndex == _guns.Count)
            {
                nextGunSlotIndex = 0;
            }

            SetCurrentGun(nextGunSlotIndex);
        }

        private void Die()
        {
            EmitSignal(nameof(Died), this);
        }

        private void CheckInteractRay()
        {
            if (_interactRay.IsColliding())
            {
                IInteractable interactable = (IInteractable)_interactRay.GetCollider();
                _hud.UpdateInteractiveText(interactable.GetInteractText());
            }
            else
            {
                _hud.UpdateInteractiveText(null);
            }
        }

        private void UseGunWallBuy(GunWallBuy gunWallBuy)
        {
            int useCost = -1;
            gunWallBuy.Use(_guns, _points, out useCost);

            if (useCost != -1)
            {
                IncrementPoints(-useCost);

                foreach (Gun playerGun in _guns)
                {
                    if (playerGun.Filename == gunWallBuy.GunPackedScene.ResourcePath)
                    {
                        playerGun.MaxAmmo();
                        _hud.UpdateAmmoLabel(_currentGun.AmmoLoaded, _currentGun.AmmoRemainder);
                        return;
                    }
                }
                
                EquipGun(gunWallBuy.GunPackedScene);
            }
        }

        private void UseObstruction(Obstruction obstruction)
        {
            if (obstruction.BuyCost <= _points)
            {
                obstruction.Clear(this);
            }
        }

        private async void UseBoardedWindow(BoardedWindow window)
        {
            if (!window.IsFullyBoarded)
            {
                window.PlayerAddBoard(this);
                await ToSignal(window, nameof(BoardedWindow.BoardAdded));
                if (_userInputComponent.IsInteractPressed)
                {
                    UseBoardedWindow(window);
                }
            }
        }

        private void UseZoneEffectFortify(ZoneEffectFortify zoneEffect)
        {
            if (zoneEffect.BuyCost <= _points)
            {
                IncrementPoints(-zoneEffect.BuyCost);
                zoneEffect.Use();
            }
        }

        private void Interact(Godot.Object interactiveObject)
        {
            // TODO: Move interact code off the player and onto the interactive objects
            if (interactiveObject is GunWallBuy && _timerSwitchGun.IsStopped() && _timerReload.IsStopped())
            {
                UseGunWallBuy((GunWallBuy)interactiveObject);
            }
            else if (interactiveObject is Obstruction)
            {
                UseObstruction((Obstruction)interactiveObject);
            }
            else if (interactiveObject is BoardedWindow)
            {
                UseBoardedWindow((BoardedWindow)interactiveObject);
            }
            else if (interactiveObject is ZoneEffectFortify)
            {
                UseZoneEffectFortify((ZoneEffectFortify)interactiveObject);
            }
        }
    
        private void IncrementRecoilTrauma(float increment)
        {
            _recoilTrauma = Mathf.Clamp(_recoilTrauma + increment, 0f, 1.0f);
        }

        private void ProcessRecoil(float delta)
        {
            if (_recoilTrauma > 0)
            {
                Vector3 newTranslation = _gunsAnchor.Translation;
                newTranslation.z = Mathf.Lerp(
                    _recoilRestZTranslation,
                    _recoilRestZTranslation + _recoilZDisplacementMax,
                    _recoilTrauma
                );
                _gunsAnchor.Translation = newTranslation;

                IncrementRecoilTrauma(-_recoilRecoveryRate * delta);
            }
        }
    }
}

