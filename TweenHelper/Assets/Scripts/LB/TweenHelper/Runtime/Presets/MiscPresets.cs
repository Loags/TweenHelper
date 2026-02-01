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
    /// <b>Easing override:</b> Primary ease controls expand leg; secondary ease controls contract leg.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var halfDur = GetDuration(duration, options) * 0.5f;
            var expandEase = options.Ease ?? Ease.InOutSine;
            var contractEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var upOptions = MergeWithDefaultEase(options.SetEase(expandEase), expandEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(contractEase), contractEase);
            bool applyDelay = true;

            Tween tween = null;

            void Expand()
            {
                tween = t.DOScale(originalScale * (1f + 0.08f * strength), halfDur)
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
    /// <b>Easing override:</b> Primary ease controls beat pulses; secondary ease controls return-to-rest phases.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var beatEase = options.Ease ?? Ease.OutQuad;
            var returnEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var loopOptions = MergeWithDefaultEase(options, beatEase);
            bool applyDelay = true;

            Tween tween = null;

            void Beat()
            {
                var seq = DOTween.Sequence()
                    // First beat (smaller)
                    .Append(t.DOScale(originalScale * (1f + 0.15f * strength), dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.12f).SetEase(returnEase))
                    // Second beat (larger)
                    .Append(t.DOScale(originalScale * (1f + 0.25f * strength), dur * 0.12f).SetEase(beatEase))
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
    /// Gently bobs the target up and down on the Y axis in a continuous hovering loop using callback-chain looping.
    /// <para>
    /// Alternates between moving <c>+0.5</c> and <c>-0.5</c> on local Y (relative), each leg taking
    /// half the total duration. Uses <c>SetRelative(true)</c> for position-independent movement.
    /// Callback-chain looping ensures delay is applied only on the first cycle via <c>WithLoopDefaults</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 6.0s (3.0s per leg) | <b>Default ease:</b> InOutCubic<br/>
    /// <b>Easing override:</b> Primary ease controls upward leg; secondary ease controls downward leg.<br/>
    /// <b>Strength override:</b> Multiplies float height (default 1.0).
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
            var strength = ResolveStrength(options);
            var halfDur = GetDuration(duration, options) * 0.5f;
            var moveUpEase = options.Ease ?? Ease.InOutCubic;
            var moveDownEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutCubic;

            // Ensure each leg uses its own ease without being overwritten by global defaults.
            var upOptions = MergeWithDefaultEase(options.SetEase(moveUpEase), moveUpEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(moveDownEase), moveDownEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.5f * strength, halfDur)
                    .SetRelative(true)
                    .SetEase(moveUpEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.5f * strength, halfDur)
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
    /// <b>Easing override:</b> Primary ease replaces InOutSine on all steps.<br/>
    /// <b>Strength override:</b> Multiplies zigzag distance (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutSine);
            var ease = ResolveEase(presetOptions, Ease.InOutSine);
            var stepDur = dur / 3f;

            return DOTween.Sequence()
                .Append(t.DOLocalMove(new Vector3(0.5f * strength, 0.5f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(-1f * strength, 0.5f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(0.5f * strength, 0.5f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
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
    /// <b>Easing override:</b> Primary ease controls expand steps; secondary ease controls contract step.<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var secondaryEase = ResolveSecondaryEase(presetOptions, Ease.InCubic);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * (1f + 0.1f * strength), dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale * (1f - 0.05f * strength), dur * 0.15f).SetEase(secondaryEase))
                .Append(t.DOScale(originalScale * (1f + 0.05f * strength), dur * 0.15f).SetEase(ease))
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
    /// <b>Easing override:</b> Primary ease controls lean; secondary ease controls springy return.<br/>
    /// <b>Strength override:</b> Multiplies tilt angle (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 12f * strength), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft breathing loop with smaller scale range and slower rhythm.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var halfDur = GetDuration(duration, options) * 0.5f;
            var expandEase = options.Ease ?? Ease.InOutSine;
            var contractEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var upOptions = MergeWithDefaultEase(options.SetEase(expandEase), expandEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(contractEase), contractEase);
            bool applyDelay = true;

            Tween tween = null;

            void Expand()
            {
                tween = t.DOScale(originalScale * (1f + 0.04f * strength), halfDur)
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
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 3.0s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var halfDur = GetDuration(duration, options) * 0.5f;
            var expandEase = options.Ease ?? Ease.InOutSine;
            var contractEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutSine;

            var upOptions = MergeWithDefaultEase(options.SetEase(expandEase), expandEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(contractEase), contractEase);
            bool applyDelay = true;

            Tween tween = null;

            void Expand()
            {
                tween = t.DOScale(originalScale * (1f + 0.15f * strength), halfDur)
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
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 1.0s per cycle | <b>Default ease:</b> OutQuad (beat), InQuad (return)<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var beatEase = options.Ease ?? Ease.OutQuad;
            var returnEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var loopOptions = MergeWithDefaultEase(options, beatEase);
            bool applyDelay = true;

            Tween tween = null;

            void Beat()
            {
                var seq = DOTween.Sequence()
                    .Append(t.DOScale(originalScale * (1f + 0.08f * strength), dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.12f).SetEase(returnEase))
                    .Append(t.DOScale(originalScale * (1f + 0.15f * strength), dur * 0.12f).SetEase(beatEase))
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
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 0.6s per cycle | <b>Default ease:</b> OutQuad (beat), InQuad (return)<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var beatEase = options.Ease ?? Ease.OutQuad;
            var returnEase = options.SecondaryEase ?? options.Ease ?? Ease.InQuad;
            var loopOptions = MergeWithDefaultEase(options, beatEase);
            bool applyDelay = true;

            Tween tween = null;

            void Beat()
            {
                var seq = DOTween.Sequence()
                    .Append(t.DOScale(originalScale * (1f + 0.25f * strength), dur * 0.12f).SetEase(beatEase))
                    .Append(t.DOScale(originalScale, dur * 0.12f).SetEase(returnEase))
                    .Append(t.DOScale(originalScale * (1f + 0.4f * strength), dur * 0.12f).SetEase(beatEase))
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
    /// Soft float with smaller range and slower speed.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 7.0s | <b>Default ease:</b> InOutCubic<br/>
    /// <b>Strength override:</b> Multiplies float height (default 1.0).
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
            var strength = ResolveStrength(options);
            var halfDur = GetDuration(duration, options) * 0.5f;
            var moveUpEase = options.Ease ?? Ease.InOutCubic;
            var moveDownEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutCubic;

            var upOptions = MergeWithDefaultEase(options.SetEase(moveUpEase), moveUpEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(moveDownEase), moveDownEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.25f * strength, halfDur)
                    .SetRelative(true)
                    .SetEase(moveUpEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.25f * strength, halfDur)
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
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 5.0s | <b>Default ease:</b> InOutCubic<br/>
    /// <b>Strength override:</b> Multiplies float height (default 1.0).
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
            var strength = ResolveStrength(options);
            var halfDur = GetDuration(duration, options) * 0.5f;
            var moveUpEase = options.Ease ?? Ease.InOutCubic;
            var moveDownEase = options.SecondaryEase ?? options.Ease ?? Ease.InOutCubic;

            var upOptions = MergeWithDefaultEase(options.SetEase(moveUpEase), moveUpEase);
            var downOptions = MergeWithDefaultEase(options.SetEase(moveDownEase), moveDownEase);
            bool applyDelay = true;

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.8f * strength, halfDur)
                    .SetRelative(true)
                    .SetEase(moveUpEase)
                    .WithLoopDefaults(upOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.8f * strength, halfDur)
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
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.2s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies zigzag distance (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutSine);
            var ease = ResolveEase(presetOptions, Ease.InOutSine);
            var stepDur = dur / 3f;

            return DOTween.Sequence()
                .Append(t.DOLocalMove(new Vector3(0.25f * strength, 0.25f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(-0.5f * strength, 0.25f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(0.25f * strength, 0.25f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard zig-zag with larger steps and faster speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.8s | <b>Default ease:</b> InOutSine<br/>
    /// <b>Strength override:</b> Multiplies zigzag distance (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.InOutSine);
            var ease = ResolveEase(presetOptions, Ease.InOutSine);
            var stepDur = dur / 3f;

            return DOTween.Sequence()
                .Append(t.DOLocalMove(new Vector3(0.8f * strength, 0.8f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(-1.6f * strength, 0.8f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .Append(t.DOLocalMove(new Vector3(0.8f * strength, 0.8f * strength, 0f), stepDur).SetRelative(true).SetEase(ease))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft attention pulse with smaller oscillations and slower rhythm.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 1.0s | <b>Default ease:</b> OutCubic, InCubic<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var secondaryEase = ResolveSecondaryEase(presetOptions, Ease.InCubic);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * (1f + 0.05f * strength), dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale * (1f - 0.03f * strength), dur * 0.15f).SetEase(secondaryEase))
                .Append(t.DOScale(originalScale * (1f + 0.03f * strength), dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale, dur * 0.15f).SetEase(ease))
                .SetLoops(2)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard attention pulse with larger oscillations and faster rhythm.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.6s | <b>Default ease:</b> OutCubic, InCubic<br/>
    /// <b>Strength override:</b> Multiplies scale pulse intensity (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var presetOptions = MergeWithDefaultEase(options, Ease.OutCubic);
            var ease = ResolveEase(presetOptions, Ease.OutCubic);
            var secondaryEase = ResolveSecondaryEase(presetOptions, Ease.InCubic);

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * (1f + 0.2f * strength), dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale * (1f - 0.1f * strength), dur * 0.15f).SetEase(secondaryEase))
                .Append(t.DOScale(originalScale * (1f + 0.1f * strength), dur * 0.15f).SetEase(ease))
                .Append(t.DOScale(originalScale, dur * 0.15f).SetEase(ease))
                .SetLoops(2)
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Soft tilt with smaller angle and slower speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.5s | <b>Default ease:</b> OutQuad (lean), OutBack (return)<br/>
    /// <b>Strength override:</b> Multiplies tilt angle (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 6f * strength), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

    /// <summary>
    /// Hard tilt with larger angle and faster speed.
    /// <para>
    /// <b>Type:</b> One-shot effect | <b>Default duration:</b> 0.3s | <b>Default ease:</b> OutQuad (lean), OutBack (return)<br/>
    /// <b>Strength override:</b> Multiplies tilt angle (default 1.0).
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
            var strength = ResolveStrength(options);
            var dur = GetDuration(duration, options);
            var leanEase = ResolveEase(options, Ease.OutQuad);
            var returnEase = ResolveSecondaryEase(options, Ease.OutBack);
            var presetOptions = MergeWithDefaultEase(options, leanEase);

            return DOTween.Sequence()
                .Append(t.DOLocalRotate(originalRot + new Vector3(0f, 0f, 20f * strength), dur * 0.4f).SetEase(leanEase))
                .Append(t.DOLocalRotate(originalRot, dur * 0.6f).SetEase(returnEase))
                .WithDefaults(presetOptions, target);
        }
    }

}
