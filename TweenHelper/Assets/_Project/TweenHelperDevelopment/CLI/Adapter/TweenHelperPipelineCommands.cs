using System;
using System.Collections.Generic;
using LB.TweenHelper.Automation.Editor;
using Newtonsoft.Json.Linq;
using Unity.Pipeline.Commands;

namespace LB.TweenHelper.Pipeline.Editor
{
    public static class TweenHelperPipelineCommands
    {
        [CliCommand("tween_helper_context", "Developer-only read-only TweenHelper automation context and capability discovery.", MainThreadRequired = true)]
        public static JObject Context([CliArg("input", "Versioned structured command input.", Required = true)] ContextInput input)
        {
            List<CommandIssue> issues = PipelineInputValidation.Validate(input);
            if (issues.Count > 0) return PipelineResultMapping.Invalid(input?.RequestId, issues);
            return PipelineResultMapping.Context(ContextService.Get(input.RequestId));
        }

        [CliCommand("tween_helper_setup_status", "Developer-only read-only TweenHelper and Pipeline compatibility diagnostics.", MainThreadRequired = true)]
        public static JObject SetupStatus([CliArg("input", "Versioned structured command input.", Required = true)] SetupStatusInput input)
        {
            List<CommandIssue> issues = PipelineInputValidation.Validate(input);
            if (issues.Count > 0) return PipelineResultMapping.Invalid(input?.RequestId, issues);
            return PipelineResultMapping.Setup(SetupStatusService.Get(input.RequestId));
        }

        [CliCommand("tween_helper_catalog", "Developer-only read-only discovery of TweenHelper's built-in preset operation catalog.", MainThreadRequired = true)]
        public static JObject Catalog([CliArg("input", "Versioned structured catalog query.", Required = true)] CatalogInput input)
        {
            List<CommandIssue> issues = PipelineInputValidation.Validate(input);
            if (issues.Count > 0) return PipelineResultMapping.Invalid(input?.RequestId, issues);

            var query = new CatalogQuery
            {
                RequestId = input.RequestId,
                Scope = input.Scope,
                Query = input.Query,
                Family = input.Family,
                Determinism = input.Determinism,
                PageSize = input.PageSize ?? 0,
                Cursor = input.Cursor
            };
            return PipelineResultMapping.Catalog(CatalogService.Query(query));
        }

        [CliCommand("tween_helper_describe_operation", "Developer-only read-only details for one built-in TweenHelper preset operation.", MainThreadRequired = true)]
        public static JObject DescribeOperation([CliArg("input", "Versioned structured operation lookup.", Required = true)] DescribeOperationInput input)
        {
            List<CommandIssue> issues = PipelineInputValidation.Validate(input);
            if (issues.Count > 0) return PipelineResultMapping.Invalid(input?.RequestId, issues);
            return PipelineResultMapping.Operation(CatalogService.Describe(input.RequestId, input.Scope, input.OperationId));
        }

        [CliCommand("tween_helper_target_profile", "Developer-only read-only profile of one explicitly referenced TweenHelper target.", MainThreadRequired = true)]
        public static JObject TargetProfile([CliArg("input", "Versioned structured target-profile request.", Required = true)] TargetProfileInput input)
        {
            List<CommandIssue> issues = PipelineInputValidation.Validate(input);
            if (issues.Count > 0) return PipelineResultMapping.Invalid(input?.RequestId, issues);

            ServiceResult<ResolvedPipelineTarget> resolved = PipelineObjectReferenceResolver.Resolve(input.RequestId, input.Target);
            if (resolved.Errors.Count > 0 || resolved.Data == null) return PipelineResultMapping.Invalid(input.RequestId, resolved.Errors);

            var request = new TargetProfileRequest
            {
                RequestId = input.RequestId,
                Identity = resolved.Data.Identity,
                CompatiblePageSize = input.CompatiblePageSize ?? 0,
                CompatibleCursor = input.CompatibleCursor
            };
            return PipelineResultMapping.TargetProfile(TargetProfileService.Profile(resolved.Data.GameObject, request));
        }

        [CliCommand("tween_helper_dev_contract_probe", "Developer-only proof that Pipeline emits and binds nested object-reference, vector, and color DTO schemas.", MainThreadRequired = true)]
        public static JObject ContractProbe([CliArg("input", "Versioned nested structured schema probe.", Required = true)] ContractProbeInput input)
        {
            List<CommandIssue> issues = PipelineInputValidation.Validate(input);
            if (input != null)
            {
                issues.AddRange(PipelineObjectReferenceResolver.ValidateShape(input.ObjectReference, "input.objectReference"));
                PipelineInputValidation.AddFiniteVectorIssues(issues, input.Vector, "input.vector");
                PipelineInputValidation.AddFiniteColorIssues(issues, input.Color, "input.color");
            }
            if (issues.Count > 0) return PipelineResultMapping.Invalid(input?.RequestId, issues);

            var data = new JObject
            {
                ["objectReference"] = new JObject
                {
                    ["addressForm"] = GetAddressForm(input.ObjectReference),
                    ["hasFileId"] = input.ObjectReference.FileId.HasValue
                },
                ["vector"] = new JObject
                {
                    ["x"] = input.Vector.X.Value,
                    ["y"] = input.Vector.Y.Value,
                    ["z"] = input.Vector.Z.Value
                },
                ["color"] = new JObject
                {
                    ["r"] = input.Color.R.Value,
                    ["g"] = input.Color.G.Value,
                    ["b"] = input.Color.B.Value,
                    ["a"] = input.Color.A.Value
                }
            };
            return PipelineResultMapping.Probe(input.RequestId, "valid", data);
        }

        private static string GetAddressForm(ObjectReferenceInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.GlobalId)) return "globalId";
            if (!string.IsNullOrWhiteSpace(input.Path)) return "path";
            if (!string.IsNullOrWhiteSpace(input.Guid)) return "guid";
            if (!string.IsNullOrWhiteSpace(input.InstanceId)) return "instanceId";
            return "selection";
        }
    }
}
