using UnityEngine;
using System.Collections.Generic;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Manages a stack of animated objects for reset functionality.
    /// Press R to reset the last animated object, Shift+R to reset all.
    /// </summary>
    public class AnimationResetManager : MonoBehaviour
    {
        public static AnimationResetManager Instance { get; private set; }

        [Header("Diagnostics")]
        [SerializeField] private bool verboseResetLogging;

        private readonly Stack<AnimationPresetDisplay> _animationHistory = new Stack<AnimationPresetDisplay>();

        public int HistoryCount => _animationHistory.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if (verboseResetLogging)
                {
                    Debug.Log($"AnimationResetManager: Key R detected (shift={shiftHeld}) history={_animationHistory.Count}");
                }

                if (shiftHeld)
                {
                    ResetAll();
                }
                else
                {
                    ResetLast();
                }
            }
        }

        /// <summary>
        /// Registers an object that was just animated.
        /// Called by AnimationPresetDisplay when an animation plays.
        /// </summary>
        public void RegisterAnimated(AnimationPresetDisplay display)
        {
            if (display == null) return;

            PruneHistory(removeInactive: false);

            // Remove if already in stack to avoid duplicates, then add to top
            var temp = new List<AnimationPresetDisplay>();
            while (_animationHistory.Count > 0)
            {
                var item = _animationHistory.Pop();
                if (IsValidHistoryEntry(item, removeInactive: false) && item != display)
                {
                    temp.Add(item);
                }
            }

            // Restore stack in reverse order
            for (int i = temp.Count - 1; i >= 0; i--)
            {
                _animationHistory.Push(temp[i]);
            }

            // Add new one on top
            _animationHistory.Push(display);

            if (verboseResetLogging)
            {
                Debug.Log($"AnimationResetManager: Registered '{display.PresetName}'. History={_animationHistory.Count}");
            }
        }

        /// <summary>
        /// Resets the last animated object.
        /// </summary>
        public void ResetLast()
        {
            PruneHistory(removeInactive: true);

            if (!TryPopNextValidDisplay(out var display, removeInactive: true))
            {
                Debug.Log("AnimationResetManager: No animations to reset.");
                return;
            }

            display.ResetAnimation();
            Debug.Log($"AnimationResetManager: Reset '{display.PresetName}'");

            if (verboseResetLogging)
            {
                Debug.Log($"AnimationResetManager: ResetLast complete. History={_animationHistory.Count}");
            }
        }

        /// <summary>
        /// Resets all animated objects at once.
        /// </summary>
        public void ResetAll()
        {
            int pruned = PruneHistory(removeInactive: true);

            if (_animationHistory.Count == 0)
            {
                Debug.Log("AnimationResetManager: No animations to reset.");
                return;
            }

            int count = 0;
            while (_animationHistory.Count > 0)
            {
                var display = _animationHistory.Pop();
                if (IsValidHistoryEntry(display, removeInactive: true))
                {
                    display.ResetAnimation();
                    count++;
                }
            }

            Debug.Log($"AnimationResetManager: Reset {count} animations." +
                      (pruned > 0 ? $" Pruned {pruned} stale entries first." : ""));
        }

        /// <summary>
        /// Clears the history without resetting objects.
        /// </summary>
        public void ClearHistory()
        {
            _animationHistory.Clear();
        }

        internal int PruneHistory(bool removeInactive)
        {
            if (_animationHistory.Count == 0) return 0;

            int originalCount = _animationHistory.Count;
            var temp = new List<AnimationPresetDisplay>(originalCount);

            while (_animationHistory.Count > 0)
            {
                var item = _animationHistory.Pop();
                if (IsValidHistoryEntry(item, removeInactive))
                {
                    temp.Add(item);
                }
            }

            for (int i = temp.Count - 1; i >= 0; i--)
            {
                _animationHistory.Push(temp[i]);
            }

            int removed = originalCount - _animationHistory.Count;
            if (removed > 0 && verboseResetLogging)
            {
                Debug.Log($"AnimationResetManager: Pruned {removed} stale history entries (removeInactive={removeInactive}).");
            }

            return removed;
        }

        private bool TryPopNextValidDisplay(out AnimationPresetDisplay display, bool removeInactive)
        {
            while (_animationHistory.Count > 0)
            {
                var candidate = _animationHistory.Pop();
                if (IsValidHistoryEntry(candidate, removeInactive))
                {
                    display = candidate;
                    return true;
                }
            }

            display = null;
            return false;
        }

        private static bool IsValidHistoryEntry(AnimationPresetDisplay display, bool removeInactive)
        {
            if (display == null) return false;
            if (display.gameObject == null) return false;
            if (removeInactive && !display.gameObject.activeInHierarchy) return false;
            return true;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
