using LB.TweenHelper.Demo;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    public static class PresetReviewSceneBuilder
    {
        private const string SceneFolder = "Assets/_Project/TweenHelperDevelopment/Validation/Scenes";
        private const string ScenePath = SceneFolder + "/TweenHelperPresetReview.unity";
        private static readonly Color BackgroundColor = new Color(0.025f, 0.04f, 0.08f);
        private static readonly Color PanelColor = new Color(0.055f, 0.08f, 0.14f, 0.94f);
        private static readonly Color BlueColor = new Color(0.1f, 0.58f, 0.95f);
        private static readonly Color MutedTextColor = new Color(0.68f, 0.74f, 0.84f);

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

            RectTransform infoPanel = CreatePanel("Animation Information", canvas.transform, PanelColor);
            SetRect(infoPanel, new Vector2(0.19f, 0.72f), new Vector2(0.81f, 0.93f), Vector2.zero, Vector2.zero);
            TMP_Text category = CreateText("Category", infoPanel, string.Empty, 17, FontStyles.Bold, TextAlignmentOptions.Center);
            category.color = BlueColor;
            SetRect(category.rectTransform, new Vector2(0f, 0.72f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            TMP_Text itemName = CreateText("Animation Name", infoPanel, string.Empty, 36, FontStyles.Bold, TextAlignmentOptions.Center);
            SetRect(itemName.rectTransform, new Vector2(0f, 0.35f), new Vector2(1f, 0.78f), new Vector2(20f, 0f), new Vector2(-20f, 0f));
            TMP_Text description = CreateText("Description", infoPanel, string.Empty, 19, FontStyles.Normal, TextAlignmentOptions.Center);
            description.color = MutedTextColor;
            SetRect(description.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.4f), new Vector2(35f, 0f), new Vector2(-35f, 0f));

            RectTransform previewFrame = CreatePanel("Preview", canvas.transform, new Color(0.03f, 0.055f, 0.1f, 0.35f));
            SetRect(previewFrame, new Vector2(0.2f, 0.24f), new Vector2(0.8f, 0.7f), Vector2.zero, Vector2.zero);
            GameObject uiTarget = CreateUiTarget(previewFrame);

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
            target.GetComponent<Renderer>().material.color = new Color(0.12f, 0.63f, 1f);
            return target;
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
            text.enableWordWrapping = true;
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

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
        }
    }
}
