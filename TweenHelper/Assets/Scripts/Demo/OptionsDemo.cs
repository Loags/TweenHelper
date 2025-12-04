using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides TweenOptions demos: easing, delays, loops, unscaled time, snapping, speed-based.
    /// </summary>
    public class OptionsDemo : MonoBehaviour, IDemoAnimationProvider
    {
        private GameObject[] demoObjects;

        private readonly Ease[] easingTypes =
        {
            Ease.Linear, Ease.InQuad, Ease.OutQuad, Ease.InOutQuad,
            Ease.InCubic, Ease.OutCubic, Ease.InOutCubic,
            Ease.InBounce, Ease.OutBounce, Ease.InOutBounce,
            Ease.InElastic, Ease.OutElastic, Ease.InOutElastic,
            Ease.InBack, Ease.OutBack, Ease.InOutBack
        };

        public string CategoryName => "Options";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "Random Easing",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var ease = easingTypes[Random.Range(0, easingTypes.Length)];
                    var options = TweenOptions.WithEase(ease);
                    Debug.Log($"Using ease: {ease}");

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.right * 5f, duration, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Delay Demo",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    float delay = Random.Range(0.5f, 2f);
                    var options = TweenOptions.WithDelay(delay);
                    Debug.Log($"Animation starts after {delay:F1}s delay");

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.up * 3f, duration, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Loops (Restart)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithLoops(3, LoopType.Restart);

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.forward * 3f, duration * 0.5f, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Loops (Yoyo)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithLoops(4, LoopType.Yoyo);

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.ScaleTo(t, 1.5f, duration * 0.3f, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Loops (Incremental)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithLoops(3, LoopType.Incremental);

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.RotateBy(t, new Vector3(0f, 45f, 0f), duration * 0.3f, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Unscaled Time",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithUnscaledTime();
                    Debug.Log("Using unscaled time - animation ignores Time.timeScale");

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.left * 4f, duration, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Snapping",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithSnapping();
                    Debug.Log("Snapping enabled - positions rounded to integers");

                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        var target = t.position + new Vector3(3.7f, 2.3f, 1.9f);
                        TweenHelper.MoveTo(t, target, duration, options);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Speed Based",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithSpeedBased();
                    float speed = 3f;
                    Debug.Log($"Speed-based: {speed} units/second");

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.back * 6f, speed, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Combined Options",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.WithEase(Ease.OutBounce)
                        .SetDelay(0.5f)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetId("combined-demo")
                        .SetSnapping(true);

                    Debug.Log("Combined: OutBounce, 0.5s delay, 2 yoyo loops, snapping, ID");

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.up * 2f, duration, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Fluent API Chain",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var options = TweenOptions.None
                        .SetEase(Ease.InOutElastic)
                        .SetDelay(0.3f)
                        .SetLoops(3, LoopType.Yoyo)
                        .SetId("fluent-demo");

                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveTo(t, t.position + Vector3.forward * 3f, duration, options);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Easing Comparison",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    var eases = new[] { Ease.Linear, Ease.OutQuart, Ease.OutBounce, Ease.OutElastic };
                    var names = new[] { "Linear", "OutQuart", "OutBounce", "OutElastic" };

                    for (int i = 0; i < Mathf.Min(transforms.Length, eases.Length); i++)
                    {
                        var t = transforms[i];
                        if (t == null) continue;

                        Debug.Log($"Object {i + 1}: {names[i]}");
                        var options = TweenOptions.WithEase(eases[i]);
                        TweenHelper.MoveTo(t, t.position + Vector3.right * 6f, duration * 1.5f, options);
                    }
                }
            };
        }
    }
}
