# TweenHelper

TweenHelper is a fluent animation builder and a catalog of reusable presets built on DOTween. It supports transform, UI, SpriteRenderer, and renderer targets while keeping preset playback, sequencing, cancellation, and reset behavior consistent.

This repository currently contains the `1.0.0-pre.1` Asset Store candidate. Unity `2022.3.0f1` or newer and DOTween `1.3.030` or newer are the declared baseline. DOTween is installed and licensed separately; it is not part of TweenHelper.

## Requirements

- Unity `2022.3.0f1` or newer.
- DOTween `1.3.030` or newer, installed separately from the [Unity Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676) or [Demigiant](https://dotween.demigiant.com/download.php).
- DOTween modules generated through **Tools > Demigiant > DOTween Utility Panel > Setup DOTween**.
- Unity UI (uGUI) and TextMesh Pro. Import TextMesh Pro Essential Resources before opening the demos.

DOTween is an external Asset Store dependency and is not redistributed with this package. Register the dependency in the Asset Store Publisher Portal when publishing TweenHelper. Use **Tools > TweenHelper > Validate > DOTween Setup** after installation for an actionable setup check.

## Installation

Install and set up DOTween first. Then import the TweenHelper `.unitypackage` from **Window > Package Management > My Assets**, or import a local release artifact with **Assets > Import Package > Custom Package**.

All distributable files are installed beneath `Assets/Loags/TweenHelper`. The included demos are in `Samples/TweenHelper Demos`. Runtime does not depend on the Input System or a scriptable render pipeline. The 3D demo materials are authored for URP, while the runtime API and 2D demo are render-pipeline independent.

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

## Preset browser

Open **Tools > TweenHelper > Preset Browser** to search and filter all registered presets. Select a scene GameObject to preview a compatible preset. **Stop and Restore** kills the preview and restores the selected object's transform and supported visual state. The browser also copies a valid fluent API example.

## Settings

No settings asset is required. TweenHelper uses safe in-memory defaults when `Resources/TweenHelperSettings` is absent. Choose **Tools > TweenHelper > Create Settings Asset** only when the project needs customized defaults.

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
- [Lifecycle and option semantics](Documentation/Lifecycle.md)
- [Preset catalog](Documentation/PresetCatalog.md)
- [Changelog](CHANGELOG.md)
- [Third-party notices](Third-Party%20Notices.txt)

## License

TweenHelper is licensed under the [MIT License](LICENSE.md). DOTween is a separate dependency governed by its own license; see [Third-Party Notices.txt](Third-Party%20Notices.txt).
