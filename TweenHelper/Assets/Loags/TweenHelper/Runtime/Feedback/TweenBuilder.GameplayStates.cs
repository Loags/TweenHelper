using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        public TweenBuilder AbilityCharging(float? duration = null, Color? accentColor = null)
            => AddGameplayState(GameplayStateAnimation.AbilityCharging, duration, 0.9f, accentColor);

        public TweenBuilder AbilityReady(float? duration = null, Color? accentColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateCooldownReady(_gameObject, ResolveFeedbackDuration(duration, options, 0.78f), accentColor, options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder DodgeRoll(float? duration = null)
            => AddGameplayState(GameplayStateAnimation.DodgeRoll, duration, 0.62f, null);

        public TweenBuilder StunStart(float? duration = null, Color? accentColor = null)
            => AddGameplayState(GameplayStateAnimation.StunStart, duration, 0.72f, accentColor);

        public TweenBuilder StunEnd(float? duration = null, Color? accentColor = null)
            => AddGameplayState(GameplayStateAnimation.StunEnd, duration, 0.58f, accentColor);

        public TweenBuilder BuffApplied(float? duration = null, Color? accentColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateHealReceive(_gameObject, ResolveFeedbackDuration(duration, options, 0.82f), accentColor ?? new Color(0.22f, 1f, 0.45f, 1f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder DebuffApplied(float? duration = null, Color? accentColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateDamageHit(_gameObject, ResolveFeedbackDuration(duration, options, 0.58f), accentColor ?? new Color(0.68f, 0.24f, 0.92f, 1f), options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder ResourceDepleted(float? duration = null, Color? accentColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateLowHealthWarning(_gameObject, ResolveFeedbackDuration(duration, options, 0.86f), accentColor, options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder ResourceRecovered(float? duration = null, Color? accentColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateHealReceive(_gameObject, ResolveFeedbackDuration(duration, options, 0.74f), accentColor, options), applyBuilderOptions: false);
            return this;
        }

        public TweenBuilder ObjectiveUnlocked(float? duration = null, Color? accentColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateRewardReveal(_gameObject, ResolveFeedbackDuration(duration, options, 1.08f), accentColor, options), applyBuilderOptions: false);
            return this;
        }

        private TweenBuilder AddGameplayState(GameplayStateAnimation animation, float? duration, float fallback, Color? accentColor)
        {
            AddStep(options => GameplayStateFeedbackUtility.Create(_gameObject, animation, ResolveFeedbackDuration(duration, options, fallback), accentColor, options), applyBuilderOptions: false);
            return this;
        }
    }
}
