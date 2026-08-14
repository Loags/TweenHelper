# Camera feedback

Camera feedback operations coordinate camera position, orientation, and field of view through one finite `TweenHandle`. They are semantic builder operations and direct extensions rather than registered presets, so the built-in preset catalog remains at 300 entries.

## API

```csharp
TweenBuilder CameraImpact(float positionStrength = 0.18f, float rotationStrength = 2.4f, float? duration = null);
TweenBuilder CameraRecoil(float distance = 0.3f, float pitch = 4f, float? duration = null);
TweenBuilder CameraLandingImpact(float dropDistance = 0.22f, float fieldOfViewKick = 3f, float? duration = null);
TweenBuilder CameraFovKick(float fieldOfViewDelta = 8f, float? duration = null);
TweenBuilder CameraFocusZoom(Transform focusTarget, float distance = 1.2f, float fieldOfViewDelta = 7f, float? duration = null);
TweenBuilder CameraBreathing(float positionAmplitude = 0.035f, float rotationAmplitude = 0.3f, float fieldOfViewAmplitude = 0.45f, float? duration = null);
```

Each operation is also available directly on `Camera` and `GameObject`. The target GameObject must contain a `Camera` component on the same object.

```csharp
gameplayCamera.CameraImpact();
gameplayCamera.CameraRecoil(distance: 0.4f, pitch: 5f);
gameplayCamera.CameraFocusZoom(bossTransform);

TweenHandle handle = gameplayCamera.gameObject.Tween()
    .CameraLandingImpact()
    .Then()
    .CameraBreathing()
    .Play();
```

## Families

- `CameraImpact` applies a deterministic decaying positional and rotational shake.
- `CameraRecoil` kicks backward with pitch and a small settling aftershock.
- `CameraLandingImpact` combines a downward bump, roll aftershock, and field-of-view kick.
- `CameraFovKick` performs a fast lens kick and settle without moving the camera.
- `CameraFocusZoom` temporarily moves and aims toward a focus target while narrowing field of view. The focus position is captured when the builder step begins.
- `CameraBreathing` provides one subtle, finite position, rotation, and lens cycle suitable for root looping.

Default durations are `0.38s`, `0.48s`, `0.55s`, `0.42s`, `0.82s`, and `2.8s` respectively. FOV channels are visually meaningful on perspective cameras; transform channels still work on orthographic cameras.

## Options and lifecycle

An explicit duration wins over `TweenOptions.Duration`, which wins over the family default. `TweenOptions.WithStrength` scales temporary movement, rotation, and field-of-view magnitude. Delay, ID, loops, update type, unscaled time, callbacks, and awaiting apply to the complete linked root. Speed-based timing is rejected.

Every family is transient:

- State is captured lazily when its builder step starts.
- Normal completion restores the exact captured local position, local rotation, and field of view.
- Interrupted kill and rewind restore the same captured state.
- Restart and finite loops do not accumulate offsets.
- Destroying the camera target kills the linked root through DOTween's normal link behavior.

The camera GameObject is the operation's single lifetime owner. A `CameraFocusZoom` focus target is a required secondary participant and must remain alive until the operation finishes or is killed; destroying it does not independently own cancellation.

During playback, field of view is clamped to Unity's valid `1..179` range and then restored exactly. Retain and kill an infinitely looped Breathing handle during teardown.

Do not let a camera controller and a feedback tween write the same transform simultaneously. Prefer a dedicated camera feedback child or pause the competing writer for the duration of the effect.
