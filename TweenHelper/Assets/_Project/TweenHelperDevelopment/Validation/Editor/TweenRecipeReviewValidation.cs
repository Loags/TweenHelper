using LB.TweenHelper.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    public static class TweenRecipeReviewValidation
    {
        private const string SampleRoot = "Assets/Loags/TweenHelper/Samples/TweenHelper Demos";
        private const string RecipeRoot = SampleRoot + "/Recipes";
        private const string GalleryLibraryPath = SampleRoot + "/Resources/TweenRecipeGalleryLibrary.asset";

        private static readonly string[] SampleNames =
        {
            "PanelMoveFade",
            "IconScalePreset",
            "MultiBindingPopup",
            "DelayedNotification"
        };

        private static readonly AnimationGalleryOperation[] GalleryOperations =
        {
            AnimationGalleryOperation.RecipePanelMoveFade,
            AnimationGalleryOperation.RecipeIconScalePreset,
            AnimationGalleryOperation.RecipeMultiBindingPopup,
            AnimationGalleryOperation.RecipeDelayedNotification
        };

        private static readonly TweenRecipeOperation[] ExpandedOperations =
        {
            TweenRecipeOperation.MoveWorldTo,
            TweenRecipeOperation.MoveWorldBy,
            TweenRecipeOperation.RotateWorldTo,
            TweenRecipeOperation.RotateWorldBy,
            TweenRecipeOperation.ArcWorldTo,
            TweenRecipeOperation.HopWorldTo,
            TweenRecipeOperation.ErrorReject,
            TweenRecipeOperation.DamageHit,
            TweenRecipeOperation.SuccessConfirm,
            TweenRecipeOperation.RewardReveal,
            TweenRecipeOperation.PageCrossFadeTo,
            TweenRecipeOperation.TypewriterReveal,
            TweenRecipeOperation.NumberCountTo,
            TweenRecipeOperation.CollectionPreset,
            TweenRecipeOperation.CameraFieldOfViewTo,
            TweenRecipeOperation.LightIntensityTo,
            TweenRecipeOperation.AudioVolumeTo,
            TweenRecipeOperation.ParticleEmissionRateTo
        };

        [MenuItem("Tween Helper/Development/Validate Tween Recipes")]
        public static void Validate()
        {
            var failures = new List<string>();
            var samplePaths = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < SampleNames.Length; i++)
            {
                string path = $"{RecipeRoot}/{SampleNames[i]}.asset";
                samplePaths.Add(path);
                TweenRecipe recipe = AssetDatabase.LoadAssetAtPath<TweenRecipe>(path);
                if (recipe == null)
                {
                    failures.Add($"Missing sample recipe: {path}");
                    continue;
                }

                TweenRecipeValidationResult result = TweenRecipeValidator.Validate(recipe);
                if (!result.IsValid) failures.Add($"{SampleNames[i]}: {result.GetSummary()}");
                ValidateStableIds(recipe, failures);
                ValidateNoSceneReferences(path, failures);
            }

            for (int i = 0; i < ExpandedOperations.Length; i++)
            {
                TweenRecipeOperation operation = ExpandedOperations[i];
                if (!TweenRecipeOperationCatalog.Operations.Contains(operation) ||
                    TweenRecipeOperationCatalog.GetDisplayName(operation) == "Unsupported" ||
                    TweenRecipeOperationCatalog.GetCategory(operation) == "Unsupported")
                {
                    failures.Add($"Expanded operation metadata is incomplete: {operation}");
                }
            }

            IReadOnlyList<AnimationGalleryEntry> gallery = AnimationGalleryCatalog.Build();
            if (gallery.Count != 423) failures.Add($"Animation Gallery expected 423 entries but found {gallery.Count}.");
            for (int i = 0; i < GalleryOperations.Length; i++)
            {
                AnimationGalleryOperation operation = GalleryOperations[i];
                if (!gallery.Any(entry => entry.Operation == operation && entry.ApiKind == AnimationGalleryApiKind.Recipe))
                {
                    failures.Add($"Gallery recipe entry is missing: {operation}");
                }
            }

            TweenRecipeGalleryLibrary library = AssetDatabase.LoadAssetAtPath<TweenRecipeGalleryLibrary>(GalleryLibraryPath);
            if (library == null)
            {
                failures.Add($"Missing Gallery recipe library: {GalleryLibraryPath}");
            }
            else
            {
                for (int i = 0; i < GalleryOperations.Length; i++)
                {
                    TweenRecipe recipe = library.Resolve(GalleryOperations[i]);
                    if (recipe == null || !samplePaths.Contains(AssetDatabase.GetAssetPath(recipe)))
                    {
                        failures.Add($"Gallery recipe library is not wired for {GalleryOperations[i]}.");
                    }
                }
            }

            if (failures.Count > 0)
            {
                throw new InvalidOperationException("Tween Recipe review validation failed:\n- " + string.Join("\n- ", failures));
            }

            Debug.Log($"Tween Recipe review validation passed: {SampleNames.Length} samples, {GalleryOperations.Length} Gallery entries, {ExpandedOperations.Length} expanded operations, {TweenRecipeOperationCatalog.Operations.Count} total operations.");
        }

        private static void ValidateStableIds(TweenRecipe recipe, ICollection<string> failures)
        {
            var bindingIds = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition binding = recipe.Bindings[i];
                if (binding == null || string.IsNullOrWhiteSpace(binding.Id) || !bindingIds.Add(binding.Id))
                {
                    failures.Add($"{recipe.name} has a missing or duplicate binding ID.");
                }
            }

            var nodeIds = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < recipe.Nodes.Count; i++)
            {
                TweenRecipeNode node = recipe.Nodes[i];
                if (node == null || string.IsNullOrWhiteSpace(node.Id) || !nodeIds.Add(node.Id))
                {
                    failures.Add($"{recipe.name} has a missing or duplicate node ID.");
                }
            }
        }

        private static void ValidateNoSceneReferences(string recipePath, ICollection<string> failures)
        {
            string[] dependencies = AssetDatabase.GetDependencies(recipePath, true);
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (dependencies[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{recipePath} depends on scene {dependencies[i]}.");
                }
            }
        }
    }
}
