# Motion preferences

Reduced motion is a package-local preference for newly created animations. It does not change DOTween time scale, the host engine, durations, delays, callback order, cancellation, or unrelated tweens. It is an animation option, not a claim of accessibility conformance.

Set **Motion Preference** on a TweenHelperSettings asset, or apply the player's saved preference at startup:

```csharp
using LB.TweenHelper;

TweenMotion.Preference = TweenMotionPreference.Reduced;
```

Override an individual builder step with `.WithMotionPreference(TweenMotionPreference.Full)` or pass `TweenOptions.WithMotionPreference(TweenMotionPreference.Reduced)` to a preset/direct extension. Like other builder options, a step override belongs to the selected step; use it for each step requiring an override. `UseProjectDefault` resolves the runtime preference, then the settings asset (Full by default).

TweenPlayer has its own **Motion Preference** field. Its `SetReducedMotion(bool)` method accepts a Toggle's dynamic `onValueChanged` event. Recipe preview uses the same preference. The authored `Samples/TweenHelper Demos/Prefabs/UI/ProgressRewardDemo.prefab` demonstrates this wiring. Place it under an existing Canvas with an EventSystem, enter Play Mode, and use the TweenPlayer Inspector to replay after changing the toggle.

## Covered behavior

| Family | Reduced behavior | Preserved outcome |
| --- | --- | --- |
| Strength-aware presets and transient feedback | Decorative amplitude and overshoot use 20% of the requested magnitude | Completion and restoration contracts |
| Single-axis Spin presets | Shortest rotation to the original final orientation; whole turns disappear | Final orientation, including custom strength |
| Camera impact, recoil, landing, FOV kick, breathing and focus zoom | Smaller position/rotation/FOV accents | Original camera pose after the effect |
| Destination arcs/hops and UI transitions | Smaller decorative arc, hop and depth accents | Requested destination and visibility |
| TMP mesh effects | Smaller strength-aware offsets/rotation/scale accents | Text contents, visibility and restoration |
| Progress/value animations | Smaller decorative feedback | Exact requested numeric value |
| Move/rotate/scale-to, fades, color, engine values | Semantic transition retained | Requested final property |

Preference changes apply when constructing the animation. Existing animations keep their captured magnitudes; kill and rebuild to apply a new preference. Infinite animations still require explicit cancellation or kill. Loop counts and joined/sequential ordering remain unchanged. Positive-duration validation remains unchanged; reduced motion never turns an animation into a zero-duration callback shortcut.

Custom presets participate when they use the inherited strength/overshoot helpers. Hard-coded motion, raw DOTween tweens, explicit semantic travel/rotation, and unsupported third-party effects are not automatically replaced. For a UI where even semantic travel is undesirable, select a fade-only recipe in your own preference UI. Reduced mode does not disable flashing or replace color-only feedback; assess those separately for your audience.
