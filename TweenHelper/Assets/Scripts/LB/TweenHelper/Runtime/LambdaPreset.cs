using DG.Tweening;
using System;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Allows defining animation presets inline without creating a class.
    /// Useful for quick, project-specific presets or runtime-defined animations.
    /// </summary>
    /// <example>
    /// // Register a simple inline preset
    /// TweenPresetRegistry.RegisterPreset(new LambdaPreset(
    ///     name: "QuickFade",
    ///     create: (target, duration, options) =>
    ///     {
    ///         var cg = target.GetComponent&lt;CanvasGroup&gt;();
    ///         return cg?.DOFade(0f, duration ?? 0.3f).WithDefaults(options, target);
    ///     },
    ///     description: "Quickly fades out a CanvasGroup",
    ///     canApply: target => target.GetComponent&lt;CanvasGroup&gt;() != null
    /// ));
    ///
    /// // Use it
    /// transform.Tween().Preset("QuickFade").Play();
    /// </example>
    public sealed class LambdaPreset : ITweenPreset
    {
        private readonly Func<GameObject, float?, TweenOptions, Tween> _createTween;
        private readonly Func<GameObject, bool> _canApply;

        /// <summary>
        /// The unique name identifier for this preset.
        /// </summary>
        public string PresetName { get; }

        /// <summary>
        /// A description of what this preset does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The default duration for this preset.
        /// </summary>
        public float DefaultDuration { get; }

        /// <summary>
        /// Creates a new LambdaPreset with the specified configuration.
        /// </summary>
        /// <param name="name">The unique name for this preset.</param>
        /// <param name="create">The function that creates the tween.</param>
        /// <param name="description">Optional description of what the preset does.</param>
        /// <param name="defaultDuration">Default duration when none is specified.</param>
        /// <param name="canApply">Optional function to check if preset can be applied to a target.</param>
        public LambdaPreset(
            string name,
            Func<GameObject, float?, TweenOptions, Tween> create,
            string description = "",
            float defaultDuration = 0.5f,
            Func<GameObject, bool> canApply = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Preset name cannot be null or empty.", nameof(name));
            }

            PresetName = name;
            _createTween = create ?? throw new ArgumentNullException(nameof(create));
            Description = description ?? string.Empty;
            DefaultDuration = defaultDuration;
            _canApply = canApply ?? DefaultCanApply;
        }

        /// <summary>
        /// Creates and configures a tween for the specified target.
        /// </summary>
        public Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            if (target == null)
            {
                Debug.LogWarning($"LambdaPreset '{PresetName}': Cannot create tween for null target.");
                return null;
            }

            try
            {
                return _createTween(target, duration, options);
            }
            catch (Exception ex)
            {
                Debug.LogError($"LambdaPreset '{PresetName}': Error creating tween - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets whether this preset can be applied to the specified target.
        /// </summary>
        public bool CanApplyTo(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            try
            {
                return _canApply(target);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Registers this preset with the registry.
        /// </summary>
        /// <returns>This preset for chaining.</returns>
        public LambdaPreset Register()
        {
            TweenPresetRegistry.RegisterPreset(this);
            return this;
        }

        private static bool DefaultCanApply(GameObject target)
        {
            return target != null && target.activeInHierarchy;
        }

        /// <summary>
        /// Creates a simple fade-out preset.
        /// </summary>
        public static LambdaPreset FadeOut(string name = "FadeOut", float defaultDuration = 0.3f)
        {
            return new LambdaPreset(
                name: name,
                create: (target, duration, options) =>
                {
                    var cg = target.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        return cg.DOFade(0f, duration ?? defaultDuration).WithDefaults(options, target);
                    }

                    var sr = target.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        return sr.DOFade(0f, duration ?? defaultDuration).WithDefaults(options, target);
                    }

                    Debug.LogWarning($"LambdaPreset '{name}': No fadeable component found on {target.name}");
                    return null;
                },
                description: "Fades out CanvasGroup or SpriteRenderer",
                defaultDuration: defaultDuration,
                canApply: target =>
                    target.GetComponent<CanvasGroup>() != null ||
                    target.GetComponent<SpriteRenderer>() != null
            );
        }

        /// <summary>
        /// Creates a simple fade-in preset.
        /// </summary>
        public static LambdaPreset FadeIn(string name = "FadeIn", float defaultDuration = 0.3f)
        {
            return new LambdaPreset(
                name: name,
                create: (target, duration, options) =>
                {
                    var cg = target.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        return cg.DOFade(1f, duration ?? defaultDuration).WithDefaults(options, target);
                    }

                    var sr = target.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        return sr.DOFade(1f, duration ?? defaultDuration).WithDefaults(options, target);
                    }

                    Debug.LogWarning($"LambdaPreset '{name}': No fadeable component found on {target.name}");
                    return null;
                },
                description: "Fades in CanvasGroup or SpriteRenderer",
                defaultDuration: defaultDuration,
                canApply: target =>
                    target.GetComponent<CanvasGroup>() != null ||
                    target.GetComponent<SpriteRenderer>() != null
            );
        }

        /// <summary>
        /// Creates a simple scale punch preset.
        /// </summary>
        public static LambdaPreset ScalePunch(string name = "ScalePunch", float defaultDuration = 0.3f, float punchStrength = 0.2f)
        {
            return new LambdaPreset(
                name: name,
                create: (target, duration, options) =>
                {
                    return target.transform.DOPunchScale(Vector3.one * punchStrength, duration ?? defaultDuration)
                        .WithDefaults(options, target);
                },
                description: "Punches the scale for feedback",
                defaultDuration: defaultDuration
            );
        }

        /// <summary>
        /// Creates a simple position shake preset.
        /// </summary>
        public static LambdaPreset Shake(string name = "Shake", float defaultDuration = 0.5f, float strength = 0.5f)
        {
            return new LambdaPreset(
                name: name,
                create: (target, duration, options) =>
                {
                    return target.transform.DOShakePosition(duration ?? defaultDuration, strength)
                        .WithDefaults(options, target);
                },
                description: "Shakes the position",
                defaultDuration: defaultDuration
            );
        }
    }
}
