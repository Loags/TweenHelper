using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    public static class TweenRecipeOperationCatalog
    {
        private static readonly TweenRecipeOperation[] SupportedOperations =
        {
            TweenRecipeOperation.Delay,
            TweenRecipeOperation.RegisteredPreset,
            TweenRecipeOperation.MoveLocalTo,
            TweenRecipeOperation.MoveLocalBy,
            TweenRecipeOperation.RotateLocalTo,
            TweenRecipeOperation.RotateLocalBy,
            TweenRecipeOperation.ScaleTo,
            TweenRecipeOperation.FadeTo,
            TweenRecipeOperation.ColorTo,
            TweenRecipeOperation.MoveWorldTo,
            TweenRecipeOperation.MoveWorldBy,
            TweenRecipeOperation.RotateWorldTo,
            TweenRecipeOperation.RotateWorldBy,
            TweenRecipeOperation.ArcWorldTo,
            TweenRecipeOperation.HopWorldTo,
            TweenRecipeOperation.ErrorReject,
            TweenRecipeOperation.DamageHit,
            TweenRecipeOperation.SuccessConfirm,
            TweenRecipeOperation.RewardReveal,
            TweenRecipeOperation.PageCrossFadeTo,
            TweenRecipeOperation.TypewriterReveal,
            TweenRecipeOperation.NumberCountTo,
            TweenRecipeOperation.CollectionPreset,
            TweenRecipeOperation.CameraFieldOfViewTo,
            TweenRecipeOperation.LightIntensityTo,
            TweenRecipeOperation.AudioVolumeTo,
            TweenRecipeOperation.ParticleEmissionRateTo
        };

        public static IReadOnlyList<TweenRecipeOperation> Operations => SupportedOperations;

        public static bool IsSupported(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.Delay:
                case TweenRecipeOperation.RegisteredPreset:
                case TweenRecipeOperation.MoveLocalTo:
                case TweenRecipeOperation.MoveLocalBy:
                case TweenRecipeOperation.RotateLocalTo:
                case TweenRecipeOperation.RotateLocalBy:
                case TweenRecipeOperation.ScaleTo:
                case TweenRecipeOperation.FadeTo:
                case TweenRecipeOperation.ColorTo:
                case TweenRecipeOperation.MoveWorldTo:
                case TweenRecipeOperation.MoveWorldBy:
                case TweenRecipeOperation.RotateWorldTo:
                case TweenRecipeOperation.RotateWorldBy:
                case TweenRecipeOperation.ArcWorldTo:
                case TweenRecipeOperation.HopWorldTo:
                case TweenRecipeOperation.ErrorReject:
                case TweenRecipeOperation.DamageHit:
                case TweenRecipeOperation.SuccessConfirm:
                case TweenRecipeOperation.RewardReveal:
                case TweenRecipeOperation.PageCrossFadeTo:
                case TweenRecipeOperation.TypewriterReveal:
                case TweenRecipeOperation.NumberCountTo:
                case TweenRecipeOperation.CollectionPreset:
                case TweenRecipeOperation.CameraFieldOfViewTo:
                case TweenRecipeOperation.LightIntensityTo:
                case TweenRecipeOperation.AudioVolumeTo:
                case TweenRecipeOperation.ParticleEmissionRateTo:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetDisplayName(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.Delay: return "Delay";
                case TweenRecipeOperation.RegisteredPreset: return "Registered Preset";
                case TweenRecipeOperation.MoveLocalTo: return "Move Local To";
                case TweenRecipeOperation.MoveLocalBy: return "Move Local By";
                case TweenRecipeOperation.RotateLocalTo: return "Rotate Local To";
                case TweenRecipeOperation.RotateLocalBy: return "Rotate Local By";
                case TweenRecipeOperation.ScaleTo: return "Scale To";
                case TweenRecipeOperation.FadeTo: return "Fade To";
                case TweenRecipeOperation.ColorTo: return "Color To";
                case TweenRecipeOperation.MoveWorldTo: return "Move World To";
                case TweenRecipeOperation.MoveWorldBy: return "Move World By";
                case TweenRecipeOperation.RotateWorldTo: return "Rotate World To";
                case TweenRecipeOperation.RotateWorldBy: return "Rotate World By";
                case TweenRecipeOperation.ArcWorldTo: return "Arc World To";
                case TweenRecipeOperation.HopWorldTo: return "Hop World To";
                case TweenRecipeOperation.ErrorReject: return "Error Reject";
                case TweenRecipeOperation.DamageHit: return "Damage Hit";
                case TweenRecipeOperation.SuccessConfirm: return "Success Confirm";
                case TweenRecipeOperation.RewardReveal: return "Reward Reveal";
                case TweenRecipeOperation.PageCrossFadeTo: return "Page Cross Fade To";
                case TweenRecipeOperation.TypewriterReveal: return "Typewriter Reveal";
                case TweenRecipeOperation.NumberCountTo: return "Number Count To";
                case TweenRecipeOperation.CollectionPreset: return "Collection Preset";
                case TweenRecipeOperation.CameraFieldOfViewTo: return "Camera Field Of View To";
                case TweenRecipeOperation.LightIntensityTo: return "Light Intensity To";
                case TweenRecipeOperation.AudioVolumeTo: return "Audio Volume To";
                case TweenRecipeOperation.ParticleEmissionRateTo: return "Particle Emission Rate To";
                default: return "Unsupported";
            }
        }

        public static string GetCategory(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.Delay: return "Timing";
                case TweenRecipeOperation.RegisteredPreset: return "Presets";
                case TweenRecipeOperation.MoveLocalTo:
                case TweenRecipeOperation.MoveLocalBy:
                case TweenRecipeOperation.RotateLocalTo:
                case TweenRecipeOperation.RotateLocalBy:
                case TweenRecipeOperation.ScaleTo:
                    return "Transform";
                case TweenRecipeOperation.FadeTo:
                case TweenRecipeOperation.ColorTo:
                    return "Visual";
                case TweenRecipeOperation.MoveWorldTo:
                case TweenRecipeOperation.MoveWorldBy:
                case TweenRecipeOperation.RotateWorldTo:
                case TweenRecipeOperation.RotateWorldBy:
                    return "World Transform";
                case TweenRecipeOperation.ArcWorldTo:
                case TweenRecipeOperation.HopWorldTo:
                    return "Destination Motion";
                case TweenRecipeOperation.ErrorReject:
                case TweenRecipeOperation.DamageHit:
                case TweenRecipeOperation.SuccessConfirm:
                case TweenRecipeOperation.RewardReveal:
                    return "Gameplay Feedback";
                case TweenRecipeOperation.PageCrossFadeTo:
                    return "UI Sequences";
                case TweenRecipeOperation.TypewriterReveal:
                case TweenRecipeOperation.NumberCountTo:
                    return "Text And Values";
                case TweenRecipeOperation.CollectionPreset:
                    return "Collections";
                case TweenRecipeOperation.CameraFieldOfViewTo:
                case TweenRecipeOperation.LightIntensityTo:
                case TweenRecipeOperation.AudioVolumeTo:
                case TweenRecipeOperation.ParticleEmissionRateTo:
                    return "Camera And Engine";
                default:
                    return "Unsupported";
            }
        }

        public static string GetDescription(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.Delay: return "Wait before the next sequential node.";
                case TweenRecipeOperation.RegisteredPreset: return "Play a preset selected by its registered name.";
                case TweenRecipeOperation.MoveLocalTo: return "Move to a local or anchored position.";
                case TweenRecipeOperation.MoveLocalBy: return "Move by a local or anchored offset.";
                case TweenRecipeOperation.RotateLocalTo: return "Rotate to local Euler angles.";
                case TweenRecipeOperation.RotateLocalBy: return "Rotate by local Euler angles.";
                case TweenRecipeOperation.ScaleTo: return "Scale to a local scale.";
                case TweenRecipeOperation.FadeTo: return "Fade a supported visual component.";
                case TweenRecipeOperation.ColorTo: return "Animate a supported visual color.";
                case TweenRecipeOperation.MoveWorldTo: return "Move to a world position.";
                case TweenRecipeOperation.MoveWorldBy: return "Move by a world-space offset.";
                case TweenRecipeOperation.RotateWorldTo: return "Rotate to world Euler angles.";
                case TweenRecipeOperation.RotateWorldBy: return "Rotate by world Euler angles.";
                case TweenRecipeOperation.ArcWorldTo: return "Move to a world destination along an arc.";
                case TweenRecipeOperation.HopWorldTo: return "Move to a world destination with one hop.";
                case TweenRecipeOperation.ErrorReject: return "Play Tween Helper error-rejection feedback.";
                case TweenRecipeOperation.DamageHit: return "Play Tween Helper damage-hit feedback.";
                case TweenRecipeOperation.SuccessConfirm: return "Play Tween Helper success feedback.";
                case TweenRecipeOperation.RewardReveal: return "Play Tween Helper reward-reveal feedback.";
                case TweenRecipeOperation.PageCrossFadeTo: return "Cross-fade from the bound page to a second page binding.";
                case TweenRecipeOperation.TypewriterReveal: return "Reveal existing TMP text by character.";
                case TweenRecipeOperation.NumberCountTo: return "Count TMP text from zero to an integer value.";
                case TweenRecipeOperation.CollectionPreset: return "Apply a registered preset to an ordered collection with a stagger.";
                case TweenRecipeOperation.CameraFieldOfViewTo: return "Animate a Camera field of view.";
                case TweenRecipeOperation.LightIntensityTo: return "Animate a Light intensity.";
                case TweenRecipeOperation.AudioVolumeTo: return "Animate an AudioSource volume.";
                case TweenRecipeOperation.ParticleEmissionRateTo: return "Animate a ParticleSystem emission-rate multiplier.";
                default: return "This operation is not supported.";
            }
        }

        public static bool UsesBinding(TweenRecipeOperation operation) => operation != TweenRecipeOperation.Delay;
        public static bool UsesSecondaryBinding(TweenRecipeOperation operation) => operation == TweenRecipeOperation.PageCrossFadeTo;

        public static TweenRecipeBindingKind GetBindingKind(TweenRecipeOperation operation)
        {
            if (operation == TweenRecipeOperation.CollectionPreset) return TweenRecipeBindingKind.Collection;
            if (IsSupported(operation)) return TweenRecipeBindingKind.GameObject;
            throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }

        public static TweenRecipeBindingKind GetSecondaryBindingKind(TweenRecipeOperation operation)
        {
            if (operation == TweenRecipeOperation.PageCrossFadeTo) return TweenRecipeBindingKind.GameObject;
            throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }

        public static TweenRecipeParameterFields GetVisibleFields(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.RegisteredPreset:
                    return TweenRecipeParameterFields.String;
                case TweenRecipeOperation.MoveLocalTo:
                case TweenRecipeOperation.MoveLocalBy:
                case TweenRecipeOperation.RotateLocalTo:
                case TweenRecipeOperation.RotateLocalBy:
                case TweenRecipeOperation.ScaleTo:
                case TweenRecipeOperation.MoveWorldTo:
                case TweenRecipeOperation.MoveWorldBy:
                case TweenRecipeOperation.RotateWorldTo:
                case TweenRecipeOperation.RotateWorldBy:
                    return TweenRecipeParameterFields.Vector3;
                case TweenRecipeOperation.ArcWorldTo:
                case TweenRecipeOperation.HopWorldTo:
                    return TweenRecipeParameterFields.Vector3 | TweenRecipeParameterFields.Float;
                case TweenRecipeOperation.FadeTo:
                case TweenRecipeOperation.PageCrossFadeTo:
                case TweenRecipeOperation.CameraFieldOfViewTo:
                case TweenRecipeOperation.LightIntensityTo:
                case TweenRecipeOperation.AudioVolumeTo:
                case TweenRecipeOperation.ParticleEmissionRateTo:
                    return TweenRecipeParameterFields.Float;
                case TweenRecipeOperation.ColorTo:
                    return TweenRecipeParameterFields.Color;
                case TweenRecipeOperation.NumberCountTo:
                    return TweenRecipeParameterFields.Integer | TweenRecipeParameterFields.String;
                case TweenRecipeOperation.CollectionPreset:
                    return TweenRecipeParameterFields.String | TweenRecipeParameterFields.Float;
                default:
                    return TweenRecipeParameterFields.None;
            }
        }

        public static string GetParameterLabel(TweenRecipeOperation operation, TweenRecipeParameterFields field)
        {
            switch (field)
            {
                case TweenRecipeParameterFields.Vector3:
                    switch (operation)
                    {
                        case TweenRecipeOperation.MoveLocalBy: return "Local Offset";
                        case TweenRecipeOperation.RotateLocalTo: return "Local Euler Angles";
                        case TweenRecipeOperation.RotateLocalBy: return "Local Euler Offset";
                        case TweenRecipeOperation.ScaleTo: return "Target Scale";
                        case TweenRecipeOperation.MoveWorldBy: return "World Offset";
                        case TweenRecipeOperation.RotateWorldTo: return "World Euler Angles";
                        case TweenRecipeOperation.RotateWorldBy: return "World Euler Offset";
                        case TweenRecipeOperation.ArcWorldTo:
                        case TweenRecipeOperation.HopWorldTo:
                        case TweenRecipeOperation.MoveWorldTo:
                            return "World Destination";
                        default: return "Local Position";
                    }
                case TweenRecipeParameterFields.Color:
                    return "Target Color";
                case TweenRecipeParameterFields.Float:
                    switch (operation)
                    {
                        case TweenRecipeOperation.FadeTo: return "Target Alpha";
                        case TweenRecipeOperation.ArcWorldTo: return "Arc Height";
                        case TweenRecipeOperation.HopWorldTo: return "Hop Height";
                        case TweenRecipeOperation.PageCrossFadeTo: return "Depth Scale";
                        case TweenRecipeOperation.CollectionPreset: return "Stagger Delay";
                        case TweenRecipeOperation.CameraFieldOfViewTo: return "Field Of View";
                        case TweenRecipeOperation.LightIntensityTo: return "Intensity";
                        case TweenRecipeOperation.AudioVolumeTo: return "Volume";
                        case TweenRecipeOperation.ParticleEmissionRateTo: return "Emission Multiplier";
                        default: return "Value";
                    }
                case TweenRecipeParameterFields.Integer:
                    return operation == TweenRecipeOperation.NumberCountTo ? "Target Number" : "Integer";
                case TweenRecipeParameterFields.Boolean:
                    return "Enabled";
                case TweenRecipeParameterFields.String:
                    switch (operation)
                    {
                        case TweenRecipeOperation.RegisteredPreset:
                        case TweenRecipeOperation.CollectionPreset:
                            return "Preset";
                        case TweenRecipeOperation.NumberCountTo:
                            return "Number Format";
                        default:
                            return "Value";
                    }
                default:
                    return string.Empty;
            }
        }

        public static float GetDefaultDuration(TweenRecipeOperation operation) => operation == TweenRecipeOperation.Delay ? 0.25f : 0.35f;
        public static Vector3 GetDefaultVector3(TweenRecipeOperation operation) => operation == TweenRecipeOperation.ScaleTo ? Vector3.one : Vector3.zero;
        public static Color GetDefaultColor(TweenRecipeOperation operation) => Color.white;

        public static float GetDefaultFloat(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.FadeTo:
                case TweenRecipeOperation.AudioVolumeTo:
                    return 1f;
                case TweenRecipeOperation.ArcWorldTo:
                case TweenRecipeOperation.HopWorldTo:
                case TweenRecipeOperation.LightIntensityTo:
                    return 1f;
                case TweenRecipeOperation.PageCrossFadeTo:
                    return 0.04f;
                case TweenRecipeOperation.CollectionPreset:
                    return 0.08f;
                case TweenRecipeOperation.CameraFieldOfViewTo:
                    return 60f;
                case TweenRecipeOperation.ParticleEmissionRateTo:
                    return 10f;
                default:
                    return 0f;
            }
        }

        public static int GetDefaultInteger(TweenRecipeOperation operation) => operation == TweenRecipeOperation.NumberCountTo ? 100 : 0;
        public static bool GetDefaultBoolean(TweenRecipeOperation operation) => false;

        public static string GetDefaultString(TweenRecipeOperation operation)
        {
            switch (operation)
            {
                case TweenRecipeOperation.CollectionPreset: return "PopInFade";
                case TweenRecipeOperation.NumberCountTo: return "N0";
                default: return string.Empty;
            }
        }

        internal static Tween CreateTween(TweenRecipeNode node, GameObject target, GameObject secondaryTarget, IReadOnlyList<GameObject> collectionTargets)
        {
            TweenRecipeParameters parameters = node.Parameters;
            switch (node.Operation)
            {
                case TweenRecipeOperation.RegisteredPreset:
                    ITweenPreset preset = TweenPresetRegistry.GetPresetByName(parameters.StringValue);
                    TweenOptions options = TweenOptions.WithDuration(node.Duration).SetEase(node.Ease);
                    return preset.CreateTween(target, node.Duration, options);
                case TweenRecipeOperation.MoveLocalTo:
                    return TweenTargetUtility.CreateLocalMoveTween(target, parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.MoveLocalBy:
                    return TweenTargetUtility.CreateRelativeLocalMoveTween(target, parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.RotateLocalTo:
                    return target.transform.DOLocalRotate(parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.RotateLocalBy:
                    return target.transform.DOLocalRotate(parameters.Vector3Value, node.Duration, RotateMode.LocalAxisAdd);
                case TweenRecipeOperation.ScaleTo:
                    return target.transform.DOScale(parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.FadeTo:
                    return TweenTargetUtility.CreateFadeTween(target, parameters.FloatValue, node.Duration);
                case TweenRecipeOperation.ColorTo:
                    return TweenTargetUtility.CreateColorTween(target, parameters.ColorValue, node.Duration);
                case TweenRecipeOperation.MoveWorldTo:
                    return target.transform.DOMove(parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.MoveWorldBy:
                    return target.transform.DOMove(target.transform.position + parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.RotateWorldTo:
                    return target.transform.DORotate(parameters.Vector3Value, node.Duration);
                case TweenRecipeOperation.RotateWorldBy:
                    return target.transform.DORotate(parameters.Vector3Value, node.Duration, RotateMode.WorldAxisAdd);
                case TweenRecipeOperation.ArcWorldTo:
                    return BuildTween(target.Tween().ArcTo(parameters.Vector3Value, parameters.FloatValue, node.Duration), node.Ease);
                case TweenRecipeOperation.HopWorldTo:
                    return BuildTween(target.Tween().HopTo(parameters.Vector3Value, parameters.FloatValue, node.Duration), node.Ease);
                case TweenRecipeOperation.ErrorReject:
                    return BuildTween(target.Tween().ErrorReject(node.Duration), node.Ease);
                case TweenRecipeOperation.DamageHit:
                    return BuildTween(target.Tween().DamageHit(node.Duration), node.Ease);
                case TweenRecipeOperation.SuccessConfirm:
                    return BuildTween(target.Tween().SuccessConfirm(node.Duration), node.Ease);
                case TweenRecipeOperation.RewardReveal:
                    return BuildTween(target.Tween().RewardReveal(node.Duration), node.Ease);
                case TweenRecipeOperation.PageCrossFadeTo:
                    return BuildTween(target.Tween().PageCrossFadeTo(secondaryTarget, parameters.FloatValue, node.Duration), node.Ease);
                case TweenRecipeOperation.TypewriterReveal:
                    return BuildTween(target.Tween().TypewriterReveal(TextAnimationUnit.Character, node.Duration), node.Ease);
                case TweenRecipeOperation.NumberCountTo:
                    string format = string.IsNullOrWhiteSpace(parameters.StringValue) ? "N0" : parameters.StringValue;
                    return BuildTween(target.Tween().NumberCountTo(0d, parameters.IntValue, format, node.Duration), node.Ease);
                case TweenRecipeOperation.CollectionPreset:
                    return CreateCollectionPreset(node, collectionTargets);
                case TweenRecipeOperation.CameraFieldOfViewTo:
                    return target.GetComponent<Camera>().DOFieldOfView(parameters.FloatValue, node.Duration);
                case TweenRecipeOperation.LightIntensityTo:
                    return BuildTween(target.Tween().LightIntensityTo(parameters.FloatValue, node.Duration), node.Ease);
                case TweenRecipeOperation.AudioVolumeTo:
                    return BuildTween(target.Tween().AudioVolumeTo(parameters.FloatValue, node.Duration), node.Ease);
                case TweenRecipeOperation.ParticleEmissionRateTo:
                    return BuildTween(target.Tween().ParticleEmissionRateTo(parameters.FloatValue, node.Duration), node.Ease);
                default:
                    throw new ArgumentOutOfRangeException(nameof(node.Operation), node.Operation, null);
            }
        }

        private static Tween BuildTween(TweenBuilder builder, Ease ease)
        {
            TweenHandle handle = builder.WithEase(ease).Build();
            return handle.Tween;
        }

        private static Tween CreateCollectionPreset(TweenRecipeNode node, IReadOnlyList<GameObject> targets)
        {
            ITweenPreset preset = TweenPresetRegistry.GetPresetByName(node.Parameters.StringValue);
            TweenOptions options = TweenOptions.WithDuration(node.Duration).SetEase(node.Ease);
            Sequence sequence = DOTween.Sequence();
            try
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Tween tween = preset.CreateTween(targets[i], node.Duration, options);
                    if (tween == null) throw new InvalidOperationException($"Preset '{preset.PresetName}' did not create a tween for '{targets[i].name}'.");
                    sequence.Insert(i * node.Parameters.FloatValue, tween);
                }

                return sequence;
            }
            catch
            {
                if (sequence.IsActive()) sequence.Kill();
                throw;
            }
        }
    }
}
