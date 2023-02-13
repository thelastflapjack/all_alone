using Godot;
using HealthSystem;


namespace Weapons
{
    public class Bullet : Spatial
    {
        public Vector3 Velocity { set; get; }
        public int Damage { set; get; }
        public Vector3 SpawnVector { set; get; }
        public float MaxRange { set; get; }
        public Vector3 DistanceVector { set; get; }
        public Node Origin {set; get; }


        private Vector3 _frameMoveVector;

        private float _maxRangeSquared;
        private RayCast _rayCast;
        private int _penetrationPointsInitial = 100;
        private int _penetrationPoints;


        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            _maxRangeSquared = Mathf.Pow(MaxRange, 2);
            _penetrationPoints = _penetrationPointsInitial;

            _rayCast = GetNode<RayCast>("RayCast");

            GlobalTranslation = SpawnVector;
        }

        public override void _PhysicsProcess(float delta)
        {
            _frameMoveVector = Velocity * delta;
            _rayCast.ClearExceptions();
            _rayCast.CastTo = _frameMoveVector;
            _rayCast.ForceRaycastUpdate();

            while (_rayCast.IsColliding())
            {
                Godot.Object collider = _rayCast.GetCollider();
                if (collider is HurtBox)
                {
                    CollideWithHurtBox((HurtBox)collider, delta);
                    _rayCast.AddException(collider);
                    _rayCast.ForceRaycastUpdate();
                }
                else
                {
                    _penetrationPoints = 0;
                    Destroy();
                }
            }
            
            if (_penetrationPoints > 0)
            {
                Move(delta);
            }
        }


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
        private void Move(float delta)
        {
            GlobalTranslation += _frameMoveVector;
            DistanceVector += _frameMoveVector;
            if (DistanceVector.LengthSquared() > _maxRangeSquared)
            {
                QueueFree();
            }
        }

        private void CollideWithHurtBox(HurtBox box, float delta)
        {
            float penetrationMultiplier = (float)_penetrationPoints / (float)_penetrationPointsInitial;
            int effectiveDamage = Mathf.RoundToInt(Damage * penetrationMultiplier);
            box.ApplyDamage(effectiveDamage, Origin);
            _penetrationPoints -= box.PenetrationResistance;
            if (_penetrationPoints <= 0)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            GlobalTranslation = _rayCast.GetCollisionPoint();
            _rayCast.Enabled = false;
            SetPhysicsProcess(false);
            QueueFree();
        }
    }
}

