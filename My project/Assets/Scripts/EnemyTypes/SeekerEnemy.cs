using Assets.Scripts.Agents;
using Assets.Scripts.Behaviors;
using NUnit.Framework.Internal.Filters;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum SeekerEnemyState
{
    Wandering,
    Seeking
}
class SeekerEnemy : EnemyAgent
{
    private SeekBehavior _seek;
    private WanderBehavior _wander;
    private Transform _target;

    [SerializeField]
    private SeekerEnemyState _state;


    public override void Awake()
    {
        base.Awake();
        _seek = new();
        _wander = new();
    }

    public override Vector3 CalculateSteering()
    {
        Vector3 steering = Vector3.zero;
        switch (_state)
        {
            case SeekerEnemyState.Seeking:
                _seek.Target = _target;
                steering += _seek.GetVelocity(_char, Time.deltaTime);
                break;
            case SeekerEnemyState.Wandering:
                steering += _wander.GetVelocity(_char, Time.deltaTime);
                break;
        }
        return steering;
    }
    public override void RunLogic()
    {
        switch (_state)
        {
            case SeekerEnemyState.Seeking:
                _target = FindClosestTarget();
                if (_target == null)
                    _state = SeekerEnemyState.Wandering;
                break;
            case SeekerEnemyState.Wandering:
                _target = FindClosestTarget();
                if (_target != null)
                    _state = SeekerEnemyState.Seeking;
                break;
        }
    }
}