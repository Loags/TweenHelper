using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides preset animation demos: PopIn, PopOut, Bounce, Shake, FadeIn, FadeOut.
    /// </summary>
    public class PresetDemo : MonoBehaviour, IDemoAnimationProvider
    {
        private GameObject[] demoObjects;

        public string CategoryName => "Presets";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "PopIn",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        t.localScale = Vector3.zero;
                        TweenHelper.PlayPreset("PopIn", t, duration);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "PopOut",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.PlayPreset("PopOut", t, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Bounce",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.PlayPreset("Bounce", t, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Shake",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.PlayPreset("Shake", t, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "FadeIn",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        var cg = t.GetComponent<CanvasGroup>();
                        if (cg != null) cg.alpha = 0f;
                        TweenHelper.PlayPreset("FadeIn", t, duration);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "FadeOut",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.PlayPreset("FadeOut", t, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Preset with Options",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithEase(Ease.OutElastic)
                        .SetLoops(2, LoopType.Yoyo);

                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        t.localScale = Vector3.zero;
                        TweenHelper.PlayPreset("PopIn", t, duration, options);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Preset Chain",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        t.localScale = Vector3.zero;

                        var popIn = TweenHelper.PlayPreset("PopIn", t, duration * 0.5f);
                        popIn?.OnComplete(() =>
                        {
                            var bounce = TweenHelper.PlayPreset("Bounce", t, duration * 0.5f);
                            bounce?.OnComplete(() =>
                            {
                                TweenHelper.PlayPreset("Shake", t, duration * 0.3f);
                            });
                        });
                    }
                }
            };
        }
    }
}
