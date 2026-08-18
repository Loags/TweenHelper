# Destination-aware motion

Destination motions move a target toward coordinates supplied by the caller. They are `TweenBuilder` operations, not registered presets, because the destination and shape parameters belong to each use site. The preset catalog therefore remains at 300 entries.

## API

```csharp
TweenBuilder ArcTo(Vector3 destination, float height, float? duration = null);
TweenBuilder ArcLocalTo(Vector3 destination, float height, float? duration = null);

TweenBuilder BezierTo(Vector3 destination, Vector3 controlA, Vector3 controlB, float? duration = null);
TweenBuilder BezierLocalTo(Vector3 destination, Vector3 controlA, Vector3 controlB, float? duration = null);

TweenBuilder HopTo(Vector3 destination, float height, float? duration = null);
TweenBuilder HopLocalTo(Vector3 destination, float height, float? duration = null);

TweenBuilder SpringTo(Vector3 destination, float? duration = null, float overshoot = 0.35f);
TweenBuilder SpringLocalTo(Vector3 destination, float? duration = null, float overshoot = 0.35f);

TweenBuilder MagneticSnapTo(Vector3 destination, float? duration = null, float pullback = 0.2f, float overshoot = 0.25f);
TweenBuilder MagneticSnapLocalTo(Vector3 destination, float? duration = null, float pullback = 0.2f, float overshoot = 0.25f);

TweenBuilder PathThrough(IEnumerable<Vector3> waypoints, DestinationPathInterpolation interpolation = DestinationPathInterpolation.CatmullRom, float? duration = null);
TweenBuilder PathLocalThrough(IEnumerable<Vector3> waypoints, DestinationPathInterpolation interpolation = DestinationPathInterpolation.CatmullRom, float? duration = null);

TweenBuilder SpiralTo(Vector3 destination, float radius, float revolutions = 1.5f, float? duration = null);
TweenBuilder SpiralLocalTo(Vector3 destination, float radius, float revolutions = 1.5f, float? duration = null);

TweenBuilder MultiHopTo(Vector3 destination, float height, int hopCount = 3, float decay = 1.25f, float? duration = null);
TweenBuilder MultiHopLocalTo(Vector3 destination, float height, int hopCount = 3, float decay = 1.25f, float? duration = null);
```

All distances use the selected coordinate space. A world overshoot of `0.35f` means 0.35 world units. An anchored UI overshoot is normally supplied in canvas units, such as `24f` pixels.

## Coordinate spaces

World methods read and write `Transform.position`. Their destinations and Bezier controls are world coordinates.

Local methods read and write `Transform.localPosition`. For a `RectTransform`, they instead use `anchoredPosition3D`, so values match normal authored UI anchors and retain the anchored Z component. Their destinations and Bezier controls are local or anchored coordinates.

```csharp
worldObject.Tween().ArcTo(new Vector3(4f, 1f, 0f), 2f, 0.8f).Play();

RectTransform cardRect = card.GetComponent<RectTransform>();
Vector3 destination = new Vector3(320f, 40f, cardRect.anchoredPosition3D.z);
card.Tween().ArcLocalTo(destination, 150f, 0.8f).Play();
```

## Motion families

### Arc

`height` is the signed displacement from the straight-line midpoint along Y. Positive values arc upward and negative values arc downward. The target passes through that displaced midpoint and completes exactly at `destination`.

### Bezier

Bezier motion evaluates a cubic curve from the target's position at step start through `controlA` and `controlB` to `destination`. The world method expects all three supplied points in world space; the local method expects all three in local or anchored space.

```csharp
Vector3 controlA = new Vector3(-120f, 180f, 0f);
Vector3 controlB = new Vector3(140f, 80f, 0f);
panel.Tween().BezierLocalTo(destination, controlA, controlB, 1f).Play();
```

### Hop

Hop briefly anticipates with its visual bottom anchored, releases the squash during takeoff, travels through a signed arc, and compresses downward from the top on landing. The landing recovery keeps the same bottom anchor fixed while restoring the scale captured when the tween is built. `height` controls the path without separate soft or hard variants. Killing during the anticipation, flight, or landing restores the captured scale and removes the temporary grounding offset.

### Spring

Spring travels quickly toward the destination, passes it by `overshoot` along the normalized start-to-destination direction, and settles exactly. A zero-distance request remains valid: its direction is treated as zero and no invalid vector is produced.

### Magnetic snap

Magnetic snap first moves `pullback` units opposite the destination direction, accelerates toward the target, passes it by `overshoot`, and settles exactly. Separate strengths keep it visually distinct from Spring.

### Waypoint path

`PathThrough` copies its waypoint enumerable immediately and traverses each segment in equal normalized time. The final waypoint is the exact destination. `Linear` interpolation connects each point directly; `CatmullRom` produces a smooth curve that passes through every supplied waypoint.

```csharp
Vector3[] path = { checkpointA, checkpointB, destination };
token.Tween().PathThrough(path, DestinationPathInterpolation.CatmullRom, 1.2f).Play();
```

### Spiral

Spiral motion advances toward the destination while a sinusoidal radial envelope opens and closes at the endpoints. World motion forms a three-dimensional spiral around the travel axis. `SpiralLocalTo` uses a visible XY-plane spiral for `RectTransform` targets, so it does not disturb authored anchored Z. A negative revolution count reverses winding direction.

### Multi-hop

Multi-hop advances continuously while applying the requested number of diminishing signed Y hops. `decay` controls how quickly successive hop heights reduce; zero keeps all hops at equal height. Positive height hops upward and negative height hops downward.

Upward and downward Arc, Hop, and Multi-Hop paths are reviewed in both world and anchored space. Positive and negative Spiral revolution counts are also reviewed in both contexts, with the guide using the exact signed values of the selected entry.

## Builder composition and options

Destination steps use the same builder contract as movement, scale, and preset steps:

```csharp
TweenHandle handle = token.Tween()
    .ArcTo(slotPosition, 1.25f, 0.55f)
    .WithEase(Ease.OutCubic)
    .WithDelay(0.1f)
    .Then()
    .SpringTo(finalPosition, 0.35f, 0.2f)
    .OnComplete(EnableInteraction)
    .OnKill(ClearPendingState)
    .Play();
```

`Then()` appends and `With()` joins a destination motion like any other builder step. The step captures its position when playback reaches it, so a preceding movement does not leave it with a stale path start. `WithEase()` controls the main travel phase. Multi-stage families preserve their own anticipation or settle phases while applying caller delay, loop, ID, update type, and unscaled-time settings at the motion root. Waypoint, spiral, and multi-hop paths use normalized timing and reject speed-based options.

## Completion, kill, reset, and replay

- Normal one-shot completion writes the exact destination to the selected position property.
- Restart loops reuse the original absolute path instead of adding offsets, preventing accumulated positional drift.
- Yoyo loops return to the exact step-start position when their final pass ends in reverse.
- Killing a motion leaves its current position available to the caller; reset that position from application state when cancellation should visually rewind it.
- Killing Hop during a temporary deformation restores the scale captured at build time and removes its temporary bottom-anchor offset while leaving the current base path position intact.
- `TweenHandle.Restart()` and newly built replays do not add offsets to the previous result.
- Destroying the target kills the linked root through DOTween's normal link behavior.

```csharp
Vector3 closedPosition = panelRect.anchoredPosition3D;
TweenHandle handle = panel.Tween().HopLocalTo(openPosition, 120f, 0.7f).Play();

// Cancel and explicitly rewind application position.
handle.Kill();
panelRect.anchoredPosition3D = closedPosition;
```

Destinations, waypoints, controls, duration, height, radius, revolutions, decay, pullback, and overshoot must be finite. Duration must be greater than zero; radius, decay, pullback, and overshoot cannot be negative; hop count must be positive. Signed arc, hop, multi-hop heights, and signed spiral revolutions are supported intentionally.

## Animation Gallery

Open **Destination Motion** to compare twelve motion families. Contextual controls switch world/local targets, signed variants, and path interpolation while the preview and C# call stay synchronized.

## World-to-UI projection

`ArcToUI`, `HopToUI`, `BezierToUI`, and `PathThroughUI` bridge a world source point to a `RectTransform` destination. A `RectTransform` animation target is treated as a UI pickup proxy and starts at the projected world point. An ordinary transform starts at the supplied world point and converges visually on the UI anchor at its captured camera depth.

```csharp
pickupIcon.Tween()
    .ArcToUI(droppedItem.position, inventorySlot, 145f, worldCamera: gameplayCamera)
    .Play();

worldPickup.Tween()
    .HopToUI(worldPickup.transform.position, counterAnchor, 2f, worldCamera: gameplayCamera)
    .Play();
```

The camera resolution order is explicit camera, destination canvas `worldCamera`, then `Camera.main`. Overlay and camera-space canvases are supported. `lockDestination: true` snapshots the UI endpoint when the step starts; set it to `false` when the animation should follow a moving UI anchor. Interrupted kill and rewind restore the invocation position and temporary deformation.
