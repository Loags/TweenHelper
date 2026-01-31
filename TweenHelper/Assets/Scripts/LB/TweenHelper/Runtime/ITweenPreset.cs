using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Interface for defining reusable tween presets.
    /// Presets can be defined in code or as ScriptableObject assets.
    /// </summary>
    public interface ITweenPreset
    {
        /// <summary>
        /// The unique name identifier for this preset.
        /// </summary>
        string PresetName { get; }
        
        /// <summary>
        /// A description of what this preset does.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// The default duration for this preset (can be overridden when played).
        /// </summary>
        float DefaultDuration { get; }
        
        /// <summary>
        /// Creates and configures a tween for the specified target using this preset.
        /// </summary>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The configured tween or sequence.</returns>
        Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default);
        
        /// <summary>
        /// Gets whether this preset can be applied to the specified target.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>True if the preset can be applied to the target.</returns>
        bool CanApplyTo(GameObject target);
    }
}

