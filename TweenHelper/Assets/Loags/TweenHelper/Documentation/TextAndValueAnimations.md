# Text and value animations

Tween Helper provides five TextMesh Pro animation families through type-safe one-line extensions and composable `TweenBuilder` steps. These are semantic operations and do not add entries to the 300-preset registry.

## Quick start

```csharp
using LB.TweenHelper;
using TMPro;
using UnityEngine;

public sealed class ScorePresentation : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text score;

    public void RevealTitle()
    {
        title.TypewriterReveal();
    }

    public void AddScore(int previousScore, int currentScore)
    {
        score.ScoreIncrease(previousScore, currentScore, format: "N0");
    }
}
```

## Included operations

| Operation | Default duration | Purpose |
|---|---:|---|
| `TypewriterReveal` | `0.85s` | Reveals TMP characters without changing the source text. |
| `TypewriterHide` | `0.65s` | Hides the currently visible TMP characters. |
| `NumberCountTo` | `0.8s` | Counts in either direction between explicit numeric values. |
| `TextCharacterStaggerIn` | `0.65s` | Reveals visible glyphs with offset, alpha, scale, and stagger. |
| `TextWave` | `0.8s` | Sends one or more finite directional waves across visible glyphs. |
| `ScoreIncrease` | `0.9s` | Counts upward while applying temporary scale and color feedback. |

`TextCharacterStaggerIn` defaults to `UISequenceDirection.Up`, an `18`-unit offset, and `0.025s` between character starts. Long labels compress their start offsets into the requested total duration. `TextWave` defaults to an upward `12`-unit wave and one sweep.

## Builder composition

Every operation is also available on `TweenBuilder`:

```csharp
TweenHandle handle = title.Tween()
    .TypewriterReveal(0.7f)
    .Then()
    .TextWave(UISequenceDirection.Up, amplitude: 10f)
    .Play();
```

An explicit method duration wins over `TweenOptions.Duration`, which wins over the operation default. Delay, ID, loops, update mode, and unscaled time apply at the operation root. Speed-based timing is rejected because these operations use normalized semantic timing.

`TweenOptions.WithStrength` scales character distance, wave amplitude, and Score Increase feedback. It never changes a numeric destination.

## Formatting values

`NumberCountTo` and `ScoreIncrease` accept explicit start and destination values. They never parse arbitrary label content.

Use a standard numeric format string:

```csharp
score.NumberCountTo(0, 1250, format: "N0");
```

Format strings use the current culture. Use the formatter overload for units, localization, or project-specific formatting:

```csharp
distance.NumberCountTo(0, 12.5, value => $"{value:0.0} km");
```

The destination formatter is evaluated explicitly on normal completion, so the final displayed value is exact rather than an accumulated approximation.

## Rich text and character meshes

Typewriter operations change `TMP_Text.maxVisibleCharacters`; they do not split or rewrite `TMP_Text.text`. Rich-text tags therefore remain intact.

Character Stagger and Text Wave:

- Work with `TextMeshProUGUI` and world-space `TextMeshPro`.
- Animate visible TMP elements while skipping layout-only characters such as line breaks.
- Preserve alignment, wrapping, rich text, vertex colors, and multiple material submeshes.
- Use one normalized tween for the whole label instead of creating one tween per glyph.
- Recapture the current mesh if TMP reports a text or layout rebuild during playback.

Do not run two character-mesh-writing operations in parallel on the same label. Sequence them with `Then()`, or use separate TMP targets.

## Completion and interruption

- Typewriter completion leaves all characters shown or hidden according to the operation.
- Number Count completion writes the exact formatted destination.
- Killing Typewriter or Number Count preserves the current visible/counting progress.
- Rewinding Typewriter or Number Count restores the value captured when playback began.
- Character Stagger and Text Wave restore the current TMP mesh baseline on completion, interrupted kill, and rewind.
- Score Increase completion leaves the destination score displayed and restores scale, rotation, and color.
- Killing Score Increase preserves its current displayed value while restoring transient scale, rotation, and color.
- Rewinding Score Increase restores the invocation text and visual state.
- Destroying the TMP target kills the linked root through DOTween's normal link behavior.

For an intentionally looping wave, loop the finite root operation:

```csharp
TweenHandle handle = label.TextWave(
    options: TweenOptions.WithLoops(-1, DG.Tweening.LoopType.Restart));
```

Retain and kill infinite handles during owner teardown.
