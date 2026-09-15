# Tween Player: choose animations in the Inspector

## Animate without a recipe asset

1. Add **Tween Helper > Tween Player** to an existing GameObject.
2. Leave **Mode** set to **Preset**.
3. Choose a **Category**, search by name or description, then choose an **Animation**.
4. Leave **Target Override** empty to animate this GameObject, or assign another object.
5. Press **Preview** in Edit Mode, or enable **Play On Start** for runtime playback.

The selection comes from the preset registry, including the 300 built-in presets and custom registered presets. Category and search are filters: changing them does not replace your saved animation. A selected animation outside the filter stays visible. Selections are stored by unique preset name, so new entries and sorting do not change existing components.

No ScriptableObject or binding synchronization is needed for Preset mode. A target must satisfy the preset's component requirements and active-state check. Incompatible presets remain in the list; the Inspector explains why playback is unavailable.

## Settings and playback

**Override Duration** replaces the preset's duration argument. Otherwise the preset's own default is used. Some multi-phase presets interpret this as the duration of a phase; it is not necessarily the total timeline length.

Presets can declare supported overrides with `PresetOverridesAttribute`. Only declared controls appear, and only enabled supported overrides are passed to the preset. This release declares controls for the basic Pop In/Out variants (including overshoot), Fade In/Out variants, and Pulse Scale variants. Other presets retain their defaults and duration override until their additional controls are declared. This avoids controls that silently do nothing. Settings are retained when switching animations, but unsupported overrides are ignored.

**Use Unscaled Time** runs independently of `Time.timeScale`. **Motion Preference** retains the existing project/full/reduced behavior; custom presets need to use the shared motion helpers to participate.

The Events foldout exposes **On Started**, **On Completed**, and **On Killed**. Connect a Button or other UnityEvent to `TweenPlayer.PlayFromEvent()` to play without code. Play On Start runs once when Unity calls Start; it is not a Play On Enable trigger.

Play, Pause, Resume, Restart, Rewind, Complete, and Kill remain available. Play replaces this player's active handle. Completed handles are retained for Restart/Rewind until killed. Disable and destruction release the owned handle. Multiple players can share a target, but avoid competing animations that write the same properties.

## Edit Mode preview

Preview uses manual DOTween updates and restores supported target-hierarchy state when stopped or completed. Closing the Inspector, changing the animation, Undo/Redo, assembly reload, and Play Mode transitions also stop preview. Looping presets can be stopped explicitly. Open prefab assets in Prefab Mode to preview them. Preview and runtime buttons require a single selected player.

Built-in presets support preview. Custom presets must explicitly opt in with `[PresetPreviewSupported]`. Only add it when all effects stay within the target hierarchy and properties covered by the snapshot: transforms and RectTransforms, active state, supported UI/renderer visual state, and the engine properties handled by recipe preview. It does not cover arbitrary static state, external objects, files, network activity, created/destroyed GameObjects, or arbitrary custom component fields. An unsupported custom preset still works at runtime.

## Optional recipes

Choose **Recipe** mode for an existing reusable sequence or multi-target presentation. Assign its asset and fill in the generated target fields. Missing binding slots are created automatically, while existing assignments are matched by stable binding ID. Renaming a binding keeps its assignment; replacing its ID creates a new slot. Removed slots are retained until **Remove Deleted** is pressed.

Loaded scene/prefab-stage players reconcile after project changes and before Play Mode. Unopened prefab assets reconcile when opened or inspected; the editor does not rewrite every prefab asset in the project. Multi-selection supports common player settings and recipe selection; edit each player's individual bindings separately.

Existing serialized players retain Recipe mode, recipe references, events, and targets. Newly added components default to Preset mode. Switching modes keeps both configurations. Recipe playback remains supported by the same executor as before.

## Custom animations

Create a runtime class in your own project or assembly that references Tween Helper. No package edits or recipe assets are needed. Use a unique, stable name and a public parameterless constructor (implicit for this example):

```csharp
using DG.Tweening;
using LB.TweenHelper;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[AutoRegisterPreset]
[PresetCategory("My Game/Feedback")]
[PresetOverrides(PresetOverrideFields.Strength)]
[PresetPreviewSupported]
public sealed class MyScalePulsePreset : CodePreset
{
    public override string PresetName => "MyGame.ScalePulse";
    public override string Description => "A small scale pulse for game feedback.";
    public override float DefaultDuration => 0.3f;

    public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
    {
        Vector3 originalScale = target.transform.localScale;
        float halfDuration = GetDuration(duration, options) * 0.5f;
        float peak = 1f + 0.12f * ResolveStrength(options);
        return DOTween.Sequence()
            .Append(target.transform.DOScale(originalScale * peak, halfDuration))
            .Append(target.transform.DOScale(originalScale, halfDuration))
            .WithDefaults(options, target);
    }
}
```

After compilation, the preset appears under **My Game/Feedback**. Without a category attribute, custom presets appear under **Custom**. `ITweenPreset` implementations also work; no new interface member is required.

You can still call `TweenPresetRegistry.RegisterPreset(new MyScalePulsePreset())`. Runtime-only registration appears only after that code executes. `DiscoverPresets()` finds newly loaded attributed types without clearing explicit registrations; `Refresh()` intentionally clears and rebuilds the registry. Browser catalog refresh now uses discovery.

Duplicate names retain the registry's existing overwrite warning/behavior; use unique names. Renaming or removing a selected preset leaves an unresolved name on the player until you explicitly replace it. Preserve custom classes and constructors for stripped builds, or reference and register them explicitly. Editor discovery does not prove stripped-player compatibility.

The sample `CustomPulsePreset` demonstrates this extension path. `DirectPresetDemo.prefab` and `CustomPresetDemo.prefab` can be placed in an existing scene and played without a recipe asset.

## Scope

The dropdown contains registered presets. Browser operations such as destination travel, collection choreography, and other helpers that need extra targets or arguments retain their existing recipe/code workflows. Arbitrary custom parameter editors are not inferred from custom preset fields.
