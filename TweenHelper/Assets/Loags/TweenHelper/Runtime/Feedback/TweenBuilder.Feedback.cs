using UnityEngine;

namespace LB.TweenHelper
{
    public partial class TweenBuilder
    {
        /// <summary>Rejects an action with a sharp shake, tilt, and optional red color flash.</summary>
        public TweenBuilder ErrorReject(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateErrorReject(_gameObject, ResolveFeedbackDuration(duration, options, 0.58f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Communicates damage with a hit shake, grounded squash, recoil, and optional red color flash.</summary>
        public TweenBuilder DamageHit(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateDamageHit(_gameObject, ResolveFeedbackDuration(duration, options, 0.5f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Confirms success with a pop, two diminishing bounces, and optional green color flash.</summary>
        public TweenBuilder SuccessConfirm(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateSuccessConfirm(_gameObject, ResolveFeedbackDuration(duration, options, 0.78f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Reveals a reward with anticipation, a relative spin, overshoot, pulse, and optional gold color flash.</summary>
        public TweenBuilder RewardReveal(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateRewardReveal(_gameObject, ResolveFeedbackDuration(duration, options, 1.08f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Punches, arcs, shrinks, and fades to a world-space collection destination.</summary>
        public TweenBuilder PickupCollectTo(Vector3 destination, float? arcHeight = null, float? duration = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreatePickupCollect(_gameObject, destination, arcHeight, ResolveFeedbackDuration(duration, options, 0.92f), options, false), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Punches, arcs, shrinks, and fades to a local destination. RectTransform targets use anchoredPosition3D.</summary>
        public TweenBuilder PickupCollectLocalTo(Vector3 destination, float? arcHeight = null, float? duration = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreatePickupCollect(_gameObject, destination, arcHeight, ResolveFeedbackDuration(duration, options, 0.92f), options, true), applyBuilderOptions: false);
            return this;
        }

        private static float ResolveFeedbackDuration(float? duration, TweenOptions options, float fallback) => duration ?? options.Duration ?? fallback;
    }
}
