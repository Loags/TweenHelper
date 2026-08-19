# Text and value animations

Tween Helper provides twelve semantic TextMesh Pro operations through type-safe one-line extensions and composable `TweenBuilder` steps. They do not add entries to the 300-preset registry.

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
| `TextCharacterStaggerOut` | `0.58s` | Hides visible glyphs in reverse order with offset, alpha, scale, and stagger. |
| `TextWave` | `0.8s` | Sends one or more finite directional waves across visible glyphs. |
| `TextCharacterBounce` | `0.72s` | Sends one finite traveling bounce across visible glyphs. |
| `TextColorSweep` | `0.78s` | Sweeps a temporary highlight through per-character vertex colors. |
| `TextGlitch` | `0.52s` | Applies a deterministic seeded offset, scale, and color glitch. |
| `TextEmphasis` | `0.55s` | Temporarily lifts, scales, and colors a selected visible-character range. |
| `TextScrambleReveal` | `0.9s` | Resolves deterministic substitute glyphs into the original source text. |
| `ScoreIncrease` | `0.9s` | Counts upward while applying temporary scale and color feedback. |

Character stagger defaults to `UISequenceDirection.Up`, an `18`-unit offset, and `0.025s` between character starts. `Up`, `Down`, `Left`, and `Right` are supported by character stagger, wave, bounce, and emphasis operations. Long labels compress their start offsets into the requested total duration. `TextWave` defaults to an upward `12`-unit wave and one sweep. `TextGlitch` and `TextScrambleReveal` accept a seed so capture, replay, and automated demos remain deterministic.

## Builder composition

Every operation is also available on `TweenBuilder`:

```csharp
TweenHandle handle = title.Tween()
    .TypewriterReveal(0.7f)
    .Then()
    .TextColorSweep()
    .With()
    .TextCharacterBounce(UISequenceDirection.Up, amplitude: 10f)
    .Play();
```

An explicit method duration wins over `TweenOptions.Duration`, which wins over the operation default. Delay, ID, loops, update mode, and unscaled time apply at the operation root. Speed-based timing is rejected because these operations use normalized semantic timing.

`TweenOptions.WithStrength` scales character distance, mesh-effect amplitude, temporary color emphasis, and Score Increase feedback. It never changes a numeric destination or source string.

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

Character Stagger, Text Wave, Character Bounce, Color Sweep, Glitch, and Emphasis:

- Work with `TextMeshProUGUI` and world-space `TextMeshPro`.
- Animate visible TMP elements while skipping layout-only characters such as line breaks.
- Preserve alignment, wrapping, rich text, vertex colors, and multiple material submeshes.
- Use one normalized tween for the whole label instead of creating one tween per glyph.
- Recapture the current mesh if TMP reports a text or layout rebuild during playback.

`TextScrambleReveal` temporarily replaces only single-code-unit visible glyphs in the source string. TMP rich-text tags, whitespace, and unsupported multi-code-unit glyphs are left untouched, and the exact source string is restored at the end.

Do not run two character-mesh-writing operations in parallel on the same label. Also avoid overlapping Scramble Reveal with another operation that writes `TMP_Text.text`. Sequence them with `Then()`, or use separate TMP targets.

## Completion and interruption

- Typewriter completion leaves all characters shown or hidden according to the operation.
- Number Count completion writes the exact formatted destination.
- Killing Typewriter or Number Count preserves the current visible/counting progress.
- Rewinding Typewriter or Number Count restores the value captured when playback began.
- Character Stagger In and the transient mesh effects restore the current TMP mesh baseline on completion, interrupted kill, and rewind.
- Character Stagger Out restores the mesh baseline and finishes with `maxVisibleCharacters = 0`; interrupted kill and rewind restore the invocation visibility.
- Scramble Reveal finishes with the exact source text fully visible; interrupted kill and rewind restore the source text and invocation visibility.
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

## Animation Gallery

Open **Text & Values** to compare thirteen curated examples for the twelve public operation families. Count-up and count-down are shown separately; directional and target-context controls update the preview and C# call.

## Progress fills and sliders

The value binding API uses normalized `0..1` values for both `Image.fillAmount` and `Slider.normalizedValue`, preserving each Slider's authored `minValue` and `maxValue`.

```csharp
healthFill.FillDrain(0.35f);
manaSlider.FillCharge(0.9f);
objectiveFill.FillAndText(0.2f, 0.75f, objectiveLabel, "P0");
healthFill.FillAlertPulse(0.2f);

panel.Tween()
    .FillFromTo(0f, 1f)
    .Then()
    .ValueFillTo(0.5f, percentageLabel)
    .Play();
```

`FillTo` captures the current normalized value. `FillFromTo` uses explicit endpoints. `ValueFillTo` and `FillAndText` update an optional TMP label on the same timeline. Drain and Charge add temporary impact/overshoot feedback; Alert Pulse changes no value and only activates at or below its threshold. Completion keeps the requested value while restoring transient visuals. Interrupted kill and rewind restore the invocation value, text, transform, and supported color.

### Image setup

An `Image` progress target must have a visible sprite assigned and use `Image.Type.Filled`. Select the required fill method and origin in the Inspector; Tween Helper animates `fillAmount` and does not replace those authored choices. A color-only Image with no sprite cannot display partial horizontal fill even though its numeric `fillAmount` changes.

Place a paired TMP percentage label over or beside the bar and pass it to `ValueFillTo` or `FillAndText`. The review and Preset Browser fixtures use a filled UI sprite and an overlaid label so fill motion and formatted text remain visible together.

`FillAlertPulse` is intentionally not a fill animation. It preserves the current Image/Slider value and applies a finite color/scale warning only when the value is at or below the threshold. `FillCharge` combines value movement with a temporary overshoot pulse; `FillDrain` moves toward the requested lower value with a short impact accent.
