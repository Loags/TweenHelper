using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    internal readonly struct TMPGlyphTransform
    {
        public TMPGlyphTransform(Vector3 positionOffset, Vector2 scale, float rotation, float horizontalShear, float alpha,
            Color? tint = null, float tintStrength = 0f, TextGlyphPivot pivot = TextGlyphPivot.Center)
        {
            PositionOffset = positionOffset;
            Scale = scale;
            Rotation = rotation;
            HorizontalShear = horizontalShear;
            Alpha = alpha;
            Tint = tint;
            TintStrength = tintStrength;
            Pivot = pivot;
        }

        public Vector3 PositionOffset { get; }
        public Vector2 Scale { get; }
        public float Rotation { get; }
        public float HorizontalShear { get; }
        public float Alpha { get; }
        public Color? Tint { get; }
        public float TintStrength { get; }
        public TextGlyphPivot Pivot { get; }

        public static TMPGlyphTransform Identity => new TMPGlyphTransform(Vector3.zero, Vector2.one, 0f, 0f, 1f);
    }

    internal sealed class TMPCharacterMeshState
    {
        private static readonly Dictionary<TMP_Text, TMPCharacterMeshState> ActiveWriters = new Dictionary<TMP_Text, TMPCharacterMeshState>();

        private readonly TMP_Text _text;
        private TMP_MeshInfo[] _baselineMeshInfo;
        private TMPTextElementMap _elementMap;
        private TMPStaggerTiming _staggerTiming;
        private TextAnimationUnit _staggerUnit;
        private StaggerOrder _staggerOrder;
        private float _staggerInterval;
        private float _staggerDuration;
        private int _staggerSeed;
        private int _staggerCaptureVersion = -1;
        private string _sourceText;
        private int _characterCount;
        private int _captureVersion;
        private bool _initialized;
        private bool _ownsWriter;

        public TMPCharacterMeshState(TMP_Text text)
        {
            _text = text;
        }

        public void Initialize() => EnsureWriter();

        public void ApplyStagger(float progress, Vector3 direction, float distance, float characterStagger, float duration, float strength)
            => ApplyStagger(progress, TextAnimationUnit.Character, StaggerOrder.FirstToLast, direction, distance, characterStagger, duration, 1337, strength);

        public void ApplyStagger(float progress, TextAnimationUnit unit, StaggerOrder order, Vector3 direction, float distance, float unitStagger, float duration, int seed, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0)
            {
                UpdateTextMesh();
                return;
            }

            TMPStaggerTiming timing = GetStaggerTiming(unit, order, unitStagger, duration, seed);
            float startScale = Mathf.Max(0.1f, 1f - 0.08f * strength);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float characterProgress = timing.GetLocalProgress(visibleOrder, progress);
                float positionProgress = EaseValue(characterProgress, Ease.OutCubic);
                float scaleProgress = EaseValue(characterProgress, Ease.OutBack);
                float alphaProgress = EaseValue(characterProgress, Ease.OutQuad);
                Vector3 offset = -direction * distance * strength * (1f - positionProgress);
                float scale = Mathf.LerpUnclamped(startScale, 1f, scaleProgress);
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], offset, scale, alphaProgress);
            }

            UpdateTextMesh();
        }

        public void ApplyStaggerOut(float progress, Vector3 direction, float distance, float characterStagger, float duration, float strength)
            => ApplyStaggerOut(progress, TextAnimationUnit.Character, StaggerOrder.LastToFirst, direction, distance, characterStagger, duration, 1337, strength);

        public void ApplyStaggerOut(float progress, TextAnimationUnit unit, StaggerOrder order, Vector3 direction, float distance, float unitStagger, float duration, int seed, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0)
            {
                UpdateTextMesh();
                return;
            }

            TMPStaggerTiming timing = GetStaggerTiming(unit, order, unitStagger, duration, seed);
            float endScale = Mathf.Max(0.1f, 1f - 0.1f * strength);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float characterProgress = timing.GetLocalProgress(visibleOrder, progress);
                float positionProgress = EaseValue(characterProgress, Ease.InCubic);
                float scaleProgress = EaseValue(characterProgress, Ease.InQuad);
                float alphaProgress = 1f - EaseValue(characterProgress, Ease.InQuad);
                Vector3 offset = direction * distance * strength * positionProgress;
                float scale = Mathf.LerpUnclamped(1f, endScale, scaleProgress);
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], offset, scale, alphaProgress);
            }

            UpdateTextMesh();
        }

        public void ApplyWave(float progress, Vector3 direction, float amplitude, int waveCount, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float cycle = progress * waveCount;
            float cycleProgress = cycle - Mathf.Floor(cycle);
            if (Mathf.Approximately(cycleProgress, 0f))
            {
                UpdateTextMesh();
                return;
            }

            const float waveWidth = 1.75f;
            float center = Mathf.LerpUnclamped(-waveWidth, count - 1 + waveWidth, cycleProgress);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float proximity = Mathf.Clamp01(1f - Mathf.Abs(visibleOrder - center) / waveWidth);
                float wave = Mathf.Sin(proximity * Mathf.PI * 0.5f);
                Vector3 offset = direction * amplitude * strength * wave;
                float scale = 1f + 0.045f * strength * wave;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], offset, scale, 1f);
            }

            UpdateTextMesh();
        }

        public void ApplyBounce(float progress, Vector3 direction, float amplitude, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            const float bounceWidth = 2.25f;
            float center = Mathf.LerpUnclamped(-bounceWidth, count - 1 + bounceWidth, progress);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float proximity = Mathf.Clamp01(1f - Mathf.Abs(visibleOrder - center) / bounceWidth);
                float bounce = Mathf.Sin(proximity * Mathf.PI);
                Vector3 offset = direction * amplitude * strength * bounce;
                float scale = 1f + 0.07f * strength * bounce;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], offset, scale, 1f);
            }

            UpdateTextMesh();
        }

        public void ApplyColorSweep(float progress, Color highlightColor, float width, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float center = Mathf.LerpUnclamped(-width, count - 1 + width, progress);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float proximity = Mathf.Clamp01(1f - Mathf.Abs(visibleOrder - center) / width);
                float intensity = Mathf.Sin(proximity * Mathf.PI * 0.5f) * strength;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], Vector3.zero, 1f + 0.035f * intensity, 1f, highlightColor, Mathf.Clamp01(intensity));
            }

            UpdateTextMesh();
        }

        public void ApplyGlitch(float progress, float distance, int seed, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            float envelope = Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
            if (count == 0 || envelope <= 0.0001f)
            {
                UpdateTextMesh();
                return;
            }

            int timeSlice = Mathf.FloorToInt(progress * 21f);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float horizontal = TMPDeterministicNoise.Sample(seed, visibleOrder, timeSlice, 1) * 2f - 1f;
                float vertical = TMPDeterministicNoise.Sample(seed, visibleOrder, timeSlice, 2) * 2f - 1f;
                float scaleNoise = TMPDeterministicNoise.Sample(seed, visibleOrder, timeSlice, 3) * 2f - 1f;
                float colorNoise = TMPDeterministicNoise.Sample(seed, visibleOrder, timeSlice, 4);
                Vector3 offset = new Vector3(horizontal, vertical * 0.45f, 0f) * distance * strength * envelope;
                float scale = 1f + scaleNoise * 0.055f * strength * envelope;
                Color tint = colorNoise < 0.5f ? new Color(0.25f, 0.95f, 1f, 1f) : new Color(1f, 0.22f, 0.45f, 1f);
                float tintStrength = Mathf.Clamp01((0.18f + colorNoise * 0.28f) * strength * envelope);
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], offset, scale, 1f, tint, tintStrength);
            }

            UpdateTextMesh();
        }

        public void ApplyEmphasis(float progress, Vector3 direction, float amplitude, int startCharacter, int characterCount, Color highlightColor, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            int start = Mathf.Clamp(startCharacter, 0, count);
            int end = characterCount < 0 ? count : Mathf.Min(count, start + characterCount);
            float pulse = Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
            if (start >= end || pulse <= 0.0001f)
            {
                UpdateTextMesh();
                return;
            }

            for (int visibleOrder = start; visibleOrder < end; visibleOrder++)
            {
                float localOrder = end - start <= 1 ? 0.5f : (visibleOrder - start) / (float)(end - start - 1);
                float arch = Mathf.Sin(localOrder * Mathf.PI);
                float amount = pulse * Mathf.Lerp(0.78f, 1f, arch) * strength;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder], direction * amplitude * amount, 1f + 0.12f * amount, 1f, highlightColor, Mathf.Clamp01(amount * 0.72f));
            }

            UpdateTextMesh();
        }

        public void ApplyWiggle(float progress, float distance, float rotation, int seed, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float cycle = progress * Mathf.PI * 2f;
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float phaseX = TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 1) * Mathf.PI * 2f;
                float phaseY = TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 2) * Mathf.PI * 2f;
                float phaseRotation = TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 3) * Mathf.PI * 2f;
                float horizontal = (Mathf.Sin(cycle + phaseX) - Mathf.Sin(phaseX)) * 0.5f;
                float vertical = (Mathf.Sin(cycle + phaseY) - Mathf.Sin(phaseY)) * 0.5f;
                float zRotation = (Mathf.Sin(cycle + phaseRotation) - Mathf.Sin(phaseRotation)) * 0.5f * rotation * strength;
                Vector3 offset = new Vector3(horizontal, vertical, 0f) * distance * strength;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(offset, Vector2.one, zRotation, 0f, 1f));
            }

            UpdateTextMesh();
        }

        public void ApplyFloat(float progress, Vector3 direction, float amplitude, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float cycle = progress * Mathf.PI * 2f;
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float phase = visibleOrder * 0.72f;
                float wave = (Mathf.Sin(cycle + phase) - Mathf.Sin(phase)) * 0.5f;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(direction * amplitude * strength * wave, Vector2.one, 0f, 0f, 1f));
            }

            UpdateTextMesh();
        }

        public void ApplySwing(float progress, float angle, TextGlyphPivot pivot, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float cycle = progress * Mathf.PI * 2f;
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float phase = visibleOrder * 0.61f;
                float swing = (Mathf.Sin(cycle + phase) - Mathf.Sin(phase)) * 0.5f;
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(Vector3.zero, Vector2.one, angle * strength * swing, 0f, 1f, pivot: pivot));
            }

            UpdateTextMesh();
        }

        public void ApplyPulse(float progress, float scaleAmount, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float cycle = progress * Mathf.PI * 2f;
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float phase = visibleOrder * 0.67f;
                float wave = (Mathf.Sin(cycle + phase) - Mathf.Sin(phase)) * 0.5f;
                float scale = Mathf.Max(0.1f, 1f + scaleAmount * strength * wave);
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(Vector3.zero, new Vector2(scale, scale), 0f, 0f, 1f));
            }

            UpdateTextMesh();
        }

        public void ApplyScatter(float progress, bool entering, TextAnimationUnit unit, StaggerOrder order, float distance,
            float rotation, float unitStagger, float duration, int seed, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0)
            {
                UpdateTextMesh();
                return;
            }

            TMPStaggerTiming timing = GetStaggerTiming(unit, order, unitStagger, duration, seed);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float localProgress = timing.GetLocalProgress(visibleOrder, progress);
                float poseProgress = entering ? 1f - EaseValue(localProgress, Ease.OutCubic) : EaseValue(localProgress, Ease.InCubic);
                float alpha = entering ? EaseValue(localProgress, Ease.OutQuad) : 1f - EaseValue(localProgress, Ease.InQuad);
                float angle = TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 1) * Mathf.PI * 2f;
                float radius = Mathf.Lerp(0.45f, 1f, TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 2)) * distance * strength;
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius * poseProgress;
                float zRotation = (TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 3) * 2f - 1f) * rotation * strength * poseProgress;
                float poseScale = Mathf.Max(0.1f, 1f - (0.12f + 0.16f * TMPDeterministicNoise.Sample(seed, visibleOrder, 0, 4)) * strength);
                float scale = Mathf.LerpUnclamped(1f, poseScale, poseProgress);
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(offset, new Vector2(scale, scale), zRotation, 0f, alpha));
            }

            UpdateTextMesh();
        }

        public void ApplyRotateTransition(float progress, bool entering, TextAnimationUnit unit, StaggerOrder order, float angle,
            float unitStagger, float duration, int seed, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0)
            {
                UpdateTextMesh();
                return;
            }

            TMPStaggerTiming timing = GetStaggerTiming(unit, order, unitStagger, duration, seed);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                float localProgress = timing.GetLocalProgress(visibleOrder, progress);
                float poseProgress = entering ? 1f - EaseValue(localProgress, Ease.OutBack) : EaseValue(localProgress, Ease.InCubic);
                float alpha = entering ? EaseValue(localProgress, Ease.OutQuad) : 1f - EaseValue(localProgress, Ease.InQuad);
                float direction = visibleOrder % 2 == 0 ? 1f : -1f;
                float zRotation = angle * direction * strength * poseProgress;
                float poseScale = Mathf.Max(0.1f, 1f - 0.12f * strength);
                float scale = Mathf.LerpUnclamped(1f, poseScale, poseProgress);
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(Vector3.zero, new Vector2(scale, scale), zRotation, 0f, alpha));
            }

            UpdateTextMesh();
        }

        public void ApplyShear(float progress, float amount, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            float envelope = Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
            if (count == 0 || envelope <= 0.0001f)
            {
                UpdateTextMesh();
                return;
            }

            float shear = amount * strength * envelope;
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                ApplyCharacter(_elementMap.VisibleCharacterIndices[visibleOrder],
                    new TMPGlyphTransform(Vector3.zero, Vector2.one, 0f, shear, 1f));
            }

            UpdateTextMesh();
        }

        public void ApplyTrackingPulse(float progress, float distance, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            float envelope = Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
            if (count == 0 || envelope <= 0.0001f)
            {
                UpdateTextMesh();
                return;
            }

            GetHorizontalVisualBounds(out float center, out float halfWidth);
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                int characterIndex = _elementMap.VisibleCharacterIndices[visibleOrder];
                float glyphCenter = GetGlyphCenter(characterIndex).x;
                float normalizedOffset = halfWidth <= 0.0001f ? 0f : (glyphCenter - center) / halfWidth;
                Vector3 offset = Vector3.right * normalizedOffset * distance * strength * envelope;
                ApplyCharacter(characterIndex, new TMPGlyphTransform(offset, Vector2.one, 0f, 0f, 1f));
            }

            UpdateTextMesh();
        }

        public void ApplyImpactRipple(float progress, Vector2 impactPoint, float radius, float amplitude, float scaleAmount, float strength)
        {
            PrepareForWrite();

            int count = _elementMap.VisibleCharacterCount;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                int characterIndex = _elementMap.VisibleCharacterIndices[visibleOrder];
                Vector3 center = GetGlyphCenter(characterIndex);
                Vector2 fromImpact = new Vector2(center.x - impactPoint.x, center.y - impactPoint.y);
                float distanceFromImpact = fromImpact.magnitude;
                float normalizedDistance = distanceFromImpact / radius;
                if (normalizedDistance >= 1f) continue;

                float start = normalizedDistance * 0.35f;
                float localProgress = Mathf.Clamp01((progress - start) / Mathf.Max(0.0001f, 1f - start));
                float pulse = Mathf.Sin(localProgress * Mathf.PI) * (1f - normalizedDistance);
                Vector2 radialDirection = distanceFromImpact <= 0.0001f ? Vector2.up : fromImpact / distanceFromImpact;
                Vector3 offset = new Vector3(radialDirection.x, radialDirection.y, 0f) * amplitude * strength * pulse;
                float scale = Mathf.Max(0.1f, 1f + scaleAmount * strength * pulse);
                ApplyCharacter(characterIndex, new TMPGlyphTransform(offset, new Vector2(scale, scale), 0f, 0f, 1f));
            }

            UpdateTextMesh();
        }

        public void Restore()
        {
            if (!_initialized || !_ownsWriter)
            {
                ReleaseWriter();
                return;
            }

            if (_text != null)
            {
                if (_text.text != _sourceText || _text.havePropertiesChanged || MeshLayoutChanged())
                {
                    _text.ForceMeshUpdate();
                }
                else
                {
                    RestoreBuffers();
                    UpdateTextMesh();
                }
            }

            ReleaseWriter();
        }

        private void PrepareForWrite()
        {
            EnsureWriter();
            EnsureCurrentMesh();
            RestoreBuffers();
        }

        private void EnsureWriter()
        {
            if (_ownsWriter) return;
            if (_text == null) throw new InvalidOperationException("The TMP text animation target no longer exists.");

            if (ActiveWriters.TryGetValue(_text, out TMPCharacterMeshState writer) && writer != this)
            {
                throw new InvalidOperationException($"TMP_Text '{_text.name}' already has an active character-mesh animation. Sequence mesh-writing operations with Then() instead of running them in parallel.");
            }

            ActiveWriters[_text] = this;
            _ownsWriter = true;
            CaptureCurrentMesh();
        }

        private void ReleaseWriter()
        {
            if (!_ownsWriter) return;
            if (!ReferenceEquals(_text, null) && ActiveWriters.TryGetValue(_text, out TMPCharacterMeshState writer) && writer == this)
            {
                ActiveWriters.Remove(_text);
            }

            _ownsWriter = false;
        }

        private void EnsureCurrentMesh()
        {
            if (!_initialized || _text.text != _sourceText || _text.havePropertiesChanged || _text.textInfo.characterCount != _characterCount || MeshLayoutChanged())
            {
                CaptureCurrentMesh();
            }
        }

        private void CaptureCurrentMesh()
        {
            _text.ForceMeshUpdate();
            TMP_TextInfo textInfo = _text.textInfo;
            _baselineMeshInfo = textInfo.CopyMeshInfoVertexData();
            _elementMap = new TMPTextElementMap(textInfo);
            _sourceText = _text.text;
            _characterCount = textInfo.characterCount;
            _captureVersion++;
            _initialized = true;
        }

        private TMPStaggerTiming GetStaggerTiming(TextAnimationUnit unit, StaggerOrder order, float interval, float duration, int seed)
        {
            bool matches = _staggerTiming != null
                && _staggerCaptureVersion == _captureVersion
                && _staggerUnit == unit
                && _staggerOrder == order
                && Mathf.Approximately(_staggerInterval, interval)
                && Mathf.Approximately(_staggerDuration, duration)
                && _staggerSeed == seed;
            if (matches) return _staggerTiming;

            _staggerTiming = new TMPStaggerTiming(_elementMap, unit, order, interval, duration, seed);
            _staggerUnit = unit;
            _staggerOrder = order;
            _staggerInterval = interval;
            _staggerDuration = duration;
            _staggerSeed = seed;
            _staggerCaptureVersion = _captureVersion;
            return _staggerTiming;
        }

        private Vector3 GetGlyphCenter(int characterIndex)
        {
            TMP_CharacterInfo character = _text.textInfo.characterInfo[characterIndex];
            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;
            Vector3[] vertices = _baselineMeshInfo[materialIndex].vertices;
            return (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;
        }

        private void GetHorizontalVisualBounds(out float center, out float halfWidth)
        {
            int count = _elementMap.VisibleCharacterCount;
            if (count == 0)
            {
                center = 0f;
                halfWidth = 0f;
                return;
            }

            float minimum = float.PositiveInfinity;
            float maximum = float.NegativeInfinity;
            for (int visibleOrder = 0; visibleOrder < count; visibleOrder++)
            {
                int characterIndex = _elementMap.VisibleCharacterIndices[visibleOrder];
                TMP_CharacterInfo character = _text.textInfo.characterInfo[characterIndex];
                Vector3[] vertices = _baselineMeshInfo[character.materialReferenceIndex].vertices;
                minimum = Mathf.Min(minimum, vertices[character.vertexIndex].x, vertices[character.vertexIndex + 1].x);
                maximum = Mathf.Max(maximum, vertices[character.vertexIndex + 2].x, vertices[character.vertexIndex + 3].x);
            }

            center = (minimum + maximum) * 0.5f;
            halfWidth = Mathf.Max(0f, (maximum - minimum) * 0.5f);
        }

        private void ApplyCharacter(int characterIndex, Vector3 offset, float scale, float alpha, Color? tint = null, float tintStrength = 0f)
        {
            var transform = new TMPGlyphTransform(offset, new Vector2(scale, scale), 0f, 0f, alpha, tint, tintStrength);
            ApplyCharacter(characterIndex, transform);
        }

        private void ApplyCharacter(int characterIndex, TMPGlyphTransform transform)
        {
            TMP_CharacterInfo character = _text.textInfo.characterInfo[characterIndex];
            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;
            if (materialIndex < 0 || materialIndex >= _baselineMeshInfo.Length) return;

            Vector3[] baselineVertices = _baselineMeshInfo[materialIndex].vertices;
            Vector3[] vertices = _text.textInfo.meshInfo[materialIndex].vertices;
            Color32[] baselineColors = _baselineMeshInfo[materialIndex].colors32;
            Color32[] colors = _text.textInfo.meshInfo[materialIndex].colors32;
            if (baselineVertices == null || vertices == null || vertexIndex < 0 || vertexIndex + 3 >= baselineVertices.Length || vertexIndex + 3 >= vertices.Length) return;

            Vector3 pivot = transform.Pivot == TextGlyphPivot.Top
                ? (baselineVertices[vertexIndex + 1] + baselineVertices[vertexIndex + 2]) * 0.5f
                : (baselineVertices[vertexIndex] + baselineVertices[vertexIndex + 2]) * 0.5f;
            float radians = transform.Rotation * Mathf.Deg2Rad;
            float sine = Mathf.Sin(radians);
            float cosine = Mathf.Cos(radians);

            for (int vertexOffset = 0; vertexOffset < 4; vertexOffset++)
            {
                int index = vertexIndex + vertexOffset;
                Vector3 local = baselineVertices[index] - pivot;
                local.x *= transform.Scale.x;
                local.y *= transform.Scale.y;
                local.x += local.y * transform.HorizontalShear;
                float rotatedX = local.x * cosine - local.y * sine;
                float rotatedY = local.x * sine + local.y * cosine;
                vertices[index] = pivot + new Vector3(rotatedX, rotatedY, local.z) + transform.PositionOffset;

                if (baselineColors == null || colors == null || index >= baselineColors.Length || index >= colors.Length) continue;
                Color baselineColor = baselineColors[index];
                Color color = transform.Tint.HasValue
                    ? Color.LerpUnclamped(baselineColor, transform.Tint.Value, Mathf.Clamp01(transform.TintStrength))
                    : baselineColor;
                color.a = baselineColor.a * Mathf.Clamp01(transform.Alpha);
                colors[index] = color;
            }
        }

        private bool MeshLayoutChanged()
        {
            if (_baselineMeshInfo == null || _text == null || _text.textInfo.meshInfo == null) return true;
            TMP_MeshInfo[] current = _text.textInfo.meshInfo;
            if (current.Length != _baselineMeshInfo.Length) return true;

            for (int i = 0; i < current.Length; i++)
            {
                int baselineVertexCount = _baselineMeshInfo[i].vertices?.Length ?? 0;
                int currentVertexCount = current[i].vertices?.Length ?? 0;
                int baselineColorCount = _baselineMeshInfo[i].colors32?.Length ?? 0;
                int currentColorCount = current[i].colors32?.Length ?? 0;
                if (baselineVertexCount != currentVertexCount || baselineColorCount != currentColorCount) return true;
            }

            return false;
        }

        private void RestoreBuffers()
        {
            TMP_MeshInfo[] currentMeshInfo = _text.textInfo.meshInfo;
            int materialCount = Mathf.Min(_baselineMeshInfo.Length, currentMeshInfo.Length);
            for (int i = 0; i < materialCount; i++)
            {
                Copy(_baselineMeshInfo[i].vertices, currentMeshInfo[i].vertices);
                Copy(_baselineMeshInfo[i].colors32, currentMeshInfo[i].colors32);
            }
        }

        private void UpdateTextMesh()
        {
            _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
        }

        private static void Copy<T>(T[] source, T[] destination)
        {
            if (source == null || destination == null) return;
            Array.Copy(source, destination, Mathf.Min(source.Length, destination.Length));
        }

        private static float EaseValue(float progress, Ease ease)
            => DOVirtual.EasedValue(0f, 1f, Mathf.Clamp01(progress), ease);
    }
}
