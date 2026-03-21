using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Components
{
    class Humanoid : MonoBehaviour
    {
        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        public float Health;
        public float MaxHealth;
        public void TakeDamage(float damage)
        {
            var origHealth = Health;
            Health = Math.Clamp(Health - damage, 0, MaxHealth);
            if (Health != origHealth)
            {
                OnHealthChanged?.Invoke(origHealth, Health);
            }
            if (Health <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        
    }
}
