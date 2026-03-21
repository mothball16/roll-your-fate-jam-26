#nullable enable
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        private float _damage;
        private GameObject? _owner;
        private TeamType _teamType;
        private bool _initialized;
        private Rigidbody2D _rb;
        public AttackEffect HitEffect = new();

        public void Awake()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }
        public void Init(GameObject owner, float damage, float speed, Vector3 direction)
        {
            _owner = owner;
            _damage = damage;

            _teamType = owner.TryGetComponent<Team>(out var ownerTeam)
                ? ownerTeam.team
                : TeamType.None;


            _rb.linearVelocity = (Vector2)direction * speed;
            transform.up = _rb.linearVelocity;

            _initialized = true;
            Destroy(gameObject, 10f);
        }


        public void Update()
        {
            if (!_initialized)
                return;

            if (_rb.linearVelocity.magnitude > 0)
            {
                transform.up = _rb.linearVelocity;
            }
        }


        private void OnTriggerEnter2D(Collider2D foreign)
        {
            GameObject victim = foreign.gameObject;

            if (victim == _owner 
                || !victim.TryGetComponent<Humanoid>(out var health)
                || !DetermineIfValidVictim(victim))
                return;

            health.TakeDamage(_damage);
            HitEffect?.Play(transform.position, _rb.linearVelocity.normalized);

            Destroy(gameObject);
        }

        private bool DetermineIfValidVictim(GameObject victim)
        {

            if (!victim.TryGetComponent<Team>(out var team))
                return true;

            return team.team != _teamType;
        }
    }
}