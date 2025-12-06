using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides movement animations demonstrating MoveTo, MoveBy, and MoveToLocal functions.
    /// </summary>
    public class MoveAnimationDemo : MonoBehaviour, IDemoAnimationProvider
    {
        public string CategoryName => "Movement";

        public void Initialize(GameObject[] objects) { }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            // ═══════════════════════════════════════════════════════════════
            // BASIC MOVETO
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "MoveTo (Up)",
                Category = CategoryName,
                SubCategory = "Basic MoveTo",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.up * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Right)",
                Category = CategoryName,
                SubCategory = "Basic MoveTo",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.right * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Forward)",
                Category = CategoryName,
                SubCategory = "Basic MoveTo",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.forward * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Origin)",
                Category = CategoryName,
                SubCategory = "Basic MoveTo",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], Vector3.zero, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Diagonal)",
                Category = CategoryName,
                SubCategory = "Basic MoveTo",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + new Vector3(1f, 1f, 1f), duration)
            };

            // ═══════════════════════════════════════════════════════════════
            // MOVEBY (RELATIVE)
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "MoveBy (Offset)",
                Category = CategoryName,
                SubCategory = "MoveBy",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveBy(transforms[0], new Vector3(1f, 1f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveBy (Circle Step)",
                Category = CategoryName,
                SubCategory = "MoveBy",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveBy(transforms[0], new Vector3(Mathf.Cos(Time.time) * 2f, 0f, Mathf.Sin(Time.time) * 2f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveBy (Zigzag)",
                Category = CategoryName,
                SubCategory = "MoveBy",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.right * 1.5f, duration * 0.25f)
                        .MoveBy(t, new Vector3(-1.5f, 1f, 0f), duration * 0.25f)
                        .MoveBy(t, Vector3.right * 1.5f, duration * 0.25f)
                        .MoveBy(t, new Vector3(-1.5f, 1f, 0f), duration * 0.25f)
                        .Play();
                }
            };

            // ═══════════════════════════════════════════════════════════════
            // MOVETOLOCAL
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "MoveToLocal (Center)",
                Category = CategoryName,
                SubCategory = "MoveToLocal",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveToLocal(transforms[0], Vector3.zero, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveToLocal (Above)",
                Category = CategoryName,
                SubCategory = "MoveToLocal",
                Execute = (transforms, duration) =>
                    TweenHelper.MoveToLocal(transforms[0], new Vector3(0f, 2f, 0f), duration)
            };

            // ═══════════════════════════════════════════════════════════════
            // LOOPING PATTERNS (Using Sequences)
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Bounce (Yoyo)",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.up * 2f, duration, TweenOptions.WithEase(Ease.InOutSine))
                        .Play(TweenOptions.WithLoops(-1, LoopType.Yoyo));
                }
            };

            yield return new DemoAnimation
            {
                Name = "Square Loop",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    float step = duration / 4f;
                    var ease = TweenOptions.WithEase(Ease.InOutSine);
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.right * 2f, step, ease)
                        .MoveBy(t, Vector3.up * 2f, step, ease)
                        .MoveBy(t, Vector3.left * 2f, step, ease)
                        .MoveBy(t, Vector3.down * 2f, step, ease)
                        .Play(TweenOptions.WithLoops(-1, LoopType.Restart));
                }
            };

            yield return new DemoAnimation
            {
                Name = "Triangle Loop",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    float step = duration / 3f;
                    var ease = TweenOptions.WithEase(Ease.InOutSine);
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, new Vector3(1f, 2f, 0f), step, ease)
                        .MoveBy(t, new Vector3(1f, -2f, 0f), step, ease)
                        .MoveBy(t, Vector3.left * 2f, step, ease)
                        .Play(TweenOptions.WithLoops(-1, LoopType.Restart));
                }
            };

            yield return new DemoAnimation
            {
                Name = "Pendulum (Yoyo)",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.right * 2f, duration, TweenOptions.WithEase(Ease.InOutSine))
                        .Play(TweenOptions.WithLoops(-1, LoopType.Yoyo));
                }
            };

            // ═══════════════════════════════════════════════════════════════
            // ADVANCED / COMBINED (Using Sequences)
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Figure-8",
                Category = CategoryName,
                SubCategory = "Advanced",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, new Vector3(1f, 1f, 0f), duration)
                        .MoveBy(t, new Vector3(1f, -1f, 0f), duration)
                        .MoveBy(t, new Vector3(-1f, -1f, 0f), duration)
                        .MoveBy(t, new Vector3(-1f, 1f, 0f), duration)
                        .MoveBy(t, new Vector3(-1f, -1f, 0f), duration)
                        .MoveBy(t, new Vector3(-1f, 1f, 0f), duration)
                        .MoveBy(t, new Vector3(1f, 1f, 0f), duration)
                        .MoveBy(t, new Vector3(1f, -1f, 0f), duration)
                        .Play();
                }
            };

            yield return new DemoAnimation
            {
                Name = "Staircase",
                Category = CategoryName,
                SubCategory = "Advanced",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.right * 0.5f, duration)
                        .MoveBy(t, Vector3.up * 0.5f, duration)
                        .MoveBy(t, Vector3.right * 0.5f, duration)
                        .MoveBy(t, Vector3.up * 0.5f, duration)
                        .MoveBy(t, Vector3.right * 0.5f, duration)
                        .MoveBy(t, Vector3.up * 0.5f, duration)
                        .Play();
                }
            };

            yield return new DemoAnimation
            {
                Name = "Diamond",
                Category = CategoryName,
                SubCategory = "Advanced",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, new Vector3(1.5f, 1.5f, 0f), duration)
                        .MoveBy(t, new Vector3(1.5f, -1.5f, 0f), duration)
                        .MoveBy(t, new Vector3(-1.5f, -1.5f, 0f), duration)
                        .MoveBy(t, new Vector3(-1.5f, 1.5f, 0f), duration)
                        .Play();
                }
            };

            yield return new DemoAnimation
            {
                Name = "Spiral Out",
                Category = CategoryName,
                SubCategory = "Advanced",
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.right * 0.5f, duration)
                        .MoveBy(t, Vector3.up * 0.5f, duration)
                        .MoveBy(t, Vector3.left * 1f, duration)
                        .MoveBy(t, Vector3.down * 1f, duration)
                        .MoveBy(t, Vector3.right * 1.5f, duration)
                        .MoveBy(t, Vector3.up * 1.5f, duration)
                        .MoveBy(t, Vector3.left * 2f, duration)
                        .MoveBy(t, Vector3.down * 2f, duration)
                        .Play();
                }
            };
        }
    }
}
