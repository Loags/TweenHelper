using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    /// <summary>
    /// Built-in animation presets demonstrating how to create reusable animations.
    /// These are auto-registered at runtime via [AutoRegisterPreset] attribute.
    /// </summary>

    #region Scale Animations

    /// <summary>
    /// Scales the target from zero to its original scale with elastic overshoot, creating a snappy entrance effect.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, then animates to the stored original scale using
    /// <c>Ease.OutBack</c> with an overshoot parameter of <c>1.7</c> (above the default 1.0,
    /// producing a noticeable but controlled overshoot).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutBack (1.7 overshoot)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutBack.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element entrance, item spawn, notification pop-up, dialog appearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInPreset : CodePreset
    {
        public override string PresetName => "PopIn";
        public override string Description => "Scales from 0 to original scale with overshoot";
        public override float DefaultDuration => 0.6f;
        public override string Category => PresetCategories.Base;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutBack);
            var ease = ResolveEase(presetOptions, Ease.OutBack);

            // OutBack with overshoot parameter (1 = default, higher = more overshoot)
            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 1.7f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target down to zero with a brief anticipation pull-back, creating a satisfying exit effect.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.InBack</c>, which briefly scales slightly
    /// larger before shrinking to zero, giving a natural "wind-up" feel.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> InBack<br/>
    /// <b>Easing override:</b> Primary ease replaces InBack.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element dismissal, item collection, dialog close, notification dismiss.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutPreset : CodePreset
    {
        public override string PresetName => "PopOut";
        public override string Description => "Scales to 0 with anticipation";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Base;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);
            return target.transform.DOScale(Vector3.zero, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Applies a quick scale punch for tactile feedback, snapping outward then settling back to original scale.
    /// <para>
    /// Uses <c>DOPunchScale</c> with punch vector <c>Vector3.one * 0.15</c>, vibrato <c>6</c>,
    /// and elasticity <c>0.7</c>. The punch creates a brief scale burst that oscillates and decays.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.2s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Button press feedback, hit confirmation, score increment, interaction acknowledgment.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Punch").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PunchPreset : CodePreset
    {
        public override string PresetName => "Punch";
        public override string Description => "Quick scale punch for feedback";
        public override float DefaultDuration => 0.2f;
        public override string Category => PresetCategories.Base;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchScale(Vector3.one * 0.15f, GetDuration(duration), 6, 0.7f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Performs a squash-and-stretch scale pulse, compressing and elongating before settling back to original scale.
    /// <para>
    /// Builds a 4-step sequence: (1) squash wide/short <c>(1.3x, 0.7y)</c> at 15% duration,
    /// (2) stretch tall/narrow <c>(0.8x, 1.2y)</c> at 15%, (3) slight overshoot <c>(1.1x, 0.9y)</c> at 15%,
    /// (4) settle to original scale at 25% with <c>Ease.OutElastic</c>. Z scale is unchanged.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutElastic (final step only)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutElastic on the final settle step.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Character land impact, rubber-ball effect, playful UI feedback, cartoon-style emphasis.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Squash").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BouncePreset : CodePreset
    {
        public override string PresetName => "Squash";
        public override string Description => "Squash and stretch pulse";
        public override float DefaultDuration => 0.6f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutElastic);
            var ease = ResolveEase(presetOptions, Ease.OutElastic);

            // Squash and stretch style bounce
            return DOTween.Sequence()
                .Append(t.DOScale(new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(originalScale, dur * 0.25f).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Continuously pulses scale between original and 1.08x in a smooth breathing rhythm using callback-chain looping.
    /// <para>
    /// Alternates between expanding to <c>originalScale * 1.08</c> and contracting back to original scale,
    /// each leg taking half the total duration. Uses callback-chain looping (not DOTween's built-in loops)
    /// so delay is applied only on the first cycle via <c>WithLoopDefaults</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 4.0s (2.0s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls expand leg; secondary ease controls contract leg.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Idle animation, living object indicator, ambient pulsing, selectable item highlight.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Breathe").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BreathePreset : CodePreset
    {
        public override string PresetName => "Breathe";
        public override string Description => "Gentle scale pulse loop";
        public override float DefaultDuration => 4.0f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var halfDur = GetDuration(duration) * 0.5f;
            var expandEase = options.Ease ?? Ease.InOutSine;
            var contractEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var upOptions = MergeWithDefaultEase(options.SetEase(expandEase), expandEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(contractEase), contractEase);
            bool applyDelay = true;

            Tween tween = null;

            void Expand()
            {
                tween = t.DOScale(originalScale * 1.08f, halfDur)
                    .SetEase(expandEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(Contract);
            }

            void Contract()
            {
                tween = t.DOScale(originalScale, halfDur)
                    .SetEase(contractEase)
                    .WithLoopDefaults(downOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(Expand);
            }

            Expand();

            return tween;
        }
    }

    /// <summary>
    /// Simulates a heartbeat with two scale pulses per cycle: a smaller beat followed by a larger beat, then a rest pause.
    /// <para>
    /// Each cycle is a sequence: (1) scale to <c>1.15x</c> at 12% duration, (2) return to original at 12%,
    /// (3) scale to <c>1.25x</c> at 12%, (4) return to original at 14%, (5) pause for 50% duration.
    /// Uses callback-chain looping with a new sequence per cycle; delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain, sequence per cycle) | <b>Default duration:</b> 0.8s per cycle | <b>Default ease:</b> OutQuad (beat), InQuad (return)<br/>
    /// <b>Easing override:</b> Primary ease controls beat pulses; secondary ease controls return-to-rest phases.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Health indicator, living entity, emotional feedback, interactive object highlight.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Heartbeat").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class HeartbeatPreset : CodePreset
    {
        public override string PresetName => "Heartbeat";
        public override string Description => "Double-pulse heartbeat loop";
        public override float DefaultDuration => 0.8f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var beatEase = options.Ease ?? Ease.OutQuad;
            var returnEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var loopOptions = MergeWithDefaultEase(options, beatEase);
            bool applyDelay = true;

            Tween tween = null;

            void Beat()
            {
                var seq = DOTween.Sequence()
                    // First beat (smaller)
                    .Append(t.DOScale(originalScale * 1.15f, dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.12f).SetEase(returnEase))
                    // Second beat (larger)
                    .Append(t.DOScale(originalScale * 1.25f, dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.14f).SetEase(returnEase))
                    // Pause before next cycle
                    .AppendInterval(dur * 0.5f)
                    .WithLoopDefaults(loopOptions, target, applyDelay);

                applyDelay = false;
                tween = seq;
                seq.OnComplete(Beat);
            }

            Beat();

            return tween;
        }
    }

    /// <summary>
    /// Scales from zero to original with a tight elastic snap, producing rapid oscillation before settling.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>, then animates to original scale using
    /// <c>Ease.OutElastic</c> with amplitude <c>0.7</c> and period <c>0.3</c>.
    /// The low period produces faster oscillations than the default elastic, creating a snappy, spring-like feel.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutElastic (amplitude 0.7, period 0.3)<br/>
    /// <b>Easing override:</b> Primary ease replaces OutElastic (amplitude/period parameters still apply).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Springy UI element entrance, badge pop-in, slot-machine result, snappy icon appearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ElasticSnap").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ElasticSnapPreset : CodePreset
    {
        public override string PresetName => "ElasticSnap";
        public override string Description => "Scale from 0 with tight elastic oscillation";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutElastic);
            var ease = ResolveEase(presetOptions, Ease.OutElastic);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 0.7f, 0.3f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales cleanly from zero to original scale without overshoot, providing a smooth entrance.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and animates to original scale using <c>Ease.OutCubic</c>.
    /// Unlike PopIn (which uses OutBack with overshoot), this preset decelerates smoothly to the final value
    /// without exceeding it, giving a more subtle and professional entrance.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Subtle UI entrance, tooltip appearance, menu item reveal, understated element show.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("GrowIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class GrowInPreset : CodePreset
    {
        public override string PresetName => "GrowIn";
        public override string Description => "Clean scale from 0 to original, no overshoot";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales cleanly to zero without anticipation overshoot, providing a smooth exit.
    /// <para>
    /// Animates scale to <c>Vector3.zero</c> using <c>Ease.OutCubic</c>.
    /// Unlike PopOut (which uses InBack for a brief wind-up overshoot), this preset shrinks directly
    /// and smoothly to zero with no anticipation, creating a clean, understated disappearance.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Subtle UI dismissal, tooltip hide, quiet element removal, clean fade-to-nothing.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShrinkOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShrinkOutPreset : CodePreset
    {
        public override string PresetName => "ShrinkOut";
        public override string Description => "Clean scale to zero, no anticipation";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            return target.transform.DOScale(Vector3.zero, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Performs a quick scale bump up to 1.2x then back to original, ideal for UI tap/click feedback.
    /// <para>
    /// Builds a 2-step sequence: (1) scale to <c>originalScale * 1.2</c> over 40% duration with <c>Ease.OutQuad</c>,
    /// (2) return to original scale over 60% duration with <c>Ease.InQuad</c>.
    /// The asymmetric timing (fast up, slower down) creates a natural pulse feel.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls scale-up; secondary ease controls scale-down.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Button tap feedback, toggle state change, counter increment, selection highlight.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseScale").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseScalePreset : CodePreset
    {
        public override string PresetName => "PulseScale";
        public override string Description => "Quick scale bump for UI feedback";
        public override float DefaultDuration => 0.3f;
        public override string Category => PresetCategories.Scale;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.2f, dur * 0.4f).SetEase(upEase))
                .Append(t.DOScale(originalScale, dur * 0.6f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    #endregion

    #region Position Animations

    /// <summary>
    /// Shakes the target's position randomly using DOTween's built-in shake with moderate strength and high vibrato.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.3</c>, vibrato <c>15</c>, randomness <c>90°</c>,
    /// snapping disabled, and fade-out enabled. The high vibrato produces many oscillations within
    /// the duration, creating a convincing tremor effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween shake default<br/>
    /// <b>Easing override:</b> No ease override (shake tweens use internal decay).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Impact feedback, earthquake, error shake, camera shake, damage indication.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Shake").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakePreset : CodePreset
    {
        public override string PresetName => "Shake";
        public override string Description => "Random position shake";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.3f, 15, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Slides the target downward from 500 units above its current local position to its original position.
    /// <para>
    /// Offsets initial local position by <c>Vector3.up * 500</c>, then animates to the stored target position
    /// using <c>DOLocalMove</c> with <c>Ease.OutCubic</c>. The 500-unit offset is designed for UI/canvas
    /// coordinate systems where elements slide in from off-screen.
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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
    /// Gently bobs the target up and down on the Y axis in a continuous hovering loop using callback-chain looping.
    /// <para>
    /// Alternates between moving <c>+0.5</c> and <c>-0.5</c> on local Y (relative), each leg taking
    /// half the total duration. Uses <c>SetRelative(true)</c> for position-independent movement.
    /// Callback-chain looping ensures delay is applied only on the first cycle via <c>WithLoopDefaults</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 6.0s (3.0s per leg) | <b>Default ease:</b> InOutCubic<br/>
    /// <b>Easing override:</b> Primary ease controls upward leg; secondary ease controls downward leg.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Floating collectible, hover indicator, ambient object bob, idle character float.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Float").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FloatPreset : CodePreset
    {
        public override string PresetName => "Float";
        public override string Description => "Gentle up/down hovering loop";
        public override float DefaultDuration => 6f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var halfDur = GetDuration(duration) * 0.5f;
            var moveUpEase = options.Ease ?? Ease.InOutCubic;
            var moveDownEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutCubic;

            // Ensure each leg uses its own ease without being overwritten by global defaults.
            var upOptions = MergeWithDefaultEase(options.SetEase(moveUpEase), moveUpEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(moveDownEase), moveDownEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.5f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveUpEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.5f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveDownEase)
                    .WithLoopDefaults(downOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveUp);
            }

            MoveUp();

            return tween;
        }
    }

    /// <summary>
    /// Abstract base class providing shared orbit logic with configurable direction and radius interpolation.
    /// <para>
    /// Exposes serialized <c>startRadius</c> and <c>endRadius</c> fields (default 1.0 each) and a
    /// <c>CreateOrbitTween</c> helper that delegates to <c>OrbitTweenFactory</c>. Subclasses only need
    /// to specify the direction (clockwise or counter-clockwise).
    /// </para>
    /// </summary>
    public abstract class OrbitBasePreset : CodePreset
    {
        [SerializeField] private float startRadius = 1f;
        [SerializeField] private float endRadius = 1f;

        protected Tween CreateOrbitTween(GameObject target, float? duration, TweenOptions options, bool clockwise, float? startRadiusOverride = null, float? endRadiusOverride = null)
        {
            float startR = Mathf.Max(0.01f, startRadiusOverride ?? startRadius);
            float endR = Mathf.Max(0.01f, endRadiusOverride ?? endRadius);
            return OrbitTweenFactory.Create(target, duration, options, clockwise, startR, endR);
        }
    }

    internal static class OrbitTweenFactory
    {
        public static Tween Create(GameObject target, float? duration, TweenOptions options, bool clockwise, float startRadius, float endRadius)
        {
            var t = target.transform;
            var rb = target.GetComponent<Rigidbody>();

            startRadius = Mathf.Max(0.01f, startRadius);
            endRadius = Mathf.Max(0.01f, endRadius);

            float dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var initialCenter = t.position;
            float direction = clockwise ? -1f : 1f;
            const float fullCycle = Mathf.PI * 2f;
            var loopOptions = options;
            var orbitEase = loopOptions.Ease ?? Ease.Linear;
            if (!loopOptions.Ease.HasValue)
            {
                loopOptions = loopOptions.SetEase(orbitEase);
            }
            loopOptions = loopOptions.SetUpdateType(UpdateType.Fixed);

            bool applyDelay = true;
            Tween tween = null;

            Tween CreateCycle()
            {
                tween = DOVirtual.Float(0f, fullCycle, dur, angle =>
                    {
                        var normalized = Mathf.Repeat(angle / fullCycle, 1f);
                        float radius = Mathf.Lerp(startRadius, endRadius, normalized);
                        float directedAngle = angle * direction;

                        Vector3 offset = new Vector3(
                            Mathf.Cos(directedAngle) * radius,
                            0f,
                            Mathf.Sin(directedAngle) * radius
                        );

                        Vector3 targetPos = initialCenter + offset;

                        if (rb && rb.isKinematic == false)
                        {
                            rb.MovePosition(targetPos);
                        }
                        else
                        {
                            t.position = targetPos;
                        }
                    })
                    .SetEase(orbitEase)
                    .SetUpdate(UpdateType.Fixed)
                    .WithLoopDefaults(loopOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(() => CreateCycle());
                return tween;
            }

            return CreateCycle();
        }
    }

    /// <summary>
    /// Orbits the target counter-clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Uses <c>DOVirtual.Float</c> to drive a parametric circle (cos/sin) on X/Z with configurable start/end radii
    /// (default 1.0). Supports Rigidbody targets via <c>MovePosition</c>. Runs on <c>UpdateType.Fixed</c>.
    /// Callback-chain looping restarts each full 2π cycle; delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression (Linear recommended for uniform speed).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Orbiting satellite, patrol path, circular particle motion, planetary display.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Orbit").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitPreset : OrbitBasePreset
    {
        public override string PresetName => "Orbit";
        public override string Description => "Circles around a point on XZ plane (counter-clockwise)";
        public override float DefaultDuration => 2f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false);
        }
    }

    /// <summary>
    /// Orbits the target clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Identical to <see cref="OrbitPreset"/> but with reversed direction (clockwise). Uses the shared
    /// <c>OrbitTweenFactory</c> with <c>clockwise: true</c>, inverting the angular direction.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Clockwise patrol, reversed orbital motion, mirrored satellite.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitClockwisePreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitClockwise";
        public override string Description => "Circles around a point on XZ plane (clockwise)";
        public override float DefaultDuration => 2f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: true);
        }
    }

    /// <summary>
    /// Orbits the target counter-clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Explicit counter-clockwise variant of <see cref="OrbitPreset"/>. Functionally identical to Orbit
    /// but provides a semantically clear name when both directions are used in the same project.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Counter-clockwise patrol, explicit direction pairing with OrbitClockwise.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitCounterClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitCounterClockwisePreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitCounterClockwise";
        public override string Description => "Circles around a point on XZ plane (counter-clockwise)";
        public override float DefaultDuration => 2f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false);
        }
    }

    /// <summary>
    /// Spirals the target upward in a helical path that tightens as it rises, combining circular XZ motion with vertical climb.
    /// <para>
    /// Uses <c>DOTween.To</c> on a 0→1 progress value. At each frame, computes a helix position:
    /// X/Z from cos/sin of <c>progress * 2π * turns</c> (default 2 turns) scaled by <c>radius * (1 - progress)</c>
    /// (shrinking radius), Y from <c>progress * height</c> (default 2 units). Serialized fields:
    /// <c>turns=2</c>, <c>height=2</c>, <c>radius=0.5</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.5s | <b>Default ease:</b> OutQuad<br/>
    /// <b>Easing override:</b> Primary ease controls progress curve (affects spiral speed distribution).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Item ascension, magical effect, tornado-style rise, collectible pickup trail.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Spiral").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpiralPreset : CodePreset
    {
        public override string PresetName => "Spiral";
        public override string Description => "Spirals upward combining rotation and height";
        public override float DefaultDuration => 1.5f;
        public override string Category => PresetCategories.Movement;

        [SerializeField] private float turns = 2f;
        [SerializeField] private float height = 2f;
        [SerializeField] private float radius = 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var startPos = t.localPosition;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutQuad);
            var ease = ResolveEase(presetOptions, Ease.OutQuad);

            float progress = 0f;
            return DOTween.To(() => progress, x =>
                {
                    progress = x;
                    var rad = progress * Mathf.PI * 2f * turns;
                    t.localPosition = startPos + new Vector3(
                        Mathf.Cos(rad) * radius * (1f - progress),
                        progress * height,
                        Mathf.Sin(rad) * radius * (1f - progress)
                    );
                }, 1f, dur)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Drops the target from 8 units above with three progressively smaller bounces on landing.
    /// <para>
    /// Offsets Y by <c>+8</c>, then builds a 7-step Y-axis sequence: fall to target (40%), then three
    /// bounce pairs (up/down) with decreasing heights: 30%→10%→3% of drop height. Fall phases use
    /// <c>Ease.InQuad</c> (accelerating), bounce-up phases use <c>Ease.OutQuad</c> (decelerating).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InQuad (fall), OutQuad (bounce)<br/>
    /// <b>Easing override:</b> Primary ease controls fall phases; secondary ease controls bounce-up phases.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Object drop entrance, physics-style landing, dramatic item reveal, game piece placement.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("DropIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class DropInPreset : CodePreset
    {
        public override string PresetName => "DropIn";
        public override string Description => "Falls from above with bounce on landing";
        public override float DefaultDuration => 1.2f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var targetY = t.localPosition.y;
            var dropHeight = 8f;
            t.localPosition = t.localPosition + Vector3.up * dropHeight;

            var dur = GetDuration(duration);
            var fallEase = ResolveEase(options, Ease.InQuad);
            var bounceEase = ResolveSecondaryEase(options, Ease.OutQuad);
            var presetOptions = MergeWithDefaultEase(options.SetEase(fallEase), fallEase);

            // Manual bounce: fall fast, then bounce up/down with decreasing height
            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(targetY, dur * 0.4f).SetEase(fallEase)) // Fall
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.3f, dur * 0.15f).SetEase(bounceEase)) // Bounce 1 up
                .Append(t.DOLocalMoveY(targetY, dur * 0.15f).SetEase(fallEase)) // Bounce 1 down
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.1f, dur * 0.1f).SetEase(bounceEase)) // Bounce 2 up
                .Append(t.DOLocalMoveY(targetY, dur * 0.1f).SetEase(fallEase)) // Bounce 2 down
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.03f, dur * 0.05f).SetEase(bounceEase)) // Bounce 3 up
                .Append(t.DOLocalMoveY(targetY, dur * 0.05f).SetEase(fallEase)) // Bounce 3 down
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Launches the target 3 units upward from its current local Y position with a decelerating ease.
    /// <para>
    /// Animates local Y position by <c>+3</c> using <c>DOLocalMoveY</c> with <c>Ease.OutCubic</c>.
    /// The deceleration simulates a projectile losing momentum as it rises. Does not return to origin.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutCubic<br/>
    /// <b>Easing override:</b> Primary ease replaces OutCubic.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Projectile launch, item toss, upward exit, jump start.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("LaunchUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class LaunchUpPreset : CodePreset
    {
        public override string PresetName => "LaunchUp";
        public override string Description => "Quick upward motion with ease-out";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);

            return t.DOLocalMoveY(t.localPosition.y + 3f, GetDuration(duration))
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target 500 units upward off-screen, mirroring SlideInDown as an exit animation.
    /// <para>
    /// Animates local Y position by <c>+500</c> using <c>DOLocalMoveY</c> with <c>Ease.InCubic</c>.
    /// The accelerating ease creates momentum as the element exits. Does not return to origin.
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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
    /// Gently sways the target left and right on the X axis in a continuous loop, the horizontal equivalent of Float.
    /// <para>
    /// Alternates between moving <c>+0.5</c> and <c>-0.5</c> on local X (relative), each leg taking
    /// half the total duration. Uses <c>SetRelative(true)</c> for position-independent movement.
    /// Callback-chain looping ensures delay is applied only on the first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 4.0s (2.0s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls rightward leg; secondary ease controls leftward leg.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Pendulum motion, hanging object sway, idle animation, ambient horizontal drift.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Sway").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwayPreset : CodePreset
    {
        public override string PresetName => "Sway";
        public override string Description => "Gentle horizontal sway loop";
        public override float DefaultDuration => 4.0f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var halfDur = GetDuration(duration) * 0.5f;
            var moveRightEase = options.Ease ?? Ease.InOutSine;
            var moveLeftEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var rightOptions = MergeWithDefaultEase(options.SetEase(moveRightEase), moveRightEase);
            var leftOptions = MergeWithDefaultEase(options.SetEase(moveLeftEase), moveLeftEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveRight()
            {
                tween = t.DOLocalMoveX(0.5f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveRightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveLeft);
            }

            void MoveLeft()
            {
                tween = t.DOLocalMoveX(-0.5f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveLeftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveRight);
            }

            MoveRight();

            return tween;
        }
    }

    /// <summary>
    /// Bounces the target on the Y axis with three progressively smaller hops using a sequence.
    /// <para>
    /// Builds a 6-step Y-axis sequence from the current base Y: hop 1 rises <c>+1.5</c> (15% duration each way),
    /// hop 2 rises <c>+0.8</c> (12% each way), hop 3 rises <c>+0.3</c> (10% each way).
    /// Up phases use <c>Ease.OutQuad</c> (decelerating rise), down phases use <c>Ease.InQuad</c> (accelerating fall).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> OutQuad (up), InQuad (down)<br/>
    /// <b>Easing override:</b> Primary ease controls up phases; secondary ease controls down phases.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Ball bounce, character hop, playful feedback, item arrival.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Bounce").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PositionalBouncePreset : CodePreset
    {
        public override string PresetName => "Bounce";
        public override string Description => "Positional Y bounce with decreasing hops";
        public override float DefaultDuration => 0.8f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var baseY = t.localPosition.y;
            var dur = GetDuration(duration);
            var upEase = ResolveEase(options, Ease.OutQuad);
            var downEase = ResolveSecondaryEase(options, Ease.InQuad);
            var presetOptions = MergeWithDefaultEase(options, upEase);

            return DOTween.Sequence()
                // Hop 1 (highest)
                .Append(t.DOLocalMoveY(baseY + 1.5f, dur * 0.15f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.15f).SetEase(downEase))
                // Hop 2 (medium)
                .Append(t.DOLocalMoveY(baseY + 0.8f, dur * 0.12f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.12f).SetEase(downEase))
                // Hop 3 (small)
                .Append(t.DOLocalMoveY(baseY + 0.3f, dur * 0.1f).SetEase(upEase))
                .Append(t.DOLocalMoveY(baseY, dur * 0.1f).SetEase(downEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Applies tight, rapid positional vibration using high-frequency shake with very low amplitude.
    /// <para>
    /// Uses <c>DOShakePosition</c> with strength <c>0.08</c>, vibrato <c>40</c>, randomness <c>90°</c>,
    /// snapping disabled, and fade-out enabled. The very low strength with extremely high vibrato produces
    /// a subtle, nervous tremor rather than a dramatic shake.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.3s | <b>Default ease:</b> DOTween shake default<br/>
    /// <b>Easing override:</b> No ease override (shake tweens use internal decay).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Nervous tremor, electrical buzz, cold shiver, error micro-shake.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Jitter").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class JitterPreset : CodePreset
    {
        public override string PresetName => "Jitter";
        public override string Description => "Tight rapid vibration";
        public override float DefaultDuration => 0.3f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOShakePosition(GetDuration(duration), 0.08f, 40, 90f, false, true)
                .WithDefaults(options, target);
        }
    }

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
        public override string Category => PresetCategories.Movement;

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
        public override string Category => PresetCategories.Movement;

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
    /// Orbits the target in a circle on the XY plane (2D-friendly) using callback-chain looping.
    /// <para>
    /// Uses <c>DOVirtual.Float</c> driving a parametric circle with cos on X and sin on Y,
    /// radius of <c>1.0</c>, centered on the starting world position. Unlike the XZ-plane Orbit presets,
    /// this operates on XY for 2D games. Callback-chain looping restarts each full 2π cycle;
    /// delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D orbiting object, circular indicator, shield rotation, radial menu animation.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Orbit2D").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class Orbit2DPreset : CodePreset
    {
        public override string PresetName => "Orbit2D";
        public override string Description => "Circular orbit on XY plane";
        public override float DefaultDuration => 2.0f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            float dur = GetDuration(duration);
            var initialCenter = t.position;
            const float radius = 1f;
            const float fullCycle = Mathf.PI * 2f;
            var orbitEase = options.Ease ?? Ease.Linear;
            var loopOptions = options;
            if (!loopOptions.Ease.HasValue)
            {
                loopOptions = loopOptions.SetEase(orbitEase);
            }

            bool applyDelay = true;
            Tween tween = null;

            Tween CreateCycle()
            {
                tween = DOVirtual.Float(0f, fullCycle, dur, angle =>
                    {
                        Vector3 offset = new Vector3(
                            Mathf.Cos(angle) * radius,
                            Mathf.Sin(angle) * radius,
                            0f
                        );
                        t.position = initialCenter + offset;
                    })
                    .SetEase(orbitEase)
                    .WithLoopDefaults(loopOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(() => CreateCycle());
                return tween;
            }

            return CreateCycle();
        }
    }

    /// <summary>
    /// Moves the target in an alternating diagonal zig-zag pattern: right-up, left-up, right-up.
    /// <para>
    /// Builds a 3-step relative sequence, each step taking 1/3 of the total duration:
    /// (1) move <c>(+0.5, +0.5, 0)</c>, (2) move <c>(-1.0, +0.5, 0)</c>, (3) move <c>(+0.5, +0.5, 0)</c>.
    /// All steps use <c>SetRelative(true)</c> with <c>Ease.InOutSine</c>. The net displacement is <c>(0, +1.5, 0)</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect (non-returning) | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutSine on all steps.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Evasive movement, playful path, snake-like motion, decorative trail.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ZigZag").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ZigZagPreset : CodePreset
    {
        public override string PresetName => "ZigZag";
        public override string Description => "Alternating diagonal zig-zag movement";
        public override float DefaultDuration => 1.0f;
        public override string Category => PresetCategories.Movement;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutSine);
            var ease = ResolveEase(presetOptions, Ease.InOutSine);
            var stepDur = dur / 3f;

            return DOTween.Sequence()
                .Append(t.DOLocalMove(new Vector3(0.5f, 0.5f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(-1f, 0.5f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(0.5f, 0.5f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    #endregion

    #region Fade Animations

    /// <summary>
    /// Fades the target in from fully transparent (alpha 0) to fully opaque (alpha 1) with a slow-start ease.
    /// <para>
    /// Sets initial alpha to <c>0</c> via <c>SetAlpha</c>, then creates a fade tween to <c>1.0</c>
    /// using <c>Ease.InQuad</c>. The accelerating ease prevents the object from appearing visible too
    /// quickly at the start. Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 3.0s | <b>Default ease:</b> InQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InQuad.<br/>
    /// <b>Requires:</b> A fadeable component (CanvasGroup, SpriteRenderer, Image, Text, or Renderer with material).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Scene transition reveal, gradual element appearance, cinematic fade-in.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInPreset : CodePreset
    {
        public override string PresetName => "FadeIn";
        public override string Description => "Fades in from transparent (requires fadeable component)";
        public override float DefaultDuration => 3f;
        public override string Category => PresetCategories.Fade;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            SetAlpha(target, 0f);
            var tween = CreateFadeTween(target, 1f, GetDuration(duration));
            // Slow start to avoid appearing fully visible too early
            var presetOptions = MergeWithDefaultEase(options, Ease.InQuad);
            var ease = ResolveEase(presetOptions, Ease.InQuad);
            return tween?.SetEase(ease).WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Fades the target out from its current alpha to fully transparent (alpha 0).
    /// <para>
    /// Creates a fade tween to <c>0.0</c> using the default ease from <c>TweenHelperSettings</c>.
    /// Does not override the initial alpha — fades from whatever the current value is.
    /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 3.0s | <b>Default ease:</b> Settings default<br/>
    /// <b>Easing override:</b> Standard options apply via <c>WithDefaults</c>.<br/>
    /// <b>Requires:</b> A fadeable component (CanvasGroup, SpriteRenderer, Image, Text, or Renderer with material).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Scene transition fade-out, element dismissal, death effect, gradual disappearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeOutPreset : CodePreset
    {
        public override string PresetName => "FadeOut";
        public override string Description => "Fades out to transparent (requires fadeable component)";
        public override float DefaultDuration => 3f;
        public override string Category => PresetCategories.Fade;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var tween = CreateFadeTween(target, 0f, GetDuration(duration));
            return tween?.WithDefaults(options, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Rapidly toggles alpha between fully opaque and fully transparent in a continuous loop.
    /// <para>
    /// Alternates between fading to <c>0</c> and fading to <c>1</c>, each leg taking half the total duration.
    /// Uses callback-chain looping so delay applies only on the first cycle.
    /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer materials.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 0.4s (0.2s per leg) | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease controls fade-off; secondary ease controls fade-on.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Invincibility blink, warning indicator, cursor blink, selection flash.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Blink").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BlinkPreset : CodePreset
    {
        public override string PresetName => "Blink";
        public override string Description => "Rapid alpha on/off loop";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Fade;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration) * 0.5f;
            var offEase = options.Ease ?? Ease.InOutQuad;
            var onEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutQuad;

            var offOptions = MergeWithDefaultEase(options.SetEase(offEase), offEase);
            var onOptions = MergeWithDefaultEase(options.SetEase(onEase), onEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeOff()
            {
                var fadeTween = CreateFadeTween(target, 0f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(offEase)
                    .WithLoopDefaults(offOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOn);
            }

            void FadeOn()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(onEase)
                    .WithLoopDefaults(onOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeOff);
            }

            FadeOff();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Smoothly pulses alpha between fully opaque and 30% opacity in a continuous loop.
    /// <para>
    /// Alternates between fading down to <c>0.3</c> and fading back to <c>1.0</c>, each leg taking
    /// half the total duration. Unlike Blink (which goes to 0), this maintains partial visibility
    /// throughout. Uses callback-chain looping; delay applies only on the first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s (1.0s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls fade-down; secondary ease controls fade-up.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Breathing glow, selection highlight pulse, ambient object shimmer, status indicator.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PulseFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PulseFadePreset : CodePreset
    {
        public override string PresetName => "PulseFade";
        public override string Description => "Smooth alpha pulse loop";
        public override float DefaultDuration => 2.0f;
        public override string Category => PresetCategories.Fade;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var halfDur = GetDuration(duration) * 0.5f;
            var fadeOutEase = options.Ease ?? Ease.InOutSine;
            var fadeInEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var outOptions = MergeWithDefaultEase(options.SetEase(fadeOutEase), fadeOutEase);
            var inOptions = MergeWithDefaultEase(options.SetEase(fadeInEase), fadeInEase);
            bool applyDelay = true;

            Tween tween = null;

            void FadeDown()
            {
                var fadeTween = CreateFadeTween(target, 0.3f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeOutEase)
                    .WithLoopDefaults(outOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeUp);
            }

            void FadeUp()
            {
                var fadeTween = CreateFadeTween(target, 1f, halfDur);
                if (fadeTween == null) return;
                tween = fadeTween
                    .SetEase(fadeInEase)
                    .WithLoopDefaults(inOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(FadeDown);
            }

            FadeDown();

            return tween;
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Produces randomized alpha flickering using Perlin noise, snapping to full opacity on completion.
    /// <para>
    /// Uses <c>DOVirtual.Float(0, 1, dur)</c> with a per-invocation random seed to sample
    /// <c>Mathf.PerlinNoise(seed + t * 12, 0)</c> each frame, directly setting the target's alpha.
    /// The <c>12x</c> time multiplier produces rapid, organic fluctuations. On complete, alpha is
    /// explicitly set to <c>1.0</c> to ensure a clean final state.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> Linear (progress)<br/>
    /// <b>Easing override:</b> Ease has minimal visible effect (controls progress, not noise).<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Torch/campfire flicker, electrical malfunction, glitch effect, dying light.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Flicker").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlickerPreset : CodePreset
    {
        public override string PresetName => "Flicker";
        public override string Description => "Randomized alpha flicker effect";
        public override float DefaultDuration => 1.0f;
        public override string Category => PresetCategories.Fade;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);
            float seed = Random.Range(0f, 100f);

            return DOVirtual.Float(0f, 1f, dur, t =>
                {
                    float noise = Mathf.PerlinNoise(seed + t * 12f, 0f);
                    SetAlpha(target, noise);
                })
                .SetEase(Ease.Linear)
                .OnComplete(() => SetAlpha(target, 1f))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    /// <summary>
    /// Fades in from transparent to opaque in the first half, then fades back out to transparent in the second half.
    /// <para>
    /// Sets initial alpha to <c>0</c>, then builds a 2-step sequence: (1) fade to <c>1.0</c> over 50% duration,
    /// (2) fade to <c>0.0</c> over 50% duration. Returns null if the target lacks a fadeable component.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 2.0s | <b>Default ease:</b> InOutSine (both phases)<br/>
    /// <b>Easing override:</b> Primary ease controls fade-in; secondary ease controls fade-out.<br/>
    /// <b>Requires:</b> A fadeable component.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Temporary reveal, ghost appearance, flash highlight, transient notification.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FadeInOut").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInOutPreset : CodePreset
    {
        public override string PresetName => "FadeInOut";
        public override string Description => "Fade in then fade out";
        public override float DefaultDuration => 2.0f;
        public override string Category => PresetCategories.Fade;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var fadeInEase = ResolveEase(options, Ease.InOutSine);
            var fadeOutEase = ResolveSecondaryEase(options, Ease.InOutSine);
            var presetOptions = MergeWithDefaultEase(options, fadeInEase);

            SetAlpha(target, 0f);

            var fadeIn = CreateFadeTween(target, 1f, dur * 0.5f);
            var fadeOut = CreateFadeTween(target, 0f, dur * 0.5f);

            if (fadeIn == null || fadeOut == null) return null;

            return DOTween.Sequence()
                .Append(fadeIn.SetEase(fadeInEase))
                .Append(fadeOut.SetEase(fadeOutEase))
                .WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => CanFade(target);
    }

    #endregion

    #region Rotation Animations

    /// <summary>
    /// Spins the target a full 360 degrees around the Y axis using FastBeyond360 rotation mode.
    /// <para>
    /// Rotates to <c>(0, 360, 0)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// The InOutQuad ease creates a smooth acceleration/deceleration spin. FastBeyond360 prevents
    /// DOTween from clamping the rotation to the shortest path.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Coin spin, character turnaround, loading spinner, reveal rotation.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinPreset : CodePreset
    {
        public override string PresetName => "SpinY";
        public override string Description => "Spins 360 degrees on Y axis";
        public override float DefaultDuration => 1f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(0, 360f, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the X axis using FastBeyond360 rotation mode.
    /// <para>
    /// Rotates to <c>(360, 0, 0)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Creates a forward/backward tumble effect. Identical structure to SpinY but on the X axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Forward flip, tumble, barrel roll (pitch axis), acrobatic motion.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinXPreset : CodePreset
    {
        public override string PresetName => "SpinX";
        public override string Description => "Spins 360 degrees on X axis";
        public override float DefaultDuration => 1f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(360f, 0, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target a full 360 degrees around the Z axis using FastBeyond360 rotation mode.
    /// <para>
    /// Rotates to <c>(0, 0, 360)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Creates a clockface rotation effect. Ideal for 2D objects that rotate in-plane.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D spin, loading spinner, wheel rotation, propeller, compass needle.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinZPreset : CodePreset
    {
        public override string PresetName => "SpinZ";
        public override string Description => "Spins 360 degrees on Z axis";
        public override float DefaultDuration => 1f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(0, 0, 360f), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both X and Y axes, creating a diagonal tumble.
    /// <para>
    /// Rotates to <c>(360, 360, 0)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// The combined dual-axis rotation produces a complex, visually interesting tumbling motion.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Dramatic object tumble, acrobatic flip, item toss, complex rotation flourish.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXYPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXY";
        public override string Description => "Spins 360 degrees across X and Y axes";
        public override float DefaultDuration => 1f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(360f, 360f, 0f), dur, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both X and Z axes, creating a diagonal tumble.
    /// <para>
    /// Rotates to <c>(360, 0, 360)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Combines pitch and roll for an off-axis spinning effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Off-axis tumble, item toss, chaotic rotation, dramatic spinning exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalXZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalXZPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalXZ";
        public override string Description => "Spins 360 degrees across X and Z axes";
        public override float DefaultDuration => 1f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(360f, 0f, 360f), dur, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Spins the target 360 degrees simultaneously on both Y and Z axes, creating a diagonal tumble.
    /// <para>
    /// Rotates to <c>(0, 360, 360)</c> with <c>RotateMode.FastBeyond360</c> and <c>Ease.InOutQuad</c>.
    /// Combines yaw and roll for a sideways spinning tumble effect.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Sideways tumble, stylized rotation, coin flip variant, dynamic transition.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinDiagonalYZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinDiagonalYZPreset : CodePreset
    {
        public override string PresetName => "SpinDiagonalYZ";
        public override string Description => "Spins 360 degrees across Y and Z axes";
        public override float DefaultDuration => 1f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DORotate(new Vector3(0f, 360f, 360f), dur, RotateMode.FastBeyond360)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation back and forth on the Y axis using DOTween's punch rotation.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 15, 0)</c> (15° amplitude), vibrato <c>8</c>,
    /// and elasticity <c>0.5</c>. The punch oscillates and decays naturally over the duration.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Head shake "no", object wobble, impact reaction, playful jiggle.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobblePreset : CodePreset
    {
        public override string PresetName => "WobbleY";
        public override string Description => "Wobbles rotation back and forth on Y";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(0, 15f, 0), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation back and forth on the X axis using DOTween's punch rotation.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(15, 0, 0)</c> (15° amplitude), vibrato <c>8</c>,
    /// and elasticity <c>0.5</c>. Creates a nodding/tipping oscillation on the pitch axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Nodding motion, forward/back wobble, rocking chair, impact tilt.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleXPreset : CodePreset
    {
        public override string PresetName => "WobbleX";
        public override string Description => "Wobbles rotation back and forth on X";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(15f, 0, 0), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation back and forth on the Z axis using DOTween's punch rotation.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 0, 15)</c> (15° amplitude), vibrato <c>8</c>,
    /// and elasticity <c>0.5</c>. Creates a side-to-side tilt oscillation on the roll axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D wobble, pendulum swing, teeter-totter, balance recovery.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleZPreset : CodePreset
    {
        public override string PresetName => "WobbleZ";
        public override string Description => "Wobbles rotation back and forth on Z";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(0, 0, 15f), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation diagonally across both X and Y axes simultaneously.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(12, 12, 0)</c> (12° amplitude per axis),
    /// vibrato <c>8</c>, and elasticity <c>0.5</c>. The dual-axis punch creates a more complex,
    /// organic wobble than single-axis variants.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Complex wobble, impact reaction, organic jiggle, multi-axis disturbance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXYPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXY";
        public override string Description => "Wobbles rotation diagonally across X and Y";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(12f, 12f, 0f), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation diagonally across both X and Z axes simultaneously.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(12, 0, 12)</c> (12° amplitude per axis),
    /// vibrato <c>8</c>, and elasticity <c>0.5</c>. Combines pitch and roll wobble for a
    /// tumbling-style oscillation.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Tumble wobble, unsteady object, physical disturbance, off-balance reaction.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalXZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalXZPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalXZ";
        public override string Description => "Wobbles rotation diagonally across X and Z";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(12f, 0f, 12f), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Wobbles the target's rotation diagonally across both Y and Z axes simultaneously.
    /// <para>
    /// Uses <c>DOPunchRotation</c> with punch vector <c>(0, 12, 12)</c> (12° amplitude per axis),
    /// vibrato <c>8</c>, and elasticity <c>0.5</c>. Combines yaw and roll wobble for a
    /// sideways-tilting oscillation.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Sideways wobble, wind-blown object, lateral disturbance, off-kilter reaction.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WobbleDiagonalYZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WobbleDiagonalYZPreset : CodePreset
    {
        public override string PresetName => "WobbleDiagonalYZ";
        public override string Description => "Wobbles rotation diagonally across Y and Z";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchRotation(new Vector3(0f, 12f, 12f), GetDuration(duration), 8, 0.5f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Leans the target 12 degrees on the Z axis then springs back to the original rotation.
    /// <para>
    /// Builds a 2-step sequence: (1) rotate to <c>original + (0, 0, 12)</c> over 40% duration with
    /// <c>Ease.OutQuad</c>, (2) return to original rotation over 60% duration with <c>Ease.OutBack</c>
    /// (springy overshoot on return). Uses <c>DOLocalRotate</c> for local-space rotation.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutQuad (lean), OutBack (return)<br/>
    /// <b>Easing override:</b> Primary ease controls lean; secondary ease controls springy return.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Curiosity peek, notification tilt, playful head tilt, attention gesture.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Tilt").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class TiltPreset : CodePreset
    {
        public override string PresetName => "Tilt";
        public override string Description => "Lean on Z then spring back";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 12f), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Flips the target 180 degrees on the X axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(180, 0, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Card flip, page turn, reveal animation, object inspection.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipX").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipXPreset : CodePreset
    {
        public override string PresetName => "FlipX";
        public override string Description => "180° flip on X axis";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DOLocalRotate(new Vector3(180f, 0f, 0f), GetDuration(duration), RotateMode.LocalAxisAdd)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Flips the target 180 degrees on the Y axis using additive local rotation.
    /// <para>
    /// Rotates by <c>(0, 180, 0)</c> using <c>RotateMode.LocalAxisAdd</c> with <c>Ease.InOutQuad</c>.
    /// LocalAxisAdd ensures the rotation is added to the current orientation rather than targeting
    /// an absolute angle, making it safe to chain multiple flips.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InOutQuad<br/>
    /// <b>Easing override:</b> Primary ease replaces InOutQuad.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Horizontal card flip, character turn-around, mirror reveal, about-face.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FlipY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FlipYPreset : CodePreset
    {
        public override string PresetName => "FlipY";
        public override string Description => "180° flip on Y axis";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutQuad);
            var ease = ResolveEase(presetOptions, Ease.InOutQuad);
            return target.transform.DOLocalRotate(new Vector3(0f, 180f, 0f), GetDuration(duration), RotateMode.LocalAxisAdd)
                .SetEase(ease)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Gently rocks the target back and forth on the Z axis like a pendulum, using callback-chain looping.
    /// <para>
    /// Alternates between rotating to <c>original + (0, 0, +8)</c> and <c>original + (0, 0, -8)</c>,
    /// each leg taking half the total duration. Uses <c>DOLocalRotate</c> for local-space rotation.
    /// Callback-chain looping ensures delay applies only on the first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.0s (1.5s per leg) | <b>Default ease:</b> InOutSine<br/>
    /// <b>Easing override:</b> Primary ease controls left-rock; secondary ease controls right-rock.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Pendulum swing, cradle rock, hanging sign, idle sway, metronome.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Rock").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class RockPreset : CodePreset
    {
        public override string PresetName => "Rock";
        public override string Description => "Gentle Z-axis pendulum loop";
        public override float DefaultDuration => 3.0f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var halfDur = GetDuration(duration) * 0.5f;
            var leftEase = options.Ease ?? Ease.InOutSine;
            var rightEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var leftOptions = MergeWithDefaultEase(options.SetEase(leftEase), leftEase);
            var rightOptions = MergeWithDefaultEase(options.SetEase(rightEase), rightEase);
            bool applyDelay = true;

            Tween tween = null;

            void RockLeft()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 8f), halfDur)
                    .SetEase(leftEase)
                    .WithLoopDefaults(leftOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(RockRight);
            }

            void RockRight()
            {
                tween = t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -8f), halfDur)
                    .SetEase(rightEase)
                    .WithLoopDefaults(rightOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(RockLeft);
            }

            RockLeft();

            return tween;
        }
    }

    /// <summary>
    /// Tilts the target 15 degrees forward on the X axis then springs back to the original rotation.
    /// <para>
    /// Builds a 2-step sequence: (1) rotate to <c>original + (15, 0, 0)</c> over 40% duration with
    /// <c>Ease.OutQuad</c>, (2) return to original rotation over 60% duration with <c>Ease.OutBack</c>
    /// (springy overshoot on return). Same structure as Tilt but operates on the X axis.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutQuad (lean), OutBack (return)<br/>
    /// <b>Easing override:</b> Primary ease controls forward lean; secondary ease controls springy return.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Agreeing nod, acknowledgment gesture, bow, forward peek.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Nod").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class NodPreset : CodePreset
    {
        public override string PresetName => "Nod";
        public override string Description => "X-axis tilt forward then spring back";
        public override float DefaultDuration => 0.4f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(15f, 0f, 0f), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Winds up the target's rotation backward on Z, snaps forward past the origin, then settles to rest.
    /// <para>
    /// Builds a 3-step sequence: (1) rotate to <c>original + (0, 0, -30)</c> over 40% duration with
    /// <c>Ease.InQuad</c> (wind-up), (2) snap to <c>original + (0, 0, +5)</c> over 35% with
    /// <c>Ease.OutBack</c> (overshoot forward), (3) settle to original over 25% with <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> InQuad (wind), OutBack (snap)<br/>
    /// <b>Easing override:</b> Primary ease controls wind-up; secondary ease controls snap-forward.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Spring release, wind-up toy, throwing preparation, mechanical action.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WindUp").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WindUpPreset : CodePreset
    {
        public override string PresetName => "WindUp";
        public override string Description => "Wind up rotation then snap forward and settle";
        public override float DefaultDuration => 0.5f;
        public override string Category => PresetCategories.Rotation;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var windEase = ResolveEase(options, Ease.InQuad);
            var snapEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, windEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -30f), dur * 0.4f).SetEase(windEase))
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 5f), dur * 0.35f).SetEase(snapEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.25f).SetEase(Ease.OutQuad))
                .WithDefaults(presetOptions, target);
        }
    }

    #endregion

    #region Combined Animations

    /// <summary>
    /// Scales the target from zero to original while simultaneously fading in from transparent to opaque.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c> and alpha to <c>0</c>. Builds a parallel sequence:
    /// scale uses <c>Ease.OutCubic</c>, fade uses <c>Ease.Linear</c> (so alpha doesn't rush ahead of scale).
    /// If no fadeable component exists, only the scale animation plays. Always returns a valid tween
    /// (CanApplyTo returns true for any non-null target).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 2.0s | <b>Default ease:</b> OutCubic (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element entrance, dialog appearance, smooth combined reveal, material-design entrance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopInFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopInFadePreset : CodePreset
    {
        public override string PresetName => "PopInFade";
        public override string Description => "Scales and fades in together";
        public override float DefaultDuration => 2f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;

            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var seq = DOTween.Sequence();

            seq.Append(t.DOScale(originalScale, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 1f, dur);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                // Use Linear ease for fade so alpha doesn't rush ahead of scale
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Scales the target down to zero while simultaneously fading out to transparent.
    /// <para>
    /// Builds a parallel sequence: scale to <c>Vector3.zero</c> with <c>Ease.InBack</c> (anticipation overshoot),
    /// fade to <c>0</c> with <c>Ease.Linear</c>. If no fadeable component exists, only the scale animation plays.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InBack (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; fade is always Linear.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> UI element dismissal, dialog close, combined exit, material-design exit.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PopOutFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PopOutFadePreset : CodePreset
    {
        public override string PresetName => "PopOutFade";
        public override string Description => "Scales down and fades out together";
        public override float DefaultDuration => 1.2f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InBack);
            var ease = ResolveEase(presetOptions, Ease.InBack);

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(Vector3.zero, dur).SetEase(ease));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                fadeTween.SetEase(Ease.Linear);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target)
        {
            return target != null;
        }
    }

    /// <summary>
    /// Performs an attention-grabbing scale pulse with multiple oscillations, looped twice for emphasis.
    /// <para>
    /// Builds a 4-step scale sequence: (1) scale to <c>1.1x</c> at 15% with OutCubic,
    /// (2) scale to <c>0.95x</c> at 15% with InCubic, (3) scale to <c>1.05x</c> at 15% with OutCubic,
    /// (4) return to original at 15% with OutCubic. The sequence is set to loop <c>2</c> times via
    /// <c>SetLoops(2)</c> (DOTween built-in loops, not callback-chain).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot effect (plays twice via SetLoops) | <b>Default duration:</b> 0.8s per loop | <b>Default ease:</b> OutCubic, InCubic<br/>
    /// <b>Easing override:</b> Primary ease controls expand steps; secondary ease controls contract step.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Call-to-action highlight, notification emphasis, important element pulse, "look here" effect.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Attention").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class AttentionPreset : CodePreset
    {
        public override string PresetName => "Attention";
        public override string Description => "Attention-grabbing pulse";
        public override float DefaultDuration => 0.8f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var secondaryEase = ResolveSecondaryEase(presetOptions, Ease.InCubic);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.1f, dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale * 0.95f, dur * 0.15f).SetEase(secondaryEase))
                .Append(t.DOScale(originalScale * 1.05f, dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale, dur * 0.15f).SetEase(ease))
                .SetLoops(2)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target upward from 100 units below while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// Offsets initial position by <c>Vector3.down * 100</c> and sets alpha to <c>0</c>.
    /// Builds a parallel sequence: position animates over full duration with <c>Ease.OutCubic</c>,
    /// fade animates over 70% duration with <c>Ease.Linear</c>. The shorter fade ensures the object
    /// is fully visible before it reaches its final position.
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
        public override string Category => PresetCategories.Combined;

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
        public override string Category => PresetCategories.Combined;

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
    /// Shakes the target's position while simultaneously fading out to transparent, creating a damage/death effect.
    /// <para>
    /// Builds a parallel sequence: <c>DOShakePosition</c> with strength <c>0.3</c>, vibrato <c>15</c>,
    /// randomness <c>90°</c>, and fade-out enabled; joined with a fade to <c>0</c> using <c>Ease.InQuad</c>
    /// (accelerating fade so the object lingers visible during the strongest shaking).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.8s | <b>Default ease:</b> Linear (sequence), InQuad (fade)<br/>
    /// <b>Easing override:</b> Standard options apply; shake and fade eases are hardcoded.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Enemy death, damage feedback, destruction effect, error dismiss with shake.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ShakeFade").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ShakeFadePreset : CodePreset
    {
        public override string PresetName => "ShakeFade";
        public override string Description => "Shake position with fade out";
        public override float DefaultDuration => 0.8f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.Linear);

            var seq = DOTween.Sequence();
            seq.Append(t.DOShakePosition(dur, 0.3f, 15, 90f, false, true));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                fadeTween.SetEase(Ease.InQuad);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Spins the target 720 degrees on the Y axis while shrinking to zero scale, creating a collectible pickup effect.
    /// <para>
    /// Builds a parallel sequence: scale to <c>Vector3.zero</c> with <c>Ease.InBack</c> (anticipation),
    /// joined with 720° Y rotation using <c>RotateMode.FastBeyond360</c> and <c>Ease.Linear</c>
    /// (uniform spin speed). The two full rotations during shrink create a vortex-like exit.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.7s | <b>Default ease:</b> InBack (scale), Linear (spin)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Collectible pickup, item absorption, vortex exit, magical disappearance.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SpinScale").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SpinScalePreset : CodePreset
    {
        public override string PresetName => "SpinScale";
        public override string Description => "Spin and shrink to zero";
        public override float DefaultDuration => 0.7f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.InBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Join(t.DOScale(Vector3.zero, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 720f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Drops the target from 5 units above with two bounces, applying squash-stretch deformation on each impact.
    /// <para>
    /// Offsets Y by <c>+5</c>, then builds a multi-step sequence combining position and scale:
    /// (1) fall to target (30%), (2) squash on impact <c>(1.3x, 0.7y)</c> then restore (5%+5%),
    /// (3) bounce 1 up to 35% height (12%+12%), (4) smaller squash <c>(1.15x, 0.85y)</c> then restore (4%+4%),
    /// (5) bounce 2 up to 10% height (9%+9%), (6) final scale settle (5%).
    /// Falls use <c>Ease.InQuad</c>, bounces use <c>Ease.OutQuad</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.6s | <b>Default ease:</b> InQuad (fall), OutQuad (bounce)<br/>
    /// <b>Easing override:</b> Primary ease controls falls; secondary ease controls bounce-ups.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Heavy object landing, character ground pound, cartoon drop, dramatic arrival.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BounceLand").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BounceLandPreset : CodePreset
    {
        public override string PresetName => "BounceLand";
        public override string Description => "Drop with bounce and squash-stretch on landing";
        public override float DefaultDuration => 1.6f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var targetY = t.localPosition.y;
            var dropHeight = 5f;
            t.localPosition = t.localPosition + Vector3.up * dropHeight;

            var dur = GetDuration(duration);
            var fallEase = ResolveEase(options, Ease.InQuad);
            var bounceEase = ResolveSecondaryEase(options, Ease.OutQuad);
            var presetOptions = MergeWithDefaultEase(options, fallEase);

            // Squash scale values
            var squash1 = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z);
            var squash2 = new Vector3(originalScale.x * 1.15f, originalScale.y * 0.85f, originalScale.z);

            return DOTween.Sequence()
                // Fall
                .Append(t.DOLocalMoveY(targetY, dur * 0.3f).SetEase(fallEase))
                // Squash on first impact
                .Append(t.DOScale(squash1, dur * 0.05f).SetEase(Ease.OutQuad))
                .Append(t.DOScale(originalScale, dur * 0.05f).SetEase(Ease.InQuad))
                // Bounce 1 up
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.35f, dur * 0.12f).SetEase(bounceEase))
                .Append(t.DOLocalMoveY(targetY, dur * 0.12f).SetEase(fallEase))
                // Squash on second impact
                .Append(t.DOScale(squash2, dur * 0.04f).SetEase(Ease.OutQuad))
                .Append(t.DOScale(originalScale, dur * 0.04f).SetEase(Ease.InQuad))
                // Bounce 2 up (smaller)
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.1f, dur * 0.09f).SetEase(bounceEase))
                .Append(t.DOLocalMoveY(targetY, dur * 0.09f).SetEase(fallEase))
                // Final settle
                .Append(t.DOScale(originalScale, dur * 0.05f).SetEase(Ease.OutQuad))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Scales the target up to 1.5x its original size while simultaneously fading out, creating a burst/explosion effect.
    /// <para>
    /// Builds a parallel sequence: scale to <c>originalScale * 1.5</c> with <c>Ease.OutQuad</c>
    /// (fast initial burst), joined with fade to <c>0</c> using <c>Ease.Linear</c>.
    /// If no fadeable component exists, only the scale animation plays.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot exit | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutQuad (scale), Linear (fade)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls fade.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Explosion effect, particle burst, destruction animation, energy release.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Explode").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ExplodePreset : CodePreset
    {
        public override string PresetName => "Explode";
        public override string Description => "Scale up and fade out simultaneously";
        public override float DefaultDuration => 0.6f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.OutQuad);
            var fadeEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(originalScale * 1.5f, dur).SetEase(scaleEase));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                seq.Join(fadeTween.SetEase(fadeEase));
            }

            return seq.WithDefaults(presetOptions, target);
        }

        public override bool CanApplyTo(GameObject target) => target != null;
    }

    /// <summary>
    /// Scales the target from zero to original while spinning 360 degrees on Y, creating a whirl-in entrance.
    /// <para>
    /// Sets initial scale to <c>Vector3.zero</c>. Builds a parallel sequence: scale to original with
    /// <c>Ease.OutBack</c> (overshoot entrance), joined with 360° Y rotation using
    /// <c>RotateMode.FastBeyond360</c> and <c>Ease.Linear</c> (uniform spin).
    /// The inverse of SpinScale (which shrinks while spinning).
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutBack (scale), Linear (spin)<br/>
    /// <b>Easing override:</b> Primary ease controls scale; secondary ease controls spin.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Magical entrance, vortex appearance, power-up spawn, dramatic reveal.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("SwirlIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class SwirlInPreset : CodePreset
    {
        public override string PresetName => "SwirlIn";
        public override string Description => "Spin and scale in from zero";
        public override float DefaultDuration => 1.0f;
        public override string Category => PresetCategories.Combined;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var dur = GetDuration(duration);
            var scaleEase = ResolveEase(options, Ease.OutBack);
            var spinEase = ResolveSecondaryEase(options, Ease.Linear);
            var presetOptions = MergeWithDefaultEase(options, scaleEase);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale, dur).SetEase(scaleEase))
                .Join(t.DORotate(new Vector3(0f, 360f, 0f), dur, RotateMode.FastBeyond360).SetEase(spinEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Slides the target in from 100 units to the left while simultaneously fading in (fade completes at 70% duration).
    /// <para>
    /// Offsets initial position by <c>Vector3.left * 100</c> and sets alpha to <c>0</c>.
    /// Builds a parallel sequence: position animates over full duration with <c>Ease.OutCubic</c>,
    /// fade animates over 70% duration with <c>Ease.Linear</c>. Same pattern as SlideInFadeUp but horizontal.
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
        public override string Category => PresetCategories.Combined;

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
        public override string Category => PresetCategories.Combined;

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

    #endregion

    #region Helper Methods

    public abstract partial class CodePreset
    {
        /// <summary>
        /// Creates a fade tween for the appropriate component type.
        /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer (material).
        /// </summary>
        protected static Tween CreateFadeTween(GameObject target, float alpha, float duration)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                return canvasGroup.DOFade(alpha, duration);

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                return spriteRenderer.DOFade(alpha, duration);

            var image = target.GetComponent<Image>();
            if (image != null)
                return image.DOFade(alpha, duration);

            var text = target.GetComponent<Text>();
            if (text != null)
                return text.DOFade(alpha, duration);

            // Fallback to Renderer material fade
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
                return renderer.material.DOFade(alpha, duration);

            return null;
        }

        /// <summary>
        /// Sets the alpha for the appropriate component type.
        /// Supports CanvasGroup, SpriteRenderer, Image, Text, and Renderer (material).
        /// </summary>
        protected static void SetAlpha(GameObject target, float alpha)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                return;
            }

            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
                return;
            }

            var image = target.GetComponent<Image>();
            if (image != null)
            {
                var c = image.color;
                c.a = alpha;
                image.color = c;
                return;
            }

            var text = target.GetComponent<Text>();
            if (text != null)
            {
                var c = text.color;
                c.a = alpha;
                text.color = c;
                return;
            }

            // Fallback to Renderer material
            var renderer = target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                var c = renderer.material.color;
                c.a = alpha;
                renderer.material.color = c;
            }
        }

        /// <summary>
        /// Checks if the target has a component that supports fading.
        /// </summary>
        protected static bool CanFade(GameObject target)
        {
            if (target == null) return false;

            return target.GetComponent<CanvasGroup>() != null ||
                   target.GetComponent<SpriteRenderer>() != null ||
                   target.GetComponent<Image>() != null ||
                   target.GetComponent<Text>() != null ||
                   (target.GetComponent<Renderer>()?.material != null);
        }
    }

    #endregion
}
