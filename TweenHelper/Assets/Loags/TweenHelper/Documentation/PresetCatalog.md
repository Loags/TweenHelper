# TweenHelper preset catalog

> Generated deterministically from the registered TweenHelper preset types.

Built-in presets: **300**

Regenerate this file with **Tools > TweenHelper > Export Preset Catalog**. Generation fails on duplicate, empty, or unconstructible presets.

## Bounce

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Bounce` | 1s | One-shot | GameObject | Positional Y bounce with decreasing hops | `target.Tween().Preset("Bounce").Play();` |
| `BounceCartoon` | 1.4s | One-shot | GameObject | Cartoon bounce with squash-stretch on landing | `target.Tween().Preset("BounceCartoon").Play();` |
| `BounceCartoonHard` | 1.1s | One-shot | GameObject | Hard cartoon bounce with exaggerated squash-stretch | `target.Tween().Preset("BounceCartoonHard").Play();` |
| `BounceCartoonSoft` | 1.8s | One-shot | GameObject | Soft cartoon bounce with gentle squash-stretch | `target.Tween().Preset("BounceCartoonSoft").Play();` |
| `BounceHard` | 1.3s | One-shot | GameObject | Heavy bounce with tall hops | `target.Tween().Preset("BounceHard").Play();` |
| `BounceLand` | 2s | One-shot | GameObject | Drop with bounce and squash-stretch on landing | `target.Tween().Preset("BounceLand").Play();` |
| `BounceLandHard` | 1.6s | One-shot | GameObject | Heavy drop with sharp bounce and exaggerated squash-stretch | `target.Tween().Preset("BounceLandHard").Play();` |
| `BounceLandSoft` | 2.4s | One-shot | GameObject | Gentle drop with soft bounce and mild squash-stretch | `target.Tween().Preset("BounceLandSoft").Play();` |
| `BounceSoft` | 0.9s | One-shot | GameObject | Soft bounce | `target.Tween().Preset("BounceSoft").Play();` |

## Breathe

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Breathe` | 4s | Infinite | GameObject | Gentle scale pulse loop | `target.Tween().Preset("Breathe").Play();` |
| `BreatheHard` | 3s | Infinite | GameObject | Hard intense scale pulse loop | `target.Tween().Preset("BreatheHard").Play();` |
| `BreatheSoft` | 5s | Infinite | GameObject | Soft gentle scale pulse loop | `target.Tween().Preset("BreatheSoft").Play();` |

## Drop

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `DropIn` | 1.2s | One-shot | GameObject | Falls from above with bounce on landing | `target.Tween().Preset("DropIn").Play();` |
| `DropInHard` | 0.9s | One-shot | GameObject | Heavy drop with sharp bounce decay | `target.Tween().Preset("DropInHard").Play();` |
| `DropInSoft` | 1.5s | One-shot | GameObject | Gentle drop with soft bounce on landing | `target.Tween().Preset("DropInSoft").Play();` |

## Fade

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `FadeIn` | 3s | One-shot | Fade-capable GameObject | Fades in from transparent (requires fadeable component) | `target.Tween().Preset("FadeIn").Play();` |
| `FadeInHard` | 1s | One-shot | Fade-capable GameObject | Quick fade in from transparent (requires fadeable component) | `target.Tween().Preset("FadeInHard").Play();` |
| `FadeInOut` | 2s | One-shot | Fade-capable GameObject | Fade in then fade out | `target.Tween().Preset("FadeInOut").Play();` |
| `FadeInOutHard` | 1s | One-shot | Fade-capable GameObject | Quick fade in then quick fade out | `target.Tween().Preset("FadeInOutHard").Play();` |
| `FadeInOutSoft` | 3s | One-shot | Fade-capable GameObject | Slow fade in then slow fade out | `target.Tween().Preset("FadeInOutSoft").Play();` |
| `FadeInSoft` | 5s | One-shot | Fade-capable GameObject | Slow fade in from transparent (requires fadeable component) | `target.Tween().Preset("FadeInSoft").Play();` |
| `FadeOut` | 3s | One-shot | Fade-capable GameObject | Fades out to transparent (requires fadeable component) | `target.Tween().Preset("FadeOut").Play();` |
| `FadeOutHard` | 1s | One-shot | Fade-capable GameObject | Quick fade out to transparent (requires fadeable component) | `target.Tween().Preset("FadeOutHard").Play();` |
| `FadeOutSoft` | 5s | One-shot | Fade-capable GameObject | Slow fade out to transparent (requires fadeable component) | `target.Tween().Preset("FadeOutSoft").Play();` |

## Flip

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `FlipFadeX` | 1s | One-shot | Fade-capable GameObject | 180° flip on X axis with fade out | `target.Tween().Preset("FlipFadeX").Play();` |
| `FlipFadeXHard` | 0.5s | One-shot | Fade-capable GameObject | Quick 180° flip on X axis with fade out | `target.Tween().Preset("FlipFadeXHard").Play();` |
| `FlipFadeXSoft` | 1.6s | One-shot | Fade-capable GameObject | Slow 180° flip on X axis with fade out | `target.Tween().Preset("FlipFadeXSoft").Play();` |
| `FlipFadeY` | 1s | One-shot | Fade-capable GameObject | 180° flip on Y axis with fade out | `target.Tween().Preset("FlipFadeY").Play();` |
| `FlipFadeYHard` | 0.5s | One-shot | Fade-capable GameObject | Quick 180° flip on Y axis with fade out | `target.Tween().Preset("FlipFadeYHard").Play();` |
| `FlipFadeYSoft` | 1.6s | One-shot | Fade-capable GameObject | Slow 180° flip on Y axis with fade out | `target.Tween().Preset("FlipFadeYSoft").Play();` |
| `FlipFadeZ` | 1s | One-shot | Fade-capable GameObject | 180° flip on Z axis with fade out | `target.Tween().Preset("FlipFadeZ").Play();` |
| `FlipFadeZHard` | 0.5s | One-shot | Fade-capable GameObject | Quick 180° flip on Z axis with fade out | `target.Tween().Preset("FlipFadeZHard").Play();` |
| `FlipFadeZSoft` | 1.6s | One-shot | Fade-capable GameObject | Slow 180° flip on Z axis with fade out | `target.Tween().Preset("FlipFadeZSoft").Play();` |
| `FlipX` | 0.5s | One-shot | GameObject | 180° flip on X axis | `target.Tween().Preset("FlipX").Play();` |
| `FlipXHard` | 0.25s | One-shot | GameObject | Quick 180° flip on X axis | `target.Tween().Preset("FlipXHard").Play();` |
| `FlipXSoft` | 0.8s | One-shot | GameObject | Slow 180° flip on X axis | `target.Tween().Preset("FlipXSoft").Play();` |
| `FlipY` | 0.5s | One-shot | GameObject | 180° flip on Y axis | `target.Tween().Preset("FlipY").Play();` |
| `FlipYHard` | 0.25s | One-shot | GameObject | Quick 180° flip on Y axis | `target.Tween().Preset("FlipYHard").Play();` |
| `FlipYSoft` | 0.8s | One-shot | GameObject | Slow 180° flip on Y axis | `target.Tween().Preset("FlipYSoft").Play();` |
| `FlipZ` | 0.5s | One-shot | GameObject | 180° flip on Z axis | `target.Tween().Preset("FlipZ").Play();` |
| `FlipZHard` | 0.25s | One-shot | GameObject | Quick 180° flip on Z axis | `target.Tween().Preset("FlipZHard").Play();` |
| `FlipZSoft` | 0.8s | One-shot | GameObject | Slow 180° flip on Z axis | `target.Tween().Preset("FlipZSoft").Play();` |

## Float

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Float` | 6s | Infinite | GameObject | Gentle up/down hovering loop | `target.Tween().Preset("Float").Play();` |
| `FloatHard` | 5s | Infinite | GameObject | Hard pronounced hovering loop | `target.Tween().Preset("FloatHard").Play();` |
| `FloatSoft` | 7s | Infinite | GameObject | Soft gentle hovering loop | `target.Tween().Preset("FloatSoft").Play();` |

## Heartbeat

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Heartbeat` | 0.8s | Infinite | GameObject | Double-pulse heartbeat loop | `target.Tween().Preset("Heartbeat").Play();` |
| `HeartbeatHard` | 0.6s | Infinite | GameObject | Hard intense heartbeat loop | `target.Tween().Preset("HeartbeatHard").Play();` |
| `HeartbeatSoft` | 1s | Infinite | GameObject | Soft double-pulse heartbeat loop | `target.Tween().Preset("HeartbeatSoft").Play();` |

## Jitter

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Jitter` | 0.25s | One-shot | GameObject | Tight rapid vibration | `target.Tween().Preset("Jitter").Play();` |
| `JitterHard` | 0.4s | One-shot | GameObject | Intense rapid vibration | `target.Tween().Preset("JitterHard").Play();` |
| `JitterSoft` | 0.2s | One-shot | GameObject | Soft rapid vibration | `target.Tween().Preset("JitterSoft").Play();` |

## Launch

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `LaunchDown` | 0.4s | One-shot | GameObject | Quick downward motion with ease-out | `target.Tween().Preset("LaunchDown").Play();` |
| `LaunchDownHard` | 0.5s | One-shot | GameObject | Forceful downward motion with ease-out | `target.Tween().Preset("LaunchDownHard").Play();` |
| `LaunchDownSoft` | 0.3s | One-shot | GameObject | Gentle downward motion with ease-out | `target.Tween().Preset("LaunchDownSoft").Play();` |
| `LaunchLeft` | 0.4s | One-shot | GameObject | Quick leftward motion with ease-out | `target.Tween().Preset("LaunchLeft").Play();` |
| `LaunchLeftHard` | 0.5s | One-shot | GameObject | Forceful leftward motion with ease-out | `target.Tween().Preset("LaunchLeftHard").Play();` |
| `LaunchLeftSoft` | 0.3s | One-shot | GameObject | Gentle leftward motion with ease-out | `target.Tween().Preset("LaunchLeftSoft").Play();` |
| `LaunchRight` | 0.4s | One-shot | GameObject | Quick rightward motion with ease-out | `target.Tween().Preset("LaunchRight").Play();` |
| `LaunchRightHard` | 0.5s | One-shot | GameObject | Forceful rightward motion with ease-out | `target.Tween().Preset("LaunchRightHard").Play();` |
| `LaunchRightSoft` | 0.3s | One-shot | GameObject | Gentle rightward motion with ease-out | `target.Tween().Preset("LaunchRightSoft").Play();` |
| `LaunchUp` | 0.4s | One-shot | GameObject | Quick upward motion with ease-out | `target.Tween().Preset("LaunchUp").Play();` |
| `LaunchUpHard` | 0.5s | One-shot | GameObject | Forceful upward motion with ease-out | `target.Tween().Preset("LaunchUpHard").Play();` |
| `LaunchUpSoft` | 0.3s | One-shot | GameObject | Gentle upward motion with ease-out | `target.Tween().Preset("LaunchUpSoft").Play();` |

## Misc

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Attention` | 0.8s | One-shot | GameObject | Attention-grabbing pulse | `target.Tween().Preset("Attention").Play();` |
| `AttentionHard` | 0.6s | One-shot | GameObject | Hard attention-grabbing pulse | `target.Tween().Preset("AttentionHard").Play();` |
| `AttentionSoft` | 1s | One-shot | GameObject | Soft attention-grabbing pulse | `target.Tween().Preset("AttentionSoft").Play();` |
| `Blink` | 0.4s | Infinite | Fade-capable GameObject | Rapid alpha on/off loop | `target.Tween().Preset("Blink").Play();` |
| `BlinkHard` | 0.2s | Infinite | Fade-capable GameObject | Rapid alpha on/off loop | `target.Tween().Preset("BlinkHard").Play();` |
| `BlinkSoft` | 0.8s | Infinite | Fade-capable GameObject | Slow alpha on/off loop | `target.Tween().Preset("BlinkSoft").Play();` |
| `Explode` | 0.6s | One-shot | GameObject | Scale up and fade out simultaneously | `target.Tween().Preset("Explode").Play();` |
| `ExplodeHard` | 0.4s | One-shot | GameObject | Aggressive scale up and fade out | `target.Tween().Preset("ExplodeHard").Play();` |
| `ExplodeSoft` | 0.8s | One-shot | GameObject | Gentle scale up and fade out | `target.Tween().Preset("ExplodeSoft").Play();` |
| `Flicker` | 1s | One-shot | Fade-capable GameObject | Randomized alpha flicker effect | `target.Tween().Preset("Flicker").Play();` |
| `FlickerHard` | 0.5s | One-shot | Fade-capable GameObject | Rapid randomized alpha flicker | `target.Tween().Preset("FlickerHard").Play();` |
| `FlickerSoft` | 1.5s | One-shot | Fade-capable GameObject | Slow randomized alpha flicker | `target.Tween().Preset("FlickerSoft").Play();` |

## Nod

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Nod` | 0.35s | One-shot | GameObject | X-axis tilt forward then spring back | `target.Tween().Preset("Nod").Play();` |
| `NodHard` | 0.5s | One-shot | GameObject | Deep forward tilt and spring back | `target.Tween().Preset("NodHard").Play();` |
| `NodSoft` | 0.3s | One-shot | GameObject | Soft forward tilt and spring back | `target.Tween().Preset("NodSoft").Play();` |

## Nudge

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Nudge` | 0.3s | One-shot | GameObject | Small push right then spring back | `target.Tween().Preset("Nudge").Play();` |
| `NudgeDown` | 0.3s | One-shot | GameObject | Small push down then spring back | `target.Tween().Preset("NudgeDown").Play();` |
| `NudgeDownHard` | 0.35s | One-shot | GameObject | Strong push down then spring back | `target.Tween().Preset("NudgeDownHard").Play();` |
| `NudgeDownSoft` | 0.25s | One-shot | GameObject | Gentle push down then spring back | `target.Tween().Preset("NudgeDownSoft").Play();` |
| `NudgeHard` | 0.35s | One-shot | GameObject | Strong push right then spring back | `target.Tween().Preset("NudgeHard").Play();` |
| `NudgeLeft` | 0.3s | One-shot | GameObject | Small push left then spring back | `target.Tween().Preset("NudgeLeft").Play();` |
| `NudgeLeftHard` | 0.35s | One-shot | GameObject | Strong push left then spring back | `target.Tween().Preset("NudgeLeftHard").Play();` |
| `NudgeLeftSoft` | 0.25s | One-shot | GameObject | Gentle push left then spring back | `target.Tween().Preset("NudgeLeftSoft").Play();` |
| `NudgeRight` | 0.3s | One-shot | GameObject | Small push right then spring back | `target.Tween().Preset("NudgeRight").Play();` |
| `NudgeRightHard` | 0.35s | One-shot | GameObject | Strong push right then spring back | `target.Tween().Preset("NudgeRightHard").Play();` |
| `NudgeRightSoft` | 0.25s | One-shot | GameObject | Gentle push right then spring back | `target.Tween().Preset("NudgeRightSoft").Play();` |
| `NudgeSoft` | 0.25s | One-shot | GameObject | Gentle push right then spring back | `target.Tween().Preset("NudgeSoft").Play();` |
| `NudgeUp` | 0.3s | One-shot | GameObject | Small push up then spring back | `target.Tween().Preset("NudgeUp").Play();` |
| `NudgeUpHard` | 0.35s | One-shot | GameObject | Strong push up then spring back | `target.Tween().Preset("NudgeUpHard").Play();` |
| `NudgeUpSoft` | 0.25s | One-shot | GameObject | Gentle push up then spring back | `target.Tween().Preset("NudgeUpSoft").Play();` |

## Orbit

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `OrbitXY` | 2s | Infinite | GameObject | Circular orbit on XY plane | `target.Tween().Preset("OrbitXY").Play();` |
| `OrbitXYClockwise` | 2s | Infinite | GameObject | Circular orbit on XY plane (clockwise) | `target.Tween().Preset("OrbitXYClockwise").Play();` |
| `OrbitXYClockwiseHard` | 2s | Infinite | GameObject | Large-radius clockwise orbit on XY plane | `target.Tween().Preset("OrbitXYClockwiseHard").Play();` |
| `OrbitXYClockwiseSoft` | 2s | Infinite | GameObject | Small-radius clockwise orbit on XY plane | `target.Tween().Preset("OrbitXYClockwiseSoft").Play();` |
| `OrbitXYCounterClockwise` | 2s | Infinite | GameObject | Circular orbit on XY plane (counter-clockwise) | `target.Tween().Preset("OrbitXYCounterClockwise").Play();` |
| `OrbitXYCounterClockwiseHard` | 2s | Infinite | GameObject | Large-radius counter-clockwise orbit on XY plane | `target.Tween().Preset("OrbitXYCounterClockwiseHard").Play();` |
| `OrbitXYCounterClockwiseSoft` | 2s | Infinite | GameObject | Small-radius counter-clockwise orbit on XY plane | `target.Tween().Preset("OrbitXYCounterClockwiseSoft").Play();` |
| `OrbitXYHard` | 2s | Infinite | GameObject | Large-radius orbit on XY plane | `target.Tween().Preset("OrbitXYHard").Play();` |
| `OrbitXYSoft` | 2s | Infinite | GameObject | Small-radius orbit on XY plane | `target.Tween().Preset("OrbitXYSoft").Play();` |
| `OrbitXZ` | 2s | Infinite | GameObject | Circles around a point on XZ plane (counter-clockwise) | `target.Tween().Preset("OrbitXZ").Play();` |
| `OrbitXZClockwise` | 2s | Infinite | GameObject | Circles around a point on XZ plane (clockwise) | `target.Tween().Preset("OrbitXZClockwise").Play();` |
| `OrbitXZClockwiseHard` | 2s | Infinite | GameObject | Large-radius clockwise orbit on XZ plane | `target.Tween().Preset("OrbitXZClockwiseHard").Play();` |
| `OrbitXZClockwiseSoft` | 2s | Infinite | GameObject | Small-radius clockwise orbit on XZ plane | `target.Tween().Preset("OrbitXZClockwiseSoft").Play();` |
| `OrbitXZCounterClockwise` | 2s | Infinite | GameObject | Circles around a point on XZ plane (counter-clockwise) | `target.Tween().Preset("OrbitXZCounterClockwise").Play();` |
| `OrbitXZCounterClockwiseHard` | 2s | Infinite | GameObject | Large-radius counter-clockwise orbit on XZ plane | `target.Tween().Preset("OrbitXZCounterClockwiseHard").Play();` |
| `OrbitXZCounterClockwiseSoft` | 2s | Infinite | GameObject | Small-radius counter-clockwise orbit on XZ plane | `target.Tween().Preset("OrbitXZCounterClockwiseSoft").Play();` |
| `OrbitXZHard` | 2s | Infinite | GameObject | Large-radius orbit on XZ plane | `target.Tween().Preset("OrbitXZHard").Play();` |
| `OrbitXZSoft` | 2s | Infinite | GameObject | Small-radius orbit on XZ plane | `target.Tween().Preset("OrbitXZSoft").Play();` |
| `OrbitYZ` | 2s | Infinite | GameObject | Circular orbit on YZ plane | `target.Tween().Preset("OrbitYZ").Play();` |
| `OrbitYZClockwise` | 2s | Infinite | GameObject | Circular orbit on YZ plane (clockwise) | `target.Tween().Preset("OrbitYZClockwise").Play();` |
| `OrbitYZClockwiseHard` | 2s | Infinite | GameObject | Large-radius clockwise orbit on YZ plane | `target.Tween().Preset("OrbitYZClockwiseHard").Play();` |
| `OrbitYZClockwiseSoft` | 2s | Infinite | GameObject | Small-radius clockwise orbit on YZ plane | `target.Tween().Preset("OrbitYZClockwiseSoft").Play();` |
| `OrbitYZCounterClockwise` | 2s | Infinite | GameObject | Circular orbit on YZ plane (counter-clockwise) | `target.Tween().Preset("OrbitYZCounterClockwise").Play();` |
| `OrbitYZCounterClockwiseHard` | 2s | Infinite | GameObject | Large-radius counter-clockwise orbit on YZ plane | `target.Tween().Preset("OrbitYZCounterClockwiseHard").Play();` |
| `OrbitYZCounterClockwiseSoft` | 2s | Infinite | GameObject | Small-radius counter-clockwise orbit on YZ plane | `target.Tween().Preset("OrbitYZCounterClockwiseSoft").Play();` |
| `OrbitYZHard` | 2s | Infinite | GameObject | Large-radius orbit on YZ plane | `target.Tween().Preset("OrbitYZHard").Play();` |
| `OrbitYZSoft` | 2s | Infinite | GameObject | Small-radius orbit on YZ plane | `target.Tween().Preset("OrbitYZSoft").Play();` |

## Pendulum

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PendulumX` | 2.8s | Infinite | GameObject | Gentle X-axis pendulum loop | `target.Tween().Preset("PendulumX").Play();` |
| `PendulumXHard` | 3.5s | Infinite | GameObject | Wide X-axis pendulum loop | `target.Tween().Preset("PendulumXHard").Play();` |
| `PendulumXSoft` | 2.5s | Infinite | GameObject | Soft X-axis pendulum loop | `target.Tween().Preset("PendulumXSoft").Play();` |
| `PendulumY` | 2.8s | Infinite | GameObject | Gentle Y-axis pendulum loop | `target.Tween().Preset("PendulumY").Play();` |
| `PendulumYHard` | 3.5s | Infinite | GameObject | Wide Y-axis pendulum loop | `target.Tween().Preset("PendulumYHard").Play();` |
| `PendulumYSoft` | 2.5s | Infinite | GameObject | Soft Y-axis pendulum loop | `target.Tween().Preset("PendulumYSoft").Play();` |
| `PendulumZ` | 2.8s | Infinite | GameObject | Gentle Z-axis pendulum loop | `target.Tween().Preset("PendulumZ").Play();` |
| `PendulumZHard` | 3.5s | Infinite | GameObject | Wide Z-axis pendulum loop | `target.Tween().Preset("PendulumZHard").Play();` |
| `PendulumZSoft` | 2.5s | Infinite | GameObject | Soft Z-axis pendulum loop | `target.Tween().Preset("PendulumZSoft").Play();` |

## Pop

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PopIn` | 0.6s | One-shot | GameObject | Scales from 0 to original scale, no overshoot | `target.Tween().Preset("PopIn").Play();` |
| `PopInFade` | 2s | One-shot | Fade-capable GameObject | Scales and fades in together | `target.Tween().Preset("PopInFade").Play();` |
| `PopInFadeHard` | 1.4s | One-shot | Fade-capable GameObject | Hard scales and fades in together | `target.Tween().Preset("PopInFadeHard").Play();` |
| `PopInFadeSoft` | 2.5s | One-shot | Fade-capable GameObject | Soft scales and fades in together | `target.Tween().Preset("PopInFadeSoft").Play();` |
| `PopInHard` | 0.3s | One-shot | GameObject | Fast scale entrance, no overshoot | `target.Tween().Preset("PopInHard").Play();` |
| `PopInOvershoot` | 1s | One-shot | GameObject | Scales from 0 to original scale with overshoot | `target.Tween().Preset("PopInOvershoot").Play();` |
| `PopInOvershootHard` | 0.8s | One-shot | GameObject | Snappy scale entrance with strong overshoot | `target.Tween().Preset("PopInOvershootHard").Play();` |
| `PopInOvershootSoft` | 1.2s | One-shot | GameObject | Gentle scale entrance with mild OutBack overshoot | `target.Tween().Preset("PopInOvershootSoft").Play();` |
| `PopInSoft` | 0.8s | One-shot | GameObject | Gentle scale entrance, no overshoot | `target.Tween().Preset("PopInSoft").Play();` |
| `PopOut` | 0.4s | One-shot | GameObject | Scales to 0, no anticipation | `target.Tween().Preset("PopOut").Play();` |
| `PopOutFade` | 1.2s | One-shot | Fade-capable GameObject | Scales down and fades out together, no anticipation | `target.Tween().Preset("PopOutFade").Play();` |
| `PopOutFadeHard` | 0.8s | One-shot | Fade-capable GameObject | Hard scales down and fades out together | `target.Tween().Preset("PopOutFadeHard").Play();` |
| `PopOutFadeOvershoot` | 1.2s | One-shot | Fade-capable GameObject | Scales down and fades out with anticipation overshoot | `target.Tween().Preset("PopOutFadeOvershoot").Play();` |
| `PopOutFadeOvershootHard` | 0.8s | One-shot | Fade-capable GameObject | Hard scales down and fades out with strong overshoot | `target.Tween().Preset("PopOutFadeOvershootHard").Play();` |
| `PopOutFadeOvershootSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft scales down and fades out with mild overshoot | `target.Tween().Preset("PopOutFadeOvershootSoft").Play();` |
| `PopOutFadeSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft scales down and fades out together | `target.Tween().Preset("PopOutFadeSoft").Play();` |
| `PopOutHard` | 0.25s | One-shot | GameObject | Hard scale exit, no anticipation | `target.Tween().Preset("PopOutHard").Play();` |
| `PopOutOvershoot` | 0.4s | One-shot | GameObject | Scales to 0 with anticipation overshoot | `target.Tween().Preset("PopOutOvershoot").Play();` |
| `PopOutOvershootHard` | 0.25s | One-shot | GameObject | Hard scale exit with strong overshoot | `target.Tween().Preset("PopOutOvershootHard").Play();` |
| `PopOutOvershootSoft` | 0.6s | One-shot | GameObject | Soft scale exit with mild overshoot | `target.Tween().Preset("PopOutOvershootSoft").Play();` |
| `PopOutSoft` | 0.6s | One-shot | GameObject | Soft scale exit, no anticipation | `target.Tween().Preset("PopOutSoft").Play();` |

## Pulse

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PulseFade` | 2s | Infinite | Fade-capable GameObject | Smooth alpha pulse loop | `target.Tween().Preset("PulseFade").Play();` |
| `PulseFadeHard` | 1s | Infinite | Fade-capable GameObject | Fast punchy alpha pulse loop | `target.Tween().Preset("PulseFadeHard").Play();` |
| `PulseFadeSoft` | 3s | Infinite | Fade-capable GameObject | Slow gentle alpha pulse loop | `target.Tween().Preset("PulseFadeSoft").Play();` |

## PulseScale

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `PulseScale` | 0.28s | One-shot | GameObject | Quick scale bump for interactive feedback | `target.Tween().Preset("PulseScale").Play();` |
| `PulseScaleFade` | 0.84s | One-shot | Fade-capable GameObject | Pulse scale with alpha dip and return | `target.Tween().Preset("PulseScaleFade").Play();` |
| `PulseScaleFadeHard` | 1.05s | One-shot | Fade-capable GameObject | Bold pulse scale with deep alpha dip and return | `target.Tween().Preset("PulseScaleFadeHard").Play();` |
| `PulseScaleFadeSoft` | 0.75s | One-shot | Fade-capable GameObject | Soft pulse scale with alpha dip and return | `target.Tween().Preset("PulseScaleFadeSoft").Play();` |
| `PulseScaleHard` | 0.35s | One-shot | GameObject | Bold scale bump for emphatic feedback | `target.Tween().Preset("PulseScaleHard").Play();` |
| `PulseScaleSoft` | 0.25s | One-shot | GameObject | Soft scale bump for light feedback | `target.Tween().Preset("PulseScaleSoft").Play();` |

## Punch

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Punch` | 0.18s | One-shot | GameObject | Quick scale punch for feedback | `target.Tween().Preset("Punch").Play();` |
| `PunchHard` | 0.25s | One-shot | GameObject | Heavy scale punch for emphatic feedback | `target.Tween().Preset("PunchHard").Play();` |
| `PunchSoft` | 0.15s | One-shot | GameObject | Soft scale punch for delicate feedback | `target.Tween().Preset("PunchSoft").Play();` |

## Recoil

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Recoil` | 0.4s | One-shot | GameObject | Pull back then snap forward on local Z | `target.Tween().Preset("Recoil").Play();` |
| `RecoilHard` | 0.5s | One-shot | GameObject | Hard pull back then snap forward | `target.Tween().Preset("RecoilHard").Play();` |
| `RecoilSoft` | 0.3s | One-shot | GameObject | Soft pull back then snap forward | `target.Tween().Preset("RecoilSoft").Play();` |

## RecoilBack

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `RecoilBack` | 0.4s | One-shot | GameObject | Pull back then snap forward on local Z | `target.Tween().Preset("RecoilBack").Play();` |
| `RecoilBackHard` | 0.5s | One-shot | GameObject | Hard pull back then snap forward on local Z | `target.Tween().Preset("RecoilBackHard").Play();` |
| `RecoilBackSoft` | 0.3s | One-shot | GameObject | Soft pull back then snap forward on local Z | `target.Tween().Preset("RecoilBackSoft").Play();` |

## RecoilForward

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `RecoilForward` | 0.4s | One-shot | GameObject | Pull forward then snap back on local Z | `target.Tween().Preset("RecoilForward").Play();` |
| `RecoilForwardHard` | 0.5s | One-shot | GameObject | Hard pull forward then snap back on local Z | `target.Tween().Preset("RecoilForwardHard").Play();` |
| `RecoilForwardSoft` | 0.3s | One-shot | GameObject | Soft pull forward then snap back on local Z | `target.Tween().Preset("RecoilForwardSoft").Play();` |

## Shake

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Shake` | 0.45s | One-shot | GameObject | Random position shake | `target.Tween().Preset("Shake").Play();` |
| `ShakeFade` | 0.8s | One-shot | Fade-capable GameObject | Shake position with fade out | `target.Tween().Preset("ShakeFade").Play();` |
| `ShakeFadeHard` | 1s | One-shot | Fade-capable GameObject | Hard shake with fade out | `target.Tween().Preset("ShakeFadeHard").Play();` |
| `ShakeFadeSoft` | 0.6s | One-shot | Fade-capable GameObject | Soft shake with fade out | `target.Tween().Preset("ShakeFadeSoft").Play();` |
| `ShakeHard` | 0.6s | One-shot | GameObject | Heavy position shake | `target.Tween().Preset("ShakeHard").Play();` |
| `ShakeSoft` | 0.4s | One-shot | GameObject | Soft position shake | `target.Tween().Preset("ShakeSoft").Play();` |

## Slide

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `SlideInDown` | 1s | One-shot | GameObject | Slides down from above | `target.Tween().Preset("SlideInDown").Play();` |
| `SlideInDownHard` | 0.5s | One-shot | GameObject | Quickly slides down from above | `target.Tween().Preset("SlideInDownHard").Play();` |
| `SlideInDownSoft` | 1.5s | One-shot | GameObject | Slowly slides down from above | `target.Tween().Preset("SlideInDownSoft").Play();` |
| `SlideInFadeDown` | 2s | One-shot | Fade-capable GameObject | Slides down from above with fade in | `target.Tween().Preset("SlideInFadeDown").Play();` |
| `SlideInFadeDownHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides down from above with fade in | `target.Tween().Preset("SlideInFadeDownHard").Play();` |
| `SlideInFadeDownSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides down from above with fade in | `target.Tween().Preset("SlideInFadeDownSoft").Play();` |
| `SlideInFadeLeft` | 2.5s | One-shot | Fade-capable GameObject | Slides in from the left with fade in | `target.Tween().Preset("SlideInFadeLeft").Play();` |
| `SlideInFadeLeftHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides in from the left with fade in | `target.Tween().Preset("SlideInFadeLeftHard").Play();` |
| `SlideInFadeLeftSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides in from the left with fade in | `target.Tween().Preset("SlideInFadeLeftSoft").Play();` |
| `SlideInFadeRight` | 2.5s | One-shot | Fade-capable GameObject | Slides in from the right with fade in | `target.Tween().Preset("SlideInFadeRight").Play();` |
| `SlideInFadeRightHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides in from the right with fade in | `target.Tween().Preset("SlideInFadeRightHard").Play();` |
| `SlideInFadeRightSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides in from the right with fade in | `target.Tween().Preset("SlideInFadeRightSoft").Play();` |
| `SlideInFadeUp` | 2s | One-shot | Fade-capable GameObject | Slides up from below with fade in | `target.Tween().Preset("SlideInFadeUp").Play();` |
| `SlideInFadeUpHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides up from below with fade in | `target.Tween().Preset("SlideInFadeUpHard").Play();` |
| `SlideInFadeUpSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides up from below with fade in | `target.Tween().Preset("SlideInFadeUpSoft").Play();` |
| `SlideInLeft` | 1s | One-shot | GameObject | Slides in from the left side | `target.Tween().Preset("SlideInLeft").Play();` |
| `SlideInLeftHard` | 0.5s | One-shot | GameObject | Quickly slides in from the left | `target.Tween().Preset("SlideInLeftHard").Play();` |
| `SlideInLeftSoft` | 1.5s | One-shot | GameObject | Slowly slides in from the left | `target.Tween().Preset("SlideInLeftSoft").Play();` |
| `SlideInRight` | 1s | One-shot | GameObject | Slides in from the right side | `target.Tween().Preset("SlideInRight").Play();` |
| `SlideInRightHard` | 0.5s | One-shot | GameObject | Quickly slides in from the right | `target.Tween().Preset("SlideInRightHard").Play();` |
| `SlideInRightSoft` | 1.5s | One-shot | GameObject | Slowly slides in from the right | `target.Tween().Preset("SlideInRightSoft").Play();` |
| `SlideInUp` | 1s | One-shot | GameObject | Slides up from below | `target.Tween().Preset("SlideInUp").Play();` |
| `SlideInUpHard` | 0.5s | One-shot | GameObject | Quickly slides up from below | `target.Tween().Preset("SlideInUpHard").Play();` |
| `SlideInUpSoft` | 1.5s | One-shot | GameObject | Slowly slides up from below | `target.Tween().Preset("SlideInUpSoft").Play();` |
| `SlideOutDown` | 1s | One-shot | GameObject | Slides down off-screen | `target.Tween().Preset("SlideOutDown").Play();` |
| `SlideOutDownHard` | 0.5s | One-shot | GameObject | Quickly slides down off-screen | `target.Tween().Preset("SlideOutDownHard").Play();` |
| `SlideOutDownSoft` | 1.5s | One-shot | GameObject | Slowly slides down off-screen | `target.Tween().Preset("SlideOutDownSoft").Play();` |
| `SlideOutFadeDown` | 2s | One-shot | Fade-capable GameObject | Slides down while fading out | `target.Tween().Preset("SlideOutFadeDown").Play();` |
| `SlideOutFadeDownHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides down while fading out | `target.Tween().Preset("SlideOutFadeDownHard").Play();` |
| `SlideOutFadeDownSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides down while fading out | `target.Tween().Preset("SlideOutFadeDownSoft").Play();` |
| `SlideOutFadeLeft` | 2.5s | One-shot | Fade-capable GameObject | Slides left while fading out | `target.Tween().Preset("SlideOutFadeLeft").Play();` |
| `SlideOutFadeLeftHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides left while fading out | `target.Tween().Preset("SlideOutFadeLeftHard").Play();` |
| `SlideOutFadeLeftSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides left while fading out | `target.Tween().Preset("SlideOutFadeLeftSoft").Play();` |
| `SlideOutFadeRight` | 2.5s | One-shot | Fade-capable GameObject | Slides right while fading out | `target.Tween().Preset("SlideOutFadeRight").Play();` |
| `SlideOutFadeRightHard` | 1.5s | One-shot | Fade-capable GameObject | Quickly slides right while fading out | `target.Tween().Preset("SlideOutFadeRightHard").Play();` |
| `SlideOutFadeRightSoft` | 3.5s | One-shot | Fade-capable GameObject | Slowly slides right while fading out | `target.Tween().Preset("SlideOutFadeRightSoft").Play();` |
| `SlideOutFadeUp` | 2s | One-shot | Fade-capable GameObject | Slides up while fading out | `target.Tween().Preset("SlideOutFadeUp").Play();` |
| `SlideOutFadeUpHard` | 1s | One-shot | Fade-capable GameObject | Quickly slides up while fading out | `target.Tween().Preset("SlideOutFadeUpHard").Play();` |
| `SlideOutFadeUpSoft` | 3s | One-shot | Fade-capable GameObject | Slowly slides up while fading out | `target.Tween().Preset("SlideOutFadeUpSoft").Play();` |
| `SlideOutLeft` | 1s | One-shot | GameObject | Slides left off-screen | `target.Tween().Preset("SlideOutLeft").Play();` |
| `SlideOutLeftHard` | 0.5s | One-shot | GameObject | Quickly slides left off-screen | `target.Tween().Preset("SlideOutLeftHard").Play();` |
| `SlideOutLeftSoft` | 1.5s | One-shot | GameObject | Slowly slides left off-screen | `target.Tween().Preset("SlideOutLeftSoft").Play();` |
| `SlideOutRight` | 1s | One-shot | GameObject | Slides right off-screen | `target.Tween().Preset("SlideOutRight").Play();` |
| `SlideOutRightHard` | 0.5s | One-shot | GameObject | Quickly slides right off-screen | `target.Tween().Preset("SlideOutRightHard").Play();` |
| `SlideOutRightSoft` | 1.5s | One-shot | GameObject | Slowly slides right off-screen | `target.Tween().Preset("SlideOutRightSoft").Play();` |
| `SlideOutUp` | 1s | One-shot | GameObject | Slides up off-screen | `target.Tween().Preset("SlideOutUp").Play();` |
| `SlideOutUpHard` | 0.5s | One-shot | GameObject | Quickly slides up off-screen | `target.Tween().Preset("SlideOutUpHard").Play();` |
| `SlideOutUpSoft` | 1.5s | One-shot | GameObject | Slowly slides up off-screen | `target.Tween().Preset("SlideOutUpSoft").Play();` |

## Spin

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `SpinDiagonalXY` | 1s | One-shot | GameObject | Spins 360 degrees across X and Y axes | `target.Tween().Preset("SpinDiagonalXY").Play();` |
| `SpinDiagonalXYHard` | 0.5s | One-shot | GameObject | Fast diagonal spin across X and Y | `target.Tween().Preset("SpinDiagonalXYHard").Play();` |
| `SpinDiagonalXYSoft` | 1.5s | One-shot | GameObject | Slow diagonal spin across X and Y | `target.Tween().Preset("SpinDiagonalXYSoft").Play();` |
| `SpinDiagonalXZ` | 1s | One-shot | GameObject | Spins 360 degrees across X and Z axes | `target.Tween().Preset("SpinDiagonalXZ").Play();` |
| `SpinDiagonalXZHard` | 0.5s | One-shot | GameObject | Fast diagonal spin across X and Z | `target.Tween().Preset("SpinDiagonalXZHard").Play();` |
| `SpinDiagonalXZSoft` | 1.5s | One-shot | GameObject | Slow diagonal spin across X and Z | `target.Tween().Preset("SpinDiagonalXZSoft").Play();` |
| `SpinDiagonalYZ` | 1s | One-shot | GameObject | Spins 360 degrees across Y and Z axes | `target.Tween().Preset("SpinDiagonalYZ").Play();` |
| `SpinDiagonalYZHard` | 0.5s | One-shot | GameObject | Fast diagonal spin across Y and Z | `target.Tween().Preset("SpinDiagonalYZHard").Play();` |
| `SpinDiagonalYZSoft` | 1.5s | One-shot | GameObject | Slow diagonal spin across Y and Z | `target.Tween().Preset("SpinDiagonalYZSoft").Play();` |
| `SpinX` | 1s | One-shot | GameObject | Spins 360 degrees on X axis | `target.Tween().Preset("SpinX").Play();` |
| `SpinXHard` | 0.5s | One-shot | GameObject | Fast spin on X axis | `target.Tween().Preset("SpinXHard").Play();` |
| `SpinXSoft` | 1.5s | One-shot | GameObject | Slow spin on X axis | `target.Tween().Preset("SpinXSoft").Play();` |
| `SpinY` | 1s | One-shot | GameObject | Spins 360 degrees on Y axis | `target.Tween().Preset("SpinY").Play();` |
| `SpinYHard` | 0.5s | One-shot | GameObject | Fast spin on Y axis | `target.Tween().Preset("SpinYHard").Play();` |
| `SpinYSoft` | 1.5s | One-shot | GameObject | Slow spin on Y axis | `target.Tween().Preset("SpinYSoft").Play();` |
| `SpinZ` | 1s | One-shot | GameObject | Spins 360 degrees on Z axis | `target.Tween().Preset("SpinZ").Play();` |
| `SpinZHard` | 0.5s | One-shot | GameObject | Fast spin on Z axis | `target.Tween().Preset("SpinZHard").Play();` |
| `SpinZSoft` | 1.5s | One-shot | GameObject | Slow spin on Z axis | `target.Tween().Preset("SpinZSoft").Play();` |

## SpinScale

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `SpinScaleIn` | 2.1s | One-shot | GameObject | Spin and grow in from zero | `target.Tween().Preset("SpinScaleIn").Play();` |
| `SpinScaleInHard` | 1.5s | One-shot | GameObject | Hard spin and grow in from zero | `target.Tween().Preset("SpinScaleInHard").Play();` |
| `SpinScaleInOvershoot` | 2.1s | One-shot | GameObject | Spin and grow in with overshoot settle | `target.Tween().Preset("SpinScaleInOvershoot").Play();` |
| `SpinScaleInOvershootHard` | 1.5s | One-shot | GameObject | Hard spin and grow in with strong overshoot | `target.Tween().Preset("SpinScaleInOvershootHard").Play();` |
| `SpinScaleInOvershootSoft` | 2.7s | One-shot | GameObject | Soft spin and grow in with mild overshoot | `target.Tween().Preset("SpinScaleInOvershootSoft").Play();` |
| `SpinScaleInSoft` | 2.7s | One-shot | GameObject | Soft spin and grow in from zero | `target.Tween().Preset("SpinScaleInSoft").Play();` |
| `SpinScaleOut` | 0.7s | One-shot | GameObject | Spin and shrink to zero, no anticipation | `target.Tween().Preset("SpinScaleOut").Play();` |
| `SpinScaleOutHard` | 0.5s | One-shot | GameObject | Hard spin and shrink to zero | `target.Tween().Preset("SpinScaleOutHard").Play();` |
| `SpinScaleOutOvershoot` | 0.7s | One-shot | GameObject | Spin and shrink to zero with anticipation overshoot | `target.Tween().Preset("SpinScaleOutOvershoot").Play();` |
| `SpinScaleOutOvershootHard` | 0.5s | One-shot | GameObject | Hard spin and shrink with strong anticipation | `target.Tween().Preset("SpinScaleOutOvershootHard").Play();` |
| `SpinScaleOutOvershootSoft` | 0.9s | One-shot | GameObject | Soft spin and shrink with mild anticipation | `target.Tween().Preset("SpinScaleOutOvershootSoft").Play();` |
| `SpinScaleOutSoft` | 0.9s | One-shot | GameObject | Soft spin and shrink to zero | `target.Tween().Preset("SpinScaleOutSoft").Play();` |

## Spiral

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Spiral` | 1.5s | One-shot | GameObject | Spirals upward combining rotation and height | `target.Tween().Preset("Spiral").Play();` |
| `SpiralHard` | 2s | One-shot | GameObject | Dramatic upward spiral with wider radius | `target.Tween().Preset("SpiralHard").Play();` |
| `SpiralSoft` | 1.2s | One-shot | GameObject | Gentle upward spiral with smaller radius | `target.Tween().Preset("SpiralSoft").Play();` |
| `SwirlIn` | 1s | One-shot | GameObject | Spin and scale in from zero | `target.Tween().Preset("SwirlIn").Play();` |
| `SwirlInHard` | 1.3s | One-shot | GameObject | Dramatic spin and scale in from zero | `target.Tween().Preset("SwirlInHard").Play();` |
| `SwirlInSoft` | 0.8s | One-shot | GameObject | Gentle spin and scale in from zero | `target.Tween().Preset("SwirlInSoft").Play();` |

## Squash

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Squash` | 0.6s | One-shot | GameObject | Squash and stretch pulse | `target.Tween().Preset("Squash").Play();` |
| `SquashHard` | 0.7s | One-shot | GameObject | Hard squash and stretch pulse | `target.Tween().Preset("SquashHard").Play();` |
| `SquashSoft` | 0.5s | One-shot | GameObject | Soft squash and stretch pulse | `target.Tween().Preset("SquashSoft").Play();` |

## Sway

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Sway` | 3.5s | Infinite | GameObject | Gentle horizontal sway loop | `target.Tween().Preset("Sway").Play();` |
| `SwayHard` | 5s | Infinite | GameObject | Wide horizontal sway loop | `target.Tween().Preset("SwayHard").Play();` |
| `SwaySoft` | 3s | Infinite | GameObject | Soft horizontal sway loop | `target.Tween().Preset("SwaySoft").Play();` |

## Tilt

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `Tilt` | 0.4s | One-shot | GameObject | Lean on Z then spring back | `target.Tween().Preset("Tilt").Play();` |
| `TiltHard` | 0.3s | One-shot | GameObject | Hard lean on Z then spring back | `target.Tween().Preset("TiltHard").Play();` |
| `TiltSoft` | 0.5s | One-shot | GameObject | Soft lean on Z then spring back | `target.Tween().Preset("TiltSoft").Play();` |

## Wobble

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `WobbleDiagonalXY` | 0.5s | One-shot | GameObject | Wobbles rotation diagonally across X and Y | `target.Tween().Preset("WobbleDiagonalXY").Play();` |
| `WobbleDiagonalXYHard` | 0.6s | One-shot | GameObject | Heavy diagonal XY wobble | `target.Tween().Preset("WobbleDiagonalXYHard").Play();` |
| `WobbleDiagonalXYSoft` | 0.4s | One-shot | GameObject | Soft diagonal XY wobble | `target.Tween().Preset("WobbleDiagonalXYSoft").Play();` |
| `WobbleDiagonalXZ` | 0.5s | One-shot | GameObject | Wobbles rotation diagonally across X and Z | `target.Tween().Preset("WobbleDiagonalXZ").Play();` |
| `WobbleDiagonalXZHard` | 0.6s | One-shot | GameObject | Heavy diagonal XZ wobble | `target.Tween().Preset("WobbleDiagonalXZHard").Play();` |
| `WobbleDiagonalXZSoft` | 0.4s | One-shot | GameObject | Soft diagonal XZ wobble | `target.Tween().Preset("WobbleDiagonalXZSoft").Play();` |
| `WobbleDiagonalYZ` | 0.5s | One-shot | GameObject | Wobbles rotation diagonally across Y and Z | `target.Tween().Preset("WobbleDiagonalYZ").Play();` |
| `WobbleDiagonalYZHard` | 0.6s | One-shot | GameObject | Heavy diagonal YZ wobble | `target.Tween().Preset("WobbleDiagonalYZHard").Play();` |
| `WobbleDiagonalYZSoft` | 0.4s | One-shot | GameObject | Soft diagonal YZ wobble | `target.Tween().Preset("WobbleDiagonalYZSoft").Play();` |
| `WobbleFadeX` | 2s | One-shot | Fade-capable GameObject | Wobble on X axis with fade out | `target.Tween().Preset("WobbleFadeX").Play();` |
| `WobbleFadeXHard` | 2.4s | One-shot | Fade-capable GameObject | Heavy wobble on X axis with fade out | `target.Tween().Preset("WobbleFadeXHard").Play();` |
| `WobbleFadeXSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft wobble on X axis with fade out | `target.Tween().Preset("WobbleFadeXSoft").Play();` |
| `WobbleFadeY` | 2s | One-shot | Fade-capable GameObject | Wobble on Y axis with fade out | `target.Tween().Preset("WobbleFadeY").Play();` |
| `WobbleFadeYHard` | 2.4s | One-shot | Fade-capable GameObject | Heavy wobble on Y axis with fade out | `target.Tween().Preset("WobbleFadeYHard").Play();` |
| `WobbleFadeYSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft wobble on Y axis with fade out | `target.Tween().Preset("WobbleFadeYSoft").Play();` |
| `WobbleFadeZ` | 1.8s | One-shot | Fade-capable GameObject | Wobble on Z axis with fade out | `target.Tween().Preset("WobbleFadeZ").Play();` |
| `WobbleFadeZHard` | 2.4s | One-shot | Fade-capable GameObject | Heavy wobble on Z axis with fade out | `target.Tween().Preset("WobbleFadeZHard").Play();` |
| `WobbleFadeZSoft` | 1.6s | One-shot | Fade-capable GameObject | Soft wobble on Z axis with fade out | `target.Tween().Preset("WobbleFadeZSoft").Play();` |
| `WobbleX` | 0.5s | One-shot | GameObject | Wobbles rotation back and forth on X | `target.Tween().Preset("WobbleX").Play();` |
| `WobbleXHard` | 0.6s | One-shot | GameObject | Heavy X-axis wobble | `target.Tween().Preset("WobbleXHard").Play();` |
| `WobbleXSoft` | 0.4s | One-shot | GameObject | Soft X-axis wobble | `target.Tween().Preset("WobbleXSoft").Play();` |
| `WobbleY` | 0.5s | One-shot | GameObject | Wobbles rotation back and forth on Y | `target.Tween().Preset("WobbleY").Play();` |
| `WobbleYHard` | 0.6s | One-shot | GameObject | Heavy Y-axis wobble | `target.Tween().Preset("WobbleYHard").Play();` |
| `WobbleYSoft` | 0.4s | One-shot | GameObject | Soft Y-axis wobble | `target.Tween().Preset("WobbleYSoft").Play();` |
| `WobbleZ` | 0.45s | One-shot | GameObject | Wobbles rotation back and forth on Z | `target.Tween().Preset("WobbleZ").Play();` |
| `WobbleZHard` | 0.6s | One-shot | GameObject | Heavy Z-axis wobble | `target.Tween().Preset("WobbleZHard").Play();` |
| `WobbleZSoft` | 0.4s | One-shot | GameObject | Soft Z-axis wobble | `target.Tween().Preset("WobbleZSoft").Play();` |

## ZigZag

| Preset | Duration | Loop | Target | Description | Fluent API |
| --- | ---: | --- | --- | --- | --- |
| `ZigZag` | 1s | One-shot | GameObject | Alternating diagonal zig-zag movement | `target.Tween().Preset("ZigZag").Play();` |
| `ZigZagHard` | 0.8s | One-shot | GameObject | Hard alternating diagonal movement | `target.Tween().Preset("ZigZagHard").Play();` |
| `ZigZagSoft` | 1.2s | One-shot | GameObject | Soft alternating diagonal movement | `target.Tween().Preset("ZigZagSoft").Play();` |
