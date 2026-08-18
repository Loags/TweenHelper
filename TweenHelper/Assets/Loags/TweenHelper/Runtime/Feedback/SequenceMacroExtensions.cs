using UnityEngine;

namespace LB.TweenHelper
{
    public static class SequenceMacroExtensions
    {
        public static TweenHandle CriticalHitSequence(this GameObject target, Vector3 impactDirection, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CriticalHitSequence(impactDirection, duration, accentColor).Play();

        public static TweenHandle RewardRevealSequence(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).RewardRevealSequence(duration, accentColor).Play();

        public static TweenHandle WarningLoopSequence(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).WarningLoopSequence(duration, accentColor).Play();

        public static TweenHandle CutsceneUIEntranceSequence(this GameObject target, float? duration = null, Color? accentColor = null, TweenOptions options = default)
            => target.Tween().WithOptions(options).CutsceneUIEntranceSequence(duration, accentColor).Play();
    }
}
