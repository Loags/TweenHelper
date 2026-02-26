using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Abstract base class providing shared orbit logic with configurable direction and radius interpolation.
    /// <para>
    /// Exposes serialized <c>startRadius</c> and <c>endRadius</c> fields (default 1.0 each) and a
    /// <c>CreateOrbitTween</c> helper that delegates to <c>OrbitTweenFactory</c>. Subclasses only need
    /// to specify the direction (clockwise or counter-clockwise).
    /// </para>
    /// </summary>
    public abstract class OrbitBasePreset : CodePreset
    {
        [SerializeField] private float startRadius = 1f;
        [SerializeField] private float endRadius = 1f;

        protected Tween CreateOrbitTween(GameObject target, float? duration, TweenOptions options, bool clockwise, float? startRadiusOverride = null, float? endRadiusOverride = null)
        {
            float startR = Mathf.Max(0.01f, startRadiusOverride ?? startRadius);
            float endR = Mathf.Max(0.01f, endRadiusOverride ?? endRadius);
            return OrbitTweenFactory.Create(target, duration, options, clockwise, startR, endR);
        }
    }

    internal static class OrbitTweenFactory
    {
        public static Tween Create(GameObject target, float? duration, TweenOptions options, bool clockwise, float startRadius, float endRadius)
        {
            var t = target.transform;
            var rb = target.GetComponent<Rigidbody>();
            var strength = CodePreset.ResolveStrengthStatic(options);

            startRadius = Mathf.Max(0.01f, startRadius) * strength;
            endRadius = Mathf.Max(0.01f, endRadius) * strength;

            float dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var initialCenter = t.position;
            float direction = clockwise ? -1f : 1f;
            const float fullCycle = Mathf.PI * 2f;
            var loopOptions = options;
            var orbitEase = loopOptions.Ease ?? Ease.Linear;
            if (!loopOptions.Ease.HasValue)
            {
                loopOptions = loopOptions.SetEase(orbitEase);
            }
            loopOptions = loopOptions.SetUpdateType(UpdateType.Fixed);

            bool applyDelay = true;
            Tween tween = null;

            Tween CreateCycle()
            {
                tween = DOVirtual.Float(0f, fullCycle, dur, angle =>
                    {
                        var normalized = Mathf.Repeat(angle / fullCycle, 1f);
                        float radius = Mathf.Lerp(startRadius, endRadius, normalized);
                        float directedAngle = angle * direction;

                        Vector3 offset = new Vector3(
                            Mathf.Cos(directedAngle) * radius,
                            0f,
                            Mathf.Sin(directedAngle) * radius
                        );

                        Vector3 targetPos = initialCenter + offset;

                        if (rb && rb.isKinematic == false)
                        {
                            rb.MovePosition(targetPos);
                        }
                        else
                        {
                            t.position = targetPos;
                        }
                    })
                    .SetEase(orbitEase)
                    .SetUpdate(UpdateType.Fixed)
                    .WithLoopDefaults(loopOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(() => CreateCycle());
                return tween;
            }

            return CreateCycle();
        }
    }

    /// <summary>
    /// Internal factory for XY-plane orbit variants sharing the same callback-chain loop structure.
    /// </summary>
    internal static class OrbitXYFactory
    {
        public static Tween Create(GameObject target, float? duration, TweenOptions options, bool clockwise, float radius)
        {
            var t = target.transform;
            var strength = CodePreset.ResolveStrengthStatic(options);
            float scaledRadius = radius * strength;
            float dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var initialCenter = t.position;
            float direction = clockwise ? -1f : 1f;
            const float fullCycle = Mathf.PI * 2f;
            var orbitEase = options.Ease ?? Ease.Linear;
            var loopOptions = options;
            if (!loopOptions.Ease.HasValue)
            {
                loopOptions = loopOptions.SetEase(orbitEase);
            }

            bool applyDelay = true;
            Tween tween = null;

            Tween CreateCycle()
            {
                tween = DOVirtual.Float(0f, fullCycle, dur, angle =>
                    {
                        float directedAngle = angle * direction;
                        Vector3 offset = new Vector3(
                            Mathf.Cos(directedAngle) * scaledRadius,
                            Mathf.Sin(directedAngle) * scaledRadius,
                            0f
                        );
                        t.position = initialCenter + offset;
                    })
                    .SetEase(orbitEase)
                    .WithLoopDefaults(loopOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(() => CreateCycle());
                return tween;
            }

            return CreateCycle();
        }
    }

    /// <summary>
    /// Orbits the target counter-clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Uses <c>DOVirtual.Float</c> to drive a parametric circle (cos/sin) on X/Z with configurable start/end radii
    /// (default 1.0). Supports Rigidbody targets via <c>MovePosition</c>. Runs on <c>UpdateType.Fixed</c>.
    /// Callback-chain looping restarts each full 2pi cycle; delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression (Linear recommended for uniform speed).<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Orbiting satellite, patrol path, circular particle motion, planetary display.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZ";
        public override string Description => "Circles around a point on XZ plane (counter-clockwise)";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false);
        }
    }

    /// <summary>
    /// Orbits the target clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Identical to <see cref="OrbitXZPreset"/> but with reversed direction (clockwise). Uses the shared
    /// <c>OrbitTweenFactory</c> with <c>clockwise: true</c>, inverting the angular direction.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Clockwise patrol, reversed orbital motion, mirrored satellite.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZClockwisePreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZClockwise";
        public override string Description => "Circles around a point on XZ plane (clockwise)";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: true);
        }
    }

    /// <summary>
    /// Orbits the target counter-clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Explicit counter-clockwise variant of <see cref="OrbitXZPreset"/>. Functionally identical to OrbitXZ
    /// but provides a semantically clear name when both directions are used in the same project.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Counter-clockwise patrol, explicit direction pairing with OrbitXZClockwise.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZCounterClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZCounterClockwisePreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZCounterClockwise";
        public override string Description => "Circles around a point on XZ plane (counter-clockwise)";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false);
        }
    }

    /// <summary>
    /// Soft clockwise XZ-plane orbit with a small radius (0.5).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZClockwiseSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZClockwiseSoftPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZClockwiseSoft";
        public override string Description => "Small-radius clockwise orbit on XZ plane";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: true, startRadiusOverride: 0.5f, endRadiusOverride: 0.5f);
        }
    }

    /// <summary>
    /// Hard clockwise XZ-plane orbit with a large radius (2.0).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZClockwiseHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZClockwiseHardPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZClockwiseHard";
        public override string Description => "Large-radius clockwise orbit on XZ plane";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: true, startRadiusOverride: 2f, endRadiusOverride: 2f);
        }
    }

    /// <summary>
    /// Soft counter-clockwise XZ-plane orbit with a small radius (0.5).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZCounterClockwiseSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZCounterClockwiseSoftPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZCounterClockwiseSoft";
        public override string Description => "Small-radius counter-clockwise orbit on XZ plane";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false, startRadiusOverride: 0.5f, endRadiusOverride: 0.5f);
        }
    }

    /// <summary>
    /// Hard counter-clockwise XZ-plane orbit with a large radius (2.0).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZCounterClockwiseHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZCounterClockwiseHardPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZCounterClockwiseHard";
        public override string Description => "Large-radius counter-clockwise orbit on XZ plane";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false, startRadiusOverride: 2f, endRadiusOverride: 2f);
        }
    }

    /// <summary>
    /// Soft XZ-plane orbit with a small radius (0.5).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZSoftPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZSoft";
        public override string Description => "Small-radius orbit on XZ plane";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false, startRadiusOverride: 0.5f, endRadiusOverride: 0.5f);
        }
    }

    /// <summary>
    /// Hard XZ-plane orbit with a large radius (2.0).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXZHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXZHardPreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitXZHard";
        public override string Description => "Large-radius orbit on XZ plane";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false, startRadiusOverride: 2f, endRadiusOverride: 2f);
        }
    }

    /// <summary>
    /// Orbits the target in a circle on the XY plane (2D-friendly) using callback-chain looping.
    /// <para>
    /// Uses <c>DOVirtual.Float</c> driving a parametric circle with cos on X and sin on Y,
    /// radius of <c>1.0</c>, centered on the starting world position. Unlike the XZ-plane Orbit presets,
    /// this operates on XY for 2D games. Callback-chain looping restarts each full 2pi cycle;
    /// delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D orbiting object, circular indicator, shield rotation, radial menu animation.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXY").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYPreset : CodePreset
    {
        public override string PresetName => "OrbitXY";
        public override string Description => "Circular orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 1f);
        }
    }

    /// <summary>
    /// Orbits the target clockwise on the XY plane using callback-chain looping.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYClockwisePreset : CodePreset
    {
        public override string PresetName => "OrbitXYClockwise";
        public override string Description => "Circular orbit on XY plane (clockwise)";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: true, radius: 1f);
        }
    }

    /// <summary>
    /// Orbits the target counter-clockwise on the XY plane using callback-chain looping.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYCounterClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYCounterClockwisePreset : CodePreset
    {
        public override string PresetName => "OrbitXYCounterClockwise";
        public override string Description => "Circular orbit on XY plane (counter-clockwise)";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 1f);
        }
    }

    /// <summary>
    /// Soft clockwise XY-plane orbit with a small radius (0.5).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYClockwiseSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYClockwiseSoftPreset : CodePreset
    {
        public override string PresetName => "OrbitXYClockwiseSoft";
        public override string Description => "Small-radius clockwise orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: true, radius: 0.5f);
        }
    }

    /// <summary>
    /// Hard clockwise XY-plane orbit with a large radius (2.0).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYClockwiseHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYClockwiseHardPreset : CodePreset
    {
        public override string PresetName => "OrbitXYClockwiseHard";
        public override string Description => "Large-radius clockwise orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: true, radius: 2f);
        }
    }

    /// <summary>
    /// Soft counter-clockwise XY-plane orbit with a small radius (0.5).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYCounterClockwiseSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYCounterClockwiseSoftPreset : CodePreset
    {
        public override string PresetName => "OrbitXYCounterClockwiseSoft";
        public override string Description => "Small-radius counter-clockwise orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 0.5f);
        }
    }

    /// <summary>
    /// Hard counter-clockwise XY-plane orbit with a large radius (2.0).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYCounterClockwiseHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYCounterClockwiseHardPreset : CodePreset
    {
        public override string PresetName => "OrbitXYCounterClockwiseHard";
        public override string Description => "Large-radius counter-clockwise orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 2f);
        }
    }

    /// <summary>
    /// Soft XY-plane orbit with a small radius (0.5).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYSoftPreset : CodePreset
    {
        public override string PresetName => "OrbitXYSoft";
        public override string Description => "Small-radius orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 0.5f);
        }
    }

    /// <summary>
    /// Hard XY-plane orbit with a large radius (2.0).
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitXYHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitXYHardPreset : CodePreset
    {
        public override string PresetName => "OrbitXYHard";
        public override string Description => "Large-radius orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitXYFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 2f);
        }
    }

    /// <summary>
    /// Internal factory for YZ-plane orbit variants sharing the same callback-chain loop structure.
    /// </summary>
    internal static class OrbitYZFactory
    {
        public static Tween Create(GameObject target, float? duration, TweenOptions options, bool clockwise, float radius)
        {
            var t = target.transform;
            var strength = CodePreset.ResolveStrengthStatic(options);
            float scaledRadius = radius * strength;
            float dur = duration ?? TweenHelperSettings.Instance.DefaultDuration;
            var initialCenter = t.position;
            float direction = clockwise ? -1f : 1f;
            const float fullCycle = Mathf.PI * 2f;
            var orbitEase = options.Ease ?? Ease.Linear;
            var loopOptions = options;
            if (!loopOptions.Ease.HasValue)
            {
                loopOptions = loopOptions.SetEase(orbitEase);
            }

            bool applyDelay = true;
            Tween tween = null;

            Tween CreateCycle()
            {
                tween = DOVirtual.Float(0f, fullCycle, dur, angle =>
                    {
                        float directedAngle = angle * direction;
                        Vector3 offset = new Vector3(
                            0f,
                            Mathf.Cos(directedAngle) * scaledRadius,
                            Mathf.Sin(directedAngle) * scaledRadius
                        );
                        t.position = initialCenter + offset;
                    })
                    .SetEase(orbitEase)
                    .WithLoopDefaults(loopOptions, target, applyDelay);

                applyDelay = false;
                tween.OnComplete(() => CreateCycle());
                return tween;
            }

            return CreateCycle();
        }
    }

    /// <summary>
    /// Orbits the target in a circle on the YZ plane using callback-chain looping.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitYZ").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZPreset : CodePreset
    {
        public override string PresetName => "OrbitYZ";
        public override string Description => "Circular orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 1f);
        }
    }

    /// <summary>
    /// Orbits the target clockwise on the YZ plane using callback-chain looping.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitYZClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZClockwisePreset : CodePreset
    {
        public override string PresetName => "OrbitYZClockwise";
        public override string Description => "Circular orbit on YZ plane (clockwise)";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: true, radius: 1f);
        }
    }

    /// <summary>
    /// Orbits the target counter-clockwise on the YZ plane using callback-chain looping.
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Strength override:</b> Multiplies orbit radius (default 1.0).
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitYZCounterClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZCounterClockwisePreset : CodePreset
    {
        public override string PresetName => "OrbitYZCounterClockwise";
        public override string Description => "Circular orbit on YZ plane (counter-clockwise)";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 1f);
        }
    }

    /// <summary>
    /// Soft clockwise YZ-plane orbit with a small radius (0.5).
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZClockwiseSoftPreset : CodePreset
    {
        public override string PresetName => "OrbitYZClockwiseSoft";
        public override string Description => "Small-radius clockwise orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: true, radius: 0.5f);
        }
    }

    /// <summary>
    /// Hard clockwise YZ-plane orbit with a large radius (2.0).
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZClockwiseHardPreset : CodePreset
    {
        public override string PresetName => "OrbitYZClockwiseHard";
        public override string Description => "Large-radius clockwise orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: true, radius: 2f);
        }
    }

    /// <summary>
    /// Soft counter-clockwise YZ-plane orbit with a small radius (0.5).
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZCounterClockwiseSoftPreset : CodePreset
    {
        public override string PresetName => "OrbitYZCounterClockwiseSoft";
        public override string Description => "Small-radius counter-clockwise orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 0.5f);
        }
    }

    /// <summary>
    /// Hard counter-clockwise YZ-plane orbit with a large radius (2.0).
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZCounterClockwiseHardPreset : CodePreset
    {
        public override string PresetName => "OrbitYZCounterClockwiseHard";
        public override string Description => "Large-radius counter-clockwise orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 2f);
        }
    }

    /// <summary>
    /// Soft YZ-plane orbit with a small radius (0.5).
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZSoftPreset : CodePreset
    {
        public override string PresetName => "OrbitYZSoft";
        public override string Description => "Small-radius orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 0.5f);
        }
    }

    /// <summary>
    /// Hard YZ-plane orbit with a large radius (2.0).
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitYZHardPreset : CodePreset
    {
        public override string PresetName => "OrbitYZHard";
        public override string Description => "Large-radius orbit on YZ plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return OrbitYZFactory.Create(target, duration ?? GetDuration(null, options), options, clockwise: false, radius: 2f);
        }
    }
}
