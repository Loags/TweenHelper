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
            // LOOPING PATTERNS (Using TweenAnimations - ready-to-use!)
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Bounce",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                    TweenAnimations.Bounce(transforms[0], 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Pendulum",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                    TweenAnimations.Pendulum(transforms[0], 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Square Loop",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                    TweenAnimations.SquareLoop(transforms[0], 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Triangle Loop",
                Category = CategoryName,
                SubCategory = "Looping",
                Execute = (transforms, duration) =>
                    TweenAnimations.TriangleLoop(transforms[0], 2f, duration)
            };

            // ═══════════════════════════════════════════════════════════════
            // PATH PATTERNS (Using TweenAnimations - ready-to-use!)
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Zigzag",
                Category = CategoryName,
                SubCategory = "Paths",
                Execute = (transforms, duration) =>
                    TweenAnimations.Zigzag(transforms[0], 1.5f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Figure-8",
                Category = CategoryName,
                SubCategory = "Paths",
                Execute = (transforms, duration) =>
                    TweenAnimations.Figure8(transforms[0], 1f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Diamond",
                Category = CategoryName,
                SubCategory = "Paths",
                Execute = (transforms, duration) =>
                    TweenAnimations.Diamond(transforms[0], 1.5f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Staircase",
                Category = CategoryName,
                SubCategory = "Paths",
                Execute = (transforms, duration) =>
                    TweenAnimations.Staircase(transforms[0], 0.5f, 3, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Spiral Out",
                Category = CategoryName,
                SubCategory = "Paths",
                Execute = (transforms, duration) =>
                    TweenAnimations.SpiralOut(transforms[0], duration)
            };

            // ═══════════════════════════════════════════════════════════════
            // EFFECTS (Using TweenAnimations - ready-to-use!)
            // ═══════════════════════════════════════════════════════════════

            yield return new DemoAnimation
            {
                Name = "Pop In",
                Category = CategoryName,
                SubCategory = "Effects",
                Execute = (transforms, duration) =>
                    TweenAnimations.PopIn(transforms[0], duration)
            };

            yield return new DemoAnimation
            {
                Name = "Pop Out",
                Category = CategoryName,
                SubCategory = "Effects",
                Execute = (transforms, duration) =>
                    TweenAnimations.PopOut(transforms[0], duration)
            };

            yield return new DemoAnimation
            {
                Name = "Shake",
                Category = CategoryName,
                SubCategory = "Effects",
                Execute = (transforms, duration) =>
                    TweenAnimations.Shake(transforms[0], 0.5f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Pulse",
                Category = CategoryName,
                SubCategory = "Effects",
                Execute = (transforms, duration) =>
                    TweenAnimations.Pulse(transforms[0], 1.2f, duration)
            };
        }
    }
}
