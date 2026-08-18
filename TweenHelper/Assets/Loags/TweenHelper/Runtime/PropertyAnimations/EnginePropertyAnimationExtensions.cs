using UnityEngine;

namespace LB.TweenHelper
{
    public static class EnginePropertyAnimationExtensions
    {
        public static TweenHandle AudioVolumeTo(this AudioSource source, float destination, float? duration = null, TweenOptions options = default) => source.gameObject.Tween().WithOptions(options).AudioVolumeTo(destination, duration).Play();
        public static TweenHandle AudioPitchTo(this AudioSource source, float destination, float? duration = null, TweenOptions options = default) => source.gameObject.Tween().WithOptions(options).AudioPitchTo(destination, duration).Play();
        public static TweenHandle LightIntensityTo(this Light light, float destination, float? duration = null, TweenOptions options = default) => light.gameObject.Tween().WithOptions(options).LightIntensityTo(destination, duration).Play();
        public static TweenHandle LightColorTo(this Light light, Color destination, float? duration = null, TweenOptions options = default) => light.gameObject.Tween().WithOptions(options).LightColorTo(destination, duration).Play();
        public static TweenHandle ParticleEmissionRateTo(this ParticleSystem particles, float destination, float? duration = null, TweenOptions options = default) => particles.gameObject.Tween().WithOptions(options).ParticleEmissionRateTo(destination, duration).Play();
        public static TweenHandle MaterialFloatTo(this Renderer renderer, string propertyName, float destination, float? duration = null, TweenOptions options = default) => renderer.gameObject.Tween().WithOptions(options).MaterialFloatTo(propertyName, destination, duration).Play();
        public static TweenHandle MaterialColorTo(this Renderer renderer, string propertyName, Color destination, float? duration = null, TweenOptions options = default) => renderer.gameObject.Tween().WithOptions(options).MaterialColorTo(propertyName, destination, duration).Play();
        public static TweenHandle TorchFlicker(this Light light, float variation = 0.22f, float? duration = null, TweenOptions options = default) => light.gameObject.Tween().WithOptions(options).TorchFlicker(variation, duration).Play();
        public static TweenHandle ScannerPulse(this Light light, Color? accentColor = null, float intensityBoost = 1.2f, float? duration = null, TweenOptions options = default) => light.gameObject.Tween().WithOptions(options).ScannerPulse(accentColor, intensityBoost, duration).Play();
    }
}
