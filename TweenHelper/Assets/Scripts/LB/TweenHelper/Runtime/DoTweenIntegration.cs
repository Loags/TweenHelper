using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Core utilities for DOTween integration.
    /// Provides linking and validation utilities used by TweenBuilder and TweenDefaults.
    /// </summary>
    public static class DoTweenIntegration
    {
        /// <summary>
        /// Links a tween to a GameObject for automatic cleanup when the GameObject is destroyed.
        /// Uses DOTween's native SetLink with fallback to TweenLifecycleTracker.
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
                // Use DOTween's native linking
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
    }
}
