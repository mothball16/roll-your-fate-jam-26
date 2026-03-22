using Assets.Scripts.Agents;
using Assets.Scripts.Behaviors;
using UnityEngine;

public enum PursueEnemyState
{
    Wandering,
    Pursuing
}

class PursueEnemy : EnemyAgent
{
    private PursueBehavior _pursue;
    private WanderBehavior _wander;
    private Transform _target;

    [SerializeField]
    private PursueEnemyState _state;

    public override void Awake()
    {
        base.Awake();
        _pursue = new PursueBehavior();
        _wander = new WanderBehavior();
    }

    public override Vector3 CalculateSteering()
    {
        Vector3 steering = Vector3.zero;
        switch (_state)
        {
            case PursueEnemyState.Pursuing:
                _pursue.Target = _target;
                steering += _pursue.GetVelocity(_char, Time.deltaTime);
                break;
            case PursueEnemyState.Wandering:
                steering += _wander.GetVelocity(_char, Time.deltaTime);
                break;
        }
        return steering;
    }

    public override void RunLogic()
    {
        switch (_state)
        {
            case PursueEnemyState.Pursuing:
                _target = FindClosestTarget();
                if (_target == null)
                    _state = PursueEnemyState.Wandering;
                break;
            case PursueEnemyState.Wandering:
                _target = FindClosestTarget();
                if (_target != null)
                    _state = PursueEnemyState.Pursuing;
                break;
        }
    }
}