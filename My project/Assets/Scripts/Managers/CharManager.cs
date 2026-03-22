using DangryGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Components;

namespace Assets.Scripts.Managers
{
    public enum CharType
    {
        Player,
        Skeleton,
        Orc,
        Archer,

        KnifeGuy,
        ShieldGuy,
        GunGuy
    }

    [Serializable]
    public struct CharLink
    {
        public CharType character;
        public GameObject prototype;
    }

    public class CharManager : MonoSingleton<CharManager>
    {
        [SerializeField]
        private List<CharLink> _charFactory;
        private List<CharacterController> _activeChars;

        public override void Awake()
        {
            _activeChars = new();
            base.Awake();
        }

        public GameObject SpawnChar(CharType character, Vector2 position)
        {
            var link = _charFactory.FirstOrDefault(c => c.character == character);
            if (link.prototype == null)
            {
                Debug.LogWarning($"Prototype for {character} not found in _charFactory.");
                return null;
            }

            var clone = Instantiate(link.prototype, position, Quaternion.identity);
            
            if (clone.TryGetComponent<CharacterController>(out var controller))
            {
                _activeChars.Add(controller);
            }

            if (clone.TryGetComponent<Humanoid>(out var humanoid))
            {
                humanoid.OnDeath += () => DestroyChar(clone);
            }

            return clone;
        }

        public void DestroyChar(GameObject charObject)
        {
            if (charObject.TryGetComponent<CharacterController>(out var controller))
            {
                _activeChars.Remove(controller);
            }

            Destroy(charObject);
        }
    }
}
