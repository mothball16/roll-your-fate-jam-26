using Assets.Scripts.Interfaces;
using System;
using UnityEngine;


public enum LookType
{
    Manual,
    AimFront,
    LeftRight,
}

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] protected Vector3 _acceleration;
    [SerializeField] protected Vector3 _velocity;
    [SerializeField] protected float _snappiness = 5f;
    [SerializeField] protected float _maxSpeed = 20f;
    [SerializeField] protected float _damping = 5f;
    public Vector3 SteeringForce = Vector3.zero;

    public LookType LookType = LookType.AimFront;
    public Quaternion Rotation = Quaternion.identity;
    public Vector3 Position => transform.position;
    public Vector3 Accel => _acceleration;
    public Vector3 Velocity => _velocity;
    public float MaxSpeed => _maxSpeed;

    private Rigidbody2D _rb;

    public virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Update()
    {
        // Intentionally left empty for child overrides.
        // Physics calculations must be handled in FixedUpdate.
    }

    public virtual void FixedUpdate()
    {
        _acceleration = Vector3.zero;
        _acceleration += SteeringForce;
        _velocity += _acceleration * Time.fixedDeltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

        // slowdown stuff
        _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.fixedDeltaTime * _damping);
        if (_velocity.magnitude < 0.01)
        {
            _velocity = Vector3.zero;
        }

        // calc and set rotation
        switch (LookType)
        {
            case LookType.Manual:
                // nothing much to be implemented
                break;
            case LookType.LeftRight:
                if(_velocity.x < 0)
                {
                    Rotation = Quaternion.Euler(0, 0, 0);
                }
                else if(_velocity.x > 0)
                {
                    Rotation = Quaternion.Euler(0, 180, 0);
                }
                break;
            case LookType.AimFront:
                if (_velocity != Vector3.zero)
                {
                    float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
                    Rotation = Quaternion.Euler(0, 0, angle - 90);
                }
                break;
            default:
                throw new NotImplementedException();
        }

        _rb.linearVelocity = _velocity; 
        _rb.MoveRotation(Rotation.eulerAngles.z);
    }
    protected virtual Vector3 GetSteerForce(IBehavior behavior, float weight = 1)
    {
        return _snappiness * weight * behavior.GetVelocity(this, Time.deltaTime);

    }

}
