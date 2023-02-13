using Godot;
using System;
using System.Collections.Generic;
using ZombieHoardGame.PlayerCharacter;
using Weapons;
using PowerUps;
using GameGeneral;


namespace ZombieHoardGame
{
    public class LevelManager : Node
    {
        /// Exported Fields ///
        [Export]
        private bool _buildingMode = true;
        [Export]
        private int _playerInitialPoints = 150;
        [Export]
        private PackedScene _playerPackedScene = null;


        /// Fields - protected or private ///
        private Player _player;
        private HoardDirector _hoardDirector;
        private Timer _timerRoundBreak;
        private AudioStreamPlayer _audioNewRoundStart;
        private UI.PauseMenu.PauseMenu _pauseMenu;
        private UI.GameOverScreen _gameOverScreen;
        private Stopwatch _stopwatch;
        private RoundCounter _roundCounter;

        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            CacheNodeReferences();
            SpawnPlayer();
            SetUpLevelServices();
            ConnectComponentSignals();

            List<BoardedWindow> allWindows = new List<BoardedWindow>();
            foreach (Zone zone in GetNode<Node>("Zones").GetChildren())
            {
                allWindows.AddRange(zone.ZoneWindows);
            }
            _hoardDirector.AllBoardedWindows(allWindows);
            _hoardDirector.SetPlayerReference(_player);

            if (_buildingMode)
            {
                foreach (Obstruction obstruction in GetNode<Node>("Obstructions").GetChildren())
                {
                    obstruction.AddCollisionExceptionWith(_player);
                }
            }
            else
            {
                StartsNewRound();
                _stopwatch.Start();
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                _pauseMenu.Popup();
            }
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////

        private void OnPlayerDied(Player deadPlayer)
        {
            _stopwatch.Stop();
            _gameOverScreen.Popup(_hoardDirector.TotalZombiesKilled, _hoardDirector.TotalZombiesKilledHeadshot);
        }

        private async void OnHoardDirectorAllZombiesKilled()
        {
            _timerRoundBreak.Start();
            await ToSignal(_timerRoundBreak, "timeout");
            StartsNewRound();
        }

        private void OnMenuQuitRequested()
        {
            EndSession();
            Main.Instance.TransitionToMainMenu();
        }

        private void OnMenuRestartRequested()
        {
            EndSession();
            Main.Instance.TransitionToLevel(Filename.GetFile().Split(".")[0]);
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void CacheNodeReferences()
        {
            //_player = GetNode<Player>("Player");
            _hoardDirector = GetNode<HoardDirector>("HoardDirector");
            _timerRoundBreak = GetNode<Timer>("TimerRoundBreak");
            _audioNewRoundStart = GetNode<AudioStreamPlayer>("AudioNewRoundStart");
            _pauseMenu = GetNode<UI.PauseMenu.PauseMenu>("CanvasLayer/PauseMenu");
            _gameOverScreen = GetNode<UI.GameOverScreen>("CanvasLayer/GameOverScreen");
            _stopwatch = GetNode<Stopwatch>("Stopwatch");
            _roundCounter = GetNode<RoundCounter>("RoundCounter");
        }

        private void SpawnPlayer()
        {
            _player = (Player)_playerPackedScene.Instance();
            AddChild(_player);
            _player.GlobalTransform = GetNode<Position3D>("PlayerSpawnPoint").GlobalTransform;
            _player.IncrementPoints(_playerInitialPoints);
        }

        private void SetUpLevelServices()
        {
            LevelServices services = LevelServices.Instance;
            services.BulletSpawner = GetNode<BulletSpawner>("BulletSpawner");
            services.PowerUpSpawner = GetNode<PowerUpSpawner>("PowerUpSpawner");
            services.PointsAwarder = GetNode<PointsAwarder>("PointsAwarder");
            services.Stopwatch = _stopwatch;
            services.RoundCounter = _roundCounter;
            services.Player = _player;
        }

        private void ConnectComponentSignals()
        {
            _player.Connect(nameof(Player.Died), this, nameof(OnPlayerDied));
            _hoardDirector.Connect(nameof(HoardDirector.AllZombiesKilled), this, nameof(OnHoardDirectorAllZombiesKilled));
            _pauseMenu.Connect(nameof(UI.PauseMenu.PauseMenu.QuitRequested), this, nameof(OnMenuQuitRequested));
            _pauseMenu.Connect(nameof(UI.PauseMenu.PauseMenu.RestartRequested), this, nameof(OnMenuRestartRequested));
            _gameOverScreen.Connect(nameof(UI.GameOverScreen.QuitRequested), this, nameof(OnMenuQuitRequested));
            _gameOverScreen.Connect(nameof(UI.GameOverScreen.RestartRequested), this, nameof(OnMenuRestartRequested));
        }

        private void StartsNewRound()
        {
            _audioNewRoundStart.Play();
            _roundCounter.Increment();
            
            _player.RoundNumberChanged(_roundCounter.RoundsStarted);
            _hoardDirector.StartSpawningNewRoundZombies(_roundCounter.RoundsStarted);
        }

        private void EndSession()
        {
            _hoardDirector.Disable();
            int roundsFinished = _roundCounter.RoundsStarted - 1;
            SavedData savedData = GetNode<SavedData>("/root/SavedData");
            int currentBestRound = savedData.LevelHighestRound(Name);
            if (currentBestRound < roundsFinished)
            {
                savedData.UpdateLevelHighestRound(Name, roundsFinished);
            }
        }
    }
}

