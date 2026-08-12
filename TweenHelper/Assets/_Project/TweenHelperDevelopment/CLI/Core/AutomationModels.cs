using System;
using System.Collections.Generic;

namespace LB.TweenHelper.Automation.Editor
{
    public static class TweenHelperAutomationContract
    {
        public const int SchemaVersion = 1;
        public const int CatalogSchemaVersion = 1;
        public const int BuiltInPresetCount = 300;
        public const int DefaultPageSize = 25;
        public const int MaximumPageSize = 50;
        public const string ToolVersion = "0.1.0-dev.1";
        public const string TweenHelperVersion = "1.0.0";
        public const string BuiltInCatalogScope = "built_in";
        public const string PipelinePackageVersion = "0.3.1-exp.1";
        public const string MinimumUnityVersion = "6000.0";
        public const string MinimumDotweenPackageVersion = "1.2.025";
        public const string MinimumDotweenRuntimeVersion = "1.3.030";

        private const int MaximumRequestIdLength = 128;

        public static CommandIssue ValidateSchemaVersion(int schemaVersion)
        {
            return schemaVersion == SchemaVersion
                ? null
                : new CommandIssue("unsupported_schema", $"schemaVersion must be {SchemaVersion}.", "input.schemaVersion");
        }

        public static CommandIssue ValidateRequestId(string requestId)
        {
            if (string.IsNullOrEmpty(requestId)) return null;
            if (requestId.Length > MaximumRequestIdLength)
            {
                return new CommandIssue("invalid_request_id", $"requestId must be at most {MaximumRequestIdLength} characters.", "input.requestId");
            }

            for (int i = 0; i < requestId.Length; i++)
            {
                char character = requestId[i];
                bool allowed = char.IsLetterOrDigit(character) || character == '-' || character == '_' || character == '.' || character == ':';
                if (!allowed)
                {
                    return new CommandIssue("invalid_request_id", "requestId may contain only letters, numbers, '-', '_', '.', and ':'.", "input.requestId");
                }
            }

            return null;
        }
    }

    public sealed class CommandIssue
    {
        public string Code { get; }
        public string Message { get; }
        public string FieldPath { get; }

        public CommandIssue(string code, string message, string fieldPath = null)
        {
            Code = code;
            Message = message;
            FieldPath = fieldPath;
        }
    }

    public sealed class ServiceResult<T>
    {
        public int SchemaVersion { get; }
        public string RequestId { get; }
        public string ToolVersion { get; }
        public string Status { get; }
        public T Data { get; }
        public IReadOnlyList<CommandIssue> Warnings { get; }
        public IReadOnlyList<CommandIssue> Errors { get; }

        public ServiceResult(string requestId, string status, T data, IReadOnlyList<CommandIssue> warnings = null, IReadOnlyList<CommandIssue> errors = null)
        {
            SchemaVersion = TweenHelperAutomationContract.SchemaVersion;
            RequestId = requestId;
            ToolVersion = TweenHelperAutomationContract.ToolVersion;
            Status = status;
            Data = data;
            Warnings = warnings ?? Array.Empty<CommandIssue>();
            Errors = errors ?? Array.Empty<CommandIssue>();
        }

        public static ServiceResult<T> Invalid(string requestId, params CommandIssue[] errors)
        {
            return new ServiceResult<T>(requestId, "invalid", default, Array.Empty<CommandIssue>(), errors ?? Array.Empty<CommandIssue>());
        }
    }

    public sealed class CompatibilityTuple
    {
        public string TweenHelperVersion { get; set; }
        public string AdapterVersion { get; set; }
        public string UnityVersion { get; set; }
        public string MinimumUnityVersion { get; set; }
        public string PipelineVersion { get; set; }
        public string RequiredPipelineVersion { get; set; }
        public string DotweenRuntimeVersion { get; set; }
        public string MinimumDotweenRuntimeVersion { get; set; }
        public string MinimumDotweenPackageVersion { get; set; }
        public string UguiVersion { get; set; }
        public string TextMeshProVersion { get; set; }
        public string TextMeshProPackageId { get; set; }
        public string TestFrameworkVersion { get; set; }
    }

    public sealed class SceneContextSummary
    {
        public int OpenSceneCount { get; set; }
        public bool HasActiveScene { get; set; }
        public bool ActiveSceneLoaded { get; set; }
        public bool ActiveSceneDirty { get; set; }
        public bool ActiveSceneUntitled { get; set; }
        public int ActiveSceneRootCount { get; set; }
        public bool PrefabStageOpen { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsPaused { get; set; }
        public bool IsCompiling { get; set; }
    }

    public sealed class SelectionContextSummary
    {
        public int Count { get; set; }
        public string[] ObjectTypes { get; set; }
    }

    public sealed class CapabilitySummary
    {
        public bool Discovery { get; set; }
        public bool TargetProfiling { get; set; }
        public bool PresetPlanning { get; set; }
        public bool SandboxPreview { get; set; }
        public bool SandboxVerification { get; set; }
        public bool PersistentAuthoring { get; set; }
        public bool ProjectExtensions { get; set; }
    }

    public sealed class ContextData
    {
        public CompatibilityTuple Compatibility { get; set; }
        public SceneContextSummary Scene { get; set; }
        public SelectionContextSummary Selection { get; set; }
        public CapabilitySummary Capabilities { get; set; }
    }

    public sealed class DependencyStatus
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string InstalledVersion { get; set; }
        public string RequiredVersion { get; set; }
        public string Message { get; set; }
    }

    public sealed class SetupStatusData
    {
        public CompatibilityTuple Compatibility { get; set; }
        public IReadOnlyList<DependencyStatus> Dependencies { get; set; }
        public bool DotweenSettingsAssetPresent { get; set; }
        public bool TweenHelperSettingsAssetPresent { get; set; }
        public bool TextMeshProEssentialsPresent { get; set; }
    }

    public sealed class OperationDescriptor
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public string PresetName { get; set; }
        public string Description { get; set; }
        public string Family { get; set; }
        public string Intensity { get; set; }
        public string Direction { get; set; }
        public string AxisOrPlane { get; set; }
        public float DefaultDuration { get; set; }
        public bool IsInfinite { get; set; }
        public string Determinism { get; set; }
        public string VerificationOracle { get; set; }
        public string[] TargetRequirements { get; set; }
        public string[] OptionAllowlist { get; set; }
        public string[] MutationFootprint { get; set; }
        internal string ImplementationTypeName { get; set; }
        internal ITweenPreset Preset { get; set; }
    }

    public sealed class CatalogQuery
    {
        public string RequestId { get; set; }
        public string Scope { get; set; }
        public string Query { get; set; }
        public string Family { get; set; }
        public string Determinism { get; set; }
        public int PageSize { get; set; }
        public string Cursor { get; set; }
    }

    public sealed class CatalogPage
    {
        public string Scope { get; set; }
        public string CatalogHash { get; set; }
        public int BuiltInCount { get; set; }
        public int FilteredCount { get; set; }
        public int PageSize { get; set; }
        public string NextCursor { get; set; }
        public IReadOnlyList<OperationDescriptor> Operations { get; set; }
    }

    public sealed class OperationDescriptionData
    {
        public string Scope { get; set; }
        public string CatalogHash { get; set; }
        public OperationDescriptor Operation { get; set; }
    }

    public sealed class TargetIdentity
    {
        public string GlobalId { get; set; }
        public string AssetPath { get; set; }
        public string Guid { get; set; }
        public long? FileId { get; set; }
        public string InstanceId { get; set; }
        public string ObjectType { get; set; }
    }

    public sealed class Float2Value
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public sealed class Float3Value
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public sealed class Float4Value
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }

    public sealed class TransformSnapshot
    {
        public Float3Value LocalPosition { get; set; }
        public Float4Value LocalRotation { get; set; }
        public Float3Value LocalScale { get; set; }
    }

    public sealed class RectTransformSnapshot
    {
        public Float3Value AnchoredPosition { get; set; }
        public Float2Value SizeDelta { get; set; }
        public Float2Value AnchorMinimum { get; set; }
        public Float2Value AnchorMaximum { get; set; }
        public Float2Value Pivot { get; set; }
    }

    public sealed class VisualChannelSnapshot
    {
        public string Channel { get; set; }
        public string ComponentType { get; set; }
        public float? Alpha { get; set; }
        public Float4Value Color { get; set; }
    }

    public sealed class TargetProfileRequest
    {
        public string RequestId { get; set; }
        public TargetIdentity Identity { get; set; }
        public int CompatiblePageSize { get; set; }
        public string CompatibleCursor { get; set; }
    }

    public sealed class TargetProfileData
    {
        public TargetIdentity Identity { get; set; }
        public string CatalogHash { get; set; }
        public string TargetProfileHash { get; set; }
        public bool IsPersistentAsset { get; set; }
        public bool IsUi { get; set; }
        public bool ActiveSelf { get; set; }
        public bool ActiveInHierarchy { get; set; }
        public string[] Components { get; set; }
        public TransformSnapshot Transform { get; set; }
        public RectTransformSnapshot RectTransform { get; set; }
        public IReadOnlyList<VisualChannelSnapshot> VisualChannels { get; set; }
        public int CompatibleOperationCount { get; set; }
        public IReadOnlyList<string> CompatibleOperationIds { get; set; }
        public string NextCompatibleCursor { get; set; }
    }
}
