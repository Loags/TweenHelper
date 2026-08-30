using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    public static class TweenRecipeExecutor
    {
        public static TweenHandle Build(TweenRecipe recipe, IReadOnlyList<TweenPlayerBinding> bindings, GameObject owner, bool useUnscaledTime = false, UpdateType updateType = UpdateType.Normal)
        {
            if (!TryBuild(recipe, bindings, owner, out TweenHandle handle, out TweenRecipeValidationResult validation, useUnscaledTime, updateType))
            {
                throw new InvalidOperationException(validation.GetSummary());
            }

            return handle;
        }

        public static bool TryBuild(TweenRecipe recipe, IReadOnlyList<TweenPlayerBinding> bindings, GameObject owner, out TweenHandle handle, out TweenRecipeValidationResult validation, bool useUnscaledTime = false, UpdateType updateType = UpdateType.Normal)
        {
            handle = null;
            validation = TweenRecipeValidator.Validate(recipe, bindings);
            if (owner == null)
            {
                validation.Add(TweenRecipeValidationSeverity.Error, "A playback owner is required.");
            }

            if (!validation.IsValid) return false;

            var bindingsById = new Dictionary<string, TweenPlayerBinding>(StringComparer.Ordinal);
            for (int i = 0; i < bindings.Count; i++)
            {
                TweenPlayerBinding binding = bindings[i];
                if (binding != null && !string.IsNullOrWhiteSpace(binding.BindingId) && !bindingsById.ContainsKey(binding.BindingId))
                {
                    bindingsById.Add(binding.BindingId, binding);
                }
            }

            Sequence sequence = null;
            try
            {
                sequence = DOTween.Sequence();
                sequence.Pause();
                sequence.SetAutoKill(false);
                sequence.SetRecyclable(false);
                sequence.SetUpdate(updateType, useUnscaledTime);

                for (int i = 0; i < recipe.Nodes.Count; i++)
                {
                    TweenRecipeNode node = recipe.Nodes[i];
                    if (node.Operation == TweenRecipeOperation.Delay)
                    {
                        sequence.AppendInterval(node.Delay + node.Duration);
                        continue;
                    }

                    TweenPlayerBinding playerBinding = bindingsById[node.BindingId];
                    GameObject target = playerBinding.Target;
                    GameObject secondaryTarget = TweenRecipeOperationCatalog.UsesSecondaryBinding(node.Operation)
                        ? bindingsById[node.SecondaryBindingId].Target
                        : null;
                    Tween tween = TweenRecipeOperationCatalog.CreateTween(node, target, secondaryTarget, playerBinding.Targets);
                    if (tween == null) throw new InvalidOperationException($"'{TweenRecipeOperationCatalog.GetDisplayName(node.Operation)}' did not create a tween.");

                    tween.Pause();
                    tween.SetEase(node.Ease);
                    if (node.Delay > 0f) tween.SetDelay(node.Delay);
                    if (target != null) tween.SetTarget(target);

                    if (node.Placement == TweenRecipePlacement.With)
                    {
                        sequence.Join(tween);
                    }
                    else
                    {
                        sequence.Append(tween);
                    }
                }

                sequence.SetTarget(owner);
                sequence.SetLink(owner);
                sequence.Pause();
                handle = new TweenHandle(sequence);
                return true;
            }
            catch (Exception exception)
            {
                if (sequence != null && sequence.IsActive()) sequence.Kill();
                validation.Add(TweenRecipeValidationSeverity.Error, $"Could not build recipe: {exception.Message}");
                return false;
            }
        }
    }
}
