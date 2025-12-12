using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Extension methods to apply TweenHelperSettings defaults to raw DOTween tweens.
    /// Use these when calling DOTween directly but wanting centralized configuration.
    /// </summary>
    /// <example>
    /// // Apply defaults to raw DOTween
    /// transform.DOMove(target, 1f).WithDefaults();
    ///
    /// // Apply defaults with options
    /// transform.DOScale(2f, 0.5f).WithDefaults(TweenOptions.WithEase(Ease.OutBack));
    ///
    /// // Using default duration
    /// transform.DOMove(target, TweenDefaults.DefaultDuration).WithDefaults();
    /// </example>
    public static class TweenDefaults
    {
        #region Default Values Access

        /// <summary>
        /// Gets the default duration from TweenHelperSettings.
        /// </summary>
        public static float DefaultDuration => TweenHelperSettings.Instance.DefaultDuration;

        /// <summary>
        /// Gets the default ease from TweenHelperSettings.
        /// </summary>
        public static Ease DefaultEase => TweenHelperSettings.Instance.DefaultEase;

        /// <summary>
        /// Gets the default delay from TweenHelperSettings.
        /// </summary>
        public static float DefaultDelay => TweenHelperSettings.Instance.DefaultDelay;

        /// <summary>
        /// Gets the default update type from TweenHelperSettings.
        /// </summary>
        public static UpdateType DefaultUpdateType => TweenHelperSettings.Instance.DefaultUpdateType;

        /// <summary>
        /// Gets whether unscaled time is used by default.
        /// </summary>
        public static bool DefaultUnscaledTime => TweenHelperSettings.Instance.DefaultUnscaledTime;

        #endregion

        #region Tween Extension Methods

        /// <summary>
        /// Applies TweenHelperSettings defaults to any tween.
        /// </summary>
        /// <param name="tween">The tween to configure.</param>
        /// <param name="linkTarget">Optional GameObject to link for auto-cleanup. If null, uses SetTarget's target.</param>
        /// <returns>The configured tween.</returns>
        public static T WithDefaults<T>(this T tween, GameObject linkTarget = null) where T : Tween
        {
            return WithDefaults(tween, default, linkTarget);
        }

        /// <summary>
        /// Applies TweenHelperSettings defaults with option overrides to any tween.
        /// </summary>
        /// <param name="tween">The tween to configure.</param>
        /// <param name="options">Options that override defaults.</param>
        /// <param name="linkTarget">Optional GameObject to link for auto-cleanup.</param>
        /// <returns>The configured tween.</returns>
        public static T WithDefaults<T>(this T tween, TweenOptions options, GameObject linkTarget = null) where T : Tween
        {
            if (tween == null || !tween.IsActive())
            {
                return tween;
            }

            var settings = TweenHelperSettings.Instance;

            // Apply ease (options override > settings default)
            var ease = options.Ease ?? settings.DefaultEase;
            tween.SetEase(ease);

            // Apply delay
            var delay = options.Delay ?? settings.DefaultDelay;
            if (delay > 0)
            {
                tween.SetDelay(delay);
            }

            // Apply update type and unscaled time
            var updateType = options.UpdateType ?? settings.DefaultUpdateType;
            var unscaledTime = options.UnscaledTime ?? settings.DefaultUnscaledTime;
            tween.SetUpdate(updateType, unscaledTime);

            // Apply loops
            if (options.Loops.HasValue)
            {
                tween.SetLoops(options.Loops.Value, options.LoopType ?? LoopType.Restart);
            }

            // Apply speed-based
            if (options.SpeedBased == true)
            {
                tween.SetSpeedBased(true);
            }

            // Apply ID
            if (!string.IsNullOrEmpty(options.Id))
            {
                tween.SetId(options.Id);
            }

            // Link to GameObject for auto-cleanup
            if (linkTarget != null)
            {
                tween.SetLink(linkTarget);
                tween.SetTarget(linkTarget);
            }

            return tween;
        }

        /// <summary>
        /// Applies defaults for tweens that loop manually (via callbacks) instead of DOTween's built-in looping.
        /// Delay and loop counts are intentionally not applied every cycle to keep the loop seamless.
        /// </summary>
        /// <param name="tween">The tween to configure.</param>
        /// <param name="options">Options that override defaults.</param>
        /// <param name="linkTarget">Optional GameObject to link for auto-cleanup.</param>
        /// <param name="applyDelayThisCycle">Whether to apply delay for this cycle.</param>
        /// <returns>The configured tween.</returns>
        public static T WithLoopDefaults<T>(this T tween, TweenOptions options, GameObject linkTarget, bool applyDelayThisCycle) where T : Tween
        {
            if (tween == null || !tween.IsActive())
            {
                return tween;
            }

            var settings = TweenHelperSettings.Instance;

            // Apply ease (options override > settings default)
            var ease = options.Ease ?? settings.DefaultEase;
            tween.SetEase(ease);

            // Apply delay only for the first cycle so we don't pause between loops.
            if (applyDelayThisCycle)
            {
                var delay = options.Delay ?? settings.DefaultDelay;
                if (delay > 0f)
                {
                    tween.SetDelay(delay);
                }
            }

            // Apply update type and unscaled time
            var updateType = options.UpdateType ?? settings.DefaultUpdateType;
            var unscaledTime = options.UnscaledTime ?? settings.DefaultUnscaledTime;
            tween.SetUpdate(updateType, unscaledTime);

            // Apply speed-based
            if (options.SpeedBased == true)
            {
                tween.SetSpeedBased(true);
            }

            // Apply ID
            if (!string.IsNullOrEmpty(options.Id))
            {
                tween.SetId(options.Id);
            }

            // Link to GameObject for auto-cleanup
            if (linkTarget != null)
            {
                tween.SetLink(linkTarget);
                tween.SetTarget(linkTarget);
            }

            return tween;
        }

        #endregion

        #region Sequence Extension Methods

        /// <summary>
        /// Applies TweenHelperSettings defaults to a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to configure.</param>
        /// <param name="linkTarget">Optional GameObject to link for auto-cleanup.</param>
        /// <returns>The configured sequence.</returns>
        public static Sequence WithDefaults(this Sequence sequence, GameObject linkTarget = null)
        {
            return WithDefaults(sequence, default, linkTarget);
        }

        /// <summary>
        /// Applies TweenHelperSettings defaults with option overrides to a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to configure.</param>
        /// <param name="options">Options that override defaults.</param>
        /// <param name="linkTarget">Optional GameObject to link for auto-cleanup.</param>
        /// <returns>The configured sequence.</returns>
        public static Sequence WithDefaults(this Sequence sequence, TweenOptions options, GameObject linkTarget = null)
        {
            if (sequence == null || !sequence.IsActive())
            {
                return sequence;
            }

            var settings = TweenHelperSettings.Instance;

            // Apply ease (options override > settings default)
            var ease = options.Ease ?? settings.DefaultEase;
            sequence.SetEase(ease);

            // Apply delay
            var delay = options.Delay ?? settings.DefaultDelay;
            if (delay > 0)
            {
                sequence.SetDelay(delay);
            }

            // Apply update type and unscaled time
            var updateType = options.UpdateType ?? settings.DefaultUpdateType;
            var unscaledTime = options.UnscaledTime ?? settings.DefaultUnscaledTime;
            sequence.SetUpdate(updateType, unscaledTime);

            // Apply loops
            if (options.Loops.HasValue)
            {
                sequence.SetLoops(options.Loops.Value, options.LoopType ?? LoopType.Restart);
            }

            // Apply ID
            if (!string.IsNullOrEmpty(options.Id))
            {
                sequence.SetId(options.Id);
            }

            // Link to GameObject for auto-cleanup
            if (linkTarget != null)
            {
                sequence.SetLink(linkTarget);
                sequence.SetTarget(linkTarget);
            }

            return sequence;
        }

        #endregion

        #region Linking Extension Methods

        /// <summary>
        /// Links a tween to a GameObject for automatic cleanup when destroyed.
        /// </summary>
        /// <param name="tween">The tween to link.</param>
        /// <param name="target">The GameObject to link to.</param>
        /// <returns>The linked tween.</returns>
        public static T LinkTo<T>(this T tween, GameObject target) where T : Tween
        {
            if (tween != null && tween.IsActive() && target != null)
            {
                tween.SetLink(target);
                tween.SetTarget(target);
            }
            return tween;
        }

        /// <summary>
        /// Links a tween to a Component's GameObject for automatic cleanup when destroyed.
        /// </summary>
        /// <param name="tween">The tween to link.</param>
        /// <param name="component">The Component whose GameObject to link to.</param>
        /// <returns>The linked tween.</returns>
        public static T LinkTo<T>(this T tween, Component component) where T : Tween
        {
            return LinkTo(tween, component?.gameObject);
        }

        #endregion
    }
}
