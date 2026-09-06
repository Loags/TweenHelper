# Lifecycle and migration

## Ownership

Retain the `TweenHandle` for work you may replace or interrupt. A builder links its root tween to the target; `TweenPlayer` owns one handle and kills it on disable and destruction. For custom controllers and pooled objects, release the handle when the controller stops owning the animation. Destroy linking alone does not define your pool-disable behavior.

| Action | Playback and await behavior | State considerations |
| --- | --- | --- |
| `Build()` | Creates a paused root; `Play()` or `Resume()` starts it | Some preset factories establish an initial pose while building; paused does not mean no construction-time mutation |
| Natural finite completion | Completion callbacks run; waits settle | Transient effects restore their captured state; destination/value operations may intentionally retain the destination |
| `Kill()` | Stops playback; completion waits settle without distinguishing kill from success; boolean timeout waits return false | Restoration follows the operation's documented contract |
| `Complete()` | Drives the handle to completion | Final pose and callbacks follow the root tween and operation contract |
| `Rewind()` | Rewinds an active retained tween | A rewound tween has not naturally completed; a pending completion wait can remain pending |
| `Restart()` | Restarts an active retained tween | An autokilled tween cannot be restarted; build another handle or retain the tween with autokill disabled |
| Player disable/destroy | `TweenPlayer` kills its owned handle | Bind targets explicitly; do not rely on hierarchy lookup |
| Infinite loops | No normal completion | Kill, cancel, or apply a timeout explicitly |
| Token cancellation | Kills the active observed tween and throws `OperationCanceledException` | Tween mutation occurs on the captured Unity main thread |
| Timeout | Kills once and returns false | Timeout uses elapsed clock time, so a paused game still requires the Unity main thread to process cleanup |

The focused guide for each operation is authoritative about retained destinations and restoration. Rewind/kill is not a universal undo for arbitrary caller callbacks or injected raw DOTween behavior.

## Async details

- Start waits and all tween API calls on Unity's main thread. Cancellable active waits require its synchronization context.
- A retained tween that is already complete settles immediately, including the custom tween awaiter. Its existing completion takes precedence over a newly supplied canceled token.
- An already-canceled token cancels an otherwise active, incomplete tween.
- Completion, kill, and cancellation compete for one terminal outcome; callbacks are additive and owned subscriptions are released.
- `AwaitAny` observes the winning wait's cancellation/failure. Normal completion detaches losing observers without killing their tweens. Canceling the shared token requests cancellation for active observed tweens.
- `AwaitAll` waits for every supplied wait. Retain and cancel work deliberately when a larger operation should stop.
- Explicit cancellation takes precedence over timeout reporting when both are signaled by the time the timeout wrapper observes cancellation.
- Shutdown and Editor transitions release pending observers; they must not keep targets alive indefinitely.

## DOTween configuration migration

Earlier Tween Helper startup applied its settings to global DOTween defaults automatically. The revised default preserves the host project's global configuration. No settings asset is required.

Tween Helper still applies its own configured playback defaults to its own factory-created tweens. Builder `Build()` returns paused, `Play()` explicitly starts playback, and recipes retain their root sequence for rewind/restart. Calls to the public `WithDefaults` helper explicitly apply Tween Helper defaults to the supplied tween.

To opt in to the previous global-configuration workflow:

1. Open **Tools > Tween Helper > Settings > Create Settings Asset** or select your existing settings asset.
2. Enable **Configure Dotween Engine**.
3. Review the engine-global fields before entering Play Mode. Capacities are applied only when no active tweens exist.

Existing settings assets receive the new ownership field in its disabled state. If your game's raw DOTween calls depended on Tween Helper changing autoplay, autokill, update mode, easing, or logging, either configure those defaults in the host game or explicitly opt in. Tween Helper no longer clears the entire shared DOTween engine on application quit.

## Presets and stripped players

Built-in presets now have explicit constructor registrations. The development exporter maintains that list; customers do not need the exporter or a blanket assembly-preservation file.

Custom `[AutoRegisterPreset]` types are still discovered through reflection. For a custom preset referenced only by a string, preserve the concrete class and its public default constructor with Unity's `[UnityEngine.Scripting.Preserve]`, or register it explicitly through `TweenPresetRegistry.RegisterPreset(new YourPreset())` before use. Test the exact custom setup in your stripped player; Editor discovery does not establish player compatibility.

Built-in registration code improves linker visibility. It is not a claim that every platform/backend has been validated; consult [installation and compatibility](Installation.md).

## Update installation

Preserve existing asset GUIDs and recipe binding IDs when updating. After import, rerun setup validation, open the Browser, validate existing players, and review bindings before previewing. Importing a `.unitypackage` may leave files that were removed in a newer package; consult the release notes and remove only confirmed obsolete files, keeping project-specific assets intact.

## Troubleshooting

| Symptom | Check |
| --- | --- |
| Missing DOTween/modules assembly | Install DOTween separately and regenerate modules/assembly definitions using its utility panel |
| Missing or invisible TMP demo content | Import TMP Essential Resources and check the target's assigned font |
| Progress Image does not visibly change | Assign a sprite and set Image Type to Filled |
| Layout snaps the target back | Check whether a LayoutGroup owns the same transform; use the documented layout-transition capture workflow |
| Reused object continues animating while disabled | Kill the controller's owned handle in its disable/pool-return path |
| Restart does nothing | The old tween may have autokilled; create a new handle or use a retained recipe player |
| Recipe validation fails after editing | Sync Bindings and review stale/missing assignments rather than substituting hierarchy searches |
| Wait hangs after a pause | Check infinite loops, whether playback was started, and whether the main thread is blocked |

See [Quick start](QuickStart.md) for complete ownership examples.
