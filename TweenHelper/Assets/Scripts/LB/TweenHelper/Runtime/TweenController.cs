using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Control surface for managing groups of tweens by target or identifier.
    /// Provides pause, resume, kill, complete, and rewind operations to coordinate complex screens or game states.
    /// </summary>
    public static class TweenController
    {
        #region Target-based Control
        
        /// <summary>
        /// Pauses all tweens associated with the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to pause.</param>
        public static void Pause(GameObject target)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot pause tweens on null GameObject.");
                return;
            }
            
            target.transform.DOPause();
            LogOperation("Paused", target.name, GetActiveTweenCount(target));
        }
        
        /// <summary>
        /// Pauses all tweens associated with the specified Transform.
        /// </summary>
        /// <param name="target">The Transform whose tweens to pause.</param>
        public static void Pause(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot pause tweens on null Transform.");
                return;
            }
            
            Pause(target.gameObject);
        }
        
        /// <summary>
        /// Resumes all tweens associated with the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to resume.</param>
        public static void Resume(GameObject target)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot resume tweens on null GameObject.");
                return;
            }
            
            target.transform.DOPlay();
            LogOperation("Resumed", target.name, GetActiveTweenCount(target));
        }
        
        /// <summary>
        /// Resumes all tweens associated with the specified Transform.
        /// </summary>
        /// <param name="target">The Transform whose tweens to resume.</param>
        public static void Resume(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot resume tweens on null Transform.");
                return;
            }
            
            Resume(target.gameObject);
        }
        
        /// <summary>
        /// Kills all tweens associated with the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void Kill(GameObject target, bool complete = false)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot kill tweens on null GameObject.");
                return;
            }
            
            int count = GetActiveTweenCount(target);
            target.transform.DOKill(complete);
            
            // Also handle lifecycle tracker if present
            var lifecycleTracker = target.GetComponent<TweenLifecycleTracker>();
            if (lifecycleTracker != null)
            {
                lifecycleTracker.KillAllTrackedTweens();
            }
            
            LogOperation(complete ? "Completed" : "Killed", target.name, count);
        }
        
        /// <summary>
        /// Kills all tweens associated with the specified Transform.
        /// </summary>
        /// <param name="target">The Transform whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void Kill(Transform target, bool complete = false)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot kill tweens on null Transform.");
                return;
            }
            
            Kill(target.gameObject, complete);
        }
        
        /// <summary>
        /// Completes all tweens associated with the specified GameObject immediately.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to complete.</param>
        public static void Complete(GameObject target)
        {
            Kill(target, complete: true);
        }
        
        /// <summary>
        /// Completes all tweens associated with the specified Transform immediately.
        /// </summary>
        /// <param name="target">The Transform whose tweens to complete.</param>
        public static void Complete(Transform target)
        {
            Kill(target, complete: true);
        }
        
        /// <summary>
        /// Rewinds all tweens associated with the specified GameObject to their start values.
        /// </summary>
        /// <param name="target">The GameObject whose tweens to rewind.</param>
        public static void Rewind(GameObject target)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot rewind tweens on null GameObject.");
                return;
            }
            
            target.transform.DORewind();
            LogOperation("Rewound", target.name, GetActiveTweenCount(target));
        }
        
        /// <summary>
        /// Rewinds all tweens associated with the specified Transform to their start values.
        /// </summary>
        /// <param name="target">The Transform whose tweens to rewind.</param>
        public static void Rewind(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("TweenController: Cannot rewind tweens on null Transform.");
                return;
            }
            
            Rewind(target.gameObject);
        }
        
        #endregion
        
        #region Identifier-based Control
        
        /// <summary>
        /// Pauses all tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of tweens to pause.</param>
        public static void PauseById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("TweenController: Cannot pause tweens with null or empty identifier.");
                return;
            }
            
            int count = DOTween.Pause(id);
            LogOperation("Paused", $"ID '{id}'", count);
        }
        
        /// <summary>
        /// Resumes all tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of tweens to resume.</param>
        public static void ResumeById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("TweenController: Cannot resume tweens with null or empty identifier.");
                return;
            }
            
            int count = DOTween.Play(id);
            LogOperation("Resumed", $"ID '{id}'", count);
        }
        
        /// <summary>
        /// Kills all tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void KillById(string id, bool complete = false)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("TweenController: Cannot kill tweens with null or empty identifier.");
                return;
            }
            
            int count = DOTween.Kill(id, complete);
            LogOperation(complete ? "Completed" : "Killed", $"ID '{id}'", count);
        }
        
        /// <summary>
        /// Completes all tweens with the specified identifier immediately.
        /// </summary>
        /// <param name="id">The identifier of tweens to complete.</param>
        public static void CompleteById(string id)
        {
            KillById(id, complete: true);
        }
        
        /// <summary>
        /// Rewinds all tweens with the specified identifier to their start values.
        /// </summary>
        /// <param name="id">The identifier of tweens to rewind.</param>
        public static void RewindById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("TweenController: Cannot rewind tweens with null or empty identifier.");
                return;
            }
            
            int count = DOTween.Rewind(id);
            LogOperation("Rewound", $"ID '{id}'", count);
        }
        
        #endregion
        
        #region Global Control
        
        /// <summary>
        /// Pauses all active tweens globally.
        /// </summary>
        public static void PauseAll()
        {
            int count = DOTween.PauseAll();
            LogOperation("Paused", "all tweens", count);
        }
        
        /// <summary>
        /// Resumes all paused tweens globally.
        /// </summary>
        public static void ResumeAll()
        {
            int count = DOTween.PlayAll();
            LogOperation("Resumed", "all tweens", count);
        }
        
        /// <summary>
        /// Kills all active tweens globally.
        /// </summary>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void KillAll(bool complete = false)
        {
            int count = DOTween.TotalActiveTweens();
            DOTween.KillAll(complete);
            LogOperation(complete ? "Completed" : "Killed", "all tweens", count);
        }
        
        /// <summary>
        /// Completes all active tweens globally immediately.
        /// </summary>
        public static void CompleteAll()
        {
            KillAll(complete: true);
        }
        
        /// <summary>
        /// Rewinds all active tweens globally to their start values.
        /// </summary>
        public static void RewindAll()
        {
            int count = DOTween.RewindAll();
            LogOperation("Rewound", "all tweens", count);
        }
        
        #endregion
        
        #region Multi-target Control
        
        /// <summary>
        /// Pauses all tweens associated with the specified GameObjects.
        /// </summary>
        /// <param name="targets">The GameObjects whose tweens to pause.</param>
        public static void Pause(IEnumerable<GameObject> targets)
        {
            if (targets == null)
            {
                Debug.LogWarning("TweenController: Cannot pause tweens on null target collection.");
                return;
            }
            
            int totalCount = 0;
            foreach (var target in targets.Where(t => t != null))
            {
                totalCount += GetActiveTweenCount(target);
                target.transform.DOPause();
            }
            
            LogOperation("Paused", "multiple targets", totalCount);
        }
        
        /// <summary>
        /// Resumes all tweens associated with the specified GameObjects.
        /// </summary>
        /// <param name="targets">The GameObjects whose tweens to resume.</param>
        public static void Resume(IEnumerable<GameObject> targets)
        {
            if (targets == null)
            {
                Debug.LogWarning("TweenController: Cannot resume tweens on null target collection.");
                return;
            }
            
            int totalCount = 0;
            foreach (var target in targets.Where(t => t != null))
            {
                totalCount += GetActiveTweenCount(target);
                target.transform.DOPlay();
            }
            
            LogOperation("Resumed", "multiple targets", totalCount);
        }
        
        /// <summary>
        /// Kills all tweens associated with the specified GameObjects.
        /// </summary>
        /// <param name="targets">The GameObjects whose tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void Kill(IEnumerable<GameObject> targets, bool complete = false)
        {
            if (targets == null)
            {
                Debug.LogWarning("TweenController: Cannot kill tweens on null target collection.");
                return;
            }
            
            int totalCount = 0;
            foreach (var target in targets.Where(t => t != null))
            {
                totalCount += GetActiveTweenCount(target);
                target.transform.DOKill(complete);
                
                // Also handle lifecycle tracker if present
                var lifecycleTracker = target.GetComponent<TweenLifecycleTracker>();
                if (lifecycleTracker != null)
                {
                    lifecycleTracker.KillAllTrackedTweens();
                }
            }
            
            LogOperation(complete ? "Completed" : "Killed", "multiple targets", totalCount);
        }
        
        /// <summary>
        /// Kills all tweens associated with the specified identifiers.
        /// </summary>
        /// <param name="ids">The identifiers of tweens to kill.</param>
        /// <param name="complete">Whether to complete the tweens before killing them.</param>
        public static void KillByIds(IEnumerable<string> ids, bool complete = false)
        {
            if (ids == null)
            {
                Debug.LogWarning("TweenController: Cannot kill tweens with null identifier collection.");
                return;
            }
            
            int totalCount = 0;
            foreach (var id in ids.Where(i => !string.IsNullOrEmpty(i)))
            {
                totalCount += DOTween.Kill(id, complete);
            }
            
            LogOperation(complete ? "Completed" : "Killed", "multiple IDs", totalCount);
        }
        
        #endregion
        
        #region Information and Diagnostics
        
        /// <summary>
        /// Gets the number of active tweens associated with the specified GameObject.
        /// </summary>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>The number of active tweens.</returns>
        public static int GetActiveTweenCount(GameObject target)
        {
            if (target == null) return 0;
            
            // Count tweens linked to transform components
            int count = 0;
            var components = target.GetComponents<Component>();
            foreach (var component in components)
            {
                count += DOTween.TotalTweensById(component);
            }
            
            // Also count lifecycle tracker tweens if present
            var lifecycleTracker = target.GetComponent<TweenLifecycleTracker>();
            if (lifecycleTracker != null)
            {
                count += lifecycleTracker.ActiveTweenCount;
            }
            
            return count;
        }
        
        /// <summary>
        /// Gets the number of active tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier to check.</param>
        /// <returns>The number of active tweens with that identifier.</returns>
        public static int GetActiveTweenCountById(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            return DOTween.TotalTweensById(id);
        }
        
        /// <summary>
        /// Gets the total number of active tweens globally.
        /// </summary>
        /// <returns>The total number of active tweens.</returns>
        public static int GetTotalActiveTweenCount()
        {
            return DOTween.TotalActiveTweens();
        }
        
        /// <summary>
        /// Checks if the specified GameObject has any active tweens.
        /// </summary>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>True if the GameObject has active tweens.</returns>
        public static bool HasActiveTweens(GameObject target)
        {
            return GetActiveTweenCount(target) > 0;
        }
        
        /// <summary>
        /// Checks if there are any active tweens with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier to check.</param>
        /// <returns>True if there are active tweens with that identifier.</returns>
        public static bool HasActiveTweensById(string id)
        {
            return GetActiveTweenCountById(id) > 0;
        }
        
        #endregion
        
        #region Private Helpers
        
        private static void LogOperation(string operation, string target, int count)
        {
            if (count > 0)
            {
                Debug.Log($"TweenController: {operation} {count} tween(s) on {target}.");
            }
            else
            {
                Debug.Log($"TweenController: No tweens to {operation.ToLower()} on {target}.");
            }
        }
        
        #endregion
    }
}
