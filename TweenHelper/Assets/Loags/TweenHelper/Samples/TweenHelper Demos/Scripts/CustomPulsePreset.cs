using DG.Tweening;
using UnityEngine;
using UnityEngine.Scripting;

namespace LB.TweenHelper.Samples
{
    [Preserve]
    [AutoRegisterPreset]
    [PresetCategory("Samples/Feedback")]
    [PresetOverrides(PresetOverrideFields.Strength)]
    [PresetPreviewSupported]
    public sealed class CustomPulsePreset : CodePreset
    {
        public override string PresetName => "Sample.CustomPulse";
        public override string Description => "A custom registered scale pulse with an optional strength override.";
        public override float DefaultDuration => 0.3f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            Vector3 originalScale = target.transform.localScale;
            float halfDuration = GetDuration(duration, options) * 0.5f;
            float peak = 1f + 0.12f * ResolveStrength(options);
            return DOTween.Sequence()
                .Append(target.transform.DOScale(originalScale * peak, halfDuration))
                .Append(target.transform.DOScale(originalScale, halfDuration))
                .WithDefaults(options, target);
        }
    }
}
