# Staggered collections

`TweenStaggerBuilder` schedules one finite tween per collection item and returns one `TweenHandle` for the complete group. It supports typed presets, dynamic preset names, custom DOTween factories, five ordering modes, custom delay maps, root lifecycle options, and twenty-three gallery-facing collection examples.

Collection recipes are orchestration helpers, not `ITweenPreset` implementations. The built-in preset registry therefore remains at 300 entries.

## Create a stagger group

Pass the targets and an explicit owner. The target enumerable is copied immediately, so later collection changes do not alter an already configured builder.

```csharp
TweenHandle handle = cards.TweenStagger(panelRoot)
    .Preset<PopInFadePreset>(0.32f)
    .DelayBetween(0.06f)
    .Order(StaggerOrder.FirstToLast)
    .Play();
```

The owner is assigned as the root sequence's DOTween target and link. Destroying it cleans up the whole group. Retain the returned handle when normal teardown should pause, rewind, complete, or kill the sequence explicitly.

Both `IEnumerable<GameObject>` and `IEnumerable<Component>` are supported. A component can also be supplied as the owner.

## Select the child animation

Use a typed preset whenever its type is known:

```csharp
items.TweenStagger(owner)
    .Preset<PulseScalePreset>(0.3f, TweenOptions.WithStrength(1.2f))
    .Play();
```

Use a name only for genuinely dynamic data:

```csharp
items.TweenStagger(owner)
    .PresetByName(configuredPresetName, 0.3f)
    .Play();
```

Or return a raw DOTween tween for each item. The index always refers to the original source position, regardless of playback order.

```csharp
items.TweenStagger(owner)
    .Animate((item, index) => item.transform
        .DOPunchScale(Vector3.one * 0.1f, 0.25f))
    .Play();
```

A later `Preset`, `PresetByName`, or `Animate` call replaces the previous child-animation source.

## Ordering and delays

`DelayBetween` sets the spacing between ordered start times.

| Order | Behavior |
| --- | --- |
| `FirstToLast` | Source index 0 starts first. |
| `LastToFirst` | The final source index starts first. |
| `FromCenter` | The center item, or center pair for even counts, starts first. |
| `ToCenter` | Both edges start first and converge on the center. |
| `Random` | Uses a local `System.Random` shuffle and never changes `UnityEngine.Random` state. |

Seed random ordering when playback must be repeatable:

```csharp
items.TweenStagger(owner)
    .Preset<PulseScalePreset>()
    .Order(StaggerOrder.Random)
    .Seed(1729)
    .DelayBetween(0.08f)
    .Play();
```

`DelayBy` supplies an absolute delay for every original source index. It replaces ordered scheduling. A later `Order` or `DelayBetween` call switches back to ordered scheduling, so the last scheduling style configured wins.

```csharp
gridItems.TweenStagger(gridRoot)
    .Preset<PulseScalePreset>()
    .DelayBy((item, index) => Vector2.Distance(GridPosition(index), rippleOrigin) * 0.06f)
    .Play();
```

Delay values must be finite and non-negative.

## Root lifecycle options

The root sequence uses linear easing so its insertion timeline is not warped. Child presets retain their own easing and finite loop behavior.

```csharp
TweenHandle handle = items.TweenStagger(owner)
    .Preset<PulseScaleSoftPreset>(0.25f)
    .DelayBetween(0.1f)
    .WithDelay(0.2f)
    .WithTailDelay(0.15f)
    .WithUpdate(UpdateType.Normal, unscaledTime: true)
    .WithLoops(-1)
    .WithId("menu-loading")
    .OnKill(ReleaseState)
    .Play();
```

`WithLoops` loops the complete group. `WithTailDelay` creates a pause after the last child, which is especially useful before a root loop restarts. The returned `TweenHandle` supports the same pause, resume, restart, rewind, complete, kill, callback, and await operations as a single-target animation.

DOTween sequences cannot contain infinite child tweens. If a preset or custom factory creates one, the builder kills the partial group and throws an exception explaining that the loop belongs on the root. `LoadingDots` follows this rule by using finite `PulseScaleSoftPreset` children and an infinitely looping root.

## Ready-to-play recipes

```csharp
items.ListStaggerIn(owner);                                  // PopInFade, first to last
items.ListStaggerOut(owner);                                 // PopOutFade, last to first
items.GridWave(owner, columns: 4, direction: GridWaveDirection.LeftToRight);
items.GridRipple(owner, columns: 4);                         // Defaults to the center origin
items.GridDiagonalWave(owner, columns: 4, direction: GridDiagonalDirection.TopLeftToBottomRight);
items.GridSpiral(owner, columns: 4, direction: GridSpiralDirection.OutsideInClockwise);
items.GridCheckerboard(owner, columns: 4);
items.GridSerpentine(owner, columns: 4, direction: GridSerpentineDirection.RowsFromTopLeft);
items.CollectionDealIn(owner, origin);
items.CollectionDealOut(owner, destination);
items.CollectionBurstIn(owner, origin);
items.CollectionBurstOut(owner, origin);
items.CollectionGatherTo(owner, destination);
dots.LoadingDots(owner);                                     // Finite pulses, looping root
```

Grid collections are interpreted in row-major order. `GridWave` supports left-to-right, right-to-left, top-to-bottom, and bottom-to-top directions. `GridRipple` accepts an optional source `originIndex` and otherwise chooses a centered item. Diagonal Wave supports all four corners. Spiral supports clockwise/counter-clockwise and outside-in/inside-out traversal. Checkerboard accepts an `inverted` flag to swap its two phases. Serpentine alternates each row or column and supports `RowsFromTopLeft`, `RowsFromTopRight`, `RowsFromBottomLeft`, `RowsFromBottomRight`, `ColumnsFromTopLeft`, `ColumnsFromTopRight`, `ColumnsFromBottomLeft`, and `ColumnsFromBottomRight`. Incomplete final rows skip missing cells, so every real target receives one unique stagger rank.

`CollectionDealIn` defaults to first-to-last order, an `8` degree rotation accent, `0.9` start scale, `0.45` seconds, and a `0.06` second interval. `CollectionDealOut` defaults to last-to-first order, an `8` degree rotation accent, `0.9` end scale, `0.4` seconds, and a `0.05` second interval. Both default to local coordinates. Local `RectTransform` positions use `anchoredPosition3D`, other local targets use `localPosition`, and `local: false` uses world position. Rotation and scale remain local.

| Deal operation | Normal completion | Rewind | Early kill |
| --- | --- | --- | --- |
| Deal In | Exact captured authored position, rotation, and scale | Shared origin, centered relative Z rotation, and authored scale multiplied by `startScale` | Exact captured authored state |
| Deal Out | Shared destination, centered relative Z rotation, and authored scale multiplied by `endScale` | Exact captured authored state | Exact captured authored state |

Deal state is captured when playback begins. Restarting or replaying the same handle uses that captured baseline and does not accumulate position, rotation, or scale. Neither Deal operation changes active state, alpha, parent, sibling order, or application data.

Burst In starts all items at one origin and restores their authored positions, scale, rotation, and alpha. Burst Out scatters radially from an origin; Gather To converges on a destination. Burst Out and Gather To finish at zero scale and supported alpha. Burst Out chooses a default distance of `120` canvas units for locally animated `RectTransform` items and `1.2` units for ordinary or world-space motion. Set `local: false` for world positions; the default local mode uses `localPosition`, or `anchoredPosition3D` for `RectTransform` targets.

Every recipe returns its active `TweenHandle` and accepts duration, stagger interval, and `TweenOptions` overrides. `LoadingDots` also accepts the pause between complete cycles. Strength scales the spatial distance and deformation without moving Burst In or Gather To away from their exact requested endpoint.

## Animate Unity layout changes

Capture a container before the caller changes sibling order or layout settings, then animate from that captured visual state to Unity's rebuilt layout:

```csharp
CollectionLayoutSnapshot before = container.CaptureCollectionLayout();

items.Sort(CompareByRarity);
ApplySiblingOrder(items);

TweenHandle handle = container.TweenCollectionLayoutFrom(before, 0.35f);
```

The same two-method workflow handles responsive grid changes:

```csharp
CollectionLayoutSnapshot before = gridRoot.CaptureCollectionLayout();
grid.constraintCount = 2;
gridRoot.TweenCollectionLayoutFrom(before, options: TweenOptions.WithUnscaledTime());
```

`CollectionLayoutSnapshot` is opaque, short-lived, and not serializable. Playback supports active direct `RectTransform` children with unchanged membership under one enabled `HorizontalLayoutGroup`, `VerticalLayoutGroup`, or `GridLayoutGroup`. Sibling-order and layout-setting changes are caller-owned; TweenHelper only animates the resulting anchored positions and changed local scales.

Playback validates the snapshot, container, active child set, parent relationships, duration, speed-based option, and enabled layout group before changing layout ownership. It then rebuilds the final Unity-authored layout, temporarily disables that layout group, and links one sequence to the container. Normal completion and early kill both settle the children at the new layout, re-enable the group, and force a final rebuild. Starting another layout transition on the same container first kills and settles the current one. Destroying the container kills the linked sequence; no persistent component or serialized state is created.

Rewind may show the captured positions while the sequence remains active, but it does not restore sibling order, layout settings, or application data. Restart is supported while that rewound sequence is still active. Insertion, removal, destroyed or reparented children, nested transitions, cross-container moves, automatic sorting/filtering, world-space motion, and continuously changing layouts are outside the initial scope.

## Validation and errors

- Empty collections return an inactive `TweenHandle` and log a warning.
- Null targets and duplicate target references are rejected when the collection is copied.
- An owner or item destroyed between configuration and `Build` is rejected before child tweens are created.
- Missing presets, incompatible targets, invalid delays, invalid column counts, invalid grid directions, invalid ripple origins, and invalid Deal parameters throw descriptive exceptions.
- Layout transitions reject a null or mismatched snapshot, changed active membership, reparented children, invalid duration, speed-based timing, unsupported layout groups, and multiple enabled layout groups before taking ownership. Empty matching containers return an inactive completed handle.
- Building without first selecting a preset or custom factory is rejected.
- Killing a preset-based stagger group follows normal `TweenHandle` semantics and does not restore arbitrary target state automatically.
- Deal In and Deal Out restore captured authored transforms on interrupted kill. Deal In rewinds to its origin pose, while Deal Out rewinds to the authored pose. Burst In, Burst Out, and Gather To restore their captured item states on interrupted kill and rewind. Normal Burst Out and Gather To completion intentionally leaves their hidden endpoint.

The explicit owner is the root tween's single lifetime owner. Collection items are required secondary participants and must remain alive until the recipe finishes or is killed; destroying an item does not independently cancel the root. Deal recipes write position, scale, and local rotation without changing alpha. Other spatial recipes may also write supported alpha, so callers should not overlap other writers on those channels.

For replayable previews or pooled UI, capture the desired target state before playback and restore it before starting the next group.

## Animation Gallery

Open the **Collections** category to compare all twenty-three examples and change order, wave direction, diagonal, spiral, serpentine direction, checkerboard phase, and layout-change configuration while the matching C# call updates live.

## Expanded topology recipes

```csharp
grid.GridConcentricIn(owner, columns: 4);
grid.GridConcentricOut(owner, columns: 4);
grid.GridQuadrantSweep(owner, columns: 4);
list.ListAccordion(owner);
items.CollectionOrbitIn(owner, center);
items.CollectionOrbitOut(owner, center);
ring.LoadingRing(owner);
strip.LoadingRibbon(owner);
```

Concentric In schedules outer rings toward the center; Concentric Out reverses that topology. Quadrant Sweep supports clockwise and counter-clockwise corner starts through `GridQuadrantSweepDirection`. Accordion unfolds captured list positions from their shared center. Orbit In restores authored endpoints after a spiral entrance, while Orbit Out finishes at its faded orbit endpoint. Loading Ring and Ribbon are infinite root loops and restore all captured item state when killed.

The gallery now contains twenty-three collection examples: the twenty-two existing recipes plus the list-reorder/grid-column layout transition.
