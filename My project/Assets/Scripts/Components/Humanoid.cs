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

        public int XPOnDeath = 5;
        
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            UpdateColor();
        }

        public void TakeDamage(float damage)
        {
            if (Health <= 0) return; // Prevent multiple death events

            var origHealth = Health;
            Health = Math.Clamp(Health - damage, 0, MaxHealth);
            if (Health != origHealth)
            {
                OnHealthChanged?.Invoke(origHealth, Health);
                UpdateColor();
            }
            if (Health <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        private void UpdateColor()
        {
            if (_spriteRenderer != null && MaxHealth > 0)
            {
                float healthPercent = Health / MaxHealth;
                _spriteRenderer.color = new Color(1f, healthPercent, healthPercent);
            }
        }
        
    }
}
