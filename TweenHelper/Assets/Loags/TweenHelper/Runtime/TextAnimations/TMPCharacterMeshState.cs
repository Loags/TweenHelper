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

        private void ApplyCharacter(int characterIndex, Vector3 offset, float scale, float alpha)
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
                Color32 color = baselineColors[index];
                color.a = (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * Mathf.Clamp01(alpha)), 0, 255);
                colors[index] = color;
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
