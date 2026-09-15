using System;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    [Serializable]
    public sealed class TweenPlayerOverrides
    {
        [SerializeField] private bool overrideEase;
        [SerializeField] private Ease ease = Ease.OutQuad;
        [SerializeField] private bool overrideStrength;
        [SerializeField] private float strength = 1f;
        [SerializeField] private bool overrideOvershoot;
        [SerializeField] private float overshoot = 1f;
        [SerializeField] private bool overrideStartScale;
        [SerializeField] private Vector3 startScale;
        [SerializeField] private bool overrideTargetScale;
        [SerializeField] private Vector3 targetScale = Vector3.one;
        [SerializeField] private bool overrideStartAlpha;
        [SerializeField, Range(0f, 1f)] private float startAlpha;
        [SerializeField] private bool overrideTargetAlpha;
        [SerializeField, Range(0f, 1f)] private float targetAlpha = 1f;

        public static PresetOverrideFields GetSupportedFields(ITweenPreset preset) => preset?.GetType().GetCustomAttribute<PresetOverridesAttribute>()?.Fields ?? PresetOverrideFields.None;

        internal void Validate(ITweenPreset preset, TweenRecipeValidationResult result)
        {
            PresetOverrideFields fields = GetSupportedFields(preset);
            if (overrideStrength && fields.HasFlag(PresetOverrideFields.Strength) && (!IsFinite(strength) || strength < 0f)) result.Add(TweenRecipeValidationSeverity.Error, "Strength must be finite and non-negative.");
            if (overrideOvershoot && fields.HasFlag(PresetOverrideFields.Overshoot) && (!IsFinite(overshoot) || overshoot < 0f)) result.Add(TweenRecipeValidationSeverity.Error, "Overshoot must be finite and non-negative.");
            if (overrideStartAlpha && fields.HasFlag(PresetOverrideFields.StartAlpha) && (!IsFinite(startAlpha) || startAlpha < 0f || startAlpha > 1f)) result.Add(TweenRecipeValidationSeverity.Error, "Start alpha must be between zero and one.");
            if (overrideTargetAlpha && fields.HasFlag(PresetOverrideFields.TargetAlpha) && (!IsFinite(targetAlpha) || targetAlpha < 0f || targetAlpha > 1f)) result.Add(TweenRecipeValidationSeverity.Error, "Target alpha must be between zero and one.");
            if (overrideStartScale && fields.HasFlag(PresetOverrideFields.StartScale) && !IsFinite(startScale)) result.Add(TweenRecipeValidationSeverity.Error, "Start scale must be finite.");
            if (overrideTargetScale && fields.HasFlag(PresetOverrideFields.TargetScale) && !IsFinite(targetScale)) result.Add(TweenRecipeValidationSeverity.Error, "Target scale must be finite.");
        }

        private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        private static bool IsFinite(Vector3 value) => IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);

        internal TweenOptions Apply(TweenOptions options, ITweenPreset preset)
        {
            PresetOverrideFields fields = GetSupportedFields(preset);
            if (overrideEase && fields.HasFlag(PresetOverrideFields.Ease)) options = options.SetEase(ease);
            if (overrideStrength && fields.HasFlag(PresetOverrideFields.Strength)) options = options.SetStrength(strength);
            if (overrideOvershoot && fields.HasFlag(PresetOverrideFields.Overshoot)) options = options.SetOvershoot(overshoot);
            if (overrideStartScale && fields.HasFlag(PresetOverrideFields.StartScale)) options = options.SetStartScale(startScale);
            if (overrideTargetScale && fields.HasFlag(PresetOverrideFields.TargetScale)) options = options.SetTargetScale(targetScale);
            if (overrideStartAlpha && fields.HasFlag(PresetOverrideFields.StartAlpha)) options = options.SetStartAlpha(startAlpha);
            if (overrideTargetAlpha && fields.HasFlag(PresetOverrideFields.TargetAlpha)) options = options.SetTargetAlpha(targetAlpha);
            return options;
        }
    }
}
