using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        public TweenBuilder AudioVolumeTo(float destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateAudioVolume(_gameObject, destination, ResolvePropertyDuration(duration, options, 0.5f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder AudioPitchTo(float destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateAudioPitch(_gameObject, destination, ResolvePropertyDuration(duration, options, 0.5f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder LightIntensityTo(float destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateLightIntensity(_gameObject, destination, ResolvePropertyDuration(duration, options, 0.55f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder LightColorTo(Color destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateLightColor(_gameObject, destination, ResolvePropertyDuration(duration, options, 0.55f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder ParticleEmissionRateTo(float destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateParticleEmissionRate(_gameObject, destination, ResolvePropertyDuration(duration, options, 0.6f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder MaterialFloatTo(string propertyName, float destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateMaterialFloat(_gameObject, propertyName, destination, ResolvePropertyDuration(duration, options, 0.55f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder MaterialColorTo(string propertyName, Color destination, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateMaterialColor(_gameObject, propertyName, destination, ResolvePropertyDuration(duration, options, 0.55f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder TorchFlicker(float variation = 0.22f, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateTorchFlicker(_gameObject, variation, ResolvePropertyDuration(duration, options, 1.2f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder ScannerPulse(Color? accentColor = null, float intensityBoost = 1.2f, float? duration = null)
        {
            AddStep(options => EnginePropertyAnimationUtility.CreateScannerPulse(_gameObject, accentColor, intensityBoost, ResolvePropertyDuration(duration, options, 0.9f), options), applyBuilderOptions: false);
            return this;
        }

        private static float ResolvePropertyDuration(float? duration, TweenOptions options, float fallback) => duration ?? options.Duration ?? fallback;
    }
}
