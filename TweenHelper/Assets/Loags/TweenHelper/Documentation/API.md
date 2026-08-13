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
inventoryCells.GridDiagonalWave(inventoryRoot, columns: 4);
inventoryCells.GridSpiral(inventoryRoot, columns: 4);
inventoryCells.GridCheckerboard(inventoryRoot, columns: 4);
inventoryCells.CollectionBurstIn(inventoryRoot, origin);
inventoryCells.CollectionBurstOut(inventoryRoot, origin);
inventoryCells.CollectionGatherTo(inventoryRoot, destination);
loadingDots.LoadingDots(loadingRoot);
```

DOTween cannot nest an infinite child tween inside a sequence. `TweenStaggerBuilder` rejects infinite children with an actionable exception; use a finite child and call `WithLoops(-1)` on the root group. See [Staggered collections](StaggeredCollections.md) for the full contract.

## Destination-aware motion

Eight destination families operate in world or local coordinates without adding entries to the preset registry:

```csharp
transform.Tween().ArcTo(worldDestination, height: 2f, duration: 0.8f).Play();
transform.Tween().BezierTo(worldDestination, worldControlA, worldControlB, 1f).Play();
transform.Tween().HopTo(worldDestination, height: 1.5f, duration: 0.9f).Play();
transform.Tween().SpringTo(worldDestination, duration: 0.55f, overshoot: 0.35f).Play();
transform.Tween().MagneticSnapTo(worldDestination, duration: 0.65f, pullback: 0.2f, overshoot: 0.25f).Play();
transform.Tween().PathThrough(new[] { checkpointA, checkpointB, worldDestination }, duration: 1.2f).Play();
transform.Tween().SpiralTo(worldDestination, radius: 0.8f, revolutions: 1.5f, duration: 1.1f).Play();
transform.Tween().MultiHopTo(worldDestination, height: 1.2f, hopCount: 3, duration: 1.1f).Play();
```

Local variants are `ArcLocalTo`, `BezierLocalTo`, `HopLocalTo`, `SpringLocalTo`, `MagneticSnapLocalTo`, `PathLocalThrough`, `SpiralLocalTo`, and `MultiHopLocalTo`. They use `Transform.localPosition` for ordinary transforms and `RectTransform.anchoredPosition3D` for UI targets. Bezier controls and path waypoints use the same coordinate space as their method.

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

## Gameplay feedback sequences

Use the one-line extensions when the feedback is the complete animation:

```csharp
invalidSlot.ErrorReject();
healthIcon.DamageHit(flashColor: new Color(1f, 0.15f, 0.1f));
objective.SuccessConfirm(options: TweenOptions.WithStrength(1.2f));
reward.RewardReveal(duration: 1.1f);
player.HealReceive();
shield.ShieldBlock(impactDirection);
enemy.CriticalHit(impactDirection);
abilityIcon.CooldownReady();
levelBadge.LevelUp();
healthFrame.LowHealthWarning(options: TweenOptions.WithLoops(-1));
coin.PickupCollectTo(hudCounter.position, arcHeight: 1.8f);
```

The same operations are builder steps and can be sequenced through `Then()` and `With()`:

```csharp
TweenHandle handle = rewardCard.Tween()
    .RewardReveal()
    .Then()
    .PickupCollectLocalTo(counterPosition, arcHeight: 130f, duration: 0.8f)
    .OnComplete(IncrementCounter)
    .Play();
```

All feedback families except Pickup Collect are transient: completion, rewind, and an interrupted kill restore the position, scale, orientation, and supported color captured when the step begins. Pickup collection completes at the exact destination with zero scale and alpha; killing it early restores scale, orientation, and alpha while leaving the current path position available to the caller. A rewind restores its captured start position.

`TweenOptions.WithStrength` scales shake distance, squash/stretch, bounce height, spin, and arc height without multiplying the final pickup shrink. Local feedback uses `RectTransform.anchoredPosition3D` on UI targets. See [Gameplay feedback sequences](FeedbackSequences.md) for APIs, defaults, target support, and lifecycle details.

## Production UI sequences

Use one-line extensions for complete UI transitions:

```csharp
toast.ToastShow(UISequenceDirection.Up);
toast.ToastHide(UISequenceDirection.Right);
modalPanel.ModalOpen(backdrop, controls);
modalPanel.ModalClose(backdrop, controls);
tooltip.TooltipShow();
tooltip.TooltipHide();
dropdown.DropdownOpen(entries);
dropdown.DropdownClose(entries);
outgoingTab.TabSwitchTo(incomingTab, UISequenceDirection.Left);
drawer.DrawerShow(UISequenceDirection.Left, backdrop);
drawer.DrawerHide(UISequenceDirection.Left, backdrop);
sheet.BottomSheetShow(backdrop);
sheet.BottomSheetHide(backdrop);
outgoingPage.PagePushTo(incomingPage);
outgoingPage.PageCrossFadeTo(incomingPage);
```

The same fifteen operations are composable builder steps:

```csharp
TweenHandle handle = toast.Tween()
    .ToastShow()
    .Then()
    .Delay(2f)
    .Then()
    .ToastHide()
    .Play();
```

All targets require `RectTransform`; movement uses `anchoredPosition3D`. Show operations finish on a cached authored baseline, hide operations leave the target visually hidden, kill preserves the interrupted state, and rewind restores the state captured when playback began. Modal and dropdown child lists are copied immediately and remain inside the same root handle. Call `RefreshUIAnimationState()` after an intentional responsive-layout change to recapture the shown endpoint. See [Production UI sequences](UISequences.md) for defaults, direction semantics, layout guidance, validation rules, and lifecycle details.

## Text and value animations

Use the type-safe TextMesh Pro extensions for complete animations:

```csharp
title.TypewriterReveal();
title.TypewriterHide();
score.NumberCountTo(0, 1250, format: "N0");
timer.NumberCountTo(60, 0, value => $"{value:0}s");
message.TextCharacterStaggerIn(UISequenceDirection.Up, distance: 18f);
message.TextCharacterStaggerOut(UISequenceDirection.Up, distance: 18f);
message.TextWave(UISequenceDirection.Up, amplitude: 12f, waveCount: 2);
message.TextCharacterBounce(amplitude: 14f);
message.TextColorSweep(highlightColor);
message.TextGlitch(seed: 1729);
message.TextEmphasis(startCharacter: 0, characterCount: 8);
message.TextScrambleReveal(seed: 1729);
score.ScoreIncrease(1200, 1475, format: "N0");
```

The same operations compose through `TweenBuilder`:

```csharp
TweenHandle handle = title.Tween()
    .TypewriterReveal()
    .Then()
    .TextWave()
    .Play();
```

`NumberCountTo` determines direction from its explicit start and destination values; it does not parse the label's current text. Format-string overloads use the current culture, and formatter overloads support localized units or custom display rules. Typewriter operations animate `maxVisibleCharacters`, so rich-text tags remain intact. Character mesh effects support both `TextMeshProUGUI` and world-space `TextMeshPro`, including multiple TMP material submeshes. Scramble Reveal preserves rich-text tags while replacing eligible visible glyphs deterministically.

Typewriter and number operations preserve their current progress when killed and restore their invocation state on rewind. Character mesh effects restore the captured mesh on completion, kill, and rewind. Score Increase completes on the exact formatted destination; an interrupted kill preserves the displayed value while restoring scale, rotation, and color. Speed-based timing is not supported. See [Text and value animations](TextAndValueAnimations.md) for defaults, formatting, lifecycle, and composition guidance.

## Camera feedback

Camera feedback is available directly on `Camera` or as composable builder operations on its GameObject:

```csharp
gameplayCamera.CameraImpact();
gameplayCamera.CameraRecoil();
gameplayCamera.CameraLandingImpact();
gameplayCamera.CameraFovKick();
gameplayCamera.CameraFocusZoom(focusTarget);
gameplayCamera.CameraBreathing(options: TweenOptions.WithLoops(-1));
```

All six operations are finite and transient: completion, rewind, and interrupted kill restore the exact captured local pose and field of view. Strength scales only temporary feedback magnitude, and speed-based timing is rejected. See [Camera feedback](CameraFeedback.md) for defaults, lifecycle, perspective-camera guidance, and camera-controller integration.

## Tween lifecycle

A finite tween completes when DOTween invokes its completion callback. Killing a tween is a distinct terminal event and does not imply normal completion. Infinite loops never complete normally, so retain their `TweenHandle` and kill or cancel them during owner teardown.

Built tweens are linked to their target GameObject. Destroying the target kills the tween through DOTween's link behavior. Owners should still explicitly kill long-running or looping handles from their normal teardown path.

`TweenAsync.AwaitCompletionWithTimeout` returns `true` for normal completion and `false` for an external kill or timeout. A timeout kills the active tween. Cancellation from the caller's token kills the tween and propagates `OperationCanceledException`.

## Settings and initialization

TweenHelper initializes automatically. Without `Assets/Resources/TweenHelperSettings.asset`, it uses in-memory defaults. Use **Tools > Tween Helper > Settings > Create Settings Asset** only when the project needs custom defaults, and use **Tools > Tween Helper > Settings > Reinitialize System** after changing initialization-sensitive settings.

## Sample controls

Open **TweenHelper Demos** from `Assets/Loags/TweenHelper/Samples/TweenHelper Demos/Scenes`. The 2D scene provides 13 semantic UI recipes, eleven collection recipes with selectable stagger ordering, eight destination-motion examples, eleven gameplay-feedback sequences, fifteen production UI sequences, thirteen text/value examples, and a searchable library of 198 UI-suitable presets. When the legacy Input Manager is enabled, Space replays the current 2D selection and the 3D showcase enables its fly-camera shortcuts. The demos do not require the Input System package.
