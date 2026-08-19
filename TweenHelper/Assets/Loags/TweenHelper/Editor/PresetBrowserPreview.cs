using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
        private static readonly Color AccentColor = new Color(0.1f, 0.86f, 1f, 1f);
        private static readonly Color SecondaryColor = new Color(0.56f, 0.3f, 1f, 1f);

        private readonly List<GameObject> _collectionTargets = new List<GameObject>();
        private readonly List<GameObject> _uiChildren = new List<GameObject>();
        private readonly List<PreviewProxyBinding> _proxyBindings = new List<PreviewProxyBinding>();
        private PreviewRenderUtility _preview;
        private GameObject _stageRoot;
        private GameObject _singleTarget;
        private GameObject _incomingTarget;
        private GameObject _backdrop;
        private GameObject _meterTrack;
        private GameObject _meterFill;
        private RectTransform _uiDestination;
        private TMP_Text _textTarget;
        private TMP_Text _valueText;
        private Image _fillImage;
        private Image _sliderFillImage;
        private Slider _slider;
        private AudioSource _audioSource;
        private Light _light;
        private ParticleSystem _particles;
        private Renderer _propertyRenderer;
        private Transform _focusTarget;
        private Material _cubeMaterial;
        private Material _groundMaterial;
        private Material _secondaryMaterial;
        private Texture2D _whiteTexture;
        private Sprite _whiteSprite;
        private Tween _activeTween;
        private PresetBrowserEntry _entry;
        private int _collectionOptionIndex;
        private double _lastUpdateTime;
        private float _elapsedTime;
        private float _playbackDuration;
        private bool _isPlaying;

        public bool IsPlaying => _isPlaying && _activeTween != null && _activeTween.IsActive();

        public void SetEntry(PresetBrowserEntry entry, int collectionOptionIndex = 0)
        {
            _entry = entry;
            _collectionOptionIndex = collectionOptionIndex;
            RebuildStage();
        }

        public void Play()
        {
            if (_entry == null) return;
            RebuildStage();

            Tween tween;
            switch (_entry.Kind)
            {
                case PresetBrowserEntryKind.Preset:
                    tween = CreatePresetTween(_entry.Preset);
                    break;
                case PresetBrowserEntryKind.CollectionRecipe:
                case PresetBrowserEntryKind.StaggerVariant:
                    tween = CreateCollectionTween(_entry.CollectionKind);
                    break;
                case PresetBrowserEntryKind.BuilderOperation:
                    tween = CreateOperationTween(_entry.Operation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (tween == null) throw new InvalidOperationException($"'{_entry.Name}' did not create a tween.");
            tween.Pause();
            tween.SetAutoKill(false);
            tween.Goto(0f, false);
            _activeTween = tween;
            _elapsedTime = 0f;
            _playbackDuration = ResolvePlaybackDuration(tween);
            _isPlaying = true;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
            UpdateFixtureVisuals();
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
            UpdateFixtureVisuals();
            if (_elapsedTime >= _playbackDuration) _isPlaying = false;
            return true;
        }

        public void Draw(Rect rect)
        {
            if (Event.current.type != EventType.Repaint || rect.width < 2f || rect.height < 2f) return;
            if (_preview == null) RebuildStage();
            if (_preview == null) return;

            EditorGUI.DrawRect(rect, PreviewBackground);
            UpdateFixtureVisuals();
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
            ConfigureCameraAndLights(_entry.PreviewKind);
            CreateMaterials();
            CreateWhiteSprite();

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
                case PresetBrowserPreviewKind.UiTarget:
                    BuildUiTargetStage();
                    break;
                case PresetBrowserPreviewKind.Destination:
                    BuildDestinationStage();
                    break;
                case PresetBrowserPreviewKind.WorldToUi:
                    BuildWorldToUiStage();
                    break;
                case PresetBrowserPreviewKind.UiSequence:
                    BuildUiSequenceStage();
                    break;
                case PresetBrowserPreviewKind.Text:
                    BuildTextStage();
                    break;
                case PresetBrowserPreviewKind.ProgressImage:
                    BuildProgressImageStage();
                    break;
                case PresetBrowserPreviewKind.ProgressSlider:
                    BuildProgressSliderStage();
                    break;
                case PresetBrowserPreviewKind.Camera:
                    BuildCameraStage();
                    break;
                case PresetBrowserPreviewKind.Audio:
                    BuildAudioStage();
                    break;
                case PresetBrowserPreviewKind.Light:
                    BuildLightStage();
                    break;
                case PresetBrowserPreviewKind.Particles:
                    BuildParticleStage();
                    break;
                case PresetBrowserPreviewKind.Material:
                    BuildMaterialStage();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _preview.AddSingleGO(_stageRoot);
        }

        private void BuildSingleStage()
        {
            _singleTarget = CreateCube("Preview Cube", Vector3.zero, new Vector3(1.4f, 1.2f, 1f));
            CreateGround();
        }

        private void BuildListStage(int count)
        {
            const float spacing = 1.55f;
            float startX = -(count - 1) * spacing * 0.5f;
            for (int i = 0; i < count; i++) _collectionTargets.Add(CreateCube($"List Cube {i + 1}", new Vector3(startX + i * spacing, 0f, 0f), Vector3.one * 0.72f));
        }

        private void BuildGridStage()
        {
            const float spacing = 1.55f;
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    int index = row * 3 + column;
                    _collectionTargets.Add(CreateCube($"Grid Cube {index + 1}", new Vector3((column - 1) * spacing, (1 - row) * spacing, 0f), Vector3.one * 0.58f));
                }
            }
        }

        private void BuildLoadingDotsStage()
        {
            const float spacing = 1.8f;
            for (int i = 0; i < 3; i++) _collectionTargets.Add(CreateCube($"Loading Cube {i + 1}", new Vector3((i - 1) * spacing, 0f, 0f), Vector3.one * 0.68f));
        }

        private void BuildUiTargetStage()
        {
            _singleTarget = CreateCube("UI Target", Vector3.zero, new Vector3(3.8f, 1.5f, 0.55f));
            TMP_Text label = CreateWorldText("UI ANIMATION", new Vector3(0f, 0f, -0.5f), 4.2f, 7f);
            label.color = Color.white;
        }

        private void BuildDestinationStage()
        {
            _singleTarget = CreateCube("Moving Target", new Vector3(-3f, -0.25f, 0f), Vector3.one * 0.82f);
            GameObject destination = CreateCube("Destination", new Vector3(3f, -0.25f, 0f), Vector3.one * 0.72f, _secondaryMaterial);
            _focusTarget = destination.transform;
            CreateGround();
        }

        private void BuildWorldToUiStage()
        {
            RectTransform canvas = CreateCanvas();
            Image pickup = CreateImage("Projected Pickup", canvas, new Vector2(64f, 64f), new Vector2(-210f, -90f), AccentColor);
            _singleTarget = pickup.gameObject;

            Image destination = CreateImage("UI Destination", canvas, new Vector2(86f, 86f), new Vector2(220f, 90f), SecondaryColor);
            _uiDestination = destination.rectTransform;
            BindProxy(pickup.gameObject, CreateCube("Projected Pickup Visual", pickup.rectTransform.position, Vector3.one * 0.55f), Vector3.one * 0.55f, -0.25f, 20);
            GameObject destinationVisual = CreateCube("UI Destination Visual", destination.rectTransform.position, Vector3.one * 0.72f, _secondaryMaterial);
            destinationVisual.GetComponent<Renderer>().sortingOrder = 0;
            TMP_Text label = CreateWorldText("3D  →  2D", new Vector3(0f, 1.9f, 0f), 3.2f, 7f);
            label.color = new Color(0.72f, 0.84f, 1f, 1f);
        }

        private void BuildUiSequenceStage()
        {
            RectTransform canvas = CreateCanvas();
            if (UsesBackdrop(_entry.Operation))
            {
                Image backdrop = CreateImage("Backdrop", canvas, new Vector2(620f, 340f), Vector2.zero, new Color(0.04f, 0.06f, 0.12f, 0.82f));
                backdrop.gameObject.AddComponent<CanvasGroup>();
                _backdrop = backdrop.gameObject;
                BindProxy(_backdrop, CreateCube("Backdrop Visual", backdrop.rectTransform.position, new Vector3(7.2f, 4f, 0.18f)), new Vector3(7.2f, 4f, 0.18f), 0.7f, -20);
            }

            Image panel = CreateImage("UI Sequence Panel", canvas, new Vector2(330f, 190f), Vector2.zero, CubeColor);
            panel.gameObject.AddComponent<CanvasGroup>();
            _singleTarget = panel.gameObject;
            BindProxy(_singleTarget, CreateCube("Panel Visual", panel.rectTransform.position, new Vector3(3.8f, 2.4f, 0.5f)), new Vector3(3.8f, 2.4f, 0.5f), 0f, 0);

            if (UsesControls(_entry.Operation))
            {
                for (int i = 0; i < 3; i++)
                {
                    Image control = CreateImage($"Control {i + 1}", panel.rectTransform, new Vector2(240f, 28f), new Vector2(0f, 12f - i * 42f), SecondaryColor);
                    control.gameObject.AddComponent<CanvasGroup>();
                    _uiChildren.Add(control.gameObject);
                    BindProxy(control.gameObject, CreateCube($"Control {i + 1} Visual", control.rectTransform.position, new Vector3(2.7f, 0.28f, 0.2f), _secondaryMaterial), new Vector3(2.7f, 0.28f, 0.2f), -0.4f, 10 + i);
                }
            }

            if (UsesIncomingTarget(_entry.Operation))
            {
                Image incoming = CreateImage("Incoming Panel", canvas, new Vector2(330f, 190f), Vector2.zero, SecondaryColor);
                incoming.gameObject.AddComponent<CanvasGroup>();
                _incomingTarget = incoming.gameObject;
                BindProxy(_incomingTarget, CreateCube("Incoming Visual", incoming.rectTransform.position, new Vector3(3.8f, 2.4f, 0.5f), _secondaryMaterial), new Vector3(3.8f, 2.4f, 0.5f), -0.18f, 5);
            }
        }

        private static bool UsesBackdrop(PresetBrowserOperation operation)
        {
            return operation == PresetBrowserOperation.ModalOpen
                || operation == PresetBrowserOperation.ModalClose
                || operation == PresetBrowserOperation.DrawerShow
                || operation == PresetBrowserOperation.DrawerHide
                || operation == PresetBrowserOperation.BottomSheetShow
                || operation == PresetBrowserOperation.BottomSheetHide;
        }

        private static bool UsesControls(PresetBrowserOperation operation)
        {
            return operation == PresetBrowserOperation.ModalOpen
                || operation == PresetBrowserOperation.ModalClose
                || operation == PresetBrowserOperation.DropdownOpen
                || operation == PresetBrowserOperation.DropdownClose;
        }

        private static bool UsesIncomingTarget(PresetBrowserOperation operation)
        {
            return operation == PresetBrowserOperation.TabSwitchTo
                || operation == PresetBrowserOperation.PagePushTo
                || operation == PresetBrowserOperation.PageCrossFadeTo;
        }

        private void BuildTextStage()
        {
            _textTarget = CreateWorldText("TWEEN HELPER", Vector3.zero, 5.6f, 9f);
            _textTarget.color = new Color(0.75f, 0.9f, 1f, 1f);
            _singleTarget = _textTarget.gameObject;
        }

        private void BuildProgressImageStage()
        {
            GameObject targetRoot = new GameObject("Image Fill Target Root") { hideFlags = HideFlags.HideAndDontSave };
            targetRoot.transform.SetParent(_stageRoot.transform, false);
            targetRoot.transform.localScale = Vector3.one * 0.012f;
            var targetRect = new GameObject("Image Fill Target", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            targetRect.hideFlags = HideFlags.HideAndDontSave;
            targetRect.transform.SetParent(targetRoot.transform, false);
            _fillImage = targetRect.GetComponent<Image>();
            _fillImage.sprite = _whiteSprite;
            _fillImage.color = AccentColor;
            _fillImage.type = Image.Type.Filled;
            _fillImage.fillMethod = Image.FillMethod.Horizontal;
            _fillImage.fillOrigin = 0;
            _fillImage.fillAmount = IsDrainOrAlert(_entry.Operation) ? 0.22f : 0.15f;
            BuildMeterStage();
            _valueText = CreateWorldText($"{_fillImage.fillAmount:P0}", new Vector3(0f, -1.2f, -0.6f), 3.4f, 4f);
            _singleTarget = _fillImage.gameObject;
        }

        private void BuildProgressSliderStage()
        {
            GameObject targetRoot = new GameObject("Slider Target Root") { hideFlags = HideFlags.HideAndDontSave };
            targetRoot.transform.SetParent(_stageRoot.transform, false);
            targetRoot.transform.localScale = Vector3.one * 0.012f;
            GameObject sliderObject = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.hideFlags = HideFlags.HideAndDontSave;
            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.SetParent(targetRoot.transform, false);
            sliderRect.sizeDelta = new Vector2(470f, 84f);

            Image background = CreateImage("Slider Background", sliderRect, new Vector2(470f, 84f), Vector2.zero, new Color(0.07f, 0.11f, 0.19f, 1f));
            RectTransform fillArea = CreateRect("Fill Area", sliderRect, new Vector2(450f, 64f), Vector2.zero);
            Image fill = CreateImage("Slider Fill", fillArea, new Vector2(450f, 64f), Vector2.zero, AccentColor);
            _sliderFillImage = fill;
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;

            _slider = sliderObject.GetComponent<Slider>();
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.wholeNumbers = false;
            _slider.direction = Slider.Direction.LeftToRight;
            _slider.fillRect = fill.rectTransform;
            _slider.targetGraphic = background;
            _slider.value = IsDrainOrAlert(_entry.Operation) ? 0.22f : 0.15f;
            BuildMeterStage();
            _valueText = CreateWorldText($"{_slider.normalizedValue:P0}", new Vector3(0f, -1.2f, -0.6f), 3.4f, 4f);
            _singleTarget = sliderObject;
        }

        private void BuildCameraStage()
        {
            _singleTarget = CreateCube("Camera Focus Object", Vector3.zero, new Vector3(1.3f, 1.3f, 1.3f));
            GameObject focus = CreateCube("Focus Marker", new Vector3(2.8f, 0.8f, 1.2f), Vector3.one * 0.45f, _secondaryMaterial);
            _focusTarget = focus.transform;
            CreateGround();
        }

        private void BuildAudioStage()
        {
            _singleTarget = new GameObject("Audio Property Target") { hideFlags = HideFlags.HideAndDontSave };
            _singleTarget.transform.SetParent(_stageRoot.transform, false);
            _audioSource = _singleTarget.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.volume = 0.1f;
            _audioSource.pitch = 0.55f;
            BuildMeterStage();
        }

        private void BuildLightStage()
        {
            _singleTarget = new GameObject("Light Property Target") { hideFlags = HideFlags.HideAndDontSave };
            _singleTarget.transform.SetParent(_stageRoot.transform, false);
            _singleTarget.transform.localPosition = new Vector3(0f, 1.6f, -1.4f);
            _light = _singleTarget.AddComponent<Light>();
            _light.type = LightType.Point;
            _light.range = 8f;
            _light.intensity = 0.7f;
            _light.color = new Color(1f, 0.55f, 0.2f);
            CreateCube("Lit Object", Vector3.zero, Vector3.one * 1.2f);
            BuildMeterStage();
            CreateGround();
        }

        private void BuildParticleStage()
        {
            _singleTarget = new GameObject("Particle Property Target") { hideFlags = HideFlags.HideAndDontSave };
            _singleTarget.transform.SetParent(_stageRoot.transform, false);
            _particles = _singleTarget.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = _particles.main;
            main.playOnAwake = false;
            main.startLifetime = 1f;
            main.startSpeed = 1.2f;
            ParticleSystem.EmissionModule emission = _particles.emission;
            emission.rateOverTimeMultiplier = 5f;
            BuildMeterStage();
        }

        private void BuildMaterialStage()
        {
            _singleTarget = CreateCube("Material Property Target", Vector3.zero, new Vector3(1.6f, 1.35f, 1.2f));
            _propertyRenderer = _singleTarget.GetComponent<Renderer>();
            CreateGround();
        }

        private void BuildMeterStage()
        {
            _meterTrack = CreateCube("Meter Track", new Vector3(0f, -0.2f, 0f), new Vector3(4.2f, 0.58f, 0.5f), _groundMaterial);
            _meterFill = CreateCube("Meter Fill", new Vector3(-1.9f, -0.2f, -0.3f), new Vector3(0.2f, 0.4f, 0.4f), _secondaryMaterial);
            _meterTrack.GetComponent<Renderer>().sortingOrder = 0;
            _meterFill.GetComponent<Renderer>().sortingOrder = 10;
        }

        private void BindProxy(GameObject source, GameObject proxy, Vector3 baseScale, float depthOffset, int sortingOrder)
        {
            _proxyBindings.Add(new PreviewProxyBinding(source.transform, proxy.transform, proxy.GetComponent<Renderer>(), source.GetComponent<Graphic>(), baseScale, depthOffset, sortingOrder));
        }

        private GameObject CreateCube(string name, Vector3 localPosition, Vector3 localScale, Material material = null)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.hideFlags = HideFlags.HideAndDontSave;
            cube.transform.SetParent(_stageRoot.transform, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = localScale;
            DestroyCollider(cube);
            cube.GetComponent<Renderer>().sharedMaterial = material ?? _cubeMaterial;
            return cube;
        }

        private void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Preview Ground";
            ground.hideFlags = HideFlags.HideAndDontSave;
            ground.transform.SetParent(_stageRoot.transform, false);
            ground.transform.localPosition = new Vector3(0f, -1.15f, 0f);
            ground.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            DestroyCollider(ground);
            ground.GetComponent<Renderer>().sharedMaterial = _groundMaterial;
        }

        private RectTransform CreateCanvas()
        {
            GameObject canvasObject = new GameObject("World Space Canvas", typeof(RectTransform), typeof(Canvas));
            canvasObject.hideFlags = HideFlags.HideAndDontSave;
            RectTransform rect = canvasObject.GetComponent<RectTransform>();
            rect.SetParent(_stageRoot.transform, false);
            rect.sizeDelta = new Vector2(640f, 360f);
            rect.localScale = Vector3.one * 0.012f;
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = _preview.camera;
            canvas.sortingOrder = 10;
            return rect;
        }

        private Image CreateImage(string name, RectTransform parent, Vector2 size, Vector2 position, Color color)
        {
            GameObject target = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            target.hideFlags = HideFlags.HideAndDontSave;
            RectTransform rect = target.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            Image image = target.GetComponent<Image>();
            image.sprite = _whiteSprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private TMP_Text CreateText(string text, RectTransform parent, Vector2 size, Vector2 position, float fontSize)
        {
            GameObject target = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            target.hideFlags = HideFlags.HideAndDontSave;
            RectTransform rect = target.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            TextMeshProUGUI label = target.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            label.color = Color.white;
            return label;
        }

        private TMP_Text CreateWorldText(string text, Vector3 position, float fontSize, float width)
        {
            GameObject target = new GameObject("Text", typeof(TextMeshPro));
            target.hideFlags = HideFlags.HideAndDontSave;
            target.transform.SetParent(_stageRoot.transform, false);
            target.transform.localPosition = position;
            TextMeshPro label = target.GetComponent<TextMeshPro>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.color = Color.white;
            label.rectTransform.sizeDelta = new Vector2(width, 2f);
            label.ForceMeshUpdate();
            return label;
        }

        private static RectTransform CreateRect(string name, RectTransform parent, Vector2 size, Vector2 position)
        {
            GameObject target = new GameObject(name, typeof(RectTransform));
            target.hideFlags = HideFlags.HideAndDontSave;
            RectTransform rect = target.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            return rect;
        }

        private Tween CreatePresetTween(ITweenPreset preset)
        {
            if (preset == null || _singleTarget == null) return null;
            if (!preset.CanApplyTo(_singleTarget)) throw new InvalidOperationException($"'{preset.PresetName}' is not compatible with the internal preview cube.");
            return preset.CreateTween(_singleTarget);
        }

        private Tween CreateCollectionTween(PresetBrowserCollectionKind kind)
        {
            TweenOptions options = ManualOptions();
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
                    return _collectionTargets.GridDiagonalWave(_stageRoot, 3, (GridDiagonalDirection)Mathf.Clamp(_collectionOptionIndex, 0, 3), options: options);
                case PresetBrowserCollectionKind.GridSpiral:
                    return _collectionTargets.GridSpiral(_stageRoot, 3, (GridSpiralDirection)Mathf.Clamp(_collectionOptionIndex, 0, 3), options: options);
                case PresetBrowserCollectionKind.GridCheckerboard:
                    return _collectionTargets.GridCheckerboard(_stageRoot, 3, _collectionOptionIndex == 1, options: options);
                case PresetBrowserCollectionKind.CollectionBurstIn:
                    return _collectionTargets.CollectionBurstIn(_stageRoot, Vector3.zero, options: options);
                case PresetBrowserCollectionKind.CollectionBurstOut:
                    return _collectionTargets.CollectionBurstOut(_stageRoot, Vector3.zero, 2f, options: options);
                case PresetBrowserCollectionKind.CollectionGatherTo:
                    return _collectionTargets.CollectionGatherTo(_stageRoot, Vector3.zero, options: options);
                case PresetBrowserCollectionKind.GridConcentricIn:
                    return _collectionTargets.GridConcentricIn(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.GridConcentricOut:
                    return _collectionTargets.GridConcentricOut(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.GridQuadrantSweep:
                    return _collectionTargets.GridQuadrantSweep(_stageRoot, 3, options: options);
                case PresetBrowserCollectionKind.ListAccordion:
                    return _collectionTargets.ListAccordion(_stageRoot, options: options);
                case PresetBrowserCollectionKind.CollectionOrbitIn:
                    return _collectionTargets.CollectionOrbitIn(_stageRoot, Vector3.zero, 3f, options: options);
                case PresetBrowserCollectionKind.CollectionOrbitOut:
                    return _collectionTargets.CollectionOrbitOut(_stageRoot, Vector3.zero, 3f, options: options);
                case PresetBrowserCollectionKind.LoadingRing:
                    return _collectionTargets.LoadingRing(_stageRoot, options: options);
                case PresetBrowserCollectionKind.LoadingRibbon:
                    return _collectionTargets.LoadingRibbon(_stageRoot, options: options);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown collection preview.");
            }
        }

        private Tween CreateOperationTween(PresetBrowserOperation operation)
        {
            TweenOptions options = ManualOptions();
            Vector3 destination = new Vector3(3f, -0.25f, 0f);
            Vector3[] waypoints = { new Vector3(-1.5f, 1.6f, 0f), new Vector3(0.6f, -0.8f, 0f), new Vector3(1.8f, 1.2f, 0f), destination };
            switch (operation)
            {
                case PresetBrowserOperation.UIHover:
                    return _singleTarget.UIHover(options: options);
                case PresetBrowserOperation.UIHoverSoft:
                    return _singleTarget.UIHoverSoft(options: options);
                case PresetBrowserOperation.UIPress:
                    return _singleTarget.UIPress(options: options);
                case PresetBrowserOperation.UIPressHard:
                    return _singleTarget.UIPressHard(options: options);
                case PresetBrowserOperation.UIAppear:
                    return _singleTarget.UIAppear(options: options);
                case PresetBrowserOperation.UIAppearSoft:
                    return _singleTarget.UIAppearSoft(options: options);
                case PresetBrowserOperation.UIDisappear:
                    return _singleTarget.UIDisappear(options: options);
                case PresetBrowserOperation.UIDisappearSoft:
                    return _singleTarget.UIDisappearSoft(options: options);
                case PresetBrowserOperation.UIAttention:
                    return _singleTarget.UIAttention(options: options);
                case PresetBrowserOperation.UIAttentionSoft:
                    return _singleTarget.UIAttentionSoft(options: options);
                case PresetBrowserOperation.UIAttentionHard:
                    return _singleTarget.UIAttentionHard(options: options);
                case PresetBrowserOperation.UIDisabled:
                    return _singleTarget.UIDisabled(options: options);
                case PresetBrowserOperation.UIEnabled:
                    return CreateUIEnabledTween(options);
                case PresetBrowserOperation.ArcTo:
                    return Play(_singleTarget.Tween().ArcTo(destination, 2f, 0.8f), options);
                case PresetBrowserOperation.ArcLocalTo:
                    return Play(_singleTarget.Tween().ArcLocalTo(destination, 2f, 0.8f), options);
                case PresetBrowserOperation.BezierTo:
                    return Play(_singleTarget.Tween().BezierTo(destination, new Vector3(-1.4f, 2f, 0f), new Vector3(1.5f, 2f, 0f), 0.85f), options);
                case PresetBrowserOperation.BezierLocalTo:
                    return Play(_singleTarget.Tween().BezierLocalTo(destination, new Vector3(-1.4f, 2f, 0f), new Vector3(1.5f, 2f, 0f), 0.85f), options);
                case PresetBrowserOperation.HopTo:
                    return Play(_singleTarget.Tween().HopTo(destination, 2f, 0.8f), options);
                case PresetBrowserOperation.HopLocalTo:
                    return Play(_singleTarget.Tween().HopLocalTo(destination, 2f, 0.8f), options);
                case PresetBrowserOperation.SpringTo:
                    return Play(_singleTarget.Tween().SpringTo(destination, 0.8f), options);
                case PresetBrowserOperation.SpringLocalTo:
                    return Play(_singleTarget.Tween().SpringLocalTo(destination, 0.8f), options);
                case PresetBrowserOperation.MagneticSnapTo:
                    return Play(_singleTarget.Tween().MagneticSnapTo(destination, 0.8f), options);
                case PresetBrowserOperation.MagneticSnapLocalTo:
                    return Play(_singleTarget.Tween().MagneticSnapLocalTo(destination, 0.8f), options);
                case PresetBrowserOperation.PathThrough:
                    return Play(_singleTarget.Tween().PathThrough(waypoints, duration: 1f), options);
                case PresetBrowserOperation.PathLocalThrough:
                    return Play(_singleTarget.Tween().PathLocalThrough(waypoints, duration: 1f), options);
                case PresetBrowserOperation.SpiralTo:
                    return Play(_singleTarget.Tween().SpiralTo(destination, 1.3f, duration: 1f), options);
                case PresetBrowserOperation.SpiralLocalTo:
                    return Play(_singleTarget.Tween().SpiralLocalTo(destination, 1.3f, duration: 1f), options);
                case PresetBrowserOperation.MultiHopTo:
                    return Play(_singleTarget.Tween().MultiHopTo(destination, 1.8f, duration: 1f), options);
                case PresetBrowserOperation.MultiHopLocalTo:
                    return Play(_singleTarget.Tween().MultiHopLocalTo(destination, 1.8f, duration: 1f), options);
                case PresetBrowserOperation.ArcToUI:
                    return Play(_singleTarget.Tween().ArcToUI(WorldSource, _uiDestination, 145f, 0.8f, _preview.camera), options);
                case PresetBrowserOperation.HopToUI:
                    return Play(_singleTarget.Tween().HopToUI(WorldSource, _uiDestination, 145f, 0.8f, _preview.camera), options);
                case PresetBrowserOperation.BezierToUI:
                    return Play(_singleTarget.Tween().BezierToUI(WorldSource, new Vector3(-1.5f, 2f, 0f), new Vector3(1.5f, 2f, 0f), _uiDestination, 0.85f, _preview.camera), options);
                case PresetBrowserOperation.PathThroughUI:
                    return Play(_singleTarget.Tween().PathThroughUI(WorldSource, new[] { new Vector3(-1f, 1.8f, 0f), new Vector3(1.1f, -0.5f, 0f) }, _uiDestination, duration: 1f, worldCamera: _preview.camera), options);
                case PresetBrowserOperation.ErrorReject:
                    return Play(_singleTarget.Tween().ErrorReject(), options);
                case PresetBrowserOperation.DamageHit:
                    return Play(_singleTarget.Tween().DamageHit(), options);
                case PresetBrowserOperation.SuccessConfirm:
                    return Play(_singleTarget.Tween().SuccessConfirm(), options);
                case PresetBrowserOperation.RewardReveal:
                    return Play(_singleTarget.Tween().RewardReveal(), options);
                case PresetBrowserOperation.HealReceive:
                    return Play(_singleTarget.Tween().HealReceive(), options);
                case PresetBrowserOperation.ShieldBlock:
                    return Play(_singleTarget.Tween().ShieldBlock(Vector3.right), options);
                case PresetBrowserOperation.CriticalHit:
                    return Play(_singleTarget.Tween().CriticalHit(Vector3.right), options);
                case PresetBrowserOperation.CooldownReady:
                    return Play(_singleTarget.Tween().CooldownReady(), options);
                case PresetBrowserOperation.LevelUp:
                    return Play(_singleTarget.Tween().LevelUp(), options);
                case PresetBrowserOperation.LowHealthWarning:
                    return Play(_singleTarget.Tween().LowHealthWarning(), options);
                case PresetBrowserOperation.PickupCollectTo:
                    return Play(_singleTarget.Tween().PickupCollectTo(destination), options);
                case PresetBrowserOperation.PickupCollectLocalTo:
                    return Play(_singleTarget.Tween().PickupCollectLocalTo(destination), options);
                case PresetBrowserOperation.PickupCollectToUI:
                    return Play(_singleTarget.Tween().PickupCollectToUI(WorldSource, _uiDestination, worldCamera: _preview.camera), options);
                case PresetBrowserOperation.AbilityCharging:
                    return Play(_singleTarget.Tween().AbilityCharging(), options);
                case PresetBrowserOperation.AbilityReady:
                    return Play(_singleTarget.Tween().AbilityReady(), options);
                case PresetBrowserOperation.DodgeRoll:
                    return Play(_singleTarget.Tween().DodgeRoll(), options);
                case PresetBrowserOperation.StunStart:
                    return Play(_singleTarget.Tween().StunStart(), options);
                case PresetBrowserOperation.StunEnd:
                    return Play(_singleTarget.Tween().StunEnd(), options);
                case PresetBrowserOperation.BuffApplied:
                    return Play(_singleTarget.Tween().BuffApplied(), options);
                case PresetBrowserOperation.DebuffApplied:
                    return Play(_singleTarget.Tween().DebuffApplied(), options);
                case PresetBrowserOperation.ResourceDepleted:
                    return Play(_singleTarget.Tween().ResourceDepleted(), options);
                case PresetBrowserOperation.ResourceRecovered:
                    return Play(_singleTarget.Tween().ResourceRecovered(), options);
                case PresetBrowserOperation.ObjectiveUnlocked:
                    return Play(_singleTarget.Tween().ObjectiveUnlocked(), options);
                case PresetBrowserOperation.CriticalHitSequence:
                    return Play(_singleTarget.Tween().CriticalHitSequence(Vector3.right), options);
                case PresetBrowserOperation.RewardRevealSequence:
                    return Play(_singleTarget.Tween().RewardRevealSequence(), options);
                case PresetBrowserOperation.WarningLoopSequence:
                    return Play(_singleTarget.Tween().WarningLoopSequence(), options);
                case PresetBrowserOperation.CutsceneUIEntranceSequence:
                    return Play(_singleTarget.Tween().CutsceneUIEntranceSequence(), options);
                case PresetBrowserOperation.ToastShow:
                    return _singleTarget.ToastShow(options: options);
                case PresetBrowserOperation.ToastHide:
                    return _singleTarget.ToastHide(options: options);
                case PresetBrowserOperation.ModalOpen:
                    return _singleTarget.ModalOpen(_backdrop, _uiChildren, options: options);
                case PresetBrowserOperation.ModalClose:
                    return _singleTarget.ModalClose(_backdrop, _uiChildren, options: options);
                case PresetBrowserOperation.TooltipShow:
                    return _singleTarget.TooltipShow(options: options);
                case PresetBrowserOperation.TooltipHide:
                    return _singleTarget.TooltipHide(options: options);
                case PresetBrowserOperation.DropdownOpen:
                    return _singleTarget.DropdownOpen(_uiChildren, options: options);
                case PresetBrowserOperation.DropdownClose:
                    return _singleTarget.DropdownClose(_uiChildren, options: options);
                case PresetBrowserOperation.TabSwitchTo:
                    return _singleTarget.TabSwitchTo(_incomingTarget, options: options);
                case PresetBrowserOperation.DrawerShow:
                    return _singleTarget.DrawerShow(backdrop: _backdrop, options: options);
                case PresetBrowserOperation.DrawerHide:
                    return _singleTarget.DrawerHide(backdrop: _backdrop, options: options);
                case PresetBrowserOperation.BottomSheetShow:
                    return _singleTarget.BottomSheetShow(_backdrop, options: options);
                case PresetBrowserOperation.BottomSheetHide:
                    return _singleTarget.BottomSheetHide(_backdrop, options: options);
                case PresetBrowserOperation.PagePushTo:
                    return _singleTarget.PagePushTo(_incomingTarget, options: options);
                case PresetBrowserOperation.PageCrossFadeTo:
                    return _singleTarget.PageCrossFadeTo(_incomingTarget, options: options);
                case PresetBrowserOperation.TypewriterReveal:
                    return _textTarget.TypewriterReveal(options: options);
                case PresetBrowserOperation.TypewriterHide:
                    return _textTarget.TypewriterHide(options: options);
                case PresetBrowserOperation.NumberCountUp:
                    return _textTarget.NumberCountTo(0d, 1250d, "N0", options: options);
                case PresetBrowserOperation.NumberCountDown:
                    return _textTarget.NumberCountTo(1250d, 0d, "N0", options: options);
                case PresetBrowserOperation.TextCharacterStaggerIn:
                    return _textTarget.TextCharacterStaggerIn(distance: 0.5f, options: options);
                case PresetBrowserOperation.TextCharacterStaggerOut:
                    return _textTarget.TextCharacterStaggerOut(distance: 0.5f, options: options);
                case PresetBrowserOperation.TextWave:
                    return _textTarget.TextWave(amplitude: 0.35f, options: options);
                case PresetBrowserOperation.TextCharacterBounce:
                    return _textTarget.TextCharacterBounce(amplitude: 0.4f, options: options);
                case PresetBrowserOperation.TextColorSweep:
                    return _textTarget.TextColorSweep(AccentColor, options: options);
                case PresetBrowserOperation.TextGlitch:
                    return _textTarget.TextGlitch(distance: 0.2f, options: options);
                case PresetBrowserOperation.TextEmphasis:
                    return _textTarget.TextEmphasis(amplitude: 0.3f, startCharacter: 6, characterCount: 6, options: options);
                case PresetBrowserOperation.TextScrambleReveal:
                    return _textTarget.TextScrambleReveal(options: options);
                case PresetBrowserOperation.ScoreIncrease:
                    return _textTarget.ScoreIncrease(900d, 1250d, "N0", options: options);
                case PresetBrowserOperation.ImageFillTo:
                    return _fillImage.FillTo(0.85f, options: options);
                case PresetBrowserOperation.ImageFillFromTo:
                    return _fillImage.FillFromTo(0.15f, 0.85f, options: options);
                case PresetBrowserOperation.ImageValueFillTo:
                    return _fillImage.ValueFillTo(0.85f, _valueText, options: options);
                case PresetBrowserOperation.ImageFillDrain:
                    return _fillImage.FillDrain(0.08f, options: options);
                case PresetBrowserOperation.ImageFillCharge:
                    return _fillImage.FillCharge(0.9f, options: options);
                case PresetBrowserOperation.ImageFillAlertPulse:
                    return _fillImage.FillAlertPulse(0.25f, options: options);
                case PresetBrowserOperation.ImageFillAndText:
                    return _fillImage.FillAndText(0.15f, 0.85f, _valueText, options: options);
                case PresetBrowserOperation.SliderFillTo:
                    return _slider.FillTo(0.85f, options: options);
                case PresetBrowserOperation.SliderFillFromTo:
                    return _slider.FillFromTo(0.15f, 0.85f, options: options);
                case PresetBrowserOperation.SliderValueFillTo:
                    return _slider.ValueFillTo(0.85f, _valueText, options: options);
                case PresetBrowserOperation.SliderFillDrain:
                    return _slider.FillDrain(0.08f, options: options);
                case PresetBrowserOperation.SliderFillCharge:
                    return _slider.FillCharge(0.9f, options: options);
                case PresetBrowserOperation.SliderFillAlertPulse:
                    return _slider.FillAlertPulse(0.25f, options: options);
                case PresetBrowserOperation.SliderFillAndText:
                    return _slider.FillAndText(0.15f, 0.85f, _valueText, options: options);
                case PresetBrowserOperation.CameraImpact:
                    return Play(_preview.camera.gameObject.Tween().CameraImpact(), options);
                case PresetBrowserOperation.CameraRecoil:
                    return Play(_preview.camera.gameObject.Tween().CameraRecoil(), options);
                case PresetBrowserOperation.CameraLandingImpact:
                    return Play(_preview.camera.gameObject.Tween().CameraLandingImpact(), options);
                case PresetBrowserOperation.CameraFovKick:
                    return Play(_preview.camera.gameObject.Tween().CameraFovKick(), options);
                case PresetBrowserOperation.CameraFocusZoom:
                    return Play(_preview.camera.gameObject.Tween().CameraFocusZoom(_focusTarget), options);
                case PresetBrowserOperation.CameraBreathing:
                    return Play(_preview.camera.gameObject.Tween().CameraBreathing(), options);
                case PresetBrowserOperation.CameraRackFocus:
                    return Play(_preview.camera.gameObject.Tween().CameraRackFocus(_focusTarget), options);
                case PresetBrowserOperation.CollectLandingCameraKick:
                    return Play(_preview.camera.gameObject.Tween().CollectLandingCameraKick(), options);
                case PresetBrowserOperation.AudioVolumeTo:
                    return Play(_singleTarget.Tween().AudioVolumeTo(1f), options);
                case PresetBrowserOperation.AudioPitchTo:
                    return Play(_singleTarget.Tween().AudioPitchTo(1.7f), options);
                case PresetBrowserOperation.LightIntensityTo:
                    return Play(_singleTarget.Tween().LightIntensityTo(5f), options);
                case PresetBrowserOperation.LightColorTo:
                    return Play(_singleTarget.Tween().LightColorTo(AccentColor), options);
                case PresetBrowserOperation.ParticleEmissionRateTo:
                    return Play(_singleTarget.Tween().ParticleEmissionRateTo(60f), options);
                case PresetBrowserOperation.MaterialFloatTo:
                    return Play(_singleTarget.Tween().MaterialFloatTo(ResolveFloatPropertyName(), 1f), options);
                case PresetBrowserOperation.MaterialColorTo:
                    return Play(_singleTarget.Tween().MaterialColorTo(ResolveColorPropertyName(), SecondaryColor), options);
                case PresetBrowserOperation.TorchFlicker:
                    return Play(_singleTarget.Tween().TorchFlicker(0.45f), options);
                case PresetBrowserOperation.ScannerPulse:
                    return Play(_singleTarget.Tween().ScannerPulse(AccentColor, 3f), options);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, "Unknown animation preview.");
            }
        }

        private Tween CreateUIEnabledTween(TweenOptions options)
        {
            Tween disabled = _singleTarget.UIDisabled(duration: 0.01f, options: options);
            disabled.Complete();
            disabled.Kill(false);
            return _singleTarget.UIEnabled(options: options);
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

        private static Tween Play(TweenBuilder builder, TweenOptions options) => builder.WithOptions(options).Play().Tween;

        private static TweenOptions ManualOptions() => TweenOptions.WithUpdateType(UpdateType.Manual);

        private static Vector3 WorldSource => new Vector3(-2.6f, -1.1f, 0f);

        private string ResolveFloatPropertyName()
        {
            Material material = _propertyRenderer.sharedMaterial;
            if (material.HasProperty("_Metallic")) return "_Metallic";
            if (material.HasProperty("_Smoothness")) return "_Smoothness";
            if (material.HasProperty("_Glossiness")) return "_Glossiness";
            throw new InvalidOperationException("The preview material has no supported float property.");
        }

        private string ResolveColorPropertyName()
        {
            Material material = _propertyRenderer.sharedMaterial;
            if (material.HasProperty("_BaseColor")) return "_BaseColor";
            if (material.HasProperty("_Color")) return "_Color";
            throw new InvalidOperationException("The preview material has no supported color property.");
        }

        private void UpdateFixtureVisuals()
        {
            if (_valueText != null) _valueText.ForceMeshUpdate();
            for (int i = 0; i < _proxyBindings.Count; i++) _proxyBindings[i].Update();

            if (_meterFill == null || _entry == null) return;
            float normalized;
            switch (_entry.Operation)
            {
                case PresetBrowserOperation.ImageFillTo:
                case PresetBrowserOperation.ImageFillFromTo:
                case PresetBrowserOperation.ImageValueFillTo:
                case PresetBrowserOperation.ImageFillDrain:
                case PresetBrowserOperation.ImageFillCharge:
                case PresetBrowserOperation.ImageFillAlertPulse:
                case PresetBrowserOperation.ImageFillAndText:
                    normalized = _fillImage != null ? _fillImage.fillAmount : 0f;
                    break;
                case PresetBrowserOperation.SliderFillTo:
                case PresetBrowserOperation.SliderFillFromTo:
                case PresetBrowserOperation.SliderValueFillTo:
                case PresetBrowserOperation.SliderFillDrain:
                case PresetBrowserOperation.SliderFillCharge:
                case PresetBrowserOperation.SliderFillAlertPulse:
                case PresetBrowserOperation.SliderFillAndText:
                    normalized = _slider != null ? _slider.normalizedValue : 0f;
                    break;
                case PresetBrowserOperation.AudioVolumeTo:
                    normalized = _audioSource != null ? _audioSource.volume : 0f;
                    break;
                case PresetBrowserOperation.AudioPitchTo:
                    normalized = _audioSource != null ? Mathf.InverseLerp(0.5f, 2f, _audioSource.pitch) : 0f;
                    break;
                case PresetBrowserOperation.LightIntensityTo:
                case PresetBrowserOperation.TorchFlicker:
                case PresetBrowserOperation.ScannerPulse:
                    normalized = _light != null ? Mathf.Clamp01(_light.intensity / 5f) : 0f;
                    break;
                case PresetBrowserOperation.LightColorTo:
                    normalized = _activeTween == null ? 0.15f : _activeTween.ElapsedPercentage();
                    break;
                case PresetBrowserOperation.ParticleEmissionRateTo:
                    normalized = _particles != null ? Mathf.Clamp01(_particles.emission.rateOverTimeMultiplier / 60f) : 0f;
                    break;
                default:
                    normalized = 0.15f;
                    break;
            }

            float width = Mathf.Lerp(0.08f, 4f, Mathf.Clamp01(normalized));
            Vector3 targetScale = _singleTarget != null ? _singleTarget.transform.localScale : Vector3.one;
            Vector3 targetOffset = _singleTarget != null ? _singleTarget.transform.localPosition * 0.012f : Vector3.zero;
            _meterFill.transform.localScale = new Vector3(width * targetScale.x, 0.4f * targetScale.y, 0.4f * targetScale.z);
            _meterFill.transform.localPosition = new Vector3(-2f + width * 0.5f, -0.2f, -0.3f) + targetOffset;
            if (_fillImage != null) ApplyRendererColor(_meterFill.GetComponent<Renderer>(), _fillImage.color);
            if (_sliderFillImage != null) ApplyRendererColor(_meterFill.GetComponent<Renderer>(), _sliderFillImage.color);
            if (_slider?.targetGraphic != null && _meterTrack != null) ApplyRendererColor(_meterTrack.GetComponent<Renderer>(), _slider.targetGraphic.color);
        }

        private static void ApplyRendererColor(Renderer renderer, Color color)
        {
            if (renderer == null) return;
            var propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", color);
            propertyBlock.SetColor("_Color", color);
            propertyBlock.SetColor("_EmissionColor", new Color(color.r * color.a * 0.08f, color.g * color.a * 0.08f, color.b * color.a * 0.08f, 1f));
            renderer.SetPropertyBlock(propertyBlock);
        }

        private void ConfigureCameraAndLights(PresetBrowserPreviewKind kind)
        {
            Camera camera = _preview.camera;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = PreviewBackground;
            camera.nearClipPlane = 0.01f;
            camera.farClipPlane = 100f;
            camera.fieldOfView = 38f;

            bool frontFacing = kind == PresetBrowserPreviewKind.UiTarget
                || kind == PresetBrowserPreviewKind.WorldToUi
                || kind == PresetBrowserPreviewKind.UiSequence
                || kind == PresetBrowserPreviewKind.Text
                || kind == PresetBrowserPreviewKind.ProgressImage
                || kind == PresetBrowserPreviewKind.ProgressSlider;
            if (frontFacing)
            {
                camera.transform.position = new Vector3(0f, 0f, -11f);
                camera.transform.rotation = Quaternion.identity;
            }
            else
            {
                camera.transform.position = new Vector3(7.2f, 4.8f, -12f);
                camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0.15f, 0f) - camera.transform.position);
            }

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

            _cubeMaterial = CreateMaterial(cubeShader, "Tween Helper Polished Blue Preview", CubeColor);
            _secondaryMaterial = CreateMaterial(cubeShader, "Tween Helper Accent Preview", SecondaryColor);
            _groundMaterial = CreateMaterial(cubeShader, "Tween Helper Preview Ground", new Color(0.055f, 0.07f, 0.11f, 1f), false);
        }

        private static Material CreateMaterial(Shader shader, string name, Color color, bool transparent = true)
        {
            var material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave, name = name };
            SetMaterialColor(material, color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0.16f);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.72f);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0.72f);
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", CubeEmissionColor);
                material.EnableKeyword("_EMISSION");
            }
            if (transparent) ConfigureTransparentMaterial(material);
            else material.renderQueue = (int)RenderQueue.Geometry;
            return material;
        }

        private void CreateWhiteSprite()
        {
            _whiteTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = "Tween Helper Preview White Texture"
            };
            _whiteTexture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            _whiteTexture.Apply(false, true);
            _whiteSprite = Sprite.Create(_whiteTexture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f), 100f);
            _whiteSprite.hideFlags = HideFlags.HideAndDontSave;
            _whiteSprite.name = "Tween Helper Preview White Sprite";
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

        private static bool IsDrainOrAlert(PresetBrowserOperation operation)
        {
            return operation == PresetBrowserOperation.ImageFillDrain
                || operation == PresetBrowserOperation.ImageFillAlertPulse
                || operation == PresetBrowserOperation.SliderFillDrain
                || operation == PresetBrowserOperation.SliderFillAlertPulse;
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
            _incomingTarget = null;
            _backdrop = null;
            _meterTrack = null;
            _meterFill = null;
            _uiDestination = null;
            _textTarget = null;
            _valueText = null;
            _fillImage = null;
            _sliderFillImage = null;
            _slider = null;
            _audioSource = null;
            _light = null;
            _particles = null;
            _propertyRenderer = null;
            _focusTarget = null;
            _collectionTargets.Clear();
            _uiChildren.Clear();
            _proxyBindings.Clear();

            if (_preview != null)
            {
                _preview.Cleanup();
                _preview = null;
            }

            if (_cubeMaterial != null) UnityEngine.Object.DestroyImmediate(_cubeMaterial);
            if (_groundMaterial != null) UnityEngine.Object.DestroyImmediate(_groundMaterial);
            if (_secondaryMaterial != null) UnityEngine.Object.DestroyImmediate(_secondaryMaterial);
            if (_whiteSprite != null) UnityEngine.Object.DestroyImmediate(_whiteSprite);
            if (_whiteTexture != null) UnityEngine.Object.DestroyImmediate(_whiteTexture);
            _cubeMaterial = null;
            _groundMaterial = null;
            _secondaryMaterial = null;
            _whiteSprite = null;
            _whiteTexture = null;
            _stageRoot = null;
        }

        private sealed class PreviewProxyBinding
        {
            private readonly Transform _source;
            private readonly Transform _proxy;
            private readonly Renderer _renderer;
            private readonly Graphic _graphic;
            private readonly Vector3 _baseScale;
            private readonly float _depthOffset;
            private readonly MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();

            public PreviewProxyBinding(Transform source, Transform proxy, Renderer renderer, Graphic graphic, Vector3 baseScale, float depthOffset, int sortingOrder)
            {
                _source = source;
                _proxy = proxy;
                _renderer = renderer;
                _graphic = graphic;
                _baseScale = baseScale;
                _depthOffset = depthOffset;
                if (_renderer != null) _renderer.sortingOrder = sortingOrder;
            }

            public void Update()
            {
                if (_source == null || _proxy == null) return;
                _proxy.position = _source.position + Vector3.forward * _depthOffset;
                _proxy.localScale = Vector3.Scale(_baseScale, _source.localScale);
                if (_renderer == null || _graphic == null) return;

                Color color = _graphic.color;
                color.a *= ResolveCanvasGroupAlpha(_source);
                _renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor("_BaseColor", color);
                _propertyBlock.SetColor("_Color", color);
                _propertyBlock.SetColor("_EmissionColor", new Color(color.r * color.a * 0.08f, color.g * color.a * 0.08f, color.b * color.a * 0.08f, 1f));
                _renderer.SetPropertyBlock(_propertyBlock);
            }

            private static float ResolveCanvasGroupAlpha(Transform source)
            {
                float alpha = 1f;
                Transform current = source;
                while (current != null)
                {
                    CanvasGroup canvasGroup = current.GetComponent<CanvasGroup>();
                    if (canvasGroup != null) alpha *= canvasGroup.alpha;
                    current = current.parent;
                }

                return alpha;
            }
        }
    }
}
