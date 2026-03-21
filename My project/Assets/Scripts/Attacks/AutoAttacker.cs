using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Assets.Scripts.Components;

namespace Assets.Scripts.Attacks
{
    [Serializable]
    public class AttackState
    {
        public Attack AttackDef;
        public float CurCooldown;
    }

    [RequireComponent(typeof(CharacterController))]
    public class AutoAttacker : MonoBehaviour
    {
        public float DetectionRadius = 15f;

        public List<AttackState> Attacks;
        private CharacterController _char;

        public void Awake()
        {
            _char = gameObject.GetComponent<CharacterController>();
        }
        public void Update()
        {
            foreach (var state in Attacks)
            {
                state.CurCooldown -= Time.deltaTime;
                
                if(state.CurCooldown <= 0f)
                {
                    RunAttack(state);
                }
            }
        }
        private void RunAttack(AttackState state)
        {
            if (state.AttackDef == null) return;

            Vector3 dir = Vector3.up;
            GameObject target = null;

            switch (state.AttackDef.AimType)
            {
                case AimType.Closest:
                    target = GetClosestEnemy(state.AttackDef.DetectionRadius);
                    if (target == null) return;
                    dir = (target.transform.position - transform.position).normalized;
                    break;
                case AimType.RandomInRange:
                    target = GetRandomEnemy(state.AttackDef.DetectionRadius); 
                    if (target == null) return;
                    dir = (target.transform.position - transform.position).normalized;
                    break;
                case AimType.Direction:
                    dir = _char.Rotation * Vector3.up;
                    break;
                case AimType.North:
                    dir = Vector3.up;
                    break;
                case AimType.Mouse:
                    dir = (MouseUtils.MouseToWorldPoint() - transform.position).normalized;
                    break;
                default:
                    dir = Vector3.up;
                    break;
            }

            state.AttackDef.Execute(gameObject, dir, target);
            state.CurCooldown = state.AttackDef.Cooldown;
        }

        private GameObject GetClosestEnemy(float radius)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            GameObject closest = null;
            float minDistance = float.MaxValue;

            foreach (var col in colliders)
            {
                if (!IsValidTarget(col.gameObject)) continue;

                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = col.gameObject;
                }
            }
            return closest;
        }

        private GameObject GetRandomEnemy(float radius)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            var validEnemies = new System.Collections.Generic.List<GameObject>();

            foreach (var col in colliders)
            {
                if (IsValidTarget(col.gameObject))
                {
                    validEnemies.Add(col.gameObject);
                }
            }

            if (validEnemies.Count > 0)
            {
                return validEnemies[UnityEngine.Random.Range(0, validEnemies.Count)];
            }
            return null;
        }

        private bool IsValidTarget(GameObject victim)
        {
            if (victim == gameObject) return false;
            
            // Require targets to have a Humanoid component 
            if (!victim.TryGetComponent<Humanoid>(out _)) return false;

            TeamType myTeam = TryGetComponent<Team>(out var myTeamComp) ? myTeamComp.team : TeamType.None;
            
            if (!victim.TryGetComponent<Team>(out var victimTeam))
                return true; // No team = valid target

            return victimTeam.team != myTeam;
        }
    }
}
