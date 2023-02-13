using Godot;
using HealthSystem;


namespace ZombieHoardGame.ZombieCharacter
{
    public class Blackboard : Reference
    {
        public Zombie Character;
        public NavigationAgent NavAgent;
        public AttackTrigger AttackTrigger;
        public Vector3 PlayerPosition; // Not updated every frame
        public Health Health;
        public HitBox AttackHitBox;
        public AudioStreamPlayer3D AudioStreamPlayer;
        public BoardedWindow TargetBoardedWindow;
        public bool IsInPlayerArea;
        public AnimationTree AnimTree;
        public AnimationNodeStateMachinePlayback AnimStateMachine;
        public VisibilityNotifier VisibilityNotifier;
        public GameGeneral.LOD.Switcher LODSwitcher;
    }
}

