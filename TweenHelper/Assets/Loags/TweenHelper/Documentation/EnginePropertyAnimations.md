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

## Ambient helpers

`TorchFlicker` plays one deterministic finite intensity cycle suitable for root looping. `ScannerPulse` performs one finite light intensity/color pulse.

```csharp
TweenHandle flicker = torch.TorchFlicker(
    options: TweenOptions.WithLoops(-1, DG.Tweening.LoopType.Restart));

scanner.ScannerPulse(Color.cyan, intensityBoost: 1.5f);
```

Retain and kill infinite handles during teardown. Normal property completion keeps the requested endpoint. Rewind, finite reverse completion, and interrupted kill restore the invocation value. Ambient helpers are transient and always restore their captured light state.
