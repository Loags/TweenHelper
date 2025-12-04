using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides staggered animation demos using TweenStagger.
    /// </summary>
    public class StaggerDemo : MonoBehaviour, IDemoAnimationProvider
    {
        [Header("Settings")]
        [SerializeField] private float staggerDelay = 0.15f;

        private GameObject[] demoObjects;

        public string CategoryName => "Stagger";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "Stagger Move (Up)",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                    TweenHelper.StaggerMoveTo(transforms, Vector3.up * 2f, staggerDelay, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Stagger Move (Center)",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                    TweenHelper.StaggerMoveTo(transforms, Vector3.zero, staggerDelay, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Stagger Scale (2x)",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                    TweenHelper.StaggerScaleTo(transforms, 2f, staggerDelay, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Stagger Scale (0.5x)",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                    TweenHelper.StaggerScaleTo(transforms, 0.5f, staggerDelay, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Stagger Preset (PopIn)",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) t.localScale = Vector3.zero;

                    var gameObjects = transforms.Where(t => t != null).Select(t => t.gameObject).ToList();
                    TweenHelper.StaggerPreset("PopIn", gameObjects, staggerDelay, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Stagger Preset (Bounce)",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    var gameObjects = transforms.Where(t => t != null).Select(t => t.gameObject).ToList();
                    TweenHelper.StaggerPreset("Bounce", gameObjects, staggerDelay, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Wave Effect",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    var seq = DOTween.Sequence();
                    for (int i = 0; i < transforms.Length; i++)
                    {
                        var t = transforms[i];
                        if (t == null) continue;

                        var originalY = t.position.y;
                        var moveTween = t.DOMoveY(originalY + 1.5f, duration * 0.5f)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(2, LoopType.Yoyo);
                        seq.Insert(i * staggerDelay, moveTween);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Cascade Effect",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    for (int i = 0; i < transforms.Length; i++)
                    {
                        var t = transforms[i];
                        if (t == null) continue;

                        var delay = i * staggerDelay;
                        var originalScale = t.localScale;

                        DOVirtual.DelayedCall(delay, () =>
                        {
                            switch (i % 3)
                            {
                                case 0:
                                    TweenHelper.ScaleTo(t, originalScale * 1.5f, duration * 0.5f);
                                    TweenHelper.RotateBy(t, Vector3.up * 180f, duration);
                                    break;
                                case 1:
                                    TweenHelper.MoveTo(t, t.position + Vector3.up * 2f, duration * 0.5f);
                                    break;
                                case 2:
                                    TweenHelper.PlayPreset("Bounce", t, duration);
                                    break;
                            }
                        });
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Custom Stagger",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    TweenStagger.StaggerCustom(transforms, (t, index) =>
                    {
                        var originalPos = t.position;
                        var originalScale = t.localScale;

                        return TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, originalPos + Vector3.up * (index + 1), duration * 0.5f)
                            .JoinScale(t, originalScale * (1f + index * 0.2f), duration * 0.5f)
                            .Delay(0.2f)
                            .Move(t, originalPos, duration * 0.5f)
                            .JoinScale(t, originalScale, duration * 0.5f)
                            .Build();
                    }, staggerDelay);
                }
            };
        }
    }
}
