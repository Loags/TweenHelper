# Tween Recipes

Tween Recipes are reusable `ScriptableObject` timelines for common Tween Helper animation flows. A recipe stores only animation data and stable binding IDs; it never stores scene or prefab object references. A `TweenPlayer` maps those IDs to explicit objects on each instance.

## Create and edit a recipe

1. Open **Tools > Tween Helper > Recipe Editor**.
2. Choose an existing `TweenRecipe` or click **Create**.
3. Add named bindings in the left pane.
4. Search the operation palette and add nodes.
5. Reorder nodes in the timeline. Use **Then** for sequential steps and **With** for steps that start alongside the preceding **Then** step.
6. Select a node to edit its binding, duration, delay, ease, and operation parameters.
7. Select **Validate** and use any validation message to navigate to the affected node or binding.

The timeline is intentionally constrained. It represents only `Then`, `With`, and `Delay`; it is not a general graph or visual-scripting system.

## Bind and play

Add **Tween Player** to a GameObject, assign a recipe, and choose **Sync Bindings** in its Inspector. Assign every GameObject or ordered collection explicitly. Synchronization adds new recipe slots without silently deleting stale assignments, so removed slots can be reviewed before deletion.

Call `Play()` from code to receive the active handle. For an Inspector-wired UnityEvent, choose `PlayFromEvent()`; its void signature is compatible with persistent event bindings. A player owns exactly one active `TweenHandle`; playing again replaces its previous handle after the new recipe and all bindings have validated. The component also exposes `Pause`, `Resume`, `Restart`, `Rewind`, `Complete`, and `Kill`.

```csharp
[SerializeField] private TweenPlayer notificationPlayer;

public void ShowNotification() => notificationPlayer.Play();
```

Enable **Play On Start** for automatic playback. **Use Unscaled Time** is useful for pause menus and other UI that must animate while `Time.timeScale` is zero.

## Preview safely

The Recipe Editor and TweenPlayer Inspector use the same runtime executor as Play Mode. Preview first validates the complete recipe and binding map. It then captures every supported value before mutation and restores it on Stop, completion, window close, assembly reload, Undo/Redo, errors, and Editor shutdown.

When Play Mode is requested during an active preview, the first request is canceled after restoration. Press Play again to enter Play Mode with a clean scene or prefab stage. This explicit interruption avoids carrying a previewed value into Play Mode.

Preview supports scene objects and objects in Prefab Mode. It restores object values, hierarchy state, material/property-block values, and the pre-preview dirty state. Do not save or apply prefab overrides while a preview is active.

## Initial operation set

| Operation | Binding | Parameters |
| --- | --- | --- |
| Delay | None | Duration |
| Registered Preset | GameObject | Registered preset name |
| Move Local To / By | GameObject | Local position or offset |
| Rotate Local To / By | GameObject | Local Euler angles or offset |
| Scale To | GameObject | Target scale |
| Fade To | GameObject | Alpha from 0 to 1 |
| Color To | GameObject | Target color |

These operations are the stable initial slice. They reuse Tween Helper target utilities and the preset registry; recipes do not duplicate a second animation engine.

## Controlled operation expansion

The curated expansion adds representative operations in complete, reviewable families without promising full `TweenBuilder` parity.

| Family | Operations | Parameters and target requirements |
| --- | --- | --- |
| World transforms | Move World To / By, Rotate World To / By | GameObject plus world position, offset, or Euler values |
| Destination motion | Arc World To, Hop World To | GameObject, world destination, and arc/hop height |
| Gameplay feedback | Error Reject, Damage Hit, Success Confirm, Reward Reveal | GameObject; reuses Tween Helper feedback macros |
| Multi-binding UI | Page Cross Fade To | Distinct source and destination GameObject bindings plus depth scale |
| Text and values | Typewriter Reveal, Number Count To | GameObject with `TMP_Text`; integer destination and optional numeric format for counting |
| Collections | Collection Preset | Ordered Collection binding, registered preset name, and non-negative stagger delay |
| Camera and engine | Camera Field Of View To, Light Intensity To, Audio Volume To, Particle Emission Rate To | GameObject with the matching component and one validated numeric destination |

`By` operations use the target state present when the recipe is built, matching the existing Tween Helper utility path. Page Cross Fade expects authored shown baselines for both active UI targets and derives its hidden transition pose internally. Collection order is the explicit order stored on the player binding.

Editor preview capture includes the added transform, TMP text, Camera field of view, Light intensity, AudioSource volume, ParticleSystem emission multiplier, and every source/destination or collection target. The same interruption and dirty-state guarantees apply to the expanded operations.

## Samples

Seven sample assets are included in `Samples/TweenHelper Demos/Recipes`:

- **PanelMoveFade** demonstrates parallel move and fade.
- **IconScalePreset** combines a direct scale step with a registered preset.
- **MultiBindingPopup** maps panel, backdrop, and icon independently.
- **DelayedNotification** adds a finite delay before a move/fade group.
- **ProgressReward** fills a progress indicator before success feedback.
- **RewardPresentation** combines reward reveal and confirmation.
- **CollectionEntrance** applies a staggered entrance to an explicit collection.

The Animation Gallery includes focused entries for the original four assets and executes them through the shared validated recipe executor. See [Recipe workflows](RecipeWorkflows.md) for the additional samples and Progress Fill To, and [Motion preferences](MotionPreferences.md) for reduced-motion overrides.

## Validation rules

Validation completes before playback or preview can mutate a target. It reports duplicate or missing IDs, invalid Then/With placement, non-finite or out-of-range values, missing presets, stale or duplicate player bindings, empty collection elements, and targets that do not support an operation.

Required bindings are part of the recipe/player contract. TweenPlayer does not search by hierarchy name and does not silently substitute another object.

## Deliberate limits

Recipes do not support branches, conditions, loops, state machines, nested recipes, variables, expressions, callbacks, raw DOTween injection, runtime authoring, custom operation plugins, or implicit scene lookup. Use the fluent `TweenBuilder` API when a behavior falls outside the curated operation catalog.
