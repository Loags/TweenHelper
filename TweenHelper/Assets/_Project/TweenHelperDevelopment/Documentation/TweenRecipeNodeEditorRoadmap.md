# TweenRecipe Node Editor and TweenPlayer Roadmap

Status: **complete — initial workflow and controlled expansion validated**

Created: 2026-08-29

## Goal

Let users construct reusable TweenHelper animations in a clean node-style Editor window, bind them to scene/prefab objects through `TweenPlayer`, preview them, and control them at runtime without writing a custom animation driver.

The visual model directly represents TweenHelper sequencing:

- `Then` runs after the prior step;
- `With` runs alongside the prior step;
- Delay is a normal node;
- one recipe has one start and completion path.

This is not a general visual-scripting system, state machine, Animator replacement, or arbitrary branching graph.

## Pre-release principles

- No legacy aliases or migration layer are needed for unreleased formats.
- Recipe runtime is independent of Unity Pipeline and development CLI.
- Begin with a small explicit operation list.
- Use serializable classes/enums and explicit switches, not reflection, JSON schemas, managed-reference hierarchies, or code generation.
- Use UI Toolkit without experimental GraphView dependencies.
- Add infrastructure only when an implemented operation needs it.

## User workflow

1. Open **Tools > Tween Helper > Recipe Editor**.
2. Create or select a `TweenRecipe` asset.
3. Add named slots such as `Panel`, `Backdrop`, and `Icon`.
4. Add animation nodes from a searchable palette.
5. Select `Then`/`With`, a binding, and operation parameters.
6. Add `TweenPlayer`, assign the recipe, and bind concrete objects.
7. Validate and preview.
8. Enable Play On Start, invoke `Play()` from UnityEvent, or call it from code.

```csharp
[SerializeField] private TweenPlayer popupAnimation;

private void ShowPopup() => popupAnimation.Play();
private void HidePopup() => popupAnimation.Rewind();
```

## Editor interface

```text
---------------------------------------------------------------+
| Recipe | Validate | Preview | Stop | Frame All                 |
--------------+--------------------------------+----------------+
| Bindings     | Node timeline                  | Inspector      |
| Panel        | [Move Panel] -> [Fade Panel]   | Operation      |
| Backdrop     |                    |            | Binding        |
| Icon         |                    + [Scale]    | Duration/Ease  |
--------------+--------------------------------+----------------+
| Validation and preview status                                  |
---------------------------------------------------------------+
```

- Left: binding slots and searchable Add Node.
- Center: connected node cards in recipe order.
- Right: selected binding/node fields.
- Bottom: concise validation and preview status.

### Timeline rules

- Implicit Start cannot be deleted.
- Animation and Delay nodes form one ordered list.
- Each animation node selects one binding.
- Each node selects `Then` or `With`.
- Connections derive from order and placement; no arbitrary port wiring.
- Horizontal drag reorders; parallel-lane drop selects `With`.
- No cycles, branches, disconnected nodes, or nested graphs.
- Cards show operation, binding, duration, and placement.
- Invalid nodes remain editable with an error badge.

This constrained model provides node-style authoring while mapping exactly to the existing builder mental model.

### Initial usability

Include search, node label editing, duplicate/delete, standard shortcuts, Undo/Redo, stable selection, empty-state guidance, frame-all, and clickable validation messages.

Exclude minimaps, groups, comments, variables, reroute nodes, graph tabs, custom themes, and complex shortcut systems.

## Runtime data

Create `Runtime/Recipes` in the customer package.

### TweenRecipe

```csharp
[CreateAssetMenu(menuName = "Tween Helper/Tween Recipe")]
public sealed class TweenRecipe : ScriptableObject
{
    [SerializeField] private List<TweenRecipeBindingDefinition> bindings;
    [SerializeField] private List<TweenRecipeNode> nodes;

    public IReadOnlyList<TweenRecipeBindingDefinition> Bindings => bindings;
    public IReadOnlyList<TweenRecipeNode> Nodes => nodes;
}
```

Recipes store abstract slots and instructions only, never scene references.

### Bindings

```csharp
public enum TweenRecipeBindingKind
{
    GameObject,
    Collection
}
```

Definitions contain an Editor-generated ID, display name, and kind. IDs survive renaming without a global identity service. The initial operations use GameObject; Collection is activated only with the first collection operation.

### Nodes

```csharp
[Serializable]
public sealed class TweenRecipeNode
{
    [SerializeField] private string id;
    [SerializeField] private string label;
    [SerializeField] private TweenRecipePlacement placement;
    [SerializeField] private TweenRecipeOperation operation;
    [SerializeField] private string bindingId;
    [SerializeField] private float duration;
    [SerializeField] private float delay;
    [SerializeField] private Ease ease;
    [SerializeField] private TweenRecipeParameters parameters;
}
```

`TweenRecipePlacement` contains only `Then` and `With`. `TweenRecipeParameters` is a plain container for `Vector3`, `Color`, `float`, `int`, `bool`, and `string`; the Editor displays only fields used by the chosen operation.

Do not use `[SerializeReference]`, node subclasses, arbitrary dictionaries, or reflected parameter objects.

### Initial operations

- Delay;
- registered preset by name;
- Move Local To/By;
- Rotate Local To/By;
- Scale To;
- Fade To;
- Color To.

This proves useful UI/object sequences and the full workflow. Store preset name and validate it through `TweenPresetRegistry`; do not make 300 enum values.

## Operation catalog

One `TweenRecipeOperationCatalog` is shared by runtime validation/execution and Editor drawing. Explicit switches provide display name/category, binding kind, visible fields/defaults, validation, and creation of a paused `Tween`.

The catalog dispatches into existing preset/utility paths so animation behavior is not duplicated. Do not use attributes, scanning, external descriptors, plugin discovery, or generated schemas.

## TweenPlayer

```csharp
public sealed class TweenPlayer : MonoBehaviour
{
    [SerializeField] private TweenRecipe recipe;
    [SerializeField] private List<TweenPlayerBinding> bindings;
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool useUnscaledTime;

    public TweenHandle Play();
    public void Pause();
    public void Resume();
    public void Restart();
    public void Rewind();
    public void Complete();
    public void Kill();
}
```

`TweenPlayerBinding` stores recipe binding ID and either one explicit GameObject or an ordered GameObject collection.

Player contract:

- `Play` validates, kills current owned playback, builds, and plays.
- One player owns one handle.
- Disable/destroy kills owned playback only.
- The root sequence links to the player GameObject.
- Missing/incompatible bindings fail before mutation.
- Play On Start runs once from `Start`.
- Optional UnityEvents report started, normal completion, and kill.

Use one replace-current policy; do not add conflict enums initially. Never serialize callbacks inside recipes.

## Execution

`TweenRecipeExecutor`:

1. Validates recipe and player bindings.
2. Creates a paused root Sequence.
3. Resolves each node target.
4. Gets one paused child tween from the catalog.
5. Appends for `Then` or joins for `With`.
6. Applies delay, duration, and ease.
7. Links the root to the player.
8. Wraps it in `TweenHandle`.

The first node cannot be `With`. The executor composes existing tween factories rather than treating `TweenBuilder` as serialized data; this supports multiple targets without adding target-switching state to the builder.

## Validation

One `TweenRecipeValidator` returns messages tied to optional node/binding IDs. Validate names, unique IDs, first placement, operations/presets, binding kind/presence, target references, finite values, and operation-specific capabilities such as fade/color support.

Use the same validator in the window, player Inspector, and `Play()`. Do not add issue registries, content hashes, or schema services.

## Editor files

Create `Editor/Recipes`:

- `TweenRecipeEditorWindow.cs`;
- `TweenRecipeTimelineView.cs`;
- `TweenRecipeInspectorView.cs`;
- `TweenRecipePalette.cs`;
- `TweenPlayerEditor.cs`;
- `TweenRecipePreviewSession.cs`;
- one USS file.

Use `VisualElement`, `ListView`, `ScrollView`, and ordinary pointer manipulators. Auto-layout nodes from serialized order; do not serialize canvas positions initially.

## Binding synchronization

When a player changes recipe, retain matching IDs, add empty new bindings, and offer visible removal of deleted IDs. Use an explicit **Sync Bindings** button; do not silently change scenes or prefabs during import.

The Inspector shows recipe, Open Recipe, Sync Bindings, one field/list per slot, playback settings, validation, Preview/Stop outside Play Mode, and playback controls in Play Mode.

## Preview

Preview a selected scene or prefab-stage `TweenPlayer` only after full validation.

- User explicitly starts preview.
- Capture every property supported operations mutate.
- Run the same executor with Editor updates.
- Stop, close, reload, Play Mode transition, or error kills and restores.
- Never save the scene/prefab.
- Only one preview session exists.
- Reject unsupported operations before mutation.

Reuse matching state-capture helpers, but add one recipe session instead of refactoring all Browser preview code. If safe Edit Mode preview is not ready, ship runtime first and disable Preview with a clear explanation rather than leaving dirty state.

## Implementation phases

### Phase 1 — Model and validation

Add data types, initial catalog, validator, and asset menu. Confirm serialize, duplicate, reload, and IDs. Exit: sequential/parallel recipes are representable and Runtime has no Editor/CLI dependency.

Completion (2026-08-30): **complete**. Added the plain serializable recipe/binding/node model, explicit initial operation catalog, asset menu, and validator. Unity compilation passed. A save/reload/duplicate probe preserved one binding, two Then/With nodes, and stable IDs; both source and duplicate validated. Runtime assembly references remain only DOTween Modules and TextMeshPro, and all new Unity files have `.meta` files. No editor, CLI, Pipeline, managed-reference, reflection, schema, migration, or graph dependency was introduced.

### Phase 2 — Executor

Implement tween dispatch, append/join, settings, linking, and pre-mutation validation. Exit: multi-binding recipes match hand-written behavior and lifecycle operations leak no state.

Completion (2026-08-30): **complete**. Added explicit target binding data, binding/capability validation, initial-operation dispatch through existing preset and target utilities, and a paused non-auto-kill root sequence with append/join, delay, ease, update mode, target, and owner linking. An invalid-binding probe was rejected before target mutation. An isolated two-binding Then/With probe produced the expected halfway and final transform values and rewound exactly; Play Mode lifecycle checks covered play, pause, resume, rewind, restart, complete-retained, and kill with zero active-tween delta after explicit cleanup. Unity compilation passed and all new files have `.meta` files. No serialized builder model, operation plugins, reflection schema, or alternate animation engine was added.

### Phase 3 — TweenPlayer

Add bindings, lifecycle controls, play-on-start, events, and minimal Inspector. Exit: prefabs reuse recipes, missing bindings fail early, and ownership is isolated.

Completion (2026-08-30): **complete**. Added `TweenPlayer` with explicit serialized bindings, one replace-current handle, Play On Start, scaled/unscaled playback, start/completion/kill events, validation, full lifecycle controls, and disable/destroy cleanup, plus a minimal UI Toolkit Inspector. Play Mode probes confirmed independent ownership across two players sharing one recipe, replacement cleanup, completion retention for rewind/restart, disable/destroy cleanup, event counts, and zero active-tween delta after cleanup. A temporary prefab round-trip confirmed two instances share the recipe asset while retaining distinct child-object bindings; both validated, and the temporary assets were removed. Missing bindings are rejected by the shared preflight before replacement or mutation. Unity compilation passed and all new files/folders have `.meta` files.

### Phase 4 — Node editor

Build window, asset selection, bindings, palette, timeline/connectors, and selected-node fields. Exit: users create valid recipes without raw Inspector editing; Then/With are visually clear; Undo works.

Completion (2026-08-30): **complete**. Added the UI Toolkit Recipe Editor window, asset creation/selection, named binding list, searchable nine-operation palette, constrained auto-layout timeline, derived sequential connectors and stacked With lanes, selected binding/node fields, operation-aware parameter fields/defaults, clickable validation messages, node ordering, and Undo-backed mutations. Expanded the player Inspector with Open Recipe, explicit Sync Bindings, per-slot target fields, retained/deleted binding visibility, and explicit removal. A temporary asset/editor probe confirmed all three panes, the complete initial palette, stable IDs, one visual parallel group for a Then/With pair, valid authoring output, Undo/Redo state restoration, and ID-based player synchronization. Unity compilation passed, no temporary assets remain, and all new Editor files have `.meta` files. No GraphView, free ports, canvas positions, branches, or Editor dependency entered Runtime.

### Phase 5 — Editing polish

Add drag reorder/parallel placement, duplicate/delete, shortcuts, error badges, empty states, selection restoration, frame-all, and one restrained style pass matching current TweenHelper tools.

Completion (2026-08-30): **complete**. Added pointer-driven node reorder with parallel-lane drop semantics, while forcing the first node and Delay nodes to Then; retained arrow ordering for accessibility. Duplicate/delete actions, Ctrl/Cmd-D, Delete/Backspace, Ctrl/Cmd-S, and frame-all shortcuts are implemented alongside stable ID-based selection, node error badges, empty guidance, validation navigation, searchable filtering, and the restrained dark/cyan/violet style used by current Tween Helper tools. Editor probes verified With placement, Delay/first-node normalization, selection retention, shortcut duplicate/delete, one-result Fade search, invalid-node badging, and viewport reset. Unity compilation is clean, temporary assets were removed, and existing `.meta` files remain valid.

### Phase 6 — Safe preview

Add preflight, mutation capture, Editor playback, and restoration on every stop path. Exit: repeated preview leaves scene/prefab unchanged and uses the runtime executor.

Completion (2026-08-30): **complete**. Added one singleton Edit Mode preview session driven by DOTween's Editor preview loop and built exclusively through `TweenRecipeExecutor`. Full player validation completes before capture or tween construction. Bound hierarchies receive explicit transform, active/enabled, RectTransform, CanvasGroup, Graphic, TMP text, Slider, SpriteRenderer, renderer material/property-block, and dirty-state snapshots plus an isolated Undo safety group. Completion, explicit Stop, invalid preflight, Undo/Redo, window/Inspector close, script reload, Play Mode request, errors, and Prefab Mode all stop and restore. Scene probes showed live position/alpha/color changes and exact clean restoration; renderer property blocks restored exactly. A Prefab Mode probe restored all values, cleared stage dirtiness, and left prefab bytes unchanged. Because Unity can copy an in-flight Edit Mode value before its transition callback, pressing Play during preview safely restores and cancels that first transition with guidance to press Play again; the next Play request enters normally. Unity compilation and Console checks passed, no preview or temporary assets remain, and the preview file has a valid `.meta`.

Follow-up safety validation captures the TweenPlayer owner as well as every bound hierarchy. Components created only for preview are removed before the Undo snapshot is restored. An isolated Page Cross Fade probe showed both `UIAnimationStateCache` components during playback, then restored the original component topology, alpha, scale, clean-scene state, zero active tweens, and no dangling-component warning.

### Phase 7 — Samples/docs

Add panel Move/Fade, icon Scale/Preset, multi-binding popup, and delayed notification recipes. Add `TweenRecipes.md`, README/API/setup guidance, focused Gallery examples, and internal review coverage. Do not enumerate all combinations.

Completion (2026-08-30): **complete**. Added four scene-reference-free sample assets for panel Move/Fade, icon Scale/Preset, a three-binding popup, and a delayed notification. Added focused package, API, installation, sample, and `TweenRecipes.md` guidance without enumerating arbitrary combinations. Four new Gallery entries resolve a small sample-only Resources library and execute the assets through the same validated runtime executor; no Gallery scene rebuild or scene reference was required. Added menu-driven internal review coverage for asset validation, stable IDs, scene-reference independence, Gallery entries, and library wiring; it passed for all four samples and nine initial operations. Unity compilation passed and every new sample, documentation, script, resource, and folder asset has a `.meta` file. Runtime remains independent of development assemblies, and no new animation engine or authoring abstraction was introduced.

A final Play Mode Gallery probe confirmed all four recipe entries returned playing handles through the shared executor and the existing Gallery reset path left zero active tweens.

### Phase 8 — Controlled expansion

After the initial workflow is accepted, add in slices: world transforms, destination motion, gameplay feedback, multi-binding UI, text/value, collections, then camera/engine properties. Each slice includes catalog dispatch, validation, editor fields, preview capture, docs, and review coverage together. Do not promise every builder overload.

Completion (2026-08-30): **complete**. Expanded the explicit catalog from 9 to 27 operations in the documented order:

- World transforms: Move World To/By and Rotate World To/By.
- Destination motion: Arc World To and Hop World To through existing destination-motion builders.
- Gameplay feedback: Error Reject, Damage Hit, Success Confirm, and Reward Reveal through existing feedback macros.
- Multi-binding UI: Page Cross Fade To with one explicit, validated destination binding ID.
- Text/value: Typewriter Reveal and Number Count To with TMP capability and format validation.
- Collections: one ordered Collection Preset operation with registered-preset capability checks and non-negative stagger.
- Camera/engine: Camera Field Of View To, Light Intensity To, Audio Volume To, and Particle Emission Rate To.

Each slice has explicit dispatch, category/field metadata, value and component validation, palette/Inspector support, executor coverage, documentation, and internal catalog review coverage. Isolated preview-scene probes completed and rewound every family. The camera/engine Editor-preview probe visibly changed all four values, then restored Camera 60, Light 1, Audio 1, Particle 10 and the clean scene state exactly. The review command passed 4 sample assets, 4 Gallery entries, all 18 expanded operations, and all 27 total operations. Compilation is clean apart from the Unity Pipeline server's environment warning. No GraphView, arbitrary ports, nested recipes, runtime schema, plugin system, migration layer, or full-builder-parity claim was added.

Final audit confirmed TweenBuilder-backed operations receive the authored node ease before building, invalid capability data remains a preflight error, Runtime recipe sources have no Editor/Pipeline/CLI/GraphView/managed-reference dependency, and no probe object, preview session, or tween remained active.

## Validation matrix

Check empty/one-node recipes, Then/With, multiple bindings, missing/wrong/duplicate targets, invalid presets/values, all player lifecycle controls, replacement Play, scaled/unscaled time, asset/node duplication, Undo/Redo, reload, scene/prefab stage, every preview interruption, dirty-state preservation, and Runtime assembly independence.

Use Unity Editor/Console validation when available. Do not run a batch build unless explicitly requested.

## Non-goals

- branches, conditions, node loops, or state machines;
- nested recipes;
- arbitrary callbacks, expressions, or raw DOTween;
- runtime authoring;
- custom operation plugins;
- hierarchy-name lookup or implicit binding;
- CLI authoring;
- GraphView;
- minimaps, groups, comments, reroutes, variables, or graph debugging;
- source generation;
- migration support for unreleased formats.

Root loops can be considered only after finite playback, rewind, disable, and preview are stable.

## Done criteria

- A user can create, validate, bind, preview, and play a useful recipe without animation code.
- The UI represents only linear/parallel sequence semantics.
- Recipes have no scene references; players bind explicitly and own one handle.
- Runtime and preview share the executor.
- Preview restores every supported mutation path.
- Initial operations are documented and visually reviewed.
- Runtime remains independent of Editor, Pipeline, CLI, and development tooling.
- Every new Unity file has a `.meta` file.
- No compatibility layer or general visual-scripting framework is introduced.