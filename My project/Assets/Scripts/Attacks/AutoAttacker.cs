using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

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
            var dir = state.AttackDef.AimType switch
            {
                AimType.Direction => _char.Rotation * Vector3.up,
                AimType.North => Vector3.up,
                AimType.Mouse => (MouseUtils.MouseToWorldPoint() - _char.transform.position).normalized,
                _ => Vector3.forward,
            };
            state.AttackDef.Execute(gameObject, dir);
            state.CurCooldown = state.AttackDef.Cooldown;
        }
    }
}
