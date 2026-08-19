# Engine property animations

TweenHelper exposes finite, target-linked wrappers for common non-transform properties. Each method is available as a direct component extension and as a composable `TweenBuilder` operation.

## API

```csharp
audioSource.AudioVolumeTo(0f, 0.5f);
audioSource.AudioPitchTo(1.25f);
torch.LightIntensityTo(2.5f);
torch.LightColorTo(Color.yellow);
particles.ParticleEmissionRateTo(1.5f); // rateOverTime multiplier
renderer.MaterialFloatTo("_Dissolve", 1f);
renderer.MaterialColorTo("_EmissionColor", Color.cyan);
```

Audio volume is validated in `0..1`; audio pitch in `-3..3`; light intensity and particle emission multiplier are non-negative. Renderer operations use `MaterialPropertyBlock`, require the shader property to exist, and do not instantiate or mutate the shared material.

An explicit method duration wins over `TweenOptions.Duration`, which wins over the operation default. Delay, ID, update mode, unscaled time, loops, callbacks, and awaiting apply to the linked root. Normal completion keeps the requested endpoint. Rewind, finite reverse completion, and interrupted kill restore the invocation value.

Do not run another system against the same property while one of these tweens owns it. In particular, coordinate AudioMixer automation, lighting controllers, particle scripts, and renderer effects explicitly when they share a channel.

## Ambient helpers

`TorchFlicker` plays one deterministic finite intensity cycle suitable for root looping. `ScannerPulse` performs one finite light intensity/color pulse.

```csharp
TweenHandle flicker = torch.TorchFlicker(
    options: TweenOptions.WithLoops(-1, DG.Tweening.LoopType.Restart));

scanner.ScannerPulse(Color.cyan, intensityBoost: 1.5f);
```

Retain and kill infinite handles during teardown. Normal property completion keeps the requested endpoint. Rewind, finite reverse completion, and interrupted kill restore the invocation value. Ambient helpers are transient and always restore their captured light state.

## Preset Browser previews

The Preset Browser contains nine engine-property entries: audio volume, audio pitch, light intensity, light color, particle emission, material float, material color, Torch Flicker, and Scanner Pulse. Each isolated fixture includes a live normalized meter and textual value readout.

- Volume meters map the supported `0..1` range directly.
- Pitch meters use a presentation range centered around normal playback (`0.5..2`) while the runtime API continues to accept Unity's wider validated `-3..3` range.
- Light and particle meters normalize against the authored preview range and update from the real component property.
- Torch Flicker uses a bright warm fixture and stronger finite variation so intensity changes remain visible in the isolated stage.
- Renderer previews read the effective `MaterialPropertyBlock` value rather than mutating the shared material.

The preview meter is a diagnostic presentation aid, not a clamp on the public API.
