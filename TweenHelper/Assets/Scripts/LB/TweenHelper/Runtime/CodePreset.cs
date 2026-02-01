using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

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
    public abstract class CodePreset : ITweenPreset
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
        /// The base overshoot value for this preset, scaled by the caller's overshoot multiplier.
        /// Meaning is preset-specific: ease overshoot parameter, scale excess, etc.
        /// </summary>
        public virtual float DefaultOvershoot => 0f;

        /// <summary>
        /// Creates and configures a tween for the specified target.
        /// </summary>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null falls through to options.Duration, then preset default).</param>
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
        /// Helper method to get the actual duration (parameter or options or default).
        /// </summary>
        protected float GetDuration(float? duration, TweenOptions options = default)
        {
            return duration ?? options.Duration ?? DefaultDuration;
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
        /// Resolves the overshoot multiplier from options, defaulting to 1.0 (no change).
        /// Presets multiply their base overshoot value by this.
        /// </summary>
        protected float ResolveOvershootMultiplier(TweenOptions options)
        {
            return options.Overshoot ?? 1.0f;
        }

        /// <summary>
        /// Resolves the ease to use for a preset, falling back to the provided default.
        /// </summary>
        protected Ease ResolveEase(TweenOptions options, Ease defaultEase)
        {
            return options.Ease ?? defaultEase;
        }

        /// <summary>
        /// Resolves a secondary ease to use for presets with multiple tweens, falling back to primary or default.
        /// </summary>
        protected Ease ResolveSecondaryEase(TweenOptions options, Ease defaultEase)
        {
            return options.SecondaryEase ?? options.Ease ?? defaultEase;
        }

        /// <summary>
        /// Resolves a tertiary ease to use for presets with multiple tweens, falling back to primary or default.
        /// </summary>
        protected Ease ResolveTertiaryEase(TweenOptions options, Ease defaultEase)
        {
            return options.TertiaryEase ?? options.SecondaryEase ?? options.Ease ?? defaultEase;
        }

        /// <summary>
        /// Resolves the start scale from options, falling back to the provided default.
        /// </summary>
        protected Vector3 ResolveStartScale(TweenOptions options, Vector3 defaultStart)
        {
            return options.StartScale ?? defaultStart;
        }

        /// <summary>
        /// Resolves the target scale from options, falling back to the provided default.
        /// </summary>
        protected Vector3 ResolveTargetScale(TweenOptions options, Vector3 defaultTarget)
        {
            return options.TargetScale ?? defaultTarget;
        }

        /// <summary>
        /// Registers this preset with the registry.
        /// Call this if not using [AutoRegisterPreset] attribute.
        /// </summary>
        public void Register()
        {
            TweenPresetRegistry.RegisterPreset(this);
        }

        /// <summary>
        /// Creates a fade tween for the appropriate component type.
        /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer (material).
        /// </summary>
        /// <summary>
        /// Internal entry point for <see cref="CreateFadeTween"/> used by shared factories.
        /// </summary>
        internal static Tween CreateFadeTweenStatic(GameObject target, float alpha, float duration)
            => CreateFadeTween(target, alpha, duration);

        protected static Tween CreateFadeTween(GameObject target, float alpha, float duration)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                return canvasGroup.DOFade(alpha, duration);

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                return spriteRenderer.DOFade(alpha, duration);

            var image = target.GetComponent<Image>();
            if (image != null)
                return image.DOFade(alpha, duration);

            var text = target.GetComponent<Text>();
            if (text != null)
                return text.DOFade(alpha, duration);

            // Fallback to Renderer material fade
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
                return renderer.material.DOFade(alpha, duration);

            return null;
        }

        /// <summary>
        /// Sets the alpha for the appropriate component type.
        /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer (material).
        /// </summary>
        /// <summary>
        /// Internal entry point for <see cref="SetAlpha"/> used by shared factories.
        /// </summary>
        internal static void SetAlphaStatic(GameObject target, float alpha)
            => SetAlpha(target, alpha);

        protected static void SetAlpha(GameObject target, float alpha)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                return;
            }

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
                return;
            }

            var image = target.GetComponent<Image>();
            if (image != null)
            {
                var c = image.color;
                c.a = alpha;
                image.color = c;
                return;
            }

            var text = target.GetComponent<Text>();
            if (text != null)
            {
                var c = text.color;
                c.a = alpha;
                text.color = c;
                return;
            }

            // Fallback to Renderer material
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                var c = renderer.material.color;
                c.a = alpha;
                renderer.material.color = c;
            }
        }

        /// <summary>
        /// Checks if the target has a component that supports fading.
        /// </summary>
        protected static bool CanFade(GameObject target)
        {
            if (target == null) return false;

            return target.GetComponent<CanvasGroup>() != null ||
                   target.GetComponent<SpriteRenderer>() != null ||
                   target.GetComponent<Image>() != null ||
                   target.GetComponent<Text>() != null ||
                   (target.GetComponent<Renderer>()?.material != null);
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
