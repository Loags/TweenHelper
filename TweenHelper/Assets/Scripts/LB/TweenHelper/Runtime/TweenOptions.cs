using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Value type that represents per-call overrides for tween behavior.
    /// Allows explicit control over duration, delay, ease, update type, unscaled time, snapping, loops, and speed-based behavior.
    /// </summary>
    [System.Serializable]
    public struct TweenOptions
    {
        [SerializeField] private float? delay;
        [SerializeField] private Ease? ease;
        [SerializeField] private UpdateType? updateType;
        [SerializeField] private bool? unscaledTime;
        [SerializeField] private bool? snapping;
        [SerializeField] private int? loops;
        [SerializeField] private LoopType? loopType;
        [SerializeField] private bool? speedBased;
        [SerializeField] private string id;
        [SerializeField] private Ease? secondaryEase;
        [SerializeField] private Ease? tertiaryEase;
        [SerializeField] private float? overshoot;
        [SerializeField] private float? duration;
        [SerializeField] private Vector3? startScale;
        [SerializeField] private Vector3? targetScale;
        [SerializeField] private float? strength;
        [SerializeField] private float? startAlpha;
        [SerializeField] private float? targetAlpha;

        /// <summary>
        /// Creates a new TweenOptions with the specified duration override.
        /// </summary>
        /// <param name="duration">The duration override for the tween.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithDuration(float duration)
        {
            return new TweenOptions { duration = duration };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified delay.
        /// </summary>
        /// <param name="delay">The delay before the tween starts.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithDelay(float delay)
        {
            return new TweenOptions { delay = delay };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with the specified ease.
        /// </summary>
        /// <param name="ease">The easing function to use.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithEase(Ease ease)
        {
            return new TweenOptions { ease = ease };
        }

        /// <summary>
        /// Creates a new TweenOptions with primary/secondary/tertiary ease values.
        /// Secondary/tertiary are optional and used by presets that have multiple internal tweens.
        /// </summary>
        public static TweenOptions WithEases(Ease ease, Ease? secondary = null, Ease? tertiary = null)
        {
            return new TweenOptions { ease = ease, secondaryEase = secondary, tertiaryEase = tertiary };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with the specified loop configuration.
        /// </summary>
        /// <param name="loops">Number of loops (-1 for infinite).</param>
        /// <param name="loopType">The type of loop behavior.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithLoops(int loops, LoopType loopType)
        {
            return new TweenOptions { loops = loops, loopType = loopType };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with the specified loop configuration using Restart loop type.
        /// </summary>
        /// <param name="loops">Number of loops (-1 for infinite).</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithLoops(int loops)
        {
            return new TweenOptions { loops = loops, loopType = DG.Tweening.LoopType.Restart };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with speed-based behavior enabled.
        /// </summary>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithSpeedBased()
        {
            return new TweenOptions { speedBased = true };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with unscaled time enabled.
        /// </summary>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithUnscaledTime()
        {
            return new TweenOptions { unscaledTime = true };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with snapping enabled.
        /// </summary>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithSnapping()
        {
            return new TweenOptions { snapping = true };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier for the tween.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithId(string id)
        {
            return new TweenOptions { id = id };
        }
        
        /// <summary>
        /// Creates a new TweenOptions with the specified overshoot multiplier.
        /// Default is 1.0 (preset's built-in amount). 0.5 = half, 2.0 = double, 0.0 = none.
        /// </summary>
        /// <param name="overshoot">The overshoot multiplier.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithOvershoot(float overshoot)
        {
            return new TweenOptions { overshoot = overshoot };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified update type.
        /// </summary>
        /// <param name="updateType">The update type to use.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithUpdateType(UpdateType updateType)
        {
            return new TweenOptions { updateType = updateType };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified start scale override for entrance presets.
        /// </summary>
        /// <param name="startScale">The start scale to use instead of the preset default (typically zero).</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithStartScale(Vector3 startScale)
        {
            return new TweenOptions { startScale = startScale };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified target scale override for scale presets.
        /// </summary>
        /// <param name="targetScale">The target scale to use instead of the preset default.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithTargetScale(Vector3 targetScale)
        {
            return new TweenOptions { targetScale = targetScale };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified strength multiplier.
        /// Multiplies a preset's core magnitude (distance, angle, height, etc.). Default 1.0.
        /// </summary>
        /// <param name="strength">The strength multiplier. 0.5 = half, 2.0 = double.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithStrength(float strength)
        {
            return new TweenOptions { strength = strength };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified start alpha override for fade entrance presets.
        /// </summary>
        /// <param name="startAlpha">The start alpha to use instead of the preset default (typically 0).</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithStartAlpha(float startAlpha)
        {
            return new TweenOptions { startAlpha = startAlpha };
        }

        /// <summary>
        /// Creates a new TweenOptions with the specified target alpha override for fade presets.
        /// </summary>
        /// <param name="targetAlpha">The target alpha to use instead of the preset default.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithTargetAlpha(float targetAlpha)
        {
            return new TweenOptions { targetAlpha = targetAlpha };
        }


        #region Fluent API
        
        /// <summary>
        /// Sets the delay for this TweenOptions.
        /// </summary>
        /// <param name="delay">The delay before the tween starts.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }
        
        /// <summary>
        /// Sets the ease for this TweenOptions.
        /// </summary>
        /// <param name="ease">The easing function to use.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetEase(Ease ease)
        {
            this.ease = ease;
            return this;
        }

        /// <summary>
        /// Sets the secondary ease for this TweenOptions.
        /// </summary>
        public TweenOptions SetSecondaryEase(Ease ease)
        {
            this.secondaryEase = ease;
            return this;
        }

        /// <summary>
        /// Sets the tertiary ease for this TweenOptions.
        /// </summary>
        public TweenOptions SetTertiaryEase(Ease ease)
        {
            this.tertiaryEase = ease;
            return this;
        }
        
        /// <summary>
        /// Sets the update type for this TweenOptions.
        /// </summary>
        /// <param name="updateType">The update type to use.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetUpdateType(UpdateType updateType)
        {
            this.updateType = updateType;
            return this;
        }

        
        /// <summary>
        /// Sets whether to use unscaled time for this TweenOptions.
        /// </summary>
        /// <param name="unscaledTime">Whether to use unscaled time.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetUnscaledTime(bool unscaledTime)
        {
            this.unscaledTime = unscaledTime;
            return this;
        }
        
        /// <summary>
        /// Sets whether to use snapping for this TweenOptions.
        /// </summary>
        /// <param name="snapping">Whether to use snapping.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetSnapping(bool snapping)
        {
            this.snapping = snapping;
            return this;
        }
        
        /// <summary>
        /// Sets the loop configuration for this TweenOptions.
        /// </summary>
        /// <param name="loops">Number of loops (-1 for infinite).</param>
        /// <param name="loopType">The type of loop behavior.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetLoops(int loops, LoopType loopType)
        {
            this.loops = loops;
            this.loopType = loopType;
            return this;
        }
        
        /// <summary>
        /// Sets the loop configuration for this TweenOptions using Restart loop type.
        /// </summary>
        /// <param name="loops">Number of loops (-1 for infinite).</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetLoops(int loops)
        {
            this.loops = loops;
            this.loopType = DG.Tweening.LoopType.Restart;
            return this;
        }
        
        /// <summary>
        /// Sets whether this tween should be speed-based.
        /// </summary>
        /// <param name="speedBased">Whether the tween should be speed-based.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetSpeedBased(bool speedBased)
        {
            this.speedBased = speedBased;
            return this;
        }
        
        /// <summary>
        /// Sets the overshoot multiplier for this TweenOptions.
        /// Default is 1.0 (preset's built-in amount). 0.5 = half, 2.0 = double, 0.0 = none.
        /// </summary>
        /// <param name="overshoot">The overshoot multiplier.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetOvershoot(float overshoot)
        {
            this.overshoot = overshoot;
            return this;
        }

        /// <summary>
        /// Sets the identifier for this TweenOptions.
        /// </summary>
        /// <param name="id">The identifier for the tween.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetId(string id)
        {
            this.id = id;
            return this;
        }

        /// <summary>
        /// Sets the duration override for this TweenOptions.
        /// </summary>
        /// <param name="duration">The duration override for the tween.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetDuration(float duration)
        {
            this.duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the start scale override for entrance presets.
        /// </summary>
        /// <param name="startScale">The start scale to use instead of the preset default (typically zero).</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetStartScale(Vector3 startScale)
        {
            this.startScale = startScale;
            return this;
        }

        /// <summary>
        /// Sets the target scale override for scale presets.
        /// </summary>
        /// <param name="targetScale">The target scale to use instead of the preset default.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetTargetScale(Vector3 targetScale)
        {
            this.targetScale = targetScale;
            return this;
        }

        /// <summary>
        /// Sets the strength multiplier for this TweenOptions.
        /// Multiplies a preset's core magnitude (distance, angle, height, etc.). Default 1.0.
        /// </summary>
        /// <param name="strength">The strength multiplier. 0.5 = half, 2.0 = double.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetStrength(float strength)
        {
            this.strength = strength;
            return this;
        }

        /// <summary>
        /// Sets the start alpha override for fade entrance presets.
        /// </summary>
        /// <param name="startAlpha">The start alpha to use instead of the preset default (typically 0).</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetStartAlpha(float startAlpha)
        {
            this.startAlpha = startAlpha;
            return this;
        }

        /// <summary>
        /// Sets the target alpha override for fade presets.
        /// </summary>
        /// <param name="targetAlpha">The target alpha to use instead of the preset default.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetTargetAlpha(float targetAlpha)
        {
            this.targetAlpha = targetAlpha;
            return this;
        }

        #endregion
        
        #region Internal Property Access
        
        internal float? Delay => delay;
        internal Ease? Ease => ease;
        internal UpdateType? UpdateType => updateType;
        internal bool? UnscaledTime => unscaledTime;
        internal bool? Snapping => snapping;
        internal int? Loops => loops;
        internal LoopType? LoopType => loopType;
        internal bool? SpeedBased => speedBased;
        internal string Id => id;
        internal Ease? SecondaryEase => secondaryEase;
        internal Ease? TertiaryEase => tertiaryEase;
        internal float? Overshoot => overshoot;
        internal float? Duration => duration;
        internal Vector3? StartScale => startScale;
        internal Vector3? TargetScale => targetScale;
        internal float? Strength => strength;
        internal float? StartAlpha => startAlpha;
        internal float? TargetAlpha => targetAlpha;

        #endregion
        
        /// <summary>
        /// Creates an empty TweenOptions instance with no overrides.
        /// </summary>
        public static TweenOptions None => new TweenOptions();
        
        public override string ToString()
        {
            var parts = new System.Collections.Generic.List<string>();
            
            if (delay.HasValue) parts.Add($"Delay:{delay.Value:F2}");
            if (ease.HasValue) parts.Add($"Ease:{ease.Value}");
            if (secondaryEase.HasValue) parts.Add($"Ease2:{secondaryEase.Value}");
            if (tertiaryEase.HasValue) parts.Add($"Ease3:{tertiaryEase.Value}");
            if (updateType.HasValue) parts.Add($"Update:{updateType.Value}");
            if (unscaledTime.HasValue) parts.Add($"Unscaled:{unscaledTime.Value}");
            if (snapping.HasValue) parts.Add($"Snap:{snapping.Value}");
            if (loops.HasValue) parts.Add($"Loops:{loops.Value}");
            if (loopType.HasValue) parts.Add($"LoopType:{loopType.Value}");
            if (speedBased.HasValue) parts.Add($"Speed:{speedBased.Value}");
            if (!string.IsNullOrEmpty(id)) parts.Add($"Id:{id}");
            if (overshoot.HasValue) parts.Add($"Overshoot:{overshoot.Value:F2}");
            if (duration.HasValue) parts.Add($"Duration:{duration.Value:F2}");
            if (startScale.HasValue) parts.Add($"StartScale:{startScale.Value}");
            if (targetScale.HasValue) parts.Add($"TargetScale:{targetScale.Value}");
            if (strength.HasValue) parts.Add($"Strength:{strength.Value:F2}");
            if (startAlpha.HasValue) parts.Add($"StartAlpha:{startAlpha.Value:F2}");
            if (targetAlpha.HasValue) parts.Add($"TargetAlpha:{targetAlpha.Value:F2}");

            return parts.Count > 0 ? $"TweenOptions({string.Join(", ", parts)})" : "TweenOptions(None)";
        }
    }
}
