# Production UI sequences

Production UI sequences coordinate position, scale, and alpha around semantic interface events. They are `TweenBuilder` operations and direct extensions rather than registered presets, so the built-in preset catalog remains at 300 entries.

All sequence targets require a `RectTransform`. Local movement uses `anchoredPosition3D`, preserving the authored anchored Z value. Fading supports `CanvasGroup`, `Graphic`, and the other alpha targets supported by Tween Helper.

## API

```csharp
TweenBuilder ToastShow(UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null);
TweenBuilder ToastHide(UISequenceDirection direction = UISequenceDirection.Up, float distance = 56f, float? duration = null);

TweenBuilder ModalOpen(GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f);
TweenBuilder ModalClose(GameObject backdrop = null, IEnumerable<GameObject> controls = null, float? duration = null, float childStagger = 0.045f);

TweenBuilder TooltipShow(UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null);
TweenBuilder TooltipHide(UISequenceDirection direction = UISequenceDirection.Up, float distance = 16f, float? duration = null);

TweenBuilder DropdownOpen(IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f);
TweenBuilder DropdownClose(IEnumerable<GameObject> entries = null, float? duration = null, float childStagger = 0.035f);

TweenBuilder TabSwitchTo(GameObject incoming, UISequenceDirection direction = UISequenceDirection.Left, float distance = 72f, float? duration = null);
```

Each operation is also available directly on `GameObject` and `Component`. Direct extensions return `TweenHandle` and accept `TweenOptions` as their final optional argument.

```csharp
toast.ToastShow();
tooltip.TooltipHide(UISequenceDirection.Right);
panel.ModalOpen(backdrop, controls);
dropdown.DropdownOpen(entries);
outgoingTab.TabSwitchTo(incomingTab);
```

## Direction semantics

`UISequenceDirection` contains `Up`, `Down`, `Left`, and `Right`. It describes the visible travel direction:

- Show operations begin opposite the direction and travel toward the authored shown position.
- Hide operations travel from the shown position in the selected direction.
- `TabSwitchTo(Left)` moves outgoing content left and brings incoming content from the right.

Distances use canvas units, normally pixels.

## Families

### Toast

`ToastShow` slides and fades from a small scale, passes the shown position slightly, and settles exactly. `ToastHide` adds a short anticipation before sliding, fading, and reducing scale. Default durations are `0.4s` and `0.28s`.

### Modal

`ModalOpen` fades an optional backdrop, scales and fades the panel, and staggers optional controls into view. `ModalClose` staggers controls out in reverse order before dismissing the panel and backdrop. Default durations are `0.52s` and `0.38s`; the default control interval is `0.045s`.

When supplied, the backdrop must have a supported alpha target. Panel and control fading is applied when supported; their scale transition remains valid without one.

```csharp
TweenHandle handle = modalPanel.ModalOpen(
    modalBackdrop,
    new[] { title.gameObject, body.gameObject, confirmButton.gameObject },
    childStagger: 0.06f,
    options: TweenOptions.WithUnscaledTime());
```

### Tooltip

Tooltip transitions use a shorter distance and restrained scale change so they remain visually distinct from toast notifications. Default durations are `0.22s` for show and `0.16s` for hide.

### Dropdown

Dropdown transitions compress only local Y scale, so X and Z scale remain authored. The `RectTransform.pivot` determines where the panel visually expands or collapses; a top pivot creates a natural downward menu. Entries are staggered forward when opening and in reverse when closing. Default durations are `0.36s` and `0.26s`; the default entry interval is `0.035s`.

Animate a wrapper when a `LayoutGroup`, `ContentSizeFitter`, or another layout system controls the dropdown transform. UI sequences intentionally do not animate `sizeDelta` or layout dimensions.

### Tab switch

`TabSwitchTo` controls two required content containers through one root tween. Outgoing content moves and fades in the selected direction while incoming content begins on the opposite side and overlaps the exit. Normal completion leaves outgoing content hidden and incoming content on its exact authored shown state.

The outgoing and incoming objects must be different `RectTransform` targets.

## Composition and options

The operations compose like other builder steps:

```csharp
TweenHandle handle = toast.Tween()
    .ToastShow()
    .Then()
    .Delay(2f)
    .Then()
    .ToastHide()
    .OnComplete(() => toast.SetActive(false))
    .Play();
```

An explicit method duration wins over `TweenOptions.Duration`, which wins over the family default. `TweenOptions.WithStrength` scales travel distance, overshoot, and scale deviation without changing authored endpoints. Delay, ID, loops, update type, unscaled time, callbacks, and awaiting apply to the complete semantic sequence. Speed-based timing is rejected because these operations coordinate normalized phases rather than a single distance.

## Shown-state capture

The first UI helper or UI sequence used on a target caches its current authored state: anchored position, scale, rotation, and supported alpha/color. Show operations always finish on that state. Author reusable sequence targets in their visible state before first playback.

If responsive layout or application logic intentionally changes the shown state later, recapture it after layout has settled:

```csharp
panel.RefreshUIAnimationState();
```

Modal controls, dropdown entries, and other supplied collections are copied immediately. Null targets, duplicates, the sequence owner repeated as a child, invalid distances or durations, destroyed targets, and non-`RectTransform` targets are rejected with descriptive exceptions.

## Completion, kill, rewind, and interaction

- Show completion writes the exact cached shown position, scale, rotation, and alpha.
- Hide completion leaves the target visually hidden at the family-specific exit state.
- `TabSwitchTo` leaves outgoing content hidden and incoming content shown.
- Killing a sequence preserves its current visual state so another transition can continue from the interruption point.
- Rewind restores every involved target to the state captured when that sequence began.
- Restart and repeated playback reuse absolute cached endpoints without accumulating drift.
- A finite even-count Yoyo ends on the invocation state.
- Destroying the owner kills the linked root tween.

The helpers do not activate or deactivate GameObjects and do not change `interactable`, `blocksRaycasts`, focus, navigation, or selection. Manage those application states explicitly, commonly through callbacks.

```csharp
panel.ModalClose(backdrop, controls)
    .OnComplete(() =>
    {
        panel.SetActive(false);
        backdrop.SetActive(false);
    });
```
