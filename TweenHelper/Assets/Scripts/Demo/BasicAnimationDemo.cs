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
                    TweenHelper.MoveBy(transforms[0], new Vector3(1f, 1f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "MoveToLocal",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.MoveToLocal(transforms[0], new Vector3(0f, 2f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "RotateTo (180° Y)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.RotateTo(transforms[0], new Vector3(0f, 180f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "RotateTo (360° Y)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.RotateTo(transforms[0], new Vector3(0f, 360f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "RotateBy (45° X)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.RotateBy(transforms[0], new Vector3(45f, 0f, 0f), duration)
            };

            yield return new DemoAnimation
            {
                Name = "LookAt (Origin)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.LookAt(transforms[0], Vector3.zero, duration)
            };

            yield return new DemoAnimation
            {
                Name = "ScaleTo (2x)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.ScaleTo(transforms[0], 2f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "ScaleTo (0.5x)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.ScaleTo(transforms[0], 0.5f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "ScaleBy (1.5x)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                    TweenHelper.ScaleBy(transforms[0], 1.5f, duration)
            };

            yield return new DemoAnimation
            {
                Name = "FadeOut",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var cg = transforms[0].GetComponent<CanvasGroup>();
                    if (cg != null) TweenHelper.FadeOut(cg, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "FadeIn",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var cg = transforms[0].GetComponent<CanvasGroup>();
                    if (cg != null) TweenHelper.FadeIn(cg, duration);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Combined (Move+Scale+Rotate)",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    var t = transforms[0];
                    TweenHelper.MoveTo(t, t.position + Vector3.up * 3f, duration);
                    TweenHelper.ScaleTo(t, 1.5f, duration);
                    TweenHelper.RotateBy(t, new Vector3(0f, 360f, 0f), duration);
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
            TweenHelper.MoveTo(transforms[0], transforms[0].position + offset, duration);
        }

        private void ExecuteEasingComparison(Transform[] transforms, float duration)
        {
            var t = transforms[0];
            var targetPos = t.position + Vector3.right * 5f;
            var options = TweenOptions.WithEase(Ease.OutBounce);
            TweenHelper.MoveTo(t, targetPos, duration, options);
        }
    }
}
