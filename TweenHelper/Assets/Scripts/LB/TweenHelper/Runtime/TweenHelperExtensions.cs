using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Extension methods for Transform and GameObject to enable fluent animation syntax.
    /// Usage: transform.TweenMove(Vector3.up * 2f);
    /// </summary>
    public static class TweenHelperExtensions
    {
        #region Transform Extensions - Movement

        /// <summary>
        /// Moves the transform to a target position.
        /// </summary>
        /// <param name="t">The transform to move.</param>
        /// <param name="target">The target position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenMove(this Transform t, Vector3 target, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.MoveTo(t, target, duration, options);
        }

        /// <summary>
        /// Moves the transform by an offset relative to current position.
        /// </summary>
        /// <param name="t">The transform to move.</param>
        /// <param name="offset">The offset to move by.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenMoveBy(this Transform t, Vector3 offset, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.MoveBy(t, offset, duration, options);
        }

        /// <summary>
        /// Moves the transform to a local position.
        /// </summary>
        /// <param name="t">The transform to move.</param>
        /// <param name="target">The target local position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenMoveLocal(this Transform t, Vector3 target, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.MoveToLocal(t, target, duration, options);
        }

        #endregion

        #region Transform Extensions - Scale

        /// <summary>
        /// Scales the transform to a target scale.
        /// </summary>
        /// <param name="t">The transform to scale.</param>
        /// <param name="scale">The target scale.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenScale(this Transform t, Vector3 scale, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.ScaleTo(t, scale, duration, options);
        }

        /// <summary>
        /// Scales the transform uniformly to a target scale.
        /// </summary>
        /// <param name="t">The transform to scale.</param>
        /// <param name="scale">The target uniform scale.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenScale(this Transform t, float scale, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.ScaleTo(t, scale, duration, options);
        }

        /// <summary>
        /// Scales the transform by a multiplier relative to current scale.
        /// </summary>
        /// <param name="t">The transform to scale.</param>
        /// <param name="multiplier">The scale multiplier.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenScaleBy(this Transform t, float multiplier, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.ScaleBy(t, multiplier, duration, options);
        }

        #endregion

        #region Transform Extensions - Rotation

        /// <summary>
        /// Rotates the transform to a target rotation.
        /// </summary>
        /// <param name="t">The transform to rotate.</param>
        /// <param name="rotation">The target rotation in Euler angles.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenRotate(this Transform t, Vector3 rotation, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.RotateTo(t, rotation, duration, options);
        }

        /// <summary>
        /// Rotates the transform by an offset relative to current rotation.
        /// </summary>
        /// <param name="t">The transform to rotate.</param>
        /// <param name="rotation">The rotation offset in Euler angles.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenRotateBy(this Transform t, Vector3 rotation, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.RotateBy(t, rotation, duration, options);
        }

        /// <summary>
        /// Rotates the transform to look at a target position.
        /// </summary>
        /// <param name="t">The transform to rotate.</param>
        /// <param name="target">The position to look at.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenLookAt(this Transform t, Vector3 target, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.LookAt(t, target, duration, options);
        }

        #endregion

        #region Transform Extensions - Effects

        /// <summary>
        /// Plays a pop-in animation on the transform.
        /// </summary>
        /// <param name="t">The transform to animate.</param>
        /// <param name="duration">Duration override (null uses default).</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenPopIn(this Transform t, float? duration = null)
        {
            return TweenAnimations.PopIn(t, duration ?? 0.3f);
        }

        /// <summary>
        /// Plays a pop-out animation on the transform.
        /// </summary>
        /// <param name="t">The transform to animate.</param>
        /// <param name="duration">Duration override (null uses default).</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenPopOut(this Transform t, float? duration = null)
        {
            return TweenAnimations.PopOut(t, duration ?? 0.3f);
        }

        /// <summary>
        /// Shakes the transform.
        /// </summary>
        /// <param name="t">The transform to shake.</param>
        /// <param name="strength">Shake intensity.</param>
        /// <param name="duration">Duration override (null uses default).</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenShake(this Transform t, float strength = 0.5f, float? duration = null)
        {
            return TweenAnimations.Shake(t, strength, duration ?? 0.5f);
        }

        /// <summary>
        /// Pulses the transform scale.
        /// </summary>
        /// <param name="t">The transform to pulse.</param>
        /// <param name="scale">Maximum scale during pulse.</param>
        /// <param name="duration">Duration override (null uses default).</param>
        /// <returns>The created sequence.</returns>
        public static Sequence TweenPulse(this Transform t, float scale = 1.2f, float? duration = null)
        {
            return TweenAnimations.Pulse(t, scale, duration ?? 0.3f);
        }

        #endregion

        #region Transform Extensions - Sequence Builder

        /// <summary>
        /// Creates a new sequence builder for this transform.
        /// </summary>
        /// <param name="t">The transform to use as default target.</param>
        /// <returns>A new TweenSequenceBuilder.</returns>
        public static TweenSequenceBuilder TweenSequence(this Transform t)
        {
            return TweenHelper.CreateSequence(t);
        }

        #endregion

        #region GameObject Extensions

        /// <summary>
        /// Moves the GameObject to a target position.
        /// </summary>
        /// <param name="go">The GameObject to move.</param>
        /// <param name="target">The target position.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenMove(this GameObject go, Vector3 target, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.MoveTo(go.transform, target, duration, options);
        }

        /// <summary>
        /// Scales the GameObject uniformly to a target scale.
        /// </summary>
        /// <param name="go">The GameObject to scale.</param>
        /// <param name="scale">The target uniform scale.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenScale(this GameObject go, float scale, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.ScaleTo(go.transform, scale, duration, options);
        }

        /// <summary>
        /// Rotates the GameObject to a target rotation.
        /// </summary>
        /// <param name="go">The GameObject to rotate.</param>
        /// <param name="rotation">The target rotation in Euler angles.</param>
        /// <param name="duration">Duration override (null uses settings default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenRotate(this GameObject go, Vector3 rotation, float? duration = null, TweenOptions options = default)
        {
            return TweenHelper.RotateTo(go.transform, rotation, duration, options);
        }

        /// <summary>
        /// Creates a new sequence builder for this GameObject.
        /// </summary>
        /// <param name="go">The GameObject to use as default target.</param>
        /// <returns>A new TweenSequenceBuilder.</returns>
        public static TweenSequenceBuilder TweenSequence(this GameObject go)
        {
            return TweenHelper.CreateSequence(go);
        }

        /// <summary>
        /// Plays a pop-in animation on the GameObject.
        /// </summary>
        /// <param name="go">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses default).</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenPopIn(this GameObject go, float? duration = null)
        {
            return TweenAnimations.PopIn(go.transform, duration ?? 0.3f);
        }

        /// <summary>
        /// Plays a pop-out animation on the GameObject.
        /// </summary>
        /// <param name="go">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses default).</param>
        /// <returns>The created tween.</returns>
        public static Tween TweenPopOut(this GameObject go, float? duration = null)
        {
            return TweenAnimations.PopOut(go.transform, duration ?? 0.3f);
        }

        #endregion
    }
}
