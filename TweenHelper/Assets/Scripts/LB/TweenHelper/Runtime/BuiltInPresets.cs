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
    /// Scales from 0 to original scale with overshoot.
    /// Usage: transform.Tween().Preset("PopIn").Play();
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

            // OutBack with overshoot parameter (1 = default, higher = more overshoot)
            return t.DOScale(originalScale, GetDuration(duration))
                .SetEase(Ease.OutBack, 1.7f)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutBack), target);
        }
    }

    /// <summary>
    /// Scales to 0 with anticipation.
    /// Usage: transform.Tween().Preset("PopOut").Play();
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
            return target.transform.DOScale(Vector3.zero, GetDuration(duration))
                .SetEase(Ease.InBack)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InBack), target);
        }
    }

    /// <summary>
    /// Quick scale punch for feedback.
    /// Usage: transform.Tween().Preset("Punch").Play();
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
    /// Squash and stretch pulse (not an actual positional bounce).
    /// Usage: transform.Tween().Preset("Squash").Play();
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

            // Squash and stretch style bounce
            return DOTween.Sequence()
                .Append(t.DOScale(new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z), dur * 0.15f))
                .Append(t.DOScale(originalScale, dur * 0.25f).SetEase(Ease.OutElastic))
                .WithDefaults(options, target);
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

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
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

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
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

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
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

            return t.DOLocalMove(targetPos, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
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

            Tween tween = null;

            void MoveUp()
            {
                tween = t.DOLocalMoveY(0.5f, halfDur)
                    .SetRelative(true)
                    .SetEase(Ease.InOutCubic)
                    .OnComplete(MoveDown);
            }

            void MoveDown()
            {
                tween = t.DOLocalMoveY(-0.5f, halfDur)
                    .SetRelative(true)
                    .SetEase(Ease.InOutCubic)
                    .OnComplete(MoveUp);
            }

            MoveUp();

            return tween.WithDefaults(options, target);
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

            var tween = DOVirtual.Float(0f, fullCycle, dur, angle =>
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
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental)
                .SetUpdate(UpdateType.Fixed)
                .WithDefaults(options, target);

            return tween;
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
                .SetEase(Ease.OutQuad)
                .WithDefaults(options, target);
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

            // Manual bounce: fall fast, then bounce up/down with decreasing height
            return DOTween.Sequence()
                .Append(t.DOLocalMoveY(targetY, dur * 0.4f).SetEase(Ease.InQuad)) // Fall
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.3f, dur * 0.15f).SetEase(Ease.OutQuad)) // Bounce 1 up
                .Append(t.DOLocalMoveY(targetY, dur * 0.15f).SetEase(Ease.InQuad)) // Bounce 1 down
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.1f, dur * 0.1f).SetEase(Ease.OutQuad)) // Bounce 2 up
                .Append(t.DOLocalMoveY(targetY, dur * 0.1f).SetEase(Ease.InQuad)) // Bounce 2 down
                .Append(t.DOLocalMoveY(targetY + dropHeight * 0.03f, dur * 0.05f).SetEase(Ease.OutQuad)) // Bounce 3 up
                .Append(t.DOLocalMoveY(targetY, dur * 0.05f).SetEase(Ease.InQuad)) // Bounce 3 down
                .WithDefaults(options, target);
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

            return t.DOLocalMoveY(t.localPosition.y + 3f, GetDuration(duration))
                .SetEase(Ease.OutCubic)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutCubic), target);
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
            return tween?.SetEase(Ease.InQuad).WithDefaults(options, target);
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
            return target.transform.DORotate(new Vector3(0, 360f, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
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
            return target.transform.DORotate(new Vector3(360f, 0, 0), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
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
            return target.transform.DORotate(new Vector3(0, 0, 360f), GetDuration(duration), RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
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
            return target.transform.DORotate(new Vector3(360f, 360f, 0f), dur, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
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
            return target.transform.DORotate(new Vector3(360f, 0f, 360f), dur, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
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
            return target.transform.DORotate(new Vector3(0f, 360f, 360f), dur, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .WithDefaults(MergeWithDefaultEase(options, Ease.InOutQuad), target);
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
            var seq = DOTween.Sequence();

            seq.Append(t.DOScale(originalScale, dur).SetEase(Ease.OutCubic));

            var fadeTween = CreateFadeTween(target, 1f, dur);
            if (fadeTween != null)
            {
                SetAlpha(target, 0f);
                // Use Linear ease for fade so alpha doesn't rush ahead of scale
                seq.Join(fadeTween.SetEase(Ease.Linear));
            }

            return seq.WithDefaults(options, target);
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

            var seq = DOTween.Sequence();
            seq.Join(t.DOScale(Vector3.zero, dur).SetEase(Ease.InBack));

            var fadeTween = CreateFadeTween(target, 0f, dur);
            if (fadeTween != null)
            {
                fadeTween.SetEase(Ease.Linear);
                seq.Join(fadeTween);
            }

            return seq.WithDefaults(options, target);
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

            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * 1.1f, dur * 0.15f))
                .Append(t.DOScale(originalScale * 0.95f, dur * 0.15f))
                .Append(t.DOScale(originalScale * 1.05f, dur * 0.15f))
                .Append(t.DOScale(originalScale, dur * 0.15f))
                .SetLoops(2)
                .WithDefaults(options, target);
        }
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
