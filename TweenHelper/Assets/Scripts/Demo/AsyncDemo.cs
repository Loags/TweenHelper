using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates async/await functionality with TweenHelper.
    /// Shows awaiting completion, timeouts, cancellation, and async patterns.
    /// </summary>
    public class AsyncDemo : MonoBehaviour
    {
        [Header("Async Controls")]
        [SerializeField] private Button awaitCompletionButton;
        [SerializeField] private Button awaitTimeoutButton;
        [SerializeField] private Button awaitAllButton;
        [SerializeField] private Button awaitAnyButton;
        [SerializeField] private Button cancellationButton;
        [SerializeField] private Button asyncSequenceButton;
        [SerializeField] private Button directAwaitButton;
        
        [Header("Async Settings")]
        [SerializeField] private float animationDuration = 2f;
        [SerializeField] private float timeoutDuration = 1f;
        [SerializeField] private bool showAsyncLogs = true;
        
        private GameObject[] demoObjects;
        private int currentObjectIndex = 0;
        private CancellationTokenSource cancellationTokenSource;
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            SetupButton(awaitCompletionButton, "Await Completion", () => _ = DemoAwaitCompletion());
            SetupButton(awaitTimeoutButton, "Await Timeout", () => _ = DemoAwaitTimeout());
            SetupButton(awaitAllButton, "Await All", () => _ = DemoAwaitAll());
            SetupButton(awaitAnyButton, "Await Any", () => _ = DemoAwaitAny());
            SetupButton(cancellationButton, "Cancellation", () => _ = DemoCancellation());
            SetupButton(asyncSequenceButton, "Async Sequence", () => _ = DemoAsyncSequence());
            SetupButton(directAwaitButton, "Direct Await", () => _ = DemoDirectAwait());
        }
        
        private void SetupButton(Button button, string text, System.Action action)
        {
            if (button == null) return;
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
            
            var buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }
        
        public async Task DemoAwaitCompletion()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Starting await completion demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.up * 3f;
            
            // Start the animation
            var tween = TweenHelper.MoveTo(target.transform, targetPos, animationDuration);
            
            Log("Animation started, now awaiting completion...");
            
            try
            {
                // Await the completion
                await TweenHelper.AwaitCompletion(tween);
                
                Log("Animation completed successfully!");
                
                // Return to original position
                Log("Moving back to original position...");
                var returnTween = TweenHelper.MoveTo(target.transform, originalPos, animationDuration * 0.5f);
                await TweenHelper.AwaitCompletion(returnTween);
                
                Log("Return animation completed!");
            }
            catch (System.Exception ex)
            {
                Log($"Animation failed or was cancelled: {ex.Message}");
            }
        }
        
        public async Task DemoAwaitTimeout()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Starting await timeout demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.right * 4f;
            
            // Start a longer animation
            var tween = TweenHelper.MoveTo(target.transform, targetPos, animationDuration * 2f);
            
            Log($"Animation started (duration: {animationDuration * 2f}s), awaiting with timeout: {timeoutDuration}s");
            
            try
            {
                // Await with timeout (should timeout before completion)
                bool completed = await TweenHelper.AwaitCompletionWithTimeout(tween, timeoutDuration);
                
                if (completed)
                {
                    Log("Animation completed within timeout!");
                }
                else
                {
                    Log("Animation timed out as expected!");
                }
            }
            catch (System.Exception ex)
            {
                Log($"Await timeout failed: {ex.Message}");
            }
            
            // Reset position
            target.transform.position = originalPos;
        }
        
        public async Task DemoAwaitAll()
        {
            if (demoObjects == null || demoObjects.Length < 2) return;
            
            Log("Starting await all demo");
            
            var tweens = new System.Collections.Generic.List<DG.Tweening.Tween>();
            
            // Start multiple animations
            for (int i = 0; i < Mathf.Min(3, demoObjects.Length); i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                var originalPos = obj.transform.position;
                var offset = new Vector3(i * 2f, 2f, 0f);
                var tween = TweenHelper.MoveTo(obj.transform, originalPos + offset, animationDuration);
                
                tweens.Add(tween);
                Log($"Started animation {i + 1} on {obj.name}");
            }
            
            if (tweens.Count == 0) return;
            
            Log($"Awaiting completion of all {tweens.Count} animations...");
            
            try
            {
                // Await all animations
                await TweenHelper.AwaitAll(tweens.ToArray());
                
                Log("All animations completed successfully!");
                
                // Return all to original positions
                var returnTweens = new System.Collections.Generic.List<DG.Tweening.Tween>();
                for (int i = 0; i < tweens.Count && i < demoObjects.Length; i++)
                {
                    var obj = demoObjects[i];
                    if (obj == null) continue;
                    
                    var originalPos = obj.transform.position - new Vector3(i * 2f, 2f, 0f);
                    var returnTween = TweenHelper.MoveTo(obj.transform, originalPos, animationDuration * 0.5f);
                    returnTweens.Add(returnTween);
                }
                
                await TweenHelper.AwaitAll(returnTweens.ToArray());
                Log("All return animations completed!");
            }
            catch (System.Exception ex)
            {
                Log($"Await all failed: {ex.Message}");
            }
        }
        
        public async Task DemoAwaitAny()
        {
            if (demoObjects == null || demoObjects.Length < 2) return;
            
            Log("Starting await any demo");
            
            var tweens = new System.Collections.Generic.List<DG.Tweening.Tween>();
            
            // Start multiple animations with different durations
            for (int i = 0; i < Mathf.Min(3, demoObjects.Length); i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                var originalPos = obj.transform.position;
                var offset = Vector3.up * (i + 1) * 2f;
                // Different durations - first one should complete first
                var duration = animationDuration * (1f + i * 0.5f);
                var tween = TweenHelper.MoveTo(obj.transform, originalPos + offset, duration);
                
                tweens.Add(tween);
                Log($"Started animation {i + 1} on {obj.name} (duration: {duration:F1}s)");
            }
            
            if (tweens.Count == 0) return;
            
            Log($"Awaiting completion of ANY of {tweens.Count} animations...");
            
            try
            {
                // Await any animation (should complete when first one finishes)
                await TweenHelper.AwaitAny(tweens.ToArray());
                
                Log("First animation completed! Stopping others...");
                
                // Kill remaining animations
                foreach (var tween in tweens)
                {
                    if (tween != null && tween.IsActive())
                    {
                        tween.Kill();
                    }
                }
                
                // Reset positions
                for (int i = 0; i < Mathf.Min(3, demoObjects.Length); i++)
                {
                    var obj = demoObjects[i];
                    if (obj == null) continue;
                    
                    var originalPos = obj.transform.position - Vector3.up * (i + 1) * 2f;
                    _ = TweenHelper.MoveTo(obj.transform, originalPos, 0.5f);
                }
            }
            catch (System.Exception ex)
            {
                Log($"Await any failed: {ex.Message}");
            }
        }
        
        public async Task DemoCancellation()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Starting cancellation demo on {target.name}");
            
            // Create cancellation token
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.forward * 4f;
            
            // Start animation
            var tween = TweenHelper.MoveTo(target.transform, targetPos, animationDuration * 2f);
            
            Log("Animation started, will cancel in 1 second...");
            
            try
            {
                // Schedule cancellation
#pragma warning disable CS4014 // Because this call is not awaited, execution continues before call is completed
                _ = Task.Delay(1000, cancellationTokenSource.Token).ContinueWith(t =>
                {
                    if (!t.IsCanceled)
                    {
                        Log("Cancelling animation...");
                        cancellationTokenSource.Cancel();
                    }
                });
#pragma warning restore CS4014
                
                // Await with cancellation token
                await TweenHelper.AwaitCompletion(tween, cancellationTokenSource.Token);
                
                Log("Animation completed (this shouldn't happen!)");
            }
            catch (System.OperationCanceledException)
            {
                Log("Animation was successfully cancelled!");
            }
            catch (System.Exception ex)
            {
                Log($"Cancellation demo failed: {ex.Message}");
            }
            
            // Reset position
            target.transform.position = originalPos;
        }
        
        public async Task DemoAsyncSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Starting async sequence demo on {target.name}");
            
            var originalPos = target.transform.position;
            var originalScale = target.transform.localScale;
            
            try
            {
                // Step 1: Move up
                Log("Step 1: Moving up...");
                var moveTween = TweenHelper.MoveTo(target.transform, originalPos + Vector3.up * 3f, animationDuration);
                await TweenHelper.AwaitCompletion(moveTween);
                
                // Step 2: Scale up
                Log("Step 2: Scaling up...");
                var scaleTween = TweenHelper.ScaleTo(target.transform, originalScale * 2f, animationDuration * 0.5f);
                await TweenHelper.AwaitCompletion(scaleTween);
                
                // Step 3: Delay
                Log("Step 3: Waiting...");
                await TweenHelper.Delay(1f);
                
                // Step 4: Rotate
                Log("Step 4: Rotating...");
                var rotateTween = TweenHelper.RotateBy(target.transform, Vector3.up * 360f, animationDuration);
                await TweenHelper.AwaitCompletion(rotateTween);
                
                // Step 5: Return to original state
                Log("Step 5: Returning to original state...");
                var returnMove = TweenHelper.MoveTo(target.transform, originalPos, animationDuration * 0.5f);
                var returnScale = TweenHelper.ScaleTo(target.transform, originalScale, animationDuration * 0.5f);
                
                await TweenHelper.AwaitAll(new[] { returnMove, returnScale });
                
                Log("Async sequence completed successfully!");
            }
            catch (System.Exception ex)
            {
                Log($"Async sequence failed: {ex.Message}");
                
                // Reset state on error
                target.transform.position = originalPos;
                target.transform.localScale = originalScale;
            }
        }
        
        public async Task DemoDirectAwait()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Starting direct await demo on {target.name}");
            
            var originalPos = target.transform.position;
            
            try
            {
                // Demonstrate direct awaiting using extension method
                Log("Using direct tween awaiting...");
                
                var moveTween = TweenHelper.MoveTo(target.transform, originalPos + Vector3.left * 3f, animationDuration);
                
                // Direct await using extension method
                bool result = await moveTween;
                
                Log($"Direct await result: {result}");
                
                // Return position
                var returnTween = TweenHelper.MoveTo(target.transform, originalPos, animationDuration * 0.5f);
                await returnTween;
                
                Log("Direct await demo completed!");
            }
            catch (System.Exception ex)
            {
                Log($"Direct await failed: {ex.Message}");
                target.transform.position = originalPos;
            }
        }
        
        public async Task DemoAsyncProgress()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Starting async progress demo on {target.name}");
            
            var progressIndicator = target; // Use the same object as progress indicator
            
            await TweenAsync.RunWithProgressIndicator(
                async (progress, cancellation) =>
                {
                    for (int i = 0; i <= 10; i++)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        
                        progress.Report(i / 10f);
                        Log($"Progress: {i * 10}%");
                        
                        await Task.Delay(200, cancellation);
                    }
                },
                progressIndicator.transform
            );
            
            Log("Async progress demo completed!");
        }
        
        private GameObject GetNextDemoObject()
        {
            if (demoObjects == null || demoObjects.Length == 0) return null;
            
            var target = demoObjects[currentObjectIndex];
            currentObjectIndex = (currentObjectIndex + 1) % demoObjects.Length;
            return target;
        }
        
        private void Log(string message)
        {
            if (showAsyncLogs)
            {
                Debug.Log($"[AsyncDemo] {message}");
            }
        }
        
        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }
        
        private void Update()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) _ = DemoAwaitCompletion();
            if (Input.GetKeyDown(KeyCode.Alpha2)) _ = DemoAwaitTimeout();
            if (Input.GetKeyDown(KeyCode.Alpha3)) _ = DemoAwaitAll();
            if (Input.GetKeyDown(KeyCode.Alpha4)) _ = DemoAwaitAny();
            if (Input.GetKeyDown(KeyCode.Alpha5)) _ = DemoCancellation();
            if (Input.GetKeyDown(KeyCode.Alpha6)) _ = DemoAsyncSequence();
            if (Input.GetKeyDown(KeyCode.Alpha7)) _ = DemoDirectAwait();
            if (Input.GetKeyDown(KeyCode.P)) _ = DemoAsyncProgress();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 200));
            GUILayout.Label("Async Demo Keys:", GUI.skin.box);
            GUILayout.Label("1 : Await completion");
            GUILayout.Label("2 : Await timeout");
            GUILayout.Label("3 : Await all");
            GUILayout.Label("4 : Await any");
            GUILayout.Label("5 : Cancellation");
            GUILayout.Label("6 : Async sequence");
            GUILayout.Label("7 : Direct await");
            GUILayout.Label("P : Progress demo");
            
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                GUILayout.Label("(Cancellation token active)", GUI.skin.box);
            }
            GUILayout.EndArea();
        }
    }
}
