using Assets.Scripts.Agents;
using Assets.Scripts.Behaviors;
using UnityEngine;

public enum StrafingEnemyState
{
    Wandering,
    Combat
}

class StrafingEnemy : EnemyAgent
{
    [Header("Strafing Config")]
    public float MaintainDistance = 7f;
    public float DistanceTolerance = 1f;
    public float StrafeFrequency = 1.5f;
    public float StrafeWeight = 1f;

    private WanderBehavior _wander;
    private Transform _target;

    [SerializeField]
    private StrafingEnemyState _state;

    private float _randomTimeOffset;

    public override void Awake()
    {
        base.Awake();
        _wander = new WanderBehavior();
        
        // Offset prevents multiple enemies from strafing in perfect sync
        _randomTimeOffset = Random.Range(0f, 100f); 
    }

    public override Vector3 CalculateSteering()
    {
        Vector3 steering = Vector3.zero;
        switch (_state)
        {
            case StrafingEnemyState.Combat:
                if (_target == null) return Vector3.zero;

                Vector3 toTarget = _target.position - _char.Position;
                float distance = toTarget.magnitude;
                Vector3 dirToTarget = toTarget.normalized;

                Vector3 desiredVelocity = Vector3.zero;
                
                // 1. Maintain Distance (Seek if too far, Flee if too close)
                if (distance > MaintainDistance + DistanceTolerance)
                {
                    desiredVelocity += dirToTarget; 
                }
                else if (distance < MaintainDistance - DistanceTolerance)
                {
                    desiredVelocity -= dirToTarget; 
                }

                // 2. Strafe (Move perpendicular to the target)
                // In 2D, the perpendicular vector to (x, y) is (-y, x)
                Vector3 rightOfTarget = new Vector3(-dirToTarget.y, dirToTarget.x, 0);
                float strafeDirection = Mathf.Sin((Time.time + _randomTimeOffset) * StrafeFrequency);
                
                desiredVelocity += rightOfTarget * (strafeDirection * StrafeWeight);

                // 3. Convert desired velocity to a steering force
                desiredVelocity = desiredVelocity.normalized * _char.MaxSpeed;
                steering = desiredVelocity - _char.Velocity;
                break;

            case StrafingEnemyState.Wandering:
                steering += _wander.GetVelocity(_char, Time.deltaTime);
                break;
        }
        return steering;
    }

    public override void RunLogic()
    {
        _target = FindClosestTarget();
        _state = _target != null ? StrafingEnemyState.Combat : StrafingEnemyState.Wandering;
    }
}