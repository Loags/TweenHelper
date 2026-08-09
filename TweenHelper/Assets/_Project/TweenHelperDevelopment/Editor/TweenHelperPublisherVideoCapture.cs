using DG.Tweening;
using LB.TweenHelper;
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Media;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class TweenHelperPublisherVideoCapture
{
    private const int Width = 1280;
    private const int Height = 720;
    private const int FrameRate = 30;
    private const float ClipDuration = 2f;
    private const int ShowcaseCount = 12;

    private static Camera _camera;
    private static Canvas _canvas;
    private static TMP_Text _sectionLabel;
    private static TMP_Text _animationLabel;
    private static TMP_Text _descriptionLabel;
    private static TMP_Text _footerLabel;
    private static RectTransform _infoPanelRect;
    private static CanvasGroup _infoGroup;
    private static RectTransform _heroPanelRect;
    private static CanvasGroup _heroGroup;
    private static TMP_Text _heroEyebrow;
    private static TMP_Text _heroTitle;
    private static TMP_Text _heroSubtitle;
    private static RectTransform _heroMark;
    private static Image _uiTarget;
    private static GameObject _worldTarget;
    private static GameObject _ground;
    private static RenderTexture _renderTexture;
    private static Texture2D _readbackTexture;
    private static string _outputDirectory;
    private static int _frameIndex;
    private static string _previousScenePath;

    [InitializeOnLoadMethod]
    private static void RunRequestedCapture()
    {
        string requestPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp", "TweenHelperPublisherVideoCapture.request");
        if (!File.Exists(requestPath)) return;
        File.Delete(requestPath);
        EditorApplication.delayCall += Run;
    }

    [MenuItem("Tools/Tween Helper Dev/Capture Publisher Video")]
    private static void RunFromMenu() => Run();

    public static void Run()
    {
        try
        {
            _outputDirectory = GetOutputDirectory();
            Directory.CreateDirectory(_outputDirectory);
            foreach (string frame in Directory.GetFiles(_outputDirectory, "frame-*.png")) File.Delete(frame);
            DeleteIfPresent(Path.Combine(_outputDirectory, "capture-complete.txt"));
            DeleteIfPresent(Path.Combine(_outputDirectory, "capture-error.txt"));
            _frameIndex = 0;

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
            TweenPresetRegistry.Refresh();
            BuildStage();

            CaptureIntro();
            CaptureAppearDisappear();
            Capture2D("UI Hover", "Responsive scale and color feedback", "target.UIHover();", target => target.UIHover(0.32f, new Color(0.58f, 0.3f, 1f), TweenOptions.WithUpdateType(UpdateType.Manual)), 2, 0.64f, 3);
            Capture2D("UI Press Hard", "Punchy interaction feedback", "target.UIPressHard();", target => target.UIPressHard(0.32f, new Color(0.04f, 0.28f, 0.76f), TweenOptions.WithUpdateType(UpdateType.Manual)), 3, 0.48f, 4);
            Capture2D("UI Attention Hard", "Strong motion that draws the eye", "target.UIAttentionHard();", target => target.UIAttentionHard(0.52f, TweenOptions.WithUpdateType(UpdateType.Manual)), 2, 0.66f, 5);
            Capture2DPreset<PulseScaleHardPreset>("Pulse Scale Hard", "A bold rhythmic emphasis loop", 1.65f, 2, 0.66f, 6);
            Capture3D<PopInOvershootPreset>("Pop In Overshoot", "Energetic scale-in with a satisfying settle", 0, 7);
            Capture3D<BounceCartoonPreset>("Bounce Cartoon", "Playful squash, stretch, and landing motion", 0, 8);
            Capture3D<SlideInRightHardPreset>("Slide In Right Hard", "Fast directional entrance with impact", 0, 9);
            Capture3D<SpinDiagonalXYPreset>("Spin Diagonal", "Multi-axis rotation for dynamic reveals", 1, 10);
            Capture3D<WobblePreset>("Wobble", "Organic rotational follow-through", 1, 11);
            Capture3D<ShakeHardPreset>("Shake Hard", "High-energy impact feedback", 1, 12);
            CaptureOutro();

            File.WriteAllText(Path.Combine(_outputDirectory, "capture-complete.txt"), $"{_frameIndex} frames at {FrameRate} fps, {Width}x{Height}");
            EncodeVideo(GetVideoOutputPath());
            Debug.Log($"Tween Helper publisher capture completed: {_frameIndex} frames in {_outputDirectory}");
            Cleanup();
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            try
            {
                File.WriteAllText(Path.Combine(_outputDirectory ?? Directory.GetCurrentDirectory(), "capture-error.txt"), exception.ToString());
            }
            catch
            {
            }

            Cleanup();
            if (Application.isBatchMode) EditorApplication.Exit(1);
        }
    }

    private static void BuildStage()
    {
        _previousScenePath = SceneManager.GetActiveScene().path;
        if (Application.isPlaying) SceneManager.CreateScene("TweenHelperPublisherCapture");
        else EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var cameraObject = new GameObject("Capture Camera");
        _camera = cameraObject.AddComponent<Camera>();
        _camera.clearFlags = CameraClearFlags.SolidColor;
        _camera.backgroundColor = new Color(0.018f, 0.025f, 0.055f);
        _camera.fieldOfView = 42f;
        _camera.transform.position = new Vector3(0f, 2.15f, -8.5f);
        _camera.transform.LookAt(new Vector3(0f, 0.6f, 0f));

        _renderTexture = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 4
        };
        _renderTexture.Create();
        _camera.targetTexture = _renderTexture;
        _readbackTexture = new Texture2D(Width, Height, TextureFormat.RGB24, false);

        var keyLight = new GameObject("Key Light").AddComponent<Light>();
        keyLight.type = LightType.Directional;
        keyLight.intensity = 1.4f;
        keyLight.color = new Color(0.72f, 0.84f, 1f);
        keyLight.transform.rotation = Quaternion.Euler(42f, -28f, 0f);

        var rimLight = new GameObject("Rim Light").AddComponent<Light>();
        rimLight.type = LightType.Point;
        rimLight.intensity = 8f;
        rimLight.range = 12f;
        rimLight.color = new Color(0.55f, 0.22f, 1f);
        rimLight.transform.position = new Vector3(3.2f, 3.5f, -1.5f);

        BuildCanvas();
        BuildWorldTarget();
    }

    private static void BuildCanvas()
    {
        var canvasObject = new GameObject("Capture Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        _canvas = canvasObject.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = _camera;
        _canvas.planeDistance = 1f;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(Width, Height);

        CreatePanel(canvasObject.transform, "Ambient Left", new Color(0.08f, 0.22f, 0.48f, 0.35f), new Vector2(-120f, 80f), new Vector2(390f, 560f));
        CreatePanel(canvasObject.transform, "Ambient Right", new Color(0.32f, 0.12f, 0.56f, 0.28f), new Vector2(1050f, 140f), new Vector2(300f, 470f));

        var infoPanel = CreatePanel(canvasObject.transform, "Info Panel", new Color(0.045f, 0.07f, 0.145f, 0.98f), new Vector2(64f, 574f), new Vector2(1152f, 104f));
        _infoPanelRect = (RectTransform)infoPanel.transform;
        _infoGroup = infoPanel.AddComponent<CanvasGroup>();
        CreatePanel(infoPanel.transform, "Info Accent", new Color(0.18f, 0.72f, 1f, 1f), Vector2.zero, new Vector2(8f, 104f));
        var sectionChip = CreatePanel(infoPanel.transform, "Section Chip", new Color(0.10f, 0.32f, 0.62f, 1f), new Vector2(28f, 27f), new Vector2(170f, 50f));
        _sectionLabel = CreateText(sectionChip.transform, "Section", 21, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, new Vector2(170f, 50f));
        _animationLabel = CreateText(infoPanel.transform, "Animation", 34, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(224f, 45f), new Vector2(650f, 46f));
        _descriptionLabel = CreateText(infoPanel.transform, "Description", 20, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(224f, 14f), new Vector2(710f, 32f));
        _descriptionLabel.color = new Color(0.72f, 0.8f, 0.94f);
        _footerLabel = CreateText(infoPanel.transform, "Footer", 18, FontStyles.Normal, TextAlignmentOptions.Right, new Vector2(810f, 32f), new Vector2(310f, 38f));
        _footerLabel.color = new Color(0.38f, 0.78f, 1f);

        var card = CreatePanel(canvasObject.transform, "Preview Card", new Color(0.055f, 0.09f, 0.18f, 0.98f), new Vector2(300f, 130f), new Vector2(680f, 390f));
        CreatePanel(card.transform, "Card Edge", new Color(0.18f, 0.72f, 1f, 0.75f), Vector2.zero, new Vector2(680f, 5f));
        var accent = CreatePanel(card.transform, "Accent", new Color(0.18f, 0.72f, 1f, 1f), new Vector2(70f, 105f), new Vector2(540f, 180f));
        _uiTarget = accent.GetComponent<Image>();
        CreateText(accent.transform, "Target Label", 42, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, new Vector2(540f, 180f)).text = "TWEEN HELPER";

        BuildHero(canvasObject.transform);
    }

    private static void BuildWorldTarget()
    {
        _ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _ground.name = "Ground";
        _ground.transform.position = new Vector3(0f, -0.8f, 0.8f);
        _ground.transform.localScale = new Vector3(1.7f, 1f, 1.3f);
        SetMaterial(_ground, new Color(0.035f, 0.055f, 0.11f), 0.25f, 0.75f);

        _worldTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _worldTarget.name = "3D Tween Target";
        _worldTarget.transform.position = new Vector3(0f, 0.45f, 0.4f);
        _worldTarget.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        SetMaterial(_worldTarget, new Color(0.12f, 0.68f, 1f), 0.65f, 0.35f);
        _worldTarget.SetActive(false);
        _ground.SetActive(false);
    }

    private static void BuildHero(Transform parent)
    {
        var heroPanel = CreatePanel(parent, "Hero Panel", new Color(0.045f, 0.07f, 0.145f, 0.98f), new Vector2(150f, 145f), new Vector2(980f, 430f));
        _heroPanelRect = (RectTransform)heroPanel.transform;
        _heroGroup = heroPanel.AddComponent<CanvasGroup>();
        CreatePanel(heroPanel.transform, "Hero Edge", new Color(0.18f, 0.72f, 1f, 1f), Vector2.zero, new Vector2(980f, 8f));
        var mark = CreatePanel(heroPanel.transform, "Hero Mark", new Color(0.18f, 0.72f, 1f, 1f), new Vector2(70f, 128f), new Vector2(176f, 176f));
        _heroMark = (RectTransform)mark.transform;
        CreatePanel(mark.transform, "Mark Core", new Color(0.31f, 0.2f, 0.7f, 1f), new Vector2(38f, 38f), new Vector2(100f, 100f));
        CreateText(mark.transform, "Mark Text", 54, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, new Vector2(176f, 176f)).text = "TH";

        _heroEyebrow = CreateText(heroPanel.transform, "Hero Eyebrow", 22, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(302f, 305f), new Vector2(600f, 34f));
        _heroEyebrow.color = new Color(0.38f, 0.78f, 1f);
        _heroTitle = CreateText(heroPanel.transform, "Hero Title", 54, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(298f, 218f), new Vector2(620f, 80f));
        _heroSubtitle = CreateText(heroPanel.transform, "Hero Subtitle", 25, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(302f, 154f), new Vector2(590f, 64f));
        _heroSubtitle.color = new Color(0.75f, 0.82f, 0.94f);

        CreateStatChip(heroPanel.transform, "300 PRESETS", new Vector2(302f, 82f), 170f);
        CreateStatChip(heroPanel.transform, "2D + 3D", new Vector2(488f, 82f), 150f);
        CreateStatChip(heroPanel.transform, "FLUENT API", new Vector2(654f, 82f), 180f);
    }

    private static void CreateStatChip(Transform parent, string text, Vector2 position, float width)
    {
        var chip = CreatePanel(parent, $"{text} Chip", new Color(0.09f, 0.16f, 0.3f, 1f), position, new Vector2(width, 48f));
        CreateText(chip.transform, text, 18, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, new Vector2(width, 48f)).text = text;
    }

    private static void CaptureIntro()
    {
        Set2DVisible(false);
        Set3DVisible(false);
        SetInfoVisible(false);
        SetHeroVisible(true);
        _heroEyebrow.text = "TWEEN HELPER";
        _heroTitle.text = "Animation made simple";
        _heroSubtitle.text = "Production-ready presets for expressive UI and 3D motion.";
        RenderSeconds(2.3f, frame => ApplyHeroMotion(frame, 2.3f, false));
    }

    private static void CaptureAppearDisappear()
    {
        SetHeroVisible(false);
        Set2DVisible(true);
        Set3DVisible(false);
        SetClipInfo("2D UI", "UI Appear + Disappear", "A polished entrance and exit pair", "target.UIAppear();", 1, ShowcaseCount);
        ResetUiTarget();
        var group = GetOrAddCanvasGroup(_uiTarget.gameObject);
        group.alpha = 0f;
        _uiTarget.transform.localScale = Vector3.one * 0.72f;
        TweenHandle handle = _uiTarget.gameObject.UIAppear(1.05f, TweenOptions.WithUpdateType(UpdateType.Manual));
        int disappearFrame = Mathf.RoundToInt(FrameRate * 1.45f);

        RenderSeconds(3.15f, frame =>
        {
            ApplyInfoMotion(frame, 3.15f);
            if (frame != disappearFrame) return;
            handle?.Kill();
            handle = _uiTarget.gameObject.UIDisappear(1.05f, TweenOptions.WithUpdateType(UpdateType.Manual));
            _animationLabel.text = "UI Disappear";
            _descriptionLabel.text = "A clean exit that completes the interaction";
            _footerLabel.text = "02/12  target.UIDisappear();";
        });

        handle?.Kill();
        DOTween.Kill(_uiTarget.gameObject, false);
        DOTween.Kill(_uiTarget.transform, false);
    }

    private static void Capture2D(string animationName, string description, string api, Func<GameObject, TweenHandle> play, int replayCount, float replayInterval, int showcaseIndex)
    {
        SetHeroVisible(false);
        Set2DVisible(true);
        Set3DVisible(false);
        SetClipInfo("2D UI", animationName, description, api, showcaseIndex, ShowcaseCount);
        ResetUiTarget();
        Vector3 baseScale = _uiTarget.transform.localScale;
        Vector2 basePosition = ((RectTransform)_uiTarget.transform).anchoredPosition;
        Quaternion baseRotation = _uiTarget.transform.localRotation;
        Color baseColor = _uiTarget.color;
        float baseAlpha = GetOrAddCanvasGroup(_uiTarget.gameObject).alpha;
        float maxVisualDelta = 0f;
        TweenHandle handle = play(_uiTarget.gameObject);
        int nextReplay = replayCount > 0 ? Mathf.RoundToInt(FrameRate * replayInterval) : int.MaxValue;
        int replayed = 0;

        RenderSeconds(ClipDuration, localFrame =>
        {
            ApplyInfoMotion(localFrame, ClipDuration);
            maxVisualDelta = Mathf.Max(maxVisualDelta, GetUiVisualDelta(baseScale, basePosition, baseRotation, baseColor, baseAlpha));
            if (localFrame == nextReplay && replayed < replayCount)
            {
                handle?.Kill();
                ResetUiTarget();
                handle = play(_uiTarget.gameObject);
                replayed++;
                nextReplay += Mathf.RoundToInt(FrameRate * replayInterval);
            }
        });

        if (maxVisualDelta < 0.045f) throw new InvalidOperationException($"The {animationName} capture did not produce enough visible UI motion ({maxVisualDelta:0.000}).");
        handle?.Kill();
        DOTween.Kill(_uiTarget.gameObject, false);
        DOTween.Kill(_uiTarget.transform, false);
    }

    private static void Capture2DPreset<TPreset>(string animationName, string description, float strength, int replayCount, float replayInterval, int showcaseIndex) where TPreset : class, ITweenPreset, new()
    {
        var options = TweenOptions.WithStrength(strength).SetUpdateType(UpdateType.Manual);
        Capture2D(animationName, description, "PRESET LIBRARY", target => target.transform.Tween().WithOptions(options).Preset<TPreset>().Play(), replayCount, replayInterval, showcaseIndex);
    }

    private static float GetUiVisualDelta(Vector3 baseScale, Vector2 basePosition, Quaternion baseRotation, Color baseColor, float baseAlpha)
    {
        float scaleDelta = Vector3.Distance(_uiTarget.transform.localScale, baseScale);
        float positionDelta = Vector2.Distance(((RectTransform)_uiTarget.transform).anchoredPosition, basePosition) / 100f;
        float rotationDelta = Quaternion.Angle(_uiTarget.transform.localRotation, baseRotation) / 45f;
        float colorDelta = Vector4.Distance(_uiTarget.color, baseColor);
        float alphaDelta = Mathf.Abs(GetOrAddCanvasGroup(_uiTarget.gameObject).alpha - baseAlpha);
        return Mathf.Max(scaleDelta, positionDelta, rotationDelta, colorDelta, alphaDelta);
    }

    private static void Capture3D<TPreset>(string animationName, string description, int replayCount, int showcaseIndex) where TPreset : class, ITweenPreset, new()
    {
        SetHeroVisible(false);
        Set2DVisible(false);
        Set3DVisible(true);
        SetClipInfo("3D PRESET", animationName, description, "PRESET LIBRARY", showcaseIndex, ShowcaseCount);
        ResetWorldTarget();
        TweenHandle handle = PlayWorldPreset<TPreset>();
        int nextReplay = replayCount > 0 ? Mathf.RoundToInt(FrameRate * 1.05f) : int.MaxValue;
        int replayed = 0;

        RenderSeconds(ClipDuration, localFrame =>
        {
            ApplyInfoMotion(localFrame, ClipDuration);
            if (localFrame == nextReplay && replayed < replayCount)
            {
                handle?.Kill();
                ResetWorldTarget();
                handle = PlayWorldPreset<TPreset>();
                replayed++;
                nextReplay += Mathf.RoundToInt(FrameRate * 1.05f);
            }
        });

        handle?.Kill();
        DOTween.Kill(_worldTarget.transform, false);
    }

    private static TweenHandle PlayWorldPreset<TPreset>() where TPreset : class, ITweenPreset, new()
        => _worldTarget.transform.Tween().WithOptions(TweenOptions.WithUpdateType(UpdateType.Manual)).Preset<TPreset>().Play();

    private static void CaptureOutro()
    {
        Set2DVisible(false);
        Set3DVisible(false);
        SetInfoVisible(false);
        SetHeroVisible(true);
        _heroEyebrow.text = "TWEEN HELPER";
        _heroTitle.text = "Build better game feel";
        _heroSubtitle.text = "Reusable animation presets. Faster iteration. More expressive games.";
        RenderSeconds(2.5f, frame => ApplyHeroMotion(frame, 2.5f, true));
    }

    private static void SetClipInfo(string section, string animationName, string description, string api, int index, int total)
    {
        SetInfoVisible(true);
        _sectionLabel.text = section;
        _animationLabel.text = animationName;
        _descriptionLabel.text = description;
        _footerLabel.text = $"{index:00}/{total:00}  {api}";
    }

    private static void ApplyInfoMotion(int frame, float duration)
    {
        float progress = frame / Mathf.Max(1f, duration * FrameRate - 1f);
        float enter = SmoothStep01(progress / 0.14f);
        float exit = SmoothStep01((1f - progress) / 0.12f);
        float visibility = Mathf.Min(enter, exit);
        _infoGroup.alpha = visibility;
        _infoPanelRect.anchoredPosition = new Vector2(Mathf.Lerp(-90f, 64f, enter), 574f + Mathf.Lerp(10f, 0f, visibility));
    }

    private static void ApplyHeroMotion(int frame, float duration, bool fadeAtEnd)
    {
        float progress = frame / Mathf.Max(1f, duration * FrameRate - 1f);
        float enter = SmoothStep01(progress / 0.28f);
        float exit = fadeAtEnd ? SmoothStep01((1f - progress) / 0.22f) : 1f;
        _heroGroup.alpha = Mathf.Min(enter, exit);
        _heroPanelRect.anchoredPosition = new Vector2(150f, Mathf.Lerp(108f, 145f, enter));
        float scale = Mathf.Lerp(0.86f, 1f, enter);
        _heroPanelRect.localScale = Vector3.one * scale;
        _heroMark.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-18f, 0f, enter) + Mathf.Sin(progress * Mathf.PI * 2f) * 1.5f);
        _heroMark.localScale = Vector3.one * Mathf.Lerp(0.72f, 1f, enter);
    }

    private static float SmoothStep01(float value)
    {
        value = Mathf.Clamp01(value);
        return value * value * (3f - 2f * value);
    }

    private static void DeleteIfPresent(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }

    private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
    {
        var group = target.GetComponent<CanvasGroup>();
        return group != null ? group : target.AddComponent<CanvasGroup>();
    }

    private static void RenderSeconds(float seconds, Action<int> beforeFrame)
    {
        int frameCount = Mathf.RoundToInt(seconds * FrameRate);
        float deltaTime = 1f / FrameRate;
        for (int localFrame = 0; localFrame < frameCount; localFrame++)
        {
            beforeFrame?.Invoke(localFrame);
            DOTween.ManualUpdate(deltaTime, deltaTime);
            RenderFrame();
        }
    }

    private static void RenderFrame()
    {
        _camera.Render();
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = _renderTexture;
        _readbackTexture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0, false);
        _readbackTexture.Apply(false, false);
        File.WriteAllBytes(Path.Combine(_outputDirectory, $"frame-{_frameIndex:D5}.png"), _readbackTexture.EncodeToPNG());
        RenderTexture.active = previous;
        _frameIndex++;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color, Vector2 position, Vector2 size)
    {
        var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        var rect = (RectTransform)panel.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        panel.GetComponent<Image>().color = color;
        return panel;
    }

    private static TMP_Text CreateText(Transform parent, string name, float size, FontStyles style, TextAlignmentOptions alignment, Vector2 position, Vector2 dimensions)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        var rect = (RectTransform)textObject.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        return text;
    }

    private static void SetMaterial(GameObject target, Color color, float metallic, float smoothness)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var material = new Material(shader) { color = color };
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
        target.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void ResetUiTarget()
    {
        _uiTarget.transform.localScale = Vector3.one;
        _uiTarget.transform.localRotation = Quaternion.identity;
        ((RectTransform)_uiTarget.transform).anchoredPosition = new Vector2(70f, 105f);
        _uiTarget.color = new Color(0.18f, 0.72f, 1f, 1f);
        var canvasGroup = _uiTarget.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = _uiTarget.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
    }

    private static void ResetWorldTarget()
    {
        _worldTarget.transform.position = new Vector3(0f, 0.45f, 0.4f);
        _worldTarget.transform.rotation = Quaternion.Euler(18f, 28f, 0f);
        _worldTarget.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
    }

    private static void Set2DVisible(bool visible)
    {
        Transform card = _canvas.transform.Find("Preview Card");
        if (card != null) card.gameObject.SetActive(visible);
    }

    private static void SetInfoVisible(bool visible)
    {
        if (_infoGroup != null) _infoGroup.gameObject.SetActive(visible);
    }

    private static void SetHeroVisible(bool visible)
    {
        if (_heroGroup != null) _heroGroup.gameObject.SetActive(visible);
    }

    private static void Set3DVisible(bool visible)
    {
        _worldTarget.SetActive(visible);
        _ground.SetActive(visible);
    }

    private static string GetOutputDirectory()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        for (int i = 0; i < arguments.Length - 1; i++)
        {
            if (arguments[i] == "-captureOutput") return Path.GetFullPath(arguments[i + 1]);
        }

        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "TempValidation", "PublisherVideoFrames-20260809"));
    }

    private static string GetVideoOutputPath()
        => Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "PublisherMedia", "TweenHelper-2D-Showcase-1280x720.mp4"));

    private static void EncodeVideo(string videoPath)
    {
        string directory = Path.GetDirectoryName(videoPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        var attributes = new VideoTrackAttributes
        {
            frameRate = new MediaRational(FrameRate),
            width = Width,
            height = Height,
            includeAlpha = false
        };

        using var encoder = new MediaEncoder(videoPath, attributes);
        var loadedTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        var frameTexture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        try
        {
            for (int frame = 0; frame < _frameIndex; frame++)
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(_outputDirectory, $"frame-{frame:D5}.png"));
                if (!ImageConversion.LoadImage(loadedTexture, bytes, false)) throw new InvalidOperationException($"Could not load capture frame {frame}.");
                frameTexture.SetPixels32(loadedTexture.GetPixels32());
                frameTexture.Apply(false, false);
                if (!encoder.AddFrame(frameTexture)) throw new InvalidOperationException($"Could not encode capture frame {frame}.");
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(loadedTexture);
            UnityEngine.Object.DestroyImmediate(frameTexture);
        }
    }

    private static void Cleanup()
    {
        DOTween.KillAll(false);
        if (_camera != null) _camera.targetTexture = null;
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            UnityEngine.Object.DestroyImmediate(_renderTexture);
        }
        if (_readbackTexture != null) UnityEngine.Object.DestroyImmediate(_readbackTexture);
        if (!Application.isPlaying && !string.IsNullOrEmpty(_previousScenePath)) EditorSceneManager.OpenScene(_previousScenePath, OpenSceneMode.Single);
    }
}
