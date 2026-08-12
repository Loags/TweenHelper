using System;
using System.Collections.Generic;
using System.Linq;

namespace LB.TweenHelper.Automation.Editor
{
    public static class CatalogService
    {
        public static ServiceResult<CatalogPage> Query(CatalogQuery query)
        {
            if (query == null)
            {
                return ServiceResult<CatalogPage>.Invalid(null, new CommandIssue("invalid_input", "input is required.", "input"));
            }

            string scope = string.IsNullOrWhiteSpace(query.Scope) ? TweenHelperAutomationContract.BuiltInCatalogScope : query.Scope.Trim();
            if (!string.Equals(scope, TweenHelperAutomationContract.BuiltInCatalogScope, StringComparison.Ordinal))
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("catalog_scope_not_allowed", "Only the built_in catalog scope is available in the Phase 1 prototype.", "input.scope"));
            }

            int pageSize = query.PageSize == 0 ? TweenHelperAutomationContract.DefaultPageSize : query.PageSize;
            if (pageSize < 1 || pageSize > TweenHelperAutomationContract.MaximumPageSize)
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("invalid_page_size", $"pageSize must be between 1 and {TweenHelperAutomationContract.MaximumPageSize}.", "input.pageSize"));
            }

            if ((query.Query?.Length ?? 0) > 100)
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("invalid_filter", "query must be at most 100 characters.", "input.query"));
            }

            if ((query.Family?.Length ?? 0) > 100)
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("invalid_filter", "family must be at most 100 characters.", "input.family"));
            }

            string determinism = query.Determinism?.Trim();
            if (!string.IsNullOrEmpty(determinism) && determinism != "deterministic" && determinism != "nondeterministic")
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("invalid_filter", "determinism must be deterministic or nondeterministic.", "input.determinism"));
            }

            BuiltInPresetCatalog catalog = BuiltInPresetCatalog.Instance;
            if (catalog.Issues.Count > 0)
            {
                return new ServiceResult<CatalogPage>(query.RequestId, "invalid", null, Array.Empty<CommandIssue>(), catalog.Issues);
            }

            string normalizedQuery = query.Query?.Trim() ?? string.Empty;
            string normalizedFamily = query.Family?.Trim() ?? string.Empty;
            List<OperationDescriptor> filtered = catalog.Operations
                .Where(operation => Matches(operation, normalizedQuery, normalizedFamily, determinism))
                .ToList();
            string bindingHash = ComputeFilterHash(catalog.CatalogHash, scope, normalizedQuery, normalizedFamily, determinism ?? string.Empty, filtered.Select(operation => operation.Id));
            if (!BoundCursor.TryDecode(query.Cursor, bindingHash, out int offset))
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("invalid_cursor", "cursor is invalid, belongs to different filters, or the filtered catalog changed.", "input.cursor"));
            }

            if (offset > filtered.Count)
            {
                return ServiceResult<CatalogPage>.Invalid(query.RequestId, new CommandIssue("invalid_cursor", "cursor points beyond the filtered catalog.", "input.cursor"));
            }

            OperationDescriptor[] page = filtered.Skip(offset).Take(pageSize).ToArray();
            int nextOffset = offset + page.Length;
            string nextCursor = nextOffset < filtered.Count ? BoundCursor.Encode(nextOffset, bindingHash) : null;
            var data = new CatalogPage
            {
                Scope = scope,
                CatalogHash = catalog.CatalogHash,
                BuiltInCount = catalog.Operations.Count,
                FilteredCount = filtered.Count,
                PageSize = pageSize,
                NextCursor = nextCursor,
                Operations = page
            };
            return new ServiceResult<CatalogPage>(query.RequestId, "valid", data);
        }

        public static ServiceResult<OperationDescriptionData> Describe(string requestId, string scope, string operationId)
        {
            string normalizedScope = string.IsNullOrWhiteSpace(scope) ? TweenHelperAutomationContract.BuiltInCatalogScope : scope.Trim();
            if (!string.Equals(normalizedScope, TweenHelperAutomationContract.BuiltInCatalogScope, StringComparison.Ordinal))
            {
                return ServiceResult<OperationDescriptionData>.Invalid(requestId, new CommandIssue("catalog_scope_not_allowed", "Only the built_in catalog scope is available in the Phase 1 prototype.", "input.scope"));
            }

            if (string.IsNullOrWhiteSpace(operationId))
            {
                return ServiceResult<OperationDescriptionData>.Invalid(requestId, new CommandIssue("unsupported_operation", "operationId is required.", "input.operationId"));
            }

            BuiltInPresetCatalog catalog = BuiltInPresetCatalog.Instance;
            if (catalog.Issues.Count > 0)
            {
                return new ServiceResult<OperationDescriptionData>(requestId, "invalid", null, Array.Empty<CommandIssue>(), catalog.Issues);
            }

            OperationDescriptor operation = catalog.Find(operationId.Trim());
            if (operation == null)
            {
                return ServiceResult<OperationDescriptionData>.Invalid(requestId, new CommandIssue("unsupported_operation", $"Operation '{operationId}' is not in the built-in catalog.", "input.operationId"));
            }

            var data = new OperationDescriptionData
            {
                Scope = normalizedScope,
                CatalogHash = catalog.CatalogHash,
                Operation = operation
            };
            return new ServiceResult<OperationDescriptionData>(requestId, "valid", data);
        }

        private static bool Matches(OperationDescriptor operation, string query, string family, string determinism)
        {
            if (!string.IsNullOrEmpty(family) && !string.Equals(operation.Family, family, StringComparison.OrdinalIgnoreCase)) return false;
            if (!string.IsNullOrEmpty(determinism) && !string.Equals(operation.Determinism, determinism, StringComparison.Ordinal)) return false;
            if (string.IsNullOrEmpty(query)) return true;

            return operation.PresetName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   operation.Family.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   operation.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string ComputeFilterHash(string catalogHash, string scope, string query, string family, string determinism, IEnumerable<string> operationIds)
        {
            return CanonicalHash.Compute(writer =>
            {
                writer.BeginObject();
                writer.WritePropertyName("catalogHash");
                writer.WriteString(catalogHash);
                writer.WritePropertyName("determinism");
                writer.WriteString(determinism);
                writer.WritePropertyName("family");
                writer.WriteString(family);
                writer.WritePropertyName("operationIds");
                writer.BeginArray();
                foreach (string operationId in operationIds) writer.WriteString(operationId);
                writer.EndArray();
                writer.WritePropertyName("query");
                writer.WriteString(query);
                writer.WritePropertyName("scope");
                writer.WriteString(scope);
                writer.EndObject();
            });
        }
    }
}
