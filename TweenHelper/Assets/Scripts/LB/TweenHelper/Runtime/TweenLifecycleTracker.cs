using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Lightweight component that tracks active tweens per GameObject and kills them on destroy.
    /// This serves as a fallback when native DoTween linking is not available or fails.
    /// </summary>
    [AddComponentMenu("LB/TweenHelper/Lifecycle Tracker")]
    public class TweenLifecycleTracker : MonoBehaviour
    {
        [SerializeField] private List<int> trackedTweenIds = new List<int>();
        private readonly List<Tween> trackedTweens = new List<Tween>();
        
        #if UNITY_EDITOR
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private int activeTweenCount = 0;
        #endif
        
        /// <summary>
        /// Gets the number of currently tracked active tweens.
        /// </summary>
        public int ActiveTweenCount => GetActiveTweenCount();
        
        /// <summary>
        /// Registers a tween for tracking and automatic cleanup.
        /// </summary>
        /// <param name="tween">The tween to track.</param>
        public void RegisterTween(Tween tween)
        {
            if (tween == null)
            {
                Debug.LogWarning($"TweenLifecycleTracker: Cannot register null tween on '{gameObject.name}'.");
                return;
            }
            
            // Remove any existing reference to this tween
            UnregisterTween(tween);
            
            // Add to our tracking collections
            trackedTweens.Add(tween);
            trackedTweenIds.Add(tween.GetHashCode());
            
            // Set up callback to auto-unregister when tween completes naturally
            tween.OnComplete(() => UnregisterTween(tween));
            tween.OnKill(() => UnregisterTween(tween));
            
            #if UNITY_EDITOR
            if (showDebugInfo)
            {
                Debug.Log($"TweenLifecycleTracker: Registered tween {tween.GetHashCode()} on '{gameObject.name}'. Total tracked: {trackedTweens.Count}");
            }
            #endif
            
            UpdateDebugInfo();
        }
        
        /// <summary>
        /// Unregisters a tween from tracking.
        /// </summary>
        /// <param name="tween">The tween to unregister.</param>
        public void UnregisterTween(Tween tween)
        {
            if (tween == null) return;
            
            int tweenId = tween.GetHashCode();
            int index = trackedTweenIds.IndexOf(tweenId);
            
            if (index >= 0)
            {
                trackedTweenIds.RemoveAt(index);
                if (index < trackedTweens.Count)
                {
                    trackedTweens.RemoveAt(index);
                }
                
                #if UNITY_EDITOR
                if (showDebugInfo)
                {
                    Debug.Log($"TweenLifecycleTracker: Unregistered tween {tweenId} on '{gameObject.name}'. Total tracked: {trackedTweens.Count}");
                }
                #endif
                
                UpdateDebugInfo();
            }
        }
        
        /// <summary>
        /// Kills all tracked tweens immediately.
        /// </summary>
        public void KillAllTrackedTweens()
        {
            int killedCount = 0;
            
            // Create a copy of the list to avoid modification during iteration
            var tweensToKill = new List<Tween>(trackedTweens);
            
            foreach (var tween in tweensToKill)
            {
                if (tween != null && tween.IsActive())
                {
                    try
                    {
                        tween.Kill();
                        killedCount++;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"TweenLifecycleTracker: Failed to kill tween on '{gameObject.name}'. {ex.Message}");
                    }
                }
            }
            
            // Clear our tracking collections
            trackedTweens.Clear();
            trackedTweenIds.Clear();
            
            #if UNITY_EDITOR
            if (showDebugInfo && killedCount > 0)
            {
                Debug.Log($"TweenLifecycleTracker: Killed {killedCount} tweens on GameObject '{gameObject.name}' destruction.");
            }
            #endif
            
            UpdateDebugInfo();
        }
        
        /// <summary>
        /// Cleans up any dead or inactive tweens from our tracking collections.
        /// </summary>
        public void CleanupDeadTweens()
        {
            int initialCount = trackedTweens.Count;
            
            for (int i = trackedTweens.Count - 1; i >= 0; i--)
            {
                var tween = trackedTweens[i];
                if (tween == null || !tween.IsActive())
                {
                    trackedTweens.RemoveAt(i);
                    if (i < trackedTweenIds.Count)
                    {
                        trackedTweenIds.RemoveAt(i);
                    }
                }
            }
            
            int cleanedCount = initialCount - trackedTweens.Count;
            if (cleanedCount > 0)
            {
                #if UNITY_EDITOR
                if (showDebugInfo)
                {
                    Debug.Log($"TweenLifecycleTracker: Cleaned up {cleanedCount} dead tweens on '{gameObject.name}'.");
                }
                #endif
                
                UpdateDebugInfo();
            }
        }
        
        /// <summary>
        /// Gets the count of currently active tracked tweens.
        /// </summary>
        /// <returns>The number of active tweens.</returns>
        private int GetActiveTweenCount()
        {
            int activeCount = 0;
            foreach (var tween in trackedTweens)
            {
                if (tween != null && tween.IsActive())
                {
                    activeCount++;
                }
            }
            return activeCount;
        }
        
        private void UpdateDebugInfo()
        {
            #if UNITY_EDITOR
            activeTweenCount = GetActiveTweenCount();
            #endif
        }
        
        private void OnDestroy()
        {
            KillAllTrackedTweens();
        }
        
        private void OnDisable()
        {
            // Optionally kill tweens when GameObject is disabled
            // This can be controlled by a setting if needed
            if (Application.isPlaying)
            {
                KillAllTrackedTweens();
            }
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateDebugInfo();
        }
        
        // Clean up dead tweens periodically in editor
        private void Update()
        {
            if (!Application.isPlaying) return;
            
            // Periodically clean up dead tweens (every few seconds)
            if (Time.time % 3f < Time.deltaTime)
            {
                CleanupDeadTweens();
            }
        }
        
        /// <summary>
        /// Editor utility to manually clean up dead tweens.
        /// </summary>
        [ContextMenu("Clean Up Dead Tweens")]
        private void EditorCleanupDeadTweens()
        {
            CleanupDeadTweens();
        }
        
        /// <summary>
        /// Editor utility to manually kill all tracked tweens.
        /// </summary>
        [ContextMenu("Kill All Tracked Tweens")]
        private void EditorKillAllTweens()
        {
            KillAllTrackedTweens();
        }
        #endif
    }
}

