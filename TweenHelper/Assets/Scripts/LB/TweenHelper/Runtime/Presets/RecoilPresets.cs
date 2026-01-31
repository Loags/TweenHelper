using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Pulls the target backward on local Z then snaps it forward past the origin, simulating a recoil action.
    /// <para>
    /// Builds a 2-step relative sequence: (1) move <c>-0.5</c> on local Z over 40% duration with <c>Ease.OutQuad</c>,
    /// (2) move <c>+0.5</c> on local Z over 60% duration with <c>Ease.OutCubic</c>.
    /// Both steps use <c>SetRelative(true)</c>, so the target returns to its original position.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutQuad (pull), OutCubic (snap)<br/>
    /// <b>Easing override:</b> Primary ease controls pull-back; secondary ease controls snap-forward.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Weapon recoil, spring release, knockback response, firing animation.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Recoil").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RecoilPreset : CodePreset
    {
        public override string PresetName => "Recoil";
        public override string Description => "Pull back then snap forward on local Z";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var pullEase = ResolveEase(options, Ease.OutQuad);
            var snapEase = ResolveSecondaryEase(options, Ease.OutCubic);
            var presetOptions = MergeWithDefaultEase(options, pullEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveZ(-0.5f, dur * 0.4f).SetRelative(true).SetEase(pullEase))
                .Append(t.DOLocalMoveZ(0.5f, dur * 0.6f).SetRelative(true).SetEase(snapEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Internal factory for Recoil directional variants sharing the same pull/snap structure.
    /// </summary>
    internal static class RecoilFactory
    {
        public static Tween Create(GameObject target, float pullZ, float duration, TweenOptions options)
        {
            var t = target.transform;
            var pullEase = options.Ease ?? Ease.OutQuad;
            var snapEase = options.SecondaryEase ?? options.Ease ?? Ease.OutCubic;
            var presetOptions = options.Ease.HasValue ? options : options.SetEase(pullEase);

            return DOTween.Sequence()
                .Append(t.DOLocalMoveZ(pullZ, duration * 0.4f).SetRelative(true).SetEase(pullEase))
                .Append(t.DOLocalMoveZ(-pullZ, duration * 0.6f).SetRelative(true).SetEase(snapEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Pulls the target forward on local Z then snaps it back, simulating a forward recoil.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutQuad (pull), OutCubic (snap)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RecoilForward").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RecoilForwardPreset : CodePreset
    {
        public override string PresetName => "RecoilForward";
        public override string Description => "Pull forward then snap back on local Z";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RecoilFactory.Create(target, 0.5f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Pulls the target backward on local Z then snaps it forward, simulating a backward recoil.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutQuad (pull), OutCubic (snap)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RecoilBack").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RecoilBackPreset : CodePreset
    {
        public override string PresetName => "RecoilBack";
        public override string Description => "Pull back then snap forward on local Z";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RecoilFactory.Create(target, -0.5f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Soft recoil with shorter pull-back distance.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (pull), OutCubic (snap)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RecoilSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RecoilSoftPreset : CodePreset
    {
        public override string PresetName => "RecoilSoft";
        public override string Description => "Soft pull back then snap forward";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RecoilFactory.Create(target, -0.25f, GetDuration(duration), options);
        }
    }

    /// <summary>
    /// Hard recoil with longer pull-back distance.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (pull), OutCubic (snap)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("RecoilHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RecoilHardPreset : CodePreset
    {
        public override string PresetName => "RecoilHard";
        public override string Description => "Hard pull back then snap forward";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return RecoilFactory.Create(target, -0.8f, GetDuration(duration), options);
        }
    }
}
