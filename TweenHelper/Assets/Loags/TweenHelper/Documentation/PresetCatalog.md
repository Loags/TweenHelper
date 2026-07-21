# TweenHelper preset catalog

> Generated deterministically from the registered TweenHelper preset types.

Built-in presets: **300**

Regenerate this file with **Tools > TweenHelper > Export Preset Catalog**. Generation fails on duplicate, empty, or unconstructible presets.

## Bounce

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Bounce` | 1s | One-shot | GameObject | Positional Y bounce with decreasing hops | `target.Tween().Preset<PositionalBouncePreset>().Play();` |
| `BounceCartoon` | 1.4s | One-shot | GameObject | Cartoon bounce with squash-stretch on landing | `target.Tween().Preset<BounceCartoonPreset>().Play();` |
| `BounceCartoonHard` | 1.1s | One-shot | GameObject | Hard cartoon bounce with exaggerated squash-stretch | `target.Tween().Preset<BounceCartoonHardPreset>().Play();` |
| `BounceCartoonSoft` | 1.8s | One-shot | GameObject | Soft cartoon bounce with gentle squash-stretch | `target.Tween().Preset<BounceCartoonSoftPreset>().Play();` |
| `BounceHard` | 1.3s | One-shot | GameObject | Heavy bounce with tall hops | `target.Tween().Preset<BounceHardPreset>().Play();` |
| `BounceLand` | 2s | One-shot | GameObject | Drop with bounce and squash-stretch on landing | `target.Tween().Preset<BounceLandPreset>().Play();` |
| `BounceLandHard` | 1.6s | One-shot | GameObject | Heavy drop with sharp bounce and exaggerated squash-stretch | `target.Tween().Preset<BounceLandHardPreset>().Play();` |
| `BounceLandSoft` | 2.4s | One-shot | GameObject | Gentle drop with soft bounce and mild squash-stretch | `target.Tween().Preset<BounceLandSoftPreset>().Play();` |
| `BounceSoft` | 0.9s | One-shot | GameObject | Soft bounce | `target.Tween().Preset<BounceSoftPreset>().Play();` |

## Breathe

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Breathe` | 4s | Infinite | GameObject | Gentle scale pulse loop | `target.Tween().Preset<BreathePreset>().Play();` |
| `BreatheHard` | 3s | Infinite | GameObject | Hard intense scale pulse loop | `target.Tween().Preset<BreatheHardPreset>().Play();` |
| `BreatheSoft` | 5s | Infinite | GameObject | Soft gentle scale pulse loop | `target.Tween().Preset<BreatheSoftPreset>().Play();` |

## Drop

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `DropIn` | 1.2s | One-shot | GameObject | Falls from above with bounce on landing | `target.Tween().Preset<DropInPreset>().Play();` |
| `DropInHard` | 0.9s | One-shot | GameObject | Heavy drop with sharp bounce decay | `target.Tween().Preset<DropInHardPreset>().Play();` |
| `DropInSoft` | 1.5s | One-shot | GameObject | Gentle drop with soft bounce on landing | `target.Tween().Preset<DropInSoftPreset>().Play();` |

## Fade

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `FadeIn` | 3s | One-shot | Fade-capable GameObject | Fades in from transparent (requires fadeable component) | `target.Tween().Preset<FadeInPreset>().Play();` |
| `FadeInHard` | 1s | One-shot | Fade-capable GameObject | Quick fade in from transparent (requires fadeable component) | `target.Tween().Preset<FadeInHardPreset>().Play();` |
| `FadeInOut` | 2s | One-shot | Fade-capable GameObject | Fade in then fade out | `target.Tween().Preset<FadeInOutPreset>().Play();` |
| `FadeInOutHard` | 1s | One-shot | Fade-capable GameObject | Quick fade in then quick fade out | `target.Tween().Preset<FadeInOutHardPreset>().Play();` |
| `FadeInOutSoft` | 3s | One-shot | Fade-capable GameObject | Slow fade in then slow fade out | `target.Tween().Preset<FadeInOutSoftPreset>().Play();` |
| `FadeInSoft` | 5s | One-shot | Fade-capable GameObject | Slow fade in from transparent (requires fadeable component) | `target.Tween().Preset<FadeInSoftPreset>().Play();` |
| `FadeOut` | 3s | One-shot | Fade-capable GameObject | Fades out to transparent (requires fadeable component) | `target.Tween().Preset<FadeOutPreset>().Play();` |
| `FadeOutHard` | 1s | One-shot | Fade-capable GameObject | Quick fade out to transparent (requires fadeable component) | `target.Tween().Preset<FadeOutHardPreset>().Play();` |
| `FadeOutSoft` | 5s | One-shot | Fade-capable GameObject | Slow fade out to transparent (requires fadeable component) | `target.Tween().Preset<FadeOutSoftPreset>().Play();` |

## Flip

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `FlipFadeX` | 1s | One-shot | Fade-capable GameObject | 180° flip on X axis with fade out | `target.Tween().Preset<FlipFadeXPreset>().Play();` |
| `FlipFadeXHard` | 0.5s | One-shot | Fade-capable GameObject | Quick 180° flip on X axis with fade out | `target.Tween().Preset<FlipFadeXHardPreset>().Play();` |
| `FlipFadeXSoft` | 1.6s | One-shot | Fade-capable GameObject | Slow 180° flip on X axis with fade out | `target.Tween().Preset<FlipFadeXSoftPreset>().Play();` |
| `FlipFadeY` | 1s | One-shot | Fade-capable GameObject | 180° flip on Y axis with fade out | `target.Tween().Preset<FlipFadeYPreset>().Play();` |
| `FlipFadeYHard` | 0.5s | One-shot | Fade-capable GameObject | Quick 180° flip on Y axis with fade out | `target.Tween().Preset<FlipFadeYHardPreset>().Play();` |
| `FlipFadeYSoft` | 1.6s | One-shot | Fade-capable GameObject | Slow 180° flip on Y axis with fade out | `target.Tween().Preset<FlipFadeYSoftPreset>().Play();` |
| `FlipFadeZ` | 1s | One-shot | Fade-capable GameObject | 180° flip on Z axis with fade out | `target.Tween().Preset<FlipFadeZPreset>().Play();` |
| `FlipFadeZHard` | 0.5s | One-shot | Fade-capable GameObject | Quick 180° flip on Z axis with fade out | `target.Tween().Preset<FlipFadeZHardPreset>().Play();` |
| `FlipFadeZSoft` | 1.6s | One-shot | Fade-capable GameObject | Slow 180° flip on Z axis with fade out | `target.Tween().Preset<FlipFadeZSoftPreset>().Play();` |
| `FlipX` | 0.5s | One-shot | GameObject | 180° flip on X axis | `target.Tween().Preset<FlipXPreset>().Play();` |
| `FlipXHard` | 0.25s | One-shot | GameObject | Quick 180° flip on X axis | `target.Tween().Preset<FlipXHardPreset>().Play();` |
| `FlipXSoft` | 0.8s | One-shot | GameObject | Slow 180° flip on X axis | `target.Tween().Preset<FlipXSoftPreset>().Play();` |
| `FlipY` | 0.5s | One-shot | GameObject | 180° flip on Y axis | `target.Tween().Preset<FlipYPreset>().Play();` |
| `FlipYHard` | 0.25s | One-shot | GameObject | Quick 180° flip on Y axis | `target.Tween().Preset<FlipYHardPreset>().Play();` |
| `FlipYSoft` | 0.8s | One-shot | GameObject | Slow 180° flip on Y axis | `target.Tween().Preset<FlipYSoftPreset>().Play();` |
| `FlipZ` | 0.5s | One-shot | GameObject | 180° flip on Z axis | `target.Tween().Preset<FlipZPreset>().Play();` |
| `FlipZHard` | 0.25s | One-shot | GameObject | Quick 180° flip on Z axis | `target.Tween().Preset<FlipZHardPreset>().Play();` |
| `FlipZSoft` | 0.8s | One-shot | GameObject | Slow 180° flip on Z axis | `target.Tween().Preset<FlipZSoftPreset>().Play();` |

## Float

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Float` | 6s | Infinite | GameObject | Gentle up/down hovering loop | `target.Tween().Preset<FloatPreset>().Play();` |
| `FloatHard` | 5s | Infinite | GameObject | Hard pronounced hovering loop | `target.Tween().Preset<FloatHardPreset>().Play();` |
| `FloatSoft` | 7s | Infinite | GameObject | Soft gentle hovering loop | `target.Tween().Preset<FloatSoftPreset>().Play();` |

## Heartbeat

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Heartbeat` | 0.8s | Infinite | GameObject | Double-pulse heartbeat loop | `target.Tween().Preset<HeartbeatPreset>().Play();` |
| `HeartbeatHard` | 0.6s | Infinite | GameObject | Hard intense heartbeat loop | `target.Tween().Preset<HeartbeatHardPreset>().Play();` |
| `HeartbeatSoft` | 1s | Infinite | GameObject | Soft double-pulse heartbeat loop | `target.Tween().Preset<HeartbeatSoftPreset>().Play();` |

## Jitter

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Jitter` | 0.25s | One-shot | GameObject | Tight rapid vibration | `target.Tween().Preset<JitterPreset>().Play();` |
| `JitterHard` | 0.4s | One-shot | GameObject | Intense rapid vibration | `target.Tween().Preset<JitterHardPreset>().Play();` |
| `JitterSoft` | 0.2s | One-shot | GameObject | Soft rapid vibration | `target.Tween().Preset<JitterSoftPreset>().Play();` |

## Launch

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `LaunchDown` | 0.4s | One-shot | GameObject | Quick downward motion with ease-out | `target.Tween().Preset<LaunchDownPreset>().Play();` |
| `LaunchDownHard` | 0.5s | One-shot | GameObject | Forceful downward motion with ease-out | `target.Tween().Preset<LaunchDownHardPreset>().Play();` |
| `LaunchDownSoft` | 0.3s | One-shot | GameObject | Gentle downward motion with ease-out | `target.Tween().Preset<LaunchDownSoftPreset>().Play();` |
| `LaunchLeft` | 0.4s | One-shot | GameObject | Quick leftward motion with ease-out | `target.Tween().Preset<LaunchLeftPreset>().Play();` |
| `LaunchLeftHard` | 0.5s | One-shot | GameObject | Forceful leftward motion with ease-out | `target.Tween().Preset<LaunchLeftHardPreset>().Play();` |
| `LaunchLeftSoft` | 0.3s | One-shot | GameObject | Gentle leftward motion with ease-out | `target.Tween().Preset<LaunchLeftSoftPreset>().Play();` |
| `LaunchRight` | 0.4s | One-shot | GameObject | Quick rightward motion with ease-out | `target.Tween().Preset<LaunchRightPreset>().Play();` |
| `LaunchRightHard` | 0.5s | One-shot | GameObject | Forceful rightward motion with ease-out | `target.Tween().Preset<LaunchRightHardPreset>().Play();` |
| `LaunchRightSoft` | 0.3s | One-shot | GameObject | Gentle rightward motion with ease-out | `target.Tween().Preset<LaunchRightSoftPreset>().Play();` |
| `LaunchUp` | 0.4s | One-shot | GameObject | Quick upward motion with ease-out | `target.Tween().Preset<LaunchUpPreset>().Play();` |
| `LaunchUpHard` | 0.5s | One-shot | GameObject | Forceful upward motion with ease-out | `target.Tween().Preset<LaunchUpHardPreset>().Play();` |
| `LaunchUpSoft` | 0.3s | One-shot | GameObject | Gentle upward motion with ease-out | `target.Tween().Preset<LaunchUpSoftPreset>().Play();` |

## Misc

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Attention` | 0.8s | One-shot | GameObject | Attention-grabbing pulse | `target.Tween().Preset<AttentionPreset>().Play();` |
| `AttentionHard` | 0.6s | One-shot | GameObject | Hard attention-grabbing pulse | `target.Tween().Preset<AttentionHardPreset>().Play();` |
| `AttentionSoft` | 1s | One-shot | GameObject | Soft attention-grabbing pulse | `target.Tween().Preset<AttentionSoftPreset>().Play();` |
| `Blink` | 0.4s | Infinite | Fade-capable GameObject | Rapid alpha on/off loop | `target.Tween().Preset<BlinkPreset>().Play();` |
| `BlinkHard` | 0.2s | Infinite | Fade-capable GameObject | Rapid alpha on/off loop | `target.Tween().Preset<BlinkHardPreset>().Play();` |
| `BlinkSoft` | 0.8s | Infinite | Fade-capable GameObject | Slow alpha on/off loop | `target.Tween().Preset<BlinkSoftPreset>().Play();` |
| `Explode` | 0.6s | One-shot | GameObject | Scale up and fade out simultaneously | `target.Tween().Preset<ExplodePreset>().Play();` |
| `ExplodeHard` | 0.4s | One-shot | GameObject | Aggressive scale up and fade out | `target.Tween().Preset<ExplodeHardPreset>().Play();` |
| `ExplodeSoft` | 0.8s | One-shot | GameObject | Gentle scale up and fade out | `target.Tween().Preset<ExplodeSoftPreset>().Play();` |
| `Flicker` | 1s | One-shot | Fade-capable GameObject | Randomized alpha flicker effect | `target.Tween().Preset<FlickerPreset>().Play();` |
| `FlickerHard` | 0.5s | One-shot | Fade-capable GameObject | Rapid randomized alpha flicker | `target.Tween().Preset<FlickerHardPreset>().Play();` |
| `FlickerSoft` | 1.5s | One-shot | Fade-capable GameObject | Slow randomized alpha flicker | `target.Tween().Preset<FlickerSoftPreset>().Play();` |

## Nod

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Nod` | 0.35s | One-shot | GameObject | X-axis tilt forward then spring back | `target.Tween().Preset<NodPreset>().Play();` |
| `NodHard` | 0.5s | One-shot | GameObject | Deep forward tilt and spring back | `target.Tween().Preset<NodHardPreset>().Play();` |
| `NodSoft` | 0.3s | One-shot | GameObject | Soft forward tilt and spring back | `target.Tween().Preset<NodSoftPreset>().Play();` |

## Nudge

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Nudge` | 0.3s | One-shot | GameObject | Small push right then spring back | `target.Tween().Preset<NudgePreset>().Play();` |
| `NudgeDown` | 0.3s | One-shot | GameObject | Small push down then spring back | `target.Tween().Preset<NudgeDownPreset>().Play();` |
| `NudgeDownHard` | 0.35s | One-shot | GameObject | Strong push down then spring back | `target.Tween().Preset<NudgeDownHardPreset>().Play();` |
| `NudgeDownSoft` | 0.25s | One-shot | GameObject | Gentle push down then spring back | `target.Tween().Preset<NudgeDownSoftPreset>().Play();` |
| `NudgeHard` | 0.35s | One-shot | GameObject | Strong push right then spring back | `target.Tween().Preset<NudgeHardPreset>().Play();` |
| `NudgeLeft` | 0.3s | One-shot | GameObject | Small push left then spring back | `target.Tween().Preset<NudgeLeftPreset>().Play();` |
| `NudgeLeftHard` | 0.35s | One-shot | GameObject | Strong push left then spring back | `target.Tween().Preset<NudgeLeftHardPreset>().Play();` |
| `NudgeLeftSoft` | 0.25s | One-shot | GameObject | Gentle push left then spring back | `target.Tween().Preset<NudgeLeftSoftPreset>().Play();` |
| `NudgeRight` | 0.3s | One-shot | GameObject | Small push right then spring back | `target.Tween().Preset<NudgeRightPreset>().Play();` |
| `NudgeRightHard` | 0.35s | One-shot | GameObject | Strong push right then spring back | `target.Tween().Preset<NudgeRightHardPreset>().Play();` |
| `NudgeRightSoft` | 0.25s | One-shot | GameObject | Gentle push right then spring back | `target.Tween().Preset<NudgeRightSoftPreset>().Play();` |
| `NudgeSoft` | 0.25s | One-shot | GameObject | Gentle push right then spring back | `target.Tween().Preset<NudgeSoftPreset>().Play();` |
| `NudgeUp` | 0.3s | One-shot | GameObject | Small push up then spring back | `target.Tween().Preset<NudgeUpPreset>().Play();` |
| `NudgeUpHard` | 0.35s | One-shot | GameObject | Strong push up then spring back | `target.Tween().Preset<NudgeUpHardPreset>().Play();` |
| `NudgeUpSoft` | 0.25s | One-shot | GameObject | Gentle push up then spring back | `target.Tween().Preset<NudgeUpSoftPreset>().Play();` |

## Orbit

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `OrbitXY` | 2s | Infinite | GameObject | Circular orbit on XY plane | `target.Tween().Preset<OrbitXYPreset>().Play();` |
| `OrbitXYClockwise` | 2s | Infinite | GameObject | Circular orbit on XY plane (clockwise) | `target.Tween().Preset<OrbitXYClockwisePreset>().Play();` |
| `OrbitXYClockwiseHard` | 2s | Infinite | GameObject | Large-radius clockwise orbit on XY plane | `target.Tween().Preset<OrbitXYClockwiseHardPreset>().Play();` |
| `OrbitXYClockwiseSoft` | 2s | Infinite | GameObject | Small-radius clockwise orbit on XY plane | `target.Tween().Preset<OrbitXYClockwiseSoftPreset>().Play();` |
| `OrbitXYCounterClockwise` | 2s | Infinite | GameObject | Circular orbit on XY plane (counter-clockwise) | `target.Tween().Preset<OrbitXYCounterClockwisePreset>().Play();` |
| `OrbitXYCounterClockwiseHard` | 2s | Infinite | GameObject | Large-radius counter-clockwise orbit on XY plane | `target.Tween().Preset<OrbitXYCounterClockwiseHardPreset>().Play();` |
| `OrbitXYCounterClockwiseSoft` | 2s | Infinite | GameObject | Small-radius counter-clockwise orbit on XY plane | `target.Tween().Preset<OrbitXYCounterClockwiseSoftPreset>().Play();` |
| `OrbitXYHard` | 2s | Infinite | GameObject | Large-radius orbit on XY plane | `target.Tween().Preset<OrbitXYHardPreset>().Play();` |
| `OrbitXYSoft` | 2s | Infinite | GameObject | Small-radius orbit on XY plane | `target.Tween().Preset<OrbitXYSoftPreset>().Play();` |
| `OrbitXZ` | 2s | Infinite | GameObject | Circles around a point on XZ plane (counter-clockwise) | `target.Tween().Preset<OrbitXZPreset>().Play();` |
| `OrbitXZClockwise` | 2s | Infinite | GameObject | Circles around a point on XZ plane (clockwise) | `target.Tween().Preset<OrbitXZClockwisePreset>().Play();` |
| `OrbitXZClockwiseHard` | 2s | Infinite | GameObject | Large-radius clockwise orbit on XZ plane | `target.Tween().Preset<OrbitXZClockwiseHardPreset>().Play();` |
| `OrbitXZClockwiseSoft` | 2s | Infinite | GameObject | Small-radius clockwise orbit on XZ plane | `target.Tween().Preset<OrbitXZClockwiseSoftPreset>().Play();` |
| `OrbitXZCounterClockwise` | 2s | Infinite | GameObject | Circles around a point on XZ plane (counter-clockwise) | `target.Tween().Preset<OrbitXZCounterClockwisePreset>().Play();` |
| `OrbitXZCounterClockwiseHard` | 2s | Infinite | GameObject | Large-radius counter-clockwise orbit on XZ plane | `target.Tween().Preset<OrbitXZCounterClockwiseHardPreset>().Play();` |
| `OrbitXZCounterClockwiseSoft` | 2s | Infinite | GameObject | Small-radius counter-clockwise orbit on XZ plane | `target.Tween().Preset<OrbitXZCounterClockwiseSoftPreset>().Play();` |
| `OrbitXZHard` | 2s | Infinite | GameObject | Large-radius orbit on XZ plane | `target.Tween().Preset<OrbitXZHardPreset>().Play();` |
| `OrbitXZSoft` | 2s | Infinite | GameObject | Small-radius orbit on XZ plane | `target.Tween().Preset<OrbitXZSoftPreset>().Play();` |
| `OrbitYZ` | 2s | Infinite | GameObject | Circular orbit on YZ plane | `target.Tween().Preset<OrbitYZPreset>().Play();` |
| `OrbitYZClockwise` | 2s | Infinite | GameObject | Circular orbit on YZ plane (clockwise) | `target.Tween().Preset<OrbitYZClockwisePreset>().Play();` |
| `OrbitYZClockwiseHard` | 2s | Infinite | GameObject | Large-radius clockwise orbit on YZ plane | `target.Tween().Preset<OrbitYZClockwiseHardPreset>().Play();` |
| `OrbitYZClockwiseSoft` | 2s | Infinite | GameObject | Small-radius clockwise orbit on YZ plane | `target.Tween().Preset<OrbitYZClockwiseSoftPreset>().Play();` |
| `OrbitYZCounterClockwise` | 2s | Infinite | GameObject | Circular orbit on YZ plane (counter-clockwise) | `target.Tween().Preset<OrbitYZCounterClockwisePreset>().Play();` |
| `OrbitYZCounterClockwiseHard` | 2s | Infinite | GameObject | Large-radius counter-clockwise orbit on YZ plane | `target.Tween().Preset<OrbitYZCounterClockwiseHardPreset>().Play();` |
| `OrbitYZCounterClockwiseSoft` | 2s | Infinite | GameObject | Small-radius counter-clockwise orbit on YZ plane | `target.Tween().Preset<OrbitYZCounterClockwiseSoftPreset>().Play();` |
| `OrbitYZHard` | 2s | Infinite | GameObject | Large-radius orbit on YZ plane | `target.Tween().Preset<OrbitYZHardPreset>().Play();` |
| `OrbitYZSoft` | 2s | Infinite | GameObject | Small-radius orbit on YZ plane | `target.Tween().Preset<OrbitYZSoftPreset>().Play();` |

## Pendulum

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PendulumX` | 2.8s | Infinite | GameObject | Gentle X-axis pendulum loop | `target.Tween().Preset<PendulumXPreset>().Play();` |
| `PendulumXHard` | 3.5s | Infinite | GameObject | Wide X-axis pendulum loop | `target.Tween().Preset<PendulumXHardPreset>().Play();` |
| `PendulumXSoft` | 2.5s | Infinite | GameObject | Soft X-axis pendulum loop | `target.Tween().Preset<PendulumXSoftPreset>().Play();` |
| `PendulumY` | 2.8s | Infinite | GameObject | Gentle Y-axis pendulum loop | `target.Tween().Preset<PendulumYPreset>().Play();` |
| `PendulumYHard` | 3.5s | Infinite | GameObject | Wide Y-axis pendulum loop | `target.Tween().Preset<PendulumYHardPreset>().Play();` |
| `PendulumYSoft` | 2.5s | Infinite | GameObject | Soft Y-axis pendulum loop | `target.Tween().Preset<PendulumYSoftPreset>().Play();` |
| `PendulumZ` | 2.8s | Infinite | GameObject | Gentle Z-axis pendulum loop | `target.Tween().Preset<PendulumZPreset>().Play();` |
| `PendulumZHard` | 3.5s | Infinite | GameObject | Wide Z-axis pendulum loop | `target.Tween().Preset<PendulumZHardPreset>().Play();` |
| `PendulumZSoft` | 2.5s | Infinite | GameObject | Soft Z-axis pendulum loop | `target.Tween().Preset<PendulumZSoftPreset>().Play();` |

## Pop

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PopIn` | 0.6s | One-shot | GameObject | Scales from 0 to original scale, no overshoot | `target.Tween().Preset<PopInPreset>().Play();` |
| `PopInFade` | 2s | One-shot | Fade-capable GameObject | Scales and fades in together | `target.Tween().Preset<PopInFadePreset>().Play();` |
| `PopInFadeHard` | 1.4s | One-shot | Fade-capable GameObject | Hard scales and fades in together | `target.Tween().Preset<PopInFadeHardPreset>().Play();` |
| `PopInFadeSoft` | 2.5s | One-shot | Fade-capable GameObject | Soft scales and fades in together | `target.Tween().Preset<PopInFadeSoftPreset>().Play();` |
| `PopInHard` | 0.3s | One-shot | GameObject | Fast scale entrance, no overshoot | `target.Tween().Preset<PopInHardPreset>().Play();` |
| `PopInOvershoot` | 1s | One-shot | GameObject | Scales from 0 to original scale with overshoot | `target.Tween().Preset<PopInOvershootPreset>().Play();` |
| `PopInOvershootHard` | 0.8s | One-shot | GameObject | Snappy scale entrance with strong overshoot | `target.Tween().Preset<PopInOvershootHardPreset>().Play();` |
| `PopInOvershootSoft` | 1.2s | One-shot | GameObject | Gentle scale entrance with mild OutBack overshoot | `target.Tween().Preset<PopInOvershootSoftPreset>().Play();` |
| `PopInSoft` | 0.8s | One-shot | GameObject | Gentle scale entrance, no overshoot | `target.Tween().Preset<PopInSoftPreset>().Play();` |
| `PopOut` | 0.4s | One-shot | GameObject | Scales to 0, no anticipation | `target.Tween().Preset<PopOutPreset>().Play();` |
| `PopOutFade` | 1.2s | One-shot | Fade-capable GameObject | Scales down and fades out together, no anticipation | `target.Tween().Preset<PopOutFadePreset>().Play();` |
| `PopOutFadeHard` | 0.8s | One-shot | Fade-capable GameObject | Hard scales down and fades out together | `target.Tween().Preset<PopOutFadeHardPreset>().Play();` |
| `PopOutFadeOvershoot` | 1.2s | One-shot | Fade-capable GameObject | Scales down and fades out with anticipation overshoot | `target.Tween().Preset<PopOutFadeOvershootPreset>().Play();` |
| `PopOutFadeOvershootHard` | 0.8s | One-shot | Fade-capable GameObject | Hard scales down and fades out with strong overshoot | `target.Tween().Preset<PopOutFadeOvershootHardPreset>().Play();` |
| `PopOutFadeOvershootSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft scales down and fades out with mild overshoot | `target.Tween().Preset<PopOutFadeOvershootSoftPreset>().Play();` |
| `PopOutFadeSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft scales down and fades out together | `target.Tween().Preset<PopOutFadeSoftPreset>().Play();` |
| `PopOutHard` | 0.25s | One-shot | GameObject | Hard scale exit, no anticipation | `target.Tween().Preset<PopOutHardPreset>().Play();` |
| `PopOutOvershoot` | 0.4s | One-shot | GameObject | Scales to 0 with anticipation overshoot | `target.Tween().Preset<PopOutOvershootPreset>().Play();` |
| `PopOutOvershootHard` | 0.25s | One-shot | GameObject | Hard scale exit with strong overshoot | `target.Tween().Preset<PopOutOvershootHardPreset>().Play();` |
| `PopOutOvershootSoft` | 0.6s | One-shot | GameObject | Soft scale exit with mild overshoot | `target.Tween().Preset<PopOutOvershootSoftPreset>().Play();` |
| `PopOutSoft` | 0.6s | One-shot | GameObject | Soft scale exit, no anticipation | `target.Tween().Preset<PopOutSoftPreset>().Play();` |

## Pulse

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PulseFade` | 2s | Infinite | Fade-capable GameObject | Smooth alpha pulse loop | `target.Tween().Preset<PulseFadePreset>().Play();` |
| `PulseFadeHard` | 1s | Infinite | Fade-capable GameObject | Fast punchy alpha pulse loop | `target.Tween().Preset<PulseFadeHardPreset>().Play();` |
| `PulseFadeSoft` | 3s | Infinite | Fade-capable GameObject | Slow gentle alpha pulse loop | `target.Tween().Preset<PulseFadeSoftPreset>().Play();` |

## PulseScale

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PulseScale` | 0.28s | One-shot | GameObject | Quick scale bump for interactive feedback | `target.Tween().Preset<PulseScalePreset>().Play();` |
| `PulseScaleFade` | 0.84s | One-shot | Fade-capable GameObject | Pulse scale with alpha dip and return | `target.Tween().Preset<PulseScaleFadePreset>().Play();` |
| `PulseScaleFadeHard` | 1.05s | One-shot | Fade-capable GameObject | Bold pulse scale with deep alpha dip and return | `target.Tween().Preset<PulseScaleFadeHardPreset>().Play();` |
| `PulseScaleFadeSoft` | 0.75s | One-shot | Fade-capable GameObject | Soft pulse scale with alpha dip and return | `target.Tween().Preset<PulseScaleFadeSoftPreset>().Play();` |
| `PulseScaleHard` | 0.35s | One-shot | GameObject | Bold scale bump for emphatic feedback | `target.Tween().Preset<PulseScaleHardPreset>().Play();` |
| `PulseScaleSoft` | 0.25s | One-shot | GameObject | Soft scale bump for light feedback | `target.Tween().Preset<PulseScaleSoftPreset>().Play();` |

## Punch

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Punch` | 0.18s | One-shot | GameObject | Quick scale punch for feedback | `target.Tween().Preset<PunchPreset>().Play();` |
| `PunchHard` | 0.25s | One-shot | GameObject | Heavy scale punch for emphatic feedback | `target.Tween().Preset<PunchHardPreset>().Play();` |
| `PunchSoft` | 0.15s | One-shot | GameObject | Soft scale punch for delicate feedback | `target.Tween().Preset<PunchSoftPreset>().Play();` |

## Recoil

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Recoil` | 0.4s | One-shot | GameObject | Pull back then snap forward on local Z | `target.Tween().Preset<RecoilPreset>().Play();` |
| `RecoilHard` | 0.5s | One-shot | GameObject | Hard pull back then snap forward | `target.Tween().Preset<RecoilHardPreset>().Play();` |
| `RecoilSoft` | 0.3s | One-shot | GameObject | Soft pull back then snap forward | `target.Tween().Preset<RecoilSoftPreset>().Play();` |

## RecoilBack

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `RecoilBack` | 0.4s | One-shot | GameObject | Pull back then snap forward on local Z | `target.Tween().Preset<RecoilBackPreset>().Play();` |
| `RecoilBackHard` | 0.5s | One-shot | GameObject | Hard pull back then snap forward on local Z | `target.Tween().Preset<RecoilBackHardPreset>().Play();` |
| `RecoilBackSoft` | 0.3s | One-shot | GameObject | Soft pull back then snap forward on local Z | `target.Tween().Preset<RecoilBackSoftPreset>().Play();` |

## RecoilForward

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `RecoilForward` | 0.4s | One-shot | GameObject | Pull forward then snap back on local Z | `target.Tween().Preset<RecoilForwardPreset>().Play();` |
| `RecoilForwardHard` | 0.5s | One-shot | GameObject | Hard pull forward then snap back on local Z | `target.Tween().Preset<RecoilForwardHardPreset>().Play();` |
| `RecoilForwardSoft` | 0.3s | One-shot | GameObject | Soft pull forward then snap back on local Z | `target.Tween().Preset<RecoilForwardSoftPreset>().Play();` |

## Shake

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Shake` | 0.45s | One-shot | GameObject | Random position shake | `target.Tween().Preset<ShakePreset>().Play();` |
| `ShakeFade` | 0.8s | One-shot | Fade-capable GameObject | Shake position with fade out | `target.Tween().Preset<ShakeFadePreset>().Play();` |
| `ShakeFadeHard` | 1s | One-shot | Fade-capable GameObject | Hard shake with fade out | `target.Tween().Preset<ShakeFadeHardPreset>().Play();` |
| `ShakeFadeSoft` | 0.6s | One-shot | Fade-capable GameObject | Soft shake with fade out | `target.Tween().Preset<ShakeFadeSoftPreset>().Play();` |
| `ShakeHard` | 0.6s | One-shot | GameObject | Heavy position shake | `target.Tween().Preset<ShakeHardPreset>().Play();` |
| `ShakeSoft` | 0.4s | One-shot | GameObject | Soft position shake | `target.Tween().Preset<ShakeSoftPreset>().Play();` |

## Slide

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `SlideInDown` | 1s | One-shot | GameObject | Slides down from above | `target.Tween().Preset<SlideInDownPreset>().Play();` |
| `SlideInDownHard` | 0.5s | One-shot | GameObject | Quickly slides down from above | `target.Tween().Preset<SlideInDownHardPreset>().Play();` |
| `SlideInDownSoft` | 1.5s | One-shot | GameObject | Slowly slides down from above | `target.Tween().Preset<SlideInDownSoftPreset>().Play();` |
| `SlideInFadeDown` | 2s | One-shot | Fade-capable GameObject | Slides down from above with fade in | `target.Tween().Preset<SlideInFadeDownPreset>().Play();` |
| `SlideInFadeDownHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides down from above with fade in | `target.Tween().Preset<SlideInFadeDownHardPreset>().Play();` |
| `SlideInFadeDownSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides down from above with fade in | `target.Tween().Preset<SlideInFadeDownSoftPreset>().Play();` |
| `SlideInFadeLeft` | 2.5s | One-shot | Fade-capable GameObject | Slides in from the left with fade in | `target.Tween().Preset<SlideInFadeLeftPreset>().Play();` |
| `SlideInFadeLeftHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides in from the left with fade in | `target.Tween().Preset<SlideInFadeLeftHardPreset>().Play();` |
| `SlideInFadeLeftSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides in from the left with fade in | `target.Tween().Preset<SlideInFadeLeftSoftPreset>().Play();` |
| `SlideInFadeRight` | 2.5s | One-shot | Fade-capable GameObject | Slides in from the right with fade in | `target.Tween().Preset<SlideInFadeRightPreset>().Play();` |
| `SlideInFadeRightHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides in from the right with fade in | `target.Tween().Preset<SlideInFadeRightHardPreset>().Play();` |
| `SlideInFadeRightSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides in from the right with fade in | `target.Tween().Preset<SlideInFadeRightSoftPreset>().Play();` |
| `SlideInFadeUp` | 2s | One-shot | Fade-capable GameObject | Slides up from below with fade in | `target.Tween().Preset<SlideInFadeUpPreset>().Play();` |
| `SlideInFadeUpHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides up from below with fade in | `target.Tween().Preset<SlideInFadeUpHardPreset>().Play();` |
| `SlideInFadeUpSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides up from below with fade in | `target.Tween().Preset<SlideInFadeUpSoftPreset>().Play();` |
| `SlideInLeft` | 1s | One-shot | GameObject | Slides in from the left side | `target.Tween().Preset<SlideInLeftPreset>().Play();` |
| `SlideInLeftHard` | 0.5s | One-shot | GameObject | Quickly slides in from the left | `target.Tween().Preset<SlideInLeftHardPreset>().Play();` |
| `SlideInLeftSoft` | 1.5s | One-shot | GameObject | Slowly slides in from the left | `target.Tween().Preset<SlideInLeftSoftPreset>().Play();` |
| `SlideInRight` | 1s | One-shot | GameObject | Slides in from the right side | `target.Tween().Preset<SlideInRightPreset>().Play();` |
| `SlideInRightHard` | 0.5s | One-shot | GameObject | Quickly slides in from the right | `target.Tween().Preset<SlideInRightHardPreset>().Play();` |
| `SlideInRightSoft` | 1.5s | One-shot | GameObject | Slowly slides in from the right | `target.Tween().Preset<SlideInRightSoftPreset>().Play();` |
| `SlideInUp` | 1s | One-shot | GameObject | Slides up from below | `target.Tween().Preset<SlideInUpPreset>().Play();` |
| `SlideInUpHard` | 0.5s | One-shot | GameObject | Quickly slides up from below | `target.Tween().Preset<SlideInUpHardPreset>().Play();` |
| `SlideInUpSoft` | 1.5s | One-shot | GameObject | Slowly slides up from below | `target.Tween().Preset<SlideInUpSoftPreset>().Play();` |
| `SlideOutDown` | 1s | One-shot | GameObject | Slides down off-screen | `target.Tween().Preset<SlideOutDownPreset>().Play();` |
| `SlideOutDownHard` | 0.5s | One-shot | GameObject | Quickly slides down off-screen | `target.Tween().Preset<SlideOutDownHardPreset>().Play();` |
| `SlideOutDownSoft` | 1.5s | One-shot | GameObject | Slowly slides down off-screen | `target.Tween().Preset<SlideOutDownSoftPreset>().Play();` |
| `SlideOutFadeDown` | 2s | One-shot | Fade-capable GameObject | Slides down while fading out | `target.Tween().Preset<SlideOutFadeDownPreset>().Play();` |
| `SlideOutFadeDownHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides down while fading out | `target.Tween().Preset<SlideOutFadeDownHardPreset>().Play();` |
| `SlideOutFadeDownSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides down while fading out | `target.Tween().Preset<SlideOutFadeDownSoftPreset>().Play();` |
| `SlideOutFadeLeft` | 2.5s | One-shot | Fade-capable GameObject | Slides left while fading out | `target.Tween().Preset<SlideOutFadeLeftPreset>().Play();` |
| `SlideOutFadeLeftHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides left while fading out | `target.Tween().Preset<SlideOutFadeLeftHardPreset>().Play();` |
| `SlideOutFadeLeftSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides left while fading out | `target.Tween().Preset<SlideOutFadeLeftSoftPreset>().Play();` |
| `SlideOutFadeRight` | 2.5s | One-shot | Fade-capable GameObject | Slides right while fading out | `target.Tween().Preset<SlideOutFadeRightPreset>().Play();` |
| `SlideOutFadeRightHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides right while fading out | `target.Tween().Preset<SlideOutFadeRightHardPreset>().Play();` |
| `SlideOutFadeRightSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides right while fading out | `target.Tween().Preset<SlideOutFadeRightSoftPreset>().Play();` |
| `SlideOutFadeUp` | 2s | One-shot | Fade-capable GameObject | Slides up while fading out | `target.Tween().Preset<SlideOutFadeUpPreset>().Play();` |
| `SlideOutFadeUpHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides up while fading out | `target.Tween().Preset<SlideOutFadeUpHardPreset>().Play();` |
| `SlideOutFadeUpSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides up while fading out | `target.Tween().Preset<SlideOutFadeUpSoftPreset>().Play();` |
| `SlideOutLeft` | 1s | One-shot | GameObject | Slides left off-screen | `target.Tween().Preset<SlideOutLeftPreset>().Play();` |
| `SlideOutLeftHard` | 0.5s | One-shot | GameObject | Quickly slides left off-screen | `target.Tween().Preset<SlideOutLeftHardPreset>().Play();` |
| `SlideOutLeftSoft` | 1.5s | One-shot | GameObject | Slowly slides left off-screen | `target.Tween().Preset<SlideOutLeftSoftPreset>().Play();` |
| `SlideOutRight` | 1s | One-shot | GameObject | Slides right off-screen | `target.Tween().Preset<SlideOutRightPreset>().Play();` |
| `SlideOutRightHard` | 0.5s | One-shot | GameObject | Quickly slides right off-screen | `target.Tween().Preset<SlideOutRightHardPreset>().Play();` |
| `SlideOutRightSoft` | 1.5s | One-shot | GameObject | Slowly slides right off-screen | `target.Tween().Preset<SlideOutRightSoftPreset>().Play();` |
| `SlideOutUp` | 1s | One-shot | GameObject | Slides up off-screen | `target.Tween().Preset<SlideOutUpPreset>().Play();` |
| `SlideOutUpHard` | 0.5s | One-shot | GameObject | Quickly slides up off-screen | `target.Tween().Preset<SlideOutUpHardPreset>().Play();` |
| `SlideOutUpSoft` | 1.5s | One-shot | GameObject | Slowly slides up off-screen | `target.Tween().Preset<SlideOutUpSoftPreset>().Play();` |

## Spin

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `SpinDiagonalXY` | 1s | One-shot | GameObject | Spins 360 degrees across X and Y axes | `target.Tween().Preset<SpinDiagonalXYPreset>().Play();` |
| `SpinDiagonalXYHard` | 0.5s | One-shot | GameObject | Fast diagonal spin across X and Y | `target.Tween().Preset<SpinDiagonalXYHardPreset>().Play();` |
| `SpinDiagonalXYSoft` | 1.5s | One-shot | GameObject | Slow diagonal spin across X and Y | `target.Tween().Preset<SpinDiagonalXYSoftPreset>().Play();` |
| `SpinDiagonalXZ` | 1s | One-shot | GameObject | Spins 360 degrees across X and Z axes | `target.Tween().Preset<SpinDiagonalXZPreset>().Play();` |
| `SpinDiagonalXZHard` | 0.5s | One-shot | GameObject | Fast diagonal spin across X and Z | `target.Tween().Preset<SpinDiagonalXZHardPreset>().Play();` |
| `SpinDiagonalXZSoft` | 1.5s | One-shot | GameObject | Slow diagonal spin across X and Z | `target.Tween().Preset<SpinDiagonalXZSoftPreset>().Play();` |
| `SpinDiagonalYZ` | 1s | One-shot | GameObject | Spins 360 degrees across Y and Z axes | `target.Tween().Preset<SpinDiagonalYZPreset>().Play();` |
| `SpinDiagonalYZHard` | 0.5s | One-shot | GameObject | Fast diagonal spin across Y and Z | `target.Tween().Preset<SpinDiagonalYZHardPreset>().Play();` |
| `SpinDiagonalYZSoft` | 1.5s | One-shot | GameObject | Slow diagonal spin across Y and Z | `target.Tween().Preset<SpinDiagonalYZSoftPreset>().Play();` |
| `SpinX` | 1s | One-shot | GameObject | Spins 360 degrees on X axis | `target.Tween().Preset<SpinXPreset>().Play();` |
| `SpinXHard` | 0.5s | One-shot | GameObject | Fast spin on X axis | `target.Tween().Preset<SpinXHardPreset>().Play();` |
| `SpinXSoft` | 1.5s | One-shot | GameObject | Slow spin on X axis | `target.Tween().Preset<SpinXSoftPreset>().Play();` |
| `SpinY` | 1s | One-shot | GameObject | Spins 360 degrees on Y axis | `target.Tween().Preset<SpinPreset>().Play();` |
| `SpinYHard` | 0.5s | One-shot | GameObject | Fast spin on Y axis | `target.Tween().Preset<SpinYHardPreset>().Play();` |
| `SpinYSoft` | 1.5s | One-shot | GameObject | Slow spin on Y axis | `target.Tween().Preset<SpinYSoftPreset>().Play();` |
| `SpinZ` | 1s | One-shot | GameObject | Spins 360 degrees on Z axis | `target.Tween().Preset<SpinZPreset>().Play();` |
| `SpinZHard` | 0.5s | One-shot | GameObject | Fast spin on Z axis | `target.Tween().Preset<SpinZHardPreset>().Play();` |
| `SpinZSoft` | 1.5s | One-shot | GameObject | Slow spin on Z axis | `target.Tween().Preset<SpinZSoftPreset>().Play();` |

## SpinScale

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `SpinScaleIn` | 2.1s | One-shot | GameObject | Spin and grow in from zero | `target.Tween().Preset<SpinScaleInPreset>().Play();` |
| `SpinScaleInHard` | 1.5s | One-shot | GameObject | Hard spin and grow in from zero | `target.Tween().Preset<SpinScaleInHardPreset>().Play();` |
| `SpinScaleInOvershoot` | 2.1s | One-shot | GameObject | Spin and grow in with overshoot settle | `target.Tween().Preset<SpinScaleInOvershootPreset>().Play();` |
| `SpinScaleInOvershootHard` | 1.5s | One-shot | GameObject | Hard spin and grow in with strong overshoot | `target.Tween().Preset<SpinScaleInOvershootHardPreset>().Play();` |
| `SpinScaleInOvershootSoft` | 2.7s | One-shot | GameObject | Soft spin and grow in with mild overshoot | `target.Tween().Preset<SpinScaleInOvershootSoftPreset>().Play();` |
| `SpinScaleInSoft` | 2.7s | One-shot | GameObject | Soft spin and grow in from zero | `target.Tween().Preset<SpinScaleInSoftPreset>().Play();` |
| `SpinScaleOut` | 0.7s | One-shot | GameObject | Spin and shrink to zero, no anticipation | `target.Tween().Preset<SpinScaleOutPreset>().Play();` |
| `SpinScaleOutHard` | 0.5s | One-shot | GameObject | Hard spin and shrink to zero | `target.Tween().Preset<SpinScaleOutHardPreset>().Play();` |
| `SpinScaleOutOvershoot` | 0.7s | One-shot | GameObject | Spin and shrink to zero with anticipation overshoot | `target.Tween().Preset<SpinScaleOutOvershootPreset>().Play();` |
| `SpinScaleOutOvershootHard` | 0.5s | One-shot | GameObject | Hard spin and shrink with strong anticipation | `target.Tween().Preset<SpinScaleOutOvershootHardPreset>().Play();` |
| `SpinScaleOutOvershootSoft` | 0.9s | One-shot | GameObject | Soft spin and shrink with mild anticipation | `target.Tween().Preset<SpinScaleOutOvershootSoftPreset>().Play();` |
| `SpinScaleOutSoft` | 0.9s | One-shot | GameObject | Soft spin and shrink to zero | `target.Tween().Preset<SpinScaleOutSoftPreset>().Play();` |

## Spiral

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Spiral` | 1.5s | One-shot | GameObject | Spirals upward combining rotation and height | `target.Tween().Preset<SpiralPreset>().Play();` |
| `SpiralHard` | 2s | One-shot | GameObject | Dramatic upward spiral with wider radius | `target.Tween().Preset<SpiralHardPreset>().Play();` |
| `SpiralSoft` | 1.2s | One-shot | GameObject | Gentle upward spiral with smaller radius | `target.Tween().Preset<SpiralSoftPreset>().Play();` |
| `SwirlIn` | 1s | One-shot | GameObject | Spin and scale in from zero | `target.Tween().Preset<SwirlInPreset>().Play();` |
| `SwirlInHard` | 1.3s | One-shot | GameObject | Dramatic spin and scale in from zero | `target.Tween().Preset<SwirlInHardPreset>().Play();` |
| `SwirlInSoft` | 0.8s | One-shot | GameObject | Gentle spin and scale in from zero | `target.Tween().Preset<SwirlInSoftPreset>().Play();` |

## Squash

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Squash` | 0.6s | One-shot | GameObject | Squash and stretch pulse | `target.Tween().Preset<BouncePreset>().Play();` |
| `SquashHard` | 0.7s | One-shot | GameObject | Hard squash and stretch pulse | `target.Tween().Preset<SquashHardPreset>().Play();` |
| `SquashSoft` | 0.5s | One-shot | GameObject | Soft squash and stretch pulse | `target.Tween().Preset<SquashSoftPreset>().Play();` |

## Sway

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Sway` | 3.5s | Infinite | GameObject | Gentle horizontal sway loop | `target.Tween().Preset<SwayPreset>().Play();` |
| `SwayHard` | 5s | Infinite | GameObject | Wide horizontal sway loop | `target.Tween().Preset<SwayHardPreset>().Play();` |
| `SwaySoft` | 3s | Infinite | GameObject | Soft horizontal sway loop | `target.Tween().Preset<SwaySoftPreset>().Play();` |

## Tilt

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Tilt` | 0.4s | One-shot | GameObject | Lean on Z then spring back | `target.Tween().Preset<TiltPreset>().Play();` |
| `TiltHard` | 0.3s | One-shot | GameObject | Hard lean on Z then spring back | `target.Tween().Preset<TiltHardPreset>().Play();` |
| `TiltSoft` | 0.5s | One-shot | GameObject | Soft lean on Z then spring back | `target.Tween().Preset<TiltSoftPreset>().Play();` |

## Wobble

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `WobbleDiagonalXY` | 0.5s | One-shot | GameObject | Wobbles rotation diagonally across X and Y | `target.Tween().Preset<WobbleDiagonalXYPreset>().Play();` |
| `WobbleDiagonalXYHard` | 0.6s | One-shot | GameObject | Heavy diagonal XY wobble | `target.Tween().Preset<WobbleDiagonalXYHardPreset>().Play();` |
| `WobbleDiagonalXYSoft` | 0.4s | One-shot | GameObject | Soft diagonal XY wobble | `target.Tween().Preset<WobbleDiagonalXYSoftPreset>().Play();` |
| `WobbleDiagonalXZ` | 0.5s | One-shot | GameObject | Wobbles rotation diagonally across X and Z | `target.Tween().Preset<WobbleDiagonalXZPreset>().Play();` |
| `WobbleDiagonalXZHard` | 0.6s | One-shot | GameObject | Heavy diagonal XZ wobble | `target.Tween().Preset<WobbleDiagonalXZHardPreset>().Play();` |
| `WobbleDiagonalXZSoft` | 0.4s | One-shot | GameObject | Soft diagonal XZ wobble | `target.Tween().Preset<WobbleDiagonalXZSoftPreset>().Play();` |
| `WobbleDiagonalYZ` | 0.5s | One-shot | GameObject | Wobbles rotation diagonally across Y and Z | `target.Tween().Preset<WobbleDiagonalYZPreset>().Play();` |
| `WobbleDiagonalYZHard` | 0.6s | One-shot | GameObject | Heavy diagonal YZ wobble | `target.Tween().Preset<WobbleDiagonalYZHardPreset>().Play();` |
| `WobbleDiagonalYZSoft` | 0.4s | One-shot | GameObject | Soft diagonal YZ wobble | `target.Tween().Preset<WobbleDiagonalYZSoftPreset>().Play();` |
| `WobbleFadeX` | 2s | One-shot | Fade-capable GameObject | Wobble on X axis with fade out | `target.Tween().Preset<WobbleFadeXPreset>().Play();` |
| `WobbleFadeXHard` | 2.4s | One-shot | Fade-capable GameObject | Heavy wobble on X axis with fade out | `target.Tween().Preset<WobbleFadeXHardPreset>().Play();` |
| `WobbleFadeXSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft wobble on X axis with fade out | `target.Tween().Preset<WobbleFadeXSoftPreset>().Play();` |
| `WobbleFadeY` | 2s | One-shot | Fade-capable GameObject | Wobble on Y axis with fade out | `target.Tween().Preset<WobbleFadeYPreset>().Play();` |
| `WobbleFadeYHard` | 2.4s | One-shot | Fade-capable GameObject | Heavy wobble on Y axis with fade out | `target.Tween().Preset<WobbleFadeYHardPreset>().Play();` |
| `WobbleFadeYSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft wobble on Y axis with fade out | `target.Tween().Preset<WobbleFadeYSoftPreset>().Play();` |
| `WobbleFadeZ` | 1.8s | One-shot | Fade-capable GameObject | Wobble on Z axis with fade out | `target.Tween().Preset<WobbleFadeZPreset>().Play();` |
| `WobbleFadeZHard` | 2.4s | One-shot | Fade-capable GameObject | Heavy wobble on Z axis with fade out | `target.Tween().Preset<WobbleFadeZHardPreset>().Play();` |
| `WobbleFadeZSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft wobble on Z axis with fade out | `target.Tween().Preset<WobbleFadeZSoftPreset>().Play();` |
| `WobbleX` | 0.5s | One-shot | GameObject | Wobbles rotation back and forth on X | `target.Tween().Preset<WobbleXPreset>().Play();` |
| `WobbleXHard` | 0.6s | One-shot | GameObject | Heavy X-axis wobble | `target.Tween().Preset<WobbleXHardPreset>().Play();` |
| `WobbleXSoft` | 0.4s | One-shot | GameObject | Soft X-axis wobble | `target.Tween().Preset<WobbleXSoftPreset>().Play();` |
| `WobbleY` | 0.5s | One-shot | GameObject | Wobbles rotation back and forth on Y | `target.Tween().Preset<WobblePreset>().Play();` |
| `WobbleYHard` | 0.6s | One-shot | GameObject | Heavy Y-axis wobble | `target.Tween().Preset<WobbleYHardPreset>().Play();` |
| `WobbleYSoft` | 0.4s | One-shot | GameObject | Soft Y-axis wobble | `target.Tween().Preset<WobbleYSoftPreset>().Play();` |
| `WobbleZ` | 0.45s | One-shot | GameObject | Wobbles rotation back and forth on Z | `target.Tween().Preset<WobbleZPreset>().Play();` |
| `WobbleZHard` | 0.6s | One-shot | GameObject | Heavy Z-axis wobble | `target.Tween().Preset<WobbleZHardPreset>().Play();` |
| `WobbleZSoft` | 0.4s | One-shot | GameObject | Soft Z-axis wobble | `target.Tween().Preset<WobbleZSoftPreset>().Play();` |

## ZigZag

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `ZigZag` | 1s | One-shot | GameObject | Alternating diagonal zig-zag movement | `target.Tween().Preset<ZigZagPreset>().Play();` |
| `ZigZagHard` | 0.8s | One-shot | GameObject | Hard alternating diagonal movement | `target.Tween().Preset<ZigZagHardPreset>().Play();` |
| `ZigZagSoft` | 1.2s | One-shot | GameObject | Soft alternating diagonal movement | `target.Tween().Preset<ZigZagSoftPreset>().Play();` |
