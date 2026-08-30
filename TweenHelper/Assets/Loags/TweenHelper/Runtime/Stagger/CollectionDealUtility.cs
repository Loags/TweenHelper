using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class CollectionDealUtility
    {
        private const float MinimumTweenDuration = 0.0001f;

        public static TweenHandle CreateIn(IReadOnlyList<GameObject> targets, GameObject owner, Vector3 origin, StaggerOrder order, float rotationAccent, float startScale, float duration, float interval, bool local, TweenOptions options)
            => Create(targets, owner, origin, order, rotationAccent, startScale, duration, interval, local, options, true);

        public static TweenHandle CreateOut(IReadOnlyList<GameObject> targets, GameObject owner, Vector3 destination, StaggerOrder order, float rotationAccent, float endScale, float duration, float interval, bool local, TweenOptions options)
            => Create(targets, owner, destination, order, rotationAccent, endScale, duration, interval, local, options, false);

        private static TweenHandle Create(IReadOnlyList<GameObject> targets, GameObject owner, Vector3 anchor, StaggerOrder order, float rotationAccent, float scaleMultiplier, float duration, float interval, bool local, TweenOptions options, bool dealIn)
        {
            ValidateRequest(targets, owner, anchor, order, rotationAccent, scaleMultiplier, duration, interval, options);
            if (targets.Count == 0)
            {
                Debug.LogWarning("Collection deal: The target collection is empty.");
                return new TweenHandle(null);
            }

            float[] delays = StaggerDelayUtility.CalculateDelays(targets.Count, interval, order);
            int[] ranks = CalculateRanks(targets.Count, order);
            var states = new DealItemState[targets.Count];
            var sequence = DOTween.Sequence();
            sequence.Pause();
            float tweenDuration = Mathf.Max(duration, MinimumTweenDuration);
            Ease ease = options.Ease ?? (dealIn ? Ease.OutCubic : Ease.InCubic);

            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new DealItemState(targets[i], local, anchor, ranks[i], targets.Count, rotationAccent, scaleMultiplier, dealIn);
                DealItemState state = states[i];
                var itemSequence = DOTween.Sequence();
                itemSequence.Join(DOTween.To(() => state.PositionProgress, state.SetPositionProgress, 1f, tweenDuration).SetEase(ease));
                itemSequence.Join(DOTween.To(() => state.RotationProgress, state.SetRotationProgress, 1f, tweenDuration).SetEase(ease));
                itemSequence.Join(DOTween.To(() => state.ScaleProgress, state.SetScaleProgress, 1f, tweenDuration).SetEase(ease));
                sequence.Insert(delays[i], itemSequence);
            }

            bool initialized = false;
            bool completed = false;

            void Initialize()
            {
                if (initialized) return;
                for (int i = 0; i < states.Length; i++) states[i].CaptureAndApplyStart();
                initialized = true;
            }

            void Restore()
            {
                for (int i = 0; i < states.Length; i++) states[i].Restore();
            }

            sequence.WithDefaults(options.SetEase(Ease.Linear), owner);
            sequence.OnStart(() =>
            {
                completed = false;
                Initialize();
            });
            sequence.OnComplete(() =>
            {
                Initialize();
                completed = true;
                if (NormalizedTweenTimeline.EndsAtInvocation(options))
                {
                    Restore();
                    return;
                }

                for (int i = 0; i < states.Length; i++) states[i].Complete();
            });
            sequence.OnRewind(() =>
            {
                completed = false;
                for (int i = 0; i < states.Length; i++) states[i].Rewind();
            });
            sequence.OnKill(() =>
            {
                if (!completed) Restore();
            });
            sequence.Play();
            return new TweenHandle(sequence);
        }

        private static int[] CalculateRanks(int count, StaggerOrder order)
        {
            float[] ordering = StaggerDelayUtility.CalculateDelays(count, 1f, order);
            var indices = new int[count];
            for (int i = 0; i < count; i++) indices[i] = i;
            Array.Sort(indices, (left, right) =>
            {
                int delayComparison = ordering[left].CompareTo(ordering[right]);
                return delayComparison != 0 ? delayComparison : left.CompareTo(right);
            });

            var ranks = new int[count];
            for (int rank = 0; rank < indices.Length; rank++) ranks[indices[rank]] = rank;
            return ranks;
        }

        private static void ValidateRequest(IReadOnlyList<GameObject> targets, GameObject owner, Vector3 anchor, StaggerOrder order, float rotationAccent, float scaleMultiplier, float duration, float interval, TweenOptions options)
        {
            if (targets == null) throw new ArgumentNullException(nameof(targets));
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (!Enum.IsDefined(typeof(StaggerOrder), order)) throw new ArgumentOutOfRangeException(nameof(order));
            ValidateFinite(anchor.x, nameof(anchor));
            ValidateFinite(anchor.y, nameof(anchor));
            ValidateFinite(anchor.z, nameof(anchor));
            ValidateFinite(rotationAccent, nameof(rotationAccent));
            ValidateFinite(scaleMultiplier, nameof(scaleMultiplier));
            ValidateFinite(duration, nameof(duration));
            StaggerDelayUtility.ValidateDelay(interval, nameof(interval));
            if (scaleMultiplier < 0f) throw new ArgumentOutOfRangeException(nameof(scaleMultiplier), scaleMultiplier, "Scale must be non-negative.");
            if (duration < 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be non-negative.");
            if (options.SpeedBased == true) throw new NotSupportedException("Collection deal animations do not support speed-based timing.");

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

        private sealed class DealItemState
        {
            private readonly GameObject _target;
            private readonly bool _local;
            private readonly Vector3 _anchor;
            private readonly int _rank;
            private readonly int _count;
            private readonly float _rotationAccent;
            private readonly float _scaleMultiplier;
            private readonly bool _dealIn;
            private DestinationMotionUtility.PositionBinding _positionBinding;
            private bool _captured;
            private Vector3 _authoredPosition;
            private Quaternion _authoredRotation;
            private Vector3 _authoredScale;
            private Vector3 _startPosition;
            private Quaternion _startRotation;
            private Vector3 _startScale;
            private Vector3 _endPosition;
            private Quaternion _endRotation;
            private Vector3 _endScale;

            public DealItemState(GameObject target, bool local, Vector3 anchor, int rank, int count, float rotationAccent, float scaleMultiplier, bool dealIn)
            {
                _target = target;
                _local = local;
                _anchor = anchor;
                _rank = rank;
                _count = count;
                _rotationAccent = rotationAccent;
                _scaleMultiplier = scaleMultiplier;
                _dealIn = dealIn;
            }

            public float PositionProgress { get; private set; }
            public float RotationProgress { get; private set; }
            public float ScaleProgress { get; private set; }

            public void CaptureAndApplyStart()
            {
                Capture();
                ApplyStart();
            }

            public void SetPositionProgress(float progress)
            {
                Capture();
                PositionProgress = progress;
                _positionBinding.Set(Vector3.LerpUnclamped(_startPosition, _endPosition, progress));
            }

            public void SetRotationProgress(float progress)
            {
                Capture();
                RotationProgress = progress;
                _target.transform.localRotation = Quaternion.SlerpUnclamped(_startRotation, _endRotation, progress);
            }

            public void SetScaleProgress(float progress)
            {
                Capture();
                ScaleProgress = progress;
                _target.transform.localScale = Vector3.LerpUnclamped(_startScale, _endScale, progress);
            }

            public void Complete()
            {
                if (!_captured || _target == null) return;
                PositionProgress = 1f;
                RotationProgress = 1f;
                ScaleProgress = 1f;
                _positionBinding.Set(_endPosition);
                _target.transform.localRotation = _endRotation;
                _target.transform.localScale = _endScale;
            }

            public void Rewind()
            {
                if (!_captured || _target == null) return;
                if (_dealIn) ApplyStart();
                else Restore();
            }

            public void Restore()
            {
                if (!_captured || _target == null) return;
                PositionProgress = 0f;
                RotationProgress = 0f;
                ScaleProgress = 0f;
                _positionBinding.Set(_authoredPosition);
                _target.transform.localRotation = _authoredRotation;
                _target.transform.localScale = _authoredScale;
            }

            private void Capture()
            {
                if (_captured) return;
                _positionBinding = new DestinationMotionUtility.PositionBinding(_target, _local);
                _authoredPosition = _positionBinding.Get();
                _authoredRotation = _target.transform.localRotation;
                _authoredScale = _target.transform.localScale;
                float rotation = _count <= 1 ? 0f : Mathf.Lerp(-_rotationAccent, _rotationAccent, _rank / (float)(_count - 1));
                Quaternion rotationOffset = Quaternion.Euler(0f, 0f, rotation);
                _startPosition = _dealIn ? _anchor : _authoredPosition;
                _endPosition = _dealIn ? _authoredPosition : _anchor;
                _startRotation = _dealIn ? _authoredRotation * rotationOffset : _authoredRotation;
                _endRotation = _dealIn ? _authoredRotation : _authoredRotation * rotationOffset;
                _startScale = _dealIn ? _authoredScale * _scaleMultiplier : _authoredScale;
                _endScale = _dealIn ? _authoredScale : _authoredScale * _scaleMultiplier;
                _captured = true;
            }

            private void ApplyStart()
            {
                if (_target == null) return;
                PositionProgress = 0f;
                RotationProgress = 0f;
                ScaleProgress = 0f;
                _positionBinding.Set(_startPosition);
                _target.transform.localRotation = _startRotation;
                _target.transform.localScale = _startScale;
            }
        }
    }
}
