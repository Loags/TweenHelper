using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class EnginePropertyAnimationUtility
    {
        public static Tween CreateAudioVolume(GameObject target, float destination, float duration, TweenOptions options)
        {
            ValidateRange(destination, 0f, 1f, nameof(destination));
            AudioSource source = RequireComponent<AudioSource>(target);
            return CreateFloat(target, () => source.volume, value => source.volume = value, destination, duration, options);
        }

        public static Tween CreateAudioPitch(GameObject target, float destination, float duration, TweenOptions options)
        {
            ValidateRange(destination, -3f, 3f, nameof(destination));
            AudioSource source = RequireComponent<AudioSource>(target);
            return CreateFloat(target, () => source.pitch, value => source.pitch = value, destination, duration, options);
        }

        public static Tween CreateLightIntensity(GameObject target, float destination, float duration, TweenOptions options)
        {
            ValidateNonNegative(destination, nameof(destination));
            Light light = RequireComponent<Light>(target);
            return CreateFloat(target, () => light.intensity, value => light.intensity = value, destination, duration, options);
        }

        public static Tween CreateLightColor(GameObject target, Color destination, float duration, TweenOptions options)
        {
            Light light = RequireComponent<Light>(target);
            return CreateColor(target, () => light.color, value => light.color = value, destination, duration, options);
        }

        public static Tween CreateParticleEmissionRate(GameObject target, float destination, float duration, TweenOptions options)
        {
            ValidateNonNegative(destination, nameof(destination));
            ParticleSystem particles = RequireComponent<ParticleSystem>(target);
            return CreateFloat(
                target,
                () => particles.emission.rateOverTimeMultiplier,
                value =>
                {
                    ParticleSystem.EmissionModule emission = particles.emission;
                    emission.rateOverTimeMultiplier = value;
                },
                destination,
                duration,
                options);
        }

        public static Tween CreateMaterialFloat(GameObject target, string propertyName, float destination, float duration, TweenOptions options)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            ValidateFinite(destination, nameof(destination));
            Renderer renderer = RequireComponent<Renderer>(target);
            int propertyId = Shader.PropertyToID(propertyName);
            ValidateMaterialProperty(renderer, propertyId, propertyName);
            var block = new MaterialPropertyBlock();
            return CreateFloat(target, Get, Set, destination, duration, options);

            float Get()
            {
                renderer.GetPropertyBlock(block);
                return block.isEmpty ? renderer.sharedMaterial.GetFloat(propertyId) : block.GetFloat(propertyId);
            }

            void Set(float value)
            {
                renderer.GetPropertyBlock(block);
                block.SetFloat(propertyId, value);
                renderer.SetPropertyBlock(block);
            }
        }

        public static Tween CreateMaterialColor(GameObject target, string propertyName, Color destination, float duration, TweenOptions options)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            Renderer renderer = RequireComponent<Renderer>(target);
            int propertyId = Shader.PropertyToID(propertyName);
            ValidateMaterialProperty(renderer, propertyId, propertyName);
            var block = new MaterialPropertyBlock();
            return CreateColor(target, Get, Set, destination, duration, options);

            Color Get()
            {
                renderer.GetPropertyBlock(block);
                return block.isEmpty ? renderer.sharedMaterial.GetColor(propertyId) : block.GetColor(propertyId);
            }

            void Set(Color value)
            {
                renderer.GetPropertyBlock(block);
                block.SetColor(propertyId, value);
                renderer.SetPropertyBlock(block);
            }
        }

        public static Tween CreateTorchFlicker(GameObject target, float variation, float duration, TweenOptions options)
        {
            ValidateNonNegative(variation, nameof(variation));
            ValidateDuration(duration, options);
            Light light = RequireComponent<Light>(target);
            float start = 0f;
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () => start = light.intensity,
                progress =>
                {
                    float noise = Mathf.Sin(progress * Mathf.PI * 10f) * 0.55f + Mathf.Sin(progress * Mathf.PI * 23f + 0.7f) * 0.3f + Mathf.Sin(progress * Mathf.PI * 37f + 1.8f) * 0.15f;
                    light.intensity = Mathf.Max(0f, start * (1f + noise * variation * ResolveStrength(options)));
                },
                () => light.intensity = start,
                () => light.intensity = start,
                () => light.intensity = start,
                () => light.intensity = start);
        }

        public static Tween CreateScannerPulse(GameObject target, Color? accentColor, float intensityBoost, float duration, TweenOptions options)
        {
            ValidateNonNegative(intensityBoost, nameof(intensityBoost));
            ValidateDuration(duration, options);
            Light light = RequireComponent<Light>(target);
            float startIntensity = 0f;
            Color startColor = default;
            return NormalizedTweenTimeline.Create(
                target,
                duration,
                options.SetEase(Ease.Linear),
                () =>
                {
                    startIntensity = light.intensity;
                    startColor = light.color;
                },
                progress =>
                {
                    float pulse = Mathf.Sin(progress * Mathf.PI);
                    light.intensity = startIntensity + pulse * intensityBoost * ResolveStrength(options);
                    light.color = Color.LerpUnclamped(startColor, accentColor ?? Color.cyan, pulse * 0.72f);
                },
                () => RestoreLight(light, startIntensity, startColor),
                () => RestoreLight(light, startIntensity, startColor),
                () => RestoreLight(light, startIntensity, startColor),
                () => RestoreLight(light, startIntensity, startColor));
        }

        private static Tween CreateFloat(GameObject owner, Func<float> getter, Action<float> setter, float destination, float duration, TweenOptions options)
        {
            ValidateDuration(duration, options);
            float start = 0f;
            return NormalizedTweenTimeline.Create(
                owner,
                duration,
                options.SetEase(Ease.Linear),
                () => start = getter(),
                progress => setter(Mathf.LerpUnclamped(start, destination, EaseValue(progress, options.Ease ?? Ease.InOutCubic))),
                () => setter(destination),
                () => setter(start),
                () => setter(start),
                () => setter(start));
        }

        private static Tween CreateColor(GameObject owner, Func<Color> getter, Action<Color> setter, Color destination, float duration, TweenOptions options)
        {
            ValidateDuration(duration, options);
            Color start = default;
            return NormalizedTweenTimeline.Create(
                owner,
                duration,
                options.SetEase(Ease.Linear),
                () => start = getter(),
                progress => setter(Color.LerpUnclamped(start, destination, EaseValue(progress, options.Ease ?? Ease.InOutCubic))),
                () => setter(destination),
                () => setter(start),
                () => setter(start),
                () => setter(start));
        }

        private static T RequireComponent<T>(GameObject target) where T : Component
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            T component = target.GetComponent<T>();
            if (component == null) throw new InvalidOperationException($"Property animation target '{target.name}' requires a {typeof(T).Name} component on the same GameObject.");
            return component;
        }

        private static void ValidateMaterialProperty(Renderer renderer, int propertyId, string propertyName)
        {
            Material material = renderer.sharedMaterial;
            if (material == null || !material.HasProperty(propertyId)) throw new InvalidOperationException($"Renderer '{renderer.name}' material does not declare property '{propertyName}'.");
        }

        private static void ValidateDuration(float duration, TweenOptions options)
        {
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (options.SpeedBased == true) throw new NotSupportedException("Engine-property animations do not support speed-based timing.");
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateNonNegative(strength, nameof(TweenOptions.Strength));
            return strength;
        }

        private static void RestoreLight(Light light, float intensity, Color color)
        {
            light.intensity = intensity;
            light.color = color;
        }

        private static void ValidateRange(float value, float minimum, float maximum, string parameterName)
        {
            ValidateFinite(value, parameterName);
            if (value < minimum || value > maximum) throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {minimum} and {maximum}.");
        }

        private static void ValidateNonNegative(float value, string parameterName)
        {
            ValidateFinite(value, parameterName);
            if (value < 0f) throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private static float EaseValue(float progress, Ease ease) => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);
    }
}
