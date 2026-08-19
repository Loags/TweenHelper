# Staggered collections

`TweenStaggerBuilder` schedules one finite tween per collection item and returns one `TweenHandle` for the complete group. It supports typed presets, dynamic preset names, custom DOTween factories, five ordering modes, custom delay maps, root lifecycle options, and nineteen gallery-facing recipes.

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
items.CollectionBurstIn(owner, origin);
items.CollectionBurstOut(owner, origin);
items.CollectionGatherTo(owner, destination);
dots.LoadingDots(owner);                                     // Finite pulses, looping root
```

Grid collections are interpreted in row-major order. `GridWave` supports left-to-right, right-to-left, top-to-bottom, and bottom-to-top directions. `GridRipple` accepts an optional source `originIndex` and otherwise chooses a centered item. Diagonal Wave supports all four corners. Spiral supports clockwise/counter-clockwise and outside-in/inside-out traversal. Checkerboard accepts an `inverted` flag to swap its two phases.

Burst In starts all items at one origin and restores their authored positions, scale, rotation, and alpha. Burst Out scatters radially from an origin; Gather To converges on a destination. Burst Out and Gather To finish at zero scale and supported alpha. Burst Out chooses a default distance of `120` canvas units for locally animated `RectTransform` items and `1.2` units for ordinary or world-space motion. Set `local: false` for world positions; the default local mode uses `localPosition`, or `anchoredPosition3D` for `RectTransform` targets.

Every recipe returns its active `TweenHandle` and accepts duration, stagger interval, and `TweenOptions` overrides. `LoadingDots` also accepts the pause between complete cycles. Strength scales the spatial distance and deformation without moving Burst In or Gather To away from their exact requested endpoint.

## Validation and errors

- Empty collections return an inactive `TweenHandle` and log a warning.
- Null targets and duplicate target references are rejected when the collection is copied.
- An owner or item destroyed between configuration and `Build` is rejected before child tweens are created.
- Missing presets, incompatible targets, invalid delays, invalid column counts, and invalid ripple origins throw descriptive exceptions.
- Building without first selecting a preset or custom factory is rejected.
- Killing a preset-based stagger group follows normal `TweenHandle` semantics and does not restore arbitrary target state automatically.
- Burst In, Burst Out, and Gather To restore their captured item states on interrupted kill and rewind. Normal Burst Out and Gather To completion intentionally leaves their hidden endpoint.

The explicit owner is the root tween's single lifetime owner. Collection items are required secondary participants and must remain alive until the recipe finishes or is killed; destroying an item does not independently cancel the root. Spatial recipes write item position, scale, local rotation, and supported alpha, so callers should not overlap other writers on those channels.

For replayable previews or pooled UI, capture the desired target state before playback and restore it before starting the next group.

## Animation Gallery

Open the **Collections** category to compare all nineteen recipes and change order, wave direction, diagonal, spiral, and checkerboard options while the matching C# call updates live.

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

The gallery now contains nineteen collection recipes, including all eight topology additions.
