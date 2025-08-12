using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Central configuration hub for DoTween integration.
    /// Applies defaults and per-call options to tweens and sequences, handles GameObject linking for automatic cleanup.
    /// </summary>
    public static class DoTweenIntegration
    {
        /// <summary>
        /// Configures a tween with the specified settings and options.
        /// </summary>
        /// <param name="tween">The tween to configure.</param>
        /// <param name="target">The GameObject target for linking (optional).</param>
        /// <param name="duration">The duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured tween.</returns>
        public static T ConfigureTween<T>(T tween, GameObject target = null, float? duration = null, TweenOptions options = default) where T : Tween
        {
            if (tween == null)
            {
                Debug.LogError("DoTweenIntegration: Cannot configure null tween.");
                return null;
            }
            
            var settings = TweenHelperSettings.Instance;
            
            // Note: Duration is typically set when creating the tween
            // DoTween doesn't have SetDuration - duration is set at creation time
            
            // Apply ease (options override settings)
            var easeToUse = options.Ease ?? settings.DefaultEase;
            tween.SetEase(easeToUse);
            
            // Apply delay (options override settings, default to settings if not in options)
            var delayToUse = options.Delay ?? (options.Delay.HasValue ? 0f : settings.DefaultDelay);
            if (delayToUse > 0f)
            {
                tween.SetDelay(delayToUse);
            }
            
            // Apply update type
            var updateTypeToUse = options.UpdateType ?? settings.DefaultUpdateType;
            tween.SetUpdate(updateTypeToUse);
            
            // Apply unscaled time
            var unscaledTimeToUse = options.UnscaledTime ?? settings.DefaultUnscaledTime;
            if (unscaledTimeToUse)
            {
                tween.SetUpdate(updateTypeToUse, true);
            }
            
            // Apply loops
            if (options.Loops.HasValue)
            {
                var loopTypeToUse = options.LoopType ?? LoopType.Restart;
                tween.SetLoops(options.Loops.Value, loopTypeToUse);
            }
            
            // Apply speed-based
            if (options.SpeedBased.HasValue && options.SpeedBased.Value)
            {
                tween.SetSpeedBased();
            }
            
            // Apply identifier
            if (!string.IsNullOrEmpty(options.Id))
            {
                tween.SetId(options.Id);
            }
            
            // Link to GameObject for automatic cleanup
            if (target != null)
            {
                LinkTweenToGameObject(tween, target);
            }
            
            return tween;
        }
        
        /// <summary>
        /// Configures a sequence with the specified settings and options.
        /// </summary>
        /// <param name="sequence">The sequence to configure.</param>
        /// <param name="target">The GameObject target for linking (optional).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured sequence.</returns>
        public static Sequence ConfigureSequence(Sequence sequence, GameObject target = null, TweenOptions options = default)
        {
            if (sequence == null)
            {
                Debug.LogError("DoTweenIntegration: Cannot configure null sequence.");
                return null;
            }
            
            var settings = TweenHelperSettings.Instance;
            
            // Apply ease (options override settings)
            var easeToUse = options.Ease ?? settings.DefaultEase;
            sequence.SetEase(easeToUse);
            
            // Apply delay
            var delayToUse = options.Delay ?? (options.Delay.HasValue ? 0f : settings.DefaultDelay);
            if (delayToUse > 0f)
            {
                sequence.SetDelay(delayToUse);
            }
            
            // Apply update type
            var updateTypeToUse = options.UpdateType ?? settings.DefaultUpdateType;
            var unscaledTimeToUse = options.UnscaledTime ?? settings.DefaultUnscaledTime;
            sequence.SetUpdate(updateTypeToUse, unscaledTimeToUse);
            
            // Apply loops
            if (options.Loops.HasValue)
            {
                var loopTypeToUse = options.LoopType ?? LoopType.Restart;
                sequence.SetLoops(options.Loops.Value, loopTypeToUse);
            }
            
            // Apply speed-based
            if (options.SpeedBased.HasValue && options.SpeedBased.Value)
            {
                sequence.SetSpeedBased();
            }
            
            // Apply identifier
            if (!string.IsNullOrEmpty(options.Id))
            {
                sequence.SetId(options.Id);
            }
            
            // Link to GameObject for automatic cleanup
            if (target != null)
            {
                LinkTweenToGameObject(sequence, target);
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Links a tween to a GameObject for automatic cleanup when the GameObject is destroyed.
        /// </summary>
        /// <param name="tween">The tween to link.</param>
        /// <param name="target">The GameObject to link to.</param>
        public static void LinkTweenToGameObject(Tween tween, GameObject target)
        {
            if (tween == null || target == null)
            {
                return;
            }
            
            try
            {
                // Use DoTween's native linking if available
                tween.SetTarget(target);
                tween.SetLink(target);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"DoTweenIntegration: Failed to link tween to GameObject '{target.name}'. Using fallback. {ex.Message}");
                
                // Fallback: Use our lifecycle tracker component
                var lifecycleTracker = target.GetComponent<TweenLifecycleTracker>();
                if (lifecycleTracker == null)
                {
                    lifecycleTracker = target.AddComponent<TweenLifecycleTracker>();
                }
                lifecycleTracker.RegisterTween(tween);
            }
        }
        
        /// <summary>
        /// Applies snapping to a Vector3 tween if required by settings or options.
        /// </summary>
        /// <param name="tween">The Vector3 tween to potentially snap.</param>
        /// <param name="options">The options to check for snapping preference.</param>
        /// <returns>The tween with snapping applied if needed.</returns>
        public static TweenerCore<Vector3, Vector3, VectorOptions> ApplySnapping(
            TweenerCore<Vector3, Vector3, VectorOptions> tween, 
            TweenOptions options = default)
        {
            if (tween == null) return null;
            
            var settings = TweenHelperSettings.Instance;
            var shouldSnap = options.Snapping ?? settings.DefaultSnapping;
            
            if (shouldSnap)
            {
                tween.SetOptions(true); // Enable snapping
            }
            
            return tween;
        }
        
        /// <summary>
        /// Validates that a GameObject target is suitable for tweening.
        /// </summary>
        /// <param name="target">The GameObject to validate.</param>
        /// <param name="operationName">The name of the operation for error reporting.</param>
        /// <returns>True if the target is valid, false otherwise.</returns>
        public static bool ValidateTarget(GameObject target, string operationName)
        {
            if (target == null)
            {
                Debug.LogError($"DoTweenIntegration: Cannot perform '{operationName}' on null GameObject.");
                return false;
            }
            
            if (!target.activeInHierarchy)
            {
                Debug.LogWarning($"DoTweenIntegration: GameObject '{target.name}' is not active in hierarchy for '{operationName}'.");
                // Don't return false - inactive objects can still be tweened
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates that a Transform target is suitable for tweening.
        /// </summary>
        /// <param name="target">The Transform to validate.</param>
        /// <param name="operationName">The name of the operation for error reporting.</param>
        /// <returns>True if the target is valid, false otherwise.</returns>
        public static bool ValidateTarget(Transform target, string operationName)
        {
            if (target == null)
            {
                Debug.LogError($"DoTweenIntegration: Cannot perform '{operationName}' on null Transform.");
                return false;
            }
            
            return ValidateTarget(target.gameObject, operationName);
        }
        
        /// <summary>
        /// Validates that a Component target is suitable for tweening.
        /// </summary>
        /// <param name="target">The Component to validate.</param>
        /// <param name="operationName">The name of the operation for error reporting.</param>
        /// <returns>True if the target is valid, false otherwise.</returns>
        public static bool ValidateTarget(Component target, string operationName)
        {
            if (target == null)
            {
                Debug.LogError($"DoTweenIntegration: Cannot perform '{operationName}' on null Component.");
                return false;
            }
            
            return ValidateTarget(target.gameObject, operationName);
        }
        
        /// <summary>
        /// Creates a configured tween for moving a Transform to a target position.
        /// </summary>
        /// <param name="transform">The Transform to move.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="duration">The duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured position tween.</returns>
        public static TweenerCore<Vector3, Vector3, VectorOptions> CreateMoveTween(
            Transform transform, 
            Vector3 targetPosition, 
            float? duration = null, 
            TweenOptions options = default)
        {
            if (!ValidateTarget(transform, "Move")) return null;
            
            var tween = transform.DOMove(targetPosition, duration ?? TweenHelperSettings.Instance.DefaultDuration);
            ConfigureTween(tween, transform.gameObject, duration, options);
            return ApplySnapping(tween, options);
        }
        
        /// <summary>
        /// Creates a configured tween for rotating a Transform to a target rotation.
        /// </summary>
        /// <param name="transform">The Transform to rotate.</param>
        /// <param name="targetRotation">The target rotation (in Euler angles).</param>
        /// <param name="duration">The duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured rotation tween.</returns>
        public static TweenerCore<Quaternion, Vector3, QuaternionOptions> CreateRotateTween(
            Transform transform, 
            Vector3 targetRotation, 
            float? duration = null, 
            TweenOptions options = default)
        {
            if (!ValidateTarget(transform, "Rotate")) return null;
            
            var tween = transform.DORotate(targetRotation, duration ?? TweenHelperSettings.Instance.DefaultDuration);
            ConfigureTween(tween, transform.gameObject, duration, options);
            return tween;
        }
        
        /// <summary>
        /// Creates a configured tween for scaling a Transform to a target scale.
        /// </summary>
        /// <param name="transform">The Transform to scale.</param>
        /// <param name="targetScale">The target scale.</param>
        /// <param name="duration">The duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured scale tween.</returns>
        public static TweenerCore<Vector3, Vector3, VectorOptions> CreateScaleTween(
            Transform transform, 
            Vector3 targetScale, 
            float? duration = null, 
            TweenOptions options = default)
        {
            if (!ValidateTarget(transform, "Scale")) return null;
            
            var tween = transform.DOScale(targetScale, duration ?? TweenHelperSettings.Instance.DefaultDuration);
            ConfigureTween(tween, transform.gameObject, duration, options);
            return ApplySnapping(tween, options);
        }
    }
}
