using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class SimpleSpriteAnimator : MonoBehaviour
    {
        public Sprite[] frames;
        public float framesPerSecond = 10f;
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private Sprite[] _overrideFrames;
        private float _overrideFps;
        private float _animationTimer;
        private bool _isPlayingOverride;
        private Action _onOverrideComplete;

        void Start() => spriteRenderer = spriteRenderer != null 
            ? spriteRenderer 
            : GetComponent<SpriteRenderer>();

        void Update()
        {
            if (_isPlayingOverride)
                HandleOverrideAnimation();
            else
                HandleLoopingAnimation();
        }

        private void HandleOverrideAnimation()
        {
            _animationTimer += Time.deltaTime;
            int overrideIndex = (int)(_animationTimer * _overrideFps);

            if (overrideIndex < _overrideFrames.Length)
            {
                spriteRenderer.sprite = _overrideFrames[overrideIndex];
            }
            else
            {
                _isPlayingOverride = false;
                _onOverrideComplete?.Invoke();
                _onOverrideComplete = null;
                HandleLoopingAnimation(); // Seamlessly fall back to loop
            }
        }

        private void HandleLoopingAnimation()
        {
            if (frames == null || frames.Length == 0) return;

            int index = (int)(Time.time * framesPerSecond) % frames.Length;
            spriteRenderer.sprite = frames[index];
        }

        public void PlayOverride(Sprite[] newFrames, float fps, Action onComplete = null)
        {
            if (newFrames == null || newFrames.Length == 0) return;

            _overrideFrames = newFrames;
            _overrideFps = fps;
            _onOverrideComplete = onComplete;
            _animationTimer = 0f;
            _isPlayingOverride = true;
            
            spriteRenderer.sprite = _overrideFrames[0];
        }
    }
}
