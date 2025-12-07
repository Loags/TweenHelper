using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LB.TweenHelper
{
    /// <summary>
    /// Test component to preview TweenHelper animations in the editor.
    /// Attach to any GameObject and use the context menu or inspector buttons to test.
    /// </summary>
    public class TweenPreviewTester : MonoBehaviour
    {
        [Header("Preset Testing")]
        [Tooltip("Select a preset to test")]
        public string presetName = "PopIn";

        [Tooltip("Duration override (0 = use preset default)")]
        public float duration = 0f;

        [Header("Builder Testing")]
        public Vector3 moveTarget = Vector3.up * 2f;
        public Vector3 scaleTarget = Vector3.one * 1.5f;
        public Vector3 rotateTarget = new Vector3(0, 180, 0);

        [Header("Info")]
        [SerializeField] private string[] availablePresets;

        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private Quaternion _originalRotation;
        private TweenHandle _currentHandle;

        private void Awake()
        {
            SaveOriginalState();
            RefreshPresetList();
        }

        private void OnValidate()
        {
            RefreshPresetList();
        }

        private void SaveOriginalState()
        {
            _originalPosition = transform.localPosition;
            _originalScale = transform.localScale;
            _originalRotation = transform.localRotation;
        }

        private void RefreshPresetList()
        {
            // This will show available presets in inspector
            availablePresets = TweenPresetRegistry.PresetNames.ToArray();
        }

        #region Context Menu - Presets

        [ContextMenu("Play Selected Preset")]
        public void PlaySelectedPreset()
        {
            Kill();
            SaveOriginalState();

            var dur = duration > 0 ? duration : (float?)null;
            _currentHandle = transform.Tween().Preset(presetName, dur).Play();
            Debug.Log($"Playing preset: {presetName}");
        }

        [ContextMenu("Play PopIn")]
        public void PlayPopIn()
        {
            Kill();
            SaveOriginalState();
            _currentHandle = transform.Tween().Preset("PopIn").Play();
        }

        [ContextMenu("Play PopOut")]
        public void PlayPopOut()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("PopOut").Play();
        }

        [ContextMenu("Play Punch")]
        public void PlayPunch()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("Punch").Play();
        }

        [ContextMenu("Play Bounce")]
        public void PlayBounce()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("Bounce").Play();
        }

        [ContextMenu("Play Shake")]
        public void PlayShake()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("Shake").Play();
        }

        [ContextMenu("Play FadeIn")]
        public void PlayFadeIn()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("FadeIn").Play();
        }

        [ContextMenu("Play FadeOut")]
        public void PlayFadeOut()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("FadeOut").Play();
        }

        [ContextMenu("Play Spin")]
        public void PlaySpin()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("Spin").Play();
        }

        [ContextMenu("Play Wobble")]
        public void PlayWobble()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("Wobble").Play();
        }

        [ContextMenu("Play Attention")]
        public void PlayAttention()
        {
            Kill();
            _currentHandle = transform.Tween().Preset("Attention").Play();
        }

        #endregion

        #region Context Menu - Builder

        [ContextMenu("Test Move")]
        public void TestMove()
        {
            Kill();
            SaveOriginalState();
            _currentHandle = transform.Tween()
                .Move(transform.position + moveTarget)
                .WithEase(DG.Tweening.Ease.OutQuad)
                .Play();
        }

        [ContextMenu("Test Scale")]
        public void TestScale()
        {
            Kill();
            SaveOriginalState();
            _currentHandle = transform.Tween()
                .Scale(scaleTarget)
                .WithEase(DG.Tweening.Ease.OutBack)
                .Play();
        }

        [ContextMenu("Test Rotate")]
        public void TestRotate()
        {
            Kill();
            SaveOriginalState();
            _currentHandle = transform.Tween()
                .Rotate(rotateTarget)
                .WithEase(DG.Tweening.Ease.InOutQuad)
                .Play();
        }

        [ContextMenu("Test Sequence (Move → Scale → Rotate)")]
        public void TestSequence()
        {
            Kill();
            SaveOriginalState();
            _currentHandle = transform.Tween()
                .Move(transform.position + moveTarget, 0.5f)
                .Then()
                .Scale(scaleTarget, 0.3f)
                .Then()
                .Rotate(rotateTarget, 0.4f)
                .Play();
        }

        [ContextMenu("Test Parallel (Move + Scale)")]
        public void TestParallel()
        {
            Kill();
            SaveOriginalState();
            _currentHandle = transform.Tween()
                .Move(transform.position + moveTarget)
                .With()
                .Scale(scaleTarget)
                .Play();
        }

        #endregion

        #region Control

        [ContextMenu("Kill Current Animation")]
        public void Kill()
        {
            _currentHandle?.Kill();
            _currentHandle = null;
        }

        [ContextMenu("Reset to Original")]
        public void ResetToOriginal()
        {
            Kill();
            transform.localPosition = _originalPosition;
            transform.localScale = _originalScale;
            transform.localRotation = _originalRotation;
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            _currentHandle?.Pause();
        }

        [ContextMenu("Resume")]
        public void Resume()
        {
            _currentHandle?.Resume();
        }

        [ContextMenu("List All Presets")]
        public void ListAllPresets()
        {
            var presets = TweenPresetRegistry.PresetNames.ToList();
            Debug.Log($"Available Presets ({presets.Count}):\n" + string.Join("\n", presets.Select(p => $"  • {p}")));
        }

        #endregion
    }
}
