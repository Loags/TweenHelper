using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Ready-to-use animations. No setup needed - just call and play.
    /// All animations use smooth easing by default.
    /// </summary>
    public static class TweenAnimations
    {
        #region Looping Patterns

        /// <summary>
        /// Realistic bouncing ball animation with exponential height decay.
        /// Starts at current position, bounces up/down, each bounce smaller than the last.
        /// Always returns to the exact starting position.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="height">Initial bounce height.</param>
        /// <param name="bounces">Number of bounces (default 4).</param>
        /// <param name="duration">Total duration for all bounces.</param>
        /// <param name="decay">Height decay per bounce (0.5 = half height each time).</param>
        /// <param name="startDown">If true, drops down first then bounces up (like a falling ball). If false, goes up first.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Bounce(Transform t, float height = 2f, int bounces = 4, float duration = 2f, float decay = 0.6f, bool startDown = false)
        {
            var builder = TweenHelper.CreateSequence(t);
            Vector3 startPos = t.position;

            // Calculate total "weight" for duration distribution (earlier bounces take more time)
            float totalWeight = 0f;
            float currentHeight = height;
            for (int i = 0; i < bounces; i++)
            {
                totalWeight += Mathf.Sqrt(currentHeight); // Time proportional to sqrt of height (physics)
                currentHeight *= decay;
            }

            // Direction vectors
            Vector3 firstDir = startDown ? Vector3.down : Vector3.up;
            Vector3 secondDir = startDown ? Vector3.up : Vector3.down;
            Ease firstEase = startDown ? Ease.InQuad : Ease.OutQuad;  // Down accelerates, Up decelerates
            Ease secondEase = startDown ? Ease.OutQuad : Ease.InQuad; // Up decelerates, Down accelerates

            // Build each bounce
            currentHeight = height;
            for (int i = 0; i < bounces; i++)
            {
                // Duration proportional to sqrt of height (like real physics: t = sqrt(2h/g))
                float bounceDuration = duration * (Mathf.Sqrt(currentHeight) / totalWeight);
                float halfDuration = bounceDuration * 0.5f;

                builder.MoveBy(t, firstDir * currentHeight, halfDuration, TweenOptions.WithEase(firstEase));
                builder.MoveBy(t, secondDir * currentHeight, halfDuration, TweenOptions.WithEase(secondEase));

                currentHeight *= decay;
            }

            // Ensure we end exactly at the start position (avoid floating point drift)
            builder.OnComplete(() => t.position = startPos);

            return builder.Play();
        }

        /// <summary>
        /// Side-to-side swinging motion. Loops infinitely.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="distance">How far to swing.</param>
        /// <param name="duration">Duration for one swing cycle.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Pendulum(Transform t, float distance = 2f, float duration = 1f)
        {
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, Vector3.right * distance, duration, TweenOptions.WithEase(Ease.InOutSine))
                .Play(TweenOptions.WithLoops(-1, LoopType.Yoyo));
        }

        /// <summary>
        /// Traces a square path continuously. Loops infinitely.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="size">Size of the square.</param>
        /// <param name="duration">Duration for one complete loop.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence SquareLoop(Transform t, float size = 2f, float duration = 2f)
        {
            float step = duration / 4f;
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, Vector3.right * size, step, ease)
                .MoveBy(t, Vector3.up * size, step, ease)
                .MoveBy(t, Vector3.left * size, step, ease)
                .MoveBy(t, Vector3.down * size, step, ease)
                .Play(TweenOptions.WithLoops(-1, LoopType.Restart));
        }

        /// <summary>
        /// Traces a triangular path continuously. Loops infinitely.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="size">Size of the triangle.</param>
        /// <param name="duration">Duration for one complete loop.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence TriangleLoop(Transform t, float size = 2f, float duration = 1.5f)
        {
            float step = duration / 3f;
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, new Vector3(size * 0.5f, size, 0f), step, ease)
                .MoveBy(t, new Vector3(size * 0.5f, -size, 0f), step, ease)
                .MoveBy(t, Vector3.left * size, step, ease)
                .Play(TweenOptions.WithLoops(-1, LoopType.Restart));
        }

        #endregion

        #region Path Patterns

        /// <summary>
        /// Traces a figure-8 pattern.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="size">Size of the figure-8.</param>
        /// <param name="duration">Duration for one complete cycle.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Figure8(Transform t, float size = 1f, float duration = 2f)
        {
            float step = duration / 8f;
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, new Vector3(size, size, 0f), step, ease)
                .MoveBy(t, new Vector3(size, -size, 0f), step, ease)
                .MoveBy(t, new Vector3(-size, -size, 0f), step, ease)
                .MoveBy(t, new Vector3(-size, size, 0f), step, ease)
                .MoveBy(t, new Vector3(-size, -size, 0f), step, ease)
                .MoveBy(t, new Vector3(-size, size, 0f), step, ease)
                .MoveBy(t, new Vector3(size, size, 0f), step, ease)
                .MoveBy(t, new Vector3(size, -size, 0f), step, ease)
                .Play();
        }

        /// <summary>
        /// Traces a diamond shape path.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="size">Size of the diamond.</param>
        /// <param name="duration">Duration for one complete cycle.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Diamond(Transform t, float size = 1.5f, float duration = 2f)
        {
            float step = duration / 4f;
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, new Vector3(size, size, 0f), step, ease)
                .MoveBy(t, new Vector3(size, -size, 0f), step, ease)
                .MoveBy(t, new Vector3(-size, -size, 0f), step, ease)
                .MoveBy(t, new Vector3(-size, size, 0f), step, ease)
                .Play();
        }

        /// <summary>
        /// Moves in a staircase pattern (right then up).
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="stepSize">Size of each step.</param>
        /// <param name="steps">Number of steps.</param>
        /// <param name="duration">Total duration.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Staircase(Transform t, float stepSize = 0.5f, int steps = 3, float duration = 1f)
        {
            float stepDuration = duration / (steps * 2);
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            var builder = TweenHelper.CreateSequence(t);

            for (int i = 0; i < steps; i++)
            {
                builder.MoveBy(t, Vector3.right * stepSize, stepDuration, ease);
                builder.MoveBy(t, Vector3.up * stepSize, stepDuration, ease);
            }

            return builder.Play();
        }

        /// <summary>
        /// Spirals outward from the starting position.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="duration">Total duration.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence SpiralOut(Transform t, float duration = 2f)
        {
            float step = duration / 8f;
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, Vector3.right * 0.5f, step, ease)
                .MoveBy(t, Vector3.up * 0.5f, step, ease)
                .MoveBy(t, Vector3.left * 1f, step, ease)
                .MoveBy(t, Vector3.down * 1f, step, ease)
                .MoveBy(t, Vector3.right * 1.5f, step, ease)
                .MoveBy(t, Vector3.up * 1.5f, step, ease)
                .MoveBy(t, Vector3.left * 2f, step, ease)
                .MoveBy(t, Vector3.down * 2f, step, ease)
                .Play();
        }

        /// <summary>
        /// Moves in a zigzag pattern.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="distance">Horizontal distance of each zig.</param>
        /// <param name="duration">Total duration.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Zigzag(Transform t, float distance = 1.5f, float duration = 1f)
        {
            float step = duration / 4f;
            var ease = TweenOptions.WithEase(Ease.InOutSine);
            return TweenHelper.CreateSequence(t)
                .MoveBy(t, Vector3.right * distance, step, ease)
                .MoveBy(t, new Vector3(-distance, 1f, 0f), step, ease)
                .MoveBy(t, Vector3.right * distance, step, ease)
                .MoveBy(t, new Vector3(-distance, 1f, 0f), step, ease)
                .Play();
        }

        #endregion

        #region Effects

        /// <summary>
        /// Scales from zero to current scale with a bouncy ease.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="duration">Duration of the animation.</param>
        /// <returns>The playing tween.</returns>
        public static Tween PopIn(Transform t, float duration = 0.3f)
        {
            var originalScale = t.localScale;
            t.localScale = Vector3.zero;
            return TweenHelper.ScaleTo(t, originalScale, duration, TweenOptions.WithEase(Ease.OutBack));
        }

        /// <summary>
        /// Scales to zero with a sharp ease.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="duration">Duration of the animation.</param>
        /// <returns>The playing tween.</returns>
        public static Tween PopOut(Transform t, float duration = 0.3f)
        {
            return TweenHelper.ScaleTo(t, Vector3.zero, duration, TweenOptions.WithEase(Ease.InBack));
        }

        /// <summary>
        /// Shakes the transform randomly.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="strength">Shake intensity.</param>
        /// <param name="duration">Duration of the shake.</param>
        /// <returns>The playing tween.</returns>
        public static Tween Shake(Transform t, float strength = 0.5f, float duration = 0.5f)
        {
            return TweenHelper.Shake(t, strength, duration, 10);
        }

        /// <summary>
        /// Pulses the scale up and back down.
        /// </summary>
        /// <param name="t">Transform to animate.</param>
        /// <param name="scale">Maximum scale during pulse.</param>
        /// <param name="duration">Duration of the pulse.</param>
        /// <returns>The playing sequence.</returns>
        public static Sequence Pulse(Transform t, float scale = 1.2f, float duration = 0.3f)
        {
            return TweenHelper.Bounce(t, scale, duration);
        }

        #endregion
    }
}
