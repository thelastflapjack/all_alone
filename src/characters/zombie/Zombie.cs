using Godot;
using System;
using System.Collections.Generic;
using ZombieHoardGame.ZombieCharacter.FSMController;
using HealthSystem;
using GameGeneral;


namespace ZombieHoardGame.ZombieCharacter
{
    public class Zombie : KinematicBody
    {
        /// Signals ///
        [Signal]
        public delegate void Died(Zombie zombie, Node inflictor, Vector3 position, bool isCriticalKill);
        [Signal]
        public delegate void EnteredPlayerArea(Zombie zombie);
        [Signal]
        public delegate void PlayerPositionUpdateRequest(Zombie zombie);

        /// Exported Fields ///
        [Export]
        private List<AudioStream> _audioStreamsGroan = new List<AudioStream>();
        [Export]
        private List<AudioStream> _audioStreamsAttack = new List<AudioStream>();

        /// Properties - public, protected, private ///
        public bool IsInPlayerArea{ get { return _blackboard.IsInPlayerArea; }}
        public float HealthMultiplier{ set; private get; }

        /// Fields - protected or private ///
        private Blackboard _blackboard = new Blackboard();
        private Controller _controller;
        private GameGeneral.LOD.Switcher _lodSwitcher;
        private bool _isDead = false;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            CacheNodeReferences();
            _controller.Blackboard = _blackboard;

            _blackboard.Health.Connect(nameof(Health.Hurt), this, nameof(OnHealthHurt));
            _blackboard.VisibilityNotifier.Connect("screen_entered", this, nameof(OnVisibilityNotifierScreenEntered));
            _blackboard.VisibilityNotifier.Connect("screen_exited", this, nameof(OnVisibilityNotifierScreenExited));

            ApplyHealthMultiplier();
        }

        public override void _Process(float delta)
        {
            _controller.Update(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _controller.PhysicsUpdate(delta);
        }
        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////
        public void UpdatePlayerPosition(Vector3 position)
        {
            _blackboard.PlayerPosition = position;
        }

        public void SetTargetWindow(BoardedWindow window)
        {
            _blackboard.TargetBoardedWindow = window;
        }

        public void PlayRandomAudioGroan()
        {
            Random r = new Random();
            AudioStream randAttackAudio = _audioStreamsGroan[r.Next(0, _audioStreamsGroan.Count - 1)];
            _blackboard.AudioStreamPlayer.Stream = randAttackAudio;
            _blackboard.AudioStreamPlayer.Play();
        }

        public void PlayRandomAudioAttack()
        {
            Random r = new Random();
            AudioStream randAttackAudio = _audioStreamsAttack[r.Next(0, _audioStreamsAttack.Count - 1)];
            _blackboard.AudioStreamPlayer.Stream = randAttackAudio;
            _blackboard.AudioStreamPlayer.Play();
        }


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////
        private void OnHealthHurt(Node inflictor, int value, bool isCritical)
        {
            if (_blackboard.Health.Points <= 0 && !_isDead)
            {
                Die(inflictor, isCritical);
            }
        }

        private void OnVisibilityNotifierScreenEntered()
        {
            _lodSwitcher.Activate(true);
        }

        private void OnVisibilityNotifierScreenExited()
        {
            _lodSwitcher.Activate(false);
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////

        private void CacheNodeReferences()
        {
            _controller = GetNode<Controller>("Controller");
            _lodSwitcher = GetNode<GameGeneral.LOD.Switcher>("LODSwitcher");

            _blackboard.Character = this;
            _blackboard.NavAgent = GetNode<NavigationAgent>("NavigationAgent");
            _blackboard.AttackTrigger = GetNode<AttackTrigger>("AttackTrigger");
            _blackboard.Health = GetNode<Health>("Health");
            _blackboard.AttackHitBox = GetNode<HitBox>("HitBox");
            _blackboard.AudioStreamPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
            _blackboard.AnimTree = GetNode<AnimationTree>("AnimationTree");
            _blackboard.AnimStateMachine = (AnimationNodeStateMachinePlayback)_blackboard.AnimTree.Get("parameters/playback");
            _blackboard.VisibilityNotifier = GetNode<VisibilityNotifier>("VisibilityNotifier");
            _blackboard.LODSwitcher = _lodSwitcher;
        }

        private void ApplyHealthMultiplier()
        {
            _blackboard.Health.PointsMax = Mathf.RoundToInt(_blackboard.Health.PointsMax * HealthMultiplier);
            _blackboard.Health.RestoreToMax();
        }

        private void RemoveTargetWindowBoard() // Only to be used in the attack window animation
        {
            BoardedWindow window = _blackboard.TargetBoardedWindow;
            window.ZombieRemoveBoard();
        }

        private void Die(Node killer, bool wasKilledByCriticalHit)
        {
            _isDead = true;
            EmitSignal(nameof(Died), this, killer, GlobalTranslation, wasKilledByCriticalHit);
            GetNode<CollisionShape>("CollisionShape").Disabled = true;

            if (wasKilledByCriticalHit)
            {
                // Could spawn with a higher level script similar to the BulletSpawner?
                GetNode<CompositeEffect>("Body/Armature/Skeleton/BoneAttachHead/HeadshotDeathEffect").Play();
                GetNode<MeshInstance>("Body/Armature/Skeleton/NeckStump").Show();
                GetNode<MeshInstance>("Body/Armature/Skeleton/Head").Hide();
            }

            _controller.Die();
        }
    }
}

