using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace LB.TweenHelper
{
    public enum TweenPlayerMode
    {
        Recipe = 0,
        Preset = 1
    }

    [AddComponentMenu("Tween Helper/Tween Player")]
    public sealed class TweenPlayer : MonoBehaviour
    {
        [SerializeField] private TweenPlayerMode mode;
        [SerializeField] private string presetName;
        [SerializeField] private GameObject targetOverride;
        [SerializeField] private bool overrideDuration;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private TweenPlayerOverrides presetOverrides = new TweenPlayerOverrides();
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
        public TweenPlayerMode Mode => mode;
        public string PresetName => presetName;
        public GameObject PresetTarget => targetOverride != null ? targetOverride : gameObject;

        private void Reset() => mode = TweenPlayerMode.Preset;

        public void SetPreset(string name, GameObject target = null)
        {
            Kill();
            mode = TweenPlayerMode.Preset;
            presetName = name;
            targetOverride = target;
        }

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
            if (!TryBuild(out TweenHandle handle, out validation))
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

        public TweenRecipeValidationResult Validate()
        {
            if (mode == TweenPlayerMode.Recipe) return TweenRecipeValidator.Validate(recipe, bindings);
            var result = new TweenRecipeValidationResult();
            ITweenPreset preset = TweenPresetRegistry.GetPresetByName(presetName);
            if (preset == null) result.Add(TweenRecipeValidationSeverity.Error, $"Select a registered animation. Saved preset: '{presetName}'.");
            else if (!preset.CanApplyTo(PresetTarget)) result.Add(TweenRecipeValidationSeverity.Error, $"'{presetName}' cannot animate '{PresetTarget.name}'. Check its components and active state.");
            if (overrideDuration && (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f)) result.Add(TweenRecipeValidationSeverity.Error, "Duration must be finite and greater than zero.");
            presetOverrides.Validate(preset, result);
            return result;
        }

        public bool TryBuild(out TweenHandle handle, out TweenRecipeValidationResult validation, UpdateType updateType = UpdateType.Normal)
        {
            if (mode == TweenPlayerMode.Recipe) return TweenRecipeExecutor.TryBuild(recipe, bindings, gameObject, out handle, out validation, useUnscaledTime, updateType, motionPreference);
            handle = null;
            validation = Validate();
            if (!validation.IsValid) return false;
            try
            {
                TweenOptions options = presetOverrides.Apply(TweenOptions.WithMotionPreference(motionPreference), TweenPresetRegistry.GetPresetByName(presetName));
                handle = PresetTarget.Tween().WithOptions(options).PresetByName(presetName, overrideDuration ? duration : (float?)null).Build();
                if (handle.Tween == null) throw new InvalidOperationException("The preset did not create a tween.");
                handle.Tween.SetAutoKill(false).SetRecyclable(false).SetUpdate(updateType, useUnscaledTime);
                return true;
            }
            catch (Exception exception)
            {
                handle?.Kill();
                handle = null;
                validation.Add(TweenRecipeValidationSeverity.Error, $"Could not build preset: {exception.Message}");
                return false;
            }
        }

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
