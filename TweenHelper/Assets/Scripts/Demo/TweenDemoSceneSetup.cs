using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper.Demo;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Automatically sets up the complete TweenHelper demo scene with all necessary GameObjects and UI.
    /// Run this script in the editor to create a fully functional demo scene.
    /// </summary>
    public class TweenDemoSceneSetup : MonoBehaviour
    {
        [Header("Scene Generation")]
        [SerializeField] private bool autoSetupOnStart = false;
        [SerializeField] private Material demoMaterial;
        
        [Header("Layout Settings")]
        [SerializeField] private int demoObjectsPerRow = 3;
        [SerializeField] private float objectSpacing = 3f;
        [SerializeField] private Vector3 demoAreaCenter = Vector3.zero;
        
        [ContextMenu("Setup Complete Demo Scene")]
        public void SetupCompleteScene()
        {
            Debug.Log("Setting up TweenHelper demo scene...");
            
            // 1. Create demo objects
            var demoObjects = CreateDemoObjects();
            
            // 2. Create UI canvas and controls
            var uiCanvas = CreateUICanvas();
            
            // 3. Create demo controller and link everything
            var demoController = CreateDemoController(demoObjects, uiCanvas);
            
            // 4. Setup camera for optimal viewing
            SetupCamera();
            
            // 5. Add lighting if needed
            SetupLighting();
            
            Debug.Log($"Demo scene setup complete! Created {demoObjects.Length} demo objects and full UI.");
        }
        
        private GameObject[] CreateDemoObjects()
        {
            var demoObjects = new GameObject[9]; // 3x3 grid
            var parentObject = new GameObject("Demo Objects");
            
            for (int i = 0; i < 9; i++)
            {
                int row = i / demoObjectsPerRow;
                int col = i % demoObjectsPerRow;
                
                Vector3 position = demoAreaCenter + new Vector3(
                    (col - 1) * objectSpacing,
                    0f,
                    (row - 1) * objectSpacing
                );
                
                // Create different types of objects for variety
                GameObject obj = CreateDemoObject(i, position);
                obj.transform.SetParent(parentObject.transform);
                obj.name = $"DemoObject_{i:00}";
                
                demoObjects[i] = obj;
            }
            
            return demoObjects;
        }
        
        private GameObject CreateDemoObject(int index, Vector3 position)
        {
            GameObject obj;
            
            // Create different primitive types for variety
            switch (index % 4)
            {
                case 0:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case 1:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                case 2:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
                case 3:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    break;
                default:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
            }
            
            obj.transform.position = position;
            
            // Add random colors for visual variety
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = GetObjectColor(index);
                renderer.material = material;
            }
            
            // Add CanvasGroup for fade demonstrations
            var canvasGroup = obj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            
            // Add TweenLifecycleTracker for safety
            obj.AddComponent<LB.TweenHelper.TweenLifecycleTracker>();
            
            return obj;
        }
        
        private Color GetObjectColor(int index)
        {
            Color[] colors = {
                Color.red, Color.green, Color.blue, Color.yellow,
                Color.cyan, Color.magenta, new Color(1f, 0.5f, 0f), // orange
                new Color(0.5f, 0f, 1f), // purple
                new Color(0f, 1f, 0.5f)  // spring green
            };
            
            return colors[index % colors.Length];
        }
        
        private GameObject CreateUICanvas()
        {
            // Create main canvas
            var canvasObj = new GameObject("Demo UI Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            
            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Create header panel
            CreateHeaderPanel(canvasObj.transform);
            
            // Create demo selection buttons
            CreateDemoSelectionPanel(canvasObj.transform);
            
            // Create individual demo panels (initially hidden)
            CreateIndividualDemoPanels(canvasObj.transform);
            
            // Create footer with controls
            CreateFooterPanel(canvasObj.transform);
            
            return canvasObj;
        }
        
        private void CreateHeaderPanel(Transform canvasTransform)
        {
            var headerPanel = CreateUIPanel("Header Panel", canvasTransform);
            var headerRect = headerPanel.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.92f);
            headerRect.anchorMax = new Vector2(1, 1f);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;
            
            // Title text
            var titleText = CreateUIText("Title Text", "LB.TweenHelper Demo", headerPanel.transform, 22);
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0.6f, 1);
            titleRect.offsetMin = new Vector2(20, 0);
            titleRect.offsetMax = new Vector2(0, 0);
            
            // Current demo text
            var currentDemoText = CreateUIText("Current Demo Text", "Demo: Basic Animations", headerPanel.transform, 16);
            currentDemoText.name = "CurrentDemoText"; // For easy reference
            var currentRect = currentDemoText.GetComponent<RectTransform>();
            currentRect.anchorMin = new Vector2(0.6f, 0);
            currentRect.anchorMax = new Vector2(1f, 1);
            currentRect.offsetMin = Vector2.zero;
            currentRect.offsetMax = new Vector2(-20, 0);
        }
        
        private void CreateDemoSelectionPanel(Transform canvasTransform)
        {
            var selectionPanel = CreateUIPanel("Demo Selection Panel", canvasTransform);
            var selectionRect = selectionPanel.GetComponent<RectTransform>();
            selectionRect.anchorMin = new Vector2(0, 0.84f);
            selectionRect.anchorMax = new Vector2(1, 0.92f);
            selectionRect.offsetMin = Vector2.zero;
            selectionRect.offsetMax = Vector2.zero;
            
            // Create horizontal layout
            var layoutGroup = selectionPanel.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 8f;
            layoutGroup.padding = new RectOffset(15, 15, 5, 5);
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            
            // Demo selection buttons
            string[] demoNames = {
                "Basic Animations", "Presets", "Sequences", "Stagger", 
                "Control", "Async", "Options"
            };
            
            for (int i = 0; i < demoNames.Length; i++)
            {
                var button = CreateUIButton($"Demo{i}Button", demoNames[i], selectionPanel.transform);
                button.name = $"DemoButton_{i}"; // For easy reference
            }
        }
        
        private void CreateIndividualDemoPanels(Transform canvasTransform)
        {
            // Create panels for each demo type with their specific controls
            var basicPanel = CreateBasicAnimationPanel(canvasTransform);
            CreatePresetPanel(canvasTransform);
            CreateSequencePanel(canvasTransform);
            CreateStaggerPanel(canvasTransform);
            CreateControlPanel(canvasTransform);
            CreateAsyncPanel(canvasTransform);
            CreateOptionsPanel(canvasTransform);
            
            // Activate the first panel (Basic Animations) by default
            basicPanel.SetActive(true);
        }
        
        private GameObject CreateBasicAnimationPanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Basic Animation Panel", canvasTransform);
            
            string[] buttonNames = { "Move", "Rotate", "Scale", "Fade", "Combined" };
            CreateDemoButtons(panel.transform, buttonNames, "BasicAnim");
            
            return panel;
        }
        
        private void CreatePresetPanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Preset Panel", canvasTransform);
            
            string[] buttonNames = { "PopIn", "PopOut", "Bounce", "Shake", "FadeIn", "FadeOut", "All Presets", "Random" };
            CreateDemoButtons(panel.transform, buttonNames, "Preset");
        }
        
        private void CreateSequencePanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Sequence Panel", canvasTransform);
            
            string[] buttonNames = { "Simple", "Parallel", "Complex", "Looped", "Callbacks", "Presets" };
            CreateDemoButtons(panel.transform, buttonNames, "Sequence");
        }
        
        private void CreateStaggerPanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Stagger Panel", canvasTransform);
            
            string[] buttonNames = { "Move", "Scale", "Preset", "Fade", "Wave", "Cascade" };
            CreateDemoButtons(panel.transform, buttonNames, "Stagger");
        }
        
        private void CreateControlPanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Control Panel", canvasTransform);
            
            string[] buttonNames = { "Start", "Pause All", "Resume All", "Kill All", "Complete All", "ID Control", "Diagnostics", "Pause Obj", "Resume Obj", "Kill Obj" };
            CreateDemoButtons(panel.transform, buttonNames, "Control");
        }
        
        private void CreateAsyncPanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Async Panel", canvasTransform);
            
            string[] buttonNames = { "Await Complete", "Await Timeout", "Await All", "Await Any", "Cancellation", "Async Sequence", "Direct Await" };
            CreateDemoButtons(panel.transform, buttonNames, "Async");
        }
        
        private void CreateOptionsPanel(Transform canvasTransform)
        {
            var panel = CreateDemoPanel("Options Panel", canvasTransform);
            
            string[] buttonNames = { "Easing", "Delay", "Loops", "Unscaled Time", "Snapping", "Speed Based", "Combined", "Fluent API", "Comparison" };
            CreateDemoButtons(panel.transform, buttonNames, "Options");
        }
        
        private GameObject CreateDemoPanel(string name, Transform parent)
        {
            var panel = CreateUIPanel(name, parent);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.12f);
            panelRect.anchorMax = new Vector2(1, 0.84f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Initially hide all demo panels except the first one
            panel.SetActive(false);
            
            return panel;
        }
        
        private void CreateDemoButtons(Transform parent, string[] buttonNames, string prefix)
        {
            var layoutGroup = parent.gameObject.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(150, 35);
            layoutGroup.spacing = new Vector2(8, 8);
            layoutGroup.padding = new RectOffset(15, 15, 15, 15);
            layoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            layoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.constraintCount = 4; // Max 4 buttons per row
            
            for (int i = 0; i < buttonNames.Length; i++)
            {
                var button = CreateUIButton($"{prefix}Button_{i}", buttonNames[i], parent);
                button.name = $"{prefix}Button_{i:00}"; // For easy reference
            }
        }
        
        private void CreateFooterPanel(Transform canvasTransform)
        {
            var footerPanel = CreateUIPanel("Footer Panel", canvasTransform);
            var footerRect = footerPanel.GetComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0, 0);
            footerRect.anchorMax = new Vector2(1, 0.12f);
            footerRect.offsetMin = Vector2.zero;
            footerRect.offsetMax = Vector2.zero;
            
            // Instructions text
            var instructionsText = CreateUIText("Instructions Text", "Click buttons to see different animation demos. Use arrow keys to switch demo sections.", footerPanel.transform, 14);
            instructionsText.name = "InstructionsText"; // For easy reference
            var instructionsRect = instructionsText.GetComponent<RectTransform>();
            instructionsRect.anchorMin = Vector2.zero;
            instructionsRect.anchorMax = Vector2.one;
            instructionsRect.offsetMin = new Vector2(20, 20);
            instructionsRect.offsetMax = new Vector2(-20, -20);
        }
        
        private GameObject CreateUIPanel(string name, Transform parent)
        {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent);
            
            var rectTransform = panelObj.AddComponent<RectTransform>();
            var image = panelObj.AddComponent<Image>();
            image.color = new Color(0.05f, 0.05f, 0.05f, 0.7f); // Semi-transparent dark background
            
            return panelObj;
        }
        
        private GameObject CreateUIButton(string name, string buttonText, Transform parent)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);
            
            var rectTransform = buttonObj.AddComponent<RectTransform>();
            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.5f, 0.8f); // Blue button background with transparency
            
            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;
            
            // Button text
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var text = textObj.AddComponent<Text>();
            text.text = buttonText;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 12;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            
            return buttonObj;
        }
        
        private GameObject CreateUIText(string name, string textContent, Transform parent, int fontSize = 16)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            
            var rectTransform = textObj.AddComponent<RectTransform>();
            var text = textObj.AddComponent<Text>();
            text.text = textContent;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            
            return textObj;
        }
        
        private TweenDemoController CreateDemoController(GameObject[] demoObjects, GameObject uiCanvas)
        {
            var controllerObj = new GameObject("TweenDemoController");
            var controller = controllerObj.AddComponent<TweenDemoController>();
            
            // Add all demo components
            var basicDemo = controllerObj.AddComponent<BasicAnimationDemo>();
            var presetDemo = controllerObj.AddComponent<PresetDemo>();
            var sequenceDemo = controllerObj.AddComponent<SequenceDemo>();
            var staggerDemo = controllerObj.AddComponent<StaggerDemo>();
            var controlDemo = controllerObj.AddComponent<ControlDemo>();
            var asyncDemo = controllerObj.AddComponent<AsyncDemo>();
            var optionsDemo = controllerObj.AddComponent<OptionsDemo>();
            
            // Link demo objects
            SetFieldValue(controller, "demoObjects", demoObjects);
            
            // Initialize demo components
            basicDemo.Initialize(demoObjects);
            presetDemo.Initialize(demoObjects);
            sequenceDemo.Initialize(demoObjects);
            staggerDemo.Initialize(demoObjects);
            controlDemo.Initialize(demoObjects);
            asyncDemo.Initialize(demoObjects);
            optionsDemo.Initialize(demoObjects);
            
            // Link UI elements and set up OnClick events
            LinkUIElements(controller, uiCanvas);
            
            // Set only BasicAnimationDemo enabled initially
            basicDemo.enabled = true;
            presetDemo.enabled = false;
            sequenceDemo.enabled = false;
            staggerDemo.enabled = false;
            controlDemo.enabled = false;
            asyncDemo.enabled = false;
            optionsDemo.enabled = false;
            
            return controller;
        }
        
        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObj = new GameObject("Main Camera");
                camera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
                cameraObj.tag = "MainCamera";
            }
            
            // Position camera for optimal viewing of demo objects
            camera.transform.position = new Vector3(0, 8, -12);
            camera.transform.rotation = Quaternion.Euler(30, 0, 0);
            camera.clearFlags = CameraClearFlags.Skybox;
        }
        
        private void SetupLighting()
        {
            // Add directional light if none exists
            var existingLight = FindFirstObjectByType<Light>();
            if (existingLight == null)
            {
                var lightObj = new GameObject("Directional Light");
                var light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                light.color = Color.white;
                light.intensity = 1f;
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
        }
        
        private void LinkUIElements(TweenDemoController controller, GameObject uiCanvas)
        {
            var canvas = uiCanvas.transform;
            
            // Find main UI elements
            var currentDemoText = canvas.Find("Header Panel/CurrentDemoText")?.GetComponent<Text>();
            var instructionsText = canvas.Find("Footer Panel/InstructionsText")?.GetComponent<Text>();
            
            if (currentDemoText != null) SetFieldValue(controller, "currentDemoText", currentDemoText);
            if (instructionsText != null) SetFieldValue(controller, "instructionsText", instructionsText);
            
            // Set up demo selection buttons with OnClick events
            var selectionPanel = canvas.Find("Demo Selection Panel");
            if (selectionPanel != null)
            {
                var demoButtons = new Button[7];
                for (int i = 0; i < 7; i++)
                {
                    var button = selectionPanel.Find($"DemoButton_{i}")?.GetComponent<Button>();
                    if (button != null)
                    {
                        demoButtons[i] = button;
                        int index = i; // Capture for closure
                        button.onClick.AddListener(() => controller.SwitchToDemo(index));
                    }
                }
                SetFieldValue(controller, "demoButtons", demoButtons);
            }
            
            // Link individual demo buttons
            LinkBasicAnimationButtons(controller.GetComponent<BasicAnimationDemo>(), canvas);
            LinkPresetButtons(controller.GetComponent<PresetDemo>(), canvas);
            LinkSequenceButtons(controller.GetComponent<SequenceDemo>(), canvas);
            LinkStaggerButtons(controller.GetComponent<StaggerDemo>(), canvas);
            LinkControlButtons(controller.GetComponent<ControlDemo>(), canvas);
            LinkAsyncButtons(controller.GetComponent<AsyncDemo>(), canvas);
            LinkOptionsButtons(controller.GetComponent<OptionsDemo>(), canvas);
            
            Debug.Log("UI elements linked and OnClick events set up automatically!");
        }
        
        private void LinkBasicAnimationButtons(BasicAnimationDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Basic Animation Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "moveButton", panel.Find("BasicAnimButton_00"), demo.DemoMove);
            SetButtonField(demo, "rotateButton", panel.Find("BasicAnimButton_01"), demo.DemoRotate);
            SetButtonField(demo, "scaleButton", panel.Find("BasicAnimButton_02"), demo.DemoScale);
            SetButtonField(demo, "fadeButton", panel.Find("BasicAnimButton_03"), demo.DemoFade);
            SetButtonField(demo, "combinedButton", panel.Find("BasicAnimButton_04"), demo.DemoCombined);
        }
        
        private void LinkPresetButtons(PresetDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Preset Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "popInButton", panel.Find("PresetButton_00"), () => demo.DemoPreset("PopIn"));
            SetButtonField(demo, "popOutButton", panel.Find("PresetButton_01"), () => demo.DemoPreset("PopOut"));
            SetButtonField(demo, "bounceButton", panel.Find("PresetButton_02"), () => demo.DemoPreset("Bounce"));
            SetButtonField(demo, "shakeButton", panel.Find("PresetButton_03"), () => demo.DemoPreset("Shake"));
            SetButtonField(demo, "fadeInButton", panel.Find("PresetButton_04"), () => demo.DemoPreset("FadeIn"));
            SetButtonField(demo, "fadeOutButton", panel.Find("PresetButton_05"), () => demo.DemoPreset("FadeOut"));
            SetButtonField(demo, "allPresetsButton", panel.Find("PresetButton_06"), demo.DemoAllPresets);
            SetButtonField(demo, "randomPresetButton", panel.Find("PresetButton_07"), demo.DemoRandomPreset);
        }
        
        private void LinkSequenceButtons(SequenceDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Sequence Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "simpleSequenceButton", panel.Find("SequenceButton_00"), demo.DemoSimpleSequence);
            SetButtonField(demo, "parallelSequenceButton", panel.Find("SequenceButton_01"), demo.DemoParallelSequence);
            SetButtonField(demo, "complexSequenceButton", panel.Find("SequenceButton_02"), demo.DemoComplexSequence);
            SetButtonField(demo, "loopSequenceButton", panel.Find("SequenceButton_03"), demo.DemoLoopSequence);
            SetButtonField(demo, "callbackSequenceButton", panel.Find("SequenceButton_04"), demo.DemoCallbackSequence);
            SetButtonField(demo, "presetSequenceButton", panel.Find("SequenceButton_05"), demo.DemoPresetSequence);
        }
        
        private void LinkStaggerButtons(StaggerDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Stagger Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "staggerMoveButton", panel.Find("StaggerButton_00"), demo.DemoStaggerMove);
            SetButtonField(demo, "staggerScaleButton", panel.Find("StaggerButton_01"), demo.DemoStaggerScale);
            SetButtonField(demo, "staggerPresetButton", panel.Find("StaggerButton_02"), demo.DemoStaggerPreset);
            SetButtonField(demo, "staggerFadeButton", panel.Find("StaggerButton_03"), demo.DemoStaggerFade);
            SetButtonField(demo, "waveAnimationButton", panel.Find("StaggerButton_04"), demo.DemoWaveAnimation);
            SetButtonField(demo, "cascadeButton", panel.Find("StaggerButton_05"), demo.DemoCascadeEffect);
        }
        
        private void LinkControlButtons(ControlDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Control Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "startAnimationsButton", panel.Find("ControlButton_00"), demo.StartLongAnimations);
            SetButtonField(demo, "pauseAllButton", panel.Find("ControlButton_01"), demo.PauseAllAnimations);
            SetButtonField(demo, "resumeAllButton", panel.Find("ControlButton_02"), demo.ResumeAllAnimations);
            SetButtonField(demo, "killAllButton", panel.Find("ControlButton_03"), demo.KillAllAnimations);
            SetButtonField(demo, "completeAllButton", panel.Find("ControlButton_04"), demo.CompleteAllAnimations);
            SetButtonField(demo, "idControlButton", panel.Find("ControlButton_05"), demo.DemoIdControl);
            SetButtonField(demo, "diagnosticsButton", panel.Find("ControlButton_06"), demo.ShowDiagnostics);
            SetButtonField(demo, "pauseObjectButton", panel.Find("ControlButton_07"), demo.PauseSelectedObject);
            SetButtonField(demo, "resumeObjectButton", panel.Find("ControlButton_08"), demo.ResumeSelectedObject);
            SetButtonField(demo, "killObjectButton", panel.Find("ControlButton_09"), demo.KillSelectedObject);
        }
        
        private void LinkAsyncButtons(AsyncDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Async Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "awaitCompletionButton", panel.Find("AsyncButton_00"), () => _ = demo.DemoAwaitCompletion());
            SetButtonField(demo, "awaitTimeoutButton", panel.Find("AsyncButton_01"), () => _ = demo.DemoAwaitTimeout());
            SetButtonField(demo, "awaitAllButton", panel.Find("AsyncButton_02"), () => _ = demo.DemoAwaitAll());
            SetButtonField(demo, "awaitAnyButton", panel.Find("AsyncButton_03"), () => _ = demo.DemoAwaitAny());
            SetButtonField(demo, "cancellationButton", panel.Find("AsyncButton_04"), () => _ = demo.DemoCancellation());
            SetButtonField(demo, "asyncSequenceButton", panel.Find("AsyncButton_05"), () => _ = demo.DemoAsyncSequence());
            SetButtonField(demo, "directAwaitButton", panel.Find("AsyncButton_06"), () => _ = demo.DemoDirectAwait());
        }
        
        private void LinkOptionsButtons(OptionsDemo demo, Transform canvas)
        {
            var panel = canvas.Find("Options Panel");
            if (panel == null || demo == null) return;
            
            SetButtonField(demo, "easingButton", panel.Find("OptionsButton_00"), demo.DemoEasing);
            SetButtonField(demo, "delayButton", panel.Find("OptionsButton_01"), demo.DemoDelay);
            SetButtonField(demo, "loopsButton", panel.Find("OptionsButton_02"), demo.DemoLoops);
            SetButtonField(demo, "unscaledTimeButton", panel.Find("OptionsButton_03"), demo.DemoUnscaledTime);
            SetButtonField(demo, "snappingButton", panel.Find("OptionsButton_04"), demo.DemoSnapping);
            SetButtonField(demo, "speedBasedButton", panel.Find("OptionsButton_05"), demo.DemoSpeedBased);
            SetButtonField(demo, "combinedOptionsButton", panel.Find("OptionsButton_06"), demo.DemoCombinedOptions);
            SetButtonField(demo, "fluentApiButton", panel.Find("OptionsButton_07"), demo.DemoFluentApi);
            SetButtonField(demo, "easingComparisonButton", panel.Find("OptionsButton_08"), demo.DemoEasingComparison);
        }
        
        private void SetButtonField(object target, string fieldName, Transform buttonTransform, System.Action onClick)
        {
            if (buttonTransform == null) return;
            
            var button = buttonTransform.GetComponent<Button>();
            if (button == null) return;
            
            // Set the field reference
            SetFieldValue(target, fieldName, button);
            
            // Set up OnClick event
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }
        
        private void SetFieldValue(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupCompleteScene();
            }
        }
        
        [ContextMenu("Clear Demo Scene")]
        public void ClearDemoScene()
        {
            // Find and destroy demo objects
            var demoObjectsParent = GameObject.Find("Demo Objects");
            if (demoObjectsParent != null) DestroyImmediate(demoObjectsParent);
            
            var uiCanvas = GameObject.Find("Demo UI Canvas");
            if (uiCanvas != null) DestroyImmediate(uiCanvas);
            
            var controller = FindFirstObjectByType<TweenDemoController>();
            if (controller != null) DestroyImmediate(controller.gameObject);
            
            Debug.Log("Demo scene cleared!");
        }
    }
}
