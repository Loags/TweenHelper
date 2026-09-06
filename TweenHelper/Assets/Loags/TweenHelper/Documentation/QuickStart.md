# Your first Tween Helper animations

Complete [installation](Installation.md) first: install DOTween separately, run its setup with the UI module, and import TMP Essential Resources for the Gallery. The examples below use existing GameObjects and Inspector-assigned references.

## 1. Animate an existing object

Attach this script to the object you want to animate. Enabling it plays an entrance; disabling it releases its animation handle.

```csharp
using LB.TweenHelper;
using UnityEngine;

public sealed class AnimatedCard : MonoBehaviour
{
    private TweenHandle _entrance;

    private void OnEnable()
    {
        _entrance = gameObject.Tween().Preset<PopInPreset>(0.35f).Play();
    }

    private void OnDisable()
    {
        _entrance?.Kill();
        _entrance = null;
    }
}
```

Expected result: the card scales into view. Keep the handle when you need to pause, stop, or replace playback. Presets are registered reusable effects; parameterized builder operations and complete UI/collection sequences offer additional workflows without increasing the preset count.

## 2. Compose a sequence and own its cancellation

Attach this script to a controller and assign an existing panel `Transform`. Call `Show` from a button or another script. The movement and scale run together; the pulse follows them. Calling `Show` again cancels the previous operation before starting the replacement.

```csharp
using System;
using System.Threading;
using LB.TweenHelper;
using UnityEngine;

public sealed class PanelPresentation : MonoBehaviour
{
    [SerializeField] private Transform panel;
    [SerializeField] private Vector3 shownLocalPosition;
    private CancellationTokenSource _playback;

    public async void Show()
    {
        StopPresentation();
        var playback = new CancellationTokenSource();
        _playback = playback;
        try
        {
            TweenHandle handle = panel.gameObject.Tween()
                .MoveLocal(shownLocalPosition, 0.3f)
                .With()
                .Scale(Vector3.one, 0.3f)
                .Then()
                .Preset<PulseScalePreset>(0.25f)
                .Play();
            await handle.AwaitCompletion(playback.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (_playback == playback) _playback = null;
            playback.Dispose();
        }
    }

    private void StopPresentation()
    {
        _playback?.Cancel();
        _playback = null;
    }

    private void OnDisable() => StopPresentation();
}
```

`async void` is used here only for the UnityEvent entry point. For ordinary code, prefer an async method returning `Task`. Required Inspector references must be assigned. Cancellation is expected during replacement and disable; unexpected failures remain visible.

Start tween operations and waits on Unity's main thread. A cancellation token may be signaled from another thread; Tween Helper dispatches the tween mutation back to the captured Unity synchronization context. Do not block the main thread with `.Wait()` or `.Result`.

## 3. Reuse an authored Tween Recipe

1. Find `PanelMoveFade` in `Samples/TweenHelper Demos/Recipes` and inspect its bindings and timeline in **Tools > Tween Helper > Recipe Editor**.
2. Add a `TweenPlayer` to an existing scene or prefab object and assign the recipe.
3. Choose **Sync Bindings**. Assign the explicit targets required by the recipe; a fade target needs a supported visual component such as a `CanvasGroup`.
4. Validate before previewing. Stop the preview before saving or applying prefab overrides.
5. For a Button's `On Click`, assign the player object and select **TweenPlayer > PlayFromEvent()**. From code, call `Play()` to receive the active handle.

```csharp
using LB.TweenHelper;
using UnityEngine;

public sealed class NotificationController : MonoBehaviour
{
    [SerializeField] private TweenPlayer notification;

    public void Show() => notification.Play();
    public void Hide() => notification.Kill();
}
```

Expected result: the assigned targets execute the same recipe with instance-specific bindings. Each player owns one active handle and releases it on disable/destroy. See [Tween Recipes](TweenRecipes.md) for supported operations, timing, and preview restrictions.

## Find the next effect

- **Preset Browser**: isolated Editor previews with contextual parameters and matching copied examples.
- **Animation Gallery**: runtime demonstrations of complete motion workflows.
- **Focused guides**: [UI](UISequences.md), [collections](StaggeredCollections.md), [TMP/value animations](TextAndValueAnimations.md), and [destination motion](DestinationMotion.md).
- [Lifecycle and migration](LifecycleAndMigration.md): cancellation, retained handles, global configuration, and custom presets.
