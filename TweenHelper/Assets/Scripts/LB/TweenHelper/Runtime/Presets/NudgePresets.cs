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
    /// <b>Easing override:</b> Primary ease controls push; secondary ease controls springy return.
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
            var t = target.transform;
            var dur = GetDuration(duration);
            var pushEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, pushEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveX(0.3f, dur * 0.3f).SetRelative(true).SetEase(pushEase))
                .Append(t.DOLocalMoveX(-0.3f, dur * 0.7f).SetRelative(true).SetEase(returnEase))
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
            var t = target.transform;
            var offset = direction * distance;
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
            return NudgeFactory.Create(target, Vector3.left, 0.3f, GetDuration(duration), options);
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
            return NudgeFactory.Create(target, Vector3.right, 0.3f, GetDuration(duration), options);
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
            return NudgeFactory.Create(target, Vector3.up, 0.3f, GetDuration(duration), options);
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
            return NudgeFactory.Create(target, Vector3.down, 0.3f, GetDuration(duration), options);
        }
    }
}
