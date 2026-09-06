using DG.Tweening;

namespace LB.TweenHelper
{
    internal static class TweenLifetime
    {
        internal static void Kill(Tween tween, bool complete = false)
        {
            if (tween == null || !tween.IsActive()) return;
#if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying)
            {
                if (complete) tween.Complete();
                if (!tween.IsActive()) return;
                // DOTween's instance Kill ignores Edit Mode before runtime initialization.
                object originalId = tween.id;
                TweenCallback originalCallbacks = tween.onKill;
                var identity = new object();
                tween.id = identity;
                tween.onKill = () => tween.id = originalId;
                tween.onKill += originalCallbacks;
                try { DOTween.Kill(identity); }
                finally
                {
                    if (tween.IsActive())
                    {
                        tween.id = originalId;
                        tween.onKill = originalCallbacks;
                    }
                }
                return;
            }
#endif
            tween.Kill(complete);
        }
    }
}
