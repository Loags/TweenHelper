using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides control surface demos: pause, resume, kill, complete, rewind operations.
    /// </summary>
    public class ControlDemo : MonoBehaviour, IDemoAnimationProvider
    {
        private GameObject[] demoObjects;

        public string CategoryName => "Control";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "Start Looping Animations",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) => StartLongAnimations(transforms, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Pause All",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.Pause(t.gameObject);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Resume All",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.Resume(t.gameObject);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Kill All",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.Kill(t.gameObject);
                }
            };

            yield return new DemoAnimation
            {
                Name = "Complete All",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    foreach (var t in transforms)
                        if (t != null) TweenHelper.Complete(t.gameObject);
                }
            };

            yield return new DemoAnimation
            {
                Name = "ID Control Demo",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    if (transforms.Length == 0) return;
                    var t = transforms[0];
                    if (t == null) return;

                    var originalPos = t.position;
                    var options = TweenOptions.WithId("controllable-group");

                    TweenHelper.MoveTo(t, originalPos + Vector3.right * 3f, 3f, options);
                    TweenHelper.RotateBy(t, Vector3.up * 360f, 3f, options);
                    TweenHelper.ScaleTo(t, 1.5f, 3f, options);

                    Debug.Log("Started animations with ID 'controllable-group'");
                    StartCoroutine(DemoIdControlSequence());
                }
            };

            yield return new DemoAnimation
            {
                Name = "Show Diagnostics",
                Category = CategoryName,
                Execute = (transforms, duration) =>
                {
                    int totalActive = TweenHelper.GetTotalActiveTweenCount();
                    Debug.Log($"Total active tweens: {totalActive}");

                    foreach (var t in transforms)
                    {
                        if (t == null) continue;
                        int count = TweenHelper.GetActiveTweenCount(t.gameObject);
                        if (count > 0)
                            Debug.Log($"{t.name}: {count} active tweens");
                    }
                }
            };
        }

        private void StartLongAnimations(Transform[] transforms, float duration)
        {
            var animationIds = new[] { "group-a", "group-b", "group-c" };

            for (int i = 0; i < transforms.Length; i++)
            {
                var t = transforms[i];
                if (t == null) continue;

                var originalPos = t.position;
                var originalScale = t.localScale;
                var animId = animationIds[i % animationIds.Length];
                var options = TweenOptions.WithId(animId);

                switch (i % 4)
                {
                    case 0:
                        TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, originalPos + Vector3.right * 3f, duration * 0.25f)
                            .Move(t, originalPos + Vector3.right * 3f + Vector3.up * 2f, duration * 0.25f)
                            .Move(t, originalPos + Vector3.up * 2f, duration * 0.25f)
                            .Move(t, originalPos, duration * 0.25f)
                            .Build(options.SetLoops(-1));
                        break;

                    case 1:
                        TweenHelper.RotateBy(t, Vector3.up * 360f, duration, options.SetLoops(-1));
                        break;

                    case 2:
                        TweenHelper.ScaleTo(t, originalScale * 1.5f, duration * 0.5f,
                            options.SetLoops(-1, LoopType.Yoyo));
                        break;

                    case 3:
                        TweenHelper.CreateSequence(t.gameObject)
                            .Move(t, originalPos + Vector3.up * 3f, duration * 0.3f)
                            .JoinScale(t, originalScale * 2f, duration * 0.3f)
                            .Delay(duration * 0.2f)
                            .Move(t, originalPos, duration * 0.25f)
                            .JoinScale(t, originalScale, duration * 0.25f)
                            .Build(options.SetLoops(-1));
                        break;
                }
            }

            Debug.Log("Started looping animations");
        }

        private System.Collections.IEnumerator DemoIdControlSequence()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Pausing by ID: controllable-group");
            TweenHelper.PauseById("controllable-group");

            yield return new WaitForSeconds(1f);
            Debug.Log("Resuming by ID: controllable-group");
            TweenHelper.ResumeById("controllable-group");

            yield return new WaitForSeconds(1f);
            Debug.Log("Killing by ID: controllable-group");
            TweenHelper.KillById("controllable-group");
        }
    }
}
