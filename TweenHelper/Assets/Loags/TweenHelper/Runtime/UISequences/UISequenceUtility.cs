using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal static class UISequenceUtility
    {
        public static Tween CreateToast(GameObject target, bool show, UISequenceDirection direction, float distance, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, distance, direction, options);
            float strength = ResolveStrength(options);
            Vector3 movement = DirectionVector(direction) * distance * strength;
            var state = new UISequenceState(target);
            UISequencePose start = default;
            UISequencePose end = default;

            void Initialize()
            {
                state.Initialize();
                if (show)
                {
                    start = ResolveShowStart(state, -movement, UniformScale(state.Shown.Scale, 1f - 0.04f * strength));
                    end = state.Shown;
                }
                else
                {
                    start = state.Invocation;
                    end = HiddenPose(state.Shown, movement, UniformScale(state.Shown.Scale, 1f - 0.03f * strength));
                }
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                if (show)
                {
                    Vector3 overshootPosition = end.Position + DirectionVector(direction) * 6f * strength;
                    Vector3 overshootScale = UniformScale(end.Scale, 1f + 0.015f * strength);
                    Vector3 position = progress <= 0.78f
                        ? Vector3.LerpUnclamped(start.Position, overshootPosition, EaseValue(progress / 0.78f, options.Ease ?? Ease.OutCubic))
                        : Vector3.LerpUnclamped(overshootPosition, end.Position, EaseValue((progress - 0.78f) / 0.22f, Ease.OutSine));
                    Vector3 scale = progress <= 0.72f
                        ? Vector3.LerpUnclamped(start.Scale, overshootScale, EaseValue(progress / 0.72f, Ease.OutCubic))
                        : Vector3.LerpUnclamped(overshootScale, end.Scale, EaseValue((progress - 0.72f) / 0.28f, Ease.OutBack));
                    float alpha = Mathf.LerpUnclamped(start.Alpha, end.Alpha, EaseValue(progress / 0.68f, Ease.OutQuad));
                    state.Apply(new UISequencePose(position, scale, Quaternion.SlerpUnclamped(start.Rotation, end.Rotation, EaseValue(progress, Ease.OutCubic)), alpha));
                    return;
                }

                Vector3 anticipationPosition = start.Position - DirectionVector(direction) * 6f * strength;
                Vector3 anticipationScale = UniformScale(state.Shown.Scale, 1f + 0.012f * strength);
                Vector3 hidePosition = progress <= 0.16f
                    ? Vector3.LerpUnclamped(start.Position, anticipationPosition, EaseValue(progress / 0.16f, Ease.OutQuad))
                    : Vector3.LerpUnclamped(anticipationPosition, end.Position, EaseValue((progress - 0.16f) / 0.84f, options.Ease ?? Ease.InCubic));
                Vector3 hideScale = progress <= 0.16f
                    ? Vector3.LerpUnclamped(start.Scale, anticipationScale, EaseValue(progress / 0.16f, Ease.OutQuad))
                    : Vector3.LerpUnclamped(anticipationScale, end.Scale, EaseValue((progress - 0.16f) / 0.84f, Ease.InQuad));
                float hideAlpha = Mathf.LerpUnclamped(start.Alpha, end.Alpha, Progress(progress, 0.1f, 0.9f, Ease.InQuad));
                state.Apply(new UISequencePose(hidePosition, hideScale, Quaternion.SlerpUnclamped(start.Rotation, end.Rotation, progress), hideAlpha));
            }

            return CreateTimeline(target, duration, options, Initialize, Evaluate, () => state.Apply(end), state.RestoreInvocation);
        }

        public static Tween CreateTooltip(GameObject target, bool show, UISequenceDirection direction, float distance, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration, distance, direction, options);
            float strength = ResolveStrength(options);
            Vector3 movement = DirectionVector(direction) * distance * strength;
            var state = new UISequenceState(target);
            UISequencePose start = default;
            UISequencePose end = default;

            void Initialize()
            {
                state.Initialize();
                if (show)
                {
                    start = ResolveShowStart(state, -movement, UniformScale(state.Shown.Scale, 1f - 0.04f * strength));
                    end = state.Shown;
                }
                else
                {
                    start = state.Invocation;
                    end = HiddenPose(state.Shown, movement, UniformScale(state.Shown.Scale, 1f - 0.02f * strength));
                }
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                Ease movementEase = options.Ease ?? (show ? Ease.OutCubic : Ease.InCubic);
                float positionProgress = EaseValue(progress, movementEase);
                float scaleProgress = EaseValue(progress, show ? Ease.OutBack : Ease.InQuad);
                float alphaProgress = EaseValue(progress, show ? Ease.OutQuad : Ease.InQuad);
                ApplyInterpolated(state, start, end, positionProgress, scaleProgress, alphaProgress, positionProgress);
            }

            return CreateTimeline(target, duration, options, Initialize, Evaluate, () => state.Apply(end), state.RestoreInvocation);
        }

        public static Tween CreateModal(GameObject panel, GameObject backdrop, IReadOnlyList<GameObject> controls, bool show, float duration, float childStagger, TweenOptions options)
        {
            ValidateRequest(panel, duration, options);
            ValidateStagger(childStagger);
            if (backdrop == panel) throw new ArgumentException("Modal backdrop and panel must be different objects.", nameof(backdrop));
            ValidateLiveTargets(controls, nameof(controls));

            float strength = ResolveStrength(options);
            float childDuration = duration * (show ? 0.48f : 0.42f);
            float firstChildStart = show ? duration * 0.36f : 0f;
            float finalChildEnd = controls.Count == 0 ? 0f : firstChildStart + (controls.Count - 1) * childStagger + childDuration;
            float totalDuration = Mathf.Max(duration, finalChildEnd);
            var panelState = new UISequenceState(panel);
            var backdropState = backdrop == null ? null : new UISequenceState(backdrop);
            var controlStates = CreateStates(controls);
            UISequencePose panelStart = default;
            UISequencePose panelEnd = default;
            UISequencePose backdropStart = default;
            UISequencePose backdropEnd = default;
            var controlStarts = new UISequencePose[controls.Count];
            var controlEnds = new UISequencePose[controls.Count];

            void Initialize()
            {
                panelState.Initialize();
                backdropState?.Initialize();
                InitializeStates(controlStates);
                if (backdropState != null && !backdropState.HasAlpha) throw new InvalidOperationException($"Modal backdrop '{backdrop.name}' requires a CanvasGroup or supported Graphic alpha target.");

                if (show)
                {
                    panelStart = ResolveShowStart(panelState, Vector3.zero, UniformScale(panelState.Shown.Scale, 1f - 0.12f * strength));
                    panelEnd = panelState.Shown;
                    if (backdropState != null)
                    {
                        backdropStart = ResolveFadeInStart(backdropState);
                        backdropEnd = backdropState.Shown;
                    }

                    for (int i = 0; i < controlStates.Length; i++)
                    {
                        controlStarts[i] = ResolveShowStart(controlStates[i], Vector3.down * 12f * strength, UniformScale(controlStates[i].Shown.Scale, 1f - 0.04f * strength));
                        controlEnds[i] = controlStates[i].Shown;
                    }
                    return;
                }

                panelStart = panelState.Invocation;
                panelEnd = HiddenPose(panelState.Shown, Vector3.zero, UniformScale(panelState.Shown.Scale, 1f - 0.1f * strength));
                if (backdropState != null)
                {
                    backdropStart = backdropState.Invocation;
                    backdropEnd = HiddenPose(backdropState.Shown, Vector3.zero, backdropState.Shown.Scale);
                }

                for (int i = 0; i < controlStates.Length; i++)
                {
                    controlStarts[i] = controlStates[i].Invocation;
                    controlEnds[i] = HiddenPose(controlStates[i].Shown, Vector3.down * 10f * strength, UniformScale(controlStates[i].Shown.Scale, 1f - 0.04f * strength));
                }
            }

            void Evaluate(float time)
            {
                if (show) EvaluateModalOpen(time, duration, childStagger, panelState, backdropState, controlStates, panelStart, panelEnd, backdropStart, backdropEnd, controlStarts, controlEnds, options, strength);
                else EvaluateModalClose(time, duration, childStagger, panelState, backdropState, controlStates, panelStart, panelEnd, backdropStart, backdropEnd, controlStarts, controlEnds, options, strength);
            }

            void Complete()
            {
                panelState.Apply(panelEnd);
                if (backdropState != null) backdropState.Apply(backdropEnd);
                ApplyEndpoints(controlStates, controlEnds);
            }

            void Rewind()
            {
                panelState.RestoreInvocation();
                backdropState?.RestoreInvocation();
                RestoreInvocations(controlStates);
            }

            return CreateTimeline(panel, totalDuration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static Tween CreateDropdown(GameObject panel, IReadOnlyList<GameObject> entries, bool show, float duration, float childStagger, TweenOptions options)
        {
            ValidateRequest(panel, duration, options);
            ValidateStagger(childStagger);
            ValidateLiveTargets(entries, nameof(entries));

            float strength = ResolveStrength(options);
            float childDuration = duration * (show ? 0.52f : 0.44f);
            float firstChildStart = show ? duration * 0.3f : 0f;
            float finalChildEnd = entries.Count == 0 ? 0f : firstChildStart + (entries.Count - 1) * childStagger + childDuration;
            float totalDuration = Mathf.Max(duration, finalChildEnd);
            var panelState = new UISequenceState(panel);
            var entryStates = CreateStates(entries);
            UISequencePose panelStart = default;
            UISequencePose panelEnd = default;
            var entryStarts = new UISequencePose[entries.Count];
            var entryEnds = new UISequencePose[entries.Count];

            void Initialize()
            {
                panelState.Initialize();
                InitializeStates(entryStates);
                Vector3 compressedScale = CompressY(panelState.Shown.Scale, 1f - 0.14f * strength);
                if (show)
                {
                    panelStart = ResolveShowStart(panelState, Vector3.zero, compressedScale);
                    panelEnd = panelState.Shown;
                    for (int i = 0; i < entryStates.Length; i++)
                    {
                        entryStarts[i] = ResolveShowStart(entryStates[i], Vector3.up * 8f * strength, UniformScale(entryStates[i].Shown.Scale, 1f - 0.025f * strength));
                        entryEnds[i] = entryStates[i].Shown;
                    }
                    return;
                }

                panelStart = panelState.Invocation;
                panelEnd = HiddenPose(panelState.Shown, Vector3.zero, compressedScale);
                for (int i = 0; i < entryStates.Length; i++)
                {
                    entryStarts[i] = entryStates[i].Invocation;
                    entryEnds[i] = HiddenPose(entryStates[i].Shown, Vector3.up * 8f * strength, UniformScale(entryStates[i].Shown.Scale, 1f - 0.025f * strength));
                }
            }

            void Evaluate(float time)
            {
                float panelProgress = Mathf.Clamp01(time / duration);
                ApplyInterpolated(panelState, panelStart, panelEnd, panelProgress, EaseValue(panelProgress, options.Ease ?? (show ? Ease.OutBack : Ease.InCubic)), EaseValue(panelProgress, show ? Ease.OutQuad : Ease.InQuad), panelProgress);

                for (int i = 0; i < entryStates.Length; i++)
                {
                    int order = show ? i : entryStates.Length - 1 - i;
                    float startTime = firstChildStart + order * childStagger;
                    float progress = Mathf.Clamp01((time - startTime) / childDuration);
                    ApplyInterpolated(entryStates[i], entryStarts[i], entryEnds[i], EaseValue(progress, show ? Ease.OutCubic : Ease.InCubic), EaseValue(progress, show ? Ease.OutBack : Ease.InQuad), EaseValue(progress, show ? Ease.OutQuad : Ease.InQuad), progress);
                }
            }

            void Complete()
            {
                panelState.Apply(panelEnd);
                ApplyEndpoints(entryStates, entryEnds);
            }

            void Rewind()
            {
                panelState.RestoreInvocation();
                RestoreInvocations(entryStates);
            }

            return CreateTimeline(panel, totalDuration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static Tween CreateTabSwitch(GameObject outgoing, GameObject incoming, UISequenceDirection direction, float distance, float duration, TweenOptions options)
        {
            ValidateRequest(outgoing, duration, distance, direction, options);
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            if (incoming == outgoing) throw new ArgumentException("Outgoing and incoming tab content must be different objects.", nameof(incoming));
            ValidateRectTransform(incoming, nameof(incoming));

            float strength = ResolveStrength(options);
            Vector3 movement = DirectionVector(direction) * distance * strength;
            var outgoingState = new UISequenceState(outgoing);
            var incomingState = new UISequenceState(incoming);
            UISequencePose outgoingStart = default;
            UISequencePose outgoingEnd = default;
            UISequencePose incomingStart = default;
            UISequencePose incomingEnd = default;

            void Initialize()
            {
                outgoingState.Initialize();
                incomingState.Initialize();
                outgoingStart = outgoingState.Invocation;
                outgoingEnd = HiddenPose(outgoingState.Shown, movement, UniformScale(outgoingState.Shown.Scale, 1f - 0.015f * strength));
                incomingStart = ResolveShowStart(incomingState, -movement, UniformScale(incomingState.Shown.Scale, 1f - 0.015f * strength));
                incomingEnd = incomingState.Shown;
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                float outgoingProgress = Progress(progress, 0f, 0.82f, options.Ease ?? Ease.InCubic);
                float incomingProgress = Progress(progress, 0.18f, 0.82f, options.Ease ?? Ease.OutCubic);
                ApplyInterpolated(outgoingState, outgoingStart, outgoingEnd, outgoingProgress, EaseValue(outgoingProgress, Ease.InQuad), EaseValue(outgoingProgress, Ease.InQuad), outgoingProgress);
                ApplyInterpolated(incomingState, incomingStart, incomingEnd, incomingProgress, EaseValue(incomingProgress, Ease.OutBack), EaseValue(incomingProgress, Ease.OutQuad), incomingProgress);
            }

            void Complete()
            {
                outgoingState.Apply(outgoingEnd);
                incomingState.Apply(incomingEnd);
            }

            void Rewind()
            {
                outgoingState.RestoreInvocation();
                incomingState.RestoreInvocation();
            }

            return CreateTimeline(outgoing, duration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static Tween CreateDrawer(GameObject panel, GameObject backdrop, bool show, UISequenceDirection edge, float distance, float duration, TweenOptions options)
        {
            ValidateRequest(panel, duration, distance, edge, options);
            ValidateBackdrop(panel, backdrop);
            float strength = ResolveStrength(options);
            Vector3 movement = DirectionVector(edge) * distance * strength;
            var panelState = new UISequenceState(panel);
            var backdropState = backdrop == null ? null : new UISequenceState(backdrop);
            UISequencePose panelStart = default;
            UISequencePose panelEnd = default;
            UISequencePose backdropStart = default;
            UISequencePose backdropEnd = default;

            void Initialize()
            {
                panelState.Initialize();
                backdropState?.Initialize();
                RequireBackdropAlpha(backdrop, backdropState);
                if (show)
                {
                    panelStart = ResolveShowStart(panelState, movement, UniformScale(panelState.Shown.Scale, 1f - 0.025f * strength));
                    panelEnd = panelState.Shown;
                    if (backdropState != null)
                    {
                        backdropStart = ResolveFadeInStart(backdropState);
                        backdropEnd = backdropState.Shown;
                    }
                    return;
                }

                panelStart = panelState.Invocation;
                panelEnd = HiddenPose(panelState.Shown, movement, UniformScale(panelState.Shown.Scale, 1f - 0.025f * strength));
                if (backdropState != null)
                {
                    backdropStart = backdropState.Invocation;
                    backdropEnd = HiddenPose(backdropState.Shown, Vector3.zero, backdropState.Shown.Scale);
                }
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                float positionProgress = EaseValue(progress, options.Ease ?? (show ? Ease.OutCubic : Ease.InCubic));
                float scaleProgress = EaseValue(progress, show ? Ease.OutBack : Ease.InQuad);
                float alphaProgress = EaseValue(progress, show ? Ease.OutQuad : Ease.InQuad);
                ApplyInterpolated(panelState, panelStart, panelEnd, positionProgress, scaleProgress, alphaProgress, positionProgress);
                if (backdropState != null)
                {
                    float backdropProgress = Progress(progress, show ? 0f : 0.12f, show ? 0.72f : 0.88f, show ? Ease.OutQuad : Ease.InQuad);
                    ApplyInterpolated(backdropState, backdropStart, backdropEnd, backdropProgress, backdropProgress, backdropProgress, backdropProgress);
                }
            }

            void Complete()
            {
                panelState.Apply(panelEnd);
                if (backdropState != null) backdropState.Apply(backdropEnd);
            }

            void Rewind()
            {
                panelState.RestoreInvocation();
                backdropState?.RestoreInvocation();
            }

            return CreateTimeline(panel, duration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static Tween CreateBottomSheet(GameObject panel, GameObject backdrop, bool show, float distance, float duration, TweenOptions options)
        {
            ValidateRequest(panel, duration, distance, UISequenceDirection.Down, options);
            ValidateBackdrop(panel, backdrop);
            float strength = ResolveStrength(options);
            Vector3 movement = Vector3.down * distance * strength;
            var panelState = new UISequenceState(panel);
            var backdropState = backdrop == null ? null : new UISequenceState(backdrop);
            UISequencePose panelStart = default;
            UISequencePose panelEnd = default;
            UISequencePose backdropStart = default;
            UISequencePose backdropEnd = default;

            void Initialize()
            {
                panelState.Initialize();
                backdropState?.Initialize();
                RequireBackdropAlpha(backdrop, backdropState);
                if (show)
                {
                    panelStart = ResolveShowStart(panelState, movement, CompressY(panelState.Shown.Scale, 1f - 0.055f * strength));
                    panelEnd = panelState.Shown;
                    if (backdropState != null)
                    {
                        backdropStart = ResolveFadeInStart(backdropState);
                        backdropEnd = backdropState.Shown;
                    }
                    return;
                }

                panelStart = panelState.Invocation;
                panelEnd = HiddenPose(panelState.Shown, movement, CompressY(panelState.Shown.Scale, 1f - 0.055f * strength));
                if (backdropState != null)
                {
                    backdropStart = backdropState.Invocation;
                    backdropEnd = HiddenPose(backdropState.Shown, Vector3.zero, backdropState.Shown.Scale);
                }
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                Vector3 position;
                Vector3 scale;
                if (show)
                {
                    Vector3 overshootPosition = panelEnd.Position + Vector3.up * 12f * strength;
                    position = progress <= 0.82f
                        ? Vector3.LerpUnclamped(panelStart.Position, overshootPosition, EaseValue(progress / 0.82f, options.Ease ?? Ease.OutCubic))
                        : Vector3.LerpUnclamped(overshootPosition, panelEnd.Position, EaseValue((progress - 0.82f) / 0.18f, Ease.OutSine));
                    Vector3 overshootScale = UniformScale(panelEnd.Scale, 1f + 0.018f * strength);
                    scale = progress <= 0.76f
                        ? Vector3.LerpUnclamped(panelStart.Scale, overshootScale, EaseValue(progress / 0.76f, Ease.OutCubic))
                        : Vector3.LerpUnclamped(overshootScale, panelEnd.Scale, EaseValue((progress - 0.76f) / 0.24f, Ease.OutBack));
                }
                else
                {
                    Vector3 anticipation = panelStart.Position + Vector3.up * 9f * strength;
                    position = progress <= 0.14f
                        ? Vector3.LerpUnclamped(panelStart.Position, anticipation, EaseValue(progress / 0.14f, Ease.OutQuad))
                        : Vector3.LerpUnclamped(anticipation, panelEnd.Position, EaseValue((progress - 0.14f) / 0.86f, options.Ease ?? Ease.InCubic));
                    scale = Vector3.LerpUnclamped(panelStart.Scale, panelEnd.Scale, EaseValue(progress, Ease.InQuad));
                }

                float alpha = Mathf.LerpUnclamped(panelStart.Alpha, panelEnd.Alpha, EaseValue(progress, show ? Ease.OutQuad : Ease.InQuad));
                panelState.Apply(new UISequencePose(position, scale, Quaternion.SlerpUnclamped(panelStart.Rotation, panelEnd.Rotation, progress), alpha));
                if (backdropState != null)
                {
                    float backdropProgress = Progress(progress, show ? 0f : 0.16f, show ? 0.7f : 0.84f, show ? Ease.OutQuad : Ease.InQuad);
                    ApplyInterpolated(backdropState, backdropStart, backdropEnd, backdropProgress, backdropProgress, backdropProgress, backdropProgress);
                }
            }

            void Complete()
            {
                panelState.Apply(panelEnd);
                if (backdropState != null) backdropState.Apply(backdropEnd);
            }

            void Rewind()
            {
                panelState.RestoreInvocation();
                backdropState?.RestoreInvocation();
            }

            return CreateTimeline(panel, duration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static Tween CreatePagePush(GameObject outgoing, GameObject incoming, UISequenceDirection direction, float distance, float duration, TweenOptions options)
        {
            ValidateRequest(outgoing, duration, distance, direction, options);
            ValidateIncoming(outgoing, incoming);
            float strength = ResolveStrength(options);
            Vector3 movement = DirectionVector(direction) * distance * strength;
            var outgoingState = new UISequenceState(outgoing);
            var incomingState = new UISequenceState(incoming);
            UISequencePose outgoingStart = default;
            UISequencePose outgoingEnd = default;
            UISequencePose incomingStart = default;
            UISequencePose incomingEnd = default;

            void Initialize()
            {
                outgoingState.Initialize();
                incomingState.Initialize();
                outgoingStart = outgoingState.Invocation;
                outgoingEnd = HiddenPose(outgoingState.Shown, movement, outgoingState.Shown.Scale);
                incomingStart = ResolveShowStart(incomingState, -movement, incomingState.Shown.Scale);
                incomingEnd = incomingState.Shown;
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                float outgoingProgress = Progress(progress, 0f, 0.9f, options.Ease ?? Ease.InOutCubic);
                float incomingProgress = Progress(progress, 0.1f, 0.9f, options.Ease ?? Ease.InOutCubic);
                ApplyInterpolated(outgoingState, outgoingStart, outgoingEnd, outgoingProgress, outgoingProgress, EaseValue(outgoingProgress, Ease.InQuad), outgoingProgress);
                ApplyInterpolated(incomingState, incomingStart, incomingEnd, incomingProgress, incomingProgress, EaseValue(incomingProgress, Ease.OutQuad), incomingProgress);
            }

            void Complete()
            {
                outgoingState.Apply(outgoingEnd);
                incomingState.Apply(incomingEnd);
            }

            void Rewind()
            {
                outgoingState.RestoreInvocation();
                incomingState.RestoreInvocation();
            }

            return CreateTimeline(outgoing, duration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static Tween CreatePageCrossFade(GameObject outgoing, GameObject incoming, float depthScale, float duration, TweenOptions options)
        {
            ValidateRequest(outgoing, duration, options);
            ValidateIncoming(outgoing, incoming);
            ValidateFinite(depthScale, nameof(depthScale));
            if (depthScale < 0f || depthScale >= 1f) throw new ArgumentOutOfRangeException(nameof(depthScale), depthScale, "Depth scale must be at least zero and less than one.");
            float strength = ResolveStrength(options);
            float scaleOffset = depthScale * strength;
            var outgoingState = new UISequenceState(outgoing);
            var incomingState = new UISequenceState(incoming);
            UISequencePose outgoingStart = default;
            UISequencePose outgoingEnd = default;
            UISequencePose incomingStart = default;
            UISequencePose incomingEnd = default;

            void Initialize()
            {
                outgoingState.Initialize();
                incomingState.Initialize();
                outgoingStart = outgoingState.Invocation;
                outgoingEnd = HiddenPose(outgoingState.Shown, Vector3.zero, UniformScale(outgoingState.Shown.Scale, 1f - scaleOffset));
                incomingStart = ResolveShowStart(incomingState, Vector3.zero, UniformScale(incomingState.Shown.Scale, 1f + scaleOffset));
                incomingEnd = incomingState.Shown;
            }

            void Evaluate(float time)
            {
                float progress = Mathf.Clamp01(time / duration);
                float outgoingProgress = Progress(progress, 0f, 0.76f, options.Ease ?? Ease.InOutSine);
                float incomingProgress = Progress(progress, 0.18f, 0.82f, options.Ease ?? Ease.InOutSine);
                ApplyInterpolated(outgoingState, outgoingStart, outgoingEnd, outgoingProgress, EaseValue(outgoingProgress, Ease.InQuad), EaseValue(outgoingProgress, Ease.InQuad), outgoingProgress);
                ApplyInterpolated(incomingState, incomingStart, incomingEnd, incomingProgress, EaseValue(incomingProgress, Ease.OutCubic), EaseValue(incomingProgress, Ease.OutQuad), incomingProgress);
            }

            void Complete()
            {
                outgoingState.Apply(outgoingEnd);
                incomingState.Apply(incomingEnd);
            }

            void Rewind()
            {
                outgoingState.RestoreInvocation();
                incomingState.RestoreInvocation();
            }

            return CreateTimeline(outgoing, duration, options, Initialize, Evaluate, Complete, Rewind);
        }

        public static IReadOnlyList<GameObject> SnapshotTargets(IEnumerable<GameObject> targets, string parameterName, params GameObject[] excluded)
        {
            if (targets == null) return Array.Empty<GameObject>();
            var result = new List<GameObject>();
            var unique = new HashSet<GameObject>();
            var excludedTargets = new HashSet<GameObject>();
            for (int i = 0; i < excluded.Length; i++)
            {
                if (excluded[i] != null) excludedTargets.Add(excluded[i]);
            }

            int index = 0;
            foreach (GameObject target in targets)
            {
                if (target == null) throw new ArgumentException($"Target at index {index} is null.", parameterName);
                if (excludedTargets.Contains(target)) throw new ArgumentException($"Target '{target.name}' cannot also be the sequence owner or backdrop.", parameterName);
                if (!unique.Add(target)) throw new ArgumentException($"Target '{target.name}' occurs more than once.", parameterName);
                result.Add(target);
                index++;
            }

            return result;
        }

        private static void EvaluateModalOpen(float time, float duration, float childStagger, UISequenceState panel, UISequenceState backdrop, UISequenceState[] controls, UISequencePose panelStart, UISequencePose panelEnd, UISequencePose backdropStart, UISequencePose backdropEnd, UISequencePose[] controlStarts, UISequencePose[] controlEnds, TweenOptions options, float strength)
        {
            float panelProgress = Mathf.Clamp01(time / duration);
            Vector3 overshootScale = UniformScale(panelEnd.Scale, 1f + 0.04f * strength);
            Vector3 panelScale = panelProgress <= 0.72f
                ? Vector3.LerpUnclamped(panelStart.Scale, overshootScale, EaseValue(panelProgress / 0.72f, options.Ease ?? Ease.OutCubic))
                : Vector3.LerpUnclamped(overshootScale, panelEnd.Scale, EaseValue((panelProgress - 0.72f) / 0.28f, Ease.OutBack));
            Vector3 panelPosition = Vector3.LerpUnclamped(panelStart.Position, panelEnd.Position, EaseValue(panelProgress, Ease.OutCubic));
            float panelAlpha = Mathf.LerpUnclamped(panelStart.Alpha, panelEnd.Alpha, Progress(panelProgress, 0f, 0.58f, Ease.OutQuad));
            panel.Apply(new UISequencePose(panelPosition, panelScale, Quaternion.SlerpUnclamped(panelStart.Rotation, panelEnd.Rotation, panelProgress), panelAlpha));

            if (backdrop != null)
            {
                float backdropProgress = Mathf.Clamp01(time / (duration * 0.72f));
                ApplyInterpolated(backdrop, backdropStart, backdropEnd, backdropProgress, backdropProgress, EaseValue(backdropProgress, Ease.OutQuad), backdropProgress);
            }

            float childDuration = duration * 0.48f;
            for (int i = 0; i < controls.Length; i++)
            {
                float startTime = duration * 0.36f + i * childStagger;
                float progress = Mathf.Clamp01((time - startTime) / childDuration);
                ApplyInterpolated(controls[i], controlStarts[i], controlEnds[i], EaseValue(progress, Ease.OutCubic), EaseValue(progress, Ease.OutBack), EaseValue(progress, Ease.OutQuad), progress);
            }
        }

        private static void EvaluateModalClose(float time, float duration, float childStagger, UISequenceState panel, UISequenceState backdrop, UISequenceState[] controls, UISequencePose panelStart, UISequencePose panelEnd, UISequencePose backdropStart, UISequencePose backdropEnd, UISequencePose[] controlStarts, UISequencePose[] controlEnds, TweenOptions options, float strength)
        {
            float panelProgress = Mathf.Clamp01((time - duration * 0.08f) / (duration * 0.92f));
            Vector3 anticipationScale = UniformScale(panel.Shown.Scale, 1f + 0.015f * strength);
            Vector3 panelScale = panelProgress <= 0.14f
                ? Vector3.LerpUnclamped(panelStart.Scale, anticipationScale, EaseValue(panelProgress / 0.14f, Ease.OutQuad))
                : Vector3.LerpUnclamped(anticipationScale, panelEnd.Scale, EaseValue((panelProgress - 0.14f) / 0.86f, options.Ease ?? Ease.InCubic));
            Vector3 panelPosition = Vector3.LerpUnclamped(panelStart.Position, panelEnd.Position, EaseValue(panelProgress, Ease.InCubic));
            float panelAlpha = Mathf.LerpUnclamped(panelStart.Alpha, panelEnd.Alpha, Progress(panelProgress, 0.12f, 0.88f, Ease.InQuad));
            panel.Apply(new UISequencePose(panelPosition, panelScale, Quaternion.SlerpUnclamped(panelStart.Rotation, panelEnd.Rotation, panelProgress), panelAlpha));

            if (backdrop != null)
            {
                float backdropProgress = Progress(time / duration, 0.18f, 0.82f, Ease.InQuad);
                ApplyInterpolated(backdrop, backdropStart, backdropEnd, backdropProgress, backdropProgress, backdropProgress, backdropProgress);
            }

            float childDuration = duration * 0.42f;
            for (int i = 0; i < controls.Length; i++)
            {
                int order = controls.Length - 1 - i;
                float progress = Mathf.Clamp01((time - order * childStagger) / childDuration);
                ApplyInterpolated(controls[i], controlStarts[i], controlEnds[i], EaseValue(progress, Ease.InCubic), EaseValue(progress, Ease.InQuad), EaseValue(progress, Ease.InQuad), progress);
            }
        }

        private static Tween CreateTimeline(GameObject owner, float duration, TweenOptions options, Action initialize, Action<float> evaluate, Action complete, Action rewind)
        {
            return NormalizedTweenTimeline.Create(
                owner,
                duration,
                options.SetEase(Ease.Linear),
                initialize,
                progress => evaluate(progress * duration),
                complete,
                rewind,
                rewind);
        }

        private static void ApplyInterpolated(UISequenceState state, UISequencePose start, UISequencePose end, float positionProgress, float scaleProgress, float alphaProgress, float rotationProgress)
        {
            state.Apply(new UISequencePose(
                Vector3.LerpUnclamped(start.Position, end.Position, positionProgress),
                Vector3.LerpUnclamped(start.Scale, end.Scale, scaleProgress),
                Quaternion.SlerpUnclamped(start.Rotation, end.Rotation, rotationProgress),
                Mathf.LerpUnclamped(start.Alpha, end.Alpha, alphaProgress)));
        }

        private static UISequencePose ResolveShowStart(UISequenceState state, Vector3 positionOffset, Vector3 scale)
        {
            if (!state.IsInvocationShown) return state.Invocation;
            return new UISequencePose(state.Shown.Position + positionOffset, scale, state.Shown.Rotation, 0f);
        }

        private static UISequencePose ResolveFadeInStart(UISequenceState state)
        {
            if (!state.IsInvocationShown) return state.Invocation;
            return new UISequencePose(state.Shown.Position, state.Shown.Scale, state.Shown.Rotation, 0f);
        }

        private static UISequencePose HiddenPose(UISequencePose shown, Vector3 positionOffset, Vector3 scale)
            => new UISequencePose(shown.Position + positionOffset, scale, shown.Rotation, 0f);

        private static UISequenceState[] CreateStates(IReadOnlyList<GameObject> targets)
        {
            var states = new UISequenceState[targets.Count];
            for (int i = 0; i < targets.Count; i++) states[i] = new UISequenceState(targets[i]);
            return states;
        }

        private static void InitializeStates(UISequenceState[] states)
        {
            for (int i = 0; i < states.Length; i++) states[i].Initialize();
        }

        private static void ApplyEndpoints(UISequenceState[] states, UISequencePose[] endpoints)
        {
            for (int i = 0; i < states.Length; i++) states[i].Apply(endpoints[i]);
        }

        private static void RestoreInvocations(UISequenceState[] states)
        {
            for (int i = 0; i < states.Length; i++) states[i].RestoreInvocation();
        }

        private static Vector3 DirectionVector(UISequenceDirection direction)
        {
            switch (direction)
            {
                case UISequenceDirection.Up: return Vector3.up;
                case UISequenceDirection.Down: return Vector3.down;
                case UISequenceDirection.Left: return Vector3.left;
                case UISequenceDirection.Right: return Vector3.right;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown UI sequence direction.");
            }
        }

        private static Vector3 UniformScale(Vector3 scale, float multiplier) => scale * Mathf.Max(0f, multiplier);

        private static Vector3 CompressY(Vector3 scale, float multiplier)
            => new Vector3(scale.x, scale.y * Mathf.Max(0f, multiplier), scale.z);

        private static float Progress(float value, float start, float length, Ease ease)
            => EaseValue((value - start) / length, ease);

        private static float EaseValue(float progress, Ease ease)
            => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);

        private static float ResolveStrength(TweenOptions options)
        {
            float strength = options.Strength ?? 1f;
            ValidateFinite(strength, nameof(TweenOptions.Strength));
            if (strength < 0f) throw new ArgumentOutOfRangeException(nameof(TweenOptions.Strength), strength, "Strength cannot be negative.");
            return strength;
        }

        private static void ValidateRequest(GameObject target, float duration, TweenOptions options)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            ValidateRectTransform(target, nameof(target));
            ValidateFinite(duration, nameof(duration));
            if (duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero.");
            if (options.SpeedBased == true) throw new NotSupportedException("Semantic UI sequences do not support speed-based timing.");
        }

        private static void ValidateRequest(GameObject target, float duration, float distance, UISequenceDirection direction, TweenOptions options)
        {
            ValidateRequest(target, duration, options);
            ValidateFinite(distance, nameof(distance));
            if (distance < 0f) throw new ArgumentOutOfRangeException(nameof(distance), distance, "Distance cannot be negative.");
            DirectionVector(direction);
        }

        private static void ValidateRectTransform(GameObject target, string parameterName)
        {
            if (!(target.transform is RectTransform)) throw new ArgumentException($"UI sequence target '{target.name}' requires a RectTransform.", parameterName);
        }

        private static void ValidateStagger(float stagger)
        {
            ValidateFinite(stagger, nameof(stagger));
            if (stagger < 0f) throw new ArgumentOutOfRangeException(nameof(stagger), stagger, "Child stagger cannot be negative.");
        }

        private static void ValidateLiveTargets(IReadOnlyList<GameObject> targets, string parameterName)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null) throw new ArgumentException($"Target at index {i} was destroyed before the sequence was built.", parameterName);
                ValidateRectTransform(targets[i], parameterName);
            }
        }

        private static void ValidateBackdrop(GameObject panel, GameObject backdrop)
        {
            if (backdrop == null) return;
            if (backdrop == panel) throw new ArgumentException("Backdrop and panel must be different objects.", nameof(backdrop));
            ValidateRectTransform(backdrop, nameof(backdrop));
        }

        private static void RequireBackdropAlpha(GameObject backdrop, UISequenceState backdropState)
        {
            if (backdropState != null && !backdropState.HasAlpha) throw new InvalidOperationException($"Backdrop '{backdrop.name}' requires a CanvasGroup or supported Graphic alpha target.");
        }

        private static void ValidateIncoming(GameObject outgoing, GameObject incoming)
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));
            if (incoming == outgoing) throw new ArgumentException("Outgoing and incoming page content must be different objects.", nameof(incoming));
            ValidateRectTransform(incoming, nameof(incoming));
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        private readonly struct UISequencePose
        {
            public readonly Vector3 Position;
            public readonly Vector3 Scale;
            public readonly Quaternion Rotation;
            public readonly float Alpha;

            public UISequencePose(Vector3 position, Vector3 scale, Quaternion rotation, float alpha)
            {
                Position = position;
                Scale = scale;
                Rotation = rotation;
                Alpha = alpha;
            }
        }

        private sealed class UISequenceState
        {
            private readonly GameObject _target;
            private RectTransform _rectTransform;
            private TweenTargetUtility.TweenAlphaBinding _alphaBinding;
            private bool _initialized;

            public UISequenceState(GameObject target)
            {
                if (target == null) throw new ArgumentNullException(nameof(target));
                ValidateRectTransform(target, nameof(target));
                _target = target;
            }

            public UISequencePose Shown { get; private set; }
            public UISequencePose Invocation { get; private set; }
            public bool HasAlpha { get; private set; }
            public bool IsInvocationShown => Approximately(Invocation, Shown, HasAlpha);

            public void Initialize()
            {
                if (_initialized) return;
                _rectTransform = (RectTransform)_target.transform;
                UIAnimationStateCache cache = UIAnimationStateCache.GetOrCreate(_target);
                HasAlpha = TweenTargetUtility.TryGetAlphaBinding(_target, out _alphaBinding);
                float shownAlpha = cache.HasAlpha ? cache.BaseAlpha : HasAlpha ? _alphaBinding.GetAlpha() : 1f;
                float invocationAlpha = HasAlpha ? _alphaBinding.GetAlpha() : shownAlpha;
                Shown = new UISequencePose(cache.BaseAnchoredPosition3D, cache.BaseScale, Quaternion.Euler(cache.BaseEulerAngles), shownAlpha);
                Invocation = new UISequencePose(_rectTransform.anchoredPosition3D, _rectTransform.localScale, _rectTransform.localRotation, invocationAlpha);
                _initialized = true;
            }

            public void Apply(UISequencePose pose)
            {
                _rectTransform.anchoredPosition3D = pose.Position;
                _rectTransform.localScale = pose.Scale;
                _rectTransform.localRotation = pose.Rotation;
                if (HasAlpha) _alphaBinding.SetAlpha(pose.Alpha);
            }

            public void RestoreInvocation() => Apply(Invocation);

            private static bool Approximately(UISequencePose a, UISequencePose b, bool compareAlpha)
            {
                if ((a.Position - b.Position).sqrMagnitude > 0.0001f) return false;
                if ((a.Scale - b.Scale).sqrMagnitude > 0.000001f) return false;
                if (Quaternion.Angle(a.Rotation, b.Rotation) > 0.01f) return false;
                return !compareAlpha || Mathf.Abs(a.Alpha - b.Alpha) <= 0.0001f;
            }
        }
    }
}
