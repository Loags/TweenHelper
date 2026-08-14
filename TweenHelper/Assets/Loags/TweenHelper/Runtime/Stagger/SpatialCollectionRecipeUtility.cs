using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal enum SpatialCollectionAnimation
    {
        BurstIn,
        BurstOut,
        GatherTo
    }

    internal static class SpatialCollectionRecipeUtility
    {
        public static TweenHandle Create(IReadOnlyList<GameObject> targets, GameObject owner, SpatialCollectionAnimation animation, Vector3 anchor, float distance, float duration, float interval, bool local, TweenOptions options)
        {
            ValidateRequest(targets, owner, anchor, distance, duration, interval, options);
            if (targets.Count == 0)
            {
                Debug.LogWarning("Spatial collection recipe: The target collection is empty.");
                return new TweenHandle(null);
            }

            float strength = ResolveStrength(options);
            float totalDuration = duration + (targets.Count - 1) * interval;
            var states = new SpatialItemState[targets.Count];
            for (int i = 0; i < targets.Count; i++) states[i] = new SpatialItemState(targets[i], local, i, targets.Count);

            void Initialize()
            {
                for (int i = 0; i < states.Length; i++) states[i].Initialize(animation, anchor, distance * strength, strength);
            }

            void Evaluate(float value)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    float progress = Mathf.Clamp01((value - i * interval) / duration);
                    states[i].Apply(animation, progress, options.Ease);
                }
            }

            var tween = NormalizedTweenTimeline.Create(
                owner,
                totalDuration,
                options.SetEase(Ease.Linear),
                Initialize,
                progress => Evaluate(progress * totalDuration),
                () => Complete(states, animation),
                () => Restore(states),
                () => Restore(states),
                () => Restore(states),
                () => Evaluate(0f));
            tween.Play();
            return new TweenHandle(tween);
        }

        private static void Complete(SpatialItemState[] states, SpatialCollectionAnimation animation)
        {
            for (int i = 0; i < states.Length; i++) states[i].Complete(animation);
        }

        private static void Restore(SpatialItemState[] states)
        {
            for (int i = 0; i < states.Length; i++) states[i].Restore();
        }

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateRequest(IReadOnlyList<GameObject> targets, GameObject owner, Vector3 anchor, float distance, float duration, float interval, TweenOptions options)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            ValidateVector(anchor, nameof(anchor));
            ValidateFinite(distance, nameof(distance));
            ValidateFinite(duration, nameof(duration));
            ValidateFinite(interval, nameof(interval));
            if (distance < 0f) throw new ArgumentOutOfRangeException(nameof(distance), distance, "Distance cannot be negative.");
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (interval < 0f) throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval cannot be negative.");
            if (options.SpeedBased == true) throw new NotSupportedException("Spatial collection recipes do not support speed-based timing.");

            var unique = new HashSet<GameObject>();
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null) throw new ArgumentException($"Target at index {i} is null or destroyed.", nameof(targets));
                if (!unique.Add(targets[i])) throw new ArgumentException($"Target '{targets[i].name}' occurs more than once.", nameof(targets));
            }
        }

        private static void ValidateVector(Vector3 value, string parameterName)
        {
            ValidateFinite(value.x, parameterName);
            ValidateFinite(value.y, parameterName);
            ValidateFinite(value.z, parameterName);
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private sealed class SpatialItemState
        {
            private readonly GameObject _target;
            private readonly bool _local;
            private readonly int _index;
            private readonly int _count;
            private DestinationMotionUtility.PositionBinding _positionBinding;
            private TweenTargetUtility.TweenAlphaBinding _alphaBinding;
            private bool _hasAlpha;
            private bool _initialized;
            private Vector3 _startPosition;
            private Vector3 _endPosition;
            private Vector3 _startScale;
            private Vector3 _endScale;
            private Quaternion _startRotation;
            private Quaternion _endRotation;
            private float _startAlpha;
            private float _endAlpha;
            private Vector3 _invocationPosition;
            private Vector3 _invocationScale;
            private Quaternion _invocationRotation;
            private float _invocationAlpha;

            public SpatialItemState(GameObject target, bool local, int index, int count)
            {
                _target = target;
                _local = local;
                _index = index;
                _count = count;
            }

            public void Initialize(SpatialCollectionAnimation animation, Vector3 anchor, float distance, float strength)
            {
                if (_initialized) return;
                _positionBinding = new DestinationMotionUtility.PositionBinding(_target, _local);
                _hasAlpha = TweenTargetUtility.TryGetAlphaBinding(_target, out _alphaBinding);
                _invocationPosition = _positionBinding.Get();
                _invocationScale = _target.transform.localScale;
                _invocationRotation = _target.transform.localRotation;
                _invocationAlpha = _hasAlpha ? _alphaBinding.GetAlpha() : 1f;
                Vector3 radial = _invocationPosition - anchor;
                if (radial.sqrMagnitude <= 0.000001f)
                {
                    float angle = _count <= 0 ? 0f : _index / (float)_count * Mathf.PI * 2f;
                    radial = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                }
                radial.Normalize();

                _startPosition = animation == SpatialCollectionAnimation.BurstIn ? anchor : _invocationPosition;
                _endPosition = animation == SpatialCollectionAnimation.BurstOut ? _invocationPosition + radial * distance : animation == SpatialCollectionAnimation.GatherTo ? anchor : _invocationPosition;
                _startScale = animation == SpatialCollectionAnimation.BurstIn ? _invocationScale * Mathf.Max(0f, 1f - 0.5f * strength) : _invocationScale;
                _endScale = animation == SpatialCollectionAnimation.BurstIn ? _invocationScale : Vector3.zero;
                _startRotation = _invocationRotation;
                float rotationAngle = animation == SpatialCollectionAnimation.BurstIn ? -35f : animation == SpatialCollectionAnimation.BurstOut ? 70f : 180f;
                rotationAngle *= strength;
                if (_index % 2 != 0) rotationAngle = -rotationAngle;
                Quaternion rotationOffset = _target.transform is RectTransform ? Quaternion.Euler(0f, 0f, rotationAngle) : Quaternion.Euler(0f, rotationAngle, 0f);
                _endRotation = animation == SpatialCollectionAnimation.BurstIn ? _invocationRotation : _invocationRotation * rotationOffset;
                _startAlpha = animation == SpatialCollectionAnimation.BurstIn ? 0f : _invocationAlpha;
                _endAlpha = animation == SpatialCollectionAnimation.BurstIn ? _invocationAlpha : 0f;
                _initialized = true;
            }

            public void Apply(SpatialCollectionAnimation animation, float progress, Ease? requestedEase)
            {
                Ease positionEase = requestedEase ?? (animation == SpatialCollectionAnimation.BurstIn ? Ease.OutCubic : Ease.InCubic);
                Ease scaleEase = animation == SpatialCollectionAnimation.BurstIn ? Ease.OutBack : Ease.InBack;
                Ease alphaEase = animation == SpatialCollectionAnimation.BurstIn ? Ease.OutQuad : Ease.InQuad;
                float positionProgress = EaseValue(progress, positionEase);
                float scaleProgress = EaseValue(progress, scaleEase);
                float alphaProgress = EaseValue(progress, alphaEase);
                _positionBinding.Set(Vector3.LerpUnclamped(_startPosition, _endPosition, positionProgress));
                _target.transform.localScale = Vector3.LerpUnclamped(_startScale, _endScale, scaleProgress);
                _target.transform.localRotation = Quaternion.SlerpUnclamped(_startRotation, _endRotation, positionProgress);
                if (_hasAlpha) _alphaBinding.SetAlpha(Mathf.LerpUnclamped(_startAlpha, _endAlpha, alphaProgress));
            }

            public void Complete(SpatialCollectionAnimation animation)
            {
                if (animation == SpatialCollectionAnimation.BurstIn)
                {
                    Restore();
                    return;
                }

                _positionBinding.Set(_endPosition);
                _target.transform.localScale = _endScale;
                _target.transform.localRotation = _endRotation;
                if (_hasAlpha) _alphaBinding.SetAlpha(_endAlpha);
            }

            public void Restore()
            {
                _positionBinding.Set(_invocationPosition);
                _target.transform.localScale = _invocationScale;
                _target.transform.localRotation = _invocationRotation;
                if (_hasAlpha) _alphaBinding.Restore(_invocationAlpha);
            }

            private static float EaseValue(float progress, Ease ease) => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);
        }
    }
}
