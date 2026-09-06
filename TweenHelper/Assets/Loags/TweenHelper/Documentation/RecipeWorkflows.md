# Complete recipe workflows

The catalog contains 28 operations. Existing serialized operation values remain unchanged; Progress Fill To is appended as value 27. Existing recipes require no data migration.

| Workflow asset | Explicit bindings | Result |
| --- | --- | --- |
| MultiBindingPopup | Panel, backdrop, icon | Parallel panel entrance, backdrop fade and icon scale |
| RewardPresentation | Reward GameObject | Reward reveal followed by success feedback |
| ProgressReward | Filled Image or Slider, reward GameObject | Fill to 100%, then confirm the reward |
| CollectionEntrance | Ordered collection of targets supporting PopInFade | Staggered entrance in the binding's array order |

All assets live in `Samples/TweenHelper Demos/Recipes`. Assign one to a TweenPlayer, choose **Sync Bindings**, and wire its targets. Scene references stay on the player. Each recipe is a presentation; it does not modify inventory, grant rewards, or implement game state.

## Progress Fill To

Choose **Text And Values > Progress Fill To**, supply a GameObject binding and a **Normalized Value** from 0 to 1. Use an Image with **Type = Filled** and a sprite, or a Slider. Slider values are normalized across its configured minimum and maximum. Values outside the range, NaN/infinity, missing components and non-filled Images fail validation before playback.

The operation reuses the same value-animation utility as:

```csharp
using LB.TweenHelper;
using UnityEngine;

public sealed class ProgressPresentation : MonoBehaviour
{
    [SerializeField] private GameObject progress;
    private TweenHandle _animation;

    public void ShowProgress(float normalizedValue)
    {
        _animation?.Kill();
        _animation = progress.Tween().FillTo(normalizedValue, 0.4f).Play();
    }

    private void OnDisable() => _animation?.Kill();
}
```

Completion retains the destination. Rewind and interrupted kill restore the value captured at initialization. Recipe Editor preview snapshots and restores both Image fill amounts and Slider values. Reduced motion preserves the destination.

## Authored sample

Place `Prefabs/UI/ProgressRewardDemo.prefab` under an existing Canvas with an EventSystem. It contains the player, progress Image, reward visual, text and a Toggle wired to `TweenPlayer.SetReducedMotion`. The recipe plays on Start using unscaled time. Toggle the preference and replay through the Inspector to compare modes. Rewind before replay when you want the original progress baseline.

The original four Gallery recipe entries remain unchanged. The three additional workflow assets are reusable samples, so they do not inflate the Gallery or Browser counts. Additional TMP and collection operations were assessed but not added: the existing Typewriter Reveal, Number Count To and Collection Preset operations cover these workflows without another binding model.
