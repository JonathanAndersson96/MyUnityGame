using System;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    public class SpriteWalkAnimator : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float framesPerSecond = 8f;

        private SpriteRenderer targetRenderer;
        private Sprite idleSprite;
        private Sprite[] walkFrames;
        private Sprite upwardIdleSprite;
        private Sprite[] upwardWalkFrames;
        private Sprite leftIdleSprite;
        private Sprite[] leftWalkFrames;
        private Sprite rightIdleSprite;
        private Sprite[] rightWalkFrames;
        private Sprite activeIdleSprite;
        private Sprite[] activeWalkFrames;
        private Vector3 spriteBasePosition;
        private float frameDuration;
        private float frameElapsed;
        private float walkTimeRemaining;
        private int currentFrame;

        public void Initialize(SpriteRenderer spriteRenderer, Sprite idle, Sprite[] frames, float animationFramesPerSecond = 8f)
        {
            if (spriteRenderer == null)
            {
                throw new ArgumentNullException(nameof(spriteRenderer));
            }

            if (idle == null)
            {
                throw new ArgumentNullException(nameof(idle));
            }

            if (frames == null)
            {
                throw new ArgumentNullException(nameof(frames));
            }

            if (animationFramesPerSecond <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(animationFramesPerSecond), "The animation frame rate must be greater than zero.");
            }

            ValidateFrames(frames, nameof(frames));

            targetRenderer = spriteRenderer;
            idleSprite = idle;
            walkFrames = new Sprite[frames.Length];
            Array.Copy(frames, walkFrames, frames.Length);
            activeIdleSprite = idleSprite;
            activeWalkFrames = walkFrames;
            framesPerSecond = animationFramesPerSecond;
            frameDuration = 1f / framesPerSecond;
            spriteBasePosition = targetRenderer.transform.localPosition;
            currentFrame = 0;
            frameElapsed = 0f;
            walkTimeRemaining = 0f;
            SetSprite(idleSprite);
        }

        public void SetUpwardWalkFrames(Sprite[] frames)
        {
            if (frames == null)
            {
                throw new ArgumentNullException(nameof(frames));
            }

            for (var i = 0; i < frames.Length; i++)
            {
                if (frames[i] == null)
                {
                    throw new ArgumentException($"Upward walk frame {i} is null.", nameof(frames));
                }
            }

            upwardWalkFrames = new Sprite[frames.Length];
            Array.Copy(frames, upwardWalkFrames, frames.Length);
            upwardIdleSprite = upwardWalkFrames.Length > 0 ? upwardWalkFrames[0] : null;
        }

        public void SetHorizontalWalkFrames(Sprite[] leftFrames, Sprite[] rightFrames)
        {
            if (leftFrames == null)
            {
                throw new ArgumentNullException(nameof(leftFrames));
            }

            if (rightFrames == null)
            {
                throw new ArgumentNullException(nameof(rightFrames));
            }

            ValidateFrames(leftFrames, nameof(leftFrames));
            ValidateFrames(rightFrames, nameof(rightFrames));

            leftWalkFrames = new Sprite[leftFrames.Length];
            Array.Copy(leftFrames, leftWalkFrames, leftFrames.Length);
            leftIdleSprite = leftWalkFrames.Length > 0 ? leftWalkFrames[0] : null;
            rightWalkFrames = new Sprite[rightFrames.Length];
            Array.Copy(rightFrames, rightWalkFrames, rightFrames.Length);
            rightIdleSprite = rightWalkFrames.Length > 0 ? rightWalkFrames[0] : null;
        }

        private static void ValidateFrames(Sprite[] frames, string parameterName)
        {
            for (var i = 0; i < frames.Length; i++)
            {
                if (frames[i] == null)
                {
                    throw new ArgumentException($"Walk frame {i} is null.", parameterName);
                }
            }
        }

        public float PlayWalk()
        {
            return PlayWalk(Vector2Int.down);
        }

        public float PlayWalk(Vector2Int direction)
        {
            if (targetRenderer == null)
            {
                throw new InvalidOperationException("Initialize the sprite walk animator before playing its walk animation.");
            }

            if (direction.x < 0 && leftIdleSprite != null)
            {
                activeIdleSprite = leftIdleSprite;
                activeWalkFrames = leftWalkFrames;
            }
            else if (direction.x > 0 && rightIdleSprite != null)
            {
                activeIdleSprite = rightIdleSprite;
                activeWalkFrames = rightWalkFrames;
            }
            else if (direction.y > 0 && upwardIdleSprite != null)
            {
                activeIdleSprite = upwardIdleSprite;
                activeWalkFrames = upwardWalkFrames;
            }
            else if (direction.y < 0)
            {
                activeIdleSprite = idleSprite;
                activeWalkFrames = walkFrames;
            }

            if (activeWalkFrames.Length == 0)
            {
                SetSprite(activeIdleSprite);
                return 0f;
            }

            currentFrame = 0;
            frameElapsed = 0f;
            walkTimeRemaining = activeWalkFrames.Length * frameDuration;
            SetSprite(activeWalkFrames[currentFrame]);
            return walkTimeRemaining;
        }

        private void Update()
        {
            if (walkTimeRemaining <= 0f)
            {
                return;
            }

            var elapsed = Time.deltaTime;
            walkTimeRemaining -= elapsed;
            frameElapsed += elapsed;

            while (frameElapsed >= frameDuration && walkTimeRemaining > 0f)
            {
                frameElapsed -= frameDuration;
                currentFrame = (currentFrame + 1) % activeWalkFrames.Length;
                SetSprite(activeWalkFrames[currentFrame]);
            }

            if (walkTimeRemaining <= 0f)
            {
                walkTimeRemaining = 0f;
                SetSprite(activeIdleSprite);
            }
        }

        private void SetSprite(Sprite sprite)
        {
            targetRenderer.sprite = sprite;
            var position = spriteBasePosition;
            position.y -= sprite.bounds.min.y;
            targetRenderer.transform.localPosition = position;
        }

        private void OnDisable()
        {
            walkTimeRemaining = 0f;
            if (targetRenderer != null && activeIdleSprite != null)
            {
                SetSprite(activeIdleSprite);
            }
        }
    }
}
