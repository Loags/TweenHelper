using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Provides async/await demos with TweenHelper.
    /// </summary>
    public class AsyncDemo : MonoBehaviour, IDemoAnimationProvider
    {
        [Header("Settings")]
        [SerializeField] private float timeoutDuration = 1f;

        private GameObject[] demoObjects;
        private CancellationTokenSource cancellationTokenSource;

        public string CategoryName => "Async";

        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
        }

        public IEnumerable<DemoAnimation> GetAnimations()
        {
            yield return new DemoAnimation
            {
                Name = "Await Completion",
                Category = CategoryName,
                Execute = (transforms, duration) => _ = DemoAwaitCompletion(transforms, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Await Timeout",
                Category = CategoryName,
                Execute = (transforms, duration) => _ = DemoAwaitTimeout(transforms, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Await All",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) => _ = DemoAwaitAll(transforms, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Await Any",
                Category = CategoryName,
                RequiresMultipleObjects = true,
                Execute = (transforms, duration) => _ = DemoAwaitAny(transforms, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Cancellation",
                Category = CategoryName,
                Execute = (transforms, duration) => _ = DemoCancellation(transforms, duration)
            };

            yield return new DemoAnimation
            {
                Name = "Async Sequence",
                Category = CategoryName,
                Execute = (transforms, duration) => _ = DemoAsyncSequence(transforms, duration)
            };
        }

        private async Task DemoAwaitCompletion(Transform[] transforms, float duration)
        {
            if (transforms.Length == 0) return;
            var t = transforms[0];
            if (t == null) return;

            var originalPos = t.position;
            Debug.Log($"Await Completion: Starting on {t.name}");

            var tween = TweenHelper.MoveTo(t, originalPos + Vector3.up * 3f, duration);
            await TweenHelper.AwaitCompletion(tween);

            Debug.Log("Await Completion: Animation completed!");

            var returnTween = TweenHelper.MoveTo(t, originalPos, duration * 0.5f);
            await TweenHelper.AwaitCompletion(returnTween);

            Debug.Log("Await Completion: Return completed!");
        }

        private async Task DemoAwaitTimeout(Transform[] transforms, float duration)
        {
            if (transforms.Length == 0) return;
            var t = transforms[0];
            if (t == null) return;

            var originalPos = t.position;
            Debug.Log($"Await Timeout: Starting animation (will timeout)");

            var tween = TweenHelper.MoveTo(t, originalPos + Vector3.right * 4f, duration * 2f);
            bool completed = await TweenHelper.AwaitCompletionWithTimeout(tween, timeoutDuration);

            if (completed)
                Debug.Log("Await Timeout: Completed within timeout");
            else
                Debug.Log("Await Timeout: Timed out as expected!");

            t.position = originalPos;
        }

        private async Task DemoAwaitAll(Transform[] transforms, float duration)
        {
            var tweens = new List<Tween>();

            for (int i = 0; i < Mathf.Min(3, transforms.Length); i++)
            {
                var t = transforms[i];
                if (t == null) continue;

                var offset = new Vector3(i * 2f, 2f, 0f);
                tweens.Add(TweenHelper.MoveTo(t, t.position + offset, duration));
            }

            if (tweens.Count == 0) return;

            Debug.Log($"Await All: Waiting for {tweens.Count} animations...");
            await TweenHelper.AwaitAll(tweens.ToArray());
            Debug.Log("Await All: All animations completed!");
        }

        private async Task DemoAwaitAny(Transform[] transforms, float duration)
        {
            var tweens = new List<Tween>();

            for (int i = 0; i < Mathf.Min(3, transforms.Length); i++)
            {
                var t = transforms[i];
                if (t == null) continue;

                var d = duration * (1f + i * 0.5f);
                tweens.Add(TweenHelper.MoveTo(t, t.position + Vector3.up * 2f, d));
            }

            if (tweens.Count == 0) return;

            Debug.Log($"Await Any: Waiting for first of {tweens.Count} animations...");
            await TweenHelper.AwaitAny(tweens.ToArray());
            Debug.Log("Await Any: First animation completed!");

            foreach (var tween in tweens)
                tween?.Kill();
        }

        private async Task DemoCancellation(Transform[] transforms, float duration)
        {
            if (transforms.Length == 0) return;
            var t = transforms[0];
            if (t == null) return;

            var originalPos = t.position;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            Debug.Log("Cancellation: Starting animation (will cancel in 1s)");

            var tween = TweenHelper.MoveTo(t, originalPos + Vector3.forward * 4f, duration * 2f);

            _ = Task.Delay(1000).ContinueWith(_ =>
            {
                Debug.Log("Cancellation: Cancelling...");
                cancellationTokenSource?.Cancel();
            });

            try
            {
                await TweenHelper.AwaitCompletion(tween, cancellationTokenSource.Token);
                Debug.Log("Cancellation: Completed (unexpected)");
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("Cancellation: Successfully cancelled!");
            }

            t.position = originalPos;
        }

        private async Task DemoAsyncSequence(Transform[] transforms, float duration)
        {
            if (transforms.Length == 0) return;
            var t = transforms[0];
            if (t == null) return;

            var originalPos = t.position;
            var originalScale = t.localScale;

            Debug.Log("Async Sequence: Step 1 - Move up");
            await TweenHelper.AwaitCompletion(TweenHelper.MoveTo(t, originalPos + Vector3.up * 3f, duration * 0.3f));

            Debug.Log("Async Sequence: Step 2 - Scale up");
            await TweenHelper.AwaitCompletion(TweenHelper.ScaleTo(t, 2f, duration * 0.2f));

            Debug.Log("Async Sequence: Step 3 - Rotate");
            await TweenHelper.AwaitCompletion(TweenHelper.RotateBy(t, Vector3.up * 360f, duration * 0.3f));

            Debug.Log("Async Sequence: Step 4 - Return");
            await TweenHelper.AwaitAll(new[]
            {
                TweenHelper.MoveTo(t, originalPos, duration * 0.2f),
                TweenHelper.ScaleTo(t, originalScale, duration * 0.2f)
            });

            Debug.Log("Async Sequence: Complete!");
        }

        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
    }
}
