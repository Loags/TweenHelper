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

        /// <summary>Communicates healing with a gentle lift, restorative stretch, settle pulse, and optional green flash.</summary>
        public TweenBuilder HealReceive(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateHealReceive(_gameObject, ResolveFeedbackDuration(duration, options, 0.82f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Communicates a directional block with compression, recoil, tilt, and an optional shield flash.</summary>
        public TweenBuilder ShieldBlock(Vector3 impactDirection, float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateShieldBlock(_gameObject, impactDirection, ResolveFeedbackDuration(duration, options, 0.52f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Communicates a critical directional impact with a sharp flash, heavy squash, recoil, and aftershock.</summary>
        public TweenBuilder CriticalHit(Vector3 impactDirection, float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateCriticalHit(_gameObject, impactDirection, ResolveFeedbackDuration(duration, options, 0.62f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Announces that a cooldown is ready with a flip, pop, settle, and optional blue flash.</summary>
        public TweenBuilder CooldownReady(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateCooldownReady(_gameObject, ResolveFeedbackDuration(duration, options, 0.78f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Celebrates a level increase with lift, spin, staged pulses, and an optional gold flash.</summary>
        public TweenBuilder LevelUp(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateLevelUp(_gameObject, ResolveFeedbackDuration(duration, options, 1.15f), flashColor, options), applyBuilderOptions: false);
            return this;
        }

        /// <summary>Plays one finite low-health warning cycle that callers may loop at the root.</summary>
        public TweenBuilder LowHealthWarning(float? duration = null, Color? flashColor = null)
        {
            AddStep(options => FeedbackSequenceUtility.CreateLowHealthWarning(_gameObject, ResolveFeedbackDuration(duration, options, 0.86f), flashColor, options), applyBuilderOptions: false);
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

        /// <summary>Projects a world pickup into UI space, arcs it to a RectTransform anchor, then shrinks and fades it.</summary>
        public TweenBuilder PickupCollectToUI(Vector3 worldSource, RectTransform uiTarget, float? arcHeight = null, float? duration = null, Camera worldCamera = null, bool lockDestination = true)
        {
            AddStep(options => UIWorldProjectionUtility.CreatePickup(_gameObject, worldSource, uiTarget, arcHeight, ResolveFeedbackDuration(duration, options, 0.92f), worldCamera, lockDestination, options), applyBuilderOptions: false);
            return this;
        }

        private static float ResolveFeedbackDuration(float? duration, TweenOptions options, float fallback) => duration ?? options.Duration ?? fallback;
    }
}
