using LB.TweenHelper.Demo;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    public static class PresetReviewSceneBuilder
    {
        private const string SceneFolder = "Assets/_Project/TweenHelperDevelopment/Validation/Scenes";
        private const string ScenePath = SceneFolder + "/TweenHelperPresetReview.unity";
        private const string MaterialPath = SceneFolder + "/TweenHelperPresetReviewMaterial.mat";
        private static readonly Color BackgroundColor = new Color(0.025f, 0.04f, 0.08f);
        private static readonly Color PanelColor = new Color(0.055f, 0.08f, 0.14f, 0.94f);
        private static readonly Color BlueColor = new Color(0.1f, 0.58f, 0.95f);
        private static readonly Color MutedTextColor = new Color(0.68f, 0.74f, 0.84f);

        [InitializeOnLoadMethod]
        private static void RefreshOpenReviewSceneIfNeeded()
        {
            EditorApplication.delayCall += () =>
            {
                if (Application.isPlaying) return;
                string fullScenePath = Path.GetFullPath(ScenePath);
                if (!File.Exists(fullScenePath)) return;

                string sceneText = File.ReadAllText(fullScenePath);
                bool sceneNeedsFilters = !sceneText.Contains("allFilterToggle:");
                bool sceneNeedsCollectionPreview = !sceneText.Contains("collectionPreviewRoot:");
                bool sceneNeedsDestinationPreview = !sceneText.Contains("destinationWorldRoot:");
                bool sceneNeedsUISequencePreview = !sceneText.Contains("uiSequencePreviewRoot:");
                bool sceneNeedsTextValuePreview = !sceneText.Contains("textValuePreviewRoot:");
                bool sceneNeedsProgressPreview = !sceneText.Contains("progressPreviewRoot:");
                bool sceneNeedsCameraFeedback = !sceneText.Contains("feedbackCamera:");
                bool sceneNeedsEnginePropertyPreview = !sceneText.Contains("enginePropertyPreviewRoot:");
                bool sceneNeedsCoveragePreview = !sceneText.Contains("incompleteGridPreviewGroup:") ||
                                                 !sceneText.Contains("worldCollectionPreviewRoot:") ||
                                                 !sceneText.Contains("drawerSequenceBackdrop:") ||
                                                 !sceneText.Contains("worldTextValuePreviewRoot:");
                bool sceneNeedsMaterial = !File.Exists(Path.GetFullPath(MaterialPath));
                if (!sceneNeedsFilters && !sceneNeedsCollectionPreview && !sceneNeedsDestinationPreview && !sceneNeedsUISequencePreview && !sceneNeedsTextValuePreview && !sceneNeedsProgressPreview && !sceneNeedsCameraFeedback && !sceneNeedsEnginePropertyPreview && !sceneNeedsCoveragePreview && !sceneNeedsMaterial)
                {
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
                    if (material == null || !NeedsTransparentConfiguration(material)) return;
                    ConfigurePreviewMaterial(material);
                    EditorUtility.SetDirty(material);
                    AssetDatabase.SaveAssets();
                    return;
                }

                BuildScene();
            };
        }

        [MenuItem("Tools/Tween Helper Dev/Build Preset Review Scene")]
        public static void BuildScene()
        {
            EnsureFolder("Assets/_Project/TweenHelperDevelopment/Validation", "Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Camera camera = CreateCamera();
            CreateLight();
            GameObject worldTarget = CreateWorldTarget();
            Canvas canvas = CreateCanvas();
            CreateEventSystem();

            RectTransform header = CreatePanel("Header", canvas.transform, PanelColor);
            SetRect(header, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -112f), Vector2.zero);
            TMP_Text title = CreateText("Title", header, "TWEEN HELPER  -  ANIMATION REVIEW", 34, FontStyles.Bold, TextAlignmentOptions.Left);
            SetRect(title.rectTransform, new Vector2(0f, 0f), new Vector2(0.7f, 1f), new Vector2(34f, 0f), new Vector2(-10f, 0f));
            TMP_Text totals = CreateText("Totals", header, string.Empty, 20, FontStyles.Normal, TextAlignmentOptions.Right);
            totals.color = MutedTextColor;
            SetRect(totals.rectTransform, new Vector2(0.55f, 0f), new Vector2(1f, 1f), new Vector2(10f, 0f), new Vector2(-34f, 0f));

            RectTransform filterPanel = CreatePanel("Review Filters", canvas.transform, PanelColor);
            SetRect(filterPanel, new Vector2(0.27f, 0.86f), new Vector2(0.73f, 0.925f), Vector2.zero, Vector2.zero);
            var filterGroup = filterPanel.gameObject.AddComponent<ToggleGroup>();
            filterGroup.allowSwitchOff = false;
            Toggle allFilter = CreateToggle("All Filter", filterPanel, "ALL", filterGroup);
            SetRect((RectTransform)allFilter.transform, new Vector2(0.015f, 0.12f), new Vector2(0.325f, 0.88f), Vector2.zero, Vector2.zero);
            Toggle unreviewedFilter = CreateToggle("Unreviewed Filter", filterPanel, "NOT REVIEWED", filterGroup);
            SetRect((RectTransform)unreviewedFilter.transform, new Vector2(0.345f, 0.12f), new Vector2(0.655f, 0.88f), Vector2.zero, Vector2.zero);
            Toggle failedFilter = CreateToggle("Failed Filter", filterPanel, "NEEDS WORK", filterGroup);
            SetRect((RectTransform)failedFilter.transform, new Vector2(0.675f, 0.12f), new Vector2(0.985f, 0.88f), Vector2.zero, Vector2.zero);
            allFilter.SetIsOnWithoutNotify(true);
            unreviewedFilter.SetIsOnWithoutNotify(false);
            failedFilter.SetIsOnWithoutNotify(false);

            RectTransform infoPanel = CreatePanel("Animation Information", canvas.transform, PanelColor);
            SetRect(infoPanel, new Vector2(0.19f, 0.67f), new Vector2(0.81f, 0.85f), Vector2.zero, Vector2.zero);
            TMP_Text category = CreateText("Category", infoPanel, string.Empty, 17, FontStyles.Bold, TextAlignmentOptions.Center);
            category.color = BlueColor;
            SetRect(category.rectTransform, new Vector2(0f, 0.72f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            TMP_Text itemName = CreateText("Animation Name", infoPanel, string.Empty, 36, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(itemName.rectTransform, new Vector2(0f, 0.35f), new Vector2(1f, 0.78f), new Vector2(20f, 0f), new Vector2(-20f, 0f));
            TMP_Text description = CreateText("Description", infoPanel, string.Empty, 19, FontStyles.Normal, TextAlignmentOptions.Center);
            description.color = MutedTextColor;
            SetRect(description.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.4f), new Vector2(35f, 0f), new Vector2(-35f, 0f));

            RectTransform previewFrame = CreatePanel("Preview", canvas.transform, new Color(0.03f, 0.055f, 0.1f, 0.35f));
            SetRect(previewFrame, new Vector2(0.2f, 0.24f), new Vector2(0.8f, 0.65f), Vector2.zero, Vector2.zero);
            GameObject uiTarget = CreateUiTarget(previewFrame);
            CollectionPreview collectionPreview = CreateCollectionPreview(previewFrame);
            DestinationPreview destinationPreview = CreateDestinationPreview(previewFrame);
            UISequencePreview uiSequencePreview = CreateUISequencePreview(previewFrame);
            TextValuePreview textValuePreview = CreateTextValuePreview(previewFrame);
            ProgressPreview progressPreview = CreateProgressPreview(previewFrame);
            EnginePropertyPreview enginePropertyPreview = CreateEnginePropertyPreview(previewFrame);

            Button failed = CreateButton("Mark Wrong", canvas.transform, "[X]  WRONG / NEEDS WORK", new Color(0.42f, 0.16f, 0.2f));
            SetRect((RectTransform)failed.transform, new Vector2(0.015f, 0.3f), new Vector2(0.18f, 0.63f), Vector2.zero, Vector2.zero);
            Button passed = CreateButton("Mark Correct", canvas.transform, "[OK]  CORRECT", new Color(0.12f, 0.35f, 0.25f));
            SetRect((RectTransform)passed.transform, new Vector2(0.82f, 0.3f), new Vector2(0.985f, 0.63f), Vector2.zero, Vector2.zero);

            RectTransform footer = CreatePanel("Footer", canvas.transform, PanelColor);
            SetRect(footer, new Vector2(0f, 0f), new Vector2(1f, 0.2f), Vector2.zero, Vector2.zero);
            TMP_Text status = CreateText("Review Status", footer, string.Empty, 22, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(status.rectTransform, new Vector2(0.34f, 0.56f), new Vector2(0.66f, 1f), Vector2.zero, Vector2.zero);
            TMP_Text position = CreateText("Position", footer, string.Empty, 18, FontStyles.Bold, TextAlignmentOptions.Center);
            position.color = MutedTextColor;
            SetRect(position.rectTransform, new Vector2(0.43f, 0.01f), new Vector2(0.57f, 0.14f), Vector2.zero, Vector2.zero);

            Button previous = CreateButton("Previous", footer, "<  PREVIOUS", new Color(0.12f, 0.24f, 0.42f));
            SetRect((RectTransform)previous.transform, new Vector2(0.16f, 0.15f), new Vector2(0.34f, 0.58f), Vector2.zero, Vector2.zero);
            Button replay = CreateButton("Replay", footer, "PLAY / REPLAY", new Color(0.1f, 0.42f, 0.7f));
            SetRect((RectTransform)replay.transform, new Vector2(0.385f, 0.15f), new Vector2(0.615f, 0.58f), Vector2.zero, Vector2.zero);
            Button next = CreateButton("Next", footer, "NEXT  >", new Color(0.12f, 0.24f, 0.42f));
            SetRect((RectTransform)next.transform, new Vector2(0.66f, 0.15f), new Vector2(0.84f, 0.58f), Vector2.zero, Vector2.zero);

            var controllerObject = new GameObject("Preset Review Controller");
            var controller = controllerObject.AddComponent<PresetReviewController>();
            var serializedController = new SerializedObject(controller);
            Assign(serializedController, "uiTarget", uiTarget);
            Assign(serializedController, "worldTarget", worldTarget);
            Assign(serializedController, "collectionPreviewRoot", collectionPreview.Root);
            Assign(serializedController, "listPreviewGroup", collectionPreview.ListGroup);
            Assign(serializedController, "gridPreviewGroup", collectionPreview.GridGroup);
            Assign(serializedController, "incompleteGridPreviewGroup", collectionPreview.IncompleteGridGroup);
            Assign(serializedController, "worldCollectionPreviewRoot", collectionPreview.WorldRoot);
            Assign(serializedController, "loadingDotsPreviewGroup", collectionPreview.LoadingDotsGroup);
            AssignArray(serializedController, "listTargets", collectionPreview.ListTargets);
            AssignArray(serializedController, "gridTargets", collectionPreview.GridTargets);
            AssignArray(serializedController, "incompleteGridTargets", collectionPreview.IncompleteGridTargets);
            AssignArray(serializedController, "worldCollectionTargets", collectionPreview.WorldTargets);
            AssignArray(serializedController, "loadingDotTargets", collectionPreview.LoadingDotTargets);
            Assign(serializedController, "destinationWorldRoot", destinationPreview.WorldRoot);
            Assign(serializedController, "destinationWorldTarget", destinationPreview.WorldTarget);
            Assign(serializedController, "destinationWorldStartMarker", destinationPreview.WorldStartMarker);
            Assign(serializedController, "destinationWorldEndMarker", destinationPreview.WorldEndMarker);
            Assign(serializedController, "destinationWorldCurvedPath", destinationPreview.WorldCurvedPath);
            Assign(serializedController, "destinationUiRoot", destinationPreview.UiRoot);
            Assign(serializedController, "destinationUiTarget", destinationPreview.UiTarget);
            Assign(serializedController, "destinationUiStartMarker", destinationPreview.UiStartMarker);
            Assign(serializedController, "destinationUiEndMarker", destinationPreview.UiEndMarker);
            Assign(serializedController, "destinationUiCurvedPath", destinationPreview.UiCurvedPath);
            Assign(serializedController, "uiSequencePreviewRoot", uiSequencePreview.Root);
            Assign(serializedController, "toastSequenceTarget", uiSequencePreview.Toast);
            Assign(serializedController, "modalSequenceGroup", uiSequencePreview.ModalGroup);
            Assign(serializedController, "modalSequenceBackdrop", uiSequencePreview.ModalBackdrop);
            Assign(serializedController, "modalSequencePanel", uiSequencePreview.ModalPanel);
            AssignArray(serializedController, "modalSequenceControls", uiSequencePreview.ModalControls);
            Assign(serializedController, "tooltipSequenceTarget", uiSequencePreview.Tooltip);
            Assign(serializedController, "dropdownSequencePanel", uiSequencePreview.DropdownPanel);
            AssignArray(serializedController, "dropdownSequenceEntries", uiSequencePreview.DropdownEntries);
            Assign(serializedController, "tabSequenceGroup", uiSequencePreview.TabGroup);
            Assign(serializedController, "tabSequenceOutgoing", uiSequencePreview.TabOutgoing);
            Assign(serializedController, "tabSequenceIncoming", uiSequencePreview.TabIncoming);
            Assign(serializedController, "drawerSequenceBackdrop", uiSequencePreview.DrawerBackdrop);
            Assign(serializedController, "textValuePreviewRoot", textValuePreview.Root);
            Assign(serializedController, "typewriterText", textValuePreview.Typewriter);
            Assign(serializedController, "numberText", textValuePreview.Number);
            Assign(serializedController, "characterText", textValuePreview.Character);
            Assign(serializedController, "scoreText", textValuePreview.Score);
            Assign(serializedController, "worldTextValuePreviewRoot", textValuePreview.WorldRoot);
            Assign(serializedController, "worldCharacterText", textValuePreview.WorldCharacter);
            Assign(serializedController, "progressPreviewRoot", progressPreview.Root);
            Assign(serializedController, "progressImageGroup", progressPreview.ImageGroup);
            Assign(serializedController, "progressImage", progressPreview.Image);
            Assign(serializedController, "progressSlider", progressPreview.Slider);
            Assign(serializedController, "progressValueText", progressPreview.ValueText);
            Assign(serializedController, "progressEventText", progressPreview.EventText);
            Assign(serializedController, "feedbackCamera", camera);
            Assign(serializedController, "cameraFocusTarget", worldTarget.transform);
            Assign(serializedController, "enginePropertyPreviewRoot", enginePropertyPreview.Root);
            Assign(serializedController, "enginePropertyWorldRoot", enginePropertyPreview.WorldRoot);
            Assign(serializedController, "engineAudioSource", enginePropertyPreview.AudioSource);
            Assign(serializedController, "engineLight", enginePropertyPreview.Light);
            Assign(serializedController, "engineParticles", enginePropertyPreview.Particles);
            Assign(serializedController, "engineRenderer", enginePropertyPreview.Renderer);
            Assign(serializedController, "enginePropertyMeter", enginePropertyPreview.Meter);
            Assign(serializedController, "enginePropertyValueText", enginePropertyPreview.ValueText);
            Assign(serializedController, "itemNameText", itemName);
            Assign(serializedController, "descriptionText", description);
            Assign(serializedController, "categoryText", category);
            Assign(serializedController, "positionText", position);
            Assign(serializedController, "statusText", status);
            Assign(serializedController, "totalsText", totals);
            Assign(serializedController, "previousButton", previous);
            Assign(serializedController, "replayButton", replay);
            Assign(serializedController, "nextButton", next);
            Assign(serializedController, "failedButton", failed);
            Assign(serializedController, "passedButton", passed);
            Assign(serializedController, "allFilterToggle", allFilter);
            Assign(serializedController, "unreviewedFilterToggle", unreviewedFilter);
            Assign(serializedController, "failedFilterToggle", failedFilter);
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            camera.transform.LookAt(worldTarget.transform);
            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            Debug.Log($"TweenHelper preset review scene built at {ScenePath}");
        }

        private static Camera CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BackgroundColor;
            camera.fieldOfView = 34f;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            light.color = new Color(0.72f, 0.84f, 1f);
            lightObject.transform.rotation = Quaternion.Euler(35f, -35f, 0f);
        }

        private static GameObject CreateWorldTarget()
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "3D Preview Target";
            target.transform.position = Vector3.zero;
            target.transform.localScale = Vector3.one * 2.25f;
            target.transform.rotation = Quaternion.Euler(18f, 28f, 0f);
            target.GetComponent<Renderer>().sharedMaterial = GetOrCreatePreviewMaterial();
            return target;
        }

        private static Material GetOrCreatePreviewMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, MaterialPath);
            }

            ConfigurePreviewMaterial(material);
            return material;
        }

        private static bool NeedsTransparentConfiguration(Material material)
        {
            return material.HasProperty("_Surface") &&
                   (material.GetFloat("_Surface") < 0.5f || material.renderQueue < (int)RenderQueue.Transparent);
        }

        private static void ConfigurePreviewMaterial(Material material)
        {
            material.color = new Color(0.12f, 0.63f, 1f, 1f);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            material.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_BlendModePreserveSpecular", 0f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.SetShaderPassEnabled("ShadowCaster", false);
            material.SetShaderPassEnabled("DepthOnly", false);
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Review Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static void CreateEventSystem()
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.GetComponent<EventSystem>().sendNavigationEvents = true;
        }

        private static GameObject CreateUiTarget(Transform parent)
        {
            var target = new GameObject("2D Preview Target", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            target.transform.SetParent(parent, false);
            var rect = (RectTransform)target.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(235f, 235f);
            target.GetComponent<Image>().color = new Color(0.12f, 0.63f, 1f);

            TMP_Text label = CreateText("Target Label", rect, "TWEEN\nHELPER", 30, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return target;
        }

        private static CollectionPreview CreateCollectionPreview(Transform parent)
        {
            var root = new GameObject("Collection Preview", typeof(RectTransform));
            root.transform.SetParent(parent, false);
            SetRect((RectTransform)root.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject listGroup = CreatePreviewGroup("List Preview", root.transform);
            var listTargets = new GameObject[6];
            for (int i = 0; i < listTargets.Length; i++)
            {
                listTargets[i] = CreateCollectionItem($"List Item {i + 1}", listGroup.transform, (i + 1).ToString(), new Vector2((i - 2.5f) * 112f, 0f), new Vector2(88f, 88f));
            }

            GameObject gridGroup = CreatePreviewGroup("Grid Preview", root.transform);
            var gridTargets = new GameObject[9];
            for (int i = 0; i < gridTargets.Length; i++)
            {
                int row = i / 3;
                int column = i % 3;
                gridTargets[i] = CreateCollectionItem($"Grid Item {i + 1}", gridGroup.transform, (i + 1).ToString(), new Vector2((column - 1) * 112f, (1 - row) * 112f), new Vector2(86f, 86f));
            }

            GameObject incompleteGridGroup = CreatePreviewGroup("Incomplete Grid Preview", root.transform);
            var incompleteGridTargets = new GameObject[8];
            for (int i = 0; i < incompleteGridTargets.Length; i++)
            {
                int row = i / 3;
                int column = i % 3;
                incompleteGridTargets[i] = CreateCollectionItem($"Incomplete Grid Item {i + 1}", incompleteGridGroup.transform, (i + 1).ToString(), new Vector2((column - 1) * 112f, (1 - row) * 112f), new Vector2(86f, 86f));
            }

            var worldRoot = new GameObject("World Collection Preview");
            var worldTargets = new GameObject[6];
            for (int i = 0; i < worldTargets.Length; i++)
            {
                int row = i / 3;
                int column = i % 3;
                worldTargets[i] = CreateWorldDestinationObject($"World Collection Item {i + 1}", PrimitiveType.Cube, worldRoot.transform, new Vector3((column - 1) * 1.55f, (0.5f - row) * 1.55f, 0f), 0.58f);
                worldTargets[i].transform.localRotation = Quaternion.Euler(12f + row * 8f, 18f + column * 16f, 0f);
            }

            GameObject loadingDotsGroup = CreatePreviewGroup("Loading Dots Preview", root.transform);
            var loadingDotTargets = new GameObject[3];
            for (int i = 0; i < loadingDotTargets.Length; i++)
            {
                TMP_Text dot = CreateText($"Loading Dot {i + 1}", loadingDotsGroup.transform, "●", 92f, FontStyles.Bold, TextAlignmentOptions.Center);
                dot.color = BlueColor;
                RectTransform dotRect = dot.rectTransform;
                dotRect.anchorMin = dotRect.anchorMax = new Vector2(0.5f, 0.5f);
                dotRect.sizeDelta = new Vector2(90f, 110f);
                dotRect.anchoredPosition = new Vector2((i - 1) * 110f, 0f);
                dot.gameObject.AddComponent<CanvasGroup>();
                loadingDotTargets[i] = dot.gameObject;
            }

            listGroup.SetActive(false);
            gridGroup.SetActive(false);
            incompleteGridGroup.SetActive(false);
            worldRoot.SetActive(false);
            loadingDotsGroup.SetActive(false);
            root.SetActive(false);
            return new CollectionPreview(root, listGroup, gridGroup, incompleteGridGroup, worldRoot, loadingDotsGroup, listTargets, gridTargets, incompleteGridTargets, worldTargets, loadingDotTargets);
        }

        private static DestinationPreview CreateDestinationPreview(Transform uiParent)
        {
            var worldRoot = new GameObject("Destination World Preview");
            Vector3 worldStart = new Vector3(-2.6f, -0.9f, 0f);
            Vector3 worldEnd = new Vector3(2.6f, -0.9f, 0f);

            GameObject worldTarget = CreateWorldDestinationObject("Destination 3D Target", PrimitiveType.Cube, worldRoot.transform, worldStart, 0.82f);
            worldTarget.transform.rotation = Quaternion.Euler(18f, 28f, 0f);
            GameObject worldStartMarker = CreateWorldDestinationMarker("World Start Marker", worldRoot.transform, worldStart, 0.28f);
            GameObject worldEndMarker = CreateWorldDestinationMarker("World Destination Marker", worldRoot.transform, worldEnd, 0.38f);
            var worldCurvedPath = new GameObject("World Curved Path Reference");
            worldCurvedPath.transform.SetParent(worldRoot.transform, false);
            for (int i = 1; i < 12; i++)
            {
                float progress = i / 12f;
                Vector3 point = Vector3.Lerp(worldStart, worldEnd, progress) + Vector3.up * (4f * 2.1f * progress * (1f - progress));
                CreateWorldDestinationObject($"World Path Point {i}", PrimitiveType.Sphere, worldCurvedPath.transform, point, 0.11f);
            }

            var uiRoot = new GameObject("Destination UI Preview", typeof(RectTransform));
            uiRoot.transform.SetParent(uiParent, false);
            SetRect((RectTransform)uiRoot.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            Vector2 uiStart = new Vector2(-300f, -70f);
            Vector2 uiEnd = new Vector2(300f, -70f);
            RectTransform uiStartMarker = CreateUiDestinationMarker("UI Start Marker", uiRoot.transform, uiStart, "START", new Color(0.1f, 0.58f, 0.95f, 0.22f));
            RectTransform uiEndMarker = CreateUiDestinationMarker("UI Destination Marker", uiRoot.transform, uiEnd, "DESTINATION", new Color(1f, 0.72f, 0.2f, 0.28f));
            GameObject uiTarget = CreateUiDestinationTarget(uiRoot.transform, uiStart);

            var uiCurvedPath = new GameObject("UI Curved Path Reference", typeof(RectTransform));
            uiCurvedPath.transform.SetParent(uiRoot.transform, false);
            SetRect((RectTransform)uiCurvedPath.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            uiCurvedPath.transform.SetAsFirstSibling();
            for (int i = 1; i < 12; i++)
            {
                float progress = i / 12f;
                Vector2 point = Vector2.Lerp(uiStart, uiEnd, progress) + Vector2.up * (4f * 175f * progress * (1f - progress));
                var dot = new GameObject($"UI Path Point {i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                dot.transform.SetParent(uiCurvedPath.transform, false);
                var dotRect = (RectTransform)dot.transform;
                dotRect.anchorMin = dotRect.anchorMax = new Vector2(0.5f, 0.5f);
                dotRect.anchoredPosition = point;
                dotRect.sizeDelta = Vector2.one * 10f;
                dot.GetComponent<Image>().color = new Color(0.32f, 0.76f, 1f, 0.65f);
            }

            worldRoot.SetActive(false);
            uiRoot.SetActive(false);
            return new DestinationPreview(worldRoot, worldTarget, worldStartMarker.transform, worldEndMarker.transform, worldCurvedPath, uiRoot, uiTarget, uiStartMarker, uiEndMarker, uiCurvedPath);
        }

        private static UISequencePreview CreateUISequencePreview(Transform parent)
        {
            GameObject root = CreatePreviewGroup("UI Sequence Preview", parent);

            GameObject drawerBackdrop = CreateSequencePanel("Drawer Backdrop", root.transform, string.Empty, Vector2.zero, new Vector2(980f, 440f), new Color(0.01f, 0.02f, 0.04f, 0.72f), 1f);
            drawerBackdrop.transform.SetAsFirstSibling();

            GameObject toast = CreateSequencePanel("Toast", root.transform, "SAVED SUCCESSFULLY", new Vector2(0f, 20f), new Vector2(460f, 92f), new Color(0.08f, 0.55f, 0.82f, 1f), 23f);

            GameObject modalGroup = CreatePreviewGroup("Modal Preview", root.transform);
            GameObject modalBackdrop = CreateSequencePanel("Modal Backdrop", modalGroup.transform, string.Empty, Vector2.zero, new Vector2(980f, 440f), new Color(0.01f, 0.02f, 0.04f, 0.78f), 1f);
            GameObject modalPanel = CreateSequencePanel("Modal Panel", modalGroup.transform, "CONFIRM ACTION", new Vector2(0f, 20f), new Vector2(500f, 280f), new Color(0.08f, 0.16f, 0.28f, 1f), 28f);
            var modalControls = new GameObject[3];
            string[] modalLabels = { "CANCEL", "DETAILS", "CONFIRM" };
            for (int i = 0; i < modalControls.Length; i++)
            {
                modalControls[i] = CreateSequencePanel($"Modal Control {i + 1}", modalPanel.transform, modalLabels[i], new Vector2((i - 1) * 145f, -72f), new Vector2(126f, 58f), i == 2 ? new Color(0.1f, 0.58f, 0.95f, 1f) : new Color(0.14f, 0.24f, 0.38f, 1f), 16f);
            }

            GameObject tooltip = CreateSequencePanel("Tooltip", root.transform, "Helpful context appears here", new Vector2(0f, 25f), new Vector2(390f, 92f), new Color(0.12f, 0.18f, 0.28f, 1f), 20f);

            GameObject dropdownPanel = CreateSequencePanel("Dropdown Panel", root.transform, string.Empty, new Vector2(0f, 135f), new Vector2(390f, 300f), new Color(0.07f, 0.13f, 0.22f, 1f), 1f);
            ((RectTransform)dropdownPanel.transform).pivot = new Vector2(0.5f, 1f);
            var dropdownEntries = new GameObject[4];
            string[] dropdownLabels = { "NEW PROJECT", "OPEN PROJECT", "SETTINGS", "QUIT" };
            for (int i = 0; i < dropdownEntries.Length; i++)
            {
                dropdownEntries[i] = CreateSequencePanel($"Dropdown Entry {i + 1}", dropdownPanel.transform, dropdownLabels[i], new Vector2(0f, 105f - i * 68f), new Vector2(340f, 52f), new Color(0.12f, 0.24f, 0.39f, 1f), 17f);
            }

            GameObject tabGroup = CreatePreviewGroup("Tab Switch Preview", root.transform);
            GameObject tabIncoming = CreateSequencePanel("Incoming Tab", tabGroup.transform, "INVENTORY\n\n12 ITEMS READY", Vector2.zero, new Vector2(570f, 280f), new Color(0.12f, 0.38f, 0.32f, 1f), 25f);
            GameObject tabOutgoing = CreateSequencePanel("Outgoing Tab", tabGroup.transform, "CHARACTER\n\nLEVEL 24", Vector2.zero, new Vector2(570f, 280f), new Color(0.1f, 0.3f, 0.55f, 1f), 25f);

            toast.SetActive(false);
            modalGroup.SetActive(false);
            tooltip.SetActive(false);
            dropdownPanel.SetActive(false);
            tabGroup.SetActive(false);
            drawerBackdrop.SetActive(false);
            root.SetActive(false);
            return new UISequencePreview(root, toast, modalGroup, modalBackdrop, modalPanel, modalControls, tooltip, dropdownPanel, dropdownEntries, tabGroup, tabOutgoing, tabIncoming, drawerBackdrop);
        }

        private static TextValuePreview CreateTextValuePreview(Transform parent)
        {
            GameObject root = CreatePreviewGroup("Text & Value Preview", parent);
            TMP_Text typewriter = CreateText("Typewriter Text", root.transform, "<b>TWEEN HELPER</b>\n<color=#58BFFF>RICH TEXT READY</color>", 46f, FontStyles.Normal, TextAlignmentOptions.Center);
            TMP_Text number = CreateText("Number Text", root.transform, "1,250", 78f, FontStyles.Bold, TextAlignmentOptions.Center);
            TMP_Text character = CreateText("Character Text", root.transform, "CHARACTER MOTION\n<color=#58BFFF>MESH SAFE</color>", 48f, FontStyles.Bold, TextAlignmentOptions.Center);
            TMP_Text score = CreateText("Score Text", root.transform, "1,200", 82f, FontStyles.Bold, TextAlignmentOptions.Center);
            score.color = new Color(1f, 0.86f, 0.42f, 1f);

            SetRect(typewriter.rectTransform, Vector2.zero, Vector2.one, new Vector2(55f, 35f), new Vector2(-55f, -35f));
            SetRect(number.rectTransform, Vector2.zero, Vector2.one, new Vector2(55f, 35f), new Vector2(-55f, -35f));
            SetRect(character.rectTransform, Vector2.zero, Vector2.one, new Vector2(55f, 35f), new Vector2(-55f, -35f));
            SetRect(score.rectTransform, Vector2.zero, Vector2.one, new Vector2(55f, 35f), new Vector2(-55f, -35f));

            typewriter.gameObject.SetActive(false);
            number.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            score.gameObject.SetActive(false);
            root.SetActive(false);

            var worldRoot = new GameObject("World Text & Value Preview");
            var worldTextObject = new GameObject("World Character Text", typeof(RectTransform), typeof(MeshRenderer), typeof(TextMeshPro));
            worldTextObject.transform.SetParent(worldRoot.transform, false);
            var worldCharacter = worldTextObject.GetComponent<TextMeshPro>();
            worldCharacter.text = "WORLD TMP\n<color=#58BFFF>MESH SAFE</color>";
            worldCharacter.fontSize = 3.2f;
            worldCharacter.fontStyle = FontStyles.Bold;
            worldCharacter.alignment = TextAlignmentOptions.Center;
            worldCharacter.color = Color.white;
            worldCharacter.textWrappingMode = TextWrappingModes.Normal;
            worldCharacter.rectTransform.sizeDelta = new Vector2(8f, 3f);
            worldCharacter.rectTransform.anchoredPosition3D = Vector3.zero;
            worldRoot.SetActive(false);
            return new TextValuePreview(root, typewriter, number, character, score, worldRoot, worldCharacter);
        }

        private static ProgressPreview CreateProgressPreview(Transform parent)
        {
            GameObject root = CreatePreviewGroup("Progress Preview", parent);
            TMP_Text heading = CreateText("Progress Heading", root.transform, "NORMALIZED VALUE", 20f, FontStyles.Bold, TextAlignmentOptions.Center);
            heading.color = MutedTextColor;
            SetRect(heading.rectTransform, new Vector2(0.18f, 0.73f), new Vector2(0.82f, 0.9f), Vector2.zero, Vector2.zero);

            GameObject imageGroup = CreatePreviewGroup("Image Fill Preview", root.transform);
            RectTransform imageFrame = CreatePanel("Image Fill Background", imageGroup.transform, new Color(0.07f, 0.12f, 0.2f, 1f));
            imageFrame.anchorMin = imageFrame.anchorMax = new Vector2(0.5f, 0.5f);
            imageFrame.anchoredPosition = new Vector2(0f, 25f);
            imageFrame.sizeDelta = new Vector2(650f, 82f);
            var imageObject = new GameObject("Image Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(imageFrame, false);
            var imageRect = (RectTransform)imageObject.transform;
            SetRect(imageRect, Vector2.zero, Vector2.one, new Vector2(8f, 8f), new Vector2(-8f, -8f));
            var image = imageObject.GetComponent<Image>();
            image.color = BlueColor;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 0.18f;

            var sliderObject = new GameObject("Slider Fill Preview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Slider));
            sliderObject.transform.SetParent(root.transform, false);
            var sliderRect = (RectTransform)sliderObject.transform;
            sliderRect.anchorMin = sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(0f, 25f);
            sliderRect.sizeDelta = new Vector2(650f, 82f);
            var sliderBackground = sliderObject.GetComponent<Image>();
            sliderBackground.color = new Color(0.07f, 0.12f, 0.2f, 1f);

            var fillAreaObject = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaObject.transform.SetParent(sliderObject.transform, false);
            var fillArea = (RectTransform)fillAreaObject.transform;
            SetRect(fillArea, Vector2.zero, Vector2.one, new Vector2(8f, 8f), new Vector2(-8f, -8f));
            var sliderFillObject = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            sliderFillObject.transform.SetParent(fillArea, false);
            var sliderFillRect = (RectTransform)sliderFillObject.transform;
            SetRect(sliderFillRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            sliderFillObject.GetComponent<Image>().color = BlueColor;

            var slider = sliderObject.GetComponent<Slider>();
            slider.targetGraphic = sliderBackground;
            slider.fillRect = sliderFillRect;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 20f;
            slider.maxValue = 120f;
            slider.value = 42f;

            TMP_Text valueText = CreateText("Progress Value", root.transform, "18%", 62f, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(valueText.rectTransform, new Vector2(0.25f, 0.18f), new Vector2(0.75f, 0.46f), Vector2.zero, Vector2.zero);
            TMP_Text eventText = CreateText("Progress Event", root.transform, string.Empty, 18f, FontStyles.Bold, TextAlignmentOptions.Center);
            eventText.color = new Color(1f, 0.78f, 0.22f, 1f);
            SetRect(eventText.rectTransform, new Vector2(0.18f, 0.05f), new Vector2(0.82f, 0.2f), Vector2.zero, Vector2.zero);

            sliderObject.SetActive(false);
            root.SetActive(false);
            return new ProgressPreview(root, imageGroup, image, slider, valueText, eventText);
        }

        private static EnginePropertyPreview CreateEnginePropertyPreview(Transform parent)
        {
            GameObject root = CreatePreviewGroup("Engine Property Preview", parent);
            RectTransform meterFrame = CreatePanel("Property Meter Background", root.transform, new Color(0.07f, 0.12f, 0.2f, 0.94f));
            meterFrame.anchorMin = meterFrame.anchorMax = new Vector2(0.5f, 0.5f);
            meterFrame.anchoredPosition = new Vector2(0f, -142f);
            meterFrame.sizeDelta = new Vector2(620f, 54f);
            var meterObject = new GameObject("Property Meter", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            meterObject.transform.SetParent(meterFrame, false);
            var meterRect = (RectTransform)meterObject.transform;
            SetRect(meterRect, Vector2.zero, Vector2.one, new Vector2(6f, 6f), new Vector2(-6f, -6f));
            var meter = meterObject.GetComponent<Image>();
            meter.color = BlueColor;
            meter.type = Image.Type.Filled;
            meter.fillMethod = Image.FillMethod.Horizontal;
            meter.fillOrigin = (int)Image.OriginHorizontal.Left;
            meter.fillAmount = 0.25f;

            TMP_Text valueText = CreateText("Property Value", root.transform, string.Empty, 21f, FontStyles.Bold, TextAlignmentOptions.Center);
            valueText.color = Color.white;
            SetRect(valueText.rectTransform, new Vector2(0.2f, 0.02f), new Vector2(0.8f, 0.18f), Vector2.zero, Vector2.zero);

            var worldRoot = new GameObject("Engine Property World Preview");
            GameObject visual = CreateWorldDestinationObject("Property Renderer", PrimitiveType.Sphere, worldRoot.transform, new Vector3(0f, 0.5f, 0f), 1.35f);
            Renderer renderer = visual.GetComponent<Renderer>();

            var audioObject = new GameObject("Property Audio Source", typeof(AudioSource));
            audioObject.transform.SetParent(worldRoot.transform, false);
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.25f;
            audioSource.pitch = 0.75f;

            var lightObject = new GameObject("Property Point Light", typeof(Light));
            lightObject.transform.SetParent(worldRoot.transform, false);
            lightObject.transform.localPosition = new Vector3(0f, 1.8f, -2.2f);
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Point;
            light.range = 8f;
            light.intensity = 0.65f;
            light.color = new Color(1f, 0.56f, 0.2f, 1f);

            var particleObject = new GameObject("Property Particles", typeof(ParticleSystem));
            particleObject.transform.SetParent(worldRoot.transform, false);
            particleObject.transform.localPosition = new Vector3(0f, -1.3f, -0.5f);
            ParticleSystem particles = particleObject.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.startLifetime = 1.3f;
            main.startSpeed = 1.25f;
            main.startSize = 0.18f;
            main.startColor = new Color(0.15f, 0.82f, 1f, 0.9f);
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTimeMultiplier = 8f;
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 18f;
            shape.radius = 0.35f;

            root.SetActive(false);
            worldRoot.SetActive(false);
            return new EnginePropertyPreview(root, worldRoot, audioSource, light, particles, renderer, meter, valueText);
        }

        private static GameObject CreateSequencePanel(string name, Transform parent, string labelValue, Vector2 anchoredPosition, Vector2 size, Color color, float fontSize)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            panel.transform.SetParent(parent, false);
            var rect = (RectTransform)panel.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            panel.GetComponent<Image>().color = color;

            if (!string.IsNullOrEmpty(labelValue))
            {
                TMP_Text label = CreateText("Label", panel.transform, labelValue, fontSize, FontStyles.Bold, TextAlignmentOptions.Center);
                label.color = Color.white;
                SetRect(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(18f, 12f), new Vector2(-18f, -12f));
            }

            return panel;
        }

        private static GameObject CreateWorldDestinationObject(string name, PrimitiveType primitiveType, Transform parent, Vector3 position, float scale)
        {
            GameObject target = GameObject.CreatePrimitive(primitiveType);
            target.name = name;
            target.transform.SetParent(parent, false);
            target.transform.localPosition = position;
            target.transform.localScale = Vector3.one * scale;
            target.GetComponent<Renderer>().sharedMaterial = GetOrCreatePreviewMaterial();
            Object.DestroyImmediate(target.GetComponent<Collider>());
            return target;
        }

        private static GameObject CreateWorldDestinationMarker(string name, Transform parent, Vector3 position, float scale)
        {
            var marker = new GameObject(name);
            marker.transform.SetParent(parent, false);
            marker.transform.localPosition = position;
            CreateWorldDestinationObject("Marker Visual", PrimitiveType.Sphere, marker.transform, Vector3.down * 0.62f, scale);
            return marker;
        }

        private static RectTransform CreateUiDestinationMarker(string name, Transform parent, Vector2 position, string labelValue, Color color)
        {
            var marker = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            marker.transform.SetParent(parent, false);
            var rect = (RectTransform)marker.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = Vector2.one * 124f;
            marker.GetComponent<Image>().color = color;

            TMP_Text label = CreateText("Label", marker.transform, labelValue, 15f, FontStyles.Bold, TextAlignmentOptions.Center);
            label.color = new Color(0.82f, 0.9f, 1f, 0.9f);
            SetRect(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(-30f, -34f), new Vector2(30f, -8f));
            return rect;
        }

        private static GameObject CreateUiDestinationTarget(Transform parent, Vector2 position)
        {
            var target = new GameObject("Destination UI Target", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            target.transform.SetParent(parent, false);
            var rect = (RectTransform)target.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = Vector2.one * 96f;
            target.GetComponent<Image>().color = BlueColor;
            TMP_Text label = CreateText("Label", target.transform, "MOVE", 18f, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return target;
        }

        private static GameObject CreatePreviewGroup(string name, Transform parent)
        {
            var group = new GameObject(name, typeof(RectTransform));
            group.transform.SetParent(parent, false);
            SetRect((RectTransform)group.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return group;
        }

        private static GameObject CreateCollectionItem(string name, Transform parent, string labelValue, Vector2 anchoredPosition, Vector2 size)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            item.transform.SetParent(parent, false);
            var rect = (RectTransform)item.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            item.GetComponent<Image>().color = BlueColor;

            TMP_Text label = CreateText("Label", item.transform, labelValue, 25f, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return item;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return (RectTransform)panel.transform;
        }

        private static TMP_Text CreateText(string name, Transform parent, string value, float size, FontStyles style, TextAlignmentOptions alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = Color.white;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Color color)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.GetComponent<Image>();
            image.color = color;
            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.75f, 0.8f, 0.9f);
            colors.disabledColor = new Color(0.15f, 0.17f, 0.22f, 0.6f);
            colors.colorMultiplier = 1.2f;
            button.colors = colors;

            TMP_Text text = CreateText("Label", buttonObject.transform, label, 22, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(14f, 8f), new Vector2(-14f, -8f));
            return button;
        }

        private static Toggle CreateToggle(string name, Transform parent, string label, ToggleGroup group)
        {
            var toggleObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Toggle));
            toggleObject.transform.SetParent(parent, false);
            var background = toggleObject.GetComponent<Image>();
            background.color = new Color(0.09f, 0.15f, 0.25f);
            var toggle = toggleObject.GetComponent<Toggle>();
            toggle.group = group;
            toggle.targetGraphic = background;

            var indicatorObject = new GameObject("Selected", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            indicatorObject.transform.SetParent(toggleObject.transform, false);
            var indicator = indicatorObject.GetComponent<Image>();
            indicator.color = BlueColor;
            indicator.raycastTarget = false;
            SetRect((RectTransform)indicatorObject.transform, new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero, new Vector2(7f, 0f));
            toggle.graphic = indicator;

            TMP_Text text = CreateText("Label", toggleObject.transform, label, 17, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(10f, 5f), new Vector2(-10f, -5f));
            return toggle;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void Assign(SerializedObject serializedObject, string propertyName, Object value)
        {
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
        }

        private static void AssignArray(SerializedObject serializedObject, string propertyName, GameObject[] values)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }

        private readonly struct CollectionPreview
        {
            public readonly GameObject Root;
            public readonly GameObject ListGroup;
            public readonly GameObject GridGroup;
            public readonly GameObject IncompleteGridGroup;
            public readonly GameObject WorldRoot;
            public readonly GameObject LoadingDotsGroup;
            public readonly GameObject[] ListTargets;
            public readonly GameObject[] GridTargets;
            public readonly GameObject[] IncompleteGridTargets;
            public readonly GameObject[] WorldTargets;
            public readonly GameObject[] LoadingDotTargets;

            public CollectionPreview(GameObject root, GameObject listGroup, GameObject gridGroup, GameObject incompleteGridGroup, GameObject worldRoot, GameObject loadingDotsGroup, GameObject[] listTargets, GameObject[] gridTargets, GameObject[] incompleteGridTargets, GameObject[] worldTargets, GameObject[] loadingDotTargets)
            {
                Root = root;
                ListGroup = listGroup;
                GridGroup = gridGroup;
                IncompleteGridGroup = incompleteGridGroup;
                WorldRoot = worldRoot;
                LoadingDotsGroup = loadingDotsGroup;
                ListTargets = listTargets;
                GridTargets = gridTargets;
                IncompleteGridTargets = incompleteGridTargets;
                WorldTargets = worldTargets;
                LoadingDotTargets = loadingDotTargets;
            }
        }

        private readonly struct DestinationPreview
        {
            public readonly GameObject WorldRoot;
            public readonly GameObject WorldTarget;
            public readonly Transform WorldStartMarker;
            public readonly Transform WorldEndMarker;
            public readonly GameObject WorldCurvedPath;
            public readonly GameObject UiRoot;
            public readonly GameObject UiTarget;
            public readonly RectTransform UiStartMarker;
            public readonly RectTransform UiEndMarker;
            public readonly GameObject UiCurvedPath;

            public DestinationPreview(GameObject worldRoot, GameObject worldTarget, Transform worldStartMarker, Transform worldEndMarker, GameObject worldCurvedPath, GameObject uiRoot, GameObject uiTarget, RectTransform uiStartMarker, RectTransform uiEndMarker, GameObject uiCurvedPath)
            {
                WorldRoot = worldRoot;
                WorldTarget = worldTarget;
                WorldStartMarker = worldStartMarker;
                WorldEndMarker = worldEndMarker;
                WorldCurvedPath = worldCurvedPath;
                UiRoot = uiRoot;
                UiTarget = uiTarget;
                UiStartMarker = uiStartMarker;
                UiEndMarker = uiEndMarker;
                UiCurvedPath = uiCurvedPath;
            }
        }

        private readonly struct UISequencePreview
        {
            public readonly GameObject Root;
            public readonly GameObject Toast;
            public readonly GameObject ModalGroup;
            public readonly GameObject ModalBackdrop;
            public readonly GameObject ModalPanel;
            public readonly GameObject[] ModalControls;
            public readonly GameObject Tooltip;
            public readonly GameObject DropdownPanel;
            public readonly GameObject[] DropdownEntries;
            public readonly GameObject TabGroup;
            public readonly GameObject TabOutgoing;
            public readonly GameObject TabIncoming;
            public readonly GameObject DrawerBackdrop;

            public UISequencePreview(GameObject root, GameObject toast, GameObject modalGroup, GameObject modalBackdrop, GameObject modalPanel, GameObject[] modalControls, GameObject tooltip, GameObject dropdownPanel, GameObject[] dropdownEntries, GameObject tabGroup, GameObject tabOutgoing, GameObject tabIncoming, GameObject drawerBackdrop)
            {
                Root = root;
                Toast = toast;
                ModalGroup = modalGroup;
                ModalBackdrop = modalBackdrop;
                ModalPanel = modalPanel;
                ModalControls = modalControls;
                Tooltip = tooltip;
                DropdownPanel = dropdownPanel;
                DropdownEntries = dropdownEntries;
                TabGroup = tabGroup;
                TabOutgoing = tabOutgoing;
                TabIncoming = tabIncoming;
                DrawerBackdrop = drawerBackdrop;
            }
        }

        private readonly struct TextValuePreview
        {
            public readonly GameObject Root;
            public readonly TMP_Text Typewriter;
            public readonly TMP_Text Number;
            public readonly TMP_Text Character;
            public readonly TMP_Text Score;
            public readonly GameObject WorldRoot;
            public readonly TMP_Text WorldCharacter;

            public TextValuePreview(GameObject root, TMP_Text typewriter, TMP_Text number, TMP_Text character, TMP_Text score, GameObject worldRoot, TMP_Text worldCharacter)
            {
                Root = root;
                Typewriter = typewriter;
                Number = number;
                Character = character;
                Score = score;
                WorldRoot = worldRoot;
                WorldCharacter = worldCharacter;
            }
        }

        private readonly struct ProgressPreview
        {
            public readonly GameObject Root;
            public readonly GameObject ImageGroup;
            public readonly Image Image;
            public readonly Slider Slider;
            public readonly TMP_Text ValueText;
            public readonly TMP_Text EventText;

            public ProgressPreview(GameObject root, GameObject imageGroup, Image image, Slider slider, TMP_Text valueText, TMP_Text eventText)
            {
                Root = root;
                ImageGroup = imageGroup;
                Image = image;
                Slider = slider;
                ValueText = valueText;
                EventText = eventText;
            }
        }

        private readonly struct EnginePropertyPreview
        {
            public readonly GameObject Root;
            public readonly GameObject WorldRoot;
            public readonly AudioSource AudioSource;
            public readonly Light Light;
            public readonly ParticleSystem Particles;
            public readonly Renderer Renderer;
            public readonly Image Meter;
            public readonly TMP_Text ValueText;

            public EnginePropertyPreview(GameObject root, GameObject worldRoot, AudioSource audioSource, Light light, ParticleSystem particles, Renderer renderer, Image meter, TMP_Text valueText)
            {
                Root = root;
                WorldRoot = worldRoot;
                AudioSource = audioSource;
                Light = light;
                Particles = particles;
                Renderer = renderer;
                Meter = meter;
                ValueText = valueText;
            }
        }
    }
}
