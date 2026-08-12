using LB.TweenHelper.Demo;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    public static class ShowcaseCollectionPrefabBuilder
    {
        private const string PrefabPath = "Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Prefabs/UI/2D/TweenHelper2DShowcase.prefab";
        private static bool _isUpdating;

        [InitializeOnLoadMethod]
        private static void UpgradePrefabIfNeeded()
        {
            EditorApplication.delayCall += () =>
            {
                if (Application.isPlaying || _isUpdating) return;
                UpgradePrefab(false);
            };
        }

        [MenuItem("Tools/Tween Helper Dev/Update 2D Showcase Feature Tabs")]
        public static void UpgradePrefabFromMenu() => UpgradePrefab(true);

        private static void UpgradePrefab(bool force)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
            if (root == null) return;

            _isUpdating = true;
            try
            {
                var controller = root.GetComponent<PresetShowcaseSpawner2D>();
                var serializedController = new SerializedObject(controller);
                SerializedProperty tabProperty = serializedController.FindProperty("collectionsTabButton");
                SerializedProperty rootProperty = serializedController.FindProperty("collectionPreviewRoot");
                SerializedProperty targetsProperty = serializedController.FindProperty("collectionTargets");
                SerializedProperty destinationsTabProperty = serializedController.FindProperty("destinationsTabButton");
                SerializedProperty feedbackTabProperty = serializedController.FindProperty("feedbackTabButton");
                SerializedProperty uiSequencesTabProperty = serializedController.FindProperty("uiSequencesTabButton");
                SerializedProperty textValuesTabProperty = serializedController.FindProperty("textValuesTabButton");
                SerializedProperty destinationRootProperty = serializedController.FindProperty("destinationPreviewRoot");
                SerializedProperty destinationTargetProperty = serializedController.FindProperty("destinationTarget");
                SerializedProperty destinationStartProperty = serializedController.FindProperty("destinationStartMarker");
                SerializedProperty destinationEndProperty = serializedController.FindProperty("destinationEndMarker");
                SerializedProperty destinationPathProperty = serializedController.FindProperty("destinationCurvedPath");
                SerializedProperty uiSequenceRootProperty = serializedController.FindProperty("uiSequencePreviewRoot");
                SerializedProperty toastSequenceProperty = serializedController.FindProperty("toastSequenceTarget");
                SerializedProperty modalSequenceGroupProperty = serializedController.FindProperty("modalSequenceGroup");
                SerializedProperty modalBackdropProperty = serializedController.FindProperty("modalSequenceBackdrop");
                SerializedProperty modalPanelProperty = serializedController.FindProperty("modalSequencePanel");
                SerializedProperty modalControlsProperty = serializedController.FindProperty("modalSequenceControls");
                SerializedProperty tooltipSequenceProperty = serializedController.FindProperty("tooltipSequenceTarget");
                SerializedProperty dropdownPanelProperty = serializedController.FindProperty("dropdownSequencePanel");
                SerializedProperty dropdownEntriesProperty = serializedController.FindProperty("dropdownSequenceEntries");
                SerializedProperty tabSequenceGroupProperty = serializedController.FindProperty("tabSequenceGroup");
                SerializedProperty tabOutgoingProperty = serializedController.FindProperty("tabSequenceOutgoing");
                SerializedProperty tabIncomingProperty = serializedController.FindProperty("tabSequenceIncoming");
                SerializedProperty textValueRootProperty = serializedController.FindProperty("textValuePreviewRoot");
                SerializedProperty typewriterTextProperty = serializedController.FindProperty("typewriterText");
                SerializedProperty numberTextProperty = serializedController.FindProperty("numberText");
                SerializedProperty characterTextProperty = serializedController.FindProperty("characterText");
                SerializedProperty scoreTextProperty = serializedController.FindProperty("scoreText");
                bool collectionsConfigured = tabProperty.objectReferenceValue != null && rootProperty.objectReferenceValue != null && targetsProperty.arraySize >= 9;
                bool destinationsConfigured = destinationsTabProperty.objectReferenceValue != null && destinationRootProperty.objectReferenceValue != null && destinationTargetProperty.objectReferenceValue != null && destinationStartProperty.objectReferenceValue != null && destinationEndProperty.objectReferenceValue != null && destinationPathProperty.objectReferenceValue != null;
                bool feedbackConfigured = feedbackTabProperty.objectReferenceValue != null;
                bool uiSequencesConfigured = uiSequencesTabProperty.objectReferenceValue != null && uiSequenceRootProperty.objectReferenceValue != null && toastSequenceProperty.objectReferenceValue != null && modalSequenceGroupProperty.objectReferenceValue != null && modalBackdropProperty.objectReferenceValue != null && modalPanelProperty.objectReferenceValue != null && modalControlsProperty.arraySize >= 3 && tooltipSequenceProperty.objectReferenceValue != null && dropdownPanelProperty.objectReferenceValue != null && dropdownEntriesProperty.arraySize >= 4 && tabSequenceGroupProperty.objectReferenceValue != null && tabOutgoingProperty.objectReferenceValue != null && tabIncomingProperty.objectReferenceValue != null;
                bool textValuesConfigured = textValuesTabProperty.objectReferenceValue != null && textValueRootProperty.objectReferenceValue != null && typewriterTextProperty.objectReferenceValue != null && numberTextProperty.objectReferenceValue != null && characterTextProperty.objectReferenceValue != null && scoreTextProperty.objectReferenceValue != null;
                bool alreadyConfigured = collectionsConfigured && destinationsConfigured && feedbackConfigured && uiSequencesConfigured && textValuesConfigured;
                if (alreadyConfigured && !force) return;

                Button collectionsTab = tabProperty.objectReferenceValue as Button;
                if (collectionsTab == null)
                {
                    var recipesTab = (Button)serializedController.FindProperty("recipesTabButton").objectReferenceValue;
                    GameObject tabObject = Object.Instantiate(recipesTab.gameObject, recipesTab.transform.parent);
                    tabObject.name = "CollectionsTab";
                    collectionsTab = tabObject.GetComponent<Button>();
                    var tabRect = (RectTransform)tabObject.transform;
                    tabRect.anchoredPosition = new Vector2(538f, -118f);
                    tabRect.sizeDelta = new Vector2(260f, 48f);
                    tabObject.GetComponentInChildren<TMP_Text>().text = "COLLECTIONS";
                    tabProperty.objectReferenceValue = collectionsTab;
                }

                GameObject collectionRoot = rootProperty.objectReferenceValue as GameObject;
                if (collectionRoot == null)
                {
                    var presetImage = (Image)serializedController.FindProperty("presetImage").objectReferenceValue;
                    var previewText = (TextMeshProUGUI)serializedController.FindProperty("animatedText").objectReferenceValue;
                    collectionRoot = new GameObject("CollectionPreview", typeof(RectTransform));
                    collectionRoot.transform.SetParent(presetImage.transform.parent, false);
                    var rootRect = (RectTransform)collectionRoot.transform;
                    rootRect.anchorMin = Vector2.zero;
                    rootRect.anchorMax = Vector2.one;
                    rootRect.offsetMin = Vector2.zero;
                    rootRect.offsetMax = Vector2.zero;

                    targetsProperty.arraySize = 9;
                    for (int i = 0; i < targetsProperty.arraySize; i++)
                    {
                        GameObject target = CreateTarget(collectionRoot.transform, previewText, i);
                        targetsProperty.GetArrayElementAtIndex(i).objectReferenceValue = target;
                    }

                    collectionRoot.SetActive(false);
                    rootProperty.objectReferenceValue = collectionRoot;
                }

                Button destinationsTab = destinationsTabProperty.objectReferenceValue as Button;
                if (destinationsTab == null)
                {
                    var recipesTab = (Button)serializedController.FindProperty("recipesTabButton").objectReferenceValue;
                    GameObject tabObject = Object.Instantiate(recipesTab.gameObject, recipesTab.transform.parent);
                    tabObject.name = "DestinationsTab";
                    destinationsTab = tabObject.GetComponent<Button>();
                    var tabRect = (RectTransform)tabObject.transform;
                    tabRect.anchoredPosition = new Vector2(810f, -118f);
                    tabRect.sizeDelta = new Vector2(260f, 48f);
                    tabObject.GetComponentInChildren<TMP_Text>().text = "DESTINATIONS";
                    destinationsTabProperty.objectReferenceValue = destinationsTab;
                }

                Button feedbackTab = feedbackTabProperty.objectReferenceValue as Button;
                if (feedbackTab == null)
                {
                    var recipesTab = (Button)serializedController.FindProperty("recipesTabButton").objectReferenceValue;
                    GameObject tabObject = Object.Instantiate(recipesTab.gameObject, recipesTab.transform.parent);
                    tabObject.name = "FeedbackTab";
                    feedbackTab = tabObject.GetComponent<Button>();
                    tabObject.GetComponentInChildren<TMP_Text>().text = "FEEDBACK";
                    feedbackTabProperty.objectReferenceValue = feedbackTab;
                }

                Button uiSequencesTab = uiSequencesTabProperty.objectReferenceValue as Button;
                if (uiSequencesTab == null)
                {
                    var recipesTab = (Button)serializedController.FindProperty("recipesTabButton").objectReferenceValue;
                    GameObject tabObject = Object.Instantiate(recipesTab.gameObject, recipesTab.transform.parent);
                    tabObject.name = "UISequencesTab";
                    uiSequencesTab = tabObject.GetComponent<Button>();
                    tabObject.GetComponentInChildren<TMP_Text>().text = "UI SEQUENCES";
                    uiSequencesTabProperty.objectReferenceValue = uiSequencesTab;
                }

                Button textValuesTab = textValuesTabProperty.objectReferenceValue as Button;
                if (textValuesTab == null)
                {
                    var recipesTab = (Button)serializedController.FindProperty("recipesTabButton").objectReferenceValue;
                    GameObject tabObject = Object.Instantiate(recipesTab.gameObject, recipesTab.transform.parent);
                    tabObject.name = "TextValuesTab";
                    textValuesTab = tabObject.GetComponent<Button>();
                    tabObject.GetComponentInChildren<TMP_Text>().text = "TEXT & VALUES";
                    textValuesTabProperty.objectReferenceValue = textValuesTab;
                }

                var recipesTabButton = (Button)serializedController.FindProperty("recipesTabButton").objectReferenceValue;
                var presetsTabButton = (Button)serializedController.FindProperty("presetsTabButton").objectReferenceValue;
                LayoutTab(recipesTabButton, 0);
                LayoutTab(presetsTabButton, 1);
                LayoutTab(collectionsTab, 2);
                LayoutTab(destinationsTab, 3);
                LayoutTab(feedbackTab, 4);
                LayoutTab(uiSequencesTab, 5);
                LayoutTab(textValuesTab, 6);

                GameObject destinationRoot = destinationRootProperty.objectReferenceValue as GameObject;
                if (destinationRoot == null)
                {
                    var presetImage = (Image)serializedController.FindProperty("presetImage").objectReferenceValue;
                    var previewText = (TextMeshProUGUI)serializedController.FindProperty("animatedText").objectReferenceValue;
                    destinationRoot = CreateDestinationPreview(presetImage.transform.parent, previewText, out GameObject destinationTarget, out RectTransform startMarker, out RectTransform endMarker, out GameObject curvedPath);
                    destinationRootProperty.objectReferenceValue = destinationRoot;
                    destinationTargetProperty.objectReferenceValue = destinationTarget;
                    destinationStartProperty.objectReferenceValue = startMarker;
                    destinationEndProperty.objectReferenceValue = endMarker;
                    destinationPathProperty.objectReferenceValue = curvedPath;
                }

                GameObject uiSequenceRoot = uiSequenceRootProperty.objectReferenceValue as GameObject;
                if (uiSequenceRoot == null)
                {
                    var presetImage = (Image)serializedController.FindProperty("presetImage").objectReferenceValue;
                    var previewText = (TextMeshProUGUI)serializedController.FindProperty("animatedText").objectReferenceValue;
                    UISequencePreview preview = CreateUISequencePreview(presetImage.transform.parent, previewText);
                    uiSequenceRootProperty.objectReferenceValue = preview.Root;
                    toastSequenceProperty.objectReferenceValue = preview.Toast;
                    modalSequenceGroupProperty.objectReferenceValue = preview.ModalGroup;
                    modalBackdropProperty.objectReferenceValue = preview.ModalBackdrop;
                    modalPanelProperty.objectReferenceValue = preview.ModalPanel;
                    AssignArray(modalControlsProperty, preview.ModalControls);
                    tooltipSequenceProperty.objectReferenceValue = preview.Tooltip;
                    dropdownPanelProperty.objectReferenceValue = preview.DropdownPanel;
                    AssignArray(dropdownEntriesProperty, preview.DropdownEntries);
                    tabSequenceGroupProperty.objectReferenceValue = preview.TabGroup;
                    tabOutgoingProperty.objectReferenceValue = preview.TabOutgoing;
                    tabIncomingProperty.objectReferenceValue = preview.TabIncoming;
                }

                GameObject textValueRoot = textValueRootProperty.objectReferenceValue as GameObject;
                if (textValueRoot == null)
                {
                    var presetImage = (Image)serializedController.FindProperty("presetImage").objectReferenceValue;
                    var previewText = (TextMeshProUGUI)serializedController.FindProperty("animatedText").objectReferenceValue;
                    TextValuePreview preview = CreateTextValuePreview(presetImage.transform.parent, previewText);
                    textValueRootProperty.objectReferenceValue = preview.Root;
                    typewriterTextProperty.objectReferenceValue = preview.Typewriter;
                    numberTextProperty.objectReferenceValue = preview.Number;
                    characterTextProperty.objectReferenceValue = preview.Character;
                    scoreTextProperty.objectReferenceValue = preview.Score;
                }

                serializedController.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(controller);
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"TweenHelper 2D showcase feature tabs updated at {PrefabPath}");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
                _isUpdating = false;
            }
        }

        private static void LayoutTab(Button tab, int index)
        {
            var rect = (RectTransform)tab.transform;
            rect.anchoredPosition = new Vector2(24f + index * 212f, -118f);
            rect.sizeDelta = new Vector2(200f, 48f);
        }

        private static GameObject CreateTarget(Transform parent, TextMeshProUGUI fontSource, int index)
        {
            var target = new GameObject($"Collection Item {index + 1}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            target.transform.SetParent(parent, false);
            var targetRect = (RectTransform)target.transform;
            targetRect.anchorMin = targetRect.anchorMax = new Vector2(0.5f, 0.5f);
            targetRect.sizeDelta = new Vector2(62f, 62f);
            target.GetComponent<Image>().color = new Color(0.25f, 0.68f, 1f, 1f);

            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(target.transform, false);
            var labelRect = (RectTransform)labelObject.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelObject.GetComponent<TextMeshProUGUI>();
            label.text = (index + 1).ToString();
            label.font = fontSource.font;
            label.fontSize = 22f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;
            return target;
        }

        private static GameObject CreateDestinationPreview(Transform parent, TextMeshProUGUI fontSource, out GameObject destinationTarget, out RectTransform startMarker, out RectTransform endMarker, out GameObject curvedPath)
        {
            var root = new GameObject("DestinationPreview", typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var rootRect = (RectTransform)root.transform;
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            Vector2 start = new Vector2(-230f, 62f);
            Vector2 end = new Vector2(230f, 62f);
            startMarker = CreateDestinationMarker(root.transform, fontSource, "Start Marker", start, "START", new Color(0.25f, 0.68f, 1f, 0.22f));
            endMarker = CreateDestinationMarker(root.transform, fontSource, "Destination Marker", end, "DESTINATION", new Color(1f, 0.72f, 0.2f, 0.28f));

            destinationTarget = new GameObject("Destination Target", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            destinationTarget.transform.SetParent(root.transform, false);
            var targetRect = (RectTransform)destinationTarget.transform;
            targetRect.anchorMin = targetRect.anchorMax = new Vector2(0.5f, 0.5f);
            targetRect.anchoredPosition = start;
            targetRect.sizeDelta = Vector2.one * 74f;
            destinationTarget.GetComponent<Image>().color = new Color(0.25f, 0.68f, 1f, 1f);
            CreateLabel(destinationTarget.transform, fontSource, "MOVE", 16f, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            curvedPath = new GameObject("Curved Path Reference", typeof(RectTransform));
            curvedPath.transform.SetParent(root.transform, false);
            var pathRect = (RectTransform)curvedPath.transform;
            pathRect.anchorMin = Vector2.zero;
            pathRect.anchorMax = Vector2.one;
            pathRect.offsetMin = Vector2.zero;
            pathRect.offsetMax = Vector2.zero;
            curvedPath.transform.SetAsFirstSibling();

            for (int i = 1; i < 11; i++)
            {
                float progress = i / 11f;
                Vector2 point = Vector2.Lerp(start, end, progress) + Vector2.up * (4f * 145f * progress * (1f - progress));
                var dot = new GameObject($"Path Point {i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                dot.transform.SetParent(curvedPath.transform, false);
                var dotRect = (RectTransform)dot.transform;
                dotRect.anchorMin = dotRect.anchorMax = new Vector2(0.5f, 0.5f);
                dotRect.anchoredPosition = point;
                dotRect.sizeDelta = Vector2.one * 8f;
                dot.GetComponent<Image>().color = new Color(0.32f, 0.76f, 1f, 0.65f);
            }

            root.SetActive(false);
            return root;
        }

        private static UISequencePreview CreateUISequencePreview(Transform parent, TextMeshProUGUI fontSource)
        {
            var root = new GameObject("UISequencePreview", typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var rootRect = (RectTransform)root.transform;
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            GameObject toast = CreateSequencePanel("Toast", root.transform, fontSource, "SAVED SUCCESSFULLY", new Vector2(0f, 72f), new Vector2(430f, 82f), new Color(0.08f, 0.55f, 0.82f, 1f), 21f);

            GameObject modalGroup = CreateSequenceGroup("Modal Preview", root.transform);
            GameObject modalBackdrop = CreateSequencePanel("Modal Backdrop", modalGroup.transform, fontSource, string.Empty, new Vector2(0f, 72f), new Vector2(700f, 370f), new Color(0.01f, 0.02f, 0.04f, 0.78f), 1f);
            GameObject modalPanel = CreateSequencePanel("Modal Panel", modalGroup.transform, fontSource, "CONFIRM ACTION", new Vector2(0f, 72f), new Vector2(450f, 250f), new Color(0.08f, 0.16f, 0.28f, 1f), 25f);
            var modalControls = new GameObject[3];
            string[] modalLabels = { "CANCEL", "DETAILS", "CONFIRM" };
            for (int i = 0; i < modalControls.Length; i++)
            {
                modalControls[i] = CreateSequencePanel($"Modal Control {i + 1}", modalPanel.transform, fontSource, modalLabels[i], new Vector2((i - 1) * 126f, -64f), new Vector2(112f, 52f), i == 2 ? new Color(0.1f, 0.58f, 0.95f, 1f) : new Color(0.14f, 0.24f, 0.38f, 1f), 14f);
            }

            GameObject tooltip = CreateSequencePanel("Tooltip", root.transform, fontSource, "Helpful context appears here", new Vector2(0f, 72f), new Vector2(370f, 82f), new Color(0.12f, 0.18f, 0.28f, 1f), 18f);

            GameObject dropdownPanel = CreateSequencePanel("Dropdown Panel", root.transform, fontSource, string.Empty, new Vector2(0f, 205f), new Vector2(360f, 280f), new Color(0.07f, 0.13f, 0.22f, 1f), 1f);
            ((RectTransform)dropdownPanel.transform).pivot = new Vector2(0.5f, 1f);
            var dropdownEntries = new GameObject[4];
            string[] dropdownLabels = { "NEW PROJECT", "OPEN PROJECT", "SETTINGS", "QUIT" };
            for (int i = 0; i < dropdownEntries.Length; i++)
            {
                dropdownEntries[i] = CreateSequencePanel($"Dropdown Entry {i + 1}", dropdownPanel.transform, fontSource, dropdownLabels[i], new Vector2(0f, 98f - i * 62f), new Vector2(318f, 48f), new Color(0.12f, 0.24f, 0.39f, 1f), 15f);
            }

            GameObject tabGroup = CreateSequenceGroup("Tab Switch Preview", root.transform);
            GameObject tabIncoming = CreateSequencePanel("Incoming Tab", tabGroup.transform, fontSource, "INVENTORY\n\n12 ITEMS READY", new Vector2(0f, 72f), new Vector2(520f, 250f), new Color(0.12f, 0.38f, 0.32f, 1f), 23f);
            GameObject tabOutgoing = CreateSequencePanel("Outgoing Tab", tabGroup.transform, fontSource, "CHARACTER\n\nLEVEL 24", new Vector2(0f, 72f), new Vector2(520f, 250f), new Color(0.1f, 0.3f, 0.55f, 1f), 23f);

            toast.SetActive(false);
            modalGroup.SetActive(false);
            tooltip.SetActive(false);
            dropdownPanel.SetActive(false);
            tabGroup.SetActive(false);
            root.SetActive(false);
            return new UISequencePreview(root, toast, modalGroup, modalBackdrop, modalPanel, modalControls, tooltip, dropdownPanel, dropdownEntries, tabGroup, tabOutgoing, tabIncoming);
        }

        private static TextValuePreview CreateTextValuePreview(Transform parent, TextMeshProUGUI fontSource)
        {
            var root = new GameObject("TextValuePreview", typeof(RectTransform));
            root.transform.SetParent(parent, false);
            var rootRect = (RectTransform)root.transform;
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            TextMeshProUGUI typewriter = CreateTextValueLabel(root.transform, fontSource, "Typewriter Text", "<b>TWEEN HELPER</b>\n<color=#58BFFF>RICH TEXT READY</color>", 43f, Color.white);
            TextMeshProUGUI number = CreateTextValueLabel(root.transform, fontSource, "Number Text", "1,250", 72f, Color.white);
            TextMeshProUGUI character = CreateTextValueLabel(root.transform, fontSource, "Character Text", "CHARACTER MOTION\n<color=#58BFFF>MESH SAFE</color>", 44f, Color.white);
            TextMeshProUGUI score = CreateTextValueLabel(root.transform, fontSource, "Score Text", "1,200", 76f, new Color(1f, 0.86f, 0.42f, 1f));

            typewriter.gameObject.SetActive(false);
            number.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            score.gameObject.SetActive(false);
            root.SetActive(false);
            return new TextValuePreview(root, typewriter, number, character, score);
        }

        private static TextMeshProUGUI CreateTextValueLabel(Transform parent, TextMeshProUGUI fontSource, string name, string value, float fontSize, Color color)
        {
            var labelObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(parent, false);
            var rect = (RectTransform)labelObject.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(45f, 30f);
            rect.offsetMax = new Vector2(-45f, -30f);

            var label = labelObject.GetComponent<TextMeshProUGUI>();
            label.text = value;
            label.font = fontSource.font;
            label.fontSize = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color;
            label.textWrappingMode = TextWrappingModes.Normal;
            label.raycastTarget = false;
            return label;
        }

        private static GameObject CreateSequenceGroup(string name, Transform parent)
        {
            var group = new GameObject(name, typeof(RectTransform));
            group.transform.SetParent(parent, false);
            var rect = (RectTransform)group.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return group;
        }

        private static GameObject CreateSequencePanel(string name, Transform parent, TextMeshProUGUI fontSource, string labelValue, Vector2 anchoredPosition, Vector2 size, Color color, float fontSize)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            panel.transform.SetParent(parent, false);
            var rect = (RectTransform)panel.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            panel.GetComponent<Image>().color = color;
            if (!string.IsNullOrEmpty(labelValue)) CreateLabel(panel.transform, fontSource, labelValue, fontSize, Vector2.zero, Vector2.one, new Vector2(16f, 10f), new Vector2(-16f, -10f));
            return panel;
        }

        private static RectTransform CreateDestinationMarker(Transform parent, TextMeshProUGUI fontSource, string name, Vector2 position, string labelValue, Color color)
        {
            var marker = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            marker.transform.SetParent(parent, false);
            var rect = (RectTransform)marker.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = Vector2.one * 94f;
            marker.GetComponent<Image>().color = color;
            CreateLabel(marker.transform, fontSource, labelValue, 13f, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(-24f, -28f), new Vector2(24f, -6f));
            return rect;
        }

        private static void CreateLabel(Transform parent, TextMeshProUGUI fontSource, string value, float fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(parent, false);
            var labelRect = (RectTransform)labelObject.transform;
            labelRect.anchorMin = anchorMin;
            labelRect.anchorMax = anchorMax;
            labelRect.offsetMin = offsetMin;
            labelRect.offsetMax = offsetMax;

            var label = labelObject.GetComponent<TextMeshProUGUI>();
            label.text = value;
            label.font = fontSource.font;
            label.fontSize = fontSize;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;
        }

        private static void AssignArray(SerializedProperty property, GameObject[] values)
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
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

            public UISequencePreview(GameObject root, GameObject toast, GameObject modalGroup, GameObject modalBackdrop, GameObject modalPanel, GameObject[] modalControls, GameObject tooltip, GameObject dropdownPanel, GameObject[] dropdownEntries, GameObject tabGroup, GameObject tabOutgoing, GameObject tabIncoming)
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
            }
        }

        private readonly struct TextValuePreview
        {
            public readonly GameObject Root;
            public readonly TextMeshProUGUI Typewriter;
            public readonly TextMeshProUGUI Number;
            public readonly TextMeshProUGUI Character;
            public readonly TextMeshProUGUI Score;

            public TextValuePreview(GameObject root, TextMeshProUGUI typewriter, TextMeshProUGUI number, TextMeshProUGUI character, TextMeshProUGUI score)
            {
                Root = root;
                Typewriter = typewriter;
                Number = number;
                Character = character;
                Score = score;
            }
        }
    }
}
