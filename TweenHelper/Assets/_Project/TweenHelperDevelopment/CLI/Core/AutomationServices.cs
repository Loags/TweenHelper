using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PackageManagerPackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace LB.TweenHelper.Automation.Editor
{
    public static class ContextService
    {
        public static ServiceResult<ContextData> Get(string requestId)
        {
            CompatibilityTuple compatibility = CompatibilityService.GetTuple();
            Scene activeScene = SceneManager.GetActiveScene();
            UnityEngine.Object[] selectedObjects = Selection.objects ?? Array.Empty<UnityEngine.Object>();
            string[] selectedTypes = selectedObjects
                .Where(selected => selected != null)
                .Select(GetSelectionObjectType)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(typeName => typeName, StringComparer.Ordinal)
                .ToArray();

            var data = new ContextData
            {
                Compatibility = compatibility,
                Scene = new SceneContextSummary
                {
                    OpenSceneCount = SceneManager.sceneCount,
                    HasActiveScene = activeScene.IsValid(),
                    ActiveSceneLoaded = activeScene.IsValid() && activeScene.isLoaded,
                    ActiveSceneDirty = activeScene.IsValid() && activeScene.isDirty,
                    ActiveSceneUntitled = activeScene.IsValid() && string.IsNullOrEmpty(activeScene.path),
                    ActiveSceneRootCount = activeScene.IsValid() && activeScene.isLoaded ? activeScene.rootCount : 0,
                    PrefabStageOpen = PrefabStageUtility.GetCurrentPrefabStage() != null,
                    IsPlaying = EditorApplication.isPlaying,
                    IsPaused = EditorApplication.isPaused,
                    IsCompiling = EditorApplication.isCompiling
                },
                Selection = new SelectionContextSummary
                {
                    Count = selectedObjects.Length,
                    ObjectTypes = selectedTypes
                },
                Capabilities = new CapabilitySummary
                {
                    Discovery = true,
                    TargetProfiling = true,
                    PresetPlanning = false,
                    SandboxPreview = false,
                    SandboxVerification = false,
                    PersistentAuthoring = false,
                    ProjectExtensions = false
                }
            };
            return new ServiceResult<ContextData>(requestId, "ready", data);
        }

        private static string GetSelectionObjectType(UnityEngine.Object selected)
        {
            if (selected is GameObject) return "GameObject";
            if (selected is RectTransform) return "RectTransform";
            if (selected is Transform) return "Transform";
            if (selected is CanvasGroup) return "CanvasGroup";
            if (selected is TMP_Text) return "TMP_Text";
            if (selected is Graphic) return "Graphic";
            if (selected is SpriteRenderer) return "SpriteRenderer";
            if (selected is Renderer) return "Renderer";
            if (selected is Component) return "Component";
            if (selected is SceneAsset) return "SceneAsset";
            if (EditorUtility.IsPersistent(selected)) return "Asset";
            return "UnityObject";
        }
    }

    public static class CompatibilityService
    {
        public static CompatibilityTuple GetTuple()
        {
            Dictionary<string, string> packages = GetInstalledPackageVersions();
            packages.TryGetValue("com.unity.pipeline", out string pipelineVersion);
            packages.TryGetValue("com.unity.ugui", out string uguiVersion);
            packages.TryGetValue("com.unity.textmeshpro", out string textMeshProVersion);
            packages.TryGetValue("com.unity.test-framework", out string testFrameworkVersion);
            string textMeshProPackageId = string.IsNullOrEmpty(textMeshProVersion) ? "com.unity.ugui" : "com.unity.textmeshpro";
            if (string.IsNullOrEmpty(textMeshProVersion)) textMeshProVersion = uguiVersion;

            return new CompatibilityTuple
            {
                TweenHelperVersion = TweenHelperAutomationContract.TweenHelperVersion,
                AdapterVersion = TweenHelperAutomationContract.ToolVersion,
                UnityVersion = Application.unityVersion,
                MinimumUnityVersion = TweenHelperAutomationContract.MinimumUnityVersion,
                PipelineVersion = pipelineVersion,
                RequiredPipelineVersion = TweenHelperAutomationContract.PipelinePackageVersion,
                DotweenRuntimeVersion = GetDotweenRuntimeVersion(),
                MinimumDotweenRuntimeVersion = TweenHelperAutomationContract.MinimumDotweenRuntimeVersion,
                MinimumDotweenPackageVersion = TweenHelperAutomationContract.MinimumDotweenPackageVersion,
                UguiVersion = uguiVersion,
                TextMeshProVersion = textMeshProVersion,
                TextMeshProPackageId = textMeshProPackageId,
                TestFrameworkVersion = testFrameworkVersion
            };
        }

        public static Dictionary<string, string> GetInstalledPackageVersions()
        {
            var versions = new Dictionary<string, string>(StringComparer.Ordinal);
            PackageManagerPackageInfo[] packages = PackageManagerPackageInfo.GetAllRegisteredPackages();
            foreach (PackageManagerPackageInfo package in packages)
            {
                if (package == null || string.IsNullOrEmpty(package.name)) continue;
                versions[package.name] = package.version;
            }
            return versions;
        }

        private static string GetDotweenRuntimeVersion()
        {
            try
            {
                return DOTween.Version;
            }
            catch
            {
                return null;
            }
        }
    }

    public static class SetupStatusService
    {
        public static ServiceResult<SetupStatusData> Get(string requestId)
        {
            CompatibilityTuple compatibility = CompatibilityService.GetTuple();
            var dependencies = new List<DependencyStatus>();
            var warnings = new List<CommandIssue>();
            var errors = new List<CommandIssue>();

            bool unitySupported = GetUnityMajorVersion(Application.unityVersion) >= 6000;
            dependencies.Add(CreateDependency("unity", unitySupported, Application.unityVersion, TweenHelperAutomationContract.MinimumUnityVersion,
                unitySupported ? "Unity satisfies the development adapter minimum." : "The installed Pipeline adapter requires Unity 6000.0 or newer."));
            if (!unitySupported) errors.Add(new CommandIssue("missing_dependency", "Unity 6000.0 or newer is required by the installed Pipeline package."));

            bool pipelineSupported = string.Equals(compatibility.PipelineVersion, TweenHelperAutomationContract.PipelinePackageVersion, StringComparison.Ordinal);
            dependencies.Add(CreateDependency("unity_pipeline", pipelineSupported, compatibility.PipelineVersion, TweenHelperAutomationContract.PipelinePackageVersion,
                pipelineSupported ? "The pinned experimental Pipeline version is installed." : "The development adapter is pinned to a different Pipeline version."));
            if (!pipelineSupported) errors.Add(new CommandIssue("missing_dependency", $"Unity Pipeline {TweenHelperAutomationContract.PipelinePackageVersion} is required."));

            bool dotweenLoaded = Type.GetType("DG.Tweening.DOTween, DOTween") != null;
            bool dotweenVersionSupported = dotweenLoaded && Version.TryParse(compatibility.DotweenRuntimeVersion, out Version dotweenVersion) && dotweenVersion >= new Version(1, 3, 30);
            dependencies.Add(CreateDependency("dotween_runtime", dotweenVersionSupported, compatibility.DotweenRuntimeVersion, TweenHelperAutomationContract.MinimumDotweenRuntimeVersion,
                dotweenVersionSupported ? "DOTween runtime satisfies TweenHelper's compatibility policy." : "DOTween runtime is missing, unparseable, or below the required runtime version."));
            if (!dotweenVersionSupported) errors.Add(new CommandIssue("missing_dependency", $"DOTween runtime {TweenHelperAutomationContract.MinimumDotweenRuntimeVersion} or newer is required."));

            bool dotweenUiModule = Type.GetType("DG.Tweening.DOTweenModuleUI, DOTween.Modules") != null;
            dependencies.Add(CreateDependency("dotween_ui_module", dotweenUiModule, dotweenUiModule ? "loaded" : null, "loaded",
                dotweenUiModule ? "DOTween's UI module is loaded." : "Run DOTween Setup to generate its UI module."));
            if (!dotweenUiModule) errors.Add(new CommandIssue("missing_dependency", "DOTween's UI module is required."));

            bool uguiLoaded = typeof(Graphic) != null;
            dependencies.Add(CreateDependency("unity_ugui", uguiLoaded, compatibility.UguiVersion, "installed", uguiLoaded ? "Unity UI is available." : "Unity UI is unavailable."));
            if (!uguiLoaded) errors.Add(new CommandIssue("missing_dependency", "Unity UI is required."));

            bool textMeshProLoaded = typeof(TMP_Text) != null;
            dependencies.Add(CreateDependency("text_mesh_pro", textMeshProLoaded, compatibility.TextMeshProVersion, "installed", textMeshProLoaded ? "TextMesh Pro is available." : "TextMesh Pro is unavailable."));
            if (!textMeshProLoaded) errors.Add(new CommandIssue("missing_dependency", "TextMesh Pro is required."));

            bool testFrameworkLoaded = !string.IsNullOrEmpty(compatibility.TestFrameworkVersion);
            dependencies.Add(CreateDependency("unity_test_framework", testFrameworkLoaded, compatibility.TestFrameworkVersion, "1.1.33+",
                testFrameworkLoaded ? "Unity Test Framework is installed for repository validation." : "Unity Test Framework is missing."));
            if (!testFrameworkLoaded) errors.Add(new CommandIssue("missing_dependency", "Unity Test Framework is required by the development adapter validation suite."));

            bool dotweenSettings = AssetDatabase.FindAssets("t:DOTweenSettings").Length > 0;
            bool tweenHelperSettings = AssetDatabase.FindAssets("t:TweenHelperSettings").Length > 0;
            bool textMeshProEssentials = AssetDatabase.FindAssets("t:TMP_Settings").Length > 0;
            if (!dotweenSettings) warnings.Add(new CommandIssue("optional_settings_missing", "No DOTweenSettings asset was found; DOTween can still use defaults."));
            if (!textMeshProEssentials) warnings.Add(new CommandIssue("optional_settings_missing", "Text Mesh Pro Essential Resources were not detected."));

            var data = new SetupStatusData
            {
                Compatibility = compatibility,
                Dependencies = dependencies,
                DotweenSettingsAssetPresent = dotweenSettings,
                TweenHelperSettingsAssetPresent = tweenHelperSettings,
                TextMeshProEssentialsPresent = textMeshProEssentials
            };
            return new ServiceResult<SetupStatusData>(requestId, errors.Count == 0 ? "ready" : "invalid", data, warnings, errors);
        }

        private static DependencyStatus CreateDependency(string id, bool ready, string installedVersion, string requiredVersion, string message)
        {
            return new DependencyStatus
            {
                Id = id,
                Status = ready ? "ready" : "missing_or_unsupported",
                InstalledVersion = installedVersion,
                RequiredVersion = requiredVersion,
                Message = message
            };
        }

        private static int GetUnityMajorVersion(string unityVersion)
        {
            if (string.IsNullOrEmpty(unityVersion)) return 0;
            int separator = unityVersion.IndexOf('.');
            string major = separator < 0 ? unityVersion : unityVersion.Substring(0, separator);
            return int.TryParse(major, out int parsed) ? parsed : 0;
        }
    }

    public static class TargetProfileService
    {
        public static ServiceResult<TargetProfileData> Profile(GameObject target, TargetProfileRequest request)
        {
            if (target == null)
            {
                return ServiceResult<TargetProfileData>.Invalid(request?.RequestId, new CommandIssue("invalid_target", "The resolved target is not a GameObject or Component.", "input.target"));
            }

            if (request?.Identity == null)
            {
                return ServiceResult<TargetProfileData>.Invalid(request?.RequestId, new CommandIssue("invalid_target", "A canonical target identity is required.", "input.target"));
            }

            int pageSize = request.CompatiblePageSize == 0 ? TweenHelperAutomationContract.DefaultPageSize : request.CompatiblePageSize;
            if (pageSize < 1 || pageSize > TweenHelperAutomationContract.MaximumPageSize)
            {
                return ServiceResult<TargetProfileData>.Invalid(request.RequestId, new CommandIssue("invalid_page_size", $"compatiblePageSize must be between 1 and {TweenHelperAutomationContract.MaximumPageSize}.", "input.compatiblePageSize"));
            }

            BuiltInPresetCatalog catalog = BuiltInPresetCatalog.Instance;
            if (catalog.Issues.Count > 0)
            {
                return new ServiceResult<TargetProfileData>(request.RequestId, "invalid", null, Array.Empty<CommandIssue>(), catalog.Issues);
            }

            string[] components = GetSupportedComponents(target);
            TransformSnapshot transform = CaptureTransform(target.transform);
            RectTransformSnapshot rectTransform = target.transform is RectTransform rect ? CaptureRectTransform(rect) : null;
            List<VisualChannelSnapshot> visualChannels = CaptureVisualChannels(target);
            bool isPersistent = EditorUtility.IsPersistent(target);
            bool isUi = target.transform is RectTransform || target.GetComponent<Graphic>() != null || target.GetComponent<CanvasGroup>() != null;
            string profileHash = ComputeProfileHash(request.Identity, target, components, transform, rectTransform, visualChannels, isPersistent, isUi);
            string cursorBindingHash = ComputeCursorBindingHash(profileHash, catalog.CatalogHash);

            if (!BoundCursor.TryDecode(request.CompatibleCursor, cursorBindingHash, out int offset))
            {
                return ServiceResult<TargetProfileData>.Invalid(request.RequestId, new CommandIssue("stale_target", "compatibleCursor is invalid, or the target profile or built-in catalog changed.", "input.compatibleCursor"));
            }

            var compatibleIds = new List<string>();
            var warnings = new List<CommandIssue>();
            foreach (OperationDescriptor operation in catalog.Operations)
            {
                if (catalog.IsCompatible(operation.Id, target, out string compatibilityError))
                {
                    compatibleIds.Add(operation.Id);
                }
                else if (!string.IsNullOrEmpty(compatibilityError))
                {
                    warnings.Add(new CommandIssue("compatibility_check_failed", $"Compatibility check for '{operation.Id}' failed: {compatibilityError}"));
                }
            }

            if (offset > compatibleIds.Count)
            {
                return ServiceResult<TargetProfileData>.Invalid(request.RequestId, new CommandIssue("invalid_cursor", "compatibleCursor points beyond the compatible operation list.", "input.compatibleCursor"));
            }

            string[] page = compatibleIds.Skip(offset).Take(pageSize).ToArray();
            int nextOffset = offset + page.Length;
            string nextCursor = nextOffset < compatibleIds.Count ? BoundCursor.Encode(nextOffset, cursorBindingHash) : null;
            var data = new TargetProfileData
            {
                Identity = request.Identity,
                CatalogHash = catalog.CatalogHash,
                TargetProfileHash = profileHash,
                IsPersistentAsset = isPersistent,
                IsUi = isUi,
                ActiveSelf = target.activeSelf,
                ActiveInHierarchy = target.activeInHierarchy,
                Components = components,
                Transform = transform,
                RectTransform = rectTransform,
                VisualChannels = visualChannels,
                CompatibleOperationCount = compatibleIds.Count,
                CompatibleOperationIds = page,
                NextCompatibleCursor = nextCursor
            };
            return new ServiceResult<TargetProfileData>(request.RequestId, "valid", data, warnings);
        }

        private static string ComputeCursorBindingHash(string profileHash, string catalogHash)
        {
            return CanonicalHash.Compute(writer =>
            {
                writer.BeginObject();
                writer.WritePropertyName("catalogHash");
                writer.WriteString(catalogHash);
                writer.WritePropertyName("profileHash");
                writer.WriteString(profileHash);
                writer.EndObject();
            });
        }

        private static string[] GetSupportedComponents(GameObject target)
        {
            var components = new SortedSet<string>(StringComparer.Ordinal) { "Transform" };
            if (target.transform is RectTransform) components.Add("RectTransform");
            if (target.GetComponent<CanvasGroup>() != null) components.Add("CanvasGroup");
            if (target.GetComponent<Graphic>() != null) components.Add("Graphic");
            if (target.GetComponent<TMP_Text>() != null) components.Add("TMP_Text");
            if (target.GetComponent<SpriteRenderer>() != null) components.Add("SpriteRenderer");
            if (target.GetComponent<Renderer>() != null) components.Add("Renderer");
            if (target.GetComponents<Component>().Any(component => component != null && component.GetType().FullName == "LB.TweenHelper.UIAnimationStateCache")) components.Add("UIAnimationStateCache");
            return components.ToArray();
        }

        private static TransformSnapshot CaptureTransform(Transform transform)
        {
            return new TransformSnapshot
            {
                LocalPosition = ToFloat3(transform.localPosition),
                LocalRotation = ToFloat4(transform.localRotation),
                LocalScale = ToFloat3(transform.localScale)
            };
        }

        private static RectTransformSnapshot CaptureRectTransform(RectTransform transform)
        {
            return new RectTransformSnapshot
            {
                AnchoredPosition = ToFloat3(transform.anchoredPosition3D),
                SizeDelta = ToFloat2(transform.sizeDelta),
                AnchorMinimum = ToFloat2(transform.anchorMin),
                AnchorMaximum = ToFloat2(transform.anchorMax),
                Pivot = ToFloat2(transform.pivot)
            };
        }

        private static List<VisualChannelSnapshot> CaptureVisualChannels(GameObject target)
        {
            var channels = new List<VisualChannelSnapshot>();
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                channels.Add(new VisualChannelSnapshot { Channel = "canvas_group_alpha", ComponentType = "CanvasGroup", Alpha = canvasGroup.alpha });
            }

            Graphic graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                channels.Add(new VisualChannelSnapshot { Channel = "graphic_color", ComponentType = graphic.GetType().Name, Alpha = graphic.color.a, Color = ToFloat4(graphic.color) });
            }

            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                channels.Add(new VisualChannelSnapshot { Channel = "sprite_color", ComponentType = "SpriteRenderer", Alpha = spriteRenderer.color.a, Color = ToFloat4(spriteRenderer.color) });
            }

            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null && spriteRenderer == null)
            {
                try
                {
                    Material material = renderer.sharedMaterial;
                    if (material != null && material.HasProperty("_Color"))
                    {
                        Color color = material.GetColor("_Color");
                        channels.Add(new VisualChannelSnapshot { Channel = "shared_material_color", ComponentType = renderer.GetType().Name, Alpha = color.a, Color = ToFloat4(color) });
                    }
                }
                catch
                {
                    // A shader can reject a color read. Absence is represented by no channel.
                }
            }

            channels.Sort((left, right) =>
            {
                int channel = string.CompareOrdinal(left.Channel, right.Channel);
                return channel != 0 ? channel : string.CompareOrdinal(left.ComponentType, right.ComponentType);
            });
            return channels;
        }

        private static string ComputeProfileHash(TargetIdentity identity, GameObject target, IEnumerable<string> components, TransformSnapshot transform, RectTransformSnapshot rectTransform, IEnumerable<VisualChannelSnapshot> visualChannels, bool isPersistent, bool isUi)
        {
            return CanonicalHash.Compute(writer =>
            {
                writer.BeginObject();
                writer.WritePropertyName("activeInHierarchy");
                writer.WriteBoolean(target.activeInHierarchy);
                writer.WritePropertyName("activeSelf");
                writer.WriteBoolean(target.activeSelf);
                writer.WritePropertyName("components");
                WriteStrings(writer, components);
                writer.WritePropertyName("identity");
                WriteIdentity(writer, identity);
                writer.WritePropertyName("isPersistentAsset");
                writer.WriteBoolean(isPersistent);
                writer.WritePropertyName("isUi");
                writer.WriteBoolean(isUi);
                writer.WritePropertyName("rectTransform");
                if (rectTransform == null) writer.WriteNull(); else WriteRectTransform(writer, rectTransform);
                writer.WritePropertyName("transform");
                WriteTransform(writer, transform);
                writer.WritePropertyName("visualChannels");
                writer.BeginArray();
                foreach (VisualChannelSnapshot channel in visualChannels)
                {
                    writer.BeginObject();
                    writer.WritePropertyName("alpha");
                    if (channel.Alpha.HasValue) writer.WriteSingle(channel.Alpha.Value); else writer.WriteNull();
                    writer.WritePropertyName("channel");
                    writer.WriteString(channel.Channel);
                    writer.WritePropertyName("color");
                    if (channel.Color == null) writer.WriteNull(); else WriteFloat4(writer, channel.Color);
                    writer.WritePropertyName("componentType");
                    writer.WriteString(channel.ComponentType);
                    writer.EndObject();
                }
                writer.EndArray();
                writer.EndObject();
            });
        }

        private static void WriteIdentity(CanonicalJsonWriter writer, TargetIdentity identity)
        {
            string addressKind;
            string value;
            long? fileId = null;
            if (IsStableGlobalId(identity.GlobalId))
            {
                addressKind = "globalId";
                value = identity.GlobalId;
            }
            else if (!string.IsNullOrEmpty(identity.Guid))
            {
                addressKind = "guid";
                value = identity.Guid.ToLowerInvariant();
                fileId = identity.FileId;
            }
            else if (!string.IsNullOrEmpty(identity.AssetPath))
            {
                addressKind = "path";
                value = identity.AssetPath.Replace('\\', '/');
            }
            else
            {
                addressKind = "instanceId";
                value = identity.InstanceId;
            }

            writer.BeginObject();
            writer.WritePropertyName("addressKind");
            writer.WriteString(addressKind);
            writer.WritePropertyName("fileId");
            if (fileId.HasValue) writer.WriteInt64(fileId.Value); else writer.WriteNull();
            writer.WritePropertyName("objectType");
            writer.WriteString(identity.ObjectType);
            writer.WritePropertyName("value");
            writer.WriteString(value);
            writer.EndObject();
        }

        private static bool IsStableGlobalId(string globalId)
        {
            return !string.IsNullOrEmpty(globalId) && !globalId.StartsWith("GlobalObjectId_V1-0-", StringComparison.Ordinal);
        }

        private static void WriteTransform(CanonicalJsonWriter writer, TransformSnapshot transform)
        {
            writer.BeginObject();
            writer.WritePropertyName("localPosition");
            WriteFloat3(writer, transform.LocalPosition);
            writer.WritePropertyName("localRotation");
            WriteFloat4(writer, transform.LocalRotation);
            writer.WritePropertyName("localScale");
            WriteFloat3(writer, transform.LocalScale);
            writer.EndObject();
        }

        private static void WriteRectTransform(CanonicalJsonWriter writer, RectTransformSnapshot transform)
        {
            writer.BeginObject();
            writer.WritePropertyName("anchorMaximum");
            WriteFloat2(writer, transform.AnchorMaximum);
            writer.WritePropertyName("anchorMinimum");
            WriteFloat2(writer, transform.AnchorMinimum);
            writer.WritePropertyName("anchoredPosition");
            WriteFloat3(writer, transform.AnchoredPosition);
            writer.WritePropertyName("pivot");
            WriteFloat2(writer, transform.Pivot);
            writer.WritePropertyName("sizeDelta");
            WriteFloat2(writer, transform.SizeDelta);
            writer.EndObject();
        }

        private static void WriteFloat2(CanonicalJsonWriter writer, Float2Value value)
        {
            writer.BeginArray();
            writer.WriteSingle(value.X);
            writer.WriteSingle(value.Y);
            writer.EndArray();
        }

        private static void WriteFloat3(CanonicalJsonWriter writer, Float3Value value)
        {
            writer.BeginArray();
            writer.WriteSingle(value.X);
            writer.WriteSingle(value.Y);
            writer.WriteSingle(value.Z);
            writer.EndArray();
        }

        private static void WriteFloat4(CanonicalJsonWriter writer, Float4Value value)
        {
            writer.BeginArray();
            writer.WriteSingle(value.X);
            writer.WriteSingle(value.Y);
            writer.WriteSingle(value.Z);
            writer.WriteSingle(value.W);
            writer.EndArray();
        }

        private static void WriteStrings(CanonicalJsonWriter writer, IEnumerable<string> values)
        {
            writer.BeginArray();
            foreach (string value in values) writer.WriteString(value);
            writer.EndArray();
        }

        private static Float2Value ToFloat2(Vector2 value) => new Float2Value { X = value.x, Y = value.y };
        private static Float3Value ToFloat3(Vector3 value) => new Float3Value { X = value.x, Y = value.y, Z = value.z };
        private static Float4Value ToFloat4(Quaternion value) => new Float4Value { X = value.x, Y = value.y, Z = value.z, W = value.w };
        private static Float4Value ToFloat4(Color value) => new Float4Value { X = value.r, Y = value.g, Z = value.b, W = value.a };
    }
}
