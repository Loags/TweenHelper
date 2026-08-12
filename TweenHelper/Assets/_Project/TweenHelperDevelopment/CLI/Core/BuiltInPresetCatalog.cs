using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LB.TweenHelper.Automation.Editor
{
    public sealed class BuiltInPresetCatalog
    {
        private static readonly Lazy<BuiltInPresetCatalog> LazyInstance = new Lazy<BuiltInPresetCatalog>(Build);

        private readonly Dictionary<string, OperationDescriptor> _operationsById;

        public static BuiltInPresetCatalog Instance => LazyInstance.Value;
        public IReadOnlyList<OperationDescriptor> Operations { get; }
        public IReadOnlyList<CommandIssue> Issues { get; }
        public string CatalogHash { get; }

        private BuiltInPresetCatalog(IReadOnlyList<OperationDescriptor> operations, IReadOnlyList<CommandIssue> issues)
        {
            Operations = operations;
            Issues = issues;
            _operationsById = operations.ToDictionary(operation => operation.Id, StringComparer.Ordinal);
            CatalogHash = CatalogHashing.Compute(operations);
        }

        public OperationDescriptor Find(string operationId)
        {
            if (string.IsNullOrEmpty(operationId)) return null;
            return _operationsById.TryGetValue(operationId, out OperationDescriptor operation) ? operation : null;
        }

        public bool IsCompatible(string operationId, GameObject target, out string error)
        {
            error = null;
            OperationDescriptor operation = Find(operationId);
            if (operation == null)
            {
                error = $"Operation '{operationId}' is not in the built-in catalog.";
                return false;
            }

            try
            {
                return operation.Preset.CanApplyTo(target);
            }
            catch (Exception exception)
            {
                error = exception.GetType().Name;
                return false;
            }
        }

        private static BuiltInPresetCatalog Build()
        {
            var operations = new List<OperationDescriptor>();
            var issues = new List<CommandIssue>();

            try
            {
                Assembly runtimeAssembly = typeof(ITweenPreset).Assembly;
                Type[] presetTypes = runtimeAssembly.GetTypes()
                    .Where(type => !type.IsAbstract && typeof(ITweenPreset).IsAssignableFrom(type) && type.GetCustomAttribute<AutoRegisterPresetAttribute>(false) != null)
                    .OrderBy(type => type.FullName, StringComparer.Ordinal)
                    .ToArray();

                var presetNames = new HashSet<string>(StringComparer.Ordinal);
                var operationIds = new HashSet<string>(StringComparer.Ordinal);

                foreach (Type presetType in presetTypes)
                {
                    try
                    {
                        var preset = Activator.CreateInstance(presetType) as ITweenPreset;
                        if (preset == null)
                        {
                            issues.Add(new CommandIssue("catalog_descriptor_invalid", $"Built-in preset type '{presetType.FullName}' could not be constructed."));
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(preset.PresetName))
                        {
                            issues.Add(new CommandIssue("catalog_descriptor_invalid", $"Built-in preset type '{presetType.FullName}' has an empty preset name."));
                            continue;
                        }

                        if (!presetNames.Add(preset.PresetName))
                        {
                            issues.Add(new CommandIssue("catalog_id_collision", $"Built-in preset name '{preset.PresetName}' is duplicated."));
                            continue;
                        }

                        string operationId = "preset:" + preset.PresetName;
                        if (!operationIds.Add(operationId))
                        {
                            issues.Add(new CommandIssue("catalog_id_collision", $"Built-in operation ID '{operationId}' is duplicated."));
                            continue;
                        }

                        if (float.IsNaN(preset.DefaultDuration) || float.IsInfinity(preset.DefaultDuration) || preset.DefaultDuration <= 0f)
                        {
                            issues.Add(new CommandIssue("catalog_descriptor_invalid", $"Built-in preset '{preset.PresetName}' has invalid default duration {preset.DefaultDuration}."));
                            continue;
                        }

                        operations.Add(CreateDescriptor(presetType, preset, operationId));
                    }
                    catch (Exception exception)
                    {
                        issues.Add(new CommandIssue("catalog_descriptor_invalid", $"Built-in preset type '{presetType.FullName}' failed descriptor construction ({exception.GetType().Name})."));
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                issues.Add(new CommandIssue("catalog_discovery_failed", "TweenHelper runtime types could not be enumerated (ReflectionTypeLoadException)."));
            }
            catch (Exception exception)
            {
                issues.Add(new CommandIssue("catalog_discovery_failed", $"Built-in preset discovery failed ({exception.GetType().Name})."));
            }

            operations.Sort((left, right) => string.CompareOrdinal(left.Id, right.Id));
            if (operations.Count != TweenHelperAutomationContract.BuiltInPresetCount)
            {
                issues.Add(new CommandIssue("catalog_count_mismatch", $"Expected {TweenHelperAutomationContract.BuiltInPresetCount} built-in preset descriptors but found {operations.Count}."));
            }

            return new BuiltInPresetCatalog(operations, issues);
        }

        private static OperationDescriptor CreateDescriptor(Type presetType, ITweenPreset preset, string operationId)
        {
            PresetVariantMetadata metadata = PresetVariantParser.Parse(preset.PresetName);
            bool isInfinite = IsInfinitePreset(preset.PresetName);
            bool isNondeterministic = IsNondeterministicPreset(preset.PresetName);
            bool requiresAlpha = RequiresAlphaBinding(preset.PresetName);
            MethodInfo compatibilityMethod = presetType.GetMethod(nameof(ITweenPreset.CanApplyTo), BindingFlags.Public | BindingFlags.Instance);
            bool inheritsActiveRequirement = compatibilityMethod == null || compatibilityMethod.DeclaringType == typeof(CodePreset);

            string[] requirements;
            if (requiresAlpha)
            {
                requirements = new[] { "alpha_binding", "game_object" };
            }
            else if (inheritsActiveRequirement)
            {
                requirements = new[] { "active_game_object" };
            }
            else
            {
                requirements = new[] { "game_object" };
            }

            return new OperationDescriptor
            {
                Id = operationId,
                Kind = "built_in_preset",
                PresetName = preset.PresetName,
                Description = preset.Description ?? string.Empty,
                Family = metadata.Family ?? string.Empty,
                Intensity = metadata.Intensity ?? string.Empty,
                Direction = metadata.Direction ?? string.Empty,
                AxisOrPlane = string.IsNullOrEmpty(metadata.Axis) ? metadata.Plane ?? string.Empty : metadata.Axis,
                DefaultDuration = preset.DefaultDuration,
                IsInfinite = isInfinite,
                Determinism = isNondeterministic ? "nondeterministic" : "deterministic",
                VerificationOracle = isNondeterministic ? "observational_invariants" : isInfinite ? "owned_cleanup_and_baseline" : "descriptor_endpoint",
                TargetRequirements = requirements,
                OptionAllowlist = new[] { "duration" },
                MutationFootprint = new[]
                {
                    "rect_transform.anchored_position_3d",
                    "transform.local_position",
                    "transform.local_rotation",
                    "transform.local_scale",
                    "visual.alpha",
                    "visual.color"
                },
                ImplementationTypeName = presetType.FullName,
                Preset = preset
            };
        }

        private static bool IsInfinitePreset(string presetName)
        {
            string[] prefixes = { "Blink", "Breathe", "Float", "Heartbeat", "Orbit", "Pendulum", "PulseFade", "Sway" };
            return prefixes.Any(prefix => presetName.StartsWith(prefix, StringComparison.Ordinal));
        }

        private static bool IsNondeterministicPreset(string presetName)
        {
            return presetName.StartsWith("Flicker", StringComparison.Ordinal) ||
                   presetName.StartsWith("Jitter", StringComparison.Ordinal) ||
                   presetName.StartsWith("Shake", StringComparison.Ordinal);
        }

        private static bool RequiresAlphaBinding(string presetName)
        {
            return presetName.StartsWith("Fade", StringComparison.Ordinal) ||
                   presetName.StartsWith("Blink", StringComparison.Ordinal) ||
                   presetName.StartsWith("Flicker", StringComparison.Ordinal) ||
                   presetName.StartsWith("PulseFade", StringComparison.Ordinal);
        }
    }

    public static class CatalogHashing
    {
        public static string Compute(IEnumerable<OperationDescriptor> operations)
        {
            if (operations == null) throw new ArgumentNullException(nameof(operations));
            OperationDescriptor[] ordered = operations.OrderBy(operation => operation.Id, StringComparer.Ordinal).ToArray();
            return CanonicalHash.Compute(writer =>
            {
                writer.BeginObject();
                writer.WritePropertyName("catalogSchemaVersion");
                writer.WriteInt32(TweenHelperAutomationContract.CatalogSchemaVersion);
                writer.WritePropertyName("operations");
                writer.BeginArray();
                foreach (OperationDescriptor operation in ordered)
                {
                    writer.BeginObject();
                    writer.WritePropertyName("defaultDuration");
                    writer.WriteSingle(operation.DefaultDuration);
                    writer.WritePropertyName("determinism");
                    writer.WriteString(operation.Determinism);
                    writer.WritePropertyName("id");
                    writer.WriteString(operation.Id);
                    writer.WritePropertyName("implementationType");
                    writer.WriteString(operation.ImplementationTypeName);
                    writer.WritePropertyName("infinite");
                    writer.WriteBoolean(operation.IsInfinite);
                    writer.WritePropertyName("kind");
                    writer.WriteString(operation.Kind);
                    writer.WritePropertyName("mutationFootprint");
                    WriteStrings(writer, operation.MutationFootprint);
                    writer.WritePropertyName("optionAllowlist");
                    WriteStrings(writer, operation.OptionAllowlist);
                    writer.WritePropertyName("presetName");
                    writer.WriteString(operation.PresetName);
                    writer.WritePropertyName("targetRequirements");
                    WriteStrings(writer, operation.TargetRequirements);
                    writer.WritePropertyName("verificationOracle");
                    writer.WriteString(operation.VerificationOracle);
                    writer.EndObject();
                }
                writer.EndArray();
                writer.WritePropertyName("scope");
                writer.WriteString(TweenHelperAutomationContract.BuiltInCatalogScope);
                writer.EndObject();
            });
        }

        private static void WriteStrings(CanonicalJsonWriter writer, IEnumerable<string> values)
        {
            writer.BeginArray();
            foreach (string value in values ?? Array.Empty<string>()) writer.WriteString(value);
            writer.EndArray();
        }
    }

    internal static class BoundCursor
    {
        public static string Encode(int offset, string bindingHash)
        {
            string payload = offset.ToString(System.Globalization.CultureInfo.InvariantCulture) + "|" + bindingHash;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static bool TryDecode(string cursor, string expectedBindingHash, out int offset)
        {
            offset = 0;
            if (string.IsNullOrEmpty(cursor)) return true;

            try
            {
                string encoded = cursor.Replace('-', '+').Replace('_', '/');
                int padding = encoded.Length % 4;
                if (padding > 0) encoded = encoded.PadRight(encoded.Length + 4 - padding, '=');
                string payload = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                int separator = payload.IndexOf('|');
                if (separator <= 0) return false;
                if (!int.TryParse(payload.Substring(0, separator), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out offset)) return false;
                return offset >= 0 && string.Equals(payload.Substring(separator + 1), expectedBindingHash, StringComparison.Ordinal);
            }
            catch
            {
                offset = 0;
                return false;
            }
        }
    }
}
