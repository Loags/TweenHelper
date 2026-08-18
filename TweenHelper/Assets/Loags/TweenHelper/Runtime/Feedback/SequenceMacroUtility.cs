using System;
using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    internal enum SequenceMacro
    {
        CriticalHit,
        RewardReveal,
        WarningLoop,
        CutsceneUIEntrance
    }

    internal static class SequenceMacroUtility
    {
        public static Tween Create(GameObject target, SequenceMacro macro, Vector3 direction, Color? accentColor, float duration, TweenOptions options)
        {
            ValidateRequest(target, duration);
            var sequence = DOTween.Sequence();
            switch (macro)
            {
                case SequenceMacro.CriticalHit:
                    sequence.Append(FeedbackSequenceUtility.CreateCriticalHit(target, direction, duration * 0.6f, accentColor, default));
                    sequence.AppendInterval(duration * 0.06f);
                    sequence.Append(FeedbackSequenceUtility.CreateSuccessConfirm(target, duration * 0.34f, accentColor, default));
                    break;
                case SequenceMacro.RewardReveal:
                    sequence.Append(FeedbackSequenceUtility.CreateRewardReveal(target, duration * 0.64f, accentColor, default));
                    sequence.Append(FeedbackSequenceUtility.CreateLevelUp(target, duration * 0.36f, accentColor, default));
                    break;
                case SequenceMacro.WarningLoop:
                    sequence.Append(FeedbackSequenceUtility.CreateErrorReject(target, duration * 0.42f, accentColor, default));
                    sequence.AppendInterval(duration * 0.08f);
                    sequence.Append(FeedbackSequenceUtility.CreateLowHealthWarning(target, duration * 0.5f, accentColor, default));
                    break;
                case SequenceMacro.CutsceneUIEntrance:
                    if (!(target.transform is RectTransform)) throw new InvalidOperationException("CutsceneUIEntranceSequence requires a RectTransform target.");
                    sequence.Append(UISequenceUtility.CreateToast(target, true, UISequenceDirection.Up, 60f, duration * 0.58f, default));
                    sequence.Append(FeedbackSequenceUtility.CreateRewardReveal(target, duration * 0.42f, accentColor, default));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(macro), macro, "Unknown sequence macro.");
            }

            sequence.WithDefaults(options, target);
            sequence.Pause();
            return sequence;
        }

        private static void ValidateRequest(GameObject target, float duration)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f) throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be finite and greater than zero.");
        }
    }
}
