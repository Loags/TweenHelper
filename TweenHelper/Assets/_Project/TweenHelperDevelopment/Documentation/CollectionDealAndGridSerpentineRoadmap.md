# Collection Deal and Grid Serpentine Roadmap

Status: **complete — implemented and validated**

Created: 2026-08-29

Completed: 2026-08-30

## Goal

Add two focused collection features:

1. Deal existing items from one shared stack into authored positions and gather them back.
2. Animate grid items in a snake-like row or column traversal.

Both features reuse `StaggerRecipeExtensions`, `TweenStaggerBuilder`, `StaggerDelayUtility`, `TweenOptions`, and current lifecycle behavior. Do not create another collection builder, generic path framework, or layout system.

## Collection Deal In and Out

### Behavior

Deal In places targets at a shared origin with restrained relative rotation and scale, then moves them to captured authored states in stagger order. Deal Out moves authored targets to a shared destination in reverse stagger order and leaves them stacked there on normal completion.

Use cases include cards, reward rows, inventories, shops, drafting, quests, and results.

### API

```csharp
public static TweenHandle CollectionDealIn(
    this IEnumerable<GameObject> targets,
    GameObject owner,
    Vector3 origin,
    StaggerOrder order = StaggerOrder.FirstToLast,
    float rotationAccent = 8f,
    float startScale = 0.9f,
    float duration = 0.45f,
    float interval = 0.06f,
    bool local = true,
    TweenOptions options = default);

public static TweenHandle CollectionDealOut(
    this IEnumerable<GameObject> targets,
    GameObject owner,
    Vector3 destination,
    StaggerOrder order = StaggerOrder.LastToFirst,
    float rotationAccent = 8f,
    float endScale = 0.9f,
    float duration = 0.4f,
    float interval = 0.05f,
    bool local = true,
    TweenOptions options = default);
```

Add matching `IEnumerable<Component>` overloads because the current collection API supports both forms. Do not add directional aliases or separate UI/world names; `local` already selects the coordinate mode.

### Implementation

Add `CollectionDealUtility.cs` under `Runtime/Stagger`.

For each target:

- capture position, local rotation, and local scale when playback begins;
- calculate delays with existing `StaggerOrder` logic;
- derive a small centered signed Z-rotation from stagger rank;
- join position, rotation, and scale tweens;
- reach the exact authored state after Deal In;
- reach the exact shared destination pose after Deal Out.

Use one simple centered rotation distribution. Do not add random paths, control points, physics, automatic sibling ordering, or card-layout calculation.

### Lifecycle

Deal In:

- completion leaves authored states;
- rewind returns to the origin pose;
- early kill restores authored states.

Deal Out:

- completion leaves the destination pose;
- rewind restores authored states;
- early kill restores authored states.

Neither operation changes active state, alpha, parent, sibling order, or application data. Callers decide whether dealt-out items are disabled or destroyed.

Validate targets/owner with existing rules and require finite non-negative duration, interval, and scale plus finite rotation. Empty collections follow existing completed-empty behavior.

## Grid Serpentine

### Behavior

Assign stagger ranks through alternating rows or columns:

```text
1 -> 2 -> 3
          |
6 <- 5 <- 4
|
7 -> 8 -> 9
```

The algorithm changes delays only. Child animation continues to use the existing `PopInFadePreset` recipe pattern.

### API

```csharp
public enum GridSerpentineDirection
{
    RowsFromTopLeft,
    RowsFromTopRight,
    RowsFromBottomLeft,
    RowsFromBottomRight,
    ColumnsFromTopLeft,
    ColumnsFromTopRight,
    ColumnsFromBottomLeft,
    ColumnsFromBottomRight
}

public static TweenHandle GridSerpentine(
    this IEnumerable<GameObject> targets,
    GameObject owner,
    int columns,
    GridSerpentineDirection direction = GridSerpentineDirection.RowsFromTopLeft,
    float duration = 0.32f,
    float interval = 0.055f,
    TweenOptions options = default);
```

Add the matching Component overload. Do not add eight methods or a general traversal object.

### Ordering

Keep `CalculateSerpentineRanks` private in `StaggerRecipeExtensions` unless another implemented feature immediately needs it.

1. Calculate rows from count and columns.
2. Enumerate real cells only.
3. Reverse every second row or column.
4. Apply start-corner direction.
5. Return one unique rank for each source index.

Incomplete final rows are mandatory: missing cells consume no rank and must not reorder real items. Do not modify `StaggerOrder`; serpentine is two-dimensional.

## Files

Runtime:

- modify `Runtime/Stagger/StaggerRecipeExtensions.cs`;
- add `Runtime/Stagger/GridSerpentineDirection.cs`;
- add `Runtime/Stagger/CollectionDealUtility.cs`;
- add new `.meta` files.

Discovery/docs:

- `StaggeredCollections.md`, `API.md`, and semantic index if retained;
- Browser catalog, snippets, controls, and preview;
- Gallery catalog, snippets, and player;
- internal review catalog and fixture reset.

Do not add assemblies or move existing collection files.

## Phases

### Phase 1 — Serpentine

- Add enum, rank calculation, and both target overloads.
- Validate all directions and incomplete grids.

Exit: every direction visits each target once; a seven-item, three-column grid is correct; existing recipes are unchanged.

Completion: **passed**. All eight seven-item/three-column rank maps matched the expected order, each returned ranks `0..6` exactly once, and empty/one-item inputs were verified.

### Phase 2 — Deal

- Add utility and overloads.
- Implement lazy capture and exact settlement.
- Check UI/local and world coordinates.

Exit: Deal In/Out reach exact endpoints, early kill restores state, and replay accumulates no transform changes.

Completion: **passed**. UI/local and world endpoints, lazy capture, completion, rewind, restart, repeat playback, early kill, zero duration, Component parity, owner destruction, and even-Yoyo settlement were verified.

### Phase 3 — Discovery

Add Browser/Gallery entries for Deal In, Deal Out, and Serpentine with a direction control. Add internal coverage for all eight directions, an incomplete grid, UI Deal In/Out, and one world Deal case when its path differs. Number fixtures so order is visible. Do not create one customer-facing row per enum value.

Completion: **passed**. Browser, Gallery, and review routing use one public Serpentine row with an eight-value direction control. Numbered review fixtures cover every direction, the incomplete grid, UI Deal In/Out, and world Deal Out.

### Phase 4 — Documentation and validation

Document defaults, coordinate spaces, and terminal states. Verify empty/one-item collections, completion, rewind, restart, early kill, owner destruction, repeat playback, catalog totals, Unity compilation, and Console output when available.

Completion: **passed**. Customer/API/generated documentation now records the 418-entry Gallery, 460-entry Browser, and 608-entry review catalog. Existing validators passed with 418 Gallery entries, 608 unique review IDs, 210/210 coverage IDs, and 175 lifecycle-affected IDs. Unity finished stopped and ready with no compilation errors or new Console errors.

## Done criteria

- [x] All three operations use the established collection surface.
- [x] No new builder or generic path system exists.
- [x] Every direction and incomplete-grid behavior is internally covered.
- [x] Deal lifecycle behavior is explicit and visually validated.
- [x] Browser, Gallery, docs, review, and runtime agree.
- [x] Every new Unity file has a `.meta` file.
- [x] No legacy aliases are retained.

## Completion record

- Phase 1 added the compact eight-value `GridSerpentineDirection` enum, private rank calculation, and GameObject/Component overloads. Missing cells consume no rank.
- Phase 2 added one internal `CollectionDealUtility` and exact Deal In/Out lifecycle settlement without changing alpha, active state, parent, sibling order, or application data.
- Phase 3 added snippets, controls, preview/player routing, reset-compatible numbered fixtures, and review coverage without multiplying customer-facing rows by direction.
- Phase 4 updated collection/API/install/sample/generated documentation and ran Unity `6000.5.2f1` validation. Gallery assets, review coverage, lifecycle coverage, Play Mode owner destruction, local/world motion, and visual fixtures passed.
- Unity generated and retained `.meta` files for both new runtime files. No batch build or new automated tests were run, as required.
