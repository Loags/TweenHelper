using System;
using System.Collections.Generic;
using System.Linq;
using LB.TweenHelper.Automation.Editor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Pipeline.Commands;

namespace LB.TweenHelper.Pipeline.Editor
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class StructuredInput : IStructuredCommandInput
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> _unknownFields;

        [JsonIgnore]
        public bool HasUnknownFields => _unknownFields != null && _unknownFields.Count > 0;

        [JsonIgnore]
        public IReadOnlyList<string> UnknownFieldNames => _unknownFields == null
            ? Array.Empty<string>()
            : _unknownFields.Keys.OrderBy(key => key, StringComparer.Ordinal).ToArray();
    }

    public abstract class CommandInput : StructuredInput
    {
        [CliArg("schemaVersion", "TweenHelper command input schema version. Phase 0-1 requires 1.", Required = true)]
        [JsonProperty("schemaVersion")]
        public int SchemaVersion { get; set; }

        [CliArg("requestId", "Optional caller-generated retry/correlation ID.")]
        [JsonProperty("requestId")]
        public string RequestId { get; set; }
    }

    public sealed class ContextInput : CommandInput
    {
    }

    public sealed class SetupStatusInput : CommandInput
    {
    }

    public sealed class CatalogInput : CommandInput
    {
        [CliArg("scope", "Catalog scope. Phase 1 supports built_in only.")]
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [CliArg("query", "Optional case-insensitive preset name, family, or description search.")]
        [JsonProperty("query")]
        public string Query { get; set; }

        [CliArg("family", "Optional exact family filter.")]
        [JsonProperty("family")]
        public string Family { get; set; }

        [CliArg("determinism", "Optional deterministic or nondeterministic filter.")]
        [JsonProperty("determinism")]
        public string Determinism { get; set; }

        [CliArg("pageSize", "Page size from 1 to 50. Defaults to 25.")]
        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [CliArg("cursor", "Opaque cursor returned by the prior page with identical filters.")]
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
    }

    public sealed class DescribeOperationInput : CommandInput
    {
        [CliArg("scope", "Catalog scope. Phase 1 supports built_in only.")]
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [CliArg("operationId", "Stable operation ID returned by tween_helper_catalog.", Required = true)]
        [JsonProperty("operationId")]
        public string OperationId { get; set; }
    }

    public sealed class TargetProfileInput : CommandInput
    {
        [CliArg("target", "One explicit structured Unity object reference.", Required = true)]
        [JsonProperty("target")]
        public ObjectReferenceInput Target { get; set; }

        [CliArg("compatiblePageSize", "Compatible operation page size from 1 to 50. Defaults to 25.")]
        [JsonProperty("compatiblePageSize")]
        public int? CompatiblePageSize { get; set; }

        [CliArg("compatibleCursor", "Opaque cursor returned by the prior target profile page.")]
        [JsonProperty("compatibleCursor")]
        public string CompatibleCursor { get; set; }
    }

    public sealed class ContractProbeInput : CommandInput
    {
        [CliArg("objectReference", "Structured object-reference schema probe.", Required = true)]
        [JsonProperty("objectReference")]
        public ObjectReferenceInput ObjectReference { get; set; }

        [CliArg("vector", "Structured three-component vector schema probe.", Required = true)]
        [JsonProperty("vector")]
        public Vector3Input Vector { get; set; }

        [CliArg("color", "Structured four-component color schema probe.", Required = true)]
        [JsonProperty("color")]
        public ColorInput Color { get; set; }
    }

    public sealed class ObjectReferenceInput : StructuredInput
    {
        [CliArg("globalId", "Canonical Unity GlobalObjectId.")]
        [JsonProperty("globalId")]
        public string GlobalId { get; set; }

        [CliArg("path", "Project-relative asset path under Assets or Packages.")]
        [JsonProperty("path")]
        public string Path { get; set; }

        [CliArg("guid", "Unity asset GUID, optionally paired with fileId.")]
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [CliArg("fileId", "Optional sub-asset local file ID; valid only with guid.")]
        [JsonProperty("fileId")]
        public long? FileId { get; set; }

        [CliArg("instanceId", "Loaded object ID encoded as a decimal string.")]
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [CliArg("useSelection", "Explicitly resolve the single current Editor selection.")]
        [JsonProperty("useSelection")]
        public bool UseSelection { get; set; }
    }

    public sealed class Vector3Input : StructuredInput
    {
        [CliArg("x", "X component.", Required = true)]
        [JsonProperty("x")]
        public float? X { get; set; }

        [CliArg("y", "Y component.", Required = true)]
        [JsonProperty("y")]
        public float? Y { get; set; }

        [CliArg("z", "Z component.", Required = true)]
        [JsonProperty("z")]
        public float? Z { get; set; }
    }

    public sealed class ColorInput : StructuredInput
    {
        [CliArg("r", "Red component.", Required = true)]
        [JsonProperty("r")]
        public float? R { get; set; }

        [CliArg("g", "Green component.", Required = true)]
        [JsonProperty("g")]
        public float? G { get; set; }

        [CliArg("b", "Blue component.", Required = true)]
        [JsonProperty("b")]
        public float? B { get; set; }

        [CliArg("a", "Alpha component.", Required = true)]
        [JsonProperty("a")]
        public float? A { get; set; }
    }

    internal static class PipelineInputValidation
    {
        public static List<CommandIssue> Validate(CommandInput input)
        {
            var issues = new List<CommandIssue>();
            if (input == null)
            {
                issues.Add(new CommandIssue("invalid_input", "input is required.", "input"));
                return issues;
            }

            Add(issues, TweenHelperAutomationContract.ValidateSchemaVersion(input.SchemaVersion));
            Add(issues, TweenHelperAutomationContract.ValidateRequestId(input.RequestId));
            AddUnknownFields(issues, input, "input");
            return issues;
        }

        public static void AddUnknownFields(List<CommandIssue> issues, StructuredInput input, string fieldPath)
        {
            if (input == null || !input.HasUnknownFields) return;
            issues.Add(new CommandIssue("unknown_field", $"Unrecognized field(s): {string.Join(", ", input.UnknownFieldNames)}.", fieldPath));
        }

        public static void AddFiniteVectorIssues(List<CommandIssue> issues, Vector3Input vector, string fieldPath)
        {
            if (vector == null)
            {
                issues.Add(new CommandIssue("invalid_input", "vector is required.", fieldPath));
                return;
            }

            AddUnknownFields(issues, vector, fieldPath);
            AddRequiredNumberIssue(issues, vector.X, $"{fieldPath}.x");
            AddRequiredNumberIssue(issues, vector.Y, $"{fieldPath}.y");
            AddRequiredNumberIssue(issues, vector.Z, $"{fieldPath}.z");
            if ((vector.X.HasValue && !IsFinite(vector.X.Value)) || (vector.Y.HasValue && !IsFinite(vector.Y.Value)) || (vector.Z.HasValue && !IsFinite(vector.Z.Value)))
            {
                issues.Add(new CommandIssue("invalid_number", "Vector components must be finite.", fieldPath));
            }
        }

        public static void AddFiniteColorIssues(List<CommandIssue> issues, ColorInput color, string fieldPath)
        {
            if (color == null)
            {
                issues.Add(new CommandIssue("invalid_input", "color is required.", fieldPath));
                return;
            }

            AddUnknownFields(issues, color, fieldPath);
            AddRequiredNumberIssue(issues, color.R, $"{fieldPath}.r");
            AddRequiredNumberIssue(issues, color.G, $"{fieldPath}.g");
            AddRequiredNumberIssue(issues, color.B, $"{fieldPath}.b");
            AddRequiredNumberIssue(issues, color.A, $"{fieldPath}.a");
            if ((color.R.HasValue && !IsFinite(color.R.Value)) || (color.G.HasValue && !IsFinite(color.G.Value)) || (color.B.HasValue && !IsFinite(color.B.Value)) || (color.A.HasValue && !IsFinite(color.A.Value)))
            {
                issues.Add(new CommandIssue("invalid_number", "Color components must be finite.", fieldPath));
            }
        }

        private static void AddRequiredNumberIssue(List<CommandIssue> issues, float? value, string fieldPath)
        {
            if (!value.HasValue) issues.Add(new CommandIssue("missing_required_field", "A numeric value is required.", fieldPath));
        }

        private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        private static void Add(List<CommandIssue> issues, CommandIssue issue)
        {
            if (issue != null) issues.Add(issue);
        }
    }
}
