using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides sequence composition demos using TweenSequenceBuilder.
    /// </summary>
    public class SequenceDemo : MonoBehaviour, IDemoAnimationProvider
    {
        private GameObject[] demoObjects;

        public string CategoryName => "Sequences";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "Simple Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        var originalPos = t.position;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, originalPos + Vector3.up * 2f, duration * 0.33f)
                            .Scale(t, 1.5f, duration * 0.33f)
                            .Rotate(t, new Vector3(0f, 180f, 0f), duration * 0.33f)
                            .Play();
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Parallel Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, t.position + Vector3.up * 2f, duration)
                            .JoinScale(t, new Vector3(1.5f, 1.5f, 1.5f), duration)
                            .JoinRotate(t, new Vector3(0f, 360f, 0f), duration)
                            .Play();
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Complex Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        var startPos = t.position;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Scale(t, 0.5f, duration * 0.2f)
                            .Move(t, startPos + Vector3.up * 3f, duration * 0.3f)
                            .JoinRotate(t, new Vector3(0f, 360f, 0f), duration * 0.3f)
                            .Scale(t, 1.5f, duration * 0.2f)
                            .Delay(duration * 0.1f)
                            .Move(t, startPos, duration * 0.2f)
                            .Play();
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Looped Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        var originalPos = t.position;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, originalPos + Vector3.right * 2f, duration * 0.5f)
                            .Move(t, originalPos, duration * 0.5f)
                            .Build(TweenOptions.WithLoops(3, LoopType.Restart));
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Yoyo Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, t.position + Vector3.up * 2f, duration * 0.5f)
                            .Scale(t, 1.5f, duration * 0.5f)
                            .Build(TweenOptions.WithLoops(2, LoopType.Yoyo));
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Callback Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Call(() => Debug.Log($"Starting {t.name}..."))
                            .Move(t, t.position + Vector3.up * 2f, duration * 0.4f)
                            .Call(() => Debug.Log($"Move complete on {t.name}!"))
                            .Scale(t, 1.5f, duration * 0.4f)
                            .Call(() => Debug.Log($"Animation finished on {t.name}!"))
                            .Play();
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Preset Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        t.localScale = Vector3.zero;

                        TweenHelper.CreateSequence(t.gameObject)
                            .Preset("PopIn", t.gameObject, duration * 0.4f)
                            .Delay(duration * 0.2f)
                            .Preset("Bounce", t.gameObject, duration * 0.4f)
                            .Play();
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Multi-Object Swap",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) =>
                {
                    if (transforms.Length < 2) return;
                    var t1 = transforms[0];
                    var t2 = transforms[1];
                    if (t1 == null || t2 == null) return;

                    var pos1 = t1.position;
                    var pos2 = t2.position;

                    TweenHelper.CreateSequence()
                        .Move(t1, pos2, duration * 0.5f)
                        .JoinMove(t2, pos1, duration * 0.5f)
                        .Delay(0.2f)
                        .Move(t1, pos1, duration * 0.5f)
                        .JoinMove(t2, pos2, duration * 0.5f)
                        .Play();
                }
            };
        }
    }
}
