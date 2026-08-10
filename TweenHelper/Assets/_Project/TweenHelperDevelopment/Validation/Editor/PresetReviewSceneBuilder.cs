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
                bool sceneNeedsMaterial = !File.Exists(Path.GetFullPath(MaterialPath));
                if (!sceneNeedsFilters && !sceneNeedsCollectionPreview && !sceneNeedsDestinationPreview && !sceneNeedsMaterial)
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
            SetRect(position.rectTransform, new Vector2(0.43f, 0f), new Vector2(0.57f, 0.34f), Vector2.zero, Vector2.zero);

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
            Assign(serializedController, "loadingDotsPreviewGroup", collectionPreview.LoadingDotsGroup);
            AssignArray(serializedController, "listTargets", collectionPreview.ListTargets);
            AssignArray(serializedController, "gridTargets", collectionPreview.GridTargets);
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
            loadingDotsGroup.SetActive(false);
            root.SetActive(false);
            return new CollectionPreview(root, listGroup, gridGroup, loadingDotsGroup, listTargets, gridTargets, loadingDotTargets);
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
            public readonly GameObject LoadingDotsGroup;
            public readonly GameObject[] ListTargets;
            public readonly GameObject[] GridTargets;
            public readonly GameObject[] LoadingDotTargets;

            public CollectionPreview(GameObject root, GameObject listGroup, GameObject gridGroup, GameObject loadingDotsGroup, GameObject[] listTargets, GameObject[] gridTargets, GameObject[] loadingDotTargets)
            {
                Root = root;
                ListGroup = listGroup;
                GridGroup = gridGroup;
                LoadingDotsGroup = loadingDotsGroup;
                ListTargets = listTargets;
                GridTargets = gridTargets;
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
    }
}
