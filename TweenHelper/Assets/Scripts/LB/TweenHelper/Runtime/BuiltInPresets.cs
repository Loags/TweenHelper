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
        public override string Description => "Scales from 0 to 1 with overshoot";
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
        public override string Description => "Quick scale punch for 3D object feedback";
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
    /// Double-pulse heartbeat loop (1.15x→rest→1.25x→rest→pause), callback-chain with sequence per cycle.
    /// Usage: transform.Tween().Preset("Heartbeat").Play();
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
    /// Scale from 0 with tight elastic oscillation (amplitude 0.7, period 0.3).
    /// Usage: transform.Tween().Preset("ElasticSnap").Play();
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
    /// Clean scale from 0 to original, no overshoot (contrast to PopIn's OutBack).
    /// Usage: transform.Tween().Preset("GrowIn").Play();
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
    /// Clean scale to zero, no anticipation (contrast to PopOut's InBack overshoot).
    /// Usage: transform.Tween().Preset("ShrinkOut").Play();
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
    /// Quick scale bump (1.0→1.2→1.0) via 2-step sequence. Ideal for UI feedback.
    /// Usage: transform.Tween().Preset("PulseScale").Play();
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
    /// Shakes the position randomly.
    /// Usage: transform.Tween().Preset("Shake").Play();
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
    /// Slides down from above.
    /// Usage: transform.Tween().Preset("SlideInDown").Play();
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
    /// Slides up from below.
    /// Usage: transform.Tween().Preset("SlideInUp").Play();
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
    /// Slides in from the left side.
    /// Usage: transform.Tween().Preset("SlideInLeft").Play();
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
    /// Slides in from the right side.
    /// Usage: transform.Tween().Preset("SlideInRight").Play();
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
    /// Gentle floating hover animation (looping).
    /// Usage: transform.Tween().Preset("Float").Play();
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
    /// Shared orbit logic with configurable direction and radius interpolation.
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
    /// Orbits around current position on XZ plane (counter-clockwise).
    /// Usage: transform.Tween().Preset("Orbit").Play();
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
    /// Orbits clockwise around current position on XZ plane.
    /// Usage: transform.Tween().Preset("OrbitClockwise").Play();
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
    /// Orbits counter-clockwise around current position on XZ plane.
    /// Usage: transform.Tween().Preset("OrbitCounterClockwise").Play();
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
    /// Spirals upward with rotation.
    /// Usage: transform.Tween().Preset("Spiral").Play();
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
    /// Drops from above with bounce on landing.
    /// Usage: transform.Tween().Preset("DropIn").Play();
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
    /// Quick upward launch motion.
    /// Usage: transform.Tween().Preset("LaunchUp").Play();
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
    /// Slides up off-screen (+500 units), mirrors SlideInDown.
    /// Usage: transform.Tween().Preset("SlideOutUp").Play();
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
    /// Slides down off-screen (-500 units), mirrors SlideInUp.
    /// Usage: transform.Tween().Preset("SlideOutDown").Play();
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
    /// Slides left off-screen (-500 units), mirrors SlideInRight.
    /// Usage: transform.Tween().Preset("SlideOutLeft").Play();
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
    /// Slides right off-screen (+500 units), mirrors SlideInLeft.
    /// Usage: transform.Tween().Preset("SlideOutRight").Play();
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
    /// Horizontal Float equivalent, ±0.5 on X axis, callback-chain loop.
    /// Usage: transform.Tween().Preset("Sway").Play();
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
    /// Positional Y bounce (3 decreasing hops), sequence-based.
    /// Usage: transform.Tween().Preset("Bounce").Play();
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
    /// Tight rapid vibration (DOShakePosition, strength 0.08, vibrato 40).
    /// Usage: transform.Tween().Preset("Jitter").Play();
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
    /// Pull back on local Z then snap forward. 2-step sequence.
    /// Usage: transform.Tween().Preset("Recoil").Play();
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
    /// Small push right then spring back. Notification nudge.
    /// Usage: transform.Tween().Preset("Nudge").Play();
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
    /// Circular orbit on XY plane via DOVirtual.Float callback-chain loop.
    /// Usage: transform.Tween().Preset("Orbit2D").Play();
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
    /// Alternating diagonal: right+up, left+up, right+up via 3-step relative DOLocalMove sequence.
    /// Usage: transform.Tween().Preset("ZigZag").Play();
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
    /// Fades in from transparent.
    /// Usage: transform.Tween().Preset("FadeIn").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class FadeInPreset : CodePreset
    {
        public override string PresetName => "FadeIn";
        public override string Description => "Fades in from transparent (requires transparent material)";
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
    /// Fades out to transparent.
    /// Usage: transform.Tween().Preset("FadeOut").Play();
    /// </summary>
    [AutoRegisterPreset]
    public class FadeOutPreset : CodePreset
    {
        public override string PresetName => "FadeOut";
        public override string Description => "Fades out to transparent (requires transparent material)";
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
    /// Rapid alpha on/off loop (1→0→1), callback-chain.
    /// Usage: transform.Tween().Preset("Blink").Play();
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
    /// Smooth alpha cycle loop (1.0→0.3→1.0), callback-chain.
    /// Usage: transform.Tween().Preset("PulseFade").Play();
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
    /// Randomized alpha flicker via Perlin noise, snaps to 1.0 on complete.
    /// Usage: transform.Tween().Preset("Flicker").Play();
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
    /// Fade in (0→1) first half, then fade out (1→0) second half. Sets alpha to 0 initially.
    /// Usage: transform.Tween().Preset("FadeInOut").Play();
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
    /// Spins 360 degrees on Y axis.
    /// Usage: transform.Tween().Preset("SpinY").Play();
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
    /// Spins 360 degrees on X axis.
    /// Usage: transform.Tween().Preset("SpinX").Play();
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
    /// Spins 360 degrees on Z axis.
    /// Usage: transform.Tween().Preset("SpinZ").Play();
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
    /// Spins 360 degrees across X and Y axes.
    /// Usage: transform.Tween().Preset("SpinDiagonalXY").Play();
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
    /// Spins 360 degrees across X and Z axes.
    /// Usage: transform.Tween().Preset("SpinDiagonalXZ").Play();
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
    /// Spins 360 degrees across Y and Z axes.
    /// Usage: transform.Tween().Preset("SpinDiagonalYZ").Play();
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
    /// Wobbles rotation back and forth on Y.
    /// Usage: transform.Tween().Preset("WobbleY").Play();
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
    /// Wobbles rotation back and forth on X.
    /// Usage: transform.Tween().Preset("WobbleX").Play();
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
    /// Wobbles rotation back and forth on Z.
    /// Usage: transform.Tween().Preset("WobbleZ").Play();
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
    /// Wobbles rotation diagonally across X and Y.
    /// Usage: transform.Tween().Preset("WobbleDiagonalXY").Play();
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
    /// Wobbles rotation diagonally across X and Z.
    /// Usage: transform.Tween().Preset("WobbleDiagonalXZ").Play();
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
    /// Wobbles rotation diagonally across Y and Z.
    /// Usage: transform.Tween().Preset("WobbleDiagonalYZ").Play();
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
    /// Lean 12° on Z then spring back, 2-step sequence.
    /// Usage: transform.Tween().Preset("Tilt").Play();
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
    /// 180° flip on X axis via RotateMode.LocalAxisAdd.
    /// Usage: transform.Tween().Preset("FlipX").Play();
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
    /// 180° flip on Y axis via RotateMode.LocalAxisAdd.
    /// Usage: transform.Tween().Preset("FlipY").Play();
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
    /// Gentle Z-axis pendulum loop (±8°), callback-chain like Float.
    /// Usage: transform.Tween().Preset("Rock").Play();
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
    /// X-axis tilt forward (15°) then spring back. 2-step sequence.
    /// Usage: transform.Tween().Preset("Nod").Play();
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
    /// Rotate backward (-30° Z) then snap forward past origin (+5° Z) then settle to 0°. 3-step sequence.
    /// Usage: transform.Tween().Preset("WindUp").Play();
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
    /// Pops in with fade (scale + fade together).
    /// Usage: transform.Tween().Preset("PopInFade").Play();
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
    /// Scales down and fades out together.
    /// Usage: transform.Tween().Preset("PopOutFade").Play();
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
    /// Attention-grabbing pulse animation.
    /// Usage: transform.Tween().Preset("Attention").Play();
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
    /// Slide up from below + fade in (fade at 70% duration).
    /// Usage: transform.Tween().Preset("SlideInFadeUp").Play();
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
    /// Slide down from above + fade in.
    /// Usage: transform.Tween().Preset("SlideInFadeDown").Play();
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
    /// Shake position + fade out (damage/death effect).
    /// Usage: transform.Tween().Preset("ShakeFade").Play();
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
    /// 720° Y spin + scale to zero (collectible pickup effect).
    /// Usage: transform.Tween().Preset("SpinScale").Play();
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
    /// Drop from above with bounce + squash-stretch on each landing.
    /// Usage: transform.Tween().Preset("BounceLand").Play();
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
    /// Scale up 1.5x + fade out simultaneously. Burst/destruction effect.
    /// Usage: transform.Tween().Preset("Explode").Play();
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
    /// 360° Y spin + scale from 0 to original. Entrance whirl effect.
    /// Usage: transform.Tween().Preset("SwirlIn").Play();
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
    /// Slide from left (-100 X) to original + fade in. Same pattern as SlideInFadeUp.
    /// Usage: transform.Tween().Preset("SlideInFadeLeft").Play();
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
    /// Slide from right (+100 X) to original + fade in. Same pattern as SlideInFadeUp.
    /// Usage: transform.Tween().Preset("SlideInFadeRight").Play();
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
