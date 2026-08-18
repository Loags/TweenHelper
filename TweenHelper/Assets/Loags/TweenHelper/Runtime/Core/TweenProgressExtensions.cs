using System;
using DG.Tweening;

namespace LB.TweenHelper
{
    /// <summary>Progress-based callback hooks that compose with an already-built TweenHandle.</summary>
    public static class TweenProgressExtensions
    {
        public static TweenHandle OnProgress(this TweenHandle handle, float fraction, Action callback, bool everyLoop = false)
        {
            if (handle == null) throw new ArgumentNullException(nameof(handle));
            if (handle.Tween == null) throw new InvalidOperationException("Cannot attach a progress hook to an empty TweenHandle.");
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            if (float.IsNaN(fraction) || float.IsInfinity(fraction) || fraction < 0f || fraction > 1f) throw new ArgumentOutOfRangeException(nameof(fraction), fraction, "Progress fraction must be between zero and one.");

            Tween tween = handle.Tween;
            bool fired = false;
            int observedLoop = tween.CompletedLoops();
            tween.onUpdate += () =>
            {
                int currentLoop = tween.CompletedLoops();
                if (everyLoop && currentLoop != observedLoop)
                {
                    observedLoop = currentLoop;
                    fired = false;
                }

                if (fired || tween.ElapsedPercentage(false) < fraction) return;
                fired = true;
                callback();
            };
            tween.onRewind += () =>
            {
                observedLoop = 0;
                fired = false;
            };
            return handle;
        }
    }
}
