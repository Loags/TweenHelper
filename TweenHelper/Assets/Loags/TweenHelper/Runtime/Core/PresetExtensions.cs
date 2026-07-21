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
            => PlayPreset<PopInPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Scales from 0 to original scale with overshoot.
        /// </summary>
        public static TweenHandle PopIn(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopInPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Scales to 0 with anticipation.
        /// </summary>
        public static TweenHandle PopOut(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopOutPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Scales to 0 with anticipation.
        /// </summary>
        public static TweenHandle PopOut(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopOutPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Quick scale punch for feedback.
        /// </summary>
        public static TweenHandle Punch(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PunchPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Quick scale punch for feedback.
        /// </summary>
        public static TweenHandle Punch(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PunchPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Squash and stretch effect (was Bounce).
        /// </summary>
        public static TweenHandle Bounce(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<BouncePreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Squash and stretch effect (was Bounce).
        /// </summary>
        public static TweenHandle Bounce(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<BouncePreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Squash and stretch effect.
        /// </summary>
        public static TweenHandle Squash(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<BouncePreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Squash and stretch effect.
        /// </summary>
        public static TweenHandle Squash(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<BouncePreset>(go, duration, ApplyEases(options, ease));

        #endregion

        #region Position Presets

        /// <summary>
        /// Random position shake.
        /// </summary>
        public static TweenHandle Shake(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<ShakePreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Random position shake.
        /// </summary>
        public static TweenHandle Shake(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<ShakePreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides in from above.
        /// </summary>
        public static TweenHandle SlideInDown(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInDownPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides in from above.
        /// </summary>
        public static TweenHandle SlideInDown(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInDownPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides up from below.
        /// </summary>
        public static TweenHandle SlideInUp(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInUpPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides up from below.
        /// </summary>
        public static TweenHandle SlideInUp(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInUpPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides in from the left side.
        /// </summary>
        public static TweenHandle SlideInLeft(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInLeftPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides in from the left side.
        /// </summary>
        public static TweenHandle SlideInLeft(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInLeftPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides in from the right side.
        /// </summary>
        public static TweenHandle SlideInRight(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInRightPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Slides in from the right side.
        /// </summary>
        public static TweenHandle SlideInRight(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SlideInRightPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Gentle up/down hovering loop.
        /// </summary>
        public static TweenHandle Float(this Transform t, float? duration = null, TweenOptions options = default, Ease? moveUpEase = null, Ease? moveDownEase = null)
            => PlayPreset<FloatPreset>(t, duration, ApplyEases(options, moveUpEase, moveDownEase));

        /// <summary>
        /// Gentle up/down hovering loop.
        /// </summary>
        public static TweenHandle Float(this GameObject go, float? duration = null, TweenOptions options = default, Ease? moveUpEase = null, Ease? moveDownEase = null)
            => PlayPreset<FloatPreset>(go, duration, ApplyEases(options, moveUpEase, moveDownEase));

        /// <summary>
        /// Circles around a point on XZ plane.
        /// </summary>
        public static TweenHandle OrbitXZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<OrbitXZPreset>(t, duration, ApplyEases(options, ease));

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
            => PlayPreset<OrbitXZPreset>(go, duration, ApplyEases(options, ease));

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
            => PlayPreset<OrbitXZClockwisePreset>(t, duration, ApplyEases(options, ease));

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
            => PlayPreset<OrbitXZClockwisePreset>(go, duration, ApplyEases(options, ease));

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
            => PlayPreset<OrbitXZCounterClockwisePreset>(t, duration, ApplyEases(options, ease));

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
            => PlayPreset<OrbitXZCounterClockwisePreset>(go, duration, ApplyEases(options, ease));

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
            => PlayPreset<SpiralPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spirals upward combining rotation and height.
        /// </summary>
        public static TweenHandle Spiral(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpiralPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Moves toward camera with scale increase.
        /// </summary>
        /// Falls from above with bounce on landing.
        /// </summary>
        public static TweenHandle DropIn(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<DropInPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Falls from above with bounce on landing.
        /// </summary>
        public static TweenHandle DropIn(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<DropInPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Quick upward motion with ease-out.
        /// </summary>
        public static TweenHandle LaunchUp(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<LaunchUpPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Quick upward motion with ease-out.
        /// </summary>
        public static TweenHandle LaunchUp(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<LaunchUpPreset>(go, duration, ApplyEases(options, ease));

        #endregion

        #region Fade Presets

        /// <summary>
        /// Fades in from transparent.
        /// </summary>
        public static TweenHandle FadeIn(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<FadeInPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Fades in from transparent.
        /// </summary>
        public static TweenHandle FadeIn(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<FadeInPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Fades out to transparent.
        /// </summary>
        public static TweenHandle FadeOut(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<FadeOutPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Fades out to transparent.
        /// </summary>
        public static TweenHandle FadeOut(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<FadeOutPreset>(go, duration, ApplyEases(options, ease));

        #endregion

        #region Rotation Presets

        /// <summary>
        /// Spins 360 degrees on Y axis.
        /// </summary>
        public static TweenHandle SpinY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins 360 degrees on Y axis.
        /// </summary>
        public static TweenHandle SpinY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins 360 degrees on X axis.
        /// </summary>
        public static TweenHandle SpinX(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinXPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins 360 degrees on X axis.
        /// </summary>
        public static TweenHandle SpinX(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinXPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins 360 degrees on Z axis.
        /// </summary>
        public static TweenHandle SpinZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinZPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins 360 degrees on Z axis.
        /// </summary>
        public static TweenHandle SpinZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinZPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins diagonally across X and Y.
        /// </summary>
        public static TweenHandle SpinDiagonalXY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinDiagonalXYPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins diagonally across X and Y.
        /// </summary>
        public static TweenHandle SpinDiagonalXY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinDiagonalXYPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins diagonally across X and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalXZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinDiagonalXZPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins diagonally across X and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalXZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinDiagonalXZPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins diagonally across Y and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalYZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinDiagonalYZPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Spins diagonally across Y and Z.
        /// </summary>
        public static TweenHandle SpinDiagonalYZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<SpinDiagonalYZPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation back and forth on Y.
        /// </summary>
        public static TweenHandle WobbleY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobblePreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation back and forth on Y.
        /// </summary>
        public static TweenHandle WobbleY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobblePreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation back and forth on X.
        /// </summary>
        public static TweenHandle WobbleX(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleXPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation back and forth on X.
        /// </summary>
        public static TweenHandle WobbleX(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleXPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation back and forth on Z.
        /// </summary>
        public static TweenHandle WobbleZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleZPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation back and forth on Z.
        /// </summary>
        public static TweenHandle WobbleZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleZPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation diagonally across X and Y.
        /// </summary>
        public static TweenHandle WobbleDiagonalXY(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleDiagonalXYPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation diagonally across X and Y.
        /// </summary>
        public static TweenHandle WobbleDiagonalXY(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleDiagonalXYPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation diagonally across X and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalXZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleDiagonalXZPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation diagonally across X and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalXZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleDiagonalXZPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation diagonally across Y and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalYZ(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleDiagonalYZPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Wobbles rotation diagonally across Y and Z.
        /// </summary>
        public static TweenHandle WobbleDiagonalYZ(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<WobbleDiagonalYZPreset>(go, duration, ApplyEases(options, ease));

        #endregion

        #region Combined Presets

        /// <summary>
        /// Scales and fades in together.
        /// </summary>
        public static TweenHandle PopInFade(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopInFadePreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Scales and fades in together.
        /// </summary>
        public static TweenHandle PopInFade(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopInFadePreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Attention-grabbing pulse.
        /// </summary>
        public static TweenHandle Attention(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<AttentionPreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Attention-grabbing pulse.
        /// </summary>
        public static TweenHandle Attention(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<AttentionPreset>(go, duration, ApplyEases(options, ease));

        /// <summary>
        /// Scales down and fades out together.
        /// </summary>
        public static TweenHandle PopOutFade(this Transform t, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopOutFadePreset>(t, duration, ApplyEases(options, ease));

        /// <summary>
        /// Scales down and fades out together.
        /// </summary>
        public static TweenHandle PopOutFade(this GameObject go, float? duration = null, TweenOptions options = default, Ease? ease = null)
            => PlayPreset<PopOutFadePreset>(go, duration, ApplyEases(options, ease));

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

        private static TweenHandle PlayPreset<TPreset>(Transform target, float? duration, TweenOptions options) where TPreset : class, ITweenPreset
            => PlayPreset<TPreset>(target.gameObject, duration, options);

        private static TweenHandle PlayPreset<TPreset>(GameObject target, float? duration, TweenOptions options) where TPreset : class, ITweenPreset
            => new TweenHandle(TweenPresetRegistry.PlayUnchecked<TPreset>(target, duration, options));

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
