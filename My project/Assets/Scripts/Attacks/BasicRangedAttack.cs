#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    [CreateAssetMenu(fileName = "New Ranged Attack", menuName = "Attacks/Basic Ranged Attack")]
    public class BasicRangedAttack : Attack
    {
        public Projectile ProjectilePrefab = null!;
        public float Damage = 10f;
        public float Speed = 20f;
        public AttackEffect FireEffect = new AttackEffect();

        public override void Execute(GameObject owner, Vector3 direction, GameObject? target = null)
        {
            FireEffect?.Play(owner.transform.position, direction);
            
            if (ProjectilePrefab != null)
            {
                var proj = UnityEngine.Object.Instantiate(ProjectilePrefab, owner.transform.position, Quaternion.identity);
                proj.Init(owner, Damage, Speed, direction);
            }
        }
    }
}
