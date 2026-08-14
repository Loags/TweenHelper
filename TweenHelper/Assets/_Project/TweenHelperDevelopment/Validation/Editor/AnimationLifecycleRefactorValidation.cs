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
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    [InitializeOnLoad]
    internal static class AnimationLifecycleRefactorValidation
    {
        private const string PendingSessionKey = "TweenHelper.AnimationLifecycleRefactorValidation.Pending";
        private const string ResultPath = "Temp/AnimationLifecycleRefactorValidation.txt";
        private const string ReviewResetResultPath = "Temp/AnimationLifecycleRefactorReviewReset.txt";
        private const string StatusKeyPrefix = "TweenHelper.PresetReview.Status.";
        private const int NeedsWorkStatus = 1;
        private const int ExpectedAffectedReviewCount = 106;
        private const int ExpectedReviewCount = 474;

        static AnimationLifecycleRefactorValidation()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("Tools/Tween Helper Dev/Validate Animation Lifecycle Refactor", false, 102)]
        private static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError("Animation lifecycle refactor validation must start while the Editor is stopped.");
                return;
            }

            string resultPath = Path.GetFullPath(ResultPath);
            if (File.Exists(resultPath)) File.Delete(resultPath);
            SessionState.SetBool(PendingSessionKey, true);
            EditorApplication.EnterPlaymode();
        }

        [MenuItem("Tools/Tween Helper Dev/Mark Lifecycle Refactor Reviews As Needs Work", false, 103)]
        private static void MarkAffectedReviewsAsNeedsWork()
        {
            HashSet<string> ids = BuildAffectedReviewIds();
            if (ids.Count != ExpectedAffectedReviewCount)
            {
                throw new InvalidOperationException($"Expected {ExpectedAffectedReviewCount} lifecycle-affected review IDs but resolved {ids.Count}. No review statuses were changed.");
            }
            ValidateAffectedReviewIds(ids);

            int changed = 0;
            int alreadyNeedsWork = 0;
            foreach (string id in ids)
            {
                string key = StatusKeyPrefix + id;
                if (PlayerPrefs.GetInt(key, 0) == NeedsWorkStatus)
                {
                    alreadyNeedsWork++;
                    continue;
                }

                PlayerPrefs.SetInt(key, NeedsWorkStatus);
                changed++;
            }

            PlayerPrefs.Save();
            bool verified = ids.All(id => PlayerPrefs.GetInt(StatusKeyPrefix + id, 0) == NeedsWorkStatus);
            var results = new[]
            {
                verified ? "RESULT: PASS" : "RESULT: FAIL",
                $"Affected review IDs: {ids.Count}",
                $"Changed to Needs Work: {changed}",
                $"Already Needs Work: {alreadyNeedsWork}",
                "Unaffected review IDs were not written."
            };
            File.WriteAllLines(Path.GetFullPath(ReviewResetResultPath), results);
            if (!verified) throw new InvalidOperationException("One or more lifecycle-affected review statuses did not persist.");
            Debug.Log($"Marked {ids.Count} lifecycle-refactor review entries as Needs Work ({changed} changed, {alreadyNeedsWork} already set). Unaffected statuses were preserved.");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(PendingSessionKey, false)) return;
            SessionState.SetBool(PendingSessionKey, false);
            RunValidation();
        }

        private static void RunValidation()
        {
            string resultPath = Path.GetFullPath(ResultPath);
            var results = new List<string>();
            var root = new GameObject("Animation Lifecycle Refactor Validation");

            RunCase(results, "Camera lazy capture, interrupted kill, callbacks, and nested cleanup", () => ValidateCamera(root));
            RunCase(results, "Feedback transient and Pickup Collect lifecycle policies", () => ValidateFeedback(root));
            RunCase(results, "TMP partial preservation and transient visual restoration", () => ValidateText(root));
            RunCase(results, "UI even-Yoyo invocation restoration", () => ValidateUiYoyo(root));
            RunCase(results, "Spatial direct-play, interruption, completion, and Yoyo", () => ValidateSpatial(root));
            RunCase(results, $"Affected review ID manifest contains exactly {ExpectedAffectedReviewCount} unique IDs", () =>
            {
                HashSet<string> ids = BuildAffectedReviewIds();
                Require(ids.Count == ExpectedAffectedReviewCount, $"actual={ids.Count}");
            });
            RunCase(results, "Every affected review ID exists in the 474-entry catalog", ValidateAffectedReviewIds);

            DOTween.Kill(root, false);
            UnityEngine.Object.DestroyImmediate(root);
            int failures = results.Count(line => line.StartsWith("FAIL |", StringComparison.Ordinal));
            results.Insert(0, failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({failures})");
            File.WriteAllLines(resultPath, results);
            if (failures == 0) Debug.Log($"Animation lifecycle refactor validation passed. See {resultPath}");
            else Debug.LogError($"Animation lifecycle refactor validation failed. See {resultPath}");
            EditorApplication.ExitPlaymode();
        }

        private static void ValidateCamera(GameObject root)
        {
            var cameraObject = new GameObject("Camera", typeof(Camera));
            cameraObject.transform.SetParent(root.transform, false);
            Camera camera = cameraObject.GetComponent<Camera>();
            cameraObject.transform.localPosition = new Vector3(2f, -1f, 4f);
            cameraObject.transform.localRotation = Quaternion.Euler(4f, 12f, -3f);
            camera.fieldOfView = 61f;

            TweenHandle interrupted = cameraObject.Tween().CameraImpact(0.5f, 4f, 1f).Build();
            Vector3 lazyPosition = new Vector3(-3f, 2f, 6f);
            Quaternion lazyRotation = Quaternion.Euler(-7f, 18f, 5f);
            cameraObject.transform.localPosition = lazyPosition;
            cameraObject.transform.localRotation = lazyRotation;
            camera.fieldOfView = 73f;
            Seek(interrupted, 0.37f);
            Require(Vector3.Distance(cameraObject.transform.localPosition, lazyPosition) > 0.0001f || Quaternion.Angle(cameraObject.transform.localRotation, lazyRotation) > 0.001f, "Camera did not leave the lazy-captured pose.");
            interrupted.Kill();
            RequireCamera(camera, lazyPosition, lazyRotation, 73f);

            int completedCallbacks = 0;
            int killedCallbacks = 0;
            bool completionObservedRestoredState = false;
            TweenHandle forced = cameraObject.Tween()
                .CameraFovKick(12f, 1f)
                .OnComplete(() =>
                {
                    completedCallbacks++;
                    completionObservedRestoredState = Mathf.Abs(camera.fieldOfView - 73f) <= 0.001f;
                })
                .OnKill(() => killedCallbacks++)
                .Build();
            Seek(forced, 0.25f);
            forced.Kill(true);
            RequireCamera(camera, lazyPosition, lazyRotation, 73f);
            Require(completedCallbacks == 1 && killedCallbacks == 1, $"callbacks complete={completedCallbacks}, kill={killedCallbacks}");
            Require(completionObservedRestoredState, "Caller completion ran before internal camera restoration.");

            TweenHandle nested = cameraObject.Tween().CameraImpact(0.5f, 4f, 1f).Then().Delay(0.2f).Build();
            Seek(nested, 0.3f);
            nested.Kill();
            RequireCamera(camera, lazyPosition, lazyRotation, 73f);
        }

        private static void ValidateFeedback(GameObject root)
        {
            GameObject target = CreateUiTarget("Feedback", root.transform, new Vector3(25f, -14f, 0f), new Vector3(1.1f, 0.9f, 1f), 0.78f, new Color(0.35f, 0.7f, 0.9f, 0.83f));
            TargetPose baseline = TargetPose.Capture(target);
            TweenHandle transient = target.Tween().ErrorReject(1f).Build();
            Seek(transient, 0.31f);
            transient.Kill();
            Require(TargetPose.Capture(target).Matches(baseline), "Transient feedback did not restore its invocation state.");

            Vector3 destination = new Vector3(280f, 125f, 0f);
            TweenHandle pickup = target.Tween().PickupCollectLocalTo(destination, 90f, 1f).Build();
            Seek(pickup, 0.5f);
            Vector3 interruptedPosition = ((RectTransform)target.transform).anchoredPosition3D;
            Require(Vector3.Distance(interruptedPosition, baseline.Position) > 0.01f, "Pickup did not advance before interruption.");
            pickup.Kill();
            TargetPose interruptedPose = TargetPose.Capture(target);
            Require(Vector3.Distance(interruptedPose.Position, interruptedPosition) <= 0.01f, "Pickup kill did not preserve its current path position.");
            Require(interruptedPose.VisualsMatch(baseline), "Pickup kill did not restore scale, rotation, alpha, and color.");

            baseline.Apply(target);
            TweenHandle forced = target.Tween().PickupCollectLocalTo(destination, 90f, 1f).Build();
            Seek(forced, 0.24f);
            forced.Kill(true);
            TargetPose endpoint = TargetPose.Capture(target);
            Require(Vector3.Distance(endpoint.Position, destination) <= 0.01f, "Kill(true) did not apply the Pickup destination.");
            Require(endpoint.Scale.sqrMagnitude <= 0.000001f && endpoint.Alpha <= 0.001f, "Kill(true) did not apply the Pickup hidden endpoint.");
            Require(Quaternion.Angle(endpoint.Rotation, baseline.Rotation) <= 0.01f, "Kill(true) did not restore Pickup orientation.");
        }

        private static void ValidateText(GameObject root)
        {
            GameObject canvasObject = new GameObject("Text Canvas", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(root.transform, false);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            TextMeshProUGUI typewriter = CreateText("Typewriter", canvasObject.transform, "Lifecycle capture");
            typewriter.maxVisibleCharacters = 0;
            TweenHandle typewriterHandle = typewriter.gameObject.Tween().TypewriterReveal(1f).Build();
            Seek(typewriterHandle, 0.5f);
            int partialVisibility = typewriter.maxVisibleCharacters;
            Require(partialVisibility > 0 && partialVisibility < typewriter.textInfo.characterCount, $"partial visibility={partialVisibility}");
            typewriterHandle.Kill();
            Require(typewriter.maxVisibleCharacters == partialVisibility, "Typewriter interruption did not preserve partial visibility.");

            TextMeshProUGUI bounce = CreateText("Bounce", canvasObject.transform, "Mesh restore");
            Vector3[][] baselineVertices = CaptureVertices(bounce);
            TweenHandle bounceHandle = bounce.gameObject.Tween().TextCharacterBounce(amplitude: 18f, duration: 1f).Build();
            Seek(bounceHandle, 0.43f);
            bounceHandle.Kill();
            Require(VerticesMatch(bounce, baselineVertices), "Character mesh interruption did not restore baseline vertices.");

            TextMeshProUGUI score = CreateText("Score", canvasObject.transform, "100");
            score.color = new Color(0.4f, 0.75f, 0.95f, 0.8f);
            score.transform.localScale = new Vector3(0.9f, 1.1f, 1f);
            score.transform.localRotation = Quaternion.Euler(0f, 0f, 6f);
            Color scoreColor = score.color;
            Vector3 scoreScale = score.transform.localScale;
            Quaternion scoreRotation = score.transform.localRotation;
            TweenHandle scoreHandle = score.gameObject.Tween().ScoreIncrease(100, 300, "0", 1f).Build();
            Seek(scoreHandle, 0.5f);
            string interruptedValue = score.text;
            scoreHandle.Kill();
            Require(score.text == interruptedValue && score.text != "100" && score.text != "300", $"interrupted score={score.text}");
            Require(Vector3.Distance(score.transform.localScale, scoreScale) <= 0.001f, "Score interruption did not restore scale.");
            Require(Quaternion.Angle(score.transform.localRotation, scoreRotation) <= 0.01f, "Score interruption did not restore rotation.");
            Require(ColorsMatch(score.color, scoreColor), "Score interruption did not restore color.");
        }

        private static void ValidateUiYoyo(GameObject root)
        {
            GameObject target = CreateUiTarget("UI Yoyo", root.transform, new Vector3(-18f, 42f, 0f), new Vector3(0.95f, 1.05f, 1f), 0.74f, Color.white);
            TargetPose baseline = TargetPose.Capture(target);
            TweenHandle handle = target.ToastHide(UISequenceDirection.Right, 80f, 0.5f, TweenOptions.WithLoops(2, LoopType.Yoyo));
            handle.Pause();
            handle.Complete();
            Require(TargetPose.Capture(target).Matches(baseline), "Even Yoyo UI completion did not restore invocation state.");
        }

        private static void ValidateSpatial(GameObject root)
        {
            var owner = new GameObject("Spatial Owner");
            owner.transform.SetParent(root.transform, false);
            GameObject[] items =
            {
                CreateUiTarget("Spatial A", root.transform, new Vector3(-70f, 10f, 0f), Vector3.one, 0.8f, Color.white),
                CreateUiTarget("Spatial B", root.transform, new Vector3(0f, 30f, 0f), new Vector3(0.9f, 1.1f, 1f), 0.85f, Color.white),
                CreateUiTarget("Spatial C", root.transform, new Vector3(75f, -15f, 0f), new Vector3(1.08f, 0.92f, 1f), 0.9f, Color.white)
            };
            TargetPose[] baselines = items.Select(TargetPose.Capture).ToArray();

            TweenHandle interrupted = items.CollectionBurstOut(owner, Vector3.zero, 120f, 1f, 0.08f);
            Require(interrupted.IsPlaying, "Spatial recipe did not return an active playing handle.");
            Seek(interrupted, 0.5f);
            interrupted.Kill();
            RequireItemsMatch(items, baselines, "Spatial interruption did not restore every captured item.");

            TweenHandle completed = items.CollectionGatherTo(owner, new Vector3(20f, 15f, 0f), 1f, 0.08f);
            completed.Pause();
            completed.Complete();
            for (int i = 0; i < items.Length; i++)
            {
                TargetPose endpoint = TargetPose.Capture(items[i]);
                Require(endpoint.Scale.sqrMagnitude <= 0.000001f && endpoint.Alpha <= 0.001f, $"Spatial completion endpoint failed at index {i}.");
            }

            for (int i = 0; i < items.Length; i++) baselines[i].Apply(items[i]);
            TweenHandle yoyo = items.CollectionBurstOut(owner, Vector3.zero, 120f, 0.6f, 0.04f, options: TweenOptions.WithLoops(2, LoopType.Yoyo));
            yoyo.Pause();
            yoyo.Complete();
            RequireItemsMatch(items, baselines, "Spatial even-Yoyo completion did not restore invocation states.");
        }

        private static HashSet<string> BuildAffectedReviewIds()
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            AddNames(ids, "CameraFeedback", "Impact", "Recoil", "LandingImpact", "FovKick", "FocusZoom", "Breathing");
            ids.Add("CameraFeedback:FovKick:In");

            string[] pairedFeedback = { "ErrorReject", "DamageHit", "SuccessConfirm", "RewardReveal", "PickupCollect" };
            for (int i = 0; i < pairedFeedback.Length; i++)
            {
                ids.Add($"Feedback:{pairedFeedback[i]}:World");
                ids.Add($"Feedback:{pairedFeedback[i]}:UI");
            }
            AddVariants(ids, "Feedback:HealReceive", "World", "UI");
            AddVariants(ids, "Feedback:ShieldBlock", "World", "UI");
            AddVariants(ids, "Feedback:CriticalHit", "World", "UI");
            AddVariants(ids, "Feedback:CooldownReady", "UI", "World");
            AddVariants(ids, "Feedback:LevelUp", "UI", "World");
            AddVariants(ids, "Feedback:LowHealthWarning", "UI", "World");

            AddNames(ids, "TextValue", "TypewriterReveal", "TypewriterHide", "NumberCountUp", "NumberCountDown", "TextCharacterStaggerIn", "TextWave", "ScoreIncrease", "TextCharacterStaggerOut", "TextCharacterBounce", "TextColorSweep", "TextGlitch", "TextEmphasis", "TextScrambleReveal");
            AddDirectionalVariants(ids, "TextValue:TextCharacterStaggerIn");
            AddDirectionalVariants(ids, "TextValue:TextCharacterStaggerOut");
            AddDirectionalVariants(ids, "TextValue:TextWave");
            AddDirectionalVariants(ids, "TextValue:TextCharacterBounce");
            AddDirectionalVariants(ids, "TextValue:TextEmphasis");
            AddVariants(ids, "TextValue:TextCharacterStaggerIn", "World");
            AddVariants(ids, "TextValue:TextColorSweep", "World");
            AddVariants(ids, "TextValue:TextScrambleReveal", "World");

            AddNames(ids, "UISequence", "ToastShow", "ToastHide", "ModalOpen", "ModalClose", "TooltipShow", "TooltipHide", "DropdownOpen", "DropdownClose", "TabSwitch", "DrawerShow", "DrawerHide", "BottomSheetShow", "BottomSheetHide", "PagePush", "PageCrossFade");
            AddDirectionalVariants(ids, "UISequence:ToastShow");
            AddDirectionalVariants(ids, "UISequence:ToastHide");
            AddDirectionalVariants(ids, "UISequence:TooltipShow");
            AddDirectionalVariants(ids, "UISequence:TooltipHide");
            AddVariants(ids, "UISequence:TabSwitch", UISequenceDirection.Up.ToString(), UISequenceDirection.Down.ToString(), UISequenceDirection.Right.ToString());
            AddVariants(ids, "UISequence:DrawerShow", UISequenceDirection.Up.ToString(), UISequenceDirection.Down.ToString(), UISequenceDirection.Right.ToString());
            AddVariants(ids, "UISequence:DrawerHide", UISequenceDirection.Up.ToString(), UISequenceDirection.Down.ToString(), UISequenceDirection.Right.ToString());
            AddVariants(ids, "UISequence:PagePush", UISequenceDirection.Up.ToString(), UISequenceDirection.Down.ToString(), UISequenceDirection.Right.ToString());

            AddNames(ids, "Collection", "CollectionBurstIn", "CollectionBurstOut", "CollectionGatherTo");
            AddVariants(ids, "Collection:CollectionBurstIn", "World");
            AddVariants(ids, "Collection:CollectionBurstOut", "World", "DefaultDistanceUI");
            AddVariants(ids, "Collection:CollectionGatherTo", "World");
            return ids;
        }

        private static void ValidateAffectedReviewIds() => ValidateAffectedReviewIds(BuildAffectedReviewIds());

        private static void ValidateAffectedReviewIds(HashSet<string> affectedIds)
        {
            PresetReviewController controller = Resources.FindObjectsOfTypeAll<PresetReviewController>().FirstOrDefault(value => value.gameObject.scene.IsValid());
            if (controller == null) throw new InvalidOperationException("The preset review controller is not loaded.");

            FieldInfo allItemsField = typeof(PresetReviewController).GetField("_allItems", BindingFlags.Instance | BindingFlags.NonPublic);
            if (allItemsField?.GetValue(controller) is not IList items) throw new InvalidOperationException("The review catalog could not be read.");
            if (items.Count == 0)
            {
                MethodInfo buildReviewItems = typeof(PresetReviewController).GetMethod("BuildReviewItems", BindingFlags.Instance | BindingFlags.NonPublic);
                if (buildReviewItems == null) throw new InvalidOperationException("The review catalog builder could not be resolved.");
                buildReviewItems.Invoke(controller, null);
            }

            var catalogIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (object item in items)
            {
                FieldInfo idField = item.GetType().GetField("Id", BindingFlags.Instance | BindingFlags.Public);
                if (idField?.GetValue(item) is string id) catalogIds.Add(id);
            }

            Require(items.Count == ExpectedReviewCount, $"entries={items.Count}");
            Require(catalogIds.Count == items.Count, $"unique={catalogIds.Count}, entries={items.Count}");
            string[] missing = affectedIds.Where(id => !catalogIds.Contains(id)).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            Require(missing.Length == 0, "missing=" + string.Join(", ", missing));
        }

        private static void AddDirectionalVariants(HashSet<string> ids, string baseId)
        {
            AddVariants(ids, baseId, UISequenceDirection.Down.ToString(), UISequenceDirection.Left.ToString(), UISequenceDirection.Right.ToString());
        }

        private static void AddVariants(HashSet<string> ids, string baseId, params string[] variants)
        {
            for (int i = 0; i < variants.Length; i++) ids.Add($"{baseId}:{variants[i]}");
        }

        private static void AddNames(HashSet<string> ids, string category, params string[] names)
        {
            for (int i = 0; i < names.Length; i++) ids.Add($"{category}:{names[i]}");
        }

        private static GameObject CreateUiTarget(string name, Transform parent, Vector3 position, Vector3 scale, float alpha, Color color)
        {
            var target = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            target.transform.SetParent(parent, false);
            var rect = (RectTransform)target.transform;
            rect.anchoredPosition3D = position;
            rect.localScale = scale;
            rect.localRotation = Quaternion.Euler(0f, 0f, 5f);
            rect.sizeDelta = new Vector2(120f, 60f);
            target.GetComponent<CanvasGroup>().alpha = alpha;
            target.GetComponent<Image>().color = color;
            return target;
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string value)
        {
            var target = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            target.transform.SetParent(parent, false);
            var text = target.GetComponent<TextMeshProUGUI>();
            text.font = TMP_Settings.defaultFontAsset;
            if (text.font == null) throw new InvalidOperationException("TMP default font asset is required for lifecycle validation.");
            text.text = value;
            text.fontSize = 32f;
            ((RectTransform)target.transform).sizeDelta = new Vector2(500f, 100f);
            text.ForceMeshUpdate(true, true);
            return text;
        }

        private static Vector3[][] CaptureVertices(TMP_Text text)
        {
            text.ForceMeshUpdate(true, true);
            var vertices = new Vector3[text.textInfo.meshInfo.Length][];
            for (int i = 0; i < vertices.Length; i++) vertices[i] = text.textInfo.meshInfo[i].vertices.ToArray();
            return vertices;
        }

        private static bool VerticesMatch(TMP_Text text, Vector3[][] expected)
        {
            if (text.textInfo.meshInfo.Length != expected.Length) return false;
            for (int i = 0; i < expected.Length; i++)
            {
                Vector3[] actual = text.textInfo.meshInfo[i].vertices;
                if (actual.Length != expected[i].Length) return false;
                for (int j = 0; j < actual.Length; j++)
                {
                    if (Vector3.Distance(actual[j], expected[i][j]) > 0.001f) return false;
                }
            }

            return true;
        }

        private static void Seek(TweenHandle handle, float progress)
        {
            Require(handle?.Tween != null && handle.Tween.IsActive(), "Validation received no active tween.");
            handle.Pause();
            handle.Tween.Goto(handle.Tween.Duration(false) * Mathf.Clamp01(progress), false);
        }

        private static void RequireCamera(Camera camera, Vector3 position, Quaternion rotation, float fieldOfView)
        {
            Require(Vector3.Distance(camera.transform.localPosition, position) <= 0.001f, "Camera position was not restored.");
            Require(Quaternion.Angle(camera.transform.localRotation, rotation) <= 0.01f, "Camera rotation was not restored.");
            Require(Mathf.Abs(camera.fieldOfView - fieldOfView) <= 0.001f, "Camera field of view was not restored.");
        }

        private static void RequireItemsMatch(GameObject[] items, TargetPose[] expected, string message)
        {
            for (int i = 0; i < items.Length; i++) Require(TargetPose.Capture(items[i]).Matches(expected[i]), $"{message} Index {i}.");
        }

        private static bool ColorsMatch(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) <= 0.001f && Mathf.Abs(a.g - b.g) <= 0.001f && Mathf.Abs(a.b - b.b) <= 0.001f && Mathf.Abs(a.a - b.a) <= 0.001f;
        }

        private static void RunCase(List<string> results, string name, Action validation)
        {
            try
            {
                validation();
                results.Add("PASS | " + name);
            }
            catch (Exception exception)
            {
                results.Add($"FAIL | {name} | {exception.GetType().Name}: {exception.Message}");
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private readonly struct TargetPose
        {
            public readonly Vector3 Position;
            public readonly Vector3 Scale;
            public readonly Quaternion Rotation;
            public readonly float Alpha;
            public readonly Color Color;

            private TargetPose(Vector3 position, Vector3 scale, Quaternion rotation, float alpha, Color color)
            {
                Position = position;
                Scale = scale;
                Rotation = rotation;
                Alpha = alpha;
                Color = color;
            }

            public static TargetPose Capture(GameObject target)
            {
                var rect = (RectTransform)target.transform;
                return new TargetPose(rect.anchoredPosition3D, rect.localScale, rect.localRotation, target.GetComponent<CanvasGroup>().alpha, target.GetComponent<Image>().color);
            }

            public void Apply(GameObject target)
            {
                var rect = (RectTransform)target.transform;
                rect.anchoredPosition3D = Position;
                rect.localScale = Scale;
                rect.localRotation = Rotation;
                target.GetComponent<CanvasGroup>().alpha = Alpha;
                target.GetComponent<Image>().color = Color;
            }

            public bool Matches(TargetPose other)
            {
                return Vector3.Distance(Position, other.Position) <= 0.01f && VisualsMatch(other);
            }

            public bool VisualsMatch(TargetPose other)
            {
                return Vector3.Distance(Scale, other.Scale) <= 0.001f &&
                       Quaternion.Angle(Rotation, other.Rotation) <= 0.01f &&
                       Mathf.Abs(Alpha - other.Alpha) <= 0.001f &&
                       ColorsMatch(Color, other.Color);
            }
        }
    }
}
