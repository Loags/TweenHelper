using System;
using UnityEngine;

namespace LB.TweenHelper
{
    public enum TweenMotionPreference
    {
        UseProjectDefault,
        Full,
        Reduced
    }

    /// <summary>Controls decorative motion in newly created Tween Helper animations.</summary>
    public static class TweenMotion
    {
        private static TweenMotionPreference _preference;

        public static TweenMotionPreference Preference
        {
            get => _preference;
            set
            {
                Validate(value);
                _preference = value;
            }
        }

        internal static TweenMotionPreference Resolve(TweenMotionPreference preference)
        {
            if (preference != TweenMotionPreference.UseProjectDefault) return preference;
            return _preference == TweenMotionPreference.UseProjectDefault ? TweenHelperSettings.Instance.MotionPreference : _preference;
        }

        internal static void Validate(TweenMotionPreference preference)
        {
            if (preference < TweenMotionPreference.UseProjectDefault || preference > TweenMotionPreference.Reduced)
                throw new ArgumentOutOfRangeException(nameof(preference));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => _preference = TweenMotionPreference.UseProjectDefault;
    }
}
