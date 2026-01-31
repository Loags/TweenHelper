using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Slides the target downward from 500 units above its current local position to its original position.
    /// <para>
    /// Offsets initial local position by <c>Vector3.up * 500</c>, then animates to the stored target position
    /// using <c>DOLocalMove</c> with <c>Ease.OutCubic</c>. Mirrors SlideInUp but enters from above.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI panel entrance from top, dropdown menu appearance, notification slide-in.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInDownPreset : CodePreset
    {
        public override string PresetName => "SlideInDown";
        public override string Description => "Slides down from above";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.up * 500f;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target upward from 500 units below its current local position to its original position.
    /// <para>
    /// Offsets initial local position by <c>Vector3.down * 500</c>, then animates to the stored target position
    /// using <c>DOLocalMove</c> with <c>Ease.OutCubic</c>. Mirrors SlideInDown but enters from below.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI panel entrance from bottom, toast notification, bottom sheet reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInUpPreset : CodePreset
    {
        public override string PresetName => "SlideInUp";
        public override string Description => "Slides up from below";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.down * 500f;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target in from 500 units to the left of its current local position.
    /// <para>
    /// Offsets initial local position by <c>Vector3.left * 500</c>, then animates to the stored target position
    /// using <c>DOLocalMove</c> with <c>Ease.OutCubic</c>. Mirrors SlideInRight but enters from the left.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Side panel entrance, carousel item slide-in, horizontal menu reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInLeftPreset : CodePreset
    {
        public override string PresetName => "SlideInLeft";
        public override string Description => "Slides in from the left side";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.left * 500f;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target in from 500 units to the right of its current local position.
    /// <para>
    /// Offsets initial local position by <c>Vector3.right * 500</c>, then animates to the stored target position
    /// using <c>DOLocalMove</c> with <c>Ease.OutCubic</c>. Mirrors SlideInLeft but enters from the right.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Side panel entrance, navigation drawer, horizontal content reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInRightPreset : CodePreset
    {
        public override string PresetName => "SlideInRight";
        public override string Description => "Slides in from the right side";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.right * 500f;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target 500 units upward off-screen, mirroring SlideInDown as an exit animation.
    /// <para>
    /// Animates local Y position by <c>+500</c> using <c>DOLocalMoveY</c> with <c>Ease.InCubic</c>.
    /// The accelerating ease creates momentum as the element exits upward. Does not return to origin.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI panel dismiss upward, notification exit, top-exit transition.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutUpPreset : CodePreset
    {
        public override string PresetName => "SlideOutUp";
        public override string Description => "Slides up off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var presetOptions = MergeWithDefaultEase(options, Ease.InCubic);
            var ease = ResolveEase(presetOptions, Ease.InCubic);

            return t.DOLocalMoveY(t.localPosition.y + 500f, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target 500 units downward off-screen, mirroring SlideInUp as an exit animation.
    /// <para>
    /// Animates local Y position by <c>-500</c> using <c>DOLocalMoveY</c> with <c>Ease.InCubic</c>.
    /// The accelerating ease creates momentum as the element exits downward. Does not return to origin.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI panel dismiss downward, bottom-exit transition, dropdown close.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutDownPreset : CodePreset
    {
        public override string PresetName => "SlideOutDown";
        public override string Description => "Slides down off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var presetOptions = MergeWithDefaultEase(options, Ease.InCubic);
            var ease = ResolveEase(presetOptions, Ease.InCubic);

            return t.DOLocalMoveY(t.localPosition.y - 500f, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target 500 units to the left off-screen, mirroring SlideInRight as an exit animation.
    /// <para>
    /// Animates local X position by <c>-500</c> using <c>DOLocalMoveX</c> with <c>Ease.InCubic</c>.
    /// The accelerating ease creates momentum as the element exits leftward. Does not return to origin.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Left-exit transition, swipe dismiss, carousel item exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutLeftPreset : CodePreset
    {
        public override string PresetName => "SlideOutLeft";
        public override string Description => "Slides left off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var presetOptions = MergeWithDefaultEase(options, Ease.InCubic);
            var ease = ResolveEase(presetOptions, Ease.InCubic);

            return t.DOLocalMoveX(t.localPosition.x - 500f, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target 500 units to the right off-screen, mirroring SlideInLeft as an exit animation.
    /// <para>
    /// Animates local X position by <c>+500</c> using <c>DOLocalMoveX</c> with <c>Ease.InCubic</c>.
    /// The accelerating ease creates momentum as the element exits rightward. Does not return to origin.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces InCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Right-exit transition, swipe dismiss, carousel item exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideOutRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideOutRightPreset : CodePreset
    {
        public override string PresetName => "SlideOutRight";
        public override string Description => "Slides right off-screen";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var presetOptions = MergeWithDefaultEase(options, Ease.InCubic);
            var ease = ResolveEase(presetOptions, Ease.InCubic);

            return t.DOLocalMoveX(t.localPosition.x + 500f, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target upward from 100 units below while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// Offsets initial position by <c>Vector3.down * 100</c> and sets alpha to <c>0</c>.
    /// Builds a parallel sequence: position animates over full duration with <c>Ease.OutCubic</c>,
    /// fade animates over 70% duration with <c>Ease.Linear</c>. Same pattern as SlideInFadeDown but from below.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls movement; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> List item stagger entrance, card reveal, content section entrance, bottom-up reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeUpPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeUp";
        public override string Description => "Slides up from below with fade in";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.down * 100f;

            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            var seq = DOTween.Sequence();
            seq.Append(t.DOLocalMove(targetPos, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 1f, dur * 0.7f);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slides the target downward from 100 units above while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// Offsets initial position by <c>Vector3.up * 100</c> and sets alpha to <c>0</c>.
    /// Builds a parallel sequence: position animates over full duration with <c>Ease.OutCubic</c>,
    /// fade animates over 70% duration with <c>Ease.Linear</c>. Same pattern as SlideInFadeUp but from above.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls movement; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Dropdown content reveal, notification entrance, header slide-in, top-down reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeDown").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeDownPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeDown";
        public override string Description => "Slides down from above with fade in";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.up * 100f;

            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            var seq = DOTween.Sequence();
            seq.Append(t.DOLocalMove(targetPos, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 1f, dur * 0.7f);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slides the target in from 100 units to the left while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// Offsets initial position by <c>Vector3.left * 100</c> and sets alpha to <c>0</c>.
    /// Builds a parallel sequence: position animates over full duration with <c>Ease.OutCubic</c>,
    /// fade animates over 70% duration with <c>Ease.Linear</c>. Same pattern as SlideInFadeUp but from the left.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls movement; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Side panel reveal, horizontal content entrance, left-to-right reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeLeft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeLeftPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeLeft";
        public override string Description => "Slides in from the left with fade in";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.left * 100f;

            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            var seq = DOTween.Sequence();
            seq.Append(t.DOLocalMove(targetPos, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 1f, dur * 0.7f);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Slides the target in from 100 units to the right while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// Offsets initial position by <c>Vector3.right * 100</c> and sets alpha to <c>0</c>.
    /// Builds a parallel sequence: position animates over full duration with <c>Ease.OutCubic</c>,
    /// fade animates over 70% duration with <c>Ease.Linear</c>. Same pattern as SlideInFadeUp but from the right.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutCubic (move), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls movement; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Side panel reveal, horizontal content entrance, right-to-left reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SlideInFadeRight").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SlideInFadeRightPreset : CodePreset
    {
        public override string PresetName => "SlideInFadeRight";
        public override string Description => "Slides in from the right with fade in";
        public override float DefaultDuration => 1.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetPos = t.localPosition;
            t.localPosition = targetPos + Vector3.right * 100f;

            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            var seq = DOTween.Sequence();
            seq.Append(t.DOLocalMove(targetPos, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 1f, dur * 0.7f);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }
}
