#nullable enable
using Assets;
using System;
using UnityEngine;

namespace Assets.Scripts.Attacks.Types
{
    [CreateAssetMenu(fileName = "New Volley Ranged Attack", menuName = "Attacks/Volley Ranged Attack")]
    public class VolleyRangedAttack : Attack
    {
        public Projectile ProjectilePrefab = null!;
        public float Damage = 10f;
        public float Speed = 20f;
        public int ProjectileCount = 3;
        public float SpreadAngle = 45f;
        public AttackEffect FireEffect = new AttackEffect();

        public override void Execute(GameObject owner, Vector3 direction, GameObject? target = null)
        {
            FireEffect?.Play(owner.transform.position, direction);
            
            if (ProjectilePrefab != null && ProjectileCount > 0)
            {
                float angleStep = ProjectileCount > 1 ? SpreadAngle / (ProjectileCount - 1) : 0f;
                float startAngle = ProjectileCount > 1 ? -SpreadAngle / 2f : 0f;

                for (int i = 0; i < ProjectileCount; i++)
                {
                    float currentAngle = startAngle + (angleStep * i);
                    Vector3 spreadDirection = Quaternion.Euler(0, 0, currentAngle) * direction;

                    var proj = Instantiate(ProjectilePrefab, owner.transform.position, Quaternion.identity);
                    proj.Init(owner, Damage, Speed, spreadDirection.normalized);
                }
            }
        }
    }
}