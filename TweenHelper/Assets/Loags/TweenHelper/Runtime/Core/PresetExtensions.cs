using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Extension methods for directly calling presets on Transform and GameObject.
    /// Usage: transform.PopIn() or gameObject.FadeOut()
    /// </summary>
    public static class PresetExtensions
    {
        #region Scale Presets

        /// <summary>
        /// Scales from 0 to original scale with overshoot.
        /// </summary>
        public static TweenHandle PopIn(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<PopInPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Scales from 0 to original scale with overshoot.
        /// </summary>
        public static TweenHandle PopIn(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<PopInPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Scales to 0 with anticipation.
        /// </summary>
        public static TweenHandle PopOut(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<PopOutPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Scales to 0 with anticipation.
        /// </summary>
        public static TweenHandle PopOut(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<PopOutPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Quick scale punch for feedback.
        /// </summary>
        public static TweenHandle Punch(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<PunchPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Quick scale punch for feedback.
        /// </summary>
        public static TweenHandle Punch(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<PunchPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Squash and stretch effect (was Bounce).
        /// </summary>
        public static TweenHandle Bounce(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<BouncePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Squash and stretch effect (was Bounce).
        /// </summary>
        public static TweenHandle Bounce(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<BouncePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Squash and stretch effect.
        /// </summary>
        public static TweenHandle Squash(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<BouncePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Squash and stretch effect.
        /// </summary>
        public static TweenHandle Squash(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<BouncePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        #endregion

        #region Position Presets

        /// <summary>
        /// Random position shake.
        /// </summary>
        public static TweenHandle Shake(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<ShakePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Random position shake.
        /// </summary>
        public static TweenHandle Shake(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<ShakePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides in from above.
        /// </summary>
        public static TweenHandle SlideInDown(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SlideInDownPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides in from above.
        /// </summary>
        public static TweenHandle SlideInDown(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SlideInDownPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides up from below.
        /// </summary>
        public static TweenHandle SlideInUp(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SlideInUpPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides up from below.
        /// </summary>
        public static TweenHandle SlideInUp(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SlideInUpPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides in from the left side.
        /// </summary>
        public static TweenHandle SlideInLeft(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SlideInLeftPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides in from the left side.
        /// </summary>
        public static TweenHandle SlideInLeft(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SlideInLeftPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides in from the right side.
        /// </summary>
        public static TweenHandle SlideInRight(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SlideInRightPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Slides in from the right side.
        /// </summary>
        public static TweenHandle SlideInRight(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SlideInRightPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Gentle up/down hovering loop.
        /// </summary>
        public static TweenHandle Float(this Transform t, float? duration = null, TweenOptions options = default, Ease? moveUpEase = null, Ease? moveDownEase = null)
            => new TweenBuilder(t).Preset<FloatPreset>(duration).WithOptions(ApplyEases(options, moveUpEase, moveDownEase)).Play();

        /// <summary>
        /// Gentle up/down hovering loop.
        /// </summary>
        public static TweenHandle Float(this GameObject go, float? duration = null, TweenOptions options = default, Ease? moveUpEase = null, Ease? moveDownEase = null)
            => new TweenBuilder(go).Preset<FloatPreset>(duration).WithOptions(ApplyEases(options, moveUpEase, moveDownEase)).Play();

        /// <summary>
        /// Circles around a point on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<OrbitXZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Circles with a custom radius on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this Transform t, float radius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(t.gameObject, duration, ApplyEases(options, ease), clockwise: false, startRadius: radius, endRadius: radius);

        /// <summary>
        /// Circles with custom start and end radii on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this Transform t, float startRadius, float endRadius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(t.gameObject, duration, ApplyEases(options, ease), clockwise: false, startRadius: startRadius, endRadius: endRadius);

        /// <summary>
        /// Circles around a point on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<OrbitXZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Circles with a custom radius on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this GameObject go, float radius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(go, duration, ApplyEases(options, ease), clockwise: false, startRadius: radius, endRadius: radius);

        /// <summary>
        /// Circles with custom start and end radii on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this GameObject go, float startRadius, float endRadius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(go, duration, ApplyEases(options, ease), clockwise: false, startRadius: startRadius, endRadius: endRadius);

        /// <summary>
        /// Circles clockwise on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZClockwise(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<OrbitXZClockwisePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Circles clockwise with a custom radius on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZClockwise(this Transform t, float radius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(t.gameObject, duration, ApplyEases(options, ease), clockwise: true, startRadius: radius, endRadius: radius);

        /// <summary>
        /// Circles clockwise with custom start and end radii on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZClockwise(this Transform t, float startRadius, float endRadius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(t.gameObject, duration, ApplyEases(options, ease), clockwise: true, startRadius: startRadius, endRadius: endRadius);

        /// <summary>
        /// Circles clockwise on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZClockwise(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<OrbitXZClockwisePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Circles clockwise with a custom radius on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZClockwise(this GameObject go, float radius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(go, duration, ApplyEases(options, ease), clockwise: true, startRadius: radius, endRadius: radius);

        /// <summary>
        /// Circles clockwise with custom start and end radii on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZClockwise(this GameObject go, float startRadius, float endRadius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(go, duration, ApplyEases(options, ease), clockwise: true, startRadius: startRadius, endRadius: endRadius);

        /// <summary>
        /// Circles counter-clockwise on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZCounterClockwise(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<OrbitXZCounterClockwisePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Circles counter-clockwise with a custom radius on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZCounterClockwise(this Transform t, float radius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(t.gameObject, duration, ApplyEases(options, ease), clockwise: false, startRadius: radius, endRadius: radius);

        /// <summary>
        /// Circles counter-clockwise with custom start and end radii on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZCounterClockwise(this Transform t, float startRadius, float endRadius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(t.gameObject, duration, ApplyEases(options, ease), clockwise: false, startRadius: startRadius, endRadius: endRadius);

        /// <summary>
        /// Circles counter-clockwise on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZCounterClockwise(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<OrbitXZCounterClockwisePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Circles counter-clockwise with a custom radius on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZCounterClockwise(this GameObject go, float radius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(go, duration, ApplyEases(options, ease), clockwise: false, startRadius: radius, endRadius: radius);

        /// <summary>
        /// Circles counter-clockwise with custom start and end radii on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZCounterClockwise(this GameObject go, float startRadius, float endRadius, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => OrbitInternal(go, duration, ApplyEases(options, ease), clockwise: false, startRadius: startRadius, endRadius: endRadius);

        /// <summary>
        /// Spirals upward combining rotation and height.
        /// </summary>
        public static TweenHandle Spiral(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpiralPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spirals upward combining rotation and height.
        /// </summary>
        public static TweenHandle Spiral(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpiralPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Moves toward camera with scale increase.
        /// </summary>
        /// Falls from above with bounce on landing.
        /// </summary>
        public static TweenHandle DropIn(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<DropInPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Falls from above with bounce on landing.
        /// </summary>
        public static TweenHandle DropIn(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<DropInPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Quick upward motion with ease-out.
        /// </summary>
        public static TweenHandle LaunchUp(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<LaunchUpPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Quick upward motion with ease-out.
        /// </summary>
        public static TweenHandle LaunchUp(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<LaunchUpPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        #endregion

        #region Fade Presets

        /// <summary>
        /// Fades in from transparent.
        /// </summary>
        public static TweenHandle FadeIn(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<FadeInPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Fades in from transparent.
        /// </summary>
        public static TweenHandle FadeIn(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<FadeInPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Fades out to transparent.
        /// </summary>
        public static TweenHandle FadeOut(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<FadeOutPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Fades out to transparent.
        /// </summary>
        public static TweenHandle FadeOut(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<FadeOutPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        #endregion

        #region Rotation Presets

        /// <summary>
        /// Spins 360 degrees on Y axis.
        /// </summary>
        public static TweenHandle SpinY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpinPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins 360 degrees on Y axis.
        /// </summary>
        public static TweenHandle SpinY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpinPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins 360 degrees on X axis.
        /// </summary>
        public static TweenHandle SpinX(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpinXPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins 360 degrees on X axis.
        /// </summary>
        public static TweenHandle SpinX(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpinXPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins 360 degrees on Z axis.
        /// </summary>
        public static TweenHandle SpinZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpinZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins 360 degrees on Z axis.
        /// </summary>
        public static TweenHandle SpinZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpinZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins diagonally across X and Y.
        /// </summary>
        public static TweenHandle SpinDiagonalXY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpinDiagonalXYPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins diagonally across X and Y.
        /// </summary>
        public static TweenHandle SpinDiagonalXY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpinDiagonalXYPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins diagonally across X and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalXZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpinDiagonalXZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins diagonally across X and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalXZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpinDiagonalXZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins diagonally across Y and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalYZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<SpinDiagonalYZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Spins diagonally across Y and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalYZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<SpinDiagonalYZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation back and forth on Y.
        /// </summary>
        public static TweenHandle WobbleY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<WobblePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation back and forth on Y.
        /// </summary>
        public static TweenHandle WobbleY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<WobblePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation back and forth on X.
        /// </summary>
        public static TweenHandle WobbleX(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<WobbleXPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation back and forth on X.
        /// </summary>
        public static TweenHandle WobbleX(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<WobbleXPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation back and forth on Z.
        /// </summary>
        public static TweenHandle WobbleZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<WobbleZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation back and forth on Z.
        /// </summary>
        public static TweenHandle WobbleZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<WobbleZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation diagonally across X and Y.
        /// </summary>
        public static TweenHandle WobbleDiagonalXY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<WobbleDiagonalXYPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation diagonally across X and Y.
        /// </summary>
        public static TweenHandle WobbleDiagonalXY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<WobbleDiagonalXYPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation diagonally across X and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalXZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<WobbleDiagonalXZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation diagonally across X and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalXZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<WobbleDiagonalXZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation diagonally across Y and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalYZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<WobbleDiagonalYZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Wobbles rotation diagonally across Y and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalYZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<WobbleDiagonalYZPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        #endregion

        #region Combined Presets

        /// <summary>
        /// Scales and fades in together.
        /// </summary>
        public static TweenHandle PopInFade(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<PopInFadePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Scales and fades in together.
        /// </summary>
        public static TweenHandle PopInFade(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<PopInFadePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Attention-grabbing pulse.
        /// </summary>
        public static TweenHandle Attention(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<AttentionPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Attention-grabbing pulse.
        /// </summary>
        public static TweenHandle Attention(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<AttentionPreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Scales down and fades out together.
        /// </summary>
        public static TweenHandle PopOutFade(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(t).Preset<PopOutFadePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        /// <summary>
        /// Scales down and fades out together.
        /// </summary>
        public static TweenHandle PopOutFade(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => new TweenBuilder(go).Preset<PopOutFadePreset>(duration).WithOptions(ApplyEases(options, ease)).Play();

        #endregion

        #region Compatibility aliases

        public static TweenHandle Spin(this Transform t, float? duration = null, TweenOptions options = default)
            => t.SpinY(duration, options);

        public static TweenHandle Spin(this GameObject go, float? duration = null, TweenOptions options = default)
            => go.SpinY(duration, options);

        public static TweenHandle Wobble(this Transform t, float? duration = null, TweenOptions options = default)
            => t.WobbleY(duration, options);

        public static TweenHandle Wobble(this GameObject go, float? duration = null, TweenOptions options = default)
            => go.WobbleY(duration, options);

        #endregion

        #region Internal helpers

        private static TweenOptions ApplyEases(TweenOptions options, Ease? ease, Ease? secondaryEase = null, Ease? tertiaryEase = null)
        {
            if (ease.HasValue)
            {
                options = options.SetEase(ease.Value);
            }
            if (secondaryEase.HasValue)
            {
                options = options.SetSecondaryEase(secondaryEase.Value);
            }
            if (tertiaryEase.HasValue)
            {
                options = options.SetTertiaryEase(tertiaryEase.Value);
            }
            return options;
        }

        private static TweenHandle OrbitInternal(GameObject target, float? duration, TweenOptions options, bool clockwise, float startRadius, float endRadius)
        {
            var tween = OrbitTweenFactory.Create(target, duration, options, clockwise, startRadius, endRadius);
            tween?.Play();
            return new TweenHandle(tween);
        }

        #endregion
    }
}
