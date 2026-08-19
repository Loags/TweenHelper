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

## Advanced feature guides

The advanced APIs use the same `TweenBuilder`, `TweenOptions`, and `TweenHandle` lifecycle described above. Their focused guides own the complete method lists, defaults, target requirements, replay behavior, and limitations:

| Feature | Start here |
| --- | --- |
| Staggered lists, grids, loading dots, burst, and gather recipes | [Staggered collections](StaggeredCollections.md) |
| Arc, Bezier, hop, spring, snap, path, spiral, and multi-hop movement | [Destination-aware motion](DestinationMotion.md) |
| Error, damage, success, reward, healing, defense, warning, and pickup feedback | [Gameplay feedback sequences](FeedbackSequences.md) |
| Toasts, modals, tooltips, dropdowns, tabs, drawers, sheets, and page transitions | [Production UI sequences](UISequences.md) |
| Typewriter, character mesh effects, numeric values, scores, and scramble reveal | [Text and value animations](TextAndValueAnimations.md) |
| Impact, recoil, landing, field-of-view, focus zoom, and breathing | [Camera feedback](CameraFeedback.md) |
| Audio, light, particle emission, renderer properties, and ambient pulses | [Engine property animations](EnginePropertyAnimations.md) |
| Complete v1 expansion plan and implementation status | [Implementation roadmap](ImplementationRoadmap.md) |

Use a direct extension when the operation is the complete animation, or add the same operation to a builder when it must compose with other steps:

```csharp
invalidSlot.ErrorReject();

TweenHandle handle = toast.Tween()
    .ToastShow()
    .Then()
    .Delay(2f)
    .Then()
    .ToastHide()
    .Play();
```

## Tween lifecycle

A finite tween completes when DOTween invokes its completion callback. Killing a tween is a distinct terminal event and does not imply normal completion. Infinite loops never complete normally, so retain their `TweenHandle` and kill or cancel them during owner teardown.

Built tweens are linked to their target GameObject. Destroying the target kills the tween through DOTween's link behavior. Owners should still explicitly kill long-running or looping handles from their normal teardown path.

`TweenAsync.AwaitCompletionWithTimeout` returns `true` for normal completion and `false` for an external kill or timeout. A timeout kills the active tween. Cancellation from the caller's token kills the tween and propagates `OperationCanceledException`.

## Settings and initialization

TweenHelper initializes automatically. Without `Assets/Resources/TweenHelperSettings.asset`, it uses in-memory defaults. Use **Tools > Tween Helper > Settings > Create Settings Asset** only when the project needs custom defaults, and use **Tools > Tween Helper > Settings > Reinitialize System** after changing initialization-sensitive settings.

## Sample controls

Open `TweenHelperAnimationGallery.unity` from `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes`. The mouse-driven gallery exposes all 300 presets plus 13 UI recipes, nineteen collection recipes, twelve destination-motion operations, twenty-five gameplay-feedback and macro sequences, sixteen production UI sequences, thirteen text/value examples, and eight camera-feedback operations. Selection auto-plays after reset; Replay, Reset, previous/next navigation, contextual enum options, search, preset-family filters, and a live C# example remain available at runtime. Component-specific fill, audio, light, particle, and renderer examples are documented in their focused guides. The gallery does not require the Input System package.

Open **Tools > Tween Helper > Preset Browser** for the complete 446-entry Editor discovery surface. It contains all 300 presets plus 146 semantic and component-property previews. The preview uses an isolated, purpose-built fixture and never reads from or modifies the active scene. UI sequence fixtures include only required participants; progress and engine-property fixtures expose live value meters and labels.
