using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
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
    /// Usage: <c>transform.Tween().Preset("ElasticSnapIn").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ElasticSnapInPreset : CodePreset
    {
        public override string PresetName => "ElasticSnapIn";
        public override string Description => "Scale from 0 with tight elastic oscillation";
        public override float DefaultDuration => 0.5f;


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

    /// <summary>
    /// Soft breathing loop with smaller scale range and slower rhythm.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BreatheSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BreatheSoftPreset : CodePreset
    {
        public override string PresetName => "BreatheSoft";
        public override string Description => "Soft gentle scale pulse loop";
        public override float DefaultDuration => 5.0f;


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
                tween = t.DOScale(originalScale * 1.04f, halfDur)
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
    /// Hard breathing loop with larger scale range and faster rhythm.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.0s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("BreatheHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class BreatheHardPreset : CodePreset
    {
        public override string PresetName => "BreatheHard";
        public override string Description => "Hard intense scale pulse loop";
        public override float DefaultDuration => 3.0f;


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
                tween = t.DOScale(originalScale * 1.15f, halfDur)
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
    /// Soft heartbeat with smaller pulses and slower rhythm.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 1.0s per cycle | <b>Default ease:</b> OutQuad (beat), InQuad (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("HeartbeatSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class HeartbeatSoftPreset : CodePreset
    {
        public override string PresetName => "HeartbeatSoft";
        public override string Description => "Soft double-pulse heartbeat loop";
        public override float DefaultDuration => 1.0f;


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
                    .Append(t.DOScale(originalScale * 1.08f, dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.12f).SetEase(returnEase))
                    .Append(t.DOScale(originalScale * 1.15f, dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.14f).SetEase(returnEase))
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
    /// Hard heartbeat with larger pulses and faster rhythm.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 0.6s per cycle | <b>Default ease:</b> OutQuad (beat), InQuad (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("HeartbeatHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class HeartbeatHardPreset : CodePreset
    {
        public override string PresetName => "HeartbeatHard";
        public override string Description => "Hard intense heartbeat loop";
        public override float DefaultDuration => 0.6f;


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
                    .Append(t.DOScale(originalScale * 1.25f, dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.12f).SetEase(returnEase))
                    .Append(t.DOScale(originalScale * 1.4f, dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.14f).SetEase(returnEase))
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
    /// Soft elastic snap in with lower amplitude and longer period.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutElastic (amplitude 0.4, period 0.4)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ElasticSnapInSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ElasticSnapInSoftPreset : CodePreset
    {
        public override string PresetName => "ElasticSnapInSoft";
        public override string Description => "Soft elastic snap from zero";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutElastic);
            var ease = ResolveEase(presetOptions, Ease.OutElastic);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 0.4f, 0.4f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard elastic snap in with higher amplitude and shorter period.
    /// <para>
    /// <b>Type:</b> One-shot entrance | <b>Default duration:</b> 0.4s | <b>Default ease:</b> OutElastic (amplitude 1.0, period 0.2)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ElasticSnapInHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ElasticSnapInHardPreset : CodePreset
    {
        public override string PresetName => "ElasticSnapInHard";
        public override string Description => "Hard elastic snap from zero";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            var presetOptions = MergeWithDefaultEase(options, Ease.OutElastic);
            var ease = ResolveEase(presetOptions, Ease.OutElastic);

            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(ease, 1.0f, 0.2f)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft float with smaller range and slower speed.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 7.0s | <b>Default ease:</b> InOutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FloatSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FloatSoftPreset : CodePreset
    {
        public override string PresetName => "FloatSoft";
        public override string Description => "Soft gentle hovering loop";
        public override float DefaultDuration => 7.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var halfDur = GetDuration(duration) * 0.5f;
            var moveUpEase = options.Ease ?? Ease.InOutCubic;
            var moveDownEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutCubic;

            var upOptions = MergeWithDefaultEase(options.SetEase(moveUpEase), moveUpEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(moveDownEase), moveDownEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.25f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveUpEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.25f, halfDur)
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
    /// Hard float with larger range and faster speed.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InOutCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("FloatHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class FloatHardPreset : CodePreset
    {
        public override string PresetName => "FloatHard";
        public override string Description => "Hard pronounced hovering loop";
        public override float DefaultDuration => 5.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var halfDur = GetDuration(duration) * 0.5f;
            var moveUpEase = options.Ease ?? Ease.InOutCubic;
            var moveDownEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutCubic;

            var upOptions = MergeWithDefaultEase(options.SetEase(moveUpEase), moveUpEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(moveDownEase), moveDownEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.8f, halfDur)
                    .SetRelative(true)
                    .SetEase(moveUpEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.8f, halfDur)
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
    /// Soft zig-zag with smaller steps and slower speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ZigZagSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ZigZagSoftPreset : CodePreset
    {
        public override string PresetName => "ZigZagSoft";
        public override string Description => "Soft alternating diagonal movement";
        public override float DefaultDuration => 1.2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutSine);
            var ease = ResolveEase(presetOptions, Ease.InOutSine);
            var stepDur = dur / 3f;

            return DOTween.Sequence()
                .Append(t.DOLocalMove(new Vector3(0.25f, 0.25f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(-0.5f, 0.25f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(0.25f, 0.25f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard zig-zag with larger steps and faster speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InOutSine
    /// </para>
    /// Usage: <c>transform.Tween().Preset("ZigZagHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class ZigZagHardPreset : CodePreset
    {
        public override string PresetName => "ZigZagHard";
        public override string Description => "Hard alternating diagonal movement";
        public override float DefaultDuration => 0.8f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutSine);
            var ease = ResolveEase(presetOptions, Ease.InOutSine);
            var stepDur = dur / 3f;

            return DOTween.Sequence()
                .Append(t.DOLocalMove(new Vector3(0.8f, 0.8f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(-1.6f, 0.8f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(0.8f, 0.8f, 0f), stepDur).SetRelative(true).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft attention pulse with smaller oscillations and slower rhythm.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic, InCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("AttentionSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class AttentionSoftPreset : CodePreset
    {
        public override string PresetName => "AttentionSoft";
        public override string Description => "Soft attention-grabbing pulse";
        public override float DefaultDuration => 1.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var secondaryEase = ResolveSecondaryEase(presetOptions, Ease.InCubic);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.05f, dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale * 0.97f, dur * 0.15f).SetEase(secondaryEase))
                .Append(t.DOScale(originalScale * 1.03f, dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale, dur * 0.15f).SetEase(ease))
                .SetLoops(2)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard attention pulse with larger oscillations and faster rhythm.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutCubic, InCubic
    /// </para>
    /// Usage: <c>transform.Tween().Preset("AttentionHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class AttentionHardPreset : CodePreset
    {
        public override string PresetName => "AttentionHard";
        public override string Description => "Hard attention-grabbing pulse";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalScale = t.localScale;
            var dur = GetDuration(duration);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var secondaryEase = ResolveSecondaryEase(presetOptions, Ease.InCubic);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.2f, dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale * 0.9f, dur * 0.15f).SetEase(secondaryEase))
                .Append(t.DOScale(originalScale * 1.1f, dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale, dur * 0.15f).SetEase(ease))
                .SetLoops(2)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft tilt with smaller angle and slower speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (lean), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("TiltSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class TiltSoftPreset : CodePreset
    {
        public override string PresetName => "TiltSoft";
        public override string Description => "Soft lean on Z then spring back";
        public override float DefaultDuration => 0.5f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 6f), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard tilt with larger angle and faster speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (lean), OutBack (return)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("TiltHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class TiltHardPreset : CodePreset
    {
        public override string PresetName => "TiltHard";
        public override string Description => "Hard lean on Z then spring back";
        public override float DefaultDuration => 0.3f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 20f), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft wind-up with smaller rotation and slower speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s | <b>Default ease:</b> InQuad (wind), OutBack (snap)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WindUpSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WindUpSoftPreset : CodePreset
    {
        public override string PresetName => "WindUpSoft";
        public override string Description => "Soft wind up then snap forward";
        public override float DefaultDuration => 0.6f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var windEase = ResolveEase(options, Ease.InQuad);
            var snapEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, windEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -15f), dur * 0.4f).SetEase(windEase))
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 3f), dur * 0.35f).SetEase(snapEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.25f).SetEase(Ease.OutQuad))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard wind-up with larger rotation and faster speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.4s | <b>Default ease:</b> InQuad (wind), OutBack (snap)
    /// </para>
    /// Usage: <c>transform.Tween().Preset("WindUpHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class WindUpHardPreset : CodePreset
    {
        public override string PresetName => "WindUpHard";
        public override string Description => "Hard wind up then snap forward";
        public override float DefaultDuration => 0.4f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            var originalRot = t.localEulerAngles;
            var dur = GetDuration(duration);
            var windEase = ResolveEase(options, Ease.InQuad);
            var snapEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, windEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, -50f), dur * 0.4f).SetEase(windEase))
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 8f), dur * 0.35f).SetEase(snapEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.25f).SetEase(Ease.OutQuad))
                .WithDefaults(presetOptions, target);
        }
    }
}
