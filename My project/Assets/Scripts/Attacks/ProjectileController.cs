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
        public AttackEffect HitEffect = new AttackEffect();

        public void Init(GameObject owner, float damage, float speed, Vector3 direction)
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _owner = owner;
            _damage = damage;

            _teamType = owner.TryGetComponent<Team>(out var ownerTeam)
                ? ownerTeam.team
                : TeamType.None;


            _rb.linearVelocity = (Vector2)direction * speed;
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


        // Must be 2D to match Rigidbody2D
        private void OnTriggerEnter2D(Collider2D foreign)
        {
            GameObject victim = foreign.gameObject;

            if (victim == _owner || !DetermineIfValidVictim(victim))
                return;

            if (victim.TryGetComponent<Humanoid>(out var health))
            {
                health.TakeDamage(_damage);
            }

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