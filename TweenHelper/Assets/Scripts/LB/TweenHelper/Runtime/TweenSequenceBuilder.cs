using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Fluent builder for creating readable multi-step animations.
    /// Provides a step-by-step approach to building complex sequences.
    /// </summary>
    public class TweenSequenceBuilder
    {
        private readonly Sequence _sequence;
        private readonly GameObject _defaultTarget;
        private readonly List<Tween> _steps = new List<Tween>();
        private bool _hasBeenBuilt = false;
        
        /// <summary>
        /// Creates a new sequence builder.
        /// </summary>
        /// <param name="defaultTarget">The default target GameObject for linking.</param>
        public TweenSequenceBuilder(GameObject defaultTarget = null)
        {
            _sequence = DOTween.Sequence();
            _defaultTarget = defaultTarget;
        }
        
        /// <summary>
        /// Adds a movement step to the sequence.
        /// </summary>
        /// <param name="transform">The Transform to move.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Move(Transform transform, Vector3 targetPosition, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            var tween = TweenHelper.MoveTo(transform, targetPosition, duration, options);
            if (tween != null)
            {
                _sequence.Append(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a rotation step to the sequence.
        /// </summary>
        /// <param name="transform">The Transform to rotate.</param>
        /// <param name="targetRotation">The target rotation (in Euler angles).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Rotate(Transform transform, Vector3 targetRotation, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            var tween = TweenHelper.RotateTo(transform, targetRotation, duration, options);
            if (tween != null)
            {
                _sequence.Append(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a scaling step to the sequence.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="targetScale">The target scale.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Scale(Transform transform, Vector3 targetScale, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            var tween = TweenHelper.ScaleTo(transform, targetScale, duration, options);
            if (tween != null)
            {
                _sequence.Append(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a scaling step with uniform scale to the sequence.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="targetScale">The uniform target scale value.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Scale(Transform transform, float targetScale, float? duration = null, TweenOptions options = default)
        {
            return Scale(transform, Vector3.one * targetScale, duration, options);
        }
        
        /// <summary>
        /// Adds a fade step to the sequence.
        /// </summary>
        /// <param name="target">The target component to fade (CanvasGroup, SpriteRenderer, Image, Text).</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Fade(Component target, float targetAlpha, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            Tween tween = null;
            
            if (target is CanvasGroup canvasGroup)
            {
                tween = TweenHelper.FadeTo(canvasGroup, targetAlpha, duration, options);
            }
            else if (target is SpriteRenderer spriteRenderer)
            {
                tween = TweenHelper.FadeTo(spriteRenderer, targetAlpha, duration, options);
            }
            else if (target is UnityEngine.UI.Image image)
            {
                tween = TweenHelper.FadeTo(image, targetAlpha, duration, options);
            }
            else if (target is UnityEngine.UI.Text text)
            {
                tween = TweenHelper.FadeTo(text, targetAlpha, duration, options);
            }
            else
            {
                Debug.LogWarning($"TweenSequenceBuilder: Unsupported fade target type '{target?.GetType().Name}'. Supported types: CanvasGroup, SpriteRenderer, Image, Text.");
                return this;
            }
            
            if (tween != null)
            {
                _sequence.Append(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a preset animation step to the sequence.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Preset(string presetName, GameObject target, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            var tween = TweenHelper.PlayPreset(presetName, target, duration, options);
            if (tween != null)
            {
                _sequence.Append(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a delay step to the sequence.
        /// </summary>
        /// <param name="duration">The duration of the delay.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Delay(float duration)
        {
            ThrowIfBuilt();
            _sequence.AppendInterval(duration);
            return this;
        }
        
        /// <summary>
        /// Adds a callback step to the sequence.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Call(Action callback)
        {
            ThrowIfBuilt();
            _sequence.AppendCallback(() => callback?.Invoke());
            return this;
        }
        
        /// <summary>
        /// Adds a custom tween step to the sequence.
        /// </summary>
        /// <param name="tween">The tween to add.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Then(Tween tween)
        {
            ThrowIfBuilt();
            if (tween != null)
            {
                _sequence.Append(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Starts a parallel group where subsequent steps will run simultaneously.
        /// Call EndParallel() to close the parallel group.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder BeginParallel()
        {
            ThrowIfBuilt();
            // Implementation note: DoTween sequences don't have explicit parallel grouping,
            // so we'll need to collect tweens and Join them
            Debug.LogWarning("TweenSequenceBuilder: Parallel groups not yet implemented. Use JoinPrevious() for parallel animations.");
            return this;
        }
        
        /// <summary>
        /// Adds a movement step that runs in parallel with the previous step.
        /// </summary>
        /// <param name="transform">The Transform to move.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder JoinMove(Transform transform, Vector3 targetPosition, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            var tween = TweenHelper.MoveTo(transform, targetPosition, duration, options);
            if (tween != null)
            {
                _sequence.Join(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a scaling step that runs in parallel with the previous step.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="targetScale">The target scale.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder JoinScale(Transform transform, Vector3 targetScale, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            var tween = TweenHelper.ScaleTo(transform, targetScale, duration, options);
            if (tween != null)
            {
                _sequence.Join(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a fade step that runs in parallel with the previous step.
        /// </summary>
        /// <param name="target">The target component to fade.</param>
        /// <param name="targetAlpha">The target alpha value (0-1).</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder JoinFade(Component target, float targetAlpha, float? duration = null, TweenOptions options = default)
        {
            ThrowIfBuilt();
            Tween tween = null;
            
            if (target is CanvasGroup canvasGroup)
            {
                tween = TweenHelper.FadeTo(canvasGroup, targetAlpha, duration, options);
            }
            else if (target is SpriteRenderer spriteRenderer)
            {
                tween = TweenHelper.FadeTo(spriteRenderer, targetAlpha, duration, options);
            }
            else if (target is UnityEngine.UI.Image image)
            {
                tween = TweenHelper.FadeTo(image, targetAlpha, duration, options);
            }
            else if (target is UnityEngine.UI.Text text)
            {
                tween = TweenHelper.FadeTo(text, targetAlpha, duration, options);
            }
            
            if (tween != null)
            {
                _sequence.Join(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Adds a custom tween that runs in parallel with the previous step.
        /// </summary>
        /// <param name="tween">The tween to join.</param>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Join(Tween tween)
        {
            ThrowIfBuilt();
            if (tween != null)
            {
                _sequence.Join(tween);
                _steps.Add(tween);
            }
            return this;
        }
        
        /// <summary>
        /// Builds and returns the completed sequence.
        /// </summary>
        /// <param name="options">Additional options to apply to the sequence.</param>
        /// <returns>The configured sequence.</returns>
        public Sequence Build(TweenOptions options = default)
        {
            if (_hasBeenBuilt)
            {
                Debug.LogWarning("TweenSequenceBuilder: Sequence has already been built. Create a new builder for additional sequences.");
                return _sequence;
            }
            
            _hasBeenBuilt = true;
            
            // Configure the sequence with options and link to default target
            DoTweenIntegration.ConfigureSequence(_sequence, _defaultTarget, options);
            
            return _sequence;
        }
        
        /// <summary>
        /// Builds the sequence and immediately plays it.
        /// </summary>
        /// <param name="options">Additional options to apply to the sequence.</param>
        /// <returns>The configured and playing sequence.</returns>
        public Sequence Play(TweenOptions options = default)
        {
            var sequence = Build(options);
            sequence.Play();
            return sequence;
        }
        
        private void ThrowIfBuilt()
        {
            if (_hasBeenBuilt)
            {
                throw new InvalidOperationException("TweenSequenceBuilder: Cannot modify sequence after it has been built. Create a new builder.");
            }
        }
        
        /// <summary>
        /// Gets the number of steps currently in the sequence.
        /// </summary>
        public int StepCount => _steps.Count;
        
        /// <summary>
        /// Clears all steps from the sequence builder.
        /// Can only be called before the sequence is built.
        /// </summary>
        /// <returns>This builder for chaining.</returns>
        public TweenSequenceBuilder Clear()
        {
            ThrowIfBuilt();
            _steps.Clear();
            _sequence.Kill();
            return this;
        }
    }
}
