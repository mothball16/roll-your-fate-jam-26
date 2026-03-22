using Assets.Scripts.Components;
using System;
using UnityEngine;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Attacks.Types
{
    [CreateAssetMenu(fileName = "New Sword Attack", menuName = "Attacks/Sword Attack")]
    public class SwordAttack : Attack
    {
        public float Damage = 10f;
        public float Range = 2.5f;
        public float AttackArcAngle = 90f;
        public LayerMask TargetLayer;
        public AttackEffect Effect = new AttackEffect();

        [Header("Animation")]
        public Sprite[] AttackFrames;
        public float AttackAnimFps = 12f;
        private bool DetermineIfValidVictim(GameObject owner, GameObject victim)
        {
            if (victim == owner)
                return false;

            TeamType teamType = owner.TryGetComponent<Team>(out var ownerTeam)
                ? ownerTeam.team 
                : TeamType.None;

            if (!victim.TryGetComponent<Team>(out var team))
                return true;

            return team.team != teamType;
        }

        public override void Execute(GameObject owner, Vector3 direction, GameObject? target = null)
        {

            Effect?.Play(owner.transform.position, direction);

            if (AttackFrames != null && AttackFrames.Length > 0 && owner.TryGetComponent<SimpleSpriteAnimator>(out var animator))
            {
                animator.PlayOverride(AttackFrames, AttackAnimFps);
            }

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(owner.transform.position, Range, TargetLayer);

            foreach (var hit in hitColliders)
            {
                if (!DetermineIfValidVictim(owner, hit.gameObject))
                    continue;
                Vector3 dirToTarget = (hit.transform.position - owner.transform.position).normalized;

                if (Vector3.Angle(direction, dirToTarget) < AttackArcAngle / 2f)
                {

                    if (hit.TryGetComponent<Humanoid>(out var health))
                    {
                        health.TakeDamage(Damage);
                    }

                    Debug.Log($"Sword hit: {hit.name}");
                }
            }
        }
    }
}