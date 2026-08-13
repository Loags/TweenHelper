using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace LB.TweenHelper.Editor
{
    internal sealed class PresetBrowserPreview : IDisposable
    {
        private const float MinimumLoopPreviewDuration = 2f;
        private const float MaximumLoopPreviewDuration = 6f;
        private const float LoopPreviewCycles = 3f;
        private static readonly Color PreviewBackground = new Color(0.035f, 0.047f, 0.08f, 1f);
        private static readonly Color CubeColor = new Color(0.055f, 0.48f, 0.94f, 1f);
        private static readonly Color CubeEmissionColor = new Color(0.015f, 0.1f, 0.22f, 1f);

        private PreviewRenderUtility _preview;
        private GameObject _stageRoot;
        private GameObject _singleTarget;
        private readonly List<GameObject> _collectionTargets = new List<GameObject>();
        private Material _cubeMaterial;
        private Material _groundMaterial;
        private Tween _activeTween;
        private PresetBrowserEntry _entry;
        private double _lastUpdateTime;
        private float _elapsedTime;
        private float _playbackDuration;
        private bool _isPlaying;

        public bool IsPlaying => _isPlaying && _activeTween != null && _activeTween.IsActive();

        public void SetEntry(PresetBrowserEntry entry)
        {
            _entry = entry;
            RebuildStage();
        }

        public void Play()
        {
            if (_entry == null) return;
            RebuildStage();

            Tween tween = _entry.Kind == PresetBrowserEntryKind.Preset
                ? CreatePresetTween(_entry.Preset)
                : CreateCollectionTween(_entry.CollectionKind);

            if (tween == null) throw new InvalidOperationException($"'{_entry.Name}' did not create a tween.");
            tween.Pause();
            tween.SetAutoKill(false);
            tween.Goto(0f, false);
            _activeTween = tween;
            _elapsedTime = 0f;
            _playbackDuration = ResolvePlaybackDuration(tween);
            _isPlaying = true;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        public bool Update()
        {
            if (!_isPlaying || _activeTween == null || !_activeTween.IsActive()) return false;

            double now = EditorApplication.timeSinceStartup;
            float deltaTime = Mathf.Min((float)(now - _lastUpdateTime), 0.1f);
            _lastUpdateTime = now;
            _elapsedTime += deltaTime;
            float duration = _activeTween.Duration(true);
            float targetTime = float.IsInfinity(duration) ? _elapsedTime : Mathf.Min(_elapsedTime, duration);
            _activeTween.Goto(targetTime, false);
            if (_elapsedTime >= _playbackDuration) _isPlaying = false;
            return true;
        }

        public void Draw(Rect rect)
        {
            if (Event.current.type != EventType.Repaint || rect.width < 2f || rect.height < 2f) return;
            if (_preview == null) RebuildStage();

            EditorGUI.DrawRect(rect, PreviewBackground);
            _preview.BeginPreview(rect, GUIStyle.none);
            _preview.Render(true);
            Texture texture = _preview.EndPreview();
            GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, false);
        }

        public void Dispose() => CleanupStage();

        private void RebuildStage()
        {
            CleanupStage();
            if (_entry == null) return;

            _preview = new PreviewRenderUtility();
            ConfigureCameraAndLights();
            CreateMaterials();

            _stageRoot = new GameObject("Tween Helper Preview Stage") { hideFlags = HideFlags.HideAndDontSave };
            switch (_entry.PreviewKind)
            {
                case PresetBrowserPreviewKind.Single:
                    BuildSingleStage();
                    break;
                case PresetBrowserPreviewKind.List:
                    BuildListStage(5);
                    break;
                case PresetBrowserPreviewKind.Grid:
                    BuildGridStage();
                    break;
                case PresetBrowserPreviewKind.LoadingDots:
                    BuildLoadingDotsStage();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _preview.AddSingleGO(_stageRoot);
        }

        private void BuildSingleStage()
        {
            _singleTarget = CreateCube("Preview Cube", Vector3.zero, new Vector3(1.4f, 1.2f, 1f));
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Preview Ground";
            ground.hideFlags = HideFlags.HideAndDontSave;
            ground.transform.SetParent(_stageRoot.transform, false);
            ground.transform.localPosition = new Vector3(0f, -1.15f, 0f);
            ground.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            DestroyCollider(ground);
            ground.GetComponent<Renderer>().sharedMaterial = _groundMaterial;
        }

        private void BuildListStage(int count)
        {
            const float spacing = 1.55f;
            float startX = -(count - 1) * spacing * 0.5f;
            for (int i = 0; i < count; i++)
            {
                _collectionTargets.Add(CreateCube($"List Cube {i + 1}", new Vector3(startX + i * spacing, 0f, 0f), Vector3.one * 0.72f));
            }
        }

        private void BuildGridStage()
        {
            const float spacing = 1.55f;
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    int index = row * 3 + column;
                    var position = new Vector3((column - 1) * spacing, (1 - row) * spacing, 0f);
                    _collectionTargets.Add(CreateCube($"Grid Cube {index + 1}", position, Vector3.one * 0.58f));
                }
            }
        }

        private void BuildLoadingDotsStage()
        {
            const float spacing = 1.8f;
            for (int i = 0; i < 3; i++)
            {
                _collectionTargets.Add(CreateCube($"Loading Cube {i + 1}", new Vector3((i - 1) * spacing, 0f, 0f), Vector3.one * 0.68f));
            }
        }

        private GameObject CreateCube(string name, Vector3 localPosition, Vector3 localScale)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.hideFlags = HideFlags.HideAndDontSave;
            cube.transform.SetParent(_stageRoot.transform, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = localScale;
            DestroyCollider(cube);
            cube.GetComponent<Renderer>().sharedMaterial = _cubeMaterial;
            return cube;
        }

        private Tween CreatePresetTween(ITweenPreset preset)
        {
            if (preset == null || _singleTarget == null) return null;
            if (!preset.CanApplyTo(_singleTarget)) throw new InvalidOperationException($"'{preset.PresetName}' is not compatible with the internal preview cube.");
            return preset.CreateTween(_singleTarget);
        }

        private Tween CreateCollectionTween(PresetBrowserCollectionKind kind)
        {
            var options = TweenOptions.WithUpdateType(UpdateType.Manual);
            switch (kind)
            {
                case PresetBrowserCollectionKind.ListStaggerIn:
                    return _collectionTargets.ListStaggerIn(_stageRoot, options: options);
                case PresetBrowserCollectionKind.ListStaggerOut:
                    return _collectionTargets.ListStaggerOut(_stageRoot, options: options);
                case PresetBrowserCollectionKind.GridWave:
                    return _collectionTargets.GridWave(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.GridRipple:
                    return _collectionTargets.GridRipple(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.LoadingDots:
                    return _collectionTargets.LoadingDots(_stageRoot, options: options);
                case PresetBrowserCollectionKind.OrderFirstToLast:
                    return CreateOrderTween(StaggerOrder.FirstToLast, options);
                case PresetBrowserCollectionKind.OrderLastToFirst:
                    return CreateOrderTween(StaggerOrder.LastToFirst, options);
                case PresetBrowserCollectionKind.OrderFromCenter:
                    return CreateOrderTween(StaggerOrder.FromCenter, options);
                case PresetBrowserCollectionKind.OrderToCenter:
                    return CreateOrderTween(StaggerOrder.ToCenter, options);
                case PresetBrowserCollectionKind.OrderRandom:
                    return CreateOrderTween(StaggerOrder.Random, options, 1729);
                case PresetBrowserCollectionKind.GridWaveRightToLeft:
                    return _collectionTargets.GridWave(_stageRoot, 3, GridWaveDirection.RightToLeft, options: options);
                case PresetBrowserCollectionKind.GridWaveTopToBottom:
                    return _collectionTargets.GridWave(_stageRoot, 3, GridWaveDirection.TopToBottom, options: options);
                case PresetBrowserCollectionKind.GridWaveBottomToTop:
                    return _collectionTargets.GridWave(_stageRoot, 3, GridWaveDirection.BottomToTop, options: options);
                case PresetBrowserCollectionKind.GridDiagonalWave:
                    return _collectionTargets.GridDiagonalWave(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.GridSpiral:
                    return _collectionTargets.GridSpiral(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.GridCheckerboard:
                    return _collectionTargets.GridCheckerboard(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.CollectionBurstIn:
                    return _collectionTargets.CollectionBurstIn(_stageRoot, Vector3.zero, options: options);
                case PresetBrowserCollectionKind.CollectionBurstOut:
                    return _collectionTargets.CollectionBurstOut(_stageRoot, Vector3.zero, 2f, options: options);
                case PresetBrowserCollectionKind.CollectionGatherTo:
                    return _collectionTargets.CollectionGatherTo(_stageRoot, Vector3.zero, options: options);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown collection preview.");
            }
        }

        private Tween CreateOrderTween(StaggerOrder order, TweenOptions options, int seed = 0)
        {
            return _collectionTargets.TweenStagger(_stageRoot)
                .Preset<PulseScalePreset>(0.36f, options)
                .Order(order)
                .DelayBetween(0.14f)
                .Seed(seed)
                .WithUpdate(UpdateType.Manual)
                .Play();
        }

        private void ConfigureCameraAndLights()
        {
            Camera camera = _preview.camera;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = PreviewBackground;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 100f;
            camera.fieldOfView = 38f;
            camera.transform.position = new Vector3(7.2f, 4.8f, -12f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.15f, 0f) - camera.transform.position);

            _preview.lights[0].intensity = 1.45f;
            _preview.lights[0].color = new Color(0.72f, 0.92f, 1f);
            _preview.lights[0].transform.rotation = Quaternion.Euler(38f, 34f, 0f);
            _preview.lights[1].intensity = 0.95f;
            _preview.lights[1].color = new Color(0.5f, 0.34f, 1f);
            _preview.lights[1].transform.rotation = Quaternion.Euler(340f, 218f, 0f);
            _preview.ambientColor = new Color(0.18f, 0.21f, 0.32f);
        }

        private void CreateMaterials()
        {
            Shader cubeShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Unlit/Transparent");
            if (cubeShader == null) throw new InvalidOperationException("No supported shader was found for the Preset Browser preview.");

            _cubeMaterial = new Material(cubeShader) { hideFlags = HideFlags.HideAndDontSave, name = "Tween Helper Polished Blue Preview" };
            SetMaterialColor(_cubeMaterial, CubeColor);
            if (_cubeMaterial.HasProperty("_Metallic")) _cubeMaterial.SetFloat("_Metallic", 0.16f);
            if (_cubeMaterial.HasProperty("_Smoothness")) _cubeMaterial.SetFloat("_Smoothness", 0.72f);
            if (_cubeMaterial.HasProperty("_Glossiness")) _cubeMaterial.SetFloat("_Glossiness", 0.72f);
            if (_cubeMaterial.HasProperty("_EmissionColor"))
            {
                _cubeMaterial.SetColor("_EmissionColor", CubeEmissionColor);
                _cubeMaterial.EnableKeyword("_EMISSION");
            }
            ConfigureTransparentMaterial(_cubeMaterial);

            _groundMaterial = new Material(cubeShader) { hideFlags = HideFlags.HideAndDontSave, name = "Tween Helper Preview Ground" };
            SetMaterialColor(_groundMaterial, new Color(0.055f, 0.07f, 0.11f, 1f));
            _groundMaterial.renderQueue = (int)RenderQueue.Geometry;
        }

        private static void ConfigureTransparentMaterial(Material material)
        {
            if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
            if (material.HasProperty("_Blend")) material.SetFloat("_Blend", 0f);
            if (material.HasProperty("_Mode")) material.SetFloat("_Mode", 3f);
            if (material.HasProperty("_SrcBlend")) material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            if (material.HasProperty("_DstBlend")) material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0f);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        private static void SetMaterialColor(Material material, Color color)
        {
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        }

        private static void DestroyCollider(GameObject target)
        {
            Collider collider = target.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
        }

        private static float ResolvePlaybackDuration(Tween tween)
        {
            float fullDuration = tween.Duration(true);
            if (!float.IsInfinity(fullDuration)) return fullDuration;

            float cycleDuration = tween.Duration(false);
            if (float.IsInfinity(cycleDuration) || cycleDuration <= 0f) return MaximumLoopPreviewDuration;
            return Mathf.Clamp(cycleDuration * LoopPreviewCycles, MinimumLoopPreviewDuration, MaximumLoopPreviewDuration);
        }

        private void CleanupStage()
        {
            if (_activeTween != null && _activeTween.IsActive()) _activeTween.Kill(false);
            _activeTween = null;
            _elapsedTime = 0f;
            _playbackDuration = 0f;
            _isPlaying = false;
            _singleTarget = null;
            _collectionTargets.Clear();

            if (_preview != null)
            {
                _preview.Cleanup();
                _preview = null;
            }

            if (_cubeMaterial != null) UnityEngine.Object.DestroyImmediate(_cubeMaterial);
            if (_groundMaterial != null) UnityEngine.Object.DestroyImmediate(_groundMaterial);
            _cubeMaterial = null;
            _groundMaterial = null;
            _stageRoot = null;
        }
    }
}
