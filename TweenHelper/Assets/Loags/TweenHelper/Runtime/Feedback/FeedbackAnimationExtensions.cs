using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>One-line gameplay feedback sequences for world and UI targets.</summary>
    public static class FeedbackAnimationExtensions
    {
        public static TweenHandle ErrorReject(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => ErrorReject(component.gameObject, duration, flashColor, options);

        public static TweenHandle ErrorReject(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).ErrorReject(duration, flashColor).Play();

        public static TweenHandle DamageHit(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => DamageHit(component.gameObject, duration, flashColor, options);

        public static TweenHandle DamageHit(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).DamageHit(duration, flashColor).Play();

        public static TweenHandle SuccessConfirm(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => SuccessConfirm(component.gameObject, duration, flashColor, options);

        public static TweenHandle SuccessConfirm(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).SuccessConfirm(duration, flashColor).Play();

        public static TweenHandle RewardReveal(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => RewardReveal(component.gameObject, duration, flashColor, options);

        public static TweenHandle RewardReveal(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).RewardReveal(duration, flashColor).Play();

        public static TweenHandle HealReceive(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => HealReceive(component.gameObject, duration, flashColor, options);

        public static TweenHandle HealReceive(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).HealReceive(duration, flashColor).Play();

        public static TweenHandle ShieldBlock(this Component component, Vector3 impactDirection, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => ShieldBlock(component.gameObject, impactDirection, duration, flashColor, options);

        public static TweenHandle ShieldBlock(this GameObject target, Vector3 impactDirection, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).ShieldBlock(impactDirection, duration, flashColor).Play();

        public static TweenHandle CriticalHit(this Component component, Vector3 impactDirection, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => CriticalHit(component.gameObject, impactDirection, duration, flashColor, options);

        public static TweenHandle CriticalHit(this GameObject target, Vector3 impactDirection, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CriticalHit(impactDirection, duration, flashColor).Play();

        public static TweenHandle CooldownReady(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => CooldownReady(component.gameObject, duration, flashColor, options);

        public static TweenHandle CooldownReady(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CooldownReady(duration, flashColor).Play();

        public static TweenHandle LevelUp(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => LevelUp(component.gameObject, duration, flashColor, options);

        public static TweenHandle LevelUp(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).LevelUp(duration, flashColor).Play();

        public static TweenHandle LowHealthWarning(this Component component, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => LowHealthWarning(component.gameObject, duration, flashColor, options);

        public static TweenHandle LowHealthWarning(this GameObject target, float? duration = null, Color? flashColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).LowHealthWarning(duration, flashColor).Play();

        public static TweenHandle PickupCollectTo(this Component component, Vector3 destination, float? arcHeight = null, float? duration = null, TweenOptions options = default)
            => PickupCollectTo(component.gameObject, destination, arcHeight, duration, options);

        public static TweenHandle PickupCollectTo(this GameObject target, Vector3 destination, float? arcHeight = null, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).PickupCollectTo(destination, arcHeight, duration).Play();

        public static TweenHandle PickupCollectLocalTo(this Component component, Vector3 destination, float? arcHeight = null, float? duration = null, TweenOptions options = default)
            => PickupCollectLocalTo(component.gameObject, destination, arcHeight, duration, options);

        public static TweenHandle PickupCollectLocalTo(this GameObject target, Vector3 destination, float? arcHeight = null, float? duration = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).PickupCollectLocalTo(destination, arcHeight, duration).Play();
    }
}
