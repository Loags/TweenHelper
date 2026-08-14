using System;
using System.Linq;
using LB.TweenHelper.Demo;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    public static class ShowcaseCollectionPrefabBuilder
    {
        private const string GalleryScenePath = "Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes/TweenHelperAnimationGallery.unity";
        private const string ListItemPath = "Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Prefabs/UI/Gallery/AnimationGalleryListItem.prefab";

        [MenuItem("Tools/Tween Helper Dev/Validate Animation Gallery Assets")]
        public static void ValidateGalleryAssets()
        {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GalleryScenePath);
            GameObject listItem = AssetDatabase.LoadAssetAtPath<GameObject>(ListItemPath);
            if (scene == null) throw new InvalidOperationException($"Missing gallery scene: {GalleryScenePath}");
            if (listItem == null || listItem.GetComponent<AnimationGalleryListItem>() == null)
            {
                throw new InvalidOperationException($"Missing or invalid gallery list-item prefab: {ListItemPath}");
            }

            var catalog = AnimationGalleryCatalog.Build();
            int presetCount = catalog.Count(entry => entry.Category == AnimationGalleryCategory.Presets);
            int categoryCount = catalog.Select(entry => entry.Category).Distinct().Count();
            if (presetCount != 300) throw new InvalidOperationException($"Expected 300 presets but found {presetCount}.");
            if (categoryCount != 8) throw new InvalidOperationException($"Expected 8 gallery categories but found {categoryCount}.");
            Debug.Log($"Animation Gallery assets validated: {catalog.Count} entries across {categoryCount} categories.");
        }
    }
}
