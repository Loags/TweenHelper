using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Abstract base class for ScriptableObject-based tween presets.
    /// Provides a foundation for creating reusable animation assets.
    /// </summary>
    public abstract class TweenPresetBase : ScriptableObject, ITweenPreset
    {
        [Header("Preset Configuration")]
        [SerializeField] private string presetName;
        [SerializeField] private string description;
        [SerializeField, Min(0.01f)] private float defaultDuration = 1f;
        
        [Header("Default Options")]
        [SerializeField] private Ease defaultEase = Ease.OutQuart;
        [SerializeField, Min(0f)] private float defaultDelay = 0f;
        [SerializeField] private bool defaultUnscaledTime = false;
        [SerializeField] private bool defaultSnapping = false;
        
        /// <summary>
        /// The unique name identifier for this preset.
        /// </summary>
        public virtual string PresetName => string.IsNullOrEmpty(presetName) ? name : presetName;
        
        /// <summary>
        /// A description of what this preset does.
        /// </summary>
        public virtual string Description => description;
        
        /// <summary>
        /// The default duration for this preset.
        /// </summary>
        public virtual float DefaultDuration => defaultDuration;
        
        /// <summary>
        /// The default ease for this preset.
        /// </summary>
        public virtual Ease DefaultEase => defaultEase;
        
        /// <summary>
        /// The default delay for this preset.
        /// </summary>
        public virtual float DefaultDelay => defaultDelay;
        
        /// <summary>
        /// Whether this preset uses unscaled time by default.
        /// </summary>
        public virtual bool DefaultUnscaledTime => defaultUnscaledTime;
        
        /// <summary>
        /// Whether this preset uses snapping by default.
        /// </summary>
        public virtual bool DefaultSnapping => defaultSnapping;
        
        /// <summary>
        /// Creates the default options for this preset, combining preset defaults with any overrides.
        /// </summary>
        /// <param name="optionOverrides">Options to override the preset defaults.</param>
        /// <returns>The combined options.</returns>
        protected virtual TweenOptions CreateDefaultOptions(TweenOptions optionOverrides = default)
        {
            return new TweenOptions()
                .SetEase(optionOverrides.Ease ?? DefaultEase)
                .SetDelay(optionOverrides.Delay ?? DefaultDelay)
                .SetUnscaledTime(optionOverrides.UnscaledTime ?? DefaultUnscaledTime)
                .SetSnapping(optionOverrides.Snapping ?? DefaultSnapping);
        }
        
        /// <summary>
        /// Creates and configures a tween for the specified target using this preset.
        /// Derived classes must implement this to define the specific animation behavior.
        /// </summary>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured tween or sequence.</returns>
        public abstract Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default);
        
        /// <summary>
        /// Gets whether this preset can be applied to the specified target.
        /// Default implementation checks for a Transform component.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the preset can be applied to the target.</returns>
        public virtual bool CanApplyTo(GameObject target)
        {
            return target != null && target.transform != null;
        }
        
        /// <summary>
        /// Validates the preset configuration and logs warnings for potential issues.
        /// </summary>
        public virtual void ValidatePreset()
        {
            if (string.IsNullOrEmpty(PresetName))
            {
                Debug.LogWarning($"TweenPreset '{name}': PresetName is empty, using asset name instead.");
            }
            
            if (defaultDuration <= 0f)
            {
                Debug.LogWarning($"TweenPreset '{PresetName}': Default duration should be greater than zero.");
            }
        }
        
        private void OnValidate()
        {
            // Ensure reasonable minimum values
            defaultDuration = Mathf.Max(0.01f, defaultDuration);
            defaultDelay = Mathf.Max(0f, defaultDelay);
            
            // Auto-generate preset name from asset name if empty
            if (string.IsNullOrEmpty(presetName))
            {
                presetName = name;
            }
        }
    }
}

