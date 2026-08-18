using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>One-line semantic gameplay-state feedback animations.</summary>
    public static class GameplayStateFeedbackExtensions
    {
        public static TweenHandle AbilityCharging(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.AbilityCharging(duration, accentColor, options);
        public static TweenHandle AbilityCharging(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).AbilityCharging(duration, accentColor).Play();
        public static TweenHandle AbilityReady(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.AbilityReady(duration, accentColor, options);
        public static TweenHandle AbilityReady(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).AbilityReady(duration, accentColor).Play();
        public static TweenHandle DodgeRoll(this Component target, float? duration = null, TweenOptions options = default) => target.gameObject.DodgeRoll(duration, options);
        public static TweenHandle DodgeRoll(this GameObject target, float? duration = null, TweenOptions options = default) => target.Tween().WithOptions(options).DodgeRoll(duration).Play();
        public static TweenHandle StunStart(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.StunStart(duration, accentColor, options);
        public static TweenHandle StunStart(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).StunStart(duration, accentColor).Play();
        public static TweenHandle StunEnd(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.StunEnd(duration, accentColor, options);
        public static TweenHandle StunEnd(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).StunEnd(duration, accentColor).Play();
        public static TweenHandle BuffApplied(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.BuffApplied(duration, accentColor, options);
        public static TweenHandle BuffApplied(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).BuffApplied(duration, accentColor).Play();
        public static TweenHandle DebuffApplied(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.DebuffApplied(duration, accentColor, options);
        public static TweenHandle DebuffApplied(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).DebuffApplied(duration, accentColor).Play();
        public static TweenHandle ResourceDepleted(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.ResourceDepleted(duration, accentColor, options);
        public static TweenHandle ResourceDepleted(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).ResourceDepleted(duration, accentColor).Play();
        public static TweenHandle ResourceRecovered(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.ResourceRecovered(duration, accentColor, options);
        public static TweenHandle ResourceRecovered(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).ResourceRecovered(duration, accentColor).Play();
        public static TweenHandle ObjectiveUnlocked(this Component target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.gameObject.ObjectiveUnlocked(duration, accentColor, options);
        public static TweenHandle ObjectiveUnlocked(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default) => target.Tween().WithOptions(options).ObjectiveUnlocked(duration, accentColor).Play();
    }
}
