using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Applies a quick scale punch for tactile feedback, snapping outward then settling back to original scale.
    /// <para>
    /// Uses <c>DOPunchScale</c> with punch vector <c>Vector3.one * 0.11</c>, vibrato <c>5</c>,
    /// and elasticity <c>0.7</c>. The punch creates a brief scale burst that oscillates and decays.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.18s | <b>Default ease:</b> DOTween punch default<br/>
    /// <b>Easing override:</b> No ease override (punch tweens use internal oscillation).
    /// </para>
    /// <para>
    /// <b>Use cases:</b> Button press feedback, hit confirmation, score increment, interaction acknowledgment.
    /// </para>
    /// Usage: <c>transform.Tween().Preset("Punch").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PunchPreset : CodePreset
    {
        public override string PresetName => "Punch";
        public override string Description => "Quick scale punch for feedback";
        public override float DefaultDuration => 0.18f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchScale(Vector3.one * 0.11f, GetDuration(duration), 5, 0.7f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Soft scale punch with smaller vector and fewer vibrations, for delicate feedback.
    /// <para>
    /// Uses <c>DOPunchScale</c> with punch vector <c>Vector3.one * 0.08</c>, vibrato <c>4</c>, elasticity <c>0.7</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.15s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PunchSoft").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PunchSoftPreset : CodePreset
    {
        public override string PresetName => "PunchSoft";
        public override string Description => "Soft scale punch for delicate feedback";
        public override float DefaultDuration => 0.15f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchScale(Vector3.one * 0.08f, GetDuration(duration), 4, 0.7f)
                .WithDefaults(options, target);
        }
    }

    /// <summary>
    /// Heavy scale punch with larger vector and more vibrations, for emphatic feedback.
    /// <para>
    /// Uses <c>DOPunchScale</c> with punch vector <c>Vector3.one * 0.25</c>, vibrato <c>8</c>, elasticity <c>0.7</c>.
    /// </para>
    /// <para>
    /// <b>Type:</b> One-shot feedback | <b>Default duration:</b> 0.25s
    /// </para>
    /// Usage: <c>transform.Tween().Preset("PunchHard").Play();</c>
    /// </summary>
    [AutoRegisterPreset]
    public class PunchHardPreset : CodePreset
    {
        public override string PresetName => "PunchHard";
        public override string Description => "Heavy scale punch for emphatic feedback";
        public override float DefaultDuration => 0.25f;


        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOPunchScale(Vector3.one * 0.25f, GetDuration(duration), 8, 0.7f)
                .WithDefaults(options, target);
        }
    }
}
