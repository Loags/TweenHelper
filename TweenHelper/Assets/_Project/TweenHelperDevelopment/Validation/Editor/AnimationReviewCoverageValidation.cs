using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using LB.TweenHelper.Demo;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LB.TweenHelper.Editor
{
    [InitializeOnLoad]
    internal static class AnimationReviewCoverageValidation
    {
        private const string ReviewScenePath = "Assets/_Project/TweenHelperDevelopment/Validation/Scenes/TweenHelperPresetReview.unity";
        private const string ResultPath = "Temp/AnimationReviewCoverageValidation.txt";
        private const string PendingSessionKey = "TweenHelper.AnimationReviewCoverageValidation.Pending";
        private const string StatusKeyPrefix = "TweenHelper.PresetReview.Status.";
        private const int ExpectedPresetCount = 300;
        private const int ExpectedLegacySemanticCount = 98;
        private const int ExpectedCoverageCount = 76;
        private const int ExpectedTotalCount = 474;

        static AnimationReviewCoverageValidation()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("Tools/Tween Helper Dev/Validate Animation Review Coverage", false, 101)]
        private static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Animation review coverage validation must start while the Editor is stopped.");
                return;
            }

            string resultPath = Path.GetFullPath(ResultPath);
            var results = new List<string>();
            try
            {
                EnsureReviewSceneIsOpen();
                PresetReviewController controller = GetReviewController();
                RunStaticValidation(controller, results);
            }
            catch (Exception exception)
            {
                results.Add("FAIL | Static validator exception | " + Unwrap(exception));
            }

            File.WriteAllLines(resultPath, results);
            if (results.Any(IsFailure))
            {
                WriteResultSummary(resultPath, results);
                Debug.LogError($"Animation review coverage validation failed before Play Mode. See {resultPath}");
                return;
            }

            SessionState.SetBool(PendingSessionKey, true);
            EditorApplication.EnterPlaymode();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(PendingSessionKey, false)) return;
            SessionState.SetBool(PendingSessionKey, false);
            RunRuntimeValidation();
        }

        private static void RunStaticValidation(PresetReviewController controller, List<string> results)
        {
            IList items = BuildReviewItems(controller);
            List<string> ids = GetIds(items);
            HashSet<string> uniqueIds = new HashSet<string>(ids, StringComparer.Ordinal);
            HashSet<string> coverageIds = BuildExpectedCoverageIds();
            HashSet<string> legacySemanticIds = BuildExpectedLegacySemanticIds();
            HashSet<string> presetIds = ids.Where(id => id.StartsWith("Preset:", StringComparison.Ordinal)).ToHashSet(StringComparer.Ordinal);
            HashSet<string> actualCoverageIds = ids.Where(id => !presetIds.Contains(id) && !legacySemanticIds.Contains(id)).ToHashSet(StringComparer.Ordinal);

            Record(results, $"Catalog contains exactly {ExpectedTotalCount} entries", ids.Count == ExpectedTotalCount, $"actual={ids.Count}");
            Record(results, "Every review ID is unique", uniqueIds.Count == ids.Count, $"unique={uniqueIds.Count}, total={ids.Count}");
            Record(results, $"Catalog still contains {ExpectedPresetCount} preset IDs", presetIds.Count == ExpectedPresetCount, $"actual={presetIds.Count}");
            Record(results, $"All {ExpectedLegacySemanticCount} legacy semantic IDs are unchanged", legacySemanticIds.Count == ExpectedLegacySemanticCount && legacySemanticIds.All(uniqueIds.Contains), DescribeMissing(legacySemanticIds, uniqueIds));
            Record(results, $"Exactly {ExpectedCoverageCount} coverage IDs were added", actualCoverageIds.SetEquals(coverageIds), DescribeSetDifference(coverageIds, actualCoverageIds));
            Record(results, "The preserved baseline still contains 398 IDs", legacySemanticIds.Count + presetIds.Count == 398, $"actual={legacySemanticIds.Count + presetIds.Count}");
            int signedDownwardItems = items.Cast<object>().Count(item => GetItemId(item).StartsWith("Destination:", StringComparison.Ordinal) && GetItemId(item).EndsWith(":Downward", StringComparison.Ordinal) && GetItemFloat(item, "SignedMagnitude") < 0f);
            Record(results, "All 6 downward destination entries carry a negative signed magnitude", signedDownwardItems == 6, $"actual={signedDownwardItems}");

            int existingCoverageStatuses = coverageIds.Count(id => PlayerPrefs.HasKey(StatusKeyPrefix + id));
            results.Add($"INFO | New coverage IDs with an existing manual status | {existingCoverageStatuses} (zero is expected before first review; later runs may be non-zero)");
            ValidateSerializedFixtures(controller, results);
        }

        private static void RunRuntimeValidation()
        {
            string resultPath = Path.GetFullPath(ResultPath);
            var results = File.Exists(resultPath) ? File.ReadAllLines(resultPath).ToList() : new List<string>();

            try
            {
                PresetReviewController controller = GetReviewController();
                IList allItems = GetAllItems(controller);
                Dictionary<string, object> itemById = allItems.Cast<object>().ToDictionary(GetItemId, StringComparer.Ordinal);
                HashSet<string> coverageIds = BuildExpectedCoverageIds();
                FieldInfo filteredItemsField = GetRequiredField("_items");
                FieldInfo currentIndexField = GetRequiredField("_currentIndex");
                FieldInfo activeTweenField = GetRequiredField("_activeTween");
                MethodInfo showCurrentItem = GetRequiredMethod("ShowCurrentItem");
                MethodInfo stopPlayback = GetRequiredMethod("StopPlayback");
                MethodInfo resetTargets = GetRequiredMethod("ResetTargets");
                var filteredItems = (IList)filteredItemsField.GetValue(controller);
                int smokePasses = 0;
                int downwardPathPasses = 0;

                foreach (string id in coverageIds.OrderBy(value => value, StringComparer.Ordinal))
                {
                    try
                    {
                        filteredItems.Clear();
                        filteredItems.Add(itemById[id]);
                        currentIndexField.SetValue(controller, 0);
                        showCurrentItem.Invoke(controller, null);
                        RuntimeBaseline baseline = RuntimeBaseline.Capture(controller, id);
                        controller.ReplayCurrent();
                        var handle = (TweenHandle)activeTweenField.GetValue(controller);
                        if (handle?.Tween == null || !handle.Tween.IsActive()) throw new InvalidOperationException("Playback returned no active tween.");
                        if (CanSeekDownwardPath(id))
                        {
                            baseline.ValidateDownwardExcursion(controller, handle.Tween, id);
                            downwardPathPasses++;
                        }
                        handle.Complete();
                        baseline.ValidateCompleted(controller, id);

                        if (id == "CameraFeedback:FovKick:In") ValidateInterruptedCameraFov(controller, activeTweenField, baseline);

                        stopPlayback.Invoke(controller, null);
                        resetTargets.Invoke(controller, null);
                        smokePasses++;
                    }
                    catch (Exception exception)
                    {
                        results.Add($"FAIL | Runtime smoke playback | {id} | {Unwrap(exception)}");
                        TryCleanup(controller, stopPlayback, resetTargets);
                    }
                }

                Record(results, $"All {ExpectedCoverageCount} new review entries build and complete", smokePasses == ExpectedCoverageCount, $"passed={smokePasses}");
                Record(results, "All 4 directly seekable signed downward paths travel below their endpoint floor", downwardPathPasses == 4, $"passed={downwardPathPasses}");
                Record(results, "Runtime smoke validation did not change manual review statuses", BuildExpectedCoverageIds().Count(id => PlayerPrefs.HasKey(StatusKeyPrefix + id)) == CountExistingCoverageStatuses(results), "manual status keys remain caller-owned");
            }
            catch (Exception exception)
            {
                results.Add("FAIL | Runtime validator exception | " + Unwrap(exception));
            }

            WriteResultSummary(resultPath, results);
            bool failed = results.Any(IsFailure);
            if (failed) Debug.LogError($"Animation review coverage validation failed. See {resultPath}");
            else Debug.Log($"Animation review coverage validation passed. See {resultPath}");
            EditorApplication.ExitPlaymode();
        }

        private static int CountExistingCoverageStatuses(List<string> results)
        {
            const string prefix = "INFO | New coverage IDs with an existing manual status | ";
            string line = results.FirstOrDefault(value => value.StartsWith(prefix, StringComparison.Ordinal));
            if (line == null) return 0;
            string value = line.Substring(prefix.Length).Split(' ')[0];
            return int.TryParse(value, out int count) ? count : 0;
        }

        private static void ValidateInterruptedCameraFov(PresetReviewController controller, FieldInfo activeTweenField, RuntimeBaseline baseline)
        {
            controller.ReplayCurrent();
            var interruptedHandle = (TweenHandle)activeTweenField.GetValue(controller);
            Tween tween = interruptedHandle?.Tween;
            if (tween == null || !tween.IsActive()) throw new InvalidOperationException("Interrupted FOV playback returned no active tween.");
            baseline.ValidateCameraNarrowed(controller, tween);
            interruptedHandle.Kill();
            baseline.ValidateCameraRestored(controller);
        }

        private static void ValidateSerializedFixtures(PresetReviewController controller, List<string> results)
        {
            var serializedController = new SerializedObject(controller);
            string[] objectReferences =
            {
                "incompleteGridPreviewGroup",
                "worldCollectionPreviewRoot",
                "drawerSequenceBackdrop",
                "worldTextValuePreviewRoot",
                "worldCharacterText"
            };
            for (int i = 0; i < objectReferences.Length; i++)
            {
                string propertyName = objectReferences[i];
                SerializedProperty property = serializedController.FindProperty(propertyName);
                bool resolved = property != null && property.objectReferenceValue != null;
                Record(results, $"Scene reference resolves: {propertyName}", resolved, resolved ? "resolved" : "missing or unresolved");
            }

            ValidateArrayProperty(serializedController, "incompleteGridTargets", 8, results);
            ValidateArrayProperty(serializedController, "worldCollectionTargets", 6, results);

            SerializedProperty worldTextProperty = serializedController.FindProperty("worldCharacterText");
            Record(results, "World text fixture uses TextMeshPro rather than TextMeshProUGUI", worldTextProperty?.objectReferenceValue is TextMeshPro, worldTextProperty?.objectReferenceValue?.GetType().Name ?? "null");
        }

        private static void ValidateArrayProperty(SerializedObject serializedObject, string propertyName, int expectedSize, List<string> results)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            bool valid = property != null && property.isArray && property.arraySize == expectedSize;
            if (valid)
            {
                for (int i = 0; i < property.arraySize; i++) valid &= property.GetArrayElementAtIndex(i).objectReferenceValue != null;
            }

            Record(results, $"Scene array resolves: {propertyName}[{expectedSize}]", valid, property == null ? "missing" : $"actual={property.arraySize}");
        }

        private static IList BuildReviewItems(PresetReviewController controller)
        {
            IList items = GetAllItems(controller);
            items.Clear();
            GetRequiredMethod("BuildReviewItems").Invoke(controller, null);
            return items;
        }

        private static IList GetAllItems(PresetReviewController controller) => (IList)GetRequiredField("_allItems").GetValue(controller);

        private static List<string> GetIds(IList items) => items.Cast<object>().Select(GetItemId).ToList();

        private static string GetItemId(object item)
        {
            FieldInfo idField = item.GetType().GetField("Id", BindingFlags.Instance | BindingFlags.Public);
            return (string)idField.GetValue(item);
        }

        private static float GetItemFloat(object item, string fieldName)
        {
            FieldInfo field = item.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            return (float)field.GetValue(item);
        }

        private static bool CanSeekDownwardPath(string id)
        {
            return id.EndsWith(":Downward", StringComparison.Ordinal) &&
                   (id.StartsWith("Destination:Arc", StringComparison.Ordinal) || id.StartsWith("Destination:MultiHop", StringComparison.Ordinal));
        }

        private static FieldInfo GetRequiredField(string name)
            => typeof(PresetReviewController).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingFieldException(typeof(PresetReviewController).FullName, name);

        private static MethodInfo GetRequiredMethod(string name)
            => typeof(PresetReviewController).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingMethodException(typeof(PresetReviewController).FullName, name);

        private static PresetReviewController GetReviewController()
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                PresetReviewController controller = roots[i].GetComponentInChildren<PresetReviewController>(true);
                if (controller != null) return controller;
            }

            throw new InvalidOperationException($"No {nameof(PresetReviewController)} exists in the active review scene.");
        }

        private static void EnsureReviewSceneIsOpen()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.path == ReviewScenePath) return;
            if (activeScene.isDirty) throw new InvalidOperationException("The active scene has unsaved changes. Save or discard them before running review coverage validation.");
            EditorSceneManager.OpenScene(ReviewScenePath, OpenSceneMode.Single);
        }

        private static HashSet<string> BuildExpectedCoverageIds()
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            AddVariants(ids, "Collection:GridDiagonalWave", "TopRightToBottomLeft", "BottomLeftToTopRight", "BottomRightToTopLeft", "IncompleteGrid");
            AddVariants(ids, "Collection:GridSpiral", "OutsideInCounterClockwise", "InsideOutClockwise", "InsideOutCounterClockwise", "IncompleteGrid");
            AddVariants(ids, "Collection:GridCheckerboard", "Inverted");
            AddVariants(ids, "Collection:GridRipple", "CornerOrigin", "EdgeOrigin");
            AddVariants(ids, "Collection:StaggerPresetByName", "PresetByName");
            AddVariants(ids, "Collection:StaggerCustomAnimate", "CustomAnimate");
            AddVariants(ids, "Collection:CollectionBurstIn", "World");
            AddVariants(ids, "Collection:CollectionBurstOut", "World", "DefaultDistanceUI");
            AddVariants(ids, "Collection:CollectionGatherTo", "World");

            AddVariants(ids, "Destination:PathThrough3D", "Linear");
            AddVariants(ids, "Destination:PathLocalThroughUi", "Linear");
            AddVariants(ids, "Destination:ArcTo3D", "Downward");
            AddVariants(ids, "Destination:ArcLocalToUi", "Downward");
            AddVariants(ids, "Destination:HopTo3D", "Downward");
            AddVariants(ids, "Destination:HopLocalToUi", "Downward");
            AddVariants(ids, "Destination:MultiHopTo3D", "Downward");
            AddVariants(ids, "Destination:MultiHopLocalToUi", "Downward");
            AddVariants(ids, "Destination:SpiralTo3D", "ReverseWinding");
            AddVariants(ids, "Destination:SpiralLocalToUi", "ReverseWinding");

            AddDirectionalVariants(ids, "UISequence:ToastShow", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:ToastHide", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:TooltipShow", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:TooltipHide", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:TabSwitch", UISequenceDirection.Up, UISequenceDirection.Down, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:DrawerShow", UISequenceDirection.Up, UISequenceDirection.Down, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:DrawerHide", UISequenceDirection.Up, UISequenceDirection.Down, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "UISequence:PagePush", UISequenceDirection.Up, UISequenceDirection.Down, UISequenceDirection.Right);

            AddDirectionalVariants(ids, "TextValue:TextCharacterStaggerIn", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "TextValue:TextCharacterStaggerOut", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "TextValue:TextWave", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "TextValue:TextCharacterBounce", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddDirectionalVariants(ids, "TextValue:TextEmphasis", UISequenceDirection.Down, UISequenceDirection.Left, UISequenceDirection.Right);
            AddVariants(ids, "TextValue:TextCharacterStaggerIn", "World");
            AddVariants(ids, "TextValue:TextColorSweep", "World");
            AddVariants(ids, "TextValue:TextScrambleReveal", "World");

            ids.Add("Feedback:HealReceive:UI");
            ids.Add("Feedback:ShieldBlock:UI");
            ids.Add("Feedback:CriticalHit:UI");
            ids.Add("Feedback:CooldownReady:World");
            ids.Add("Feedback:LevelUp:World");
            ids.Add("Feedback:LowHealthWarning:World");
            AddVariants(ids, "CameraFeedback:FovKick", "In");
            return ids;
        }

        private static HashSet<string> BuildExpectedLegacySemanticIds()
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            AddNames(ids, "Recipe", "UIAppear", "UIAppearSoft", "UIDisappear", "UIDisappearSoft", "UIHover", "UIHoverSoft", "UIPress", "UIPressHard", "UIAttention", "UIAttentionSoft", "UIAttentionHard", "UIDisabled", "UIEnabled");
            AddNames(ids, "Collection", "ListStaggerIn", "ListStaggerOut", "GridWave", "GridRipple", "LoadingDots", "OrderFirstToLast", "OrderLastToFirst", "OrderFromCenter", "OrderToCenter", "OrderRandom", "GridWaveRightToLeft", "GridWaveTopToBottom", "GridWaveBottomToTop", "GridDiagonalWave", "GridSpiral", "GridCheckerboard", "CollectionBurstIn", "CollectionBurstOut", "CollectionGatherTo");
            AddNames(ids, "Destination", "ArcTo3D", "ArcLocalToUi", "BezierTo3D", "BezierLocalToUi", "HopTo3D", "HopLocalToUi", "SpringTo3D", "SpringLocalToUi", "MagneticSnapTo3D", "MagneticSnapLocalToUi", "PathThrough3D", "PathLocalThroughUi", "SpiralTo3D", "SpiralLocalToUi", "MultiHopTo3D", "MultiHopLocalToUi");

            string[] pairedFeedback = { "ErrorReject", "DamageHit", "SuccessConfirm", "RewardReveal", "PickupCollect" };
            for (int i = 0; i < pairedFeedback.Length; i++)
            {
                ids.Add($"Feedback:{pairedFeedback[i]}:World");
                ids.Add($"Feedback:{pairedFeedback[i]}:UI");
            }
            ids.Add("Feedback:HealReceive:World");
            ids.Add("Feedback:ShieldBlock:World");
            ids.Add("Feedback:CriticalHit:World");
            ids.Add("Feedback:CooldownReady:UI");
            ids.Add("Feedback:LevelUp:UI");
            ids.Add("Feedback:LowHealthWarning:UI");

            AddNames(ids, "UISequence", "ToastShow", "ToastHide", "ModalOpen", "ModalClose", "TooltipShow", "TooltipHide", "DropdownOpen", "DropdownClose", "TabSwitch", "DrawerShow", "DrawerHide", "BottomSheetShow", "BottomSheetHide", "PagePush", "PageCrossFade");
            AddNames(ids, "TextValue", "TypewriterReveal", "TypewriterHide", "NumberCountUp", "NumberCountDown", "TextCharacterStaggerIn", "TextWave", "ScoreIncrease", "TextCharacterStaggerOut", "TextCharacterBounce", "TextColorSweep", "TextGlitch", "TextEmphasis", "TextScrambleReveal");
            AddNames(ids, "CameraFeedback", "Impact", "Recoil", "LandingImpact", "FovKick", "FocusZoom", "Breathing");
            return ids;
        }

        private static void AddDirectionalVariants(HashSet<string> ids, string baseId, params UISequenceDirection[] directions)
        {
            for (int i = 0; i < directions.Length; i++) ids.Add($"{baseId}:{directions[i]}");
        }

        private static void AddVariants(HashSet<string> ids, string baseId, params string[] variants)
        {
            for (int i = 0; i < variants.Length; i++) ids.Add($"{baseId}:{variants[i]}");
        }

        private static void AddNames(HashSet<string> ids, string category, params string[] names)
        {
            for (int i = 0; i < names.Length; i++) ids.Add($"{category}:{names[i]}");
        }

        private static void Record(List<string> results, string name, bool passed, string details)
        {
            results.Add($"{(passed ? "PASS" : "FAIL")} | {name} | {details}");
        }

        private static string DescribeMissing(HashSet<string> expected, HashSet<string> actual)
        {
            string[] missing = expected.Where(id => !actual.Contains(id)).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            return missing.Length == 0 ? "none missing" : "missing=" + string.Join(", ", missing);
        }

        private static string DescribeSetDifference(HashSet<string> expected, HashSet<string> actual)
        {
            string[] missing = expected.Where(id => !actual.Contains(id)).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            string[] extra = actual.Where(id => !expected.Contains(id)).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            return $"missing=[{string.Join(", ", missing)}], extra=[{string.Join(", ", extra)}]";
        }

        private static bool IsFailure(string line) => line.StartsWith("FAIL |", StringComparison.Ordinal);

        private static Exception Unwrap(Exception exception)
        {
            while (exception is TargetInvocationException && exception.InnerException != null) exception = exception.InnerException;
            return exception;
        }

        private static void TryCleanup(PresetReviewController controller, MethodInfo stopPlayback, MethodInfo resetTargets)
        {
            try
            {
                stopPlayback.Invoke(controller, null);
                resetTargets.Invoke(controller, null);
            }
            catch
            {
            }
        }

        private static void WriteResultSummary(string resultPath, List<string> results)
        {
            int failures = results.Count(IsFailure);
            results.RemoveAll(line => line.StartsWith("RESULT:", StringComparison.Ordinal));
            results.Insert(0, failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({failures})");
            File.WriteAllLines(resultPath, results);
        }

        private readonly struct RuntimeBaseline
        {
            private readonly bool _hasDestination;
            private readonly bool _destinationUsesUi;
            private readonly Vector3 _destinationStart;
            private readonly Vector3 _destination;
            private readonly Vector3 _scale;
            private readonly Quaternion _rotation;
            private readonly bool _hasWorldText;
            private readonly string _worldText;
            private readonly Color _worldTextColor;
            private readonly Vector3 _worldTextPosition;
            private readonly Vector3 _worldTextScale;
            private readonly Quaternion _worldTextRotation;
            private readonly bool _hasCamera;
            private readonly float _cameraFov;

            private RuntimeBaseline(bool hasDestination, bool destinationUsesUi, Vector3 destinationStart, Vector3 destination, Vector3 scale, Quaternion rotation, bool hasWorldText, string worldText, Color worldTextColor, Vector3 worldTextPosition, Vector3 worldTextScale, Quaternion worldTextRotation, bool hasCamera, float cameraFov)
            {
                _hasDestination = hasDestination;
                _destinationUsesUi = destinationUsesUi;
                _destinationStart = destinationStart;
                _destination = destination;
                _scale = scale;
                _rotation = rotation;
                _hasWorldText = hasWorldText;
                _worldText = worldText;
                _worldTextColor = worldTextColor;
                _worldTextPosition = worldTextPosition;
                _worldTextScale = worldTextScale;
                _worldTextRotation = worldTextRotation;
                _hasCamera = hasCamera;
                _cameraFov = cameraFov;
            }

            public static RuntimeBaseline Capture(PresetReviewController controller, string id)
            {
                bool hasDestination = id.StartsWith("Destination:", StringComparison.Ordinal);
                bool destinationUsesUi = hasDestination && id.Contains("Ui", StringComparison.Ordinal);
                GameObject destinationTarget = null;
                Vector3 destinationStart = Vector3.zero;
                Vector3 destination = Vector3.zero;
                Vector3 scale = Vector3.one;
                Quaternion rotation = Quaternion.identity;
                if (hasDestination)
                {
                    destinationTarget = (GameObject)GetRequiredField(destinationUsesUi ? "destinationUiTarget" : "destinationWorldTarget").GetValue(controller);
                    Transform startMarker = (Transform)GetRequiredField(destinationUsesUi ? "destinationUiStartMarker" : "destinationWorldStartMarker").GetValue(controller);
                    Transform marker = (Transform)GetRequiredField(destinationUsesUi ? "destinationUiEndMarker" : "destinationWorldEndMarker").GetValue(controller);
                    destinationStart = destinationUsesUi ? ((RectTransform)startMarker).anchoredPosition3D : startMarker.position;
                    destination = destinationUsesUi ? ((RectTransform)marker).anchoredPosition3D : marker.position;
                    scale = destinationTarget.transform.localScale;
                    rotation = destinationTarget.transform.localRotation;
                }

                bool hasWorldText = id.StartsWith("TextValue:", StringComparison.Ordinal) && id.EndsWith(":World", StringComparison.Ordinal);
                TMP_Text worldText = (TMP_Text)GetRequiredField("worldCharacterText").GetValue(controller);
                bool hasCamera = id == "CameraFeedback:FovKick:In";
                Camera camera = (Camera)GetRequiredField("feedbackCamera").GetValue(controller);
                return new RuntimeBaseline(
                    hasDestination,
                    destinationUsesUi,
                    destinationStart,
                    destination,
                    scale,
                    rotation,
                    hasWorldText,
                    worldText.text,
                    worldText.color,
                    worldText.transform.localPosition,
                    worldText.transform.localScale,
                    worldText.transform.localRotation,
                    hasCamera,
                    camera.fieldOfView);
            }

            public void ValidateDownwardExcursion(PresetReviewController controller, Tween tween, string id)
            {
                if (!_hasDestination) throw new InvalidOperationException($"{id} has no destination fixture.");
                GameObject target = (GameObject)GetRequiredField(_destinationUsesUi ? "destinationUiTarget" : "destinationWorldTarget").GetValue(controller);
                float lowestY = float.PositiveInfinity;
                for (int i = 1; i < 20; i++)
                {
                    tween.Goto(tween.Duration(false) * (i / 20f), false);
                    Vector3 position = _destinationUsesUi ? ((RectTransform)target.transform).anchoredPosition3D : target.transform.position;
                    lowestY = Mathf.Min(lowestY, position.y);
                }

                float endpointFloor = Mathf.Min(_destinationStart.y, _destination.y);
                if (lowestY >= endpointFloor - 0.01f) throw new InvalidOperationException($"{id} reached a lowest Y of {lowestY:F3}; it did not travel below the endpoint floor {endpointFloor:F3}.");
            }

            public void ValidateCompleted(PresetReviewController controller, string id)
            {
                if (_hasDestination)
                {
                    GameObject target = (GameObject)GetRequiredField(_destinationUsesUi ? "destinationUiTarget" : "destinationWorldTarget").GetValue(controller);
                    Vector3 position = _destinationUsesUi ? ((RectTransform)target.transform).anchoredPosition3D : target.transform.position;
                    if (Vector3.Distance(position, _destination) > 0.01f) throw new InvalidOperationException($"Destination completion drifted by {Vector3.Distance(position, _destination):F4}.");
                    if (Vector3.Distance(target.transform.localScale, _scale) > 0.001f) throw new InvalidOperationException("Destination completion did not restore scale.");
                    if (Quaternion.Angle(target.transform.localRotation, _rotation) > 0.01f) throw new InvalidOperationException("Destination completion did not restore orientation.");
                }

                if (_hasWorldText)
                {
                    TMP_Text text = (TMP_Text)GetRequiredField("worldCharacterText").GetValue(controller);
                    bool restored = text.text == _worldText &&
                                    Approximately(text.color, _worldTextColor) &&
                                    Vector3.Distance(text.transform.localPosition, _worldTextPosition) <= 0.001f &&
                                    Vector3.Distance(text.transform.localScale, _worldTextScale) <= 0.001f &&
                                    Quaternion.Angle(text.transform.localRotation, _worldTextRotation) <= 0.01f;
                    if (!restored) throw new InvalidOperationException($"World TMP completion did not restore its source state for {id}.");
                }

                if (_hasCamera) ValidateCameraRestored(controller);
            }

            public void ValidateCameraRestored(PresetReviewController controller)
            {
                Camera camera = (Camera)GetRequiredField("feedbackCamera").GetValue(controller);
                if (Mathf.Abs(camera.fieldOfView - _cameraFov) > 0.001f) throw new InvalidOperationException($"Camera FOV restored to {camera.fieldOfView:F3} instead of {_cameraFov:F3}.");
            }

            public void ValidateCameraNarrowed(PresetReviewController controller, Tween tween)
            {
                tween.Goto(tween.Duration(false) * 0.2f, false);
                Camera camera = (Camera)GetRequiredField("feedbackCamera").GetValue(controller);
                if (camera.fieldOfView >= _cameraFov - 0.001f) throw new InvalidOperationException($"Camera inward FOV kick reached {camera.fieldOfView:F3} from {_cameraFov:F3}; it did not narrow the lens.");
            }

            private static bool Approximately(Color a, Color b)
                => Mathf.Abs(a.r - b.r) <= 0.001f && Mathf.Abs(a.g - b.g) <= 0.001f && Mathf.Abs(a.b - b.b) <= 0.001f && Mathf.Abs(a.a - b.a) <= 0.001f;
        }
    }
}
