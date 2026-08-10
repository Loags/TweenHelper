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

        [MenuItem("Tools/Tween Helper Dev/Update 2D Showcase Collections")]
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
                bool alreadyConfigured = tabProperty.objectReferenceValue != null && rootProperty.objectReferenceValue != null && targetsProperty.arraySize >= 9;
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

                serializedController.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(controller);
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"TweenHelper collection showcase updated at {PrefabPath}");
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
    }
}
