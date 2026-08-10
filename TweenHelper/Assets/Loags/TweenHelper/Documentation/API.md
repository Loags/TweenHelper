# API examples

All snippets assume `using LB.TweenHelper;`, `using DG.Tweening;`, and `using UnityEngine;`.

## Basic builder steps

```csharp
TweenHandle handle = gameObject.Tween()
    .MoveLocal(Vector3.up, 0.25f)
    .Then()
    .RotateLocal(new Vector3(0f, 180f, 0f), 0.2f)
    .Then()
    .Scale(1.1f, 0.15f)
    .Play();
```

`Fade`, `FadeIn`, and `FadeOut` require a `CanvasGroup`, `Graphic`, `SpriteRenderer`, or supported `Renderer` on the target.

```csharp
TweenHandle handle = gameObject.Tween()
    .FadeIn(0.2f)
    .Then()
    .Delay(0.4f)
    .Then()
    .FadeOut(0.2f)
    .Play();
```

## Play a preset

```csharp
TweenHandle handle = gameObject.Tween()
    .Preset<SlideInLeftPreset>(0.4f)
    .Play();
```

The generic API is compile-time checked and should be used whenever the preset type is known. Dynamic names remain available through the explicitly named fallback:

For allocation-sensitive call sites that do not need `TweenHandle`, play the typed preset directly and retain DOTween's raw `Tween`:

```csharp
Tween tween = transform.PlayPreset<SlideInLeftPreset>(0.4f);
```

This path does not allocate a `TweenBuilder`, builder step storage, or a handle wrapper.

```csharp
TweenHandle handle = gameObject.Tween()
    .PresetByName(presetNameFromSaveData)
    .Play();
```

## Sequence and join

```csharp
TweenHandle handle = transform.Tween()
    .MoveLocal(Vector3.up, 0.25f)
    .With()
    .Scale(1.2f, 0.25f)
    .Then()
    .Scale(1f, 0.15f)
    .Play();
```

`Then()` appends the next step. `With()` joins the next step at the previous insertion point.

## Delay, callbacks, and raw DOTween injection

```csharp
Tween rawTween = transform.DOPunchRotation(Vector3.forward * 8f, 0.2f).Pause();

TweenHandle handle = transform.Tween()
    .MoveLocal(Vector3.up, 0.25f)
    .Then()
    .Delay(0.1f)
    .Then(rawTween)
    .Call(() => Debug.Log("Raw tween finished"))
    .Play();
```

## Configure one step

```csharp
TweenHandle handle = transform.Tween()
    .MoveLocal(Vector3.zero)
    .WithOptions(TweenOptions.WithDuration(0.4f))
    .WithEase(DG.Tweening.Ease.OutCubic)
    .Then()
    .WithOptions(TweenOptions.WithDuration(0.2f))
    .Scale(1f)
    .Play();
```

`WithOptions` replaces the options currently associated with its step. Individual modifiers such as `WithEase`, `WithDelay`, and `WithLoops` update only their populated value.

Options can also be composed before applying them:

```csharp
TweenOptions options = TweenOptions.WithDuration(0.5f)
    .SetEase(Ease.OutBack)
    .SetDelay(0.1f)
    .SetLoops(2, LoopType.Yoyo)
    .SetStrength(1.25f)
    .SetStartScale(Vector3.zero)
    .SetTargetScale(Vector3.one);

TweenHandle handle = transform.Tween()
    .WithOptions(options)
    .Preset<PopInPreset>()
    .Play();
```

Precedence is: explicit method duration, then `TweenOptions.Duration`, then `TweenHelperSettings.DefaultDuration`. `WithOptions` replaces the current step value; individual fluent modifiers merge only their own field.

## Callbacks and cleanup

```csharp
TweenHandle handle = transform.Tween()
    .Preset<PopInPreset>()
    .OnComplete(ShowContent)
    .OnKill(ReleaseAnimationState)
    .Play();

handle.OnComplete(TrackAnalytics);

// Later
handle.Kill();
```

Callbacks are additive. Registering through a builder or handle does not replace callbacks that a preset already installed.

## Await and cancel

```csharp
using System.Threading;
using System.Threading.Tasks;

private async Task AnimateAsync(CancellationToken cancellationToken)
{
    TweenHandle handle = transform.Tween()
        .Preset<FadeInPreset>()
        .Play();

    await TweenAsync.AwaitCompletion(handle.Tween, cancellationToken);
}
```

Cancelling the token kills an active tween and throws `OperationCanceledException`. Use `AwaitCompletionWithTimeout` when a boolean normal-completion result is more convenient.

```csharp
bool completedNormally = await TweenAsync.AwaitCompletionWithTimeout(handle.Tween, 2f, cancellationToken);
```

When a tween-owned cancellation token must outlive a local scope, retain and dispose its registration:

```csharp
using TweenAsync.TweenCancellationRegistration registration =
    TweenAsync.CreateTweenLinkedCancellation(handle.Tween, cancellationToken);

await RunDependentWork(registration.Token);
```

## Direct registry access

```csharp
PulseScalePreset preset = TweenPresetRegistry.GetPreset<PulseScalePreset>();
if (preset != null && preset.CanApplyTo(gameObject))
{
    Tween tween = preset.CreateTween(gameObject, 0.5f);
    tween.Play();
}
```

For a dynamic name, use `TweenPresetRegistry.GetPresetByName(name)` or `TweenPresetRegistry.PlayByName(name, target)`.

## Staggered collections

Use `TweenStagger(owner)` to combine one finite animation per item into a single sequence. The explicit owner controls the lifecycle of the complete group.

```csharp
TweenHandle handle = cards.TweenStagger(panel)
    .Preset<PopInFadePreset>(0.32f, TweenOptions.WithStrength(1.1f))
    .Order(StaggerOrder.FromCenter)
    .DelayBetween(0.06f)
    .OnComplete(EnableInput)
    .Play();
```

Dynamic preset names and custom DOTween factories use the same scheduling layer:

```csharp
TweenHandle dynamicHandle = cards.TweenStagger(panel)
    .PresetByName(savedPresetName, 0.3f)
    .Order(StaggerOrder.Random)
    .Seed(1729)
    .Play();

TweenHandle customHandle = cards.TweenStagger(panel)
    .Animate((item, index) => item.transform.DORotate(Vector3.forward * 12f, 0.2f).SetLoops(2, LoopType.Yoyo))
    .DelayBy((item, index) => index * index * 0.025f)
    .Play();
```

Built-in orders are `FirstToLast`, `LastToFirst`, `FromCenter`, `ToCenter`, and deterministic `Random`. `DelayBy` supplies absolute per-item delays and replaces the configured order until a later `Order` or `DelayBetween` call.

Ready-to-play recipes are available directly on `IEnumerable<GameObject>` and component collections:

```csharp
menuItems.ListStaggerIn(menuRoot);
menuItems.ListStaggerOut(menuRoot);
inventoryCells.GridWave(inventoryRoot, columns: 4, direction: GridWaveDirection.TopToBottom);
inventoryCells.GridRipple(inventoryRoot, columns: 4);
loadingDots.LoadingDots(loadingRoot);
```

DOTween cannot nest an infinite child tween inside a sequence. `TweenStaggerBuilder` rejects infinite children with an actionable exception; use a finite child and call `WithLoops(-1)` on the root group. See [Staggered collections](StaggeredCollections.md) for the full contract.

## Destination-aware motion

The five destination families operate in world or local coordinates without adding entries to the preset registry:

```csharp
transform.Tween().ArcTo(worldDestination, height: 2f, duration: 0.8f).Play();
transform.Tween().BezierTo(worldDestination, worldControlA, worldControlB, 1f).Play();
transform.Tween().HopTo(worldDestination, height: 1.5f, duration: 0.9f).Play();
transform.Tween().SpringTo(worldDestination, duration: 0.55f, overshoot: 0.35f).Play();
transform.Tween().MagneticSnapTo(worldDestination, duration: 0.65f, pullback: 0.2f, overshoot: 0.25f).Play();
```

Local variants are `ArcLocalTo`, `BezierLocalTo`, `HopLocalTo`, `SpringLocalTo`, and `MagneticSnapLocalTo`. They use `Transform.localPosition` for ordinary transforms and `RectTransform.anchoredPosition3D` for UI targets. Bezier control points use the same coordinate space as their method.

```csharp
TweenHandle handle = panel.Tween()
    .ArcLocalTo(openPosition, 140f, 0.6f)
    .WithEase(Ease.OutCubic)
    .Then()
    .MagneticSnapLocalTo(restPosition, 0.45f, pullback: 18f, overshoot: 12f)
    .WithDelay(0.1f)
    .Play();
```

Each motion is safe to compose through `Then()` and `With()`, is linked to its target, and applies delay, ease, ID, loop, update, and unscaled-time options at its own root. Normal completion corrects the final position exactly. Hop also restores the scale captured when the tween is built if playback is killed during its temporary squash. See [Destination-aware motion](DestinationMotion.md) for the full coordinate and lifecycle contract.

## Tween lifecycle

A finite tween completes when DOTween invokes its completion callback. Killing a tween is a distinct terminal event and does not imply normal completion. Infinite loops never complete normally, so retain their `TweenHandle` and kill or cancel them during owner teardown.

Built tweens are linked to their target GameObject. Destroying the target kills the tween through DOTween's link behavior. Owners should still explicitly kill long-running or looping handles from their normal teardown path.

`TweenAsync.AwaitCompletionWithTimeout` returns `true` for normal completion and `false` for an external kill or timeout. A timeout kills the active tween. Cancellation from the caller's token kills the tween and propagates `OperationCanceledException`.

## Settings and initialization

TweenHelper initializes automatically. Without `Assets/Resources/TweenHelperSettings.asset`, it uses in-memory defaults. Use **Tools > Tween Helper > Settings > Create Settings Asset** only when the project needs custom defaults, and use **Tools > Tween Helper > Settings > Reinitialize System** after changing initialization-sensitive settings.

## Sample controls

Open **TweenHelper Demos** from `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes`. The 2D scene provides 13 semantic UI recipes, five collection recipes with selectable stagger ordering, five destination-motion examples, and a searchable library of 198 UI-suitable presets. When the legacy Input Manager is enabled, Space replays the current 2D selection and the 3D showcase enables its fly-camera shortcuts. The demos do not require the Input System package.
