using DG.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Async utilities for awaiting tween completion.
    /// Provides simple "await until complete" and timeout utilities for integration with async flows.
    /// </summary>
    public static class TweenAsync
    {
        /// <summary>
        /// Awaits the completion of a tween.
        /// </summary>
        /// <param name="tween">The tween to await.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when the tween finishes.</returns>
        public static async Task AwaitCompletion(Tween tween, CancellationToken cancellationToken = default)
        {
            if (tween == null)
            {
                Debug.LogWarning("TweenAsync: Cannot await null tween.");
                return;
            }
            
            if (!tween.IsActive())
            {
                Debug.LogWarning("TweenAsync: Cannot await inactive tween.");
                return;
            }
            
            var tcs = new TaskCompletionSource<bool>();
            
            // Handle cancellation
            cancellationToken.Register(() =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    tween.Kill();
                    tcs.TrySetCanceled();
                }
            });
            
            // Set up completion callback
            tween.OnComplete(() =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    tcs.TrySetResult(true);
                }
            });
            
            // Set up kill callback for early termination
            tween.OnKill(() =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    tcs.TrySetResult(false);
                }
            });
            
            try
            {
                await tcs.Task;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("TweenAsync: Tween await was cancelled.");
                throw;
            }
        }
        
        /// <summary>
        /// Awaits the completion of a tween with a timeout.
        /// </summary>
        /// <param name="tween">The tween to await.</param>
        /// <param name="timeoutSeconds">The timeout in seconds.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>True if the tween completed normally, false if it was killed or timed out.</returns>
        public static async Task<bool> AwaitCompletionWithTimeout(Tween tween, float timeoutSeconds, CancellationToken cancellationToken = default)
        {
            if (tween == null)
            {
                Debug.LogWarning("TweenAsync: Cannot await null tween.");
                return false;
            }
            
            if (!tween.IsActive())
            {
                Debug.LogWarning("TweenAsync: Cannot await inactive tween.");
                return false;
            }
            
            if (timeoutSeconds <= 0f)
            {
                Debug.LogWarning("TweenAsync: Timeout must be greater than zero.");
                return false;
            }
            
            using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
            using (var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
            {
                try
                {
                    await AwaitCompletion(tween, combinedCts.Token);
                    return true;
                }
                catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
                {
                    Debug.LogWarning($"TweenAsync: Tween await timed out after {timeoutSeconds} seconds. Killing tween.");
                    tween.Kill();
                    return false;
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("TweenAsync: Tween await was cancelled by external token.");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Awaits the completion of multiple tweens.
        /// </summary>
        /// <param name="tweens">The tweens to await.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when all tweens finish.</returns>
        public static async Task AwaitAll(Tween[] tweens, CancellationToken cancellationToken = default)
        {
            if (tweens == null || tweens.Length == 0)
            {
                Debug.LogWarning("TweenAsync: Cannot await null or empty tween array.");
                return;
            }
            
            var tasks = new Task[tweens.Length];
            for (int i = 0; i < tweens.Length; i++)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution continues before call is completed
                tasks[i] = AwaitCompletion(tweens[i], cancellationToken);
#pragma warning restore CS4014
            }
            
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("TweenAsync: Awaiting multiple tweens was cancelled.");
                throw;
            }
        }
        
        /// <summary>
        /// Awaits the completion of any one of multiple tweens.
        /// </summary>
        /// <param name="tweens">The tweens to await.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when the first tween finishes.</returns>
        public static async Task AwaitAny(Tween[] tweens, CancellationToken cancellationToken = default)
        {
            if (tweens == null || tweens.Length == 0)
            {
                Debug.LogWarning("TweenAsync: Cannot await null or empty tween array.");
                return;
            }
            
            var tasks = new Task[tweens.Length];
            for (int i = 0; i < tweens.Length; i++)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution continues before call is completed
                tasks[i] = AwaitCompletion(tweens[i], cancellationToken);
#pragma warning restore CS4014
            }
            
            try
            {
                await Task.WhenAny(tasks);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("TweenAsync: Awaiting any tween was cancelled.");
                throw;
            }
        }
        
        /// <summary>
        /// Awaits a delay using DoTween's time system.
        /// </summary>
        /// <param name="delaySeconds">The delay in seconds.</param>
        /// <param name="unscaledTime">Whether to use unscaled time.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the delay.</param>
        /// <returns>A task that completes after the delay.</returns>
        public static async Task Delay(float delaySeconds, bool unscaledTime = false, CancellationToken cancellationToken = default)
        {
            if (delaySeconds <= 0f)
            {
                Debug.LogWarning("TweenAsync: Delay must be greater than zero.");
                return;
            }
            
            // Create a dummy tween for the delay
            var delayTween = DOTween.To(() => 0f, x => { }, 1f, delaySeconds);
            delayTween.SetUpdate(unscaledTime);
            
            await AwaitCompletion(delayTween, cancellationToken);
        }
        
        /// <summary>
        /// Creates an awaitable wrapper for a tween that can be used with async/await.
        /// </summary>
        /// <param name="tween">The tween to wrap.</param>
        /// <returns>An awaitable tween wrapper.</returns>
        public static TweenAwaiter GetAwaiter(this Tween tween)
        {
            return new TweenAwaiter(tween);
        }
        
        /// <summary>
        /// Awaitable wrapper for tweens that enables direct awaiting of tween instances.
        /// </summary>
        public struct TweenAwaiter : System.Runtime.CompilerServices.INotifyCompletion
        {
            private readonly TaskCompletionSource<bool> _tcs;
            
            public TweenAwaiter(Tween tween)
            {
                _tcs = new TaskCompletionSource<bool>();
                
                if (tween != null && tween.IsActive())
                {
                    var tcs = _tcs; // Capture for lambda
                    tween.OnComplete(() => tcs.TrySetResult(true));
                    tween.OnKill(() => tcs.TrySetResult(false));
                }
                else
                {
                    _tcs.TrySetResult(false);
                }
            }
            
            public bool IsCompleted => _tcs.Task.IsCompleted;
            
            public void OnCompleted(Action continuation)
            {
                _tcs.Task.GetAwaiter().OnCompleted(continuation);
            }
            
            public bool GetResult()
            {
                return _tcs.Task.GetAwaiter().GetResult();
            }
        }
        
        /// <summary>
        /// Utility for running an async operation with a tween progress indicator.
        /// </summary>
        /// <param name="asyncOperation">The async operation to run.</param>
        /// <param name="progressIndicator">Optional transform to animate as a progress indicator.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A task that completes when the async operation finishes.</returns>
        public static async Task RunWithProgressIndicator(
            Func<IProgress<float>, CancellationToken, Task> asyncOperation,
            Transform progressIndicator = null,
            CancellationToken cancellationToken = default)
        {
            Tween progressTween = null;
            
            try
            {
                var progress = new Progress<float>(value =>
                {
                    if (progressIndicator != null)
                    {
                        // Animate the progress indicator scale based on progress
                        progressTween?.Kill();
                        var targetScale = Vector3.one * Mathf.Lerp(0.1f, 1f, value);
                        progressTween = progressIndicator.DOScale(targetScale, 0.1f);
                    }
                });
                
                await asyncOperation(progress, cancellationToken);
            }
            finally
            {
                progressTween?.Kill();
                
                if (progressIndicator != null)
                {
                    // Reset progress indicator
                    progressIndicator.localScale = Vector3.one;
                }
            }
        }
        
        /// <summary>
        /// Creates a cancellation token that automatically cancels when a tween is killed.
        /// </summary>
        /// <param name="tween">The tween to monitor.</param>
        /// <param name="parentToken">Optional parent cancellation token to link with.</param>
        /// <returns>A cancellation token that cancels when the tween is killed.</returns>
        public static CancellationToken CreateTweenLinkedToken(Tween tween, CancellationToken parentToken = default)
        {
            if (tween == null || !tween.IsActive())
            {
                return new CancellationToken(true); // Already cancelled
            }
            
            var cts = parentToken.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(parentToken)
                : new CancellationTokenSource();
            
            tween.OnKill(() =>
            {
                if (!cts.Token.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            });
            
            return cts.Token;
        }
    }
}
