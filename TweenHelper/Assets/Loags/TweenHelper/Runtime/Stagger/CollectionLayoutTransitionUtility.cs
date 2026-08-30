using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper
{
    internal static class CollectionLayoutTransitionUtility
    {
        private const float MinimumTweenDuration = 0.0001f;
        private static readonly Dictionary<RectTransform, TransitionState> ActiveTransitions = new Dictionary<RectTransform, TransitionState>();

        public static CollectionLayoutSnapshot Capture(RectTransform container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var entries = new List<CollectionLayoutSnapshot.Entry>();
            for (int i = 0; i < container.childCount; i++)
            {
                if (!(container.GetChild(i) is RectTransform child) || !child.gameObject.activeInHierarchy) continue;
                ValidateFinite(child.position, $"Child '{child.name}' world position");
                ValidateFinite(child.localScale, $"Child '{child.name}' local scale");
                entries.Add(new CollectionLayoutSnapshot.Entry(child, child.position, child.localScale, child.GetSiblingIndex()));
            }

            return new CollectionLayoutSnapshot(container, entries.ToArray());
        }

        public static TweenHandle Play(RectTransform container, CollectionLayoutSnapshot snapshot, float? duration, TweenOptions options)
        {
            float resolvedDuration = ResolveDuration(duration, options);
            TransitionState currentTransition = GetCurrentTransition(container);
            LayoutGroup layoutGroup = ValidateRequest(container, snapshot, options, currentTransition);
            if (snapshot.Entries.Count == 0) return new TweenHandle(null);

            currentTransition?.KillAndSettle();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);

            var childStates = new ChildState[snapshot.Entries.Count];
            for (int i = 0; i < childStates.Length; i++)
            {
                CollectionLayoutSnapshot.Entry entry = snapshot.Entries[i];
                RectTransform child = entry.Child;
                Vector2 finalAnchoredPosition = child.anchoredPosition;
                Vector3 finalLocalPosition = child.localPosition;
                Vector3 finalScale = child.localScale;
                Vector3 capturedLocalPosition = container.InverseTransformPoint(entry.WorldPosition);
                Vector3 localDifference = capturedLocalPosition - finalLocalPosition;
                Vector2 capturedAnchoredPosition = finalAnchoredPosition + new Vector2(localDifference.x, localDifference.y);
                ValidateFinite(capturedAnchoredPosition, $"Child '{child.name}' captured anchored position");
                ValidateFinite(finalAnchoredPosition, $"Child '{child.name}' final anchored position");
                ValidateFinite(finalScale, $"Child '{child.name}' final local scale");
                childStates[i] = new ChildState(child, capturedAnchoredPosition, entry.LocalScale, finalAnchoredPosition, finalScale);
            }

            var state = new TransitionState(container, layoutGroup, childStates);
            Sequence sequence = null;

            try
            {
                layoutGroup.enabled = false;
                state.ApplyCapturedState();

                sequence = DOTween.Sequence();
                sequence.Pause();
                sequence.SetRecyclable(false);
                float tweenDuration = Mathf.Max(resolvedDuration, MinimumTweenDuration);
                Ease ease = options.Ease ?? TweenHelperSettings.Instance.DefaultEase;
                bool snapping = options.Snapping ?? TweenHelperSettings.Instance.DefaultSnapping;

                for (int i = 0; i < childStates.Length; i++)
                {
                    ChildState childState = childStates[i];
                    sequence.Join(childState.Child.DOAnchorPos(childState.FinalAnchoredPosition, tweenDuration, snapping).SetEase(ease));
                    if (childState.CapturedScale != childState.FinalScale)
                    {
                        sequence.Join(childState.Child.DOScale(childState.FinalScale, tweenDuration).SetEase(ease));
                    }
                }

                sequence.WithDefaults(options.SetEase(Ease.Linear), container.gameObject);
                state.SetSequence(sequence);
                sequence.OnComplete(state.SettleFinalState);
                sequence.OnKill(() =>
                {
                    state.SettleFinalState();
                    RemoveTransition(state);
                });

                ActiveTransitions[container] = state;
                sequence.Play();
                return new TweenHandle(sequence);
            }
            catch
            {
                if (sequence != null && sequence.IsActive()) sequence.Kill();
                state.SettleFinalState();
                RemoveTransition(state);
                throw;
            }
        }

        private static LayoutGroup ValidateRequest(RectTransform container, CollectionLayoutSnapshot snapshot, TweenOptions options, TransitionState currentTransition)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.Container == null) throw new ArgumentException("The snapshot container has been destroyed.", nameof(snapshot));
            if (snapshot.Container != container) throw new ArgumentException("The snapshot was captured from a different container.", nameof(snapshot));
            if (options.SpeedBased == true) throw new NotSupportedException("Collection layout transitions do not support speed-based timing.");

            var currentChildren = new HashSet<RectTransform>();
            for (int i = 0; i < container.childCount; i++)
            {
                if (container.GetChild(i) is RectTransform child && child.gameObject.activeInHierarchy) currentChildren.Add(child);
            }

            if (currentChildren.Count != snapshot.Entries.Count)
            {
                throw new ArgumentException("The container's active direct-child membership changed after the snapshot was captured.", nameof(snapshot));
            }

            var capturedChildren = new HashSet<RectTransform>();
            for (int i = 0; i < snapshot.Entries.Count; i++)
            {
                CollectionLayoutSnapshot.Entry entry = snapshot.Entries[i];
                RectTransform child = entry.Child;
                if (child == null) throw new ArgumentException($"The captured child at index {i} was destroyed.", nameof(snapshot));
                if (child.parent != container) throw new ArgumentException($"Captured child '{child.name}' is no longer a direct child of the container.", nameof(snapshot));
                if (!child.gameObject.activeInHierarchy) throw new ArgumentException($"Captured child '{child.name}' is no longer active.", nameof(snapshot));
                if (!capturedChildren.Add(child)) throw new ArgumentException($"Captured child '{child.name}' occurs more than once.", nameof(snapshot));
                if (!currentChildren.Contains(child)) throw new ArgumentException($"Captured child '{child.name}' is no longer in the container's active direct-child set.", nameof(snapshot));
                ValidateFinite(entry.WorldPosition, $"Captured child '{child.name}' world position");
                ValidateFinite(entry.LocalScale, $"Captured child '{child.name}' local scale");
            }

            LayoutGroup[] layoutGroups = container.GetComponents<LayoutGroup>();
            LayoutGroup resolvedLayoutGroup = null;
            int enabledCount = 0;
            for (int i = 0; i < layoutGroups.Length; i++)
            {
                LayoutGroup candidate = layoutGroups[i];
                bool ownedByCurrentTransition = currentTransition != null && currentTransition.LayoutGroup == candidate;
                if (!candidate.enabled && !ownedByCurrentTransition) continue;
                enabledCount++;
                resolvedLayoutGroup = candidate;
            }

            if (enabledCount > 1) throw new InvalidOperationException("Collection layout transitions require exactly one enabled LayoutGroup, but multiple enabled groups were found.");
            if (enabledCount == 0) throw new InvalidOperationException("Collection layout transitions require one enabled HorizontalLayoutGroup, VerticalLayoutGroup, or GridLayoutGroup.");
            if (!(resolvedLayoutGroup is HorizontalLayoutGroup) && !(resolvedLayoutGroup is VerticalLayoutGroup) && !(resolvedLayoutGroup is GridLayoutGroup))
            {
                throw new NotSupportedException($"Layout group type '{resolvedLayoutGroup.GetType().Name}' is not supported by collection layout transitions.");
            }

            return resolvedLayoutGroup;
        }

        private static float ResolveDuration(float? duration, TweenOptions options)
        {
            float resolved = duration ?? options.Duration ?? TweenDefaults.DefaultDuration;
            if (float.IsNaN(resolved) || float.IsInfinity(resolved) || resolved < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), resolved, "Duration must be finite and non-negative.");
            }

            return resolved;
        }

        private static TransitionState GetCurrentTransition(RectTransform container)
        {
            if (container == null) return null;
            ActiveTransitions.TryGetValue(container, out TransitionState state);
            return state;
        }

        private static void RemoveTransition(TransitionState state)
        {
            if (ReferenceEquals(state.Container, null)) return;
            if (ActiveTransitions.TryGetValue(state.Container, out TransitionState current) && ReferenceEquals(current, state)) ActiveTransitions.Remove(state.Container);
        }

        private static void ValidateFinite(Vector2 value, string description)
        {
            ValidateFinite(value.x, description);
            ValidateFinite(value.y, description);
        }

        private static void ValidateFinite(Vector3 value, string description)
        {
            ValidateFinite(value.x, description);
            ValidateFinite(value.y, description);
            ValidateFinite(value.z, description);
        }

        private static void ValidateFinite(float value, string description)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidOperationException($"{description} must be finite.");
        }

        private readonly struct ChildState
        {
            public ChildState(RectTransform child, Vector2 capturedAnchoredPosition, Vector3 capturedScale, Vector2 finalAnchoredPosition, Vector3 finalScale)
            {
                Child = child;
                CapturedAnchoredPosition = capturedAnchoredPosition;
                CapturedScale = capturedScale;
                FinalAnchoredPosition = finalAnchoredPosition;
                FinalScale = finalScale;
            }

            public RectTransform Child { get; }
            public Vector2 CapturedAnchoredPosition { get; }
            public Vector3 CapturedScale { get; }
            public Vector2 FinalAnchoredPosition { get; }
            public Vector3 FinalScale { get; }
        }

        private sealed class TransitionState
        {
            private readonly ChildState[] _children;
            private Sequence _sequence;
            private bool _settled;

            public TransitionState(RectTransform container, LayoutGroup layoutGroup, ChildState[] children)
            {
                Container = container;
                LayoutGroup = layoutGroup;
                _children = children;
            }

            public RectTransform Container { get; }
            public LayoutGroup LayoutGroup { get; }

            public void SetSequence(Sequence sequence) => _sequence = sequence;

            public void ApplyCapturedState()
            {
                _settled = false;
                for (int i = 0; i < _children.Length; i++)
                {
                    ChildState child = _children[i];
                    child.Child.anchoredPosition = child.CapturedAnchoredPosition;
                    child.Child.localScale = child.CapturedScale;
                }
            }

            public void SettleFinalState()
            {
                if (_settled) return;

                for (int i = 0; i < _children.Length; i++)
                {
                    ChildState child = _children[i];
                    if (child.Child == null) continue;
                    child.Child.anchoredPosition = child.FinalAnchoredPosition;
                    child.Child.localScale = child.FinalScale;
                }

                if (LayoutGroup != null) LayoutGroup.enabled = true;
                if (Container != null)
                {
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(Container);
                }

                _settled = true;
            }

            public void KillAndSettle()
            {
                if (_sequence != null && _sequence.IsActive()) _sequence.Kill();
                else
                {
                    SettleFinalState();
                    RemoveTransition(this);
                }
            }
        }
    }
}
