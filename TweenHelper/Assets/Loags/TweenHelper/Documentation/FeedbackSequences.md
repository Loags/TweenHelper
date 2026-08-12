# Gameplay feedback sequences

Gameplay feedback sequences combine several coordinated channels into one semantic animation. They are builder operations and direct extensions rather than registered presets, so the built-in preset catalog remains at 300 entries.

## API

```csharp
TweenBuilder ErrorReject(float? duration = null, Color? flashColor = null);
TweenBuilder DamageHit(float? duration = null, Color? flashColor = null);
TweenBuilder SuccessConfirm(float? duration = null, Color? flashColor = null);
TweenBuilder RewardReveal(float? duration = null, Color? flashColor = null);

TweenBuilder PickupCollectTo(Vector3 destination, float? arcHeight = null, float? duration = null);
TweenBuilder PickupCollectLocalTo(Vector3 destination, float? arcHeight = null, float? duration = null);
```

Each name is also a one-line extension on `GameObject` and `Component`. Direct extensions return `TweenHandle` and accept `TweenOptions` as their last optional argument.

```csharp
button.ErrorReject();
playerView.DamageHit(duration: 0.45f);
resultPanel.SuccessConfirm(options: TweenOptions.WithStrength(0.8f));
rewardIcon.RewardReveal(flashColor: new Color(1f, 0.75f, 0.2f));
pickup.PickupCollectTo(inventorySlot.position, arcHeight: 2f);
```

## Families

### Error reject

`ErrorReject` uses a sharp deterministic shake, opposing Z tilt, and red flash. Its amplitude decays to zero without random state, making repeated playback visually consistent.

### Damage hit

`DamageHit` combines an impact shake, grounded squash, recoil, and stronger red flash. The grounding correction keeps the visual bottom planted during the temporary deformation.

### Success confirm

`SuccessConfirm` uses a small pop, two diminishing upward bounces, and green flash. It is intended for successful input, completed objectives, and positive UI acknowledgements.

### Reward reveal

`RewardReveal` anticipates, performs a relative full spin, overshoots, pulses, and flashes gold. Rotation is applied relative to the orientation captured at step start, so an already rotated object does not snap to a global orientation.

### Pickup collect

`PickupCollectTo` and `PickupCollectLocalTo` punch first, travel through a signed vertical arc, then shrink and fade during the latter part of the path. Normal completion writes the exact destination, restores the captured orientation, and leaves scale and supported alpha at zero.

World pickup uses `Transform.position`. Local pickup uses `Transform.localPosition`, or `RectTransform.anchoredPosition3D` for UI. When `arcHeight` is omitted, the automatic default is `2f` for world targets and `145f` for UI targets. Negative heights intentionally create a downward arc.

## Composition and tuning

The operations compose like other `TweenBuilder` steps:

```csharp
TweenHandle handle = reward.Tween()
    .WithOptions(TweenOptions.WithStrength(1.15f))
    .RewardReveal()
    .Then()
    .PickupCollectLocalTo(counterPosition, 120f, 0.8f)
    .WithDelay(0.15f)
    .Play();
```

Default durations are `0.58s` for Error Reject, `0.5s` for Damage Hit, `0.78s` for Success Confirm, `1.08s` for Reward Reveal, and `0.92s` for Pickup Collect. An explicit method duration wins over `TweenOptions.Duration`, which wins over that family default.

`TweenOptions.WithStrength` scales the expressive magnitude: shake distance, squash/stretch, bounce or lift height, relative rotation, and pickup arc height. A strength of zero removes those accents, but Pickup Collect still reaches its destination and shrinks to zero because those are its semantic outcome. Strength must be finite and non-negative.

The caller's primary ease controls pickup travel. Multi-stage transient families use coordinated internal phase eases while still applying delay, ID, loops, update type, and unscaled-time options to their linked root tween.

## Color and alpha targets

Color flashes support `Graphic`, `TMP_Text`, `SpriteRenderer`, and a `Renderer` whose material exposes `_BaseColor` or `_Color`. Renderer changes use `MaterialPropertyBlock`; they do not instantiate or mutate the shared material. The complete property block captured at step start is restored after transient feedback.

Pickup fading supports `CanvasGroup`, `Graphic`, `TMP_Text`, `SpriteRenderer`, and the same renderer color properties. If no supported alpha target exists, Pickup Collect still completes through position and scale.

## Completion, kill, rewind, and replay

- Error Reject, Damage Hit, Success Confirm, and Reward Reveal capture state lazily when their builder step starts.
- Normal completion restores captured position, scale, local orientation, and supported color.
- Killing one of those transient sequences before completion restores the same captured state.
- Rewind restores the captured state, and restart replays without accumulating offsets or rotation.
- Pickup Collect normally finishes at the exact destination with zero scale and supported alpha.
- Killing Pickup Collect before completion restores captured scale, local orientation, and alpha while preserving its current path position.
- Rewinding Pickup Collect restores its captured start position and visual state.
- A finite even-count Yoyo Pickup Collect ends at its captured start state.
- Destroying the target kills the linked root through DOTween's normal link behavior.

Durations, destinations, optional arc height, and strength must be finite. Duration must be greater than zero.
