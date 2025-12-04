using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides basic TweenHelper animations: Move, Rotate, Scale, and Fade.
    /// </summary>
    public class BasicAnimationDemo : MonoBehaviour, IDemoAnimationProvider
    {
        private GameObject[] demoObjects;

        public string CategoryName => "Basic";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "MoveTo (Up)",
                Category = CategoryName,
                Execute = (transforms, duration) => ExecuteMove(transforms, Vector3.up * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Right)",
                Category = CategoryName,
                Execute = (transforms, duration) => ExecuteMove(transforms, Vector3.right * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveTo (Forward)",
                Category = CategoryName,
                Execute = (transforms, duration) => ExecuteMove(transforms, Vector3.forward * 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveBy (Offset)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveBy(t, new Vector3(1f, 1f, 0f), duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "MoveToLocal",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.MoveToLocal(t, new Vector3(0f, 2f, 0f), duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "RotateTo (180° Y)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.RotateTo(t, new Vector3(0f, 180f, 0f), duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "RotateTo (360° Y)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.RotateTo(t, new Vector3(0f, 360f, 0f), duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "RotateBy (45° X)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.RotateBy(t, new Vector3(45f, 0f, 0f), duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "LookAt (Origin)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.LookAt(t, Vector3.zero, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "ScaleTo (2x)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.ScaleTo(t, 2f, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "ScaleTo (0.5x)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.ScaleTo(t, 0.5f, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "ScaleBy (1.5x)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.ScaleBy(t, 1.5f, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "FadeOut",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        var cg = t.GetComponent<CanvasGroup>();
                        if (cg != null) TweenHelper.FadeOut(cg, duration);
                    }
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
                        if (cg != null) TweenHelper.FadeIn(cg, duration);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Combined (Move+Scale+Rotate)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        TweenHelper.MoveTo(t, t.position + Vector3.up * 3f, duration);
                        TweenHelper.ScaleTo(t, 1.5f, duration);
                        TweenHelper.RotateBy(t, new Vector3(0f, 360f, 0f), duration);
                    }
                }
            };

            yield return new DemoAnimation
            {
                Name = "Easing Comparison",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) => ExecuteEasingComparison(transforms, duration)
            };
        }

        private void ExecuteMove(Transform[] transforms, Vector3 offset, float duration)
        {
            foreach (var t in transforms)
            {
                if (t != null)
                    TweenHelper.MoveTo(t, t.position + offset, duration);
            }
        }

        private void ExecuteEasingComparison(Transform[] transforms, float duration)
        {
            var eases = new[] { Ease.Linear, Ease.OutQuart, Ease.OutBounce, Ease.OutElastic };

            for (int i = 0; i < Mathf.Min(transforms.Length, eases.Length); i++)
            {
                var t = transforms[i];
                if (t == null) continue;

                var targetPos = t.position + Vector3.right * 5f;
                var options = TweenOptions.WithEase(eases[i]);
                TweenHelper.MoveTo(t, targetPos, duration, options);
            }
        }
    }
}
