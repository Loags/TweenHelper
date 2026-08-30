using DG.DOTweenEditor;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    [InitializeOnLoad]
    internal static class TweenRecipePreviewSession
    {
        private static readonly List<ObjectDirtyState> DirtyObjects = new List<ObjectDirtyState>();
        private static readonly List<SceneDirtyState> DirtyScenes = new List<SceneDirtyState>();
        private static readonly List<RendererState> RendererStates = new List<RendererState>();
        private static readonly List<TargetState> TargetStates = new List<TargetState>();
        private static readonly MethodInfo ClearSceneDirtinessMethod = typeof(EditorSceneManager).GetMethod("ClearSceneDirtiness", BindingFlags.Static | BindingFlags.NonPublic);

        private static TweenPlayer _player;
        private static TweenHandle _handle;
        private static int _undoGroup = -1;
        private static bool _ownsDotweenPreview;
        private static bool _restoring;
        private static bool _stopScheduled;
        private static string _statusMessage = "Preview is stopped.";

        public static bool IsPreviewing => _player != null && _handle != null;
        public static TweenPlayer Player => _player;
        public static string StatusMessage => _statusMessage;

        static TweenRecipePreviewSession()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () => Stop("Preview stopped for script reload.");
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.quitting += () => Stop("Preview stopped because Unity is closing.");
            Undo.undoRedoPerformed += () =>
            {
                if (IsPreviewing && !_restoring) ScheduleStop("Preview stopped for Undo/Redo.");
            };
        }

        public static bool CanPreview(TweenPlayer player, out string reason)
        {
            if (Application.isPlaying)
            {
                reason = "Edit Mode preview is unavailable in Play Mode.";
                return false;
            }

            if (player == null)
            {
                reason = "Select a GameObject with a TweenPlayer.";
                return false;
            }

            if (EditorUtility.IsPersistent(player))
            {
                reason = "Open the prefab in Prefab Mode or select a scene instance.";
                return false;
            }

            Scene scene = player.gameObject.scene;
            if (!scene.IsValid() || !scene.isLoaded)
            {
                reason = "The TweenPlayer must belong to an open scene or Prefab Stage.";
                return false;
            }

            TweenRecipeValidationResult validation = player.Validate();
            if (!validation.IsValid)
            {
                reason = validation.GetSummary();
                return false;
            }

            if (DOTweenEditorPreview.isPreviewing && !_ownsDotweenPreview)
            {
                reason = "Another DOTween Editor preview is active.";
                return false;
            }

            reason = "Ready to preview.";
            return true;
        }

        public static bool Start(TweenPlayer player, out string message)
        {
            if (IsPreviewing) Stop("Previous recipe preview restored.");
            if (!CanPreview(player, out message))
            {
                _statusMessage = message;
                return false;
            }

            _player = player;
            try
            {
                CaptureState(player);
                if (!TweenRecipeExecutor.TryBuild(player.Recipe, player.Bindings, player.gameObject, out _handle, out TweenRecipeValidationResult validation, false, UpdateType.Manual))
                {
                    message = validation.GetSummary();
                    RestoreState(message);
                    return false;
                }

                _handle.OnComplete(ScheduleCompletedStop);
                _handle.OnKill(ScheduleKilledStop);
                DOTweenEditorPreview.PrepareTweenForPreview(_handle.Tween, false, true, true);
                _ownsDotweenPreview = true;
                DOTweenEditorPreview.Start(Repaint);
                _statusMessage = $"Previewing '{player.Recipe.name}' on '{player.name}'.";
                message = _statusMessage;
                Repaint();
                return true;
            }
            catch (Exception exception)
            {
                message = $"Preview failed: {exception.Message}";
                RestoreState(message);
                return false;
            }
        }

        public static void Stop(string message = "Preview stopped and all target state was restored.")
        {
            if (!IsPreviewing && _undoGroup < 0)
            {
                _statusMessage = message;
                return;
            }

            RestoreState(message);
        }

        private static void CaptureState(TweenPlayer player)
        {
            DirtyObjects.Clear();
            DirtyScenes.Clear();
            RendererStates.Clear();
            TargetStates.Clear();

            Undo.IncrementCurrentGroup();
            _undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Tween Recipe Preview");

            var capturedObjects = new HashSet<UnityEngine.Object>();
            var capturedScenes = new HashSet<ulong>();
            var capturedTargets = new HashSet<GameObject>();
            CaptureTarget(player.gameObject, capturedTargets, capturedObjects, capturedScenes);
            for (int i = 0; i < player.Bindings.Count; i++)
            {
                TweenPlayerBinding binding = player.Bindings[i];
                if (binding == null) continue;
                CaptureTarget(binding.Target, capturedTargets, capturedObjects, capturedScenes);
                IReadOnlyList<GameObject> targets = binding.Targets;
                if (targets == null) continue;
                for (int targetIndex = 0; targetIndex < targets.Count; targetIndex++)
                {
                    CaptureTarget(targets[targetIndex], capturedTargets, capturedObjects, capturedScenes);
                }
            }

            Undo.CollapseUndoOperations(_undoGroup);
        }

        private static void CaptureTarget(GameObject target, ISet<GameObject> capturedTargets, ISet<UnityEngine.Object> capturedObjects, ISet<ulong> capturedScenes)
        {
            if (target == null || !capturedTargets.Add(target)) return;
            CaptureScene(target.scene, capturedScenes);
            Undo.RegisterFullObjectHierarchyUndo(target, "Tween Recipe Preview");

            Transform[] transforms = target.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                GameObject gameObject = transforms[i].gameObject;
                bool isNewGameObject = !capturedObjects.Contains(gameObject);
                CaptureDirtyObject(gameObject, capturedObjects);
                if (isNewGameObject) TargetStates.Add(new TargetState(gameObject));
                Component[] components = gameObject.GetComponents<Component>();
                for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
                {
                    CaptureDirtyObject(components[componentIndex], capturedObjects);
                }
            }

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                RendererStates.Add(new RendererState(renderer));
                Material[] materials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                {
                    Material material = materials[materialIndex];
                    if (material == null || !capturedObjects.Add(material)) continue;
                    DirtyObjects.Add(new ObjectDirtyState(material, EditorUtility.IsDirty(material)));
                    Undo.RegisterCompleteObjectUndo(material, "Tween Recipe Preview");
                }
            }
        }

        private static void CaptureScene(Scene scene, ISet<ulong> capturedScenes)
        {
            if (!scene.IsValid() || !capturedScenes.Add(scene.handle.GetRawData())) return;
            DirtyScenes.Add(new SceneDirtyState(scene, scene.isDirty));
        }

        private static void CaptureDirtyObject(UnityEngine.Object target, ISet<UnityEngine.Object> capturedObjects)
        {
            if (target == null || !capturedObjects.Add(target)) return;
            DirtyObjects.Add(new ObjectDirtyState(target, EditorUtility.IsDirty(target)));
        }

        private static void RestoreState(string message)
        {
            if (_restoring) return;
            _restoring = true;
            _stopScheduled = false;

            try
            {
                if (_ownsDotweenPreview)
                {
                    DOTweenEditorPreview.Stop(false, true);
                }
                else if (_handle != null && _handle.IsActive)
                {
                    _handle.Kill();
                }

                for (int i = TargetStates.Count - 1; i >= 0; i--) TargetStates[i].RemoveAddedComponents();
                if (_undoGroup >= 0) Undo.RevertAllDownToGroup(_undoGroup);

                for (int i = 0; i < TargetStates.Count; i++) TargetStates[i].RestoreProperties();
                for (int i = TargetStates.Count - 1; i >= 0; i--) TargetStates[i].RestoreActiveState();
                for (int i = 0; i < RendererStates.Count; i++) RendererStates[i].Restore();
                for (int i = 0; i < DirtyObjects.Count; i++) DirtyObjects[i].RestoreDirtyState();
                for (int i = 0; i < DirtyScenes.Count; i++) DirtyScenes[i].RestoreDirtyState();

                _statusMessage = message;
            }
            finally
            {
                _player = null;
                _handle = null;
                _undoGroup = -1;
                _ownsDotweenPreview = false;
                DirtyObjects.Clear();
                DirtyScenes.Clear();
                RendererStates.Clear();
                TargetStates.Clear();
                _restoring = false;
                Repaint();
            }
        }

        private static void ScheduleCompletedStop()
        {
            ScheduleStop("Preview completed and all target state was restored.");
        }

        private static void ScheduleKilledStop()
        {
            if (!_restoring) ScheduleStop("Preview ended and all target state was restored.");
        }

        private static void ScheduleStop(string message)
        {
            if (_stopScheduled) return;
            _stopScheduled = true;
            EditorApplication.delayCall += () =>
            {
                if (_stopScheduled) Stop(message);
            };
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && IsPreviewing)
            {
                EditorApplication.isPlaying = false;
                ScheduleStop("Preview restored. Play Mode entry was canceled; press Play again.");
            }
            else if (state == PlayModeStateChange.EnteredPlayMode && IsPreviewing)
            {
                Stop("Preview stopped for Play Mode.");
            }
        }

        private static void Repaint()
        {
            SceneView.RepaintAll();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        private readonly struct ObjectDirtyState
        {
            private readonly UnityEngine.Object _target;
            private readonly bool _wasDirty;

            public ObjectDirtyState(UnityEngine.Object target, bool wasDirty)
            {
                _target = target;
                _wasDirty = wasDirty;
            }

            public void RestoreDirtyState()
            {
                if (_target != null && !_wasDirty) EditorUtility.ClearDirty(_target);
            }
        }

        private readonly struct SceneDirtyState
        {
            private readonly Scene _scene;
            private readonly bool _wasDirty;

            public SceneDirtyState(Scene scene, bool wasDirty)
            {
                _scene = scene;
                _wasDirty = wasDirty;
            }

            public void RestoreDirtyState()
            {
                if (_scene.IsValid() && !_wasDirty && _scene.isDirty) ClearSceneDirtinessMethod?.Invoke(null, new object[] { _scene });
            }
        }

        private sealed class TargetState
        {
            private readonly GameObject _gameObject;
            private readonly Transform _transform;
            private readonly Transform _parent;
            private readonly int _siblingIndex;
            private readonly bool _activeSelf;
            private readonly int _layer;
            private readonly HashSet<Component> _components;
            private readonly Vector3 _localPosition;
            private readonly Quaternion _localRotation;
            private readonly Vector3 _localScale;
            private readonly RectTransform _rectTransform;
            private readonly Vector2 _anchorMin;
            private readonly Vector2 _anchorMax;
            private readonly Vector2 _pivot;
            private readonly Vector2 _sizeDelta;
            private readonly Vector3 _anchoredPosition3D;
            private readonly Behaviour[] _behaviours;
            private readonly bool[] _behaviourEnabled;
            private readonly CanvasGroup[] _canvasGroups;
            private readonly float[] _canvasGroupAlpha;
            private readonly bool[] _canvasGroupInteractable;
            private readonly bool[] _canvasGroupBlocksRaycasts;
            private readonly bool[] _canvasGroupIgnoreParent;
            private readonly Graphic[] _graphics;
            private readonly Color[] _graphicColors;
            private readonly TMP_Text[] _texts;
            private readonly string[] _textValues;
            private readonly int[] _maxVisibleCharacters;
            private readonly Slider[] _sliders;
            private readonly float[] _sliderValues;
            private readonly Camera[] _cameras;
            private readonly float[] _cameraFieldOfViews;
            private readonly Light[] _lights;
            private readonly float[] _lightIntensities;
            private readonly AudioSource[] _audioSources;
            private readonly float[] _audioVolumes;
            private readonly ParticleSystem[] _particleSystems;
            private readonly float[] _particleEmissionMultipliers;

            public TargetState(GameObject gameObject)
            {
                _gameObject = gameObject;
                _transform = gameObject.transform;
                _parent = _transform.parent;
                _siblingIndex = _transform.GetSiblingIndex();
                _activeSelf = gameObject.activeSelf;
                _layer = gameObject.layer;
                _components = new HashSet<Component>(gameObject.GetComponents<Component>());
                _localPosition = _transform.localPosition;
                _localRotation = _transform.localRotation;
                _localScale = _transform.localScale;

                _rectTransform = _transform as RectTransform;
                if (_rectTransform != null)
                {
                    _anchorMin = _rectTransform.anchorMin;
                    _anchorMax = _rectTransform.anchorMax;
                    _pivot = _rectTransform.pivot;
                    _sizeDelta = _rectTransform.sizeDelta;
                    _anchoredPosition3D = _rectTransform.anchoredPosition3D;
                }
                else
                {
                    _anchorMin = default;
                    _anchorMax = default;
                    _pivot = default;
                    _sizeDelta = default;
                    _anchoredPosition3D = default;
                }

                _behaviours = gameObject.GetComponents<Behaviour>();
                _behaviourEnabled = new bool[_behaviours.Length];
                for (int i = 0; i < _behaviours.Length; i++) _behaviourEnabled[i] = _behaviours[i].enabled;

                _canvasGroups = gameObject.GetComponents<CanvasGroup>();
                _canvasGroupAlpha = new float[_canvasGroups.Length];
                _canvasGroupInteractable = new bool[_canvasGroups.Length];
                _canvasGroupBlocksRaycasts = new bool[_canvasGroups.Length];
                _canvasGroupIgnoreParent = new bool[_canvasGroups.Length];
                for (int i = 0; i < _canvasGroups.Length; i++)
                {
                    _canvasGroupAlpha[i] = _canvasGroups[i].alpha;
                    _canvasGroupInteractable[i] = _canvasGroups[i].interactable;
                    _canvasGroupBlocksRaycasts[i] = _canvasGroups[i].blocksRaycasts;
                    _canvasGroupIgnoreParent[i] = _canvasGroups[i].ignoreParentGroups;
                }

                _graphics = gameObject.GetComponents<Graphic>();
                _graphicColors = new Color[_graphics.Length];
                for (int i = 0; i < _graphics.Length; i++) _graphicColors[i] = _graphics[i].color;

                _texts = gameObject.GetComponents<TMP_Text>();
                _textValues = new string[_texts.Length];
                _maxVisibleCharacters = new int[_texts.Length];
                for (int i = 0; i < _texts.Length; i++)
                {
                    _textValues[i] = _texts[i].text;
                    _maxVisibleCharacters[i] = _texts[i].maxVisibleCharacters;
                }

                _sliders = gameObject.GetComponents<Slider>();
                _sliderValues = new float[_sliders.Length];
                for (int i = 0; i < _sliders.Length; i++) _sliderValues[i] = _sliders[i].value;

                _cameras = gameObject.GetComponents<Camera>();
                _cameraFieldOfViews = new float[_cameras.Length];
                for (int i = 0; i < _cameras.Length; i++) _cameraFieldOfViews[i] = _cameras[i].fieldOfView;

                _lights = gameObject.GetComponents<Light>();
                _lightIntensities = new float[_lights.Length];
                for (int i = 0; i < _lights.Length; i++) _lightIntensities[i] = _lights[i].intensity;

                _audioSources = gameObject.GetComponents<AudioSource>();
                _audioVolumes = new float[_audioSources.Length];
                for (int i = 0; i < _audioSources.Length; i++) _audioVolumes[i] = _audioSources[i].volume;

                _particleSystems = gameObject.GetComponents<ParticleSystem>();
                _particleEmissionMultipliers = new float[_particleSystems.Length];
                for (int i = 0; i < _particleSystems.Length; i++) _particleEmissionMultipliers[i] = _particleSystems[i].emission.rateOverTimeMultiplier;
            }

            public void RemoveAddedComponents()
            {
                if (_gameObject == null) return;
                Component[] currentComponents = _gameObject.GetComponents<Component>();
                for (int i = currentComponents.Length - 1; i >= 0; i--)
                {
                    Component component = currentComponents[i];
                    if (component != null && !_components.Contains(component)) UnityEngine.Object.DestroyImmediate(component);
                }
            }

            public void RestoreProperties()
            {
                if (_gameObject == null || _transform == null) return;
                if (_transform.parent != _parent) _transform.SetParent(_parent, false);
                _transform.localPosition = _localPosition;
                _transform.localRotation = _localRotation;
                _transform.localScale = _localScale;
                _gameObject.layer = _layer;
                _transform.SetSiblingIndex(_siblingIndex);

                if (_rectTransform != null)
                {
                    _rectTransform.anchorMin = _anchorMin;
                    _rectTransform.anchorMax = _anchorMax;
                    _rectTransform.pivot = _pivot;
                    _rectTransform.sizeDelta = _sizeDelta;
                    _rectTransform.anchoredPosition3D = _anchoredPosition3D;
                }

                for (int i = 0; i < _behaviours.Length; i++) if (_behaviours[i] != null) _behaviours[i].enabled = _behaviourEnabled[i];
                for (int i = 0; i < _canvasGroups.Length; i++)
                {
                    if (_canvasGroups[i] == null) continue;
                    _canvasGroups[i].alpha = _canvasGroupAlpha[i];
                    _canvasGroups[i].interactable = _canvasGroupInteractable[i];
                    _canvasGroups[i].blocksRaycasts = _canvasGroupBlocksRaycasts[i];
                    _canvasGroups[i].ignoreParentGroups = _canvasGroupIgnoreParent[i];
                }
                for (int i = 0; i < _graphics.Length; i++) if (_graphics[i] != null) _graphics[i].color = _graphicColors[i];
                for (int i = 0; i < _texts.Length; i++)
                {
                    if (_texts[i] == null) continue;
                    _texts[i].text = _textValues[i];
                    _texts[i].maxVisibleCharacters = _maxVisibleCharacters[i];
                }
                for (int i = 0; i < _sliders.Length; i++) if (_sliders[i] != null) _sliders[i].value = _sliderValues[i];
                for (int i = 0; i < _cameras.Length; i++) if (_cameras[i] != null) _cameras[i].fieldOfView = _cameraFieldOfViews[i];
                for (int i = 0; i < _lights.Length; i++) if (_lights[i] != null) _lights[i].intensity = _lightIntensities[i];
                for (int i = 0; i < _audioSources.Length; i++) if (_audioSources[i] != null) _audioSources[i].volume = _audioVolumes[i];
                for (int i = 0; i < _particleSystems.Length; i++)
                {
                    if (_particleSystems[i] == null) continue;
                    ParticleSystem.EmissionModule emission = _particleSystems[i].emission;
                    emission.rateOverTimeMultiplier = _particleEmissionMultipliers[i];
                }
            }

            public void RestoreActiveState()
            {
                if (_gameObject != null && _gameObject.activeSelf != _activeSelf) _gameObject.SetActive(_activeSelf);
            }
        }

        private sealed class RendererState
        {
            private readonly Renderer _renderer;
            private readonly MaterialPropertyBlock _propertyBlock;
            private readonly Material[] _materials;
            private readonly bool _enabled;
            private readonly SpriteRenderer _spriteRenderer;
            private readonly Color _spriteColor;

            public RendererState(Renderer renderer)
            {
                _renderer = renderer;
                _propertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(_propertyBlock);
                _materials = renderer.sharedMaterials;
                _enabled = renderer.enabled;
                _spriteRenderer = renderer as SpriteRenderer;
                _spriteColor = _spriteRenderer != null ? _spriteRenderer.color : default;
            }

            public void Restore()
            {
                if (_renderer == null) return;
                _renderer.sharedMaterials = _materials;
                _renderer.enabled = _enabled;
                if (_spriteRenderer != null) _spriteRenderer.color = _spriteColor;
                _renderer.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}
