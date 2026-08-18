using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal enum CollectionTopologyAnimation
    {
        Accordion,
        OrbitIn,
        OrbitOut,
        LoadingRing,
        LoadingRibbon
    }

    internal static class CollectionTopologyUtility
    {
        public static TweenHandle CreateSpatial(IReadOnlyList<GameObject> targets, GameObject owner, CollectionTopologyAnimation animation, Vector3 anchor, float radius, float duration, float interval, bool local, TweenOptions options)
        {
            ValidateRequest(targets, owner, anchor, radius, duration, interval, options);
            if (targets.Count == 0) return new TweenHandle(null);
            float strength = ResolveStrength(options);
            float totalDuration = duration + (targets.Count - 1) * interval;
            var states = CreateStates(targets, local);
            Vector3 center = default;

            void Initialize()
            {
                for (int i = 0; i < states.Length; i++) states[i].Capture();
                center = CalculateCenter(states);
                for (int i = 0; i < states.Length; i++) states[i].InitializeSpatial(animation, anchor, center, radius * strength, strength);
            }

            void Evaluate(float elapsed)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    float progress = Mathf.Clamp01((elapsed - i * interval) / duration);
                    states[i].ApplySpatial(animation, progress, options.Ease);
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

        public static TweenHandle CreateLoading(IReadOnlyList<GameObject> targets, GameObject owner, CollectionTopologyAnimation animation, float amplitude, float cycleDuration, bool local, TweenOptions options)
        {
            ValidateRequest(targets, owner, Vector3.zero, amplitude, cycleDuration, 0f, options);
            if (targets.Count == 0) return new TweenHandle(null);
            float strength = ResolveStrength(options);
            var states = CreateStates(targets, local);
            TweenOptions loopOptions = options.SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

            void Initialize()
            {
                for (int i = 0; i < states.Length; i++) states[i].Capture();
            }

            void Evaluate(float progress)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    float phase = Mathf.Repeat(progress - i / (float)states.Length + 1f, 1f);
                    states[i].ApplyLoading(animation, phase, amplitude * strength);
                }
            }

            var tween = NormalizedTweenTimeline.Create(owner, cycleDuration, loopOptions, Initialize, Evaluate, () => Restore(states), () => Restore(states), () => Restore(states), () => Restore(states));
            tween.Play();
            return new TweenHandle(tween);
        }

        private static ItemState[] CreateStates(IReadOnlyList<GameObject> targets, bool local)
        {
            var states = new ItemState[targets.Count];
            for (int i = 0; i < targets.Count; i++) states[i] = new ItemState(targets[i], local, i, targets.Count);
            return states;
        }

        private static Vector3 CalculateCenter(IReadOnlyList<ItemState> states)
        {
            Vector3 sum = default;
            for (int i = 0; i < states.Count; i++) sum += states[i].InvocationPosition;
            return sum / states.Count;
        }

        private static void Complete(ItemState[] states, CollectionTopologyAnimation animation)
        {
            for (int i = 0; i < states.Length; i++) states[i].Complete(animation);
        }

        private static void Restore(ItemState[] states)
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

        private static void ValidateRequest(IReadOnlyList<GameObject> targets, GameObject owner, Vector3 anchor, float magnitude, float duration, float interval, TweenOptions options)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            ValidateFinite(anchor.x, nameof(anchor));
            ValidateFinite(anchor.y, nameof(anchor));
            ValidateFinite(anchor.z, nameof(anchor));
            ValidateFinite(magnitude, nameof(magnitude));
            ValidateFinite(duration, nameof(duration));
            ValidateFinite(interval, nameof(interval));
            if (magnitude < 0f) throw new ArgumentOutOfRangeException(nameof(magnitude), magnitude, "Magnitude cannot be negative.");
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (interval < 0f) throw new ArgumentOutOfRangeException(nameof(interval), interval, "Interval cannot be negative.");
            if (options.SpeedBased == true) throw new NotSupportedException("Collection topology animations do not support speed-based timing.");

            var unique = new HashSet<GameObject>();
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null) throw new ArgumentException($"Target at index {i} is null or destroyed.", nameof(targets));
                if (!unique.Add(targets[i])) throw new ArgumentException($"Target '{targets[i].name}' occurs more than once.", nameof(targets));
            }
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private sealed class ItemState
        {
            private readonly GameObject _target;
            private readonly bool _local;
            private readonly int _index;
            private readonly int _count;
            private DestinationMotionUtility.PositionBinding _binding;
            private TweenTargetUtility.TweenAlphaBinding _alphaBinding;
            private bool _hasAlpha;
            private Vector3 _startPosition;
            private Vector3 _endPosition;
            private Vector3 _startScale;
            private Vector3 _endScale;
            private Quaternion _startRotation;
            private Quaternion _endRotation;
            private float _startAlpha;
            private float _endAlpha;

            public ItemState(GameObject target, bool local, int index, int count)
            {
                _target = target;
                _local = local;
                _index = index;
                _count = count;
            }

            public Vector3 InvocationPosition { get; private set; }
            private Vector3 InvocationScale { get; set; }
            private Quaternion InvocationRotation { get; set; }
            private float InvocationAlpha { get; set; }

            public void Capture()
            {
                _binding = new DestinationMotionUtility.PositionBinding(_target, _local);
                _hasAlpha = TweenTargetUtility.TryGetAlphaBinding(_target, out _alphaBinding);
                InvocationPosition = _binding.Get();
                InvocationScale = _target.transform.localScale;
                InvocationRotation = _target.transform.localRotation;
                InvocationAlpha = _hasAlpha ? _alphaBinding.GetAlpha() : 1f;
            }

            public void InitializeSpatial(CollectionTopologyAnimation animation, Vector3 anchor, Vector3 center, float radius, float strength)
            {
                float angle = _count <= 0 ? 0f : _index / (float)_count * Mathf.PI * 2f;
                Vector3 ring = anchor + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
                _startPosition = animation == CollectionTopologyAnimation.OrbitIn ? ring : animation == CollectionTopologyAnimation.Accordion ? Vector3.LerpUnclamped(center, InvocationPosition, 0.18f) : InvocationPosition;
                _endPosition = animation == CollectionTopologyAnimation.OrbitOut ? ring : InvocationPosition;
                _startScale = animation == CollectionTopologyAnimation.OrbitIn || animation == CollectionTopologyAnimation.Accordion ? InvocationScale * Mathf.Max(0f, 1f - 0.55f * strength) : InvocationScale;
                _endScale = animation == CollectionTopologyAnimation.OrbitOut ? Vector3.zero : InvocationScale;
                float rotation = (animation == CollectionTopologyAnimation.OrbitOut ? 300f : -300f) * strength;
                Quaternion offset = _target.transform is RectTransform ? Quaternion.Euler(0f, 0f, rotation) : Quaternion.Euler(0f, rotation, 0f);
                _startRotation = animation == CollectionTopologyAnimation.OrbitIn ? InvocationRotation * offset : InvocationRotation;
                _endRotation = animation == CollectionTopologyAnimation.OrbitOut ? InvocationRotation * Quaternion.Inverse(offset) : InvocationRotation;
                _startAlpha = animation == CollectionTopologyAnimation.OrbitIn || animation == CollectionTopologyAnimation.Accordion ? 0f : InvocationAlpha;
                _endAlpha = animation == CollectionTopologyAnimation.OrbitOut ? 0f : InvocationAlpha;
            }

            public void ApplySpatial(CollectionTopologyAnimation animation, float progress, Ease? requestedEase)
            {
                float positionProgress = DOVirtual.EasedValue(0f, 1f, progress, requestedEase ?? (animation == CollectionTopologyAnimation.OrbitOut ? Ease.InCubic : Ease.OutCubic));
                float scaleProgress = DOVirtual.EasedValue(0f, 1f, progress, animation == CollectionTopologyAnimation.OrbitOut ? Ease.InBack : Ease.OutBack);
                Vector3 position;
                if (animation == CollectionTopologyAnimation.OrbitIn || animation == CollectionTopologyAnimation.OrbitOut)
                {
                    Vector3 center = animation == CollectionTopologyAnimation.OrbitIn ? _endPosition : _startPosition;
                    Vector3 radial = Vector3.LerpUnclamped(_startPosition, _endPosition, positionProgress) - center;
                    float spin = (animation == CollectionTopologyAnimation.OrbitIn ? 1f - positionProgress : positionProgress) * Mathf.PI * 1.5f;
                    float cosine = Mathf.Cos(spin);
                    float sine = Mathf.Sin(spin);
                    Vector3 rotated = new Vector3(radial.x * cosine - radial.y * sine, radial.x * sine + radial.y * cosine, radial.z);
                    position = center + rotated;
                }
                else
                {
                    position = Vector3.LerpUnclamped(_startPosition, _endPosition, positionProgress);
                }

                _binding.Set(position);
                _target.transform.localScale = Vector3.LerpUnclamped(_startScale, _endScale, scaleProgress);
                _target.transform.localRotation = Quaternion.SlerpUnclamped(_startRotation, _endRotation, positionProgress);
                if (_hasAlpha) _alphaBinding.SetAlpha(Mathf.LerpUnclamped(_startAlpha, _endAlpha, positionProgress));
            }

            public void ApplyLoading(CollectionTopologyAnimation animation, float phase, float amplitude)
            {
                float wave = Mathf.Sin(phase * Mathf.PI * 2f);
                float pulse = Mathf.Clamp01((wave + 1f) * 0.5f);
                Vector3 offset = animation == CollectionTopologyAnimation.LoadingRing
                    ? new Vector3(Mathf.Cos(phase * Mathf.PI * 2f), Mathf.Sin(phase * Mathf.PI * 2f), 0f) * amplitude * 0.08f
                    : Vector3.up * wave * amplitude;
                _binding.Set(InvocationPosition + offset);
                _target.transform.localScale = InvocationScale * (0.82f + pulse * 0.28f);
                if (_hasAlpha) _alphaBinding.SetAlpha(InvocationAlpha * (0.45f + pulse * 0.55f));
            }

            public void Complete(CollectionTopologyAnimation animation)
            {
                if (animation != CollectionTopologyAnimation.OrbitOut)
                {
                    Restore();
                    return;
                }

                _binding.Set(_endPosition);
                _target.transform.localScale = _endScale;
                _target.transform.localRotation = _endRotation;
                if (_hasAlpha) _alphaBinding.SetAlpha(_endAlpha);
            }

            public void Restore()
            {
                if (_target == null) return;
                _binding.Set(InvocationPosition);
                _target.transform.localScale = InvocationScale;
                _target.transform.localRotation = InvocationRotation;
                if (_hasAlpha) _alphaBinding.Restore(InvocationAlpha);
            }
        }
    }
}
