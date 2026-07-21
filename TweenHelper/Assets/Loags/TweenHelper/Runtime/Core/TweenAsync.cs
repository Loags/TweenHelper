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
        private static readonly TweenCallback EmptyTweenCallback = () => { };

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
            
            try
            {
                await AwaitCompletionResult(tween, cancellationToken);
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
                    return await AwaitCompletionResult(tween, combinedCts.Token);
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

        private static async Task<bool> AwaitCompletionResult(Tween tween, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            int terminalState = 0;

            void OnComplete()
            {
                if (Interlocked.CompareExchange(ref terminalState, 1, 0) == 0) tcs.TrySetResult(true);
            }

            void OnKill()
            {
                if (Interlocked.CompareExchange(ref terminalState, 2, 0) == 0) tcs.TrySetResult(false);
            }

            void Cancel()
            {
                if (Interlocked.CompareExchange(ref terminalState, 3, 0) != 0) return;
                tween.ForceInit();
                TweenExtensions.Kill(tween, false);
                tcs.TrySetCanceled();
            }

            tween.onComplete += OnComplete;
            tween.onKill += OnKill;

            using (cancellationToken.Register(Cancel))
            {
                try
                {
                    return await tcs.Task;
                }
                finally
                {
                    tween.onComplete -= OnComplete;
                    tween.onKill -= OnKill;
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
            
            var delayTween = DOVirtual.DelayedCall(delaySeconds, EmptyTweenCallback, unscaledTime);
            
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
                _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                if (tween != null && tween.IsActive())
                {
                    var tcs = _tcs;
                    TweenCallback onComplete = null;
                    TweenCallback onKill = null;

                    void Complete(bool completed)
                    {
                        tween.onComplete -= onComplete;
                        tween.onKill -= onKill;
                        tcs.TrySetResult(completed);
                    }

                    onComplete = () => Complete(true);
                    onKill = () => Complete(false);
                    tween.onComplete += onComplete;
                    tween.onKill += onKill;
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
            Tweener progressTween = null;
            
            try
            {
                var progress = new Progress<float>(value =>
                {
                    if (progressIndicator != null)
                    {
                        // Animate the progress indicator scale based on progress
                        var targetScale = Vector3.one * Mathf.Lerp(0.1f, 1f, value);
                        if (progressTween == null || !progressTween.IsActive())
                        {
                            progressTween = progressIndicator.DOScale(targetScale, 0.1f);
                        }
                        else
                        {
                            progressTween.ChangeEndValue(targetScale, 0.1f, true);
                            progressTween.Restart();
                        }
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
            if (tween == null || !tween.IsActive()) return new CancellationToken(true);
            return new TweenCancellationRegistration(tween, parentToken).Token;
        }

        /// <summary>
        /// Creates a disposable cancellation registration linked to a tween's lifetime.
        /// </summary>
        public static TweenCancellationRegistration CreateTweenLinkedCancellation(Tween tween, CancellationToken parentToken = default)
        {
            return new TweenCancellationRegistration(tween, parentToken);
        }

        public sealed class TweenCancellationRegistration : IDisposable
        {
            private readonly Tween _tween;
            private readonly CancellationTokenSource _source;
            private CancellationTokenRegistration _parentRegistration;
            private TweenCallback _onKill;
            private bool _isDisposed;

            internal TweenCancellationRegistration(Tween tween, CancellationToken parentToken)
            {
                _tween = tween;
                _source = new CancellationTokenSource();

                if (tween == null || !tween.IsActive())
                {
                    _source.Cancel();
                    return;
                }

                _onKill = OnTweenKilled;
                tween.onKill += _onKill;
                if (parentToken.CanBeCanceled) _parentRegistration = parentToken.Register(OnParentCancelled);
            }

            public CancellationToken Token => _source.Token;

            public void Dispose()
            {
                if (_isDisposed) return;

                _isDisposed = true;
                if (_tween != null && _onKill != null) _tween.onKill -= _onKill;
                _parentRegistration.Dispose();
                _source.Dispose();
                _onKill = null;
            }

            private void OnTweenKilled()
            {
                if (!_source.IsCancellationRequested) _source.Cancel();
                Dispose();
            }

            private void OnParentCancelled()
            {
                if (!_source.IsCancellationRequested) _source.Cancel();
            }
        }
    }
}
