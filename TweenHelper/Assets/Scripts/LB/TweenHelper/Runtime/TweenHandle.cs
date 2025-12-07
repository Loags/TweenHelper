using DG.Tweening;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LB.TweenHelper
{
    /// <summary>
    /// Wrapper for DOTween Tween/Sequence that provides simplified control,
    /// async/await support, and full pass-through to the underlying tween.
    /// </summary>
    public class TweenHandle
    {
        /// <summary>
        /// The underlying DOTween tween or sequence.
        /// </summary>
        public Tween Tween { get; }

        /// <summary>
        /// Creates a new TweenHandle wrapping the specified tween.
        /// </summary>
        /// <param name="tween">The tween to wrap.</param>
        public TweenHandle(Tween tween)
        {
            Tween = tween;
        }

        #region Simplified Control

        /// <summary>
        /// Pauses the tween.
        /// </summary>
        public void Pause()
        {
            Tween?.Pause();
        }

        /// <summary>
        /// Resumes a paused tween.
        /// </summary>
        public void Resume()
        {
            Tween?.Play();
        }

        /// <summary>
        /// Kills the tween.
        /// </summary>
        /// <param name="complete">If true, forces the tween to complete before killing.</param>
        public void Kill(bool complete = false)
        {
            Tween?.Kill(complete);
        }

        /// <summary>
        /// Restarts the tween from the beginning.
        /// </summary>
        /// <param name="includeDelay">If true, includes delay in restart.</param>
        public void Restart(bool includeDelay = true)
        {
            Tween?.Restart(includeDelay);
        }

        /// <summary>
        /// Rewinds the tween to its start position.
        /// </summary>
        /// <param name="includeDelay">If true, includes delay in rewind.</param>
        public void Rewind(bool includeDelay = true)
        {
            Tween?.Rewind(includeDelay);
        }

        /// <summary>
        /// Forces the tween to complete immediately.
        /// </summary>
        public void Complete()
        {
            Tween?.Complete();
        }

        #endregion

        #region Status Properties

        /// <summary>
        /// Returns true if the tween is currently playing.
        /// </summary>
        public bool IsPlaying => Tween?.IsPlaying() ?? false;

        /// <summary>
        /// Returns true if the tween has completed.
        /// </summary>
        public bool IsComplete => Tween?.IsComplete() ?? true;

        /// <summary>
        /// Returns true if the tween is active (exists and hasn't been killed).
        /// </summary>
        public bool IsActive => Tween?.IsActive() ?? false;

        /// <summary>
        /// Returns true if the tween is paused.
        /// </summary>
        public bool IsPaused => Tween != null && Tween.IsActive() && !Tween.IsPlaying();

        /// <summary>
        /// Gets the elapsed time of the tween.
        /// </summary>
        public float ElapsedTime => Tween?.Elapsed() ?? 0f;

        /// <summary>
        /// Gets the elapsed percentage of the tween (0-1).
        /// </summary>
        public float ElapsedPercentage => Tween?.ElapsedPercentage() ?? 0f;

        /// <summary>
        /// Gets the total duration of the tween.
        /// </summary>
        public float Duration => Tween?.Duration() ?? 0f;

        #endregion

        #region Await Support

        /// <summary>
        /// Awaits the completion of the tween.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>A task that completes when the tween finishes.</returns>
        public async Task AwaitCompletion(CancellationToken cancellationToken = default)
        {
            if (Tween == null || !Tween.IsActive())
            {
                return;
            }

            await TweenAsync.AwaitCompletion(Tween, cancellationToken);
        }

        /// <summary>
        /// Awaits the completion of the tween with a timeout.
        /// </summary>
        /// <param name="timeoutSeconds">The timeout in seconds.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the await.</param>
        /// <returns>True if the tween completed normally, false if it was killed or timed out.</returns>
        public async Task<bool> AwaitWithTimeout(float timeoutSeconds, CancellationToken cancellationToken = default)
        {
            if (Tween == null || !Tween.IsActive())
            {
                return false;
            }

            return await TweenAsync.AwaitCompletionWithTimeout(Tween, timeoutSeconds, cancellationToken);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Adds an OnComplete callback to the tween.
        /// </summary>
        /// <param name="callback">The callback to invoke when the tween completes.</param>
        /// <returns>This TweenHandle for chaining.</returns>
        public TweenHandle OnComplete(Action callback)
        {
            Tween?.OnComplete(() => callback?.Invoke());
            return this;
        }

        /// <summary>
        /// Adds an OnKill callback to the tween.
        /// </summary>
        /// <param name="callback">The callback to invoke when the tween is killed.</param>
        /// <returns>This TweenHandle for chaining.</returns>
        public TweenHandle OnKill(Action callback)
        {
            Tween?.OnKill(() => callback?.Invoke());
            return this;
        }

        /// <summary>
        /// Adds an OnPause callback to the tween.
        /// </summary>
        /// <param name="callback">The callback to invoke when the tween is paused.</param>
        /// <returns>This TweenHandle for chaining.</returns>
        public TweenHandle OnPause(Action callback)
        {
            Tween?.OnPause(() => callback?.Invoke());
            return this;
        }

        /// <summary>
        /// Adds an OnPlay callback to the tween.
        /// </summary>
        /// <param name="callback">The callback to invoke when the tween starts playing.</param>
        /// <returns>This TweenHandle for chaining.</returns>
        public TweenHandle OnPlay(Action callback)
        {
            Tween?.OnPlay(() => callback?.Invoke());
            return this;
        }

        /// <summary>
        /// Adds an OnUpdate callback to the tween.
        /// </summary>
        /// <param name="callback">The callback to invoke on each update.</param>
        /// <returns>This TweenHandle for chaining.</returns>
        public TweenHandle OnUpdate(Action callback)
        {
            Tween?.OnUpdate(() => callback?.Invoke());
            return this;
        }

        #endregion

        #region Implicit Conversions

        /// <summary>
        /// Implicitly converts a TweenHandle to its underlying Tween.
        /// Allows using TweenHandle wherever a Tween is expected.
        /// </summary>
        /// <param name="handle">The TweenHandle to convert.</param>
        public static implicit operator Tween(TweenHandle handle) => handle?.Tween;

        /// <summary>
        /// Implicitly converts a Tween to a TweenHandle.
        /// </summary>
        /// <param name="tween">The Tween to wrap.</param>
        public static implicit operator TweenHandle(Tween tween) => new TweenHandle(tween);

        #endregion

        /// <summary>
        /// Returns a string representation of this handle.
        /// </summary>
        public override string ToString()
        {
            if (Tween == null)
            {
                return "TweenHandle(null)";
            }

            var status = IsComplete ? "Complete" : (IsPlaying ? "Playing" : "Paused");
            return $"TweenHandle({status}, {ElapsedPercentage:P0})";
        }
    }
}
