using DG.Tweening;
using System;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Base class for code-defined animation presets.
    /// Developers can subclass this to create reusable animation patterns without ScriptableObjects.
    /// </summary>
    /// <example>
    /// [AutoRegisterPreset]
    /// public class ButtonPressPreset : CodePreset
    /// {
    ///     public override string PresetName => "ButtonPress";
    ///     public override float DefaultDuration => 0.15f;
    ///
    ///     public override Tween CreateTween(GameObject target, float? duration, TweenOptions options)
    ///     {
    ///         return target.transform.DOPunchScale(Vector3.one * 0.1f, duration ?? DefaultDuration)
    ///             .WithDefaults(options, target);
    ///     }
    /// }
    /// </example>
    public abstract partial class CodePreset : ITweenPreset, ICategorizedTweenPreset
    {
        /// <summary>
        /// The unique name identifier for this preset.
        /// </summary>
        public abstract string PresetName { get; }

        /// <summary>
        /// A description of what this preset does.
        /// </summary>
        public virtual string Description => string.Empty;

        /// <summary>
        /// The default duration for this preset.
        /// </summary>
        public virtual float DefaultDuration => 0.5f;

        /// <summary>
        /// Category used for grouping in showcases. Override in subclasses as needed.
        /// </summary>
        public virtual string Category => PresetCategories.Base;

        /// <summary>
        /// Creates and configures a tween for the specified target.
        /// </summary>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured tween.</returns>
        public abstract Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default);

        /// <summary>
        /// Gets whether this preset can be applied to the specified target.
        /// Override to add specific component requirements.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the preset can be applied.</returns>
        public virtual bool CanApplyTo(GameObject target)
        {
            return target != null && target.activeInHierarchy;
        }

        /// <summary>
        /// Helper method to get the actual duration (parameter or default).
        /// </summary>
        protected float GetDuration(float? duration)
        {
            return duration ?? DefaultDuration;
        }

        /// <summary>
        /// Helper method to merge caller options with a default ease.
        /// </summary>
        protected TweenOptions MergeWithDefaultEase(TweenOptions callerOptions, Ease defaultEase)
        {
            if (!callerOptions.Ease.HasValue)
            {
                return callerOptions.SetEase(defaultEase);
            }
            return callerOptions;
        }

        /// <summary>
        /// Registers this preset with the registry.
        /// Call this if not using [AutoRegisterPreset] attribute.
        /// </summary>
        public void Register()
        {
            TweenPresetRegistry.RegisterPreset(this);
        }
    }

    /// <summary>
    /// Attribute to mark CodePreset classes for automatic registration at runtime.
    /// The TweenPresetRegistry will scan for classes with this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoRegisterPresetAttribute : Attribute
    {
    }
}
