#nullable enable
using System;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    [CreateAssetMenu(fileName = "New Volley Ranged Attack", menuName = "Attacks/Volley Ranged Attack")]
    public class VolleyRangedAttack : Attack
    {
        public Projectile ProjectilePrefab = null!;
        public float Damage = 10f;
        public float Speed = 20f;
        public float SpeedVariance = 0f;
        public int ProjectileCount = 3;
        public float SpreadAngle = 45f;
        public float Inaccuracy = 0f;
        
        public AttackEffect FireEffect = new AttackEffect();

        public override void Execute(GameObject owner, Vector3 direction, GameObject? target = null)
        {
            FireEffect?.Play(owner.transform.position, direction);
            
            if (ProjectilePrefab != null && ProjectileCount > 0)
            {
                // Calculate the angle step between each projectile
                float angleStep = ProjectileCount > 1 ? SpreadAngle / (ProjectileCount - 1) : 0f;
                float startAngle = ProjectileCount > 1 ? -SpreadAngle / 2f : 0f;

                for (int i = 0; i < ProjectileCount; i++)
                {
                    float currentAngle = startAngle + (angleStep * i);
                    Vector3 spreadDirection = Quaternion.Euler(0, 0, currentAngle) * direction;
                    Vector3 finalDirection = ApplyInaccuracy(spreadDirection, Inaccuracy);
                    float finalSpeed = Mathf.Max(0f, Speed + UnityEngine.Random.Range(-SpeedVariance, SpeedVariance));

                    var proj = Instantiate(ProjectilePrefab, owner.transform.position, Quaternion.identity);
                    proj.Init(owner, Damage, finalSpeed, finalDirection.normalized);
                }
            }
        }
    }
}