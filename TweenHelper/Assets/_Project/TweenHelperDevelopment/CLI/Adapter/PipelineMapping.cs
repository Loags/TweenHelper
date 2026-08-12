using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LB.TweenHelper.Automation.Editor;
using Newtonsoft.Json.Linq;
using Unity.Pipeline;
using Unity.Pipeline.Editor.Authoring;
using Unity.Pipeline.Models;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Pipeline.Editor
{
    internal sealed class ResolvedPipelineTarget
    {
        public GameObject GameObject { get; set; }
        public TargetIdentity Identity { get; set; }
    }

    internal static class PipelineObjectReferenceResolver
    {
        public static ServiceResult<ResolvedPipelineTarget> Resolve(string requestId, ObjectReferenceInput input)
        {
            List<CommandIssue> issues = ValidateShape(input, "input.target");

            if (issues.Count > 0)
            {
                return new ServiceResult<ResolvedPipelineTarget>(requestId, "invalid", null, Array.Empty<CommandIssue>(), issues);
            }

            UnityEngine.Object resolved;
            if (input.UseSelection)
            {
                UnityEngine.Object[] selection = Selection.objects ?? Array.Empty<UnityEngine.Object>();
                if (selection.Length != 1 || selection[0] == null)
                {
                    return ServiceResult<ResolvedPipelineTarget>.Invalid(requestId, new CommandIssue("invalid_target", "useSelection=true requires exactly one selected object.", "input.target.useSelection"));
                }
                resolved = selection[0];
            }
            else
            {
                var objectReference = new ObjectRef
                {
                    GlobalId = NullIfWhiteSpace(input.GlobalId),
                    Path = NormalizeAssetPath(input.Path),
                    Guid = NullIfWhiteSpace(input.Guid),
                    FileId = input.FileId
                };

                if (!string.IsNullOrWhiteSpace(input.InstanceId))
                {
                    if (!ObjectId.TryParse(input.InstanceId.Trim(), out ObjectId instanceId))
                    {
                        return ServiceResult<ResolvedPipelineTarget>.Invalid(requestId, new CommandIssue("invalid_object_reference", "instanceId must be a valid non-negative decimal object ID.", "input.target.instanceId"));
                    }
                    objectReference.InstanceId = instanceId;
                }

                if (!ObjectResolver.TryResolve(objectReference, out resolved, out string error))
                {
                    return ServiceResult<ResolvedPipelineTarget>.Invalid(requestId, new CommandIssue("invalid_object_reference", error, "input.target"));
                }
            }

            GameObject target = resolved as GameObject ?? (resolved as Component)?.gameObject;
            if (target == null)
            {
                return ServiceResult<ResolvedPipelineTarget>.Invalid(requestId, new CommandIssue("invalid_target", "The resolved object is not a GameObject or Component.", "input.target"));
            }

            AuthoringResult identity = ObjectResolver.Describe(target);
            if (identity == null)
            {
                return ServiceResult<ResolvedPipelineTarget>.Invalid(requestId, new CommandIssue("invalid_target", "The resolved target could not be described canonically.", "input.target"));
            }

            var data = new ResolvedPipelineTarget
            {
                GameObject = target,
                Identity = new TargetIdentity
                {
                    GlobalId = identity.GlobalId,
                    AssetPath = identity.AssetPath,
                    Guid = identity.Guid,
                    FileId = identity.FileId,
                    InstanceId = identity.InstanceId?.ToString(),
                    ObjectType = identity.Type
                }
            };
            return new ServiceResult<ResolvedPipelineTarget>(requestId, "valid", data);
        }

        public static List<CommandIssue> ValidateShape(ObjectReferenceInput input, string fieldPath)
        {
            var issues = new List<CommandIssue>();
            if (input == null)
            {
                issues.Add(new CommandIssue("invalid_object_reference", $"{GetLeafName(fieldPath)} is required.", fieldPath));
                return issues;
            }

            PipelineInputValidation.AddUnknownFields(issues, input, fieldPath);
            if (CountPrimaryForms(input) != 1)
            {
                issues.Add(new CommandIssue("invalid_object_reference", $"{GetLeafName(fieldPath)} must contain exactly one of globalId, path, guid, instanceId, or useSelection=true.", fieldPath));
            }

            if (input.FileId.HasValue && string.IsNullOrWhiteSpace(input.Guid))
            {
                issues.Add(new CommandIssue("invalid_object_reference", "fileId is valid only when guid is supplied.", $"{fieldPath}.fileId"));
            }

            ValidateLengths(input, issues, fieldPath);
            if (!string.IsNullOrWhiteSpace(input.Path)) ValidatePath(input.Path, issues, fieldPath);
            if (!string.IsNullOrWhiteSpace(input.Guid) && !IsUnityGuid(input.Guid.Trim()))
            {
                issues.Add(new CommandIssue("invalid_object_reference", "guid must contain exactly 32 hexadecimal characters.", $"{fieldPath}.guid"));
            }

            return issues;
        }

        private static int CountPrimaryForms(ObjectReferenceInput input)
        {
            int count = 0;
            if (!string.IsNullOrWhiteSpace(input.GlobalId)) count++;
            if (!string.IsNullOrWhiteSpace(input.Path)) count++;
            if (!string.IsNullOrWhiteSpace(input.Guid)) count++;
            if (!string.IsNullOrWhiteSpace(input.InstanceId)) count++;
            if (input.UseSelection) count++;
            return count;
        }

        private static void ValidateLengths(ObjectReferenceInput input, List<CommandIssue> issues, string fieldPath)
        {
            if ((input.GlobalId?.Length ?? 0) > 512) issues.Add(new CommandIssue("invalid_object_reference", "globalId is too long.", $"{fieldPath}.globalId"));
            if ((input.Path?.Length ?? 0) > 512) issues.Add(new CommandIssue("invalid_object_reference", "path is too long.", $"{fieldPath}.path"));
            if ((input.Guid?.Length ?? 0) > 64) issues.Add(new CommandIssue("invalid_object_reference", "guid is too long.", $"{fieldPath}.guid"));
            if ((input.InstanceId?.Length ?? 0) > 32) issues.Add(new CommandIssue("invalid_object_reference", "instanceId is too long.", $"{fieldPath}.instanceId"));
        }

        private static void ValidatePath(string path, List<CommandIssue> issues, string fieldPath)
        {
            string normalized = path.Trim().Replace('\\', '/');
            if (Path.IsPathRooted(normalized))
            {
                issues.Add(new CommandIssue("invalid_object_reference", "Absolute paths are not accepted.", $"{fieldPath}.path"));
                return;
            }

            string[] segments = normalized.Split('/');
            if (segments.Any(segment => segment == "." || segment == ".."))
            {
                issues.Add(new CommandIssue("invalid_object_reference", "Path traversal segments are not accepted.", $"{fieldPath}.path"));
                return;
            }

            bool allowedRoot = normalized.StartsWith("Assets/", StringComparison.Ordinal) || normalized.StartsWith("Packages/", StringComparison.Ordinal);
            if (!allowedRoot)
            {
                issues.Add(new CommandIssue("invalid_object_reference", "Asset paths must begin with Assets/ or Packages/.", $"{fieldPath}.path"));
            }
        }

        private static bool IsUnityGuid(string value)
        {
            if (value.Length != 32) return false;
            for (int i = 0; i < value.Length; i++)
            {
                if (!Uri.IsHexDigit(value[i])) return false;
            }
            return true;
        }

        private static string NormalizeAssetPath(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().Replace('\\', '/');
        private static string NullIfWhiteSpace(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        private static string GetLeafName(string fieldPath) => string.IsNullOrEmpty(fieldPath) ? "object reference" : fieldPath.Split('.').Last();
    }

    internal static class PipelineResultMapping
    {
        public static JObject Context(ServiceResult<ContextData> result) => Map(result, MapContext);
        public static JObject Setup(ServiceResult<SetupStatusData> result) => Map(result, MapSetup);
        public static JObject Catalog(ServiceResult<CatalogPage> result) => Map(result, MapCatalogPage);
        public static JObject Operation(ServiceResult<OperationDescriptionData> result) => Map(result, MapOperationDescription);
        public static JObject TargetProfile(ServiceResult<TargetProfileData> result) => Map(result, MapTargetProfile);

        public static JObject Invalid(string requestId, IEnumerable<CommandIssue> issues)
        {
            var result = new ServiceResult<object>(requestId, "invalid", null, Array.Empty<CommandIssue>(), issues?.ToArray() ?? Array.Empty<CommandIssue>());
            return Map(result, _ => null);
        }

        public static JObject Probe(string requestId, string status, JToken data, IEnumerable<CommandIssue> errors = null)
        {
            var result = new ServiceResult<JToken>(requestId, status, data, Array.Empty<CommandIssue>(), errors?.ToArray() ?? Array.Empty<CommandIssue>());
            return Map(result, value => value);
        }

        private static JObject Map<T>(ServiceResult<T> result, Func<T, JToken> mapData)
        {
            var body = new JObject
            {
                ["schemaVersion"] = result.SchemaVersion,
                ["toolVersion"] = result.ToolVersion,
                ["status"] = result.Status,
                ["warnings"] = MapIssues(result.Warnings),
                ["errors"] = MapIssues(result.Errors),
                ["data"] = result.Data == null ? JValue.CreateNull() : mapData(result.Data)
            };
            if (!string.IsNullOrEmpty(result.RequestId)) body["requestId"] = result.RequestId;
            return body;
        }

        private static JArray MapIssues(IEnumerable<CommandIssue> issues)
        {
            var array = new JArray();
            foreach (CommandIssue issue in issues ?? Array.Empty<CommandIssue>())
            {
                var item = new JObject
                {
                    ["code"] = issue.Code,
                    ["message"] = issue.Message
                };
                if (!string.IsNullOrEmpty(issue.FieldPath)) item["fieldPath"] = issue.FieldPath;
                array.Add(item);
            }
            return array;
        }

        private static JToken MapContext(ContextData data)
        {
            return new JObject
            {
                ["compatibility"] = MapCompatibility(data.Compatibility),
                ["scene"] = new JObject
                {
                    ["openSceneCount"] = data.Scene.OpenSceneCount,
                    ["hasActiveScene"] = data.Scene.HasActiveScene,
                    ["activeSceneLoaded"] = data.Scene.ActiveSceneLoaded,
                    ["activeSceneDirty"] = data.Scene.ActiveSceneDirty,
                    ["activeSceneUntitled"] = data.Scene.ActiveSceneUntitled,
                    ["activeSceneRootCount"] = data.Scene.ActiveSceneRootCount,
                    ["prefabStageOpen"] = data.Scene.PrefabStageOpen,
                    ["isPlaying"] = data.Scene.IsPlaying,
                    ["isPaused"] = data.Scene.IsPaused,
                    ["isCompiling"] = data.Scene.IsCompiling
                },
                ["selection"] = new JObject
                {
                    ["count"] = data.Selection.Count,
                    ["objectTypes"] = new JArray(data.Selection.ObjectTypes ?? Array.Empty<string>())
                },
                ["capabilities"] = new JObject
                {
                    ["discovery"] = data.Capabilities.Discovery,
                    ["targetProfiling"] = data.Capabilities.TargetProfiling,
                    ["presetPlanning"] = data.Capabilities.PresetPlanning,
                    ["sandboxPreview"] = data.Capabilities.SandboxPreview,
                    ["sandboxVerification"] = data.Capabilities.SandboxVerification,
                    ["persistentAuthoring"] = data.Capabilities.PersistentAuthoring,
                    ["projectExtensions"] = data.Capabilities.ProjectExtensions
                }
            };
        }

        private static JToken MapSetup(SetupStatusData data)
        {
            var dependencies = new JArray();
            foreach (DependencyStatus dependency in data.Dependencies ?? Array.Empty<DependencyStatus>())
            {
                dependencies.Add(new JObject
                {
                    ["id"] = dependency.Id,
                    ["status"] = dependency.Status,
                    ["installedVersion"] = dependency.InstalledVersion == null ? JValue.CreateNull() : dependency.InstalledVersion,
                    ["requiredVersion"] = dependency.RequiredVersion,
                    ["message"] = dependency.Message
                });
            }

            return new JObject
            {
                ["compatibility"] = MapCompatibility(data.Compatibility),
                ["dependencies"] = dependencies,
                ["dotweenSettingsAssetPresent"] = data.DotweenSettingsAssetPresent,
                ["tweenHelperSettingsAssetPresent"] = data.TweenHelperSettingsAssetPresent,
                ["textMeshProEssentialsPresent"] = data.TextMeshProEssentialsPresent
            };
        }

        private static JObject MapCompatibility(CompatibilityTuple compatibility)
        {
            return new JObject
            {
                ["tweenHelperVersion"] = compatibility.TweenHelperVersion,
                ["adapterVersion"] = compatibility.AdapterVersion,
                ["unityVersion"] = compatibility.UnityVersion,
                ["minimumUnityVersion"] = compatibility.MinimumUnityVersion,
                ["pipelineVersion"] = compatibility.PipelineVersion == null ? JValue.CreateNull() : compatibility.PipelineVersion,
                ["requiredPipelineVersion"] = compatibility.RequiredPipelineVersion,
                ["dotweenRuntimeVersion"] = compatibility.DotweenRuntimeVersion == null ? JValue.CreateNull() : compatibility.DotweenRuntimeVersion,
                ["minimumDotweenRuntimeVersion"] = compatibility.MinimumDotweenRuntimeVersion,
                ["minimumDotweenPackageVersion"] = compatibility.MinimumDotweenPackageVersion,
                ["uguiVersion"] = compatibility.UguiVersion == null ? JValue.CreateNull() : compatibility.UguiVersion,
                ["textMeshProVersion"] = compatibility.TextMeshProVersion == null ? JValue.CreateNull() : compatibility.TextMeshProVersion,
                ["textMeshProPackageId"] = compatibility.TextMeshProPackageId == null ? JValue.CreateNull() : compatibility.TextMeshProPackageId,
                ["testFrameworkVersion"] = compatibility.TestFrameworkVersion == null ? JValue.CreateNull() : compatibility.TestFrameworkVersion
            };
        }

        private static JToken MapCatalogPage(CatalogPage data)
        {
            var operations = new JArray();
            foreach (OperationDescriptor operation in data.Operations ?? Array.Empty<OperationDescriptor>()) operations.Add(MapOperationSummary(operation));
            return new JObject
            {
                ["scope"] = data.Scope,
                ["catalogHash"] = data.CatalogHash,
                ["builtInCount"] = data.BuiltInCount,
                ["filteredCount"] = data.FilteredCount,
                ["pageSize"] = data.PageSize,
                ["nextCursor"] = data.NextCursor == null ? JValue.CreateNull() : data.NextCursor,
                ["operations"] = operations
            };
        }

        private static JToken MapOperationDescription(OperationDescriptionData data)
        {
            return new JObject
            {
                ["scope"] = data.Scope,
                ["catalogHash"] = data.CatalogHash,
                ["operation"] = MapOperationDetails(data.Operation)
            };
        }

        private static JObject MapOperationSummary(OperationDescriptor operation)
        {
            return new JObject
            {
                ["id"] = operation.Id,
                ["kind"] = operation.Kind,
                ["presetName"] = operation.PresetName,
                ["family"] = operation.Family,
                ["defaultDuration"] = operation.DefaultDuration,
                ["infinite"] = operation.IsInfinite,
                ["determinism"] = operation.Determinism,
                ["targetRequirements"] = new JArray(operation.TargetRequirements ?? Array.Empty<string>())
            };
        }

        private static JObject MapOperationDetails(OperationDescriptor operation)
        {
            JObject details = MapOperationSummary(operation);
            details["description"] = operation.Description;
            details["intensity"] = operation.Intensity;
            details["direction"] = operation.Direction;
            details["axisOrPlane"] = operation.AxisOrPlane;
            details["optionAllowlist"] = new JArray(operation.OptionAllowlist ?? Array.Empty<string>());
            details["mutationFootprint"] = new JArray(operation.MutationFootprint ?? Array.Empty<string>());
            details["verificationOracle"] = operation.VerificationOracle;
            return details;
        }

        private static JToken MapTargetProfile(TargetProfileData data)
        {
            var visualChannels = new JArray();
            foreach (VisualChannelSnapshot channel in data.VisualChannels ?? Array.Empty<VisualChannelSnapshot>())
            {
                visualChannels.Add(new JObject
                {
                    ["channel"] = channel.Channel,
                    ["componentType"] = channel.ComponentType,
                    ["alpha"] = channel.Alpha.HasValue ? new JValue(channel.Alpha.Value) : JValue.CreateNull(),
                    ["color"] = channel.Color == null ? JValue.CreateNull() : MapColor(channel.Color)
                });
            }

            return new JObject
            {
                ["identity"] = MapIdentity(data.Identity),
                ["catalogHash"] = data.CatalogHash,
                ["targetProfileHash"] = data.TargetProfileHash,
                ["isPersistentAsset"] = data.IsPersistentAsset,
                ["isUi"] = data.IsUi,
                ["activeSelf"] = data.ActiveSelf,
                ["activeInHierarchy"] = data.ActiveInHierarchy,
                ["components"] = new JArray(data.Components ?? Array.Empty<string>()),
                ["transform"] = MapTransform(data.Transform),
                ["rectTransform"] = data.RectTransform == null ? JValue.CreateNull() : MapRectTransform(data.RectTransform),
                ["visualChannels"] = visualChannels,
                ["compatibleOperationCount"] = data.CompatibleOperationCount,
                ["compatibleOperationIds"] = new JArray(data.CompatibleOperationIds ?? Array.Empty<string>()),
                ["nextCompatibleCursor"] = data.NextCompatibleCursor == null ? JValue.CreateNull() : data.NextCompatibleCursor
            };
        }

        private static JObject MapIdentity(TargetIdentity identity)
        {
            return new JObject
            {
                ["globalId"] = identity.GlobalId == null ? JValue.CreateNull() : identity.GlobalId,
                ["assetPath"] = identity.AssetPath == null ? JValue.CreateNull() : identity.AssetPath,
                ["guid"] = identity.Guid == null ? JValue.CreateNull() : identity.Guid,
                ["fileId"] = identity.FileId.HasValue ? new JValue(identity.FileId.Value) : JValue.CreateNull(),
                ["instanceId"] = identity.InstanceId == null ? JValue.CreateNull() : identity.InstanceId,
                ["objectType"] = identity.ObjectType
            };
        }

        private static JObject MapTransform(TransformSnapshot transform)
        {
            return new JObject
            {
                ["localPosition"] = MapVector3(transform.LocalPosition),
                ["localRotation"] = MapVector4(transform.LocalRotation),
                ["localScale"] = MapVector3(transform.LocalScale)
            };
        }

        private static JObject MapRectTransform(RectTransformSnapshot transform)
        {
            return new JObject
            {
                ["anchoredPosition"] = MapVector3(transform.AnchoredPosition),
                ["sizeDelta"] = MapVector2(transform.SizeDelta),
                ["anchorMinimum"] = MapVector2(transform.AnchorMinimum),
                ["anchorMaximum"] = MapVector2(transform.AnchorMaximum),
                ["pivot"] = MapVector2(transform.Pivot)
            };
        }

        private static JObject MapVector2(Float2Value value) => new JObject { ["x"] = value.X, ["y"] = value.Y };
        private static JObject MapVector3(Float3Value value) => new JObject { ["x"] = value.X, ["y"] = value.Y, ["z"] = value.Z };
        private static JObject MapVector4(Float4Value value) => new JObject { ["x"] = value.X, ["y"] = value.Y, ["z"] = value.Z, ["w"] = value.W };
        private static JObject MapColor(Float4Value value) => new JObject { ["r"] = value.X, ["g"] = value.Y, ["b"] = value.Z, ["a"] = value.W };
    }
}
