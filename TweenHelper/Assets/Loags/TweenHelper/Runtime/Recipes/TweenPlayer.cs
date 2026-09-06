using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LB.TweenHelper
{
    [AddComponentMenu("Tween Helper/Tween Player")]
    public sealed class TweenPlayer : MonoBehaviour
    {
        [SerializeField] private TweenRecipe recipe;
        [SerializeField] private List<TweenPlayerBinding> bindings = new List<TweenPlayerBinding>();
        [SerializeField] private bool playOnStart;
        [SerializeField] private bool useUnscaledTime;
        [SerializeField] private TweenMotionPreference motionPreference;
        [SerializeField] private UnityEvent onStarted = new UnityEvent();
        [SerializeField] private UnityEvent onCompleted = new UnityEvent();
        [SerializeField] private UnityEvent onKilled = new UnityEvent();

        private TweenHandle _activeHandle;

        public TweenRecipe Recipe => recipe;
        public IReadOnlyList<TweenPlayerBinding> Bindings => bindings;
        public TweenHandle ActiveHandle => _activeHandle;
        public bool IsPlaying => _activeHandle != null && _activeHandle.IsPlaying;
        public TweenMotionPreference MotionPreference => motionPreference;

        public void SetReducedMotion(bool reduced) => motionPreference = reduced ? TweenMotionPreference.Reduced : TweenMotionPreference.Full;

        private void Start()
        {
            if (playOnStart) Play();
        }

        public TweenHandle Play()
        {
            TweenRecipeValidationResult validation = Validate();
            if (!validation.IsValid)
            {
                Debug.LogError($"TweenPlayer '{name}' cannot play.\n{validation.GetSummary()}", this);
                return null;
            }

            Kill();
            if (!TweenRecipeExecutor.TryBuild(recipe, bindings, gameObject, out TweenHandle handle, out validation, useUnscaledTime, motionPreference: motionPreference))
            {
                Debug.LogError($"TweenPlayer '{name}' could not build its recipe.\n{validation.GetSummary()}", this);
                return null;
            }

            _activeHandle = handle;
            handle.OnComplete(() => HandleCompleted(handle));
            handle.OnKill(() => HandleKilled(handle));
            handle.Resume();
            onStarted.Invoke();
            return handle;
        }

        public void Pause() => _activeHandle?.Pause();
        public void PlayFromEvent() => Play();
        public void Resume() => _activeHandle?.Resume();
        public void Restart() => _activeHandle?.Restart();
        public void Rewind() => _activeHandle?.Rewind();
        public void Complete() => _activeHandle?.Complete();

        public void Kill()
        {
            TweenHandle handle = _activeHandle;
            if (handle == null) return;

            if (handle.IsActive)
            {
                handle.Kill();
            }
            else
            {
                _activeHandle = null;
            }
        }

        public TweenRecipeValidationResult Validate() => TweenRecipeValidator.Validate(recipe, bindings);

        private void HandleCompleted(TweenHandle handle)
        {
            if (_activeHandle != handle) return;
            onCompleted.Invoke();
        }

        private void HandleKilled(TweenHandle handle)
        {
            if (_activeHandle != handle) return;
            _activeHandle = null;
            onKilled.Invoke();
        }

        private void OnDisable() => Kill();
        private void OnDestroy() => Kill();
    }
}
