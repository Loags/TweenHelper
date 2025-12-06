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
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.up * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Right)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.right * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Forward)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.forward * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Origin)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], Vector3.zero, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Diagonal)",
                Category = CategoryName,
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
                Execute = (transforms, duration) =>
                    TweenHelper.MoveBy(transforms[0], new Vector3(1f, 1f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveBy (Circle Step)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveBy(transforms[0], new Vector3(Mathf.Cos(Time.time) * 2f, 0f, Mathf.Sin(Time.time) * 2f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveBy (Zigzag)",
                Category = CategoryName,
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
                Execute = (transforms, duration) =>
                    TweenHelper.MoveToLocal(transforms[0], Vector3.zero, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveToLocal (Above)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveToLocal(transforms[0], new Vector3(0f, 2f, 0f), duration)
            };

            // ═══════════════════════════════════════════════════════════════
            // TIMING VARIATIONS
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "With Delay (0.5s)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.up * 2f, duration,
                        TweenOptions.WithDelay(0.5f))
            };

            yield return new DemoAnimation
            {
                Name = "Speed Based",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.right * 5f, 2f,
                        TweenOptions.WithSpeedBased())
            };

            // ═══════════════════════════════════════════════════════════════
            // LOOPING PATTERNS
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Loop: Yoyo (x3)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.up * 2f, duration,
                        TweenOptions.WithLoops(3, LoopType.Yoyo))
            };

            yield return new DemoAnimation
            {
                Name = "Loop: Restart (x3)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.right * 2f, duration,
                        TweenOptions.WithLoops(3, LoopType.Restart))
            };

            yield return new DemoAnimation
            {
                Name = "Loop: Infinite Yoyo",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.up * 2f, duration,
                        TweenOptions.WithLoops(-1, LoopType.Yoyo))
            };

            // ═══════════════════════════════════════════════════════════════
            // ADVANCED / COMBINED
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Snapping Movement",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + new Vector3(2.7f, 1.3f, 0f), duration,
                        TweenOptions.WithSnapping())
            };

            yield return new DemoAnimation
            {
                Name = "Sequential Path",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.CreateSequence(t)
                        .MoveBy(t, Vector3.right * 2f, duration * 0.25f)
                        .MoveBy(t, Vector3.up * 2f, duration * 0.25f)
                        .MoveBy(t, Vector3.left * 2f, duration * 0.25f)
                        .MoveBy(t, Vector3.down * 2f, duration * 0.25f)
                        .Play();
                }
            };

            yield return new DemoAnimation
            {
                Name = "Combined Options",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveTo(transforms[0], transforms[0].position + Vector3.up * 3f, duration,
                        TweenOptions.WithDelay(0.3f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo))
            };
        }
    }
}
