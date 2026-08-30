using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper
{
    public enum TweenRecipeValidationSeverity
    {
        Info,
        Warning,
        Error
    }

    public sealed class TweenRecipeValidationMessage
    {
        public TweenRecipeValidationSeverity Severity { get; }
        public string Text { get; }
        public string NodeId { get; }
        public string BindingId { get; }

        public TweenRecipeValidationMessage(TweenRecipeValidationSeverity severity, string text, string nodeId = null, string bindingId = null)
        {
            Severity = severity;
            Text = text;
            NodeId = nodeId;
            BindingId = bindingId;
        }
    }

    public sealed class TweenRecipeValidationResult
    {
        private readonly List<TweenRecipeValidationMessage> _messages = new List<TweenRecipeValidationMessage>();

        public IReadOnlyList<TweenRecipeValidationMessage> Messages => _messages;
        public bool IsValid
        {
            get
            {
                for (int i = 0; i < _messages.Count; i++)
                {
                    if (_messages[i].Severity == TweenRecipeValidationSeverity.Error) return false;
                }

                return true;
            }
        }

        public string GetSummary()
        {
            if (_messages.Count == 0) return "Recipe is valid.";

            var builder = new StringBuilder();
            for (int i = 0; i < _messages.Count; i++)
            {
                if (i > 0) builder.AppendLine();
                builder.Append(_messages[i].Severity);
                builder.Append(": ");
                builder.Append(_messages[i].Text);
            }

            return builder.ToString();
        }

        internal void Add(TweenRecipeValidationSeverity severity, string text, string nodeId = null, string bindingId = null)
            => _messages.Add(new TweenRecipeValidationMessage(severity, text, nodeId, bindingId));
    }

    public static class TweenRecipeValidator
    {
        public static TweenRecipeValidationResult Validate(TweenRecipe recipe)
        {
            var result = new TweenRecipeValidationResult();
            if (recipe == null)
            {
                result.Add(TweenRecipeValidationSeverity.Error, "Assign a TweenRecipe.");
                return result;
            }

            var bindingsById = new Dictionary<string, TweenRecipeBindingDefinition>(StringComparer.Ordinal);
            var bindingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            IReadOnlyList<TweenRecipeBindingDefinition> bindings = recipe.Bindings;
            for (int i = 0; i < bindings.Count; i++)
            {
                TweenRecipeBindingDefinition binding = bindings[i];
                if (binding == null)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding {i + 1} is missing.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(binding.Id))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding {i + 1} has no ID.");
                }
                else if (!bindingsById.TryAdd(binding.Id, binding))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding ID '{binding.Id}' is duplicated.", bindingId: binding.Id);
                }

                if (string.IsNullOrWhiteSpace(binding.DisplayName))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, "Binding name cannot be empty.", bindingId: binding.Id);
                }
                else if (!bindingNames.Add(binding.DisplayName.Trim()))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding name '{binding.DisplayName}' is duplicated.", bindingId: binding.Id);
                }
            }

            IReadOnlyList<TweenRecipeNode> nodes = recipe.Nodes;
            if (nodes.Count == 0)
            {
                result.Add(TweenRecipeValidationSeverity.Error, "Add at least one node.");
                return result;
            }

            var nodeIds = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < nodes.Count; i++)
            {
                TweenRecipeNode node = nodes[i];
                if (node == null)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Node {i + 1} is missing.");
                    continue;
                }

                ValidateNode(node, i, bindingsById, nodeIds, result);
            }

            return result;
        }

        public static TweenRecipeValidationResult Validate(TweenRecipe recipe, IReadOnlyList<TweenPlayerBinding> playerBindings)
        {
            TweenRecipeValidationResult result = Validate(recipe);
            if (recipe == null) return result;

            var definitionsById = new Dictionary<string, TweenRecipeBindingDefinition>(StringComparer.Ordinal);
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition definition = recipe.Bindings[i];
                if (definition != null && !string.IsNullOrWhiteSpace(definition.Id) && !definitionsById.ContainsKey(definition.Id))
                {
                    definitionsById.Add(definition.Id, definition);
                }
            }

            if (playerBindings == null)
            {
                result.Add(TweenRecipeValidationSeverity.Error, "Player bindings are missing.");
                return result;
            }

            var playerBindingsById = new Dictionary<string, TweenPlayerBinding>(StringComparer.Ordinal);
            for (int i = 0; i < playerBindings.Count; i++)
            {
                TweenPlayerBinding playerBinding = playerBindings[i];
                if (playerBinding == null)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Player binding {i + 1} is missing.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(playerBinding.BindingId))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Player binding {i + 1} has no recipe binding ID.");
                    continue;
                }

                if (!playerBindingsById.TryAdd(playerBinding.BindingId, playerBinding))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Player binding ID '{playerBinding.BindingId}' is duplicated.", bindingId: playerBinding.BindingId);
                    continue;
                }

                if (!definitionsById.ContainsKey(playerBinding.BindingId))
                {
                    result.Add(TweenRecipeValidationSeverity.Warning, $"Player binding '{playerBinding.BindingId}' is no longer used by the recipe.", bindingId: playerBinding.BindingId);
                }
            }

            foreach (KeyValuePair<string, TweenRecipeBindingDefinition> pair in definitionsById)
            {
                if (!playerBindingsById.TryGetValue(pair.Key, out TweenPlayerBinding playerBinding))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding '{pair.Value.DisplayName}' is not configured on the player.", bindingId: pair.Key);
                    continue;
                }

                ValidatePlayerBinding(pair.Value, playerBinding, result);
            }

            ValidateTargetCapabilities(recipe, playerBindingsById, result);
            return result;
        }

        private static void ValidateNode(TweenRecipeNode node, int index, IReadOnlyDictionary<string, TweenRecipeBindingDefinition> bindingsById, ISet<string> nodeIds, TweenRecipeValidationResult result)
        {
            string nodeName = string.IsNullOrWhiteSpace(node.Label) ? TweenRecipeOperationCatalog.GetDisplayName(node.Operation) : node.Label;
            if (string.IsNullOrWhiteSpace(node.Id))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"Node {index + 1} has no ID.");
            }
            else if (!nodeIds.Add(node.Id))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"Node ID '{node.Id}' is duplicated.", node.Id);
            }

            if (index == 0 && node.Placement == TweenRecipePlacement.With)
            {
                result.Add(TweenRecipeValidationSeverity.Error, "The first node must use Then.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.Delay && node.Placement == TweenRecipePlacement.With)
            {
                result.Add(TweenRecipeValidationSeverity.Error, "Delay nodes must use Then.", node.Id);
            }

            if (!TweenRecipeOperationCatalog.IsSupported(node.Operation))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' uses an unsupported operation.", node.Id);
                return;
            }

            if (!IsFinite(node.Duration) || node.Duration <= 0f)
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' duration must be finite and greater than zero.", node.Id);
            }

            if (!IsFinite(node.Delay) || node.Delay < 0f)
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' delay must be finite and zero or greater.", node.Id);
            }

            if (TweenRecipeOperationCatalog.UsesBinding(node.Operation))
            {
                if (string.IsNullOrWhiteSpace(node.BindingId))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' has no binding.", node.Id);
                }
                else if (!bindingsById.TryGetValue(node.BindingId, out TweenRecipeBindingDefinition binding))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' references a missing binding.", node.Id, node.BindingId);
                }
                else if (binding.Kind != TweenRecipeOperationCatalog.GetBindingKind(node.Operation))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' requires a {TweenRecipeOperationCatalog.GetBindingKind(node.Operation)} binding.", node.Id, node.BindingId);
                }
            }

            if (TweenRecipeOperationCatalog.UsesSecondaryBinding(node.Operation))
            {
                if (string.IsNullOrWhiteSpace(node.SecondaryBindingId))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' has no destination binding.", node.Id);
                }
                else if (!bindingsById.TryGetValue(node.SecondaryBindingId, out TweenRecipeBindingDefinition secondaryBinding))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' references a missing destination binding.", node.Id, node.SecondaryBindingId);
                }
                else if (secondaryBinding.Kind != TweenRecipeOperationCatalog.GetSecondaryBindingKind(node.Operation))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' destination requires a {TweenRecipeOperationCatalog.GetSecondaryBindingKind(node.Operation)} binding.", node.Id, node.SecondaryBindingId);
                }
                else if (node.SecondaryBindingId == node.BindingId)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' source and destination bindings must be different.", node.Id, node.BindingId);
                }
            }

            ValidateParameters(node, nodeName, result);
        }

        private static void ValidateParameters(TweenRecipeNode node, string nodeName, TweenRecipeValidationResult result)
        {
            TweenRecipeParameters parameters = node.Parameters;
            if (parameters == null)
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' has no parameter data.", node.Id);
                return;
            }

            TweenRecipeParameterFields fields = TweenRecipeOperationCatalog.GetVisibleFields(node.Operation);
            if ((fields & TweenRecipeParameterFields.Vector3) != 0 && !IsFinite(parameters.Vector3Value))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' vector values must be finite.", node.Id);
            }

            if ((fields & TweenRecipeParameterFields.Color) != 0 && !IsFinite(parameters.ColorValue))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' color values must be finite.", node.Id);
            }

            if ((fields & TweenRecipeParameterFields.Float) != 0 && !IsFinite(parameters.FloatValue))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' value must be finite.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.FadeTo && (parameters.FloatValue < 0f || parameters.FloatValue > 1f))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' alpha must be between 0 and 1.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.RegisteredPreset || node.Operation == TweenRecipeOperation.CollectionPreset)
            {
                if (string.IsNullOrWhiteSpace(parameters.StringValue))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' requires a preset name.", node.Id);
                }
                else if (TweenPresetRegistry.GetPresetByName(parameters.StringValue) == null)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Preset '{parameters.StringValue}' is not registered.", node.Id);
                }
            }

            if (node.Operation == TweenRecipeOperation.PageCrossFadeTo && (parameters.FloatValue < 0f || parameters.FloatValue >= 1f))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' depth scale must be at least 0 and less than 1.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.CollectionPreset && parameters.FloatValue < 0f)
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' stagger delay must be zero or greater.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.CameraFieldOfViewTo && (parameters.FloatValue <= 0f || parameters.FloatValue >= 180f))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' field of view must be greater than 0 and less than 180.", node.Id);
            }

            if ((node.Operation == TweenRecipeOperation.LightIntensityTo || node.Operation == TweenRecipeOperation.ParticleEmissionRateTo) && parameters.FloatValue < 0f)
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' value must be zero or greater.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.AudioVolumeTo && (parameters.FloatValue < 0f || parameters.FloatValue > 1f))
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' volume must be between 0 and 1.", node.Id);
            }

            if (node.Operation == TweenRecipeOperation.NumberCountTo && !string.IsNullOrWhiteSpace(parameters.StringValue))
            {
                try
                {
                    _ = parameters.IntValue.ToString(parameters.StringValue);
                }
                catch (FormatException)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"'{nodeName}' number format is invalid.", node.Id);
                }
            }
        }

        private static void ValidatePlayerBinding(TweenRecipeBindingDefinition definition, TweenPlayerBinding playerBinding, TweenRecipeValidationResult result)
        {
            if (definition.Kind == TweenRecipeBindingKind.GameObject)
            {
                if (playerBinding.Target == null)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding '{definition.DisplayName}' has no target.", bindingId: definition.Id);
                }

                return;
            }

            IReadOnlyList<GameObject> targets = playerBinding.Targets;
            if (targets == null || targets.Count == 0)
            {
                result.Add(TweenRecipeValidationSeverity.Error, $"Binding '{definition.DisplayName}' has no targets.", bindingId: definition.Id);
                return;
            }

            var uniqueTargets = new HashSet<GameObject>();
            for (int i = 0; i < targets.Count; i++)
            {
                GameObject target = targets[i];
                if (target == null)
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding '{definition.DisplayName}' contains an empty target at index {i}.", bindingId: definition.Id);
                }
                else if (!uniqueTargets.Add(target))
                {
                    result.Add(TweenRecipeValidationSeverity.Error, $"Binding '{definition.DisplayName}' contains duplicate target '{target.name}'.", bindingId: definition.Id);
                }
            }
        }

        private static void ValidateTargetCapabilities(TweenRecipe recipe, IReadOnlyDictionary<string, TweenPlayerBinding> playerBindingsById, TweenRecipeValidationResult result)
        {
            for (int i = 0; i < recipe.Nodes.Count; i++)
            {
                TweenRecipeNode node = recipe.Nodes[i];
                if (node == null || node.Parameters == null || !TweenRecipeOperationCatalog.IsSupported(node.Operation) || !TweenRecipeOperationCatalog.UsesBinding(node.Operation)) continue;
                if (!playerBindingsById.TryGetValue(node.BindingId, out TweenPlayerBinding playerBinding)) continue;

                if (TweenRecipeOperationCatalog.GetBindingKind(node.Operation) == TweenRecipeBindingKind.GameObject)
                {
                    ValidateTargetCapability(node, playerBinding.Target, result);
                    if (TweenRecipeOperationCatalog.UsesSecondaryBinding(node.Operation) &&
                        playerBindingsById.TryGetValue(node.SecondaryBindingId, out TweenPlayerBinding secondaryPlayerBinding))
                    {
                        if (secondaryPlayerBinding.Target == playerBinding.Target && playerBinding.Target != null)
                        {
                            result.Add(TweenRecipeValidationSeverity.Error, $"'{TweenRecipeOperationCatalog.GetDisplayName(node.Operation)}' source and destination targets must be different.", node.Id, node.SecondaryBindingId);
                        }
                        else if (node.Operation == TweenRecipeOperation.PageCrossFadeTo && secondaryPlayerBinding.Target != null &&
                                 !(secondaryPlayerBinding.Target.transform is RectTransform))
                        {
                            result.Add(TweenRecipeValidationSeverity.Error, $"'{secondaryPlayerBinding.Target.name}' destination requires a RectTransform.", node.Id, node.SecondaryBindingId);
                        }
                    }

                    continue;
                }

                IReadOnlyList<GameObject> targets = playerBinding.Targets;
                if (targets == null) continue;
                for (int targetIndex = 0; targetIndex < targets.Count; targetIndex++)
                {
                    ValidateTargetCapability(node, targets[targetIndex], result);
                }
            }
        }

        private static void ValidateTargetCapability(TweenRecipeNode node, GameObject target, TweenRecipeValidationResult result)
        {
            if (target == null) return;

            switch (node.Operation)
            {
                case TweenRecipeOperation.RegisteredPreset:
                    ITweenPreset preset = TweenPresetRegistry.GetPresetByName(node.Parameters.StringValue);
                    if (preset != null && !preset.CanApplyTo(target))
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"Preset '{preset.PresetName}' cannot be applied to '{target.name}'.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.FadeTo:
                    if (!TweenTargetUtility.CanFade(target))
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' does not support fading.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.ColorTo:
                    if (!TweenTargetUtility.TryGetColor(target, out _))
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' does not support color animation.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.CollectionPreset:
                    ITweenPreset collectionPreset = TweenPresetRegistry.GetPresetByName(node.Parameters.StringValue);
                    if (collectionPreset != null && !collectionPreset.CanApplyTo(target))
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"Preset '{collectionPreset.PresetName}' cannot be applied to '{target.name}'.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.PageCrossFadeTo:
                    if (!(target.transform is RectTransform))
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' requires a RectTransform.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.TypewriterReveal:
                case TweenRecipeOperation.NumberCountTo:
                    if (target.GetComponent<TMP_Text>() == null)
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' requires a TMP_Text component.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.CameraFieldOfViewTo:
                    if (target.GetComponent<Camera>() == null)
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' requires a Camera component.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.LightIntensityTo:
                    if (target.GetComponent<Light>() == null)
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' requires a Light component.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.AudioVolumeTo:
                    if (target.GetComponent<AudioSource>() == null)
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' requires an AudioSource component.", node.Id, node.BindingId);
                    }
                    break;
                case TweenRecipeOperation.ParticleEmissionRateTo:
                    if (target.GetComponent<ParticleSystem>() == null)
                    {
                        result.Add(TweenRecipeValidationSeverity.Error, $"'{target.name}' requires a ParticleSystem component.", node.Id, node.BindingId);
                    }
                    break;
            }
        }

        private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        private static bool IsFinite(Vector3 value) => IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
        private static bool IsFinite(Color value) => IsFinite(value.r) && IsFinite(value.g) && IsFinite(value.b) && IsFinite(value.a);
    }
}
