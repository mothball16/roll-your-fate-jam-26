#nullable enable
using System;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    [Serializable]
    public class AttackEffect
    {
        public GameObject? VFX;
        public AudioClip? SFX;
        public float VFXLifetime = 0.25f;

        public void Play(Vector3 position, Vector3 direction)
        {
            if (VFX != null)
            {
                var vfxInstance = UnityEngine.Object.Instantiate(VFX, position, Quaternion.identity);

                if (direction != Vector3.zero)
                {
                    vfxInstance.transform.up = direction;
                }
            }

            if (SFX != null)
            {
                AudioSource.PlayClipAtPoint(SFX, position);
            }
        }
    }
}