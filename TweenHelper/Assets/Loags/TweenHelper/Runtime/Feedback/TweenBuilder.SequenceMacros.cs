using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        public TweenBuilder CriticalHitSequence(Vector3 impactDirection, float? duration = null, Color? accentColor = null)
            => AddSequenceMacro(SequenceMacro.CriticalHit, impactDirection, duration, 1.05f, accentColor);

        public TweenBuilder RewardRevealSequence(float? duration = null, Color? accentColor = null)
            => AddSequenceMacro(SequenceMacro.RewardReveal, Vector3.right, duration, 1.5f, accentColor);

        /// <summary>Plays one finite warning cycle suitable for root-level looping.</summary>
        public TweenBuilder WarningLoopSequence(float? duration = null, Color? accentColor = null)
            => AddSequenceMacro(SequenceMacro.WarningLoop, Vector3.right, duration, 1.35f, accentColor);

        public TweenBuilder CutsceneUIEntranceSequence(float? duration = null, Color? accentColor = null)
            => AddSequenceMacro(SequenceMacro.CutsceneUIEntrance, Vector3.right, duration, 1.4f, accentColor);

        private TweenBuilder AddSequenceMacro(SequenceMacro macro, Vector3 direction, float? duration, float fallback, Color? accentColor)
        {
            AddStep(options => SequenceMacroUtility.Create(_gameObject, macro, direction, accentColor, ResolveFeedbackDuration(duration, options, fallback), options), applyBuilderOptions: false);
            return this;
        }
    }
}
