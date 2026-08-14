using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class NormalizedTweenTimeline
    {
        public static Tween Create(
            GameObject owner,
            float duration,
            TweenOptions rootOptions,
            Action initialize,
            Action<float> evaluate,
            Action completeForward,
            Action completeAtInvocation,
            Action rewind,
            Action interruptedKill = null,
            Action started = null)
        {
            ValidateRequest(owner, duration, initialize, evaluate, completeForward, completeAtInvocation, rewind);

            float progress = 0f;
            bool initialized = false;
            bool completed = false;

            void EnsureInitialized()
            {
                if (initialized) return;
                initialize();
                initialized = true;
            }

            var tween = DOTween.To(() => progress, value =>
            {
                progress = value;
                EnsureInitialized();
                evaluate(Mathf.Clamp01(value));
            }, 1f, duration);

            tween.WithDefaults(rootOptions, owner);
            tween.OnStart(() =>
            {
                completed = false;
                EnsureInitialized();
                started?.Invoke();
            });
            tween.OnComplete(() =>
            {
                EnsureInitialized();
                completed = true;
                if (EndsAtInvocation(rootOptions)) completeAtInvocation();
                else completeForward();
            });
            tween.OnRewind(() =>
            {
                completed = false;
                if (initialized) rewind();
            });

            if (interruptedKill != null)
            {
                tween.OnKill(() =>
                {
                    if (initialized && !completed) interruptedKill();
                });
            }

            tween.Pause();
            return tween;
        }

        public static bool EndsAtInvocation(TweenOptions options)
        {
            int loops = options.Loops ?? 1;
            return loops > 0 && options.LoopType == LoopType.Yoyo && loops % 2 == 0;
        }

        private static void ValidateRequest(GameObject owner, float duration, Action initialize, Action<float> evaluate, Action completeForward, Action completeAtInvocation, Action rewind)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be finite and greater than zero.");
            if (initialize == null) throw new ArgumentNullException(nameof(initialize));
            if (evaluate == null) throw new ArgumentNullException(nameof(evaluate));
            if (completeForward == null) throw new ArgumentNullException(nameof(completeForward));
            if (completeAtInvocation == null) throw new ArgumentNullException(nameof(completeAtInvocation));
            if (rewind == null) throw new ArgumentNullException(nameof(rewind));
        }
    }
}
