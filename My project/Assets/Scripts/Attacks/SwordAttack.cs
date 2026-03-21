﻿using Assets.Scripts.Components;
using System;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    [CreateAssetMenu(fileName = "New Sword Attack", menuName = "Attacks/Sword Attack")]
    public class SwordAttack : Attack
    {
        public float Damage = 10f;
        public float Range = 2.5f;
        public float AttackArcAngle = 90f;
        public LayerMask TargetLayer;
        public AttackEffect Effect = new AttackEffect();

        public override void Execute(GameObject owner, Vector3 direction, GameObject? target = null)
        {
            Effect?.Play(owner.transform.position, direction);

            Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, Range, TargetLayer);

            foreach (var hit in hitColliders)
            {
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