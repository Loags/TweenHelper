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
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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

            // Remove if already in stack to avoid duplicates, then add to top
            var temp = new List<AnimationPresetDisplay>();
            while (_animationHistory.Count > 0)
            {
                var item = _animationHistory.Pop();
                if (item != display)
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
        }

        /// <summary>
        /// Resets the last animated object.
        /// </summary>
        public void ResetLast()
        {
            if (_animationHistory.Count == 0)
            {
                Debug.Log("AnimationResetManager: No animations to reset.");
                return;
            }

            var display = _animationHistory.Pop();
            if (display != null)
            {
                display.ResetAnimation();
                Debug.Log($"AnimationResetManager: Reset '{display.PresetName}'");
            }
        }

        /// <summary>
        /// Resets all animated objects at once.
        /// </summary>
        public void ResetAll()
        {
            if (_animationHistory.Count == 0)
            {
                Debug.Log("AnimationResetManager: No animations to reset.");
                return;
            }

            int count = _animationHistory.Count;
            while (_animationHistory.Count > 0)
            {
                var display = _animationHistory.Pop();
                if (display != null)
                {
                    display.ResetAnimation();
                }
            }

            Debug.Log($"AnimationResetManager: Reset {count} animations.");
        }

        /// <summary>
        /// Clears the history without resetting objects.
        /// </summary>
        public void ClearHistory()
        {
            _animationHistory.Clear();
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
