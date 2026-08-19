# Tween Helper

Tween Helper is a fluent animation builder and a catalog of reusable presets built on DOTween. It supports transform, UI, SpriteRenderer, renderer, TextMesh Pro, collections, destination motion, world-to-UI projection, gameplay feedback, production UI, progress values, cameras, audio, lights, particles, and material properties while keeping playback, sequencing, cancellation, and reset behavior consistent.

Version `1.1.0` is the initial public release. It was developed and validated with Unity `6000.5.2f1` and DOTween Free package `1.2.825` (runtime `1.3.030`). Lower Unity and older DOTween versions have not been tested. DOTween is installed and licensed separately; it is not included with Tween Helper.

## Requirements

- Unity `6000.5.2f1`, the version used for development and validation. Lower versions are untested.
- DOTween Free package `1.2.825` (runtime `1.3.030`), the version used for development and validation. Older versions are untested. Install it separately from the [Unity Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676) or [Demigiant](https://dotween.demigiant.com/download.php).
- DOTween modules generated through **Tools > Demigiant > DOTween Utility Panel > Setup DOTween**.
- Unity UI (uGUI) and TextMesh Pro. Import TextMesh Pro Essential Resources before opening the gallery.
- Built-in Render Pipeline or Universal Render Pipeline. HDRP and custom render pipelines are untested.

DOTween is an external Asset Store dependency and is not redistributed with this package. Use **Tools > Tween Helper > Validate > DOTween Setup** after installation for an actionable setup check.

## Installation

Install and set up DOTween first. Then import the TweenHelper `.unitypackage` from **Window > Package Management > My Assets**, or import a local release artifact with **Assets > Import Package > Custom Package**.

All distributable files are installed beneath `Assets/Loags/TweenHelper`. The included Animation Gallery is in `Samples/TweenHelper Demos`. It is mouse-driven, capture-ready at 16:9, and does not require the Input System.

Tween Helper opens **Tools > Tween Helper > Setup & Support** once for each imported package version. The window checks DOTween, the active render pipeline, Unity UI, and TextMesh Pro without changing the project automatically. It also links to the required installation locations and remains available from the Tools menu after dismissal.

## Quick start

```csharp
using LB.TweenHelper;
using UnityEngine;

public sealed class CardEntrance : MonoBehaviour
{
    private TweenHandle _animation;

    private void OnEnable()
    {
        _animation = gameObject.Tween()
            .Preset<PopInPreset>(0.35f)
            .WithEase(DG.Tweening.Ease.OutBack)
            .Play();
    }

    private void OnDisable()
    {
        _animation?.Kill();
    }
}
```

Use `Preset<TPreset>()` whenever the preset type is known at compile time. For names loaded from save data, an Inspector field, or another runtime source, use the explicit `PresetByName(string)` fallback.

Build a sequence with `Then()` and `With()`:

```csharp
TweenHandle handle = transform.Tween()
    .MoveLocal(Vector3.up * 2f, 0.3f)
    .With()
    .Fade(1f, 0.3f)
    .Then()
    .Scale(1.1f, 0.15f)
    .Then()
    .Scale(1f, 0.15f)
    .OnComplete(() => Debug.Log("Entrance complete"))
    .Play();
```

Options written immediately after a step apply to that step. Options written after `Then()` or `With()` apply to the next step. An explicit method duration wins over `TweenOptions.Duration`, which wins over the global default.

Animate a collection through one owner-linked stagger sequence:

```csharp
TweenHandle handle = cards.TweenStagger(this)
    .Preset<PopInFadePreset>(0.32f)
    .Order(StaggerOrder.FromCenter)
    .DelayBetween(0.06f)
    .Play();
```

Nineteen gallery-facing collection recipes cover list entrances/exits, directional, diagonal, spiral, checkerboard, ripple, concentric, quadrant, accordion, orbit, loading-ring/ribbon, burst, and gather motion. Collection recipes orchestrate semantic group timelines without adding entries to the 300-preset registry.

Move to explicit destinations with reusable world, local, and anchored-position motions:

```csharp
TweenHandle handle = card.Tween()
    .ArcLocalTo(destination, height: 140f, duration: 0.7f)
    .Then()
    .SpringLocalTo(restingPosition, duration: 0.35f, overshoot: 24f)
    .Play();
```

Arc, Bezier, hop, spring, magnetic-snap, waypoint-path, spiral, and multi-hop operations are parameterized builder motions rather than registered presets, so the preset registry remains at 300 entries.

Project a world pickup into a screen-space or camera-space UI destination:

```csharp
pickupProxy.Tween()
    .ArcToUI(worldPickup.position, inventorySlot, height: 145f, worldCamera: gameplayCamera)
    .Play();

worldPickup.HopToUI(worldPickup.transform.position, counterAnchor, height: 2f, worldCamera: gameplayCamera);
```

World-to-UI Arc, Hop, Bezier, Path, and pickup-collection operations resolve the target canvas and camera, optionally follow a moving UI anchor, and restore invocation position and temporary deformation when interrupted or rewound.

Play semantic gameplay feedback directly or compose it inside a larger builder sequence:

```csharp
healthIcon.DamageHit();
confirmButton.SuccessConfirm();
shield.ShieldBlock(impactDirection);
abilityIcon.CooldownReady();

TweenHandle handle = reward.Tween()
    .RewardReveal()
    .Then()
    .PickupCollectLocalTo(counterPosition, arcHeight: 120f)
    .Play();
```

Error, damage, success, reward, heal, block, critical-hit, cooldown-ready, level-up, low-health, charging, readiness, dodge, stun, buff/debuff, resource, and objective feedback restore or retain state according to each operation's documented lifecycle. `PickupCollectTo`, `PickupCollectLocalTo`, and `PickupCollectToUI` finish at their supplied destination with collection scale/alpha behavior. Reusable critical-hit, reward, warning, and cutscene macros compose the same public primitives without expanding the 300-preset catalog.

Build complete production UI transitions with the same one-line and builder APIs:

```csharp
toast.ToastShow();
modalPanel.ModalOpen(backdrop, controls);
dropdown.DropdownOpen(entries);
outgoingTab.TabSwitchTo(incomingTab);
drawer.DrawerShow(UISequenceDirection.Left, backdrop);
sheet.BottomSheetShow(backdrop);
outgoingPage.PagePushTo(incomingPage);
```

Toast, modal, tooltip, dropdown, tab, drawer, bottom-sheet, page-push, and page-cross-fade operations coordinate anchored position, scale, alpha, optional backdrops, and child staggering through one `TweenHandle`. They preserve authored UI baselines, support interruption and rewind, and remain semantic operations rather than registered presets.

Animate TextMesh Pro content and numeric values without expanding the preset registry:

```csharp
title.TypewriterReveal();
score.NumberCountTo(0, 1250, format: "N0");
message.TextCharacterStaggerIn(UISequenceDirection.Up);
message.TextWave(amplitude: 12f);
message.TextColorSweep();
message.TextScrambleReveal(seed: 1729);
score.ScoreIncrease(1200, 1475, format: "N0");
```

Typewriter operations preserve rich-text markup by animating `maxVisibleCharacters`. Stagger, wave, bounce, color-sweep, glitch, and emphasis operations evaluate visible TMP glyphs through one owner-linked tween and restore the original mesh exactly. Scramble Reveal preserves markup while resolving deterministic substitute glyphs. Number counting accepts increasing or decreasing values, while Score Increase combines an exact count destination with temporary scale and color feedback.

Animate normalized Image or Slider progress with optional synchronized text:

```csharp
healthFill.FillDrain(0.15f);
manaSlider.FillCharge(0.92f);
objectiveFill.FillAndText(0.08f, 0.9f, percentageLabel, "P0");

TweenHandle handle = progressImage.FillFromTo(0.15f, 0.82f)
    .OnProgress(0.5f, TriggerMilestone);
```

Image progress targets require a visible sprite and `Image.Type.Filled`; Slider operations use `normalizedValue` while preserving the authored minimum and maximum. Alert Pulse intentionally leaves the value fixed and animates only warning color/scale.

Apply transient camera feedback without accumulating pose or lens changes:

```csharp
gameplayCamera.CameraImpact();
gameplayCamera.CameraRecoil();
gameplayCamera.CameraFocusZoom(focusTarget);
```

Impact, recoil, landing, FOV kick, focus zoom, and one finite breathing cycle restore the exact camera pose and field of view on completion, rewind, or interrupted kill.

Animate common engine properties through target-linked direct or builder APIs:

```csharp
music.AudioVolumeTo(0.35f);
voice.AudioPitchTo(1.2f);
torch.LightIntensityTo(2.5f);
torch.TorchFlicker(options: TweenOptions.WithLoops(-1));
particles.ParticleEmissionRateTo(1.5f);
renderer.MaterialFloatTo("_Dissolve", 1f);
```

Renderer operations use `MaterialPropertyBlock`; ambient light helpers are finite cycles suitable for explicit root looping. Retain and kill infinite handles during teardown.

## Animation Gallery

The shipped mouse-driven Animation Gallery contains 406 entries across eight categories: 300 presets, 13 UI recipes, 19 collection examples, 12 destination operations, 25 gameplay-feedback/macro examples, 16 production UI sequences, 13 text/value examples, and eight camera operations. Selection resets and auto-plays; contextual controls and the displayed C# call update together.

## Preset browser

Open **Tools > Tween Helper > Preset Browser** to search 446 isolated previews: all 300 registered presets plus 146 UI, collection, destination, gameplay, UI-sequence, TextMesh Pro, progress, camera, and engine-property entries. Select an entry to inspect its metadata and fluent API example. Contextual direction, topology, phase, target, backdrop, and property options update the preview and copied example together.

The browser builds an isolated fixture appropriate to the selected entry rather than reusing the active scene. UI sequences receive only their required backdrop, controls, and incoming target; component operations receive visible progress, camera, audio, light, particle, or renderer fixtures. Preview proxies synchronize depth, sorting, Graphic color, parent `CanvasGroup` alpha, fill/value state, and projected UI state. The active scene is never read or modified.

## Support

Open **Tools > Tween Helper > Setup & Support** to prepare a bug report, feature request, documentation question, or other support email. Users choose up to five predefined tags, receive a matching message template they can insert and edit, and may optionally include the Tween Helper version, Unity version, operating system, and active render pipeline.

Only the selected Tween Helper-related environment information is added. Tween Helper does not collect project names, scenes, assets, logs, files, or machine identifiers. The report is copied to the clipboard and opened in the user's default email client for review; it is never sent automatically.

## Settings

No settings asset is required. TweenHelper uses safe in-memory defaults when `Resources/TweenHelperSettings` is absent. Choose **Tools > Tween Helper > Settings > Create Settings Asset** only when the project needs customized defaults.

## Async and cancellation contract

- Normal completion completes `TweenAsync.AwaitCompletion`.
- Killing a tween ends the internal wait without reporting a normal completion.
- Cancelling an await kills the active tween and throws `OperationCanceledException`.
- A timeout kills the active tween and returns `false`.
- Infinite loops never complete normally; kill or cancel them explicitly.
- Callback registration is additive and does not replace callbacks already attached to the tween.

## More documentation

- [Installation](Documentation/Installation.md)
- [API examples](Documentation/API.md)
- [Staggered collections](Documentation/StaggeredCollections.md)
- [Destination-aware motion](Documentation/DestinationMotion.md)
- [Gameplay feedback sequences](Documentation/FeedbackSequences.md)
- [Production UI sequences](Documentation/UISequences.md)
- [Text and value animations](Documentation/TextAndValueAnimations.md)
- [Camera feedback](Documentation/CameraFeedback.md)
- [Engine property animations](Documentation/EnginePropertyAnimations.md)
- [Preset catalog](Documentation/PresetCatalog.md)

## Licensing

Tween Helper is distributed under the Standard Unity Asset Store EULA.

DOTween is installed and licensed separately and is not included with Tween Helper. Tween Helper-owned content is governed by the Standard Unity Asset Store EULA.
