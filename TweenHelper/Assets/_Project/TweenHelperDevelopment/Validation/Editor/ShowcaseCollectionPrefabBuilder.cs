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

        [MenuItem("Tools/Tween Helper Dev/Update 2D Showcase Collections and Destinations")]
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
                SerializedProperty destinationRootProperty = serializedController.FindProperty("destinationPreviewRoot");
                SerializedProperty destinationTargetProperty = serializedController.FindProperty("destinationTarget");
                SerializedProperty destinationStartProperty = serializedController.FindProperty("destinationStartMarker");
                SerializedProperty destinationEndProperty = serializedController.FindProperty("destinationEndMarker");
                SerializedProperty destinationPathProperty = serializedController.FindProperty("destinationCurvedPath");
                bool collectionsConfigured = tabProperty.objectReferenceValue != null && rootProperty.objectReferenceValue != null && targetsProperty.arraySize >= 9;
                bool destinationsConfigured = destinationsTabProperty.objectReferenceValue != null && destinationRootProperty.objectReferenceValue != null && destinationTargetProperty.objectReferenceValue != null && destinationStartProperty.objectReferenceValue != null && destinationEndProperty.objectReferenceValue != null && destinationPathProperty.objectReferenceValue != null;
                bool alreadyConfigured = collectionsConfigured && destinationsConfigured;
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

                GameObject destinationRoot = destinationRootProperty.objectReferenceValue as GameObject;
                if (force && destinationRoot != null)
                {
                    Object.DestroyImmediate(destinationRoot);
                    destinationRoot = null;
                }

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

                serializedController.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(controller);
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"TweenHelper collection and destination showcase updated at {PrefabPath}");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
                _isUpdating = false;
            }
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
    }
}
