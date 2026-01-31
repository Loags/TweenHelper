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

            startRadius = Mathf.Max(0.01f, startRadius);
            endRadius = Mathf.Max(0.01f, endRadius);

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
    /// Orbits the target counter-clockwise around its starting position on the XZ plane using callback-chain looping.
    /// <para>
    /// Uses <c>DOVirtual.Float</c> to drive a parametric circle (cos/sin) on X/Z with configurable start/end radii
    /// (default 1.0). Supports Rigidbody targets via <c>MovePosition</c>. Runs on <c>UpdateType.Fixed</c>.
    /// Callback-chain looping restarts each full 2π cycle; delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression (Linear recommended for uniform speed).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Orbiting satellite, patrol path, circular particle motion, planetary display.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Orbit").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitPreset : OrbitBasePreset
    {
        public override string PresetName => "Orbit";
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
    /// Identical to <see cref="OrbitPreset"/> but with reversed direction (clockwise). Uses the shared
    /// <c>OrbitTweenFactory</c> with <c>clockwise: true</c>, inverting the angular direction.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Clockwise patrol, reversed orbital motion, mirrored satellite.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitClockwisePreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitClockwise";
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
    /// Explicit counter-clockwise variant of <see cref="OrbitPreset"/>. Functionally identical to Orbit
    /// but provides a semantically clear name when both directions are used in the same project.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Counter-clockwise patrol, explicit direction pairing with OrbitClockwise.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("OrbitCounterClockwise").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class OrbitCounterClockwisePreset : OrbitBasePreset
    {
        public override string PresetName => "OrbitCounterClockwise";
        public override string Description => "Circles around a point on XZ plane (counter-clockwise)";
        public override float DefaultDuration => 2f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return CreateOrbitTween(target, duration, options, clockwise: false);
        }
    }

    /// <summary>
    /// Orbits the target in a circle on the XY plane (2D-friendly) using callback-chain looping.
    /// <para>
    /// Uses <c>DOVirtual.Float</c> driving a parametric circle with cos on X and sin on Y,
    /// radius of <c>1.0</c>, centered on the starting world position. Unlike the XZ-plane Orbit presets,
    /// this operates on XY for 2D games. Callback-chain looping restarts each full 2π cycle;
    /// delay applies only on first cycle.
    /// </para>
    /// <para>
    /// <b>Type:</b> Looping (callback-chain) | <b>Default duration:</b> 2.0s per revolution | <b>Default ease:</b> Linear<br/>
    /// <b>Easing override:</b> Primary ease controls angular progression.
    /// </para>
    /// <para>
    /// <b>Use cases:</b> 2D orbiting object, circular indicator, shield rotation, radial menu animation.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Orbit2D").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class Orbit2DPreset : CodePreset
    {
        public override string PresetName => "Orbit2D";
        public override string Description => "Circular orbit on XY plane";
        public override float DefaultDuration => 2.0f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            var t = target.transform;
            float dur = GetDuration(duration);
            var initialCenter = t.position;
            const float radius = 1f;
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
                        Vector3 offset = new Vector3(
                            Mathf.Cos(angle) * radius,
                            Mathf.Sin(angle) * radius,
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
}
