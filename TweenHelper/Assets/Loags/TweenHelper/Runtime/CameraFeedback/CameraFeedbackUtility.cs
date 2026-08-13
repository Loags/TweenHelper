using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class CameraFeedbackUtility
    {
        public static Tween CreateImpact(GameObject target, float positionStrength, float rotationStrength, float duration, TweenOptions options)
        {
            ValidateMagnitude(positionStrength, nameof(positionStrength));
            ValidateMagnitude(rotationStrength, nameof(rotationStrength));
            float strength = ResolveStrength(options);
            return CreateTransient(target, duration, options, (state, progress) =>
            {
                float envelope = Mathf.Pow(1f - progress, 2f);
                float horizontal = Mathf.Sin(progress * Mathf.PI * 11f) * envelope;
                float vertical = Mathf.Sin(progress * Mathf.PI * 7f + 0.7f) * envelope;
                Vector3 offset = new Vector3(horizontal, vertical * 0.55f, 0f) * positionStrength * strength;
                Vector3 rotation = new Vector3(-vertical, horizontal * 0.35f, -horizontal) * rotationStrength * strength;
                state.ApplyLocal(offset, Quaternion.Euler(rotation), 0f);
            });
        }

        public static Tween CreateRecoil(GameObject target, float distance, float pitch, float duration, TweenOptions options)
        {
            ValidateMagnitude(distance, nameof(distance));
            ValidateMagnitude(pitch, nameof(pitch));
            float strength = ResolveStrength(options);
            return CreateTransient(target, duration, options, (state, progress) =>
            {
                float kick = progress <= 0.16f
                    ? EaseValue(progress / 0.16f, Ease.OutCubic)
                    : 1f - EaseValue((progress - 0.16f) / 0.84f, Ease.OutBack);
                float aftershock = Mathf.Sin(progress * Mathf.PI * 5f) * Mathf.Pow(1f - progress, 2f);
                Vector3 offset = Vector3.back * distance * kick * strength + Vector3.up * distance * 0.12f * aftershock * strength;
                Quaternion rotation = Quaternion.Euler(-pitch * kick * strength, aftershock * pitch * 0.08f * strength, aftershock * pitch * 0.12f * strength);
                state.ApplyLocal(offset, rotation, 0f);
            });
        }

        public static Tween CreateLandingImpact(GameObject target, float dropDistance, float fieldOfViewKick, float duration, TweenOptions options)
        {
            ValidateMagnitude(dropDistance, nameof(dropDistance));
            ValidateFinite(fieldOfViewKick, nameof(fieldOfViewKick));
            float strength = ResolveStrength(options);
            return CreateTransient(target, duration, options, (state, progress) =>
            {
                float impact = progress <= 0.12f
                    ? EaseValue(progress / 0.12f, Ease.OutQuad)
                    : (1f - EaseValue((progress - 0.12f) / 0.88f, Ease.OutElastic));
                float aftershock = Mathf.Sin(progress * Mathf.PI * 6f) * Mathf.Pow(1f - progress, 2.4f);
                Vector3 offset = Vector3.down * dropDistance * impact * strength;
                Quaternion rotation = Quaternion.Euler(aftershock * 0.8f * strength, 0f, aftershock * 1.6f * strength);
                state.ApplyLocal(offset, rotation, fieldOfViewKick * impact * strength);
            });
        }

        public static Tween CreateFovKick(GameObject target, float fieldOfViewDelta, float duration, TweenOptions options)
        {
            ValidateFinite(fieldOfViewDelta, nameof(fieldOfViewDelta));
            float strength = ResolveStrength(options);
            return CreateTransient(target, duration, options, (state, progress) =>
            {
                float kick = progress <= 0.2f
                    ? EaseValue(progress / 0.2f, Ease.OutCubic)
                    : 1f - EaseValue((progress - 0.2f) / 0.8f, Ease.OutBack);
                state.ApplyLocal(Vector3.zero, Quaternion.identity, fieldOfViewDelta * kick * strength);
            });
        }

        public static Tween CreateFocusZoom(GameObject target, Transform focusTarget, float distance, float fieldOfViewDelta, float duration, TweenOptions options)
        {
            if (focusTarget == null) throw new ArgumentNullException(nameof(focusTarget));
            ValidateMagnitude(distance, nameof(distance));
            ValidateMagnitude(fieldOfViewDelta, nameof(fieldOfViewDelta));
            float strength = ResolveStrength(options);
            Vector3 focusPosition = default;
            Vector3 destination = default;
            Quaternion focusRotation = default;
            bool focusCaptured = false;

            return CreateTransient(target, duration, options, (state, progress) =>
            {
                if (!focusCaptured)
                {
                    focusPosition = focusTarget.position;
                    Vector3 delta = focusPosition - state.WorldPosition;
                    float resolvedDistance = Mathf.Min(distance * strength, delta.magnitude * 0.65f);
                    destination = state.WorldPosition + (delta.sqrMagnitude <= 0.000001f ? state.WorldRotation * Vector3.forward : delta.normalized) * resolvedDistance;
                    Vector3 lookDirection = focusPosition - destination;
                    focusRotation = lookDirection.sqrMagnitude <= 0.000001f ? state.WorldRotation : Quaternion.LookRotation(lookDirection.normalized, state.WorldRotation * Vector3.up);
                    focusCaptured = true;
                }

                float zoom = Mathf.Sin(progress * Mathf.PI);
                float easedZoom = EaseValue(zoom, options.Ease ?? Ease.InOutSine);
                state.ApplyWorld(Vector3.LerpUnclamped(state.WorldPosition, destination, easedZoom), Quaternion.SlerpUnclamped(state.WorldRotation, focusRotation, easedZoom), -fieldOfViewDelta * strength * easedZoom);
            });
        }

        public static Tween CreateBreathing(GameObject target, float positionAmplitude, float rotationAmplitude, float fieldOfViewAmplitude, float duration, TweenOptions options)
        {
            ValidateMagnitude(positionAmplitude, nameof(positionAmplitude));
            ValidateMagnitude(rotationAmplitude, nameof(rotationAmplitude));
            ValidateMagnitude(fieldOfViewAmplitude, nameof(fieldOfViewAmplitude));
            float strength = ResolveStrength(options);
            return CreateTransient(target, duration, options, (state, progress) =>
            {
                float cycle = progress * Mathf.PI * 2f;
                float breath = Mathf.Sin(cycle);
                float secondary = Mathf.Sin(cycle * 2f) * 0.35f;
                Vector3 offset = new Vector3(secondary * 0.35f, breath, 0f) * positionAmplitude * strength;
                Quaternion rotation = Quaternion.Euler(-breath * rotationAmplitude * strength, secondary * rotationAmplitude * strength, -secondary * rotationAmplitude * 0.5f * strength);
                state.ApplyLocal(offset, rotation, breath * fieldOfViewAmplitude * strength);
            });
        }

        private static Tween CreateTransient(GameObject target, float duration, TweenOptions options, Action<CameraState, float> evaluator)
        {
            ValidateRequest(target, duration, options);
            var state = new CameraState(RequireCamera(target));
            float progress = 0f;
            bool initialized = false;
            bool completed = false;

            void EnsureInitialized()
            {
                if (initialized) return;
                state.Initialize();
                initialized = true;
            }

            var tween = DOTween.To(() => progress, value =>
            {
                progress = value;
                EnsureInitialized();
                evaluator(state, Mathf.Clamp01(value));
            }, 1f, duration);

            tween.WithDefaults(options.SetEase(Ease.Linear), target);
            tween.OnStart(() =>
            {
                completed = false;
                EnsureInitialized();
                evaluator(state, 0f);
            });
            tween.OnComplete(() =>
            {
                completed = true;
                state.Restore();
            });
            tween.OnRewind(() =>
            {
                completed = false;
                if (initialized) state.Restore();
            });
            tween.OnKill(() =>
            {
                if (!completed && initialized) state.Restore();
            });
            tween.Pause();
            return tween;
        }

        private static Camera RequireCamera(GameObject target)
        {
            var camera = target.GetComponent<Camera>();
            if (camera == null) throw new InvalidOperationException($"Camera feedback target '{target.name}' requires a Camera component on the same GameObject.");
            return camera;
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateMagnitude(strength, nameof(TweenOptions.Strength));
            return strength;
        }

        private static void ValidateRequest(GameObject target, float duration, TweenOptions options)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (options.SpeedBased == true) throw new NotSupportedException("Camera feedback does not support speed-based timing.");
        }

        private static void ValidateMagnitude(float value, string parameterName)
        {
            ValidateFinite(value, parameterName);
            if (value < 0f) throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private static float EaseValue(float progress, Ease ease) => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);

        private sealed class CameraState
        {
            private readonly Camera _camera;
            private Vector3 _localPosition;
            private Quaternion _localRotation;
            private float _fieldOfView;

            public CameraState(Camera camera)
            {
                _camera = camera;
            }

            public Vector3 WorldPosition { get; private set; }
            public Quaternion WorldRotation { get; private set; }

            public void Initialize()
            {
                Transform transform = _camera.transform;
                _localPosition = transform.localPosition;
                _localRotation = transform.localRotation;
                WorldPosition = transform.position;
                WorldRotation = transform.rotation;
                _fieldOfView = _camera.fieldOfView;
            }

            public void ApplyLocal(Vector3 positionOffset, Quaternion rotationOffset, float fieldOfViewOffset)
            {
                Transform transform = _camera.transform;
                transform.localPosition = _localPosition + _localRotation * positionOffset;
                transform.localRotation = _localRotation * rotationOffset;
                _camera.fieldOfView = Mathf.Clamp(_fieldOfView + fieldOfViewOffset, 1f, 179f);
            }

            public void ApplyWorld(Vector3 position, Quaternion rotation, float fieldOfViewOffset)
            {
                Transform transform = _camera.transform;
                transform.position = position;
                transform.rotation = rotation;
                _camera.fieldOfView = Mathf.Clamp(_fieldOfView + fieldOfViewOffset, 1f, 179f);
            }

            public void Restore()
            {
                Transform transform = _camera.transform;
                transform.localPosition = _localPosition;
                transform.localRotation = _localRotation;
                _camera.fieldOfView = _fieldOfView;
            }
        }
    }
}
