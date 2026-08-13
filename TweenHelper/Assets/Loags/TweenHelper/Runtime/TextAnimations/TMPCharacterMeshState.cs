using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    internal sealed class TMPCharacterMeshState
    {
        private readonly TMP_Text _text;
        private TMP_MeshInfo[] _baselineMeshInfo;
        private int[] _visibleCharacterIndices;
        private string _sourceText;
        private int _characterCount;
        private bool _initialized;

        public TMPCharacterMeshState(TMP_Text text)
        {
            _text = text;
        }

        public void Initialize()
        {
            if (_initialized) return;
            CaptureCurrentMesh();
        }

        public void ApplyStagger(float progress, Vector3 direction, float distance, float characterStagger, float duration, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
            if (count == 0)
            {
                UpdateTextMesh();
                return;
            }

            float requestedLastStart = duration <= 0f ? 0f : (count - 1) * characterStagger / duration;
            float compression = requestedLastStart > 0.58f ? 0.58f / requestedLastStart : 1f;
            float startScale = Mathf.Max(0.1f, 1f - 0.08f * strength);

            for (int order = 0; order < count; order++)
            {
                float start = duration <= 0f ? 0f : order * characterStagger / duration * compression;
                float characterProgress = Mathf.Clamp01((progress - start) / Mathf.Max(0.0001f, 1f - start));
                float positionProgress = EaseValue(characterProgress, Ease.OutCubic);
                float scaleProgress = EaseValue(characterProgress, Ease.OutBack);
                float alphaProgress = EaseValue(characterProgress, Ease.OutQuad);
                Vector3 offset = -direction * distance * strength * (1f - positionProgress);
                float scale = Mathf.LerpUnclamped(startScale, 1f, scaleProgress);
                ApplyCharacter(_visibleCharacterIndices[order], offset, scale, alphaProgress);
            }

            UpdateTextMesh();
        }

        public void ApplyStaggerOut(float progress, Vector3 direction, float distance, float characterStagger, float duration, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
            if (count == 0)
            {
                UpdateTextMesh();
                return;
            }

            float requestedLastStart = duration <= 0f ? 0f : (count - 1) * characterStagger / duration;
            float compression = requestedLastStart > 0.58f ? 0.58f / requestedLastStart : 1f;
            float endScale = Mathf.Max(0.1f, 1f - 0.1f * strength);

            for (int order = 0; order < count; order++)
            {
                float start = duration <= 0f ? 0f : order * characterStagger / duration * compression;
                float characterProgress = Mathf.Clamp01((progress - start) / Mathf.Max(0.0001f, 1f - start));
                float positionProgress = EaseValue(characterProgress, Ease.InCubic);
                float scaleProgress = EaseValue(characterProgress, Ease.InQuad);
                float alphaProgress = 1f - EaseValue(characterProgress, Ease.InQuad);
                Vector3 offset = direction * distance * strength * positionProgress;
                float scale = Mathf.LerpUnclamped(1f, endScale, scaleProgress);
                ApplyCharacter(_visibleCharacterIndices[count - 1 - order], offset, scale, alphaProgress);
            }

            UpdateTextMesh();
        }

        public void ApplyWave(float progress, Vector3 direction, float amplitude, int waveCount, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
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
            for (int order = 0; order < count; order++)
            {
                float proximity = Mathf.Clamp01(1f - Mathf.Abs(order - center) / waveWidth);
                float wave = Mathf.Sin(proximity * Mathf.PI * 0.5f);
                Vector3 offset = direction * amplitude * strength * wave;
                float scale = 1f + 0.045f * strength * wave;
                ApplyCharacter(_visibleCharacterIndices[order], offset, scale, 1f);
            }

            UpdateTextMesh();
        }

        public void ApplyBounce(float progress, Vector3 direction, float amplitude, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            const float bounceWidth = 2.25f;
            float center = Mathf.LerpUnclamped(-bounceWidth, count - 1 + bounceWidth, progress);
            for (int order = 0; order < count; order++)
            {
                float proximity = Mathf.Clamp01(1f - Mathf.Abs(order - center) / bounceWidth);
                float bounce = Mathf.Sin(proximity * Mathf.PI);
                Vector3 offset = direction * amplitude * strength * bounce;
                float scale = 1f + 0.07f * strength * bounce;
                ApplyCharacter(_visibleCharacterIndices[order], offset, scale, 1f);
            }

            UpdateTextMesh();
        }

        public void ApplyColorSweep(float progress, Color highlightColor, float width, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
            if (count == 0 || progress <= 0f || progress >= 1f)
            {
                UpdateTextMesh();
                return;
            }

            float center = Mathf.LerpUnclamped(-width, count - 1 + width, progress);
            for (int order = 0; order < count; order++)
            {
                float proximity = Mathf.Clamp01(1f - Mathf.Abs(order - center) / width);
                float intensity = Mathf.Sin(proximity * Mathf.PI * 0.5f) * strength;
                ApplyCharacter(_visibleCharacterIndices[order], Vector3.zero, 1f + 0.035f * intensity, 1f, highlightColor, Mathf.Clamp01(intensity));
            }

            UpdateTextMesh();
        }

        public void ApplyGlitch(float progress, float distance, int seed, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
            float envelope = Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
            if (count == 0 || envelope <= 0.0001f)
            {
                UpdateTextMesh();
                return;
            }

            int timeSlice = Mathf.FloorToInt(progress * 21f);
            for (int order = 0; order < count; order++)
            {
                float horizontal = Hash(seed, order, timeSlice, 1) * 2f - 1f;
                float vertical = Hash(seed, order, timeSlice, 2) * 2f - 1f;
                float scaleNoise = Hash(seed, order, timeSlice, 3) * 2f - 1f;
                float colorNoise = Hash(seed, order, timeSlice, 4);
                Vector3 offset = new Vector3(horizontal, vertical * 0.45f, 0f) * distance * strength * envelope;
                float scale = 1f + scaleNoise * 0.055f * strength * envelope;
                Color tint = colorNoise < 0.5f ? new Color(0.25f, 0.95f, 1f, 1f) : new Color(1f, 0.22f, 0.45f, 1f);
                float tintStrength = Mathf.Clamp01((0.18f + colorNoise * 0.28f) * strength * envelope);
                ApplyCharacter(_visibleCharacterIndices[order], offset, scale, 1f, tint, tintStrength);
            }

            UpdateTextMesh();
        }

        public void ApplyEmphasis(float progress, Vector3 direction, float amplitude, int startCharacter, int characterCount, Color highlightColor, float strength)
        {
            EnsureCurrentMesh();
            RestoreBuffers();

            int count = _visibleCharacterIndices.Length;
            int start = Mathf.Clamp(startCharacter, 0, count);
            int end = characterCount < 0 ? count : Mathf.Min(count, start + characterCount);
            float pulse = Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI);
            if (start >= end || pulse <= 0.0001f)
            {
                UpdateTextMesh();
                return;
            }

            for (int order = start; order < end; order++)
            {
                float localOrder = end - start <= 1 ? 0.5f : (order - start) / (float)(end - start - 1);
                float arch = Mathf.Sin(localOrder * Mathf.PI);
                float amount = pulse * Mathf.Lerp(0.78f, 1f, arch) * strength;
                ApplyCharacter(_visibleCharacterIndices[order], direction * amplitude * amount, 1f + 0.12f * amount, 1f, highlightColor, Mathf.Clamp01(amount * 0.72f));
            }

            UpdateTextMesh();
        }

        public void Restore()
        {
            if (!_initialized || _text == null) return;
            if (_text.text != _sourceText || _text.havePropertiesChanged)
            {
                _text.ForceMeshUpdate();
                return;
            }

            RestoreBuffers();
            UpdateTextMesh();
        }

        private void EnsureCurrentMesh()
        {
            if (!_initialized || _text.text != _sourceText || _text.havePropertiesChanged || _text.textInfo.characterCount != _characterCount)
            {
                CaptureCurrentMesh();
            }
        }

        private void CaptureCurrentMesh()
        {
            _text.ForceMeshUpdate();
            TMP_TextInfo textInfo = _text.textInfo;
            _baselineMeshInfo = textInfo.CopyMeshInfoVertexData();
            _sourceText = _text.text;
            _characterCount = textInfo.characterCount;

            var visible = new int[_characterCount];
            int visibleCount = 0;
            for (int i = 0; i < _characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                visible[visibleCount++] = i;
            }

            _visibleCharacterIndices = new int[visibleCount];
            Array.Copy(visible, _visibleCharacterIndices, visibleCount);
            _initialized = true;
        }

        private void ApplyCharacter(int characterIndex, Vector3 offset, float scale, float alpha, Color? tint = null, float tintStrength = 0f)
        {
            TMP_CharacterInfo character = _text.textInfo.characterInfo[characterIndex];
            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;
            if (materialIndex < 0 || materialIndex >= _baselineMeshInfo.Length) return;

            Vector3[] baselineVertices = _baselineMeshInfo[materialIndex].vertices;
            Vector3[] vertices = _text.textInfo.meshInfo[materialIndex].vertices;
            Color32[] baselineColors = _baselineMeshInfo[materialIndex].colors32;
            Color32[] colors = _text.textInfo.meshInfo[materialIndex].colors32;
            if (vertexIndex < 0 || vertexIndex + 3 >= baselineVertices.Length || vertexIndex + 3 >= vertices.Length) return;

            Vector3 center = (baselineVertices[vertexIndex] + baselineVertices[vertexIndex + 2]) * 0.5f;
            for (int i = 0; i < 4; i++)
            {
                int index = vertexIndex + i;
                vertices[index] = center + (baselineVertices[index] - center) * scale + offset;
                if (index >= baselineColors.Length || index >= colors.Length) continue;
                Color baselineColor = baselineColors[index];
                Color color = tint.HasValue ? Color.LerpUnclamped(baselineColor, tint.Value, Mathf.Clamp01(tintStrength)) : baselineColor;
                color.a = baselineColor.a * Mathf.Clamp01(alpha);
                colors[index] = color;
            }
        }

        private static float Hash(int seed, int character, int timeSlice, int channel)
        {
            unchecked
            {
                uint value = unchecked((uint)seed * 374761393u + (uint)character * 668265263u + (uint)timeSlice * 2246822519u + (uint)channel * 3266489917u);
                value = (value ^ (value >> 13)) * 1274126177;
                value ^= value >> 16;
                return (value & 0x00ffffff) / 16777215f;
            }
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
