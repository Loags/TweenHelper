# Text and value animations

Tween Helper provides 23 semantic TextMesh Pro operations through matching one-line extensions and composable `TweenBuilder` steps. These are parameterized operations, not entries in the 300-preset registry.

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
        title.TypewriterReveal(TextAnimationUnit.Word);
    }

    public void AddScore(int previousScore, int currentScore)
    {
        score.ScoreIncrease(previousScore, currentScore, format: "N0");
    }
}
```

## Included operations

| Operation | Default duration | Purpose |
| --- | ---: | --- |
| `TypewriterReveal` | `0.85s` | Reveals characters, words, or lines without changing the source text. |
| `TypewriterHide` | `0.65s` | Hides characters, words, or lines sequentially. |
| `NumberCountTo` | `0.8s` | Counts in either direction between explicit numeric values. |
| `TextStaggerIn` | `0.65s` | Reveals ordered character, word, or line groups with movement, alpha, and scale. |
| `TextStaggerOut` | `0.58s` | Hides ordered character, word, or line groups with movement, alpha, and scale. |
| `TextWave` | `0.8s` | Sends one or more finite directional waves across visible glyphs. |
| `TextCharacterBounce` | `0.72s` | Sends one finite traveling bounce across visible glyphs. |
| `TextColorSweep` | `0.78s` | Sweeps a temporary highlight through per-character vertex colors. |
| `TextGlitch` | `0.52s` | Applies deterministic seeded offset, scale, and color slices. |
| `TextEmphasis` | `0.55s` | Temporarily lifts, scales, and colors a selected visible-character range. |
| `TextWiggle` | `0.65s` | Plays one smooth deterministic position-and-rotation cycle. |
| `TextFloat` | `0.9s` | Plays one phase-offset directional float cycle. |
| `TextSwing` | `0.8s` | Swings glyphs around a center or top pivot. |
| `TextPulse` | `0.7s` | Plays one phase-offset per-glyph scale cycle. |
| `TextScatterIn` | `0.75s` | Resolves deterministic scattered poses into the authored layout. |
| `TextScatterOut` | `0.65s` | Scatters authored text into deterministic poses and finishes hidden. |
| `TextRotateIn` | `0.65s` | Rotates ordered groups into the authored layout. |
| `TextRotateOut` | `0.58s` | Rotates ordered groups out and finishes hidden. |
| `TextShear` | `0.6s` | Applies a finite horizontal glyph deformation. |
| `TextTrackingPulse` | `0.7s` | Expands glyph positions from the visual center without changing TMP layout. |
| `TextImpactRipple` | `0.75s` | Propagates a finite local-space radial reaction through glyph centers. |
| `TextScrambleReveal` | `0.9s` | Resolves deterministic substitute glyphs into the original source text. |
| `ScoreIncrease` | `0.9s` | Counts upward while applying temporary scale and color feedback. |

## Units, order, and deterministic seeds

`TextAnimationUnit` supports `Character`, `Word`, and `Line`. Typewriter remains sequential. Use mesh-based stagger, scatter, or rotate transitions when ordering must be `FirstToLast`, `LastToFirst`, `FromCenter`, `ToCenter`, or seeded `Random`.

```csharp
label.TextStaggerIn(
    unit: TextAnimationUnit.Word,
    order: StaggerOrder.FromCenter,
    direction: UISequenceDirection.Up);

label.TextScatterOut(
    unit: TextAnimationUnit.Line,
    order: StaggerOrder.Random,
    seed: 1729);
```

Stagger defaults to an `18`-unit offset and `0.025s` between group starts. Long labels compress their start offsets into the requested duration. Stagger, scatter, and rotate share the same order and timing path. Glitch, wiggle, and scatter share deterministic noise, so the same seed produces the same poses on replay.

## Builder composition

Every operation is also available on `TweenBuilder`:

```csharp
TweenHandle handle = title.Tween()
    .TypewriterReveal(TextAnimationUnit.Word, 0.7f)
    .Then()
    .TextColorSweep()
    .Then()
    .TextCharacterBounce(UISequenceDirection.Up, amplitude: 10f)
    .Play();
```

Independent mesh-writing operations on the same label must be sequenced with `Then()`; do not join them with `With()`. Tween Helper rejects a second simultaneous mesh writer before it can mutate the label. Whole-label operations can still run on another target or in a deliberately separate sequence.

An explicit method duration wins over `TweenOptions.Duration`, which wins over the operation default. Delay, ID, loops, update mode, unscaled time, ease, strength, and target linking apply at the operation root. Speed-based timing is rejected because text operations use normalized semantic timing.

Finite active effects play one cycle. Repeat them with `TweenOptions` rather than relying on an internal infinite loop:

```csharp
TweenHandle handle = label.TextFloat(
    options: TweenOptions.WithLoops(-1, DG.Tweening.LoopType.Yoyo));
```

Retain and kill infinite handles during owner teardown.

## Formatting values

`NumberCountTo` and `ScoreIncrease` accept explicit start and destination values. They never parse arbitrary label content.

```csharp
score.NumberCountTo(0, 1250, format: "N0");
distance.NumberCountTo(0, 12.5, value => $"{value:0.0} km");
```

Format strings use the current culture. Use the formatter overload for units, localization, or project-specific formatting. The formatter is evaluated explicitly on normal completion, so the final displayed value is exact.

## Rich text, whitespace, and mesh data

Typewriter changes `TMP_Text.maxVisibleCharacters`; it does not split or rewrite `TMP_Text.text`. Unit boundaries come from `TMP_TextInfo`, so rich-text tags remain intact.

Mesh effects:

- Support both `TextMeshProUGUI` and world-space `TextMeshPro`.
- Animate visible glyphs while preserving whitespace, line breaks, and other layout-only characters.
- Preserve alignment, wrapping, vertex colors, invisible glyphs, and every material submesh.
- Capture state when playback starts, not when a builder is created.
- Recapture safely when TMP rebuilds because text, properties, character count, or mesh data changed.
- Use one normalized tween for the label rather than one tween per glyph.
- Restore the exact captured mesh after transient completion, rewind, and interrupted kill.

`TextScrambleReveal` temporarily replaces only supported single-code-unit visible glyphs. Tags, whitespace, and unsupported multi-code-unit glyphs are left untouched, and the exact source string is restored.

## Completion and interruption

- Typewriter completion leaves the requested fully shown or hidden state. Interrupted kill preserves current progress; rewind restores invocation visibility.
- Number Count completion writes the exact destination. Interrupted kill preserves current progress; rewind restores invocation text.
- Stagger, scatter, and rotate in-transitions restore the authored mesh and finish fully visible.
- Stagger, scatter, and rotate out-transitions restore the authored mesh and finish with `maxVisibleCharacters = 0`.
- Interrupted kill and rewind of mesh transitions restore the invocation mesh and visibility.
- Wave, bounce, color sweep, glitch, emphasis, wiggle, float, swing, pulse, shear, tracking pulse, and impact ripple restore the invocation mesh on completion, kill, and rewind.
- Scramble Reveal finishes with the exact source string fully visible; interrupted kill and rewind restore the invocation source and visibility.
- Score Increase leaves the destination text while restoring transient scale, rotation, and color; rewind restores the invocation text and visuals.
- Destroying the TMP target kills the linked root through DOTween's normal link behavior.

Yoyo loops remain finite when given a finite loop count, and an even Yoyo completion returns transient mesh effects to their captured baseline.

## Impact points in TMP-local coordinates

`TextImpactRipple` receives its origin in the TMP object's local coordinate space. Convert input at the call site so the animation engine does not need to find a camera.

For UI TMP, convert a screen pointer to the label's `RectTransform` space:

```csharp
RectTransform rect = (RectTransform)uiLabel.transform;
RectTransformUtility.ScreenPointToLocalPointInRectangle(
    rect,
    pointerPosition,
    eventCamera,
    out Vector2 localPoint);

uiLabel.TextImpactRipple(localPoint);
```

Use `null` for `eventCamera` on a Screen Space - Overlay canvas. For world-space TMP, convert a world hit through the label transform and choose radius/amplitude values in that local scale:

```csharp
Vector3 localHit = worldLabel.transform.InverseTransformPoint(hit.point);
worldLabel.TextImpactRipple(
    new Vector2(localHit.x, localHit.y),
    radius: 0.8f,
    amplitude: 0.12f);
```

## Animation Gallery and Preset Browser

Open **Text & Values** in the Animation Gallery to compare 22 customer-facing entries. The baseline examples remain, and nine representative entries cover the new wiggle, float, swing, pulse, scatter, rotate, shear, tracking, and ripple engines. Contextual unit, order, direction, pivot, seed, and UI/world controls update both the preview and its displayed C# call.

The Preset Browser routes every public text operation through an isolated fixture. The internal review scene keeps exhaustive unit, order, direction, pivot, seed, UI, and world-space configurations without duplicating those parameter-only variants in the customer gallery.

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

`FillTo` captures the current normalized value. `FillFromTo` uses explicit endpoints. `ValueFillTo` and `FillAndText` update an optional TMP label on the same timeline. Drain and Charge add temporary impact/overshoot feedback; Alert Pulse changes no value and activates only at or below its threshold. Completion keeps the requested value while restoring transient visuals. Interrupted kill and rewind restore the invocation value, text, transform, and supported color.

### Image setup

An `Image` progress target must have a visible sprite assigned and use `Image.Type.Filled`. Tween Helper animates `fillAmount` without replacing the authored fill method or origin. A color-only Image with no sprite cannot display partial fill even when its numeric `fillAmount` changes.

Place a paired TMP percentage label over or beside the bar and pass it to `ValueFillTo` or `FillAndText`. The review and Preset Browser fixtures use a filled UI sprite and an overlaid label so fill motion and formatted text remain visible together.
