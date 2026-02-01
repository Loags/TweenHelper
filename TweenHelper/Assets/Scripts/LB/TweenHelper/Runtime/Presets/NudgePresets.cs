using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Nudges the target slightly to the right then springs it back to the original position.
    /// <para>
    /// Builds a 2-step relative sequence: (1) move <c>+0.3</c> on local X over 30% duration with <c>Ease.OutQuad</c>,
    /// (2) move <c>-0.3</c> on local X over 70% duration with <c>Ease.OutBack</c> (springy return).
    /// Both steps use <c>SetRelative(true)</c>, so the target returns to its original position.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (push), OutBack (return)<br/>
    /// <b>Easing override:</b> Primary ease controls push; secondary ease controls springy return.<br/>
    /// <b>Strength override:</b> Multiplies nudge distance (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Notification nudge, attention tap, gentle directional hint, interactive prompt.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Nudge").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgePreset : CodePreset
    {
        public override string PresetName => "Nudge";
        public override string Description => "Small push right then spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var strength = ResolveStrength(options);
            var t = target.transform;
            var dur = GetDuration(duration, options);
            var pushEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, pushEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveX(0.3f * strength, dur * 0.3f).SetRelative(true).SetEase(pushEase))
                .Append(t.DOLocalMoveX(-0.3f * strength, dur * 0.7f).SetRelative(true).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for Nudge directional variants sharing the same push/spring-back structure.
    /// </summary>
    internal static class NudgeFactory
    {
        public static Tween Create(GameObject target, Vector3 direction, float distance, float duration, TweenOptions options)
        {
            var strength = CodePreset.ResolveStrengthStatic(options);
            var t = target.transform;
            var offset = direction * (distance * strength);
            var pushEase = options.Ease ?? Ease.OutQuad;
            var returnEase = options.SecondaryEase ?? options.Ease ?? Ease.OutBack;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(pushEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMove(offset, duration * 0.3f).SetRelative(true).SetEase(pushEase))
                .Append(t.DOLocalMove(-offset, duration * 0.7f).SetRelative(true).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Nudges the target to the left then springs back to original position.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeLeftPreset : CodePreset
    {
        public override string PresetName => "NudgeLeft";
        public override string Description => "Small push left then spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.left, 0.3f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Nudges the target to the right then springs back to original position.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeRightPreset : CodePreset
    {
        public override string PresetName => "NudgeRight";
        public override string Description => "Small push right then spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.right, 0.3f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Nudges the target upward then springs back to original position.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeUpPreset : CodePreset
    {
        public override string PresetName => "NudgeUp";
        public override string Description => "Small push up then spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.up, 0.3f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Nudges the target downward then springs back to original position.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeDownPreset : CodePreset
    {
        public override string PresetName => "NudgeDown";
        public override string Description => "Small push down then spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.down, 0.3f, GetDuration(duration, options), options);
        }
    }

    // --- Soft variants (distance=0.15, duration=0.25s) ---

    /// <summary>
    /// Soft nudge to the right then spring back. Smaller distance, shorter duration.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (push), OutBack (return)<br/>
    /// <b>Strength override:</b> Multiplies nudge distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeSoftPreset : CodePreset
    {
        public override string PresetName => "NudgeSoft";
        public override string Description => "Gentle push right then spring back";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.right, 0.15f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft nudge to the left then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeLeftSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeLeftSoftPreset : CodePreset
    {
        public override string PresetName => "NudgeLeftSoft";
        public override string Description => "Gentle push left then spring back";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.left, 0.15f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft nudge to the right then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeRightSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeRightSoftPreset : CodePreset
    {
        public override string PresetName => "NudgeRightSoft";
        public override string Description => "Gentle push right then spring back";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.right, 0.15f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft nudge upward then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeUpSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeUpSoftPreset : CodePreset
    {
        public override string PresetName => "NudgeUpSoft";
        public override string Description => "Gentle push up then spring back";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.up, 0.15f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Soft nudge downward then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeDownSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeDownSoftPreset : CodePreset
    {
        public override string PresetName => "NudgeDownSoft";
        public override string Description => "Gentle push down then spring back";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.down, 0.15f, GetDuration(duration, options), options);
        }
    }

    // --- Hard variants (distance=0.6, duration=0.35s) ---

    /// <summary>
    /// Hard nudge to the right then spring back. Larger distance, snappier feel.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (push), OutBack (return)<br/>
    /// <b>Strength override:</b> Multiplies nudge distance (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeHardPreset : CodePreset
    {
        public override string PresetName => "NudgeHard";
        public override string Description => "Strong push right then spring back";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.right, 0.6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard nudge to the left then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeLeftHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeLeftHardPreset : CodePreset
    {
        public override string PresetName => "NudgeLeftHard";
        public override string Description => "Strong push left then spring back";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.left, 0.6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard nudge to the right then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeRightHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeRightHardPreset : CodePreset
    {
        public override string PresetName => "NudgeRightHard";
        public override string Description => "Strong push right then spring back";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.right, 0.6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard nudge upward then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeUpHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeUpHardPreset : CodePreset
    {
        public override string PresetName => "NudgeUpHard";
        public override string Description => "Strong push up then spring back";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.up, 0.6f, GetDuration(duration, options), options);
        }
    }

    /// <summary>
    /// Hard nudge downward then spring back.
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.35s | <b>Default ease:</b> OutQuad (push), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("NudgeDownHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NudgeDownHardPreset : CodePreset
    {
        public override string PresetName => "NudgeDownHard";
        public override string Description => "Strong push down then spring back";
        public override float DefaultDuration => 0.35f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return NudgeFactory.Create(target, Vector3.down, 0.6f, GetDuration(duration, options), options);
        }
    }
}
