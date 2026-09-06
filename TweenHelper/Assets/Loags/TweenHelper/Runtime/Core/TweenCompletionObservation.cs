using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal sealed class TweenCompletionObservation : IDisposable
    {
        private static readonly HashSet<TweenCompletionObservation> Active = new HashSet<TweenCompletionObservation>();
        private static SynchronizationContext _unityContext;
        private static int _unityThreadId;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CaptureMainThread()
        {
            _unityContext = SynchronizationContext.Current;
            _unityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        private readonly TaskCompletionSource<bool> _completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly SynchronizationContext _context;
        private readonly int _threadId;
        private readonly CancellationToken _cancellationToken;
        private CancellationTokenRegistration _registration;
        private Tween _tween;
        private int _terminalState;

        static TweenCompletionObservation()
        {
            Application.quitting += ReleaseAll;
#if UNITY_EDITOR
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += ReleaseAll;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
        }

        internal TweenCompletionObservation(Tween tween, CancellationToken cancellationToken = default)
        {
            if (Thread.CurrentThread.ManagedThreadId != _unityThreadId)
                throw new InvalidOperationException("Start tween waits on Unity's main thread. Cancellation may be requested from any thread.");
            _context = _unityContext;
            _threadId = _unityThreadId;
            _cancellationToken = cancellationToken;

            if (tween == null || !tween.IsActive())
            {
                _terminalState = 2;
                _completion.SetResult(false);
                return;
            }

            if (tween.IsComplete())
            {
                _terminalState = 1;
                _completion.SetResult(true);
                return;
            }

            if (cancellationToken.CanBeCanceled && _context == null)
                throw new InvalidOperationException("Start cancellable tween waits on Unity's main thread with its synchronization context.");

            _tween = tween;
            Active.Add(this);
            tween.onComplete += OnComplete;
            tween.onKill += OnKill;
            _registration = cancellationToken.Register(RequestCancellation);
            if (_tween == null) _registration.Dispose();
        }

        internal Task<bool> Task => _completion.Task;

        private void OnComplete()
        {
            if (Interlocked.CompareExchange(ref _terminalState, 1, 0) != 0) return;
            Detach();
            _completion.TrySetResult(true);
        }

        private void OnKill()
        {
            int outcome = _cancellationToken.IsCancellationRequested ? 3 : 2;
            int previous = Interlocked.CompareExchange(ref _terminalState, outcome, 0);
            if (previous != 0 && previous != 3) return;
            Detach();
            if (previous == 3 || outcome == 3) _completion.TrySetCanceled(_cancellationToken);
            else _completion.TrySetResult(false);
        }

        private void RequestCancellation()
        {
            if (Interlocked.CompareExchange(ref _terminalState, 3, 0) != 0) return;
            if (Thread.CurrentThread.ManagedThreadId == _threadId) CancelOnMainThread();
            else _context.Post(_ => CancelOnMainThread(), null);
        }

        private void CancelOnMainThread()
        {
            Tween tween = _tween;
            if (tween == null) return;
            Detach();
            try
            {
                if (tween.IsActive())
                {
                    tween.ForceInit();
                    TweenLifetime.Kill(tween);
                }
                _completion.TrySetCanceled(_cancellationToken);
            }
            catch (Exception exception)
            {
                _completion.TrySetException(exception);
            }
        }

        public void Dispose()
        {
            if (_tween == null) return;
            int previous = Interlocked.CompareExchange(ref _terminalState, 4, 0);
            if (previous == 3)
            {
                CancelOnMainThread();
                return;
            }

            if (previous != 0) return;
            Detach();
            _completion.TrySetResult(false);
        }

        private void Detach()
        {
            if (_tween == null) return;
            _tween.onComplete -= OnComplete;
            _tween.onKill -= OnKill;
            _tween = null;
            Active.Remove(this);
            _registration.Dispose();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReleaseAll()
        {
            var observations = new TweenCompletionObservation[Active.Count];
            Active.CopyTo(observations);
            foreach (TweenCompletionObservation observation in observations)
            {
                Interlocked.Exchange(ref observation._terminalState, 4);
                observation.Detach();
                observation._completion.TrySetCanceled();
            }
        }

#if UNITY_EDITOR
        private static void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode || state == UnityEditor.PlayModeStateChange.ExitingEditMode) ReleaseAll();
        }
#endif
    }
}
