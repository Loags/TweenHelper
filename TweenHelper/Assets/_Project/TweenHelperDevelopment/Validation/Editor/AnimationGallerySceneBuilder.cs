using System.Collections.Generic;
using LB.TweenHelper.Demo;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    public static class AnimationGallerySceneBuilder
    {
        private const string SampleRoot = "Assets/Loags/TweenHelper/Samples/TweenHelper Demos";
        private const string ScenePath = SampleRoot + "/Scenes/TweenHelperAnimationGallery.unity";
        private const string PrefabFolder = SampleRoot + "/Prefabs/UI/Gallery";
        private const string ListItemPath = PrefabFolder + "/AnimationGalleryListItem.prefab";
        private const string MaterialPath = SampleRoot + "/Materials/TweenHelperGalleryUnlit.mat";
        private const string WorldRenderTexturePath = SampleRoot + "/Materials/TweenHelperGalleryWorld.renderTexture";
        private const string CameraRenderTexturePath = SampleRoot + "/Materials/TweenHelperGalleryCamera.renderTexture";

        private static readonly Color Background = new Color(0.035f, 0.047f, 0.075f, 1f);
        private static readonly Color Panel = new Color(0.075f, 0.098f, 0.145f, 0.98f);
        private static readonly Color PanelLight = new Color(0.11f, 0.145f, 0.21f, 1f);
        private static readonly Color Accent = new Color(0.12f, 0.78f, 1f, 1f);
        private static readonly Color TextPrimary = new Color(0.94f, 0.97f, 1f, 1f);
        private static readonly Color TextSecondary = new Color(0.62f, 0.7f, 0.82f, 1f);

        [MenuItem("Tween Helper/Development/Build Animation Gallery")]
        public static void Build()
        {
            EnsureFolder(PrefabFolder);
            GameObject listItemPrefab = BuildListItemPrefab();
            Material material = GetOrCreateMaterial();
            RenderTexture worldTexture = GetOrCreateRenderTexture(WorldRenderTexturePath);
            RenderTexture cameraTexture = GetOrCreateRenderTexture(CameraRenderTexturePath);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TweenHelperAnimationGallery";
            GameObject root = new GameObject("Tween Helper Animation Gallery");
            CreateEventSystem(root.transform);
            Camera presentationCamera = BuildPresentationCamera(root.transform);

            GameObject worldRig = BuildWorldPreviewRig(root.transform, worldTexture, material, out GameObject worldTarget);
            GameObject cameraRig = BuildCameraPreviewRig(root.transform, cameraTexture, material, out Camera feedbackCamera, out Transform focusTarget);
            BuildInterface(root.transform, listItemPrefab, worldTexture, cameraTexture, worldRig, worldTarget, cameraRig, feedbackCamera, focusTarget, presentationCamera);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            Debug.Log($"Tween Helper Animation Gallery rebuilt at {ScenePath}");
        }

        private static void BuildInterface(Transform parent, GameObject listItemPrefab, RenderTexture worldTexture,
            RenderTexture cameraTexture, GameObject worldRig, GameObject worldTarget, GameObject cameraRig, Camera feedbackCamera,
            Transform focusTarget, Camera presentationCamera)
        {
            GameObject canvasObject = new GameObject("Gallery Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = presentationCamera;
            canvas.planeDistance = 10f;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform background = CreatePanel("Background", canvasObject.transform, Background);
            Stretch(background, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            RectTransform header = CreatePanel("Header", background, Panel);
            Stretch(header, new Vector2(0f, 1f), Vector2.one, new Vector2(24f, -86f), new Vector2(-24f, -18f));
            TMP_Text title = CreateText("Title", header, "Tween Helper Animation Gallery", 31f, FontStyles.Bold, TextPrimary, TextAlignmentOptions.Left);
            Stretch(title.rectTransform, new Vector2(0f, 0f), new Vector2(0.6f, 1f), new Vector2(22f, 8f), new Vector2(-8f, -8f));
            TMP_Text subtitle = CreateText("Subtitle", header, "300 presets + semantic animation recipes", 17f, FontStyles.Normal, TextSecondary, TextAlignmentOptions.Right);
            Stretch(subtitle.rectTransform, new Vector2(0.58f, 0f), new Vector2(0.82f, 1f), new Vector2(4f, 8f), new Vector2(-8f, -8f));
            Button presentationButton = CreateButton("Presentation Mode", header, "Presentation mode", out TMP_Text presentationButtonText);
            Stretch((RectTransform)presentationButton.transform, new Vector2(0.83f, 0.18f), new Vector2(0.985f, 0.82f), Vector2.zero, Vector2.zero);

            RectTransform body = CreateRect("Body", background);
            Stretch(body, Vector2.zero, Vector2.one, new Vector2(24f, 104f), new Vector2(-24f, -104f));

            RectTransform navigationChrome = CreateRect("Navigation Chrome", body);
            Stretch(navigationChrome, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(620f, 0f));
            RectTransform categoriesPanel = CreatePanel("Categories", navigationChrome, Panel);
            Stretch(categoriesPanel, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(250f, 0f));
            TMP_Text categoriesHeading = CreateText("Heading", categoriesPanel, "CATEGORIES", 15f, FontStyles.Bold, TextSecondary, TextAlignmentOptions.Left);
            Stretch(categoriesHeading.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -44f), new Vector2(-12f, -12f));
            RectTransform categoryList = CreateRect("Category List", categoriesPanel);
            Stretch(categoryList, Vector2.zero, Vector2.one, new Vector2(10f, 14f), new Vector2(-10f, -52f));
            var categoryLayout = categoryList.gameObject.AddComponent<VerticalLayoutGroup>();
            categoryLayout.spacing = 8f;
            categoryLayout.childControlWidth = true;
            categoryLayout.childForceExpandWidth = true;
            categoryLayout.childControlHeight = true;
            categoryLayout.childForceExpandHeight = false;

            string[] categoryNames = { "Presets", "UI Recipes", "Collections", "Destination Motion", "Gameplay Feedback", "UI Sequences", "Text & Values", "Camera Feedback" };
            var categoryButtons = new Button[categoryNames.Length];
            var categoryCounts = new TMP_Text[categoryNames.Length];
            for (int i = 0; i < categoryNames.Length; i++)
            {
                categoryButtons[i] = CreateButton(categoryNames[i], categoryList, categoryNames[i], out TMP_Text label);
                categoryButtons[i].gameObject.AddComponent<LayoutElement>().preferredHeight = 58f;
                label.fontSize = 14f;
                label.textWrappingMode = TextWrappingModes.NoWrap;
                label.alignment = TextAlignmentOptions.MidlineLeft;
                label.rectTransform.offsetMin = new Vector2(14f, 0f);
                label.rectTransform.offsetMax = new Vector2(-48f, 0f);
                categoryCounts[i] = CreateText("Count", categoryButtons[i].transform, "0", 14f, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
                Stretch(categoryCounts[i].rectTransform, new Vector2(1f, 0f), Vector2.one, new Vector2(-44f, 6f), new Vector2(-6f, -6f));
            }

            RectTransform listPanel = CreatePanel("Animation List", navigationChrome, Panel);
            Stretch(listPanel, Vector2.zero, Vector2.one, new Vector2(262f, 0f), Vector2.zero);
            TMP_InputField searchInput = CreateInputField("Search", listPanel, "Search animations...");
            Stretch((RectTransform)searchInput.transform, new Vector2(0f, 1f), Vector2.one, new Vector2(12f, -58f), new Vector2(-12f, -12f));
            Dropdown familyDropdown = CreateDropdown("Family Filter", listPanel, new[] { "All families" });
            Stretch((RectTransform)familyDropdown.transform, new Vector2(0f, 1f), Vector2.one, new Vector2(12f, -108f), new Vector2(-12f, -66f));
            TMP_Text visibleCount = CreateText("Visible Count", listPanel, "0 shown", 13f, FontStyles.Normal, TextSecondary, TextAlignmentOptions.Left);
            Stretch(visibleCount.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(14f, -136f), new Vector2(-12f, -112f));
            RectTransform listContent = CreateScrollView("Entries", listPanel);
            Stretch((RectTransform)listContent.parent.parent, Vector2.zero, Vector2.one, new Vector2(10f, 10f), new Vector2(-10f, -142f));
            GameObject listItemTemplate = (GameObject)PrefabUtility.InstantiatePrefab(listItemPrefab, listContent);
            listItemTemplate.name = "Animation Gallery List Item Template";
            listItemTemplate.SetActive(false);

            RectTransform contentArea = CreateRect("Content Area", body);
            Stretch(contentArea, Vector2.zero, Vector2.one, new Vector2(644f, 0f), Vector2.zero);
            RectTransform previewStage = CreatePanel("Preview Stage", contentArea, new Color(0.045f, 0.062f, 0.1f, 1f));
            Stretch(previewStage, new Vector2(0f, 0.43f), Vector2.one, Vector2.zero, Vector2.zero);
            TMP_Text stageLabel = CreateText("Stage Label", previewStage, "LIVE PREVIEW", 13f, FontStyles.Bold, TextSecondary, TextAlignmentOptions.Left);
            Stretch(stageLabel.rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(18f, -38f), new Vector2(-18f, -10f));
            RectTransform fixturesRoot = CreateRect("Preview Fixtures", previewStage);
            Stretch(fixturesRoot, Vector2.zero, Vector2.one, new Vector2(18f, 18f), new Vector2(-18f, -48f));

            BuildFixtures(fixturesRoot, worldTexture, cameraTexture, out FixtureBindings fixtures);

            RectTransform detailsChrome = CreatePanel("Details Chrome", contentArea, Panel);
            Stretch(detailsChrome, new Vector2(0f, 0.205f), new Vector2(1f, 0.415f), Vector2.zero, Vector2.zero);
            TMP_Text categoryText = CreateText("Category", detailsChrome, "PRESETS", 13f, FontStyles.Bold, Accent, TextAlignmentOptions.Left);
            Stretch(categoryText.rectTransform, new Vector2(0f, 0.72f), new Vector2(0.62f, 1f), new Vector2(18f, 0f), new Vector2(-8f, -8f));
            TMP_Text nameText = CreateText("Animation Name", detailsChrome, "Animation", 26f, FontStyles.Bold, TextPrimary, TextAlignmentOptions.Left);
            Stretch(nameText.rectTransform, new Vector2(0f, 0.36f), new Vector2(0.62f, 0.78f), new Vector2(18f, 0f), new Vector2(-8f, 0f));
            TMP_Text descriptionText = CreateText("Description", detailsChrome, "Select an animation to inspect its behavior.", 16f, FontStyles.Normal, TextSecondary, TextAlignmentOptions.TopLeft);
            Stretch(descriptionText.rectTransform, Vector2.zero, new Vector2(0.62f, 0.42f), new Vector2(18f, 8f), new Vector2(-8f, 0f));
            TMP_Text apiKindText = CreateBadge("API Kind", detailsChrome, "PRESET");
            Stretch(apiKindText.transform.parent.GetComponent<RectTransform>(), new Vector2(0.63f, 0.72f), new Vector2(0.81f, 0.94f), Vector2.zero, Vector2.zero);
            TMP_Text targetBadgeText = CreateBadge("Target Badge", detailsChrome, "Compatible target");
            Stretch(targetBadgeText.transform.parent.GetComponent<RectTransform>(), new Vector2(0.82f, 0.72f), new Vector2(0.99f, 0.94f), Vector2.zero, Vector2.zero);
            AnimationGalleryOptionView optionA = CreateOptionView("Option A", detailsChrome);
            Stretch((RectTransform)optionA.transform, new Vector2(0.63f, 0.37f), new Vector2(0.99f, 0.68f), Vector2.zero, Vector2.zero);
            AnimationGalleryOptionView optionB = CreateOptionView("Option B", detailsChrome);
            Stretch((RectTransform)optionB.transform, new Vector2(0.63f, 0.04f), new Vector2(0.99f, 0.34f), Vector2.zero, Vector2.zero);

            RectTransform codePanel = CreatePanel("Code Panel", contentArea, new Color(0.025f, 0.034f, 0.055f, 1f));
            Stretch(codePanel, Vector2.zero, new Vector2(1f, 0.19f), Vector2.zero, Vector2.zero);
            TMP_Text codeLabel = CreateText("Code Label", codePanel, "C# EXAMPLE", 12f, FontStyles.Bold, TextSecondary, TextAlignmentOptions.Left);
            Stretch(codeLabel.rectTransform, new Vector2(0f, 1f), new Vector2(0.72f, 1f), new Vector2(16f, -34f), new Vector2(-8f, -8f));
            TMP_Text codeText = CreateText("Code", codePanel, "target.Tween().Preset<PopInPreset>().Play();", 16f, FontStyles.Normal, new Color(0.72f, 0.92f, 1f), TextAlignmentOptions.TopLeft);
            Stretch(codeText.rectTransform, Vector2.zero, new Vector2(0.8f, 0.78f), new Vector2(16f, 12f), new Vector2(-8f, 0f));
            codeText.textWrappingMode = TextWrappingModes.Normal;
            Button copyButton = CreateButton("Copy", codePanel, "Copy code", out TMP_Text copyButtonText);
            Stretch((RectTransform)copyButton.transform, new Vector2(0.82f, 0.18f), new Vector2(0.985f, 0.72f), Vector2.zero, Vector2.zero);
            TMP_Text copyStateText = CreateText("Copy State", codePanel, "Copied", 13f, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
            Stretch(copyStateText.rectTransform, new Vector2(0.82f, 0.72f), new Vector2(0.985f, 0.96f), Vector2.zero, Vector2.zero);
            copyStateText.gameObject.SetActive(false);
            AnimationGalleryCodePresenter codePresenter = codePanel.gameObject.AddComponent<AnimationGalleryCodePresenter>();
            SetObject(codePresenter, "codeText", codeText);
            SetObject(codePresenter, "copyStateText", copyStateText);

            RectTransform footer = CreatePanel("Playback", background, Panel);
            Stretch(footer, Vector2.zero, new Vector2(1f, 0f), new Vector2(24f, 18f), new Vector2(-24f, 88f));
            Button previousButton = CreateFooterButton("Previous", footer, "Previous", 0.23f);
            Button replayButton = CreateFooterButton("Replay", footer, "Replay", 0.39f);
            Button resetButton = CreateFooterButton("Reset", footer, "Reset", 0.55f);
            Button nextButton = CreateFooterButton("Next", footer, "Next", 0.71f);

            GameObject systems = new GameObject("Gallery Systems");
            systems.transform.SetParent(parent, false);
            AnimationGalleryPreviewRouter router = systems.AddComponent<AnimationGalleryPreviewRouter>();
            AnimationGalleryPlayer player = systems.AddComponent<AnimationGalleryPlayer>();
            AnimationGalleryController controller = systems.AddComponent<AnimationGalleryController>();

            BindRouter(router, fixtures, worldTarget);
            BindPlayer(player, router, fixtures, worldRig, worldTarget, cameraRig, feedbackCamera, focusTarget);
            BindController(controller, categoryButtons, categoryCounts, searchInput, familyDropdown, listContent, listItemTemplate,
                visibleCount, categoryText, nameText, descriptionText, apiKindText, targetBadgeText, optionA, optionB, codePresenter,
                player, previousButton, replayButton, resetButton, nextButton, copyButton, presentationButton, presentationButtonText,
                navigationChrome, detailsChrome, contentArea);
            SetLayerRecursively(canvasObject, LayerMask.NameToLayer("UI"));
        }

        private static void BuildFixtures(RectTransform parent, RenderTexture worldTexture, RenderTexture cameraTexture, out FixtureBindings bindings)
        {
            bindings = new FixtureBindings();
            bindings.UiTarget = CreatePanel("UI Preview Target", parent, Accent).gameObject;
            Center((RectTransform)bindings.UiTarget.transform, new Vector2(230f, 150f), Vector2.zero);
            AddCenteredLabel(bindings.UiTarget.transform, "UI TARGET", 21f);

            bindings.WorldTargetRoot = CreateRect("World Preview", parent).gameObject;
            Stretch((RectTransform)bindings.WorldTargetRoot.transform, Vector2.zero, Vector2.one, new Vector2(80f, 32f), new Vector2(-80f, -16f));
            bindings.WorldTargetRoot.AddComponent<RawImage>().texture = worldTexture;

            bindings.ListRoot = CreateFixtureRoot("List Preview", parent);
            bindings.ListTargets = BuildCards(bindings.ListRoot.transform, 6, 1, new Vector2(128f, 58f));
            bindings.GridRoot = CreateFixtureRoot("Grid Preview", parent);
            bindings.GridTargets = BuildCards(bindings.GridRoot.transform, 9, 3, new Vector2(100f, 72f));
            bindings.LoadingDotsRoot = CreateFixtureRoot("Loading Dots Preview", parent);
            bindings.LoadingDotTargets = BuildCards(bindings.LoadingDotsRoot.transform, 3, 3, new Vector2(62f, 62f));

            bindings.DestinationUiRoot = CreateFixtureRoot("Destination UI Preview", parent);
            BuildDestination(bindings.DestinationUiRoot.transform, false, out bindings.DestinationUiTarget, out RectTransform uiStart, out RectTransform uiEnd);
            bindings.DestinationUiStart = uiStart;
            bindings.DestinationUiEnd = uiEnd;
            bindings.DestinationWorldRoot = CreateFixtureRoot("Destination World Preview", parent);
            BuildDestination(bindings.DestinationWorldRoot.transform, true, out bindings.DestinationWorldTarget, out RectTransform worldStart, out RectTransform worldEnd);
            bindings.DestinationWorldStart = worldStart;
            bindings.DestinationWorldEnd = worldEnd;

            bindings.UISequenceRoot = CreateFixtureRoot("UI Sequence Preview", parent);
            BuildUISequenceFixture(bindings);
            bindings.TextValueRoot = CreateFixtureRoot("Text And Value Preview", parent);
            BuildTextFixture(bindings.TextValueRoot.transform, out bindings.TypewriterText, out bindings.NumberText, out bindings.CharacterText, out bindings.ScoreText);
            bindings.WorldTextValueRoot = CreateFixtureRoot("World Text Preview", parent);
            bindings.WorldCharacterText = CreateText("World Character Text", bindings.WorldTextValueRoot.transform, "WORLD TEXT", 42f, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
            Stretch(bindings.WorldCharacterText.rectTransform, Vector2.zero, Vector2.one, new Vector2(40f, 40f), new Vector2(-40f, -40f));

            bindings.CameraRoot = CreateRect("Camera Feedback Preview", parent).gameObject;
            Stretch((RectTransform)bindings.CameraRoot.transform, Vector2.zero, Vector2.one, new Vector2(80f, 32f), new Vector2(-80f, -16f));
            bindings.CameraRoot.AddComponent<RawImage>().texture = cameraTexture;

            bindings.HideAll();
        }

        private static void BuildUISequenceFixture(FixtureBindings bindings)
        {
            Transform parent = bindings.UISequenceRoot.transform;
            bindings.ToastTarget = CreatePanel("Toast", parent, Accent).gameObject;
            Place((RectTransform)bindings.ToastTarget.transform, new Vector2(0f, 155f), new Vector2(360f, 70f));
            AddCenteredLabel(bindings.ToastTarget.transform, "Saved successfully", 18f);
            bindings.TooltipTarget = CreatePanel("Tooltip", parent, PanelLight).gameObject;
            Place((RectTransform)bindings.TooltipTarget.transform, new Vector2(-340f, 0f), new Vector2(260f, 90f));
            AddCenteredLabel(bindings.TooltipTarget.transform, "Helpful tooltip", 17f);
            bindings.ModalBackdrop = CreatePanel("Modal Backdrop", parent, new Color(0f, 0f, 0f, 0.48f)).gameObject;
            Place((RectTransform)bindings.ModalBackdrop.transform, new Vector2(0f, -5f), new Vector2(520f, 270f));
            bindings.ModalPanel = CreatePanel("Modal Panel", bindings.ModalBackdrop.transform, PanelLight).gameObject;
            Center((RectTransform)bindings.ModalPanel.transform, new Vector2(390f, 210f), Vector2.zero);
            AddCenteredLabel(bindings.ModalPanel.transform, "MODAL", 23f);
            bindings.ModalControls = BuildCards(bindings.ModalPanel.transform, 3, 3, new Vector2(92f, 42f));
            bindings.DropdownPanel = CreatePanel("Dropdown Panel", parent, PanelLight).gameObject;
            Place((RectTransform)bindings.DropdownPanel.transform, new Vector2(350f, -35f), new Vector2(260f, 260f));
            bindings.DropdownEntries = BuildCards(bindings.DropdownPanel.transform, 3, 1, new Vector2(210f, 46f));
            bindings.DrawerBackdrop = CreatePanel("Drawer Backdrop", parent, new Color(0f, 0f, 0f, 0.35f)).gameObject;
            Stretch((RectTransform)bindings.DrawerBackdrop.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            bindings.DrawerBackdrop.transform.SetAsFirstSibling();
            bindings.TabOutgoing = CreatePanel("Outgoing Page", parent, new Color(0.16f, 0.35f, 0.58f, 1f)).gameObject;
            Place((RectTransform)bindings.TabOutgoing.transform, new Vector2(-210f, -160f), new Vector2(300f, 90f));
            AddCenteredLabel(bindings.TabOutgoing.transform, "OUTGOING", 17f);
            bindings.TabIncoming = CreatePanel("Incoming Page", parent, new Color(0.18f, 0.64f, 0.46f, 1f)).gameObject;
            Place((RectTransform)bindings.TabIncoming.transform, new Vector2(210f, -160f), new Vector2(300f, 90f));
            AddCenteredLabel(bindings.TabIncoming.transform, "INCOMING", 17f);
        }

        private static void BuildTextFixture(Transform parent, out TMP_Text typewriter, out TMP_Text number, out TMP_Text characters, out TMP_Text score)
        {
            typewriter = CreateText("Typewriter Text", parent, "Tween Helper makes expressive motion readable.", 27f, FontStyles.Normal, TextPrimary, TextAlignmentOptions.Center);
            Place(typewriter.rectTransform, new Vector2(0f, 135f), new Vector2(850f, 80f));
            number = CreateText("Number Text", parent, "1,250", 52f, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
            Place(number.rectTransform, new Vector2(-260f, 10f), new Vector2(300f, 90f));
            characters = CreateText("Character Text", parent, "CHARACTER MOTION", 42f, FontStyles.Bold, TextPrimary, TextAlignmentOptions.Center);
            Place(characters.rectTransform, new Vector2(160f, 10f), new Vector2(560f, 90f));
            score = CreateText("Score Text", parent, "SCORE 1,200", 32f, FontStyles.Bold, new Color(1f, 0.73f, 0.2f), TextAlignmentOptions.Center);
            Place(score.rectTransform, new Vector2(0f, -120f), new Vector2(460f, 80f));
        }

        private static void BuildDestination(Transform parent, bool world, out GameObject target, out RectTransform start, out RectTransform end)
        {
            start = CreatePanel("Start Marker", parent, new Color(0.18f, 0.85f, 0.55f, 1f));
            Place(start, new Vector2(-330f, -105f), new Vector2(34f, 34f));
            end = CreatePanel("Destination Marker", parent, new Color(1f, 0.53f, 0.2f, 1f));
            Place(end, new Vector2(330f, 105f), new Vector2(34f, 34f));
            target = CreatePanel(world ? "Destination World Target" : "Destination UI Target", parent, Accent).gameObject;
            Place((RectTransform)target.transform, start.anchoredPosition, new Vector2(94f, 94f));
            AddCenteredLabel(target.transform, world ? "3D" : "UI", 18f);
        }

        private static Camera BuildPresentationCamera(Transform parent)
        {
            GameObject cameraObject = new GameObject("Gallery Presentation Camera", typeof(Camera));
            cameraObject.transform.SetParent(parent, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Background;
            camera.orthographic = true;
            camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            return camera;
        }

        private static GameObject BuildWorldPreviewRig(Transform parent, RenderTexture texture, Material material, out GameObject target)
        {
            GameObject rig = new GameObject("World Preset Preview Rig");
            rig.transform.SetParent(parent, false);
            rig.transform.position = new Vector3(100f, 0f, 0f);
            Camera camera = new GameObject("World Preview Camera").AddComponent<Camera>();
            camera.transform.SetParent(rig.transform, false);
            camera.transform.localPosition = new Vector3(0f, 0f, -10f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.04f, 0.07f);
            camera.targetTexture = texture;
            target = CreatePrimitive("World Preview Target", PrimitiveType.Cube, rig.transform, material);
            target.transform.localScale = new Vector3(2.2f, 2.2f, 0.6f);
            GameObject floor = CreatePrimitive("Reference Floor", PrimitiveType.Cube, rig.transform, material);
            floor.transform.localPosition = new Vector3(0f, -2.2f, 1f);
            floor.transform.localScale = new Vector3(8f, 0.18f, 3f);
            return rig;
        }

        private static GameObject BuildCameraPreviewRig(Transform parent, RenderTexture texture, Material material, out Camera camera, out Transform focusTarget)
        {
            GameObject rig = new GameObject("Camera Feedback Rig");
            rig.transform.SetParent(parent, false);
            rig.transform.position = new Vector3(200f, 0f, 0f);
            camera = new GameObject("Dedicated Feedback Camera").AddComponent<Camera>();
            camera.transform.SetParent(rig.transform, false);
            camera.transform.localPosition = new Vector3(0f, 0f, -10f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.04f, 0.07f);
            camera.targetTexture = texture;
            camera.fieldOfView = 48f;
            GameObject hero = CreatePrimitive("Camera Focus Target", PrimitiveType.Cube, rig.transform, material);
            hero.transform.localScale = new Vector3(2f, 2f, 0.7f);
            focusTarget = hero.transform;
            for (int i = -3; i <= 3; i++)
            {
                GameObject marker = CreatePrimitive($"Reference {i + 4}", PrimitiveType.Cube, rig.transform, material);
                marker.transform.localPosition = new Vector3(i * 1.8f, -2f, 1.5f + Mathf.Abs(i) * 0.25f);
                marker.transform.localScale = new Vector3(1.1f, 0.2f, 1.1f);
            }
            return rig;
        }

        private static void BindRouter(AnimationGalleryPreviewRouter router, FixtureBindings bindings, GameObject worldTarget)
        {
            SetObject(router, "uiTarget", bindings.UiTarget);
            SetObject(router, "worldTargetRoot", bindings.WorldTargetRoot);
            SetObject(router, "worldTarget", worldTarget);
            SetObject(router, "listRoot", bindings.ListRoot);
            SetObject(router, "gridRoot", bindings.GridRoot);
            SetObject(router, "loadingDotsRoot", bindings.LoadingDotsRoot);
            SetObject(router, "destinationUiRoot", bindings.DestinationUiRoot);
            SetObject(router, "destinationWorldRoot", bindings.DestinationWorldRoot);
            SetObject(router, "uiSequenceRoot", bindings.UISequenceRoot);
            SetObject(router, "textValueRoot", bindings.TextValueRoot);
            SetObject(router, "worldTextValueRoot", bindings.WorldTextValueRoot);
            SetObject(router, "cameraRoot", bindings.CameraRoot);
        }

        private static void BindPlayer(AnimationGalleryPlayer player, AnimationGalleryPreviewRouter router, FixtureBindings bindings,
            GameObject worldRig, GameObject worldTarget, GameObject cameraRig, Camera feedbackCamera, Transform focusTarget)
        {
            SetObject(player, "previewRouter", router);
            SetObjectArray(player, "resetScopes", new Object[] { bindings.ParentRoot, worldRig, cameraRig });
            SetObject(player, "listOwner", bindings.ListRoot);
            SetObjectArray(player, "listTargets", bindings.ListTargets);
            SetObject(player, "gridOwner", bindings.GridRoot);
            SetObjectArray(player, "gridTargets", bindings.GridTargets);
            SetObject(player, "loadingDotsOwner", bindings.LoadingDotsRoot);
            SetObjectArray(player, "loadingDotTargets", bindings.LoadingDotTargets);
            SetObject(player, "destinationUiTarget", bindings.DestinationUiTarget);
            SetObject(player, "destinationUiStart", bindings.DestinationUiStart);
            SetObject(player, "destinationUiEnd", bindings.DestinationUiEnd);
            SetObject(player, "destinationWorldTarget", bindings.DestinationWorldTarget);
            SetObject(player, "destinationWorldStart", bindings.DestinationWorldStart);
            SetObject(player, "destinationWorldEnd", bindings.DestinationWorldEnd);
            SetObject(player, "toastTarget", bindings.ToastTarget);
            SetObject(player, "modalBackdrop", bindings.ModalBackdrop);
            SetObject(player, "modalPanel", bindings.ModalPanel);
            SetObjectArray(player, "modalControls", bindings.ModalControls);
            SetObject(player, "tooltipTarget", bindings.TooltipTarget);
            SetObject(player, "dropdownPanel", bindings.DropdownPanel);
            SetObjectArray(player, "dropdownEntries", bindings.DropdownEntries);
            SetObject(player, "tabOutgoing", bindings.TabOutgoing);
            SetObject(player, "tabIncoming", bindings.TabIncoming);
            SetObject(player, "drawerBackdrop", bindings.DrawerBackdrop);
            SetObject(player, "typewriterText", bindings.TypewriterText);
            SetObject(player, "numberText", bindings.NumberText);
            SetObject(player, "characterText", bindings.CharacterText);
            SetObject(player, "scoreText", bindings.ScoreText);
            SetObject(player, "worldCharacterText", bindings.WorldCharacterText);
            SetObject(player, "previewCamera", feedbackCamera);
            SetObject(player, "cameraFocusTarget", focusTarget);
        }

        private static void BindController(AnimationGalleryController controller, Button[] categoryButtons, TMP_Text[] categoryCounts,
            TMP_InputField searchInput, Dropdown familyDropdown, RectTransform listContent, GameObject listItemPrefab,
            TMP_Text visibleCount, TMP_Text categoryText, TMP_Text nameText, TMP_Text descriptionText, TMP_Text apiKindText,
            TMP_Text targetBadgeText, AnimationGalleryOptionView optionA, AnimationGalleryOptionView optionB,
            AnimationGalleryCodePresenter codePresenter, AnimationGalleryPlayer player, Button previousButton, Button replayButton,
            Button resetButton, Button nextButton, Button copyButton, Button presentationButton, TMP_Text presentationButtonText,
            RectTransform navigationChrome, RectTransform detailsChrome, RectTransform contentArea)
        {
            SetObjectArray(controller, "categoryButtons", categoryButtons);
            SetObjectArray(controller, "categoryCountTexts", categoryCounts);
            SetObject(controller, "searchInput", searchInput);
            SetObject(controller, "familyDropdown", familyDropdown);
            SetObject(controller, "listContent", listContent);
            SetObject(controller, "listItemPrefab", listItemPrefab);
            SetObject(controller, "visibleCountText", visibleCount);
            SetObject(controller, "categoryText", categoryText);
            SetObject(controller, "nameText", nameText);
            SetObject(controller, "descriptionText", descriptionText);
            SetObject(controller, "apiKindText", apiKindText);
            SetObject(controller, "targetBadgeText", targetBadgeText);
            SetObjectArray(controller, "optionViews", new Object[] { optionA, optionB });
            SetObject(controller, "codePresenter", codePresenter);
            SetObject(controller, "player", player);
            SetObject(controller, "previousButton", previousButton);
            SetObject(controller, "replayButton", replayButton);
            SetObject(controller, "resetButton", resetButton);
            SetObject(controller, "nextButton", nextButton);
            SetObject(controller, "copyButton", copyButton);
            SetObject(controller, "presentationModeButton", presentationButton);
            SetObject(controller, "presentationModeButtonText", presentationButtonText);
            SetObject(controller, "navigationChrome", navigationChrome.gameObject);
            SetObject(controller, "detailsChrome", detailsChrome.gameObject);
            SetObject(controller, "contentArea", contentArea);
        }

        private static GameObject BuildListItemPrefab()
        {
            GameObject root = new GameObject("AnimationGalleryListItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement), typeof(AnimationGalleryListItem));
            Image background = root.GetComponent<Image>();
            background.color = PanelLight;
            root.GetComponent<LayoutElement>().preferredHeight = 66f;
            TMP_Text name = CreateText("Name", root.transform, "Animation Name", 17f, FontStyles.Bold, TextPrimary, TextAlignmentOptions.Left);
            Stretch(name.rectTransform, Vector2.zero, new Vector2(1f, 0.62f), new Vector2(16f, 0f), new Vector2(-10f, 0f));
            TMP_Text secondary = CreateText("Secondary", root.transform, "Preset family", 12f, FontStyles.Normal, TextSecondary, TextAlignmentOptions.Left);
            Stretch(secondary.rectTransform, new Vector2(0f, 0.58f), Vector2.one, new Vector2(16f, 0f), new Vector2(-10f, 0f));
            Image indicator = CreatePanel("Selected", root.transform, Accent).GetComponent<Image>();
            Stretch(indicator.rectTransform, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(5f, 0f));
            indicator.enabled = false;
            AnimationGalleryListItem view = root.GetComponent<AnimationGalleryListItem>();
            SetObject(view, "nameText", name);
            SetObject(view, "secondaryText", secondary);
            SetObject(view, "selectedIndicator", indicator);
            SetObject(view, "button", root.GetComponent<Button>());
            PrefabUtility.SaveAsPrefabAsset(root, ListItemPath);
            Object.DestroyImmediate(root);
            AssetDatabase.ImportAsset(ListItemPath, ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<GameObject>(ListItemPath);
        }

        private static AnimationGalleryOptionView CreateOptionView(string name, Transform parent)
        {
            RectTransform root = CreateRect(name, parent);
            TMP_Text label = CreateText("Label", root, "Option", 13f, FontStyles.Bold, TextSecondary, TextAlignmentOptions.Left);
            Stretch(label.rectTransform, Vector2.zero, new Vector2(0.3f, 1f), new Vector2(0f, 0f), new Vector2(-8f, 0f));
            Dropdown dropdown = CreateDropdown("Values", root, new[] { "Default" });
            Stretch((RectTransform)dropdown.transform, new Vector2(0.31f, 0f), Vector2.one, Vector2.zero, Vector2.zero);
            AnimationGalleryOptionView view = root.gameObject.AddComponent<AnimationGalleryOptionView>();
            SetObject(view, "labelText", label);
            SetObject(view, "dropdown", dropdown);
            return view;
        }

        private static RectTransform CreateScrollView(string name, Transform parent)
        {
            RectTransform root = CreatePanel(name, parent, new Color(0.03f, 0.045f, 0.075f, 1f));
            ScrollRect scroll = root.gameObject.AddComponent<ScrollRect>();
            RectTransform viewport = CreateRect("Viewport", root);
            Stretch(viewport, Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -4f));
            viewport.gameObject.AddComponent<Image>().color = Color.white;
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            RectTransform content = CreateRect("Content", viewport);
            Stretch(content, new Vector2(0f, 1f), Vector2.one, Vector2.zero, Vector2.zero);
            content.pivot = new Vector2(0.5f, 1f);
            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 7f;
            layout.padding = new RectOffset(4, 4, 4, 4);
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            return content;
        }

        private static TMP_InputField CreateInputField(string name, Transform parent, string placeholderValue)
        {
            RectTransform root = CreatePanel(name, parent, PanelLight);
            TMP_InputField input = root.gameObject.AddComponent<TMP_InputField>();
            RectTransform viewport = CreateRect("Text Area", root);
            Stretch(viewport, Vector2.zero, Vector2.one, new Vector2(12f, 5f), new Vector2(-12f, -5f));
            viewport.gameObject.AddComponent<RectMask2D>();
            TMP_Text text = CreateText("Text", viewport, string.Empty, 16f, FontStyles.Normal, TextPrimary, TextAlignmentOptions.Left);
            Stretch(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            TMP_Text placeholder = CreateText("Placeholder", viewport, placeholderValue, 16f, FontStyles.Italic, TextSecondary, TextAlignmentOptions.Left);
            Stretch(placeholder.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            input.textViewport = viewport;
            input.textComponent = text;
            input.placeholder = placeholder;
            return input;
        }

        private static Dropdown CreateDropdown(string name, Transform parent, IReadOnlyList<string> options)
        {
            RectTransform root = CreatePanel(name, parent, PanelLight);
            Dropdown dropdown = root.gameObject.AddComponent<Dropdown>();
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            Text label = CreateLegacyText("Label", root, options[0], font, 15, TextAnchor.MiddleLeft, TextPrimary);
            Stretch(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(12f, 0f), new Vector2(-34f, 0f));
            Text arrow = CreateLegacyText("Arrow", root, "v", font, 17, TextAnchor.MiddleCenter, Accent);
            Stretch(arrow.rectTransform, new Vector2(1f, 0f), Vector2.one, new Vector2(-32f, 0f), Vector2.zero);
            RectTransform template = CreatePanel("Template", root, Panel).GetComponent<RectTransform>();
            Stretch(template, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, -220f), Vector2.zero);
            ScrollRect scroll = template.gameObject.AddComponent<ScrollRect>();
            RectTransform viewport = CreateRect("Viewport", template);
            Stretch(viewport, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            viewport.gameObject.AddComponent<Image>().color = Color.white;
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            RectTransform content = CreateRect("Content", viewport);
            Stretch(content, new Vector2(0f, 1f), Vector2.one, Vector2.zero, Vector2.zero);
            content.pivot = new Vector2(0.5f, 1f);
            Toggle item = CreatePanel("Item", content, PanelLight).gameObject.AddComponent<Toggle>();
            RectTransform itemRect = (RectTransform)item.transform;
            itemRect.sizeDelta = new Vector2(0f, 34f);
            Image checkmark = CreatePanel("Item Checkmark", item.transform, Accent).GetComponent<Image>();
            Stretch(checkmark.rectTransform, Vector2.zero, new Vector2(0f, 1f), new Vector2(6f, 6f), new Vector2(12f, -6f));
            Text itemLabel = CreateLegacyText("Item Label", item.transform, "Option", font, 14, TextAnchor.MiddleLeft, TextPrimary);
            Stretch(itemLabel.rectTransform, Vector2.zero, Vector2.one, new Vector2(20f, 0f), new Vector2(-6f, 0f));
            item.targetGraphic = item.GetComponent<Image>();
            item.graphic = checkmark;
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            dropdown.targetGraphic = root.GetComponent<Image>();
            dropdown.template = template;
            dropdown.captionText = label;
            dropdown.itemText = itemLabel;
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(options));
            template.gameObject.SetActive(false);
            return dropdown;
        }

        private static Button CreateFooterButton(string name, Transform parent, string label, float centerX)
        {
            Button button = CreateButton(name, parent, label, out TMP_Text unused);
            Stretch((RectTransform)button.transform, new Vector2(centerX - 0.07f, 0.16f), new Vector2(centerX + 0.07f, 0.84f), Vector2.zero, Vector2.zero);
            return button;
        }

        private static Button CreateButton(string name, Transform parent, string label, out TMP_Text labelText)
        {
            RectTransform root = CreatePanel(name, parent, PanelLight);
            Button button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = root.GetComponent<Image>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.82f, 0.92f, 1f);
            colors.pressedColor = new Color(0.55f, 0.82f, 1f);
            colors.disabledColor = new Color(0.25f, 0.36f, 0.5f, 1f);
            button.colors = colors;
            labelText = CreateText("Label", root, label, 16f, FontStyles.Bold, TextPrimary, TextAlignmentOptions.Center);
            Stretch(labelText.rectTransform, Vector2.zero, Vector2.one, new Vector2(8f, 4f), new Vector2(-8f, -4f));
            return button;
        }

        private static TMP_Text CreateBadge(string name, Transform parent, string value)
        {
            RectTransform root = CreatePanel(name, parent, PanelLight);
            TMP_Text text = CreateText("Label", root, value, 12f, FontStyles.Bold, Accent, TextAlignmentOptions.Center);
            Stretch(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(5f, 2f), new Vector2(-5f, -2f));
            return text;
        }

        private static GameObject CreateFixtureRoot(string name, Transform parent)
        {
            RectTransform root = CreateRect(name, parent);
            Stretch(root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return root.gameObject;
        }

        private static GameObject[] BuildCards(Transform parent, int count, int columns, Vector2 cellSize)
        {
            var targets = new GameObject[count];
            float spacingX = cellSize.x + 18f;
            float spacingY = cellSize.y + 16f;
            int rows = Mathf.CeilToInt(count / (float)columns);
            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int column = i % columns;
                targets[i] = CreatePanel($"Item {i + 1}", parent, i % 2 == 0 ? Accent : new Color(0.34f, 0.52f, 0.95f, 1f)).gameObject;
                Vector2 position = new Vector2((column - (columns - 1) * 0.5f) * spacingX, ((rows - 1) * 0.5f - row) * spacingY);
                Place((RectTransform)targets[i].transform, position, cellSize);
                AddCenteredLabel(targets[i].transform, (i + 1).ToString(), 16f);
            }
            return targets;
        }

        private static void AddCenteredLabel(Transform parent, string value, float size)
        {
            TMP_Text label = CreateText("Label", parent, value, size, FontStyles.Bold, TextPrimary, TextAlignmentOptions.Center);
            Stretch(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        }

        private static GameObject CreatePrimitive(string name, PrimitiveType type, Transform parent, Material material)
        {
            GameObject primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            Object.DestroyImmediate(primitive.GetComponent<Collider>());
            primitive.GetComponent<Renderer>().sharedMaterial = material;
            return primitive;
        }

        private static void CreateEventSystem(Transform parent)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(parent, false);
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            RectTransform rect = CreateRect(name, parent);
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return rect;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<RectTransform>();
        }

        private static TMP_Text CreateText(string name, Transform parent, string value, float size, FontStyles style, Color color, TextAlignmentOptions alignment)
        {
            RectTransform rect = CreateRect(name, parent);
            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            return text;
        }

        private static Text CreateLegacyText(string name, Transform parent, string value, Font font, int size, TextAnchor alignment, Color color)
        {
            RectTransform rect = CreateRect(name, parent);
            Text text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void Center(RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        private static void Place(RectTransform rect, Vector2 position, Vector2 size) => Center(rect, size, position);

        private static void SetLayerRecursively(GameObject root, int layer)
        {
            root.layer = layer;
            foreach (Transform child in root.transform) SetLayerRecursively(child.gameObject, layer);
        }

        private static Material GetOrCreateMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material != null) return material;
            material = new Material(Shader.Find("Tween Helper/Gallery Unlit"));
            material.color = Accent;
            AssetDatabase.CreateAsset(material, MaterialPath);
            return material;
        }

        private static RenderTexture GetOrCreateRenderTexture(string path)
        {
            RenderTexture texture = AssetDatabase.LoadAssetAtPath<RenderTexture>(path);
            if (texture != null) return texture;
            texture = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32) { name = System.IO.Path.GetFileNameWithoutExtension(path) };
            AssetDatabase.CreateAsset(texture, path);
            return texture;
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = current + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, segments[i]);
                current = next;
            }
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray<T>(Object target, string propertyName, IReadOnlyList<T> values) where T : Object
        {
            var serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Count;
            for (int i = 0; i < values.Count; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private sealed class FixtureBindings
        {
            public GameObject UiTarget;
            public GameObject WorldTargetRoot;
            public GameObject ListRoot;
            public GameObject GridRoot;
            public GameObject LoadingDotsRoot;
            public GameObject DestinationUiRoot;
            public GameObject DestinationWorldRoot;
            public GameObject UISequenceRoot;
            public GameObject TextValueRoot;
            public GameObject WorldTextValueRoot;
            public GameObject CameraRoot;
            public GameObject[] ListTargets;
            public GameObject[] GridTargets;
            public GameObject[] LoadingDotTargets;
            public GameObject DestinationUiTarget;
            public RectTransform DestinationUiStart;
            public RectTransform DestinationUiEnd;
            public GameObject DestinationWorldTarget;
            public Transform DestinationWorldStart;
            public Transform DestinationWorldEnd;
            public GameObject ToastTarget;
            public GameObject ModalBackdrop;
            public GameObject ModalPanel;
            public GameObject[] ModalControls;
            public GameObject TooltipTarget;
            public GameObject DropdownPanel;
            public GameObject[] DropdownEntries;
            public GameObject TabOutgoing;
            public GameObject TabIncoming;
            public GameObject DrawerBackdrop;
            public TMP_Text TypewriterText;
            public TMP_Text NumberText;
            public TMP_Text CharacterText;
            public TMP_Text ScoreText;
            public TMP_Text WorldCharacterText;
            public GameObject ParentRoot => UiTarget.transform.parent.gameObject;

            public void HideAll()
            {
                UiTarget.SetActive(false);
                WorldTargetRoot.SetActive(false);
                ListRoot.SetActive(false);
                GridRoot.SetActive(false);
                LoadingDotsRoot.SetActive(false);
                DestinationUiRoot.SetActive(false);
                DestinationWorldRoot.SetActive(false);
                UISequenceRoot.SetActive(false);
                TextValueRoot.SetActive(false);
                WorldTextValueRoot.SetActive(false);
                CameraRoot.SetActive(false);
            }
        }
    }
}
