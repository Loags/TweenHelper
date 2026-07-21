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
    .Preset("SlideInLeft", 0.4f)
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
    .Preset("PopIn")
    .Play();
```

Precedence is: explicit method duration, then `TweenOptions.Duration`, then `TweenHelperSettings.DefaultDuration`. `WithOptions` replaces the current step value; individual fluent modifiers merge only their own field.

## Callbacks and cleanup

```csharp
TweenHandle handle = transform.Tween()
    .Preset("PopIn")
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
        .Preset("FadeIn")
        .Play();

    await TweenAsync.AwaitCompletion(handle.Tween, cancellationToken);
}
```

Cancelling the token kills an active tween and throws `OperationCanceledException`. Use `AwaitCompletionWithTimeout` when a boolean normal-completion result is more convenient.

```csharp
bool completedNormally = await TweenAsync.AwaitCompletionWithTimeout(handle.Tween, 2f, cancellationToken);
```

## Direct registry access

```csharp
ITweenPreset preset = TweenPresetRegistry.GetPreset("Pulse");
if (preset != null && preset.CanApplyTo(gameObject))
{
    Tween tween = preset.CreateTween(gameObject, 0.5f);
    tween.Play();
}
```

## Settings and initialization

TweenHelper initializes automatically. Without `Assets/Resources/TweenHelperSettings.asset`, it uses in-memory defaults. Use **Tools > TweenHelper > Create Settings Asset** only when the project needs custom defaults, and use **Tools > TweenHelper > Reinitialize System** after changing initialization-sensitive settings.

## Sample controls

Open **TweenHelper Demos** from `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes`. When the legacy Input Manager is enabled, the 2D showcase uses Space and the arrow keys and the 3D showcase enables its fly-camera shortcuts. The public context-menu controls and `AnimationResetAuditRunner` remain available without an Input System package.
