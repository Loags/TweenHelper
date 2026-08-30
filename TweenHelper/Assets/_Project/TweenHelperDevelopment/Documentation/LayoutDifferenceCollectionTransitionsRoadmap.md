# Layout-Difference Collection Transitions Roadmap

Status: **implemented and validated on 2026-08-30**

Created: 2026-08-29

## Goal

Animate existing UI children smoothly when a Unity layout changes their positions. This covers inventory sorting, leaderboard updates, responsive grids, and list-to-grid changes that currently snap instantly.

The feature transitions between two valid authored layouts. It does not replace `LayoutGroup`, calculate layouts, or manage application data.

## Existing foundation

Reuse `TweenHandle`, `TweenOptions`, target-linked DOTween sequences, and current collection validation. Unity's layout system remains the source of final positions; decorative topology utilities are not involved.

## Public workflow

```csharp
CollectionLayoutSnapshot before = container.CaptureCollectionLayout();

items.Sort(CompareByRarity);
ApplySiblingOrder(items);

TweenHandle handle = container.TweenCollectionLayoutFrom(before, duration: 0.35f);
```

## Initial API

```csharp
public static CollectionLayoutSnapshot CaptureCollectionLayout(this RectTransform container);

public static TweenHandle TweenCollectionLayoutFrom(
    this RectTransform container,
    CollectionLayoutSnapshot snapshot,
    float? duration = null,
    TweenOptions options = default);
```

Use only these methods. Sorting, filtering, and hierarchy mutation remain caller-owned. `CollectionLayoutSnapshot` is an opaque, short-lived public value; it is not serialized or stored on a component.

## Initial scope

Support:

- active direct `RectTransform` children;
- unchanged membership between capture and playback;
- sibling-order and layout-setting changes;
- position and optional layout-produced scale changes;
- horizontal, vertical, and grid layout groups;
- one transition owned by the container;
- scaled and unscaled time through `TweenOptions`.

Exclude initially:

- inserted, removed, destroyed, reparented, or nested children;
- cross-container transfers;
- automatic filtering or sorting;
- world-space transforms;
- entrance and exit effects;
- hierarchy rollback on rewind;
- callbacks that let TweenHelper perform the mutation;
- continuously changing layouts.

The unchanged-membership rule keeps the first implementation predictable. Insert/remove support should be a later roadmap with explicit terminal states.

## Implementation design

Add under `Runtime/Stagger`:

- `CollectionLayoutSnapshot.cs`;
- `CollectionLayoutTransitionUtility.cs`;
- capture and playback extensions in the most relevant existing extension file.

Snapshot data:

- container instance;
- active direct-child references;
- child world positions and local scales;
- captured sibling order for validation and diagnostics.

Playback:

1. Validate the snapshot and child set before mutation.
2. Force canvas and layout rebuilding.
3. Capture final anchored positions and scales.
4. Temporarily disable the single enabled `LayoutGroup`.
5. Convert captured world positions to container-local positions.
6. Put children at the captured visual state.
7. Join position and scale tweens into one sequence.
8. Re-enable the layout group and force a final rebuild on completion or kill.

Keep layout ownership inside this utility. Do not add helper components or a general layout framework.

## Lifecycle contract

- Normal completion settles at the new Unity-authored layout.
- Early kill settles at the new layout and restores the layout group immediately.
- Destroying the container kills the target-linked sequence.
- Starting a replacement transition first kills and settles the current transition.
- Rewind may visually return to captured positions while active but does not undo sibling order or application data.
- If reliable rewind conflicts with layout ownership, mark rewind unsupported instead of implementing hierarchy rollback.
- All validation completes before any child or layout component is changed.

Throw a clear exception for a null/mismatched snapshot, changed membership, reparented child, invalid duration, or multiple enabled layout groups. Empty containers use the established completed-empty-handle behavior.

## Discovery

Add one Browser entry and two Gallery/review configurations:

1. Reorder a numbered vertical list.
2. Change a numbered grid from three columns to two.

Update `StaggeredCollections.md`, `API.md`, the semantic operation index if retained, Browser catalog/preview/snippets, Gallery catalog/player, and internal review/reset logic. Do not create one discovery entry per layout type.

## Phases

### Phase 1 — Runtime

- [x] Add snapshot, extensions, transition utility, and validation.
- [x] Implement completion and kill settlement.
- [x] Validate list reorder and grid column change.

Exit: both examples animate without snapping, layout ownership is restored, and no persistent state is created.

Evidence: Unity 6000.5.2f1 Play Mode checks confirmed captured starts and authored-layout settlement for vertical-list reorder and grid three-to-two-column changes. Completion and early kill both restored layout ownership, and the implementation creates no persistent scene or component state.

### Phase 2 — Discovery and docs

- [x] Add Browser, Gallery, and review examples.
- [x] Add API snippets and lifecycle documentation.
- [x] Recalculate affected catalog counts.

Exit: preview, snippets, and runtime use the same API; repeated fixture reset is exact.

Evidence: Browser, Gallery, and review all use CaptureCollectionLayout followed by TweenCollectionLayoutFrom. Play Mode fixture checks restored list order, layout-group ownership, and the grid's three-column setting exactly. Final totals are Browser 461, Gallery 419, review 610, review coverage 212, and lifecycle coverage 177.

### Phase 3 — Validation

- [x] Check empty and one-child containers.
- [x] Check horizontal, vertical, and grid layout groups.
- [x] Check reversal and partial reorder.
- [x] Check scaled and unscaled playback.
- [x] Check completion, kill, restart, and replacement playback.
- [x] Check target destruction.
- [x] Check membership and invalid-duration failures before mutation.
- [x] Inspect Unity compilation and Console output.

Do not add inserted/removed-child support during validation.

Evidence: temporary Play Mode fixtures covered every item above, including scale playback, replacement-handle ownership, real frame-delayed target destruction, and unchanged state after validation failures. Unity rejected adding a second built-in layout group to one object, as expected from its component restriction; the multiple-enabled-group guard was therefore verified statically. A final recompile completed with no errors, the Editor was stopped and ready, and a fresh Console follow returned no errors. No batch build or automated tests were run.

## Done criteria

- [x] The two-method API is implemented and documented.
- [x] Direct-child reorders and layout-setting changes animate smoothly.
- [x] Layout ownership is restored after completion, kill, error, replacement playback, and target destruction.
- [x] Browser, Gallery, review, and runtime behavior agree.
- [x] Every new Unity file has a .meta file.
- [x] No legacy aliases, serialized snapshots, helper components, or general abstractions are introduced.
