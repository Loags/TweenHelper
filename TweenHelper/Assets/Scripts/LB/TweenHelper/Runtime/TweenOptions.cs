using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Value type that represents per-call overrides for tween behavior.
    /// Allows explicit control over delay, ease, update type, unscaled time, snapping, loops, and speed-based behavior.
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
        /// Creates a new TweenOptions with the specified update type.
        /// </summary>
        /// <param name="updateType">The update type to use.</param>
        /// <returns>A new TweenOptions instance.</returns>
        public static TweenOptions WithUpdateType(UpdateType updateType)
        {
            return new TweenOptions { updateType = updateType };
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
        /// Sets the identifier for this TweenOptions.
        /// </summary>
        /// <param name="id">The identifier for the tween.</param>
        /// <returns>This TweenOptions instance for chaining.</returns>
        public TweenOptions SetId(string id)
        {
            this.id = id;
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
            if (updateType.HasValue) parts.Add($"Update:{updateType.Value}");
            if (unscaledTime.HasValue) parts.Add($"Unscaled:{unscaledTime.Value}");
            if (snapping.HasValue) parts.Add($"Snap:{snapping.Value}");
            if (loops.HasValue) parts.Add($"Loops:{loops.Value}");
            if (loopType.HasValue) parts.Add($"LoopType:{loopType.Value}");
            if (speedBased.HasValue) parts.Add($"Speed:{speedBased.Value}");
            if (!string.IsNullOrEmpty(id)) parts.Add($"Id:{id}");
            
            return parts.Count > 0 ? $"TweenOptions({string.Join(", ", parts)})" : "TweenOptions(None)";
        }
    }
}
