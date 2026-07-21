using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    public sealed class PresetBrowserWindow : EditorWindow
    {
        private const string AllFamilies = "All families";

        private readonly List<ITweenPreset> _presets = new List<ITweenPreset>();
        private readonly List<string> _families = new List<string>();

        private string _search = string.Empty;
        private int _familyIndex;
        private Vector2 _listScroll;
        private ITweenPreset _selectedPreset;
        private Tween _previewTween;
        private PreviewSnapshot _previewSnapshot;
        private double _lastPreviewTime;

        [MenuItem("Tools/TweenHelper/Preset Browser", false, 0)]
        public static void Open()
        {
            var window = GetWindow<PresetBrowserWindow>();
            window.titleContent = new GUIContent("TweenHelper Presets");
            window.minSize = new Vector2(560f, 420f);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshPresets();
            EditorApplication.update += UpdatePreview;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += StopPreview;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdatePreview;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= StopPreview;
            StopPreview();
        }

        private void OnSelectionChange()
        {
            StopPreview();
            Repaint();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state) => StopPreview();

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space(4f);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawPresetList();
                DrawDetails();
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                _search = GUILayout.TextField(_search, GUI.skin.FindStyle("ToolbarSearchTextField"), GUILayout.MinWidth(180f));
                _familyIndex = EditorGUILayout.Popup(_familyIndex, _families.ToArray(), EditorStyles.toolbarPopup, GUILayout.Width(150f));

                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(58f)))
                {
                    RefreshPresets();
                }
            }
        }

        private void DrawPresetList()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(position.width * 0.46f)))
            {
                var visiblePresets = GetVisiblePresets();
                EditorGUILayout.LabelField($"Presets ({visiblePresets.Count}/{_presets.Count})", EditorStyles.boldLabel);

                _listScroll = EditorGUILayout.BeginScrollView(_listScroll, EditorStyles.helpBox);
                foreach (var preset in visiblePresets)
                {
                    bool selected = ReferenceEquals(preset, _selectedPreset);
                    var style = selected ? EditorStyles.miniButtonMid : EditorStyles.label;
                    if (GUILayout.Button(preset.PresetName, style, GUILayout.Height(22f)))
                    {
                        _selectedPreset = preset;
                        GUI.FocusControl(null);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawDetails()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                if (_selectedPreset == null)
                {
                    EditorGUILayout.LabelField("Select a preset to inspect it.", EditorStyles.wordWrappedLabel);
                    return;
                }

                var metadata = PresetVariantParser.Parse(_selectedPreset.PresetName);
                EditorGUILayout.LabelField(_selectedPreset.PresetName, EditorStyles.largeLabel);
                EditorGUILayout.LabelField(_selectedPreset.Description, EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space(8f);

                DrawValue("Family", metadata.Family);
                DrawValue("Intensity", metadata.Intensity);
                DrawValue("Direction", metadata.Direction);
                DrawValue("Axis / plane", string.IsNullOrEmpty(metadata.Axis) ? metadata.Plane : metadata.Axis);
                DrawValue("Default duration", $"{_selectedPreset.DefaultDuration:0.###}s");

                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("Fluent API", EditorStyles.boldLabel);
                string example = BuildExample(_selectedPreset);
                EditorGUILayout.SelectableLabel(example, EditorStyles.textArea, GUILayout.Height(42f));
                if (GUILayout.Button("Copy Example"))
                {
                    EditorGUIUtility.systemCopyBuffer = example;
                    ShowNotification(new GUIContent("Example copied"));
                }

                GUILayout.FlexibleSpace();
                DrawPreviewControls();
            }
        }

        private void DrawPreviewControls()
        {
            var target = GetPreviewTarget();
            string targetName = target == null ? "Select a scene GameObject to preview." : $"Preview target: {target.name}";
            EditorGUILayout.HelpBox(targetName, target == null ? MessageType.Info : MessageType.None);

            using (new EditorGUI.DisabledScope(target == null || !_selectedPreset.CanApplyTo(target)))
            {
                if (GUILayout.Button(_previewTween == null ? "Preview on Selection" : "Restart Preview", GUILayout.Height(28f)))
                {
                    StartPreview(target);
                }
            }

            using (new EditorGUI.DisabledScope(_previewTween == null))
            {
                if (GUILayout.Button("Stop and Restore")) StopPreview();
            }
        }

        private void RefreshPresets()
        {
            TweenPresetRegistry.Refresh();
            _presets.Clear();
            _presets.AddRange(TweenPresetRegistry.Presets.OrderBy(preset => preset.PresetName, StringComparer.Ordinal));

            _families.Clear();
            _families.Add(AllFamilies);
            _families.AddRange(_presets.Select(preset => PresetFamilyClassifier.GetFamilyName(preset.PresetName))
                .Where(family => !string.IsNullOrEmpty(family))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(family => family, StringComparer.Ordinal));

            _familyIndex = Mathf.Clamp(_familyIndex, 0, _families.Count - 1);
            if (_selectedPreset == null || !_presets.Contains(_selectedPreset)) _selectedPreset = _presets.FirstOrDefault();
            Repaint();
        }

        private List<ITweenPreset> GetVisiblePresets()
        {
            string family = _families.Count > 0 ? _families[_familyIndex] : AllFamilies;
            return _presets.Where(preset =>
                    (string.Equals(family, AllFamilies, StringComparison.Ordinal) ||
                     string.Equals(PresetFamilyClassifier.GetFamilyName(preset.PresetName), family, StringComparison.Ordinal)) &&
                    (string.IsNullOrWhiteSpace(_search) ||
                     preset.PresetName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                     preset.Description.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();
        }

        private void StartPreview(GameObject target)
        {
            StopPreview();
            if (target == null || _selectedPreset == null) return;

            _previewSnapshot = new PreviewSnapshot(target);
            _previewTween = _selectedPreset.CreateTween(target);
            if (_previewTween == null)
            {
                _previewSnapshot.Restore();
                _previewSnapshot = null;
                ShowNotification(new GUIContent("Preset did not create a tween"));
                return;
            }

            _previewTween.SetUpdate(UpdateType.Manual);
            _previewTween.Play();
            _lastPreviewTime = EditorApplication.timeSinceStartup;
        }

        private void StopPreview()
        {
            if (_previewTween != null && _previewTween.IsActive()) _previewTween.Kill(false);
            _previewTween = null;

            if (_previewSnapshot != null)
            {
                _previewSnapshot.Restore();
                _previewSnapshot = null;
            }

            SceneView.RepaintAll();
        }

        private void UpdatePreview()
        {
            if (_previewTween == null || !_previewTween.IsActive()) return;

            double now = EditorApplication.timeSinceStartup;
            float deltaTime = Mathf.Min((float)(now - _lastPreviewTime), 0.1f);
            _lastPreviewTime = now;
            DOTween.ManualUpdate(deltaTime, deltaTime);

            Repaint();
            SceneView.RepaintAll();
        }

        private static GameObject GetPreviewTarget()
        {
            var target = Selection.activeGameObject;
            return target != null && !EditorUtility.IsPersistent(target) ? target : null;
        }

        private static string BuildExample(ITweenPreset preset) => $"target.Tween().Preset<{preset.GetType().Name}>().Play();";

        private static void DrawValue(string label, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(105f));
                EditorGUILayout.LabelField(value);
            }
        }

        private sealed class PreviewSnapshot
        {
            private readonly GameObject _target;
            private readonly Vector3 _localPosition;
            private readonly Quaternion _localRotation;
            private readonly Vector3 _localScale;
            private readonly bool _activeSelf;
            private readonly CanvasGroup _canvasGroup;
            private readonly float _canvasAlpha;
            private readonly Graphic _graphic;
            private readonly Color _graphicColor;
            private readonly SpriteRenderer _spriteRenderer;
            private readonly Color _spriteColor;
            private readonly TMP_Text _tmpText;
            private readonly Color _tmpColor;
            private readonly Material _material;
            private readonly Color _materialColor;

            public PreviewSnapshot(GameObject target)
            {
                _target = target;
                _localPosition = target.transform.localPosition;
                _localRotation = target.transform.localRotation;
                _localScale = target.transform.localScale;
                _activeSelf = target.activeSelf;

                _canvasGroup = target.GetComponent<CanvasGroup>();
                _canvasAlpha = _canvasGroup == null ? 1f : _canvasGroup.alpha;
                _graphic = target.GetComponent<Graphic>();
                _graphicColor = _graphic == null ? Color.white : _graphic.color;
                _spriteRenderer = target.GetComponent<SpriteRenderer>();
                _spriteColor = _spriteRenderer == null ? Color.white : _spriteRenderer.color;
                _tmpText = target.GetComponent<TMP_Text>();
                _tmpColor = _tmpText == null ? Color.white : _tmpText.color;
                var renderer = target.GetComponent<Renderer>();
                _material = renderer != null && renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_Color")
                    ? renderer.material
                    : null;
                _materialColor = _material == null ? Color.white : _material.color;
            }

            public void Restore()
            {
                if (_target == null) return;

                _target.transform.DOKill(false);
                if (_canvasGroup != null) _canvasGroup.DOKill(false);
                if (_graphic != null) _graphic.DOKill(false);
                if (_spriteRenderer != null) _spriteRenderer.DOKill(false);
                if (_tmpText != null) _tmpText.DOKill(false);
                if (_material != null) _material.DOKill(false);

                _target.transform.localPosition = _localPosition;
                _target.transform.localRotation = _localRotation;
                _target.transform.localScale = _localScale;
                _target.SetActive(_activeSelf);
                if (_canvasGroup != null) _canvasGroup.alpha = _canvasAlpha;
                if (_graphic != null) _graphic.color = _graphicColor;
                if (_spriteRenderer != null) _spriteRenderer.color = _spriteColor;
                if (_tmpText != null) _tmpText.color = _tmpColor;
                if (_material != null) _material.color = _materialColor;
            }
        }
    }
}
