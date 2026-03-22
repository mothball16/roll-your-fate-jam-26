#nullable enable
using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Attacks.Types
{
    [CreateAssetMenu(fileName = "New Ranged Attack", menuName = "Attacks/Ranged Attack")]
    public class BasicRangedAttack : Attack
    {
        public Projectile ProjectilePrefab = null;
        public float Damage = 10f;
        public float Speed = 20f;
        public float SpeedVariance = 0f;
        public int ProjectileCount = 1;
        public float SpreadAngle = 15f;
        public float Inaccuracy = 0f;
        
        public float BurstDelay = 0f;
        public float CurveRate = 0f;
        public float CurveVariance = 0f;

        public bool LeadTarget = false;
        
        public AttackEffect FireEffect = new AttackEffect();

        public override void Execute(GameObject owner, Vector3 direction, GameObject? target = null)
        {
            if (ProjectilePrefab == null || ProjectileCount <= 0) return;

            if (BurstDelay > 0f && ProjectileCount > 1)
            {
                if (owner.TryGetComponent<MonoBehaviour>(out var runner))
                {
                    runner.StartCoroutine(FireBurstRoutine(owner, direction));
                    return;
                }
            }

            if (LeadTarget && target != null && target.TryGetComponent<CharacterController>(out var controller))
            {
                var originPos = owner.transform.position;
                var targetPos = target.transform.position;
                var distance = Vector3.Distance(originPos, targetPos);
                var secondsToTarget = distance / Speed;
                var newTargetPos = targetPos + controller.Velocity * secondsToTarget;
                var newDirection = (newTargetPos - originPos).normalized;

                direction = newDirection;
            }


            FireInstantly(owner, direction);
        }

        private void FireInstantly(GameObject owner, Vector3 direction)
        {
            FireEffect?.Play(owner.transform.position, direction); // Play VFX/SFX once for a shotgun blast
            
            float angleStep = ProjectileCount > 1 ? SpreadAngle / (ProjectileCount - 1) : 0f;
            float startAngle = ProjectileCount > 1 ? -SpreadAngle / 2f : 0f;

            for (int i = 0; i < ProjectileCount; i++)
            {
                SpawnProjectile(owner, direction, startAngle + (angleStep * i));
            }
        }

        private IEnumerator FireBurstRoutine(GameObject owner, Vector3 direction)
        {
            float angleStep = ProjectileCount > 1 ? SpreadAngle / (ProjectileCount - 1) : 0f;
            float startAngle = ProjectileCount > 1 ? -SpreadAngle / 2f : 0f;

            for (int i = 0; i < ProjectileCount; i++)
            {
                if (owner == null) yield break; // Safety check in case the attacker is destroyed mid-burst
                
                FireEffect?.Play(owner.transform.position, direction); // Play VFX/SFX for every shot in a burst
                SpawnProjectile(owner, direction, startAngle + (angleStep * i));

                if (i < ProjectileCount - 1)
                {
                    yield return new WaitForSeconds(BurstDelay);
                }
            }
        }

        private void SpawnProjectile(GameObject owner, Vector3 baseDirection, float angleOffset)
        {
            Vector3 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
            Vector3 finalDirection = ApplyInaccuracy(spreadDirection, Inaccuracy);
            
            float finalSpeed = Mathf.Max(0f, Speed + UnityEngine.Random.Range(-SpeedVariance, SpeedVariance));
            float finalCurve = CurveRate + UnityEngine.Random.Range(-CurveVariance, CurveVariance);

            var proj = Instantiate(ProjectilePrefab, owner.transform.position, Quaternion.identity);
            proj.Init(owner, Damage, finalSpeed, finalDirection.normalized);
            proj.CurveRate = finalCurve;
        }
    }
}
