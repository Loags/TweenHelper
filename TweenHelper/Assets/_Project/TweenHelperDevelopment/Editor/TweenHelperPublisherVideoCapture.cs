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
    private const float ClipDuration = 2.5f;

    private static Camera _camera;
    private static Canvas _canvas;
    private static TMP_Text _sectionLabel;
    private static TMP_Text _animationLabel;
    private static TMP_Text _footerLabel;
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
            _frameIndex = 0;

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
            TweenPresetRegistry.Refresh();
            BuildStage();

            CaptureIntro();
            Capture2D("UI Appear", target => target.UIAppear(1.1f, TweenOptions.WithUpdateType(UpdateType.Manual)), 0);
            Capture2D("UI Disappear", target => target.UIDisappear(1.1f, TweenOptions.WithUpdateType(UpdateType.Manual)), 0);
            Capture2D("UI Attention Hard", target => target.UIAttentionHard(1.1f, TweenOptions.WithUpdateType(UpdateType.Manual)), 1);
            Capture3D<PopInPreset>("Pop In", 0);
            Capture3D<SpinPreset>("Spin", 1);
            Capture3D<WobblePreset>("Wobble", 1);
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

        CreatePanel(canvasObject.transform, "Top Bar", new Color(0.035f, 0.055f, 0.12f, 0.96f), new Vector2(0f, 600f), new Vector2(1280f, 120f));
        CreatePanel(canvasObject.transform, "Bottom Bar", new Color(0.035f, 0.055f, 0.12f, 0.96f), new Vector2(0f, 0f), new Vector2(1280f, 82f));

        _sectionLabel = CreateText(canvasObject.transform, "Section", 27, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(64f, 628f), new Vector2(420f, 50f));
        _animationLabel = CreateText(canvasObject.transform, "Animation", 50, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(290f, 620f), new Vector2(700f, 64f));
        _footerLabel = CreateText(canvasObject.transform, "Footer", 25, FontStyles.Normal, TextAlignmentOptions.Center, new Vector2(140f, 14f), new Vector2(1000f, 48f));

        var card = CreatePanel(canvasObject.transform, "Preview Card", new Color(0.075f, 0.11f, 0.22f, 0.96f), new Vector2(330f, 190f), new Vector2(620f, 330f));
        var accent = CreatePanel(card.transform, "Accent", new Color(0.18f, 0.72f, 1f, 1f), new Vector2(55f, 82f), new Vector2(510f, 166f));
        _uiTarget = accent.GetComponent<Image>();
        CreateText(accent.transform, "Target Label", 42, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, new Vector2(510f, 166f)).text = "TWEEN HELPER";
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

    private static void CaptureIntro()
    {
        Set2DVisible(false);
        Set3DVisible(false);
        _sectionLabel.text = "TWEEN HELPER";
        _animationLabel.text = "Animation made simple";
        _footerLabel.text = "300 presets | Fluent builder API | 2D and 3D";
        RenderSeconds(1.5f, null);
    }

    private static void Capture2D(string animationName, Func<GameObject, TweenHandle> play, int replayCount)
    {
        Set2DVisible(true);
        Set3DVisible(false);
        _sectionLabel.text = "2D ANIMATION";
        _animationLabel.text = animationName;
        _footerLabel.text = $"target.{animationName.Replace(" ", string.Empty)}();";
        ResetUiTarget();
        TweenHandle handle = play(_uiTarget.gameObject);
        int nextReplay = replayCount > 0 ? Mathf.RoundToInt(FrameRate * 0.85f) : int.MaxValue;
        int replayed = 0;

        RenderSeconds(ClipDuration, localFrame =>
        {
            if (localFrame == nextReplay && replayed < replayCount)
            {
                handle?.Kill();
                ResetUiTarget();
                handle = play(_uiTarget.gameObject);
                replayed++;
                nextReplay += Mathf.RoundToInt(FrameRate * 0.85f);
            }
        });

        handle?.Kill();
        DOTween.Kill(_uiTarget.gameObject, false);
        DOTween.Kill(_uiTarget.transform, false);
    }

    private static void Capture3D<TPreset>(string animationName, int replayCount) where TPreset : class, ITweenPreset, new()
    {
        Set2DVisible(false);
        Set3DVisible(true);
        _sectionLabel.text = "3D ANIMATION";
        _animationLabel.text = animationName;
        _footerLabel.text = $"target.Tween().Preset<{typeof(TPreset).Name}>().Play();";
        ResetWorldTarget();
        TweenHandle handle = PlayWorldPreset<TPreset>();
        int nextReplay = replayCount > 0 ? Mathf.RoundToInt(FrameRate * 1.05f) : int.MaxValue;
        int replayed = 0;

        RenderSeconds(ClipDuration, localFrame =>
        {
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
        _sectionLabel.text = "TWEEN HELPER";
        _animationLabel.text = "Build better game feel";
        _footerLabel.text = "Reusable presets for UI, transforms, materials, and more";
        RenderSeconds(1.5f, null);
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
        ((RectTransform)_uiTarget.transform).anchoredPosition = new Vector2(55f, 82f);
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
