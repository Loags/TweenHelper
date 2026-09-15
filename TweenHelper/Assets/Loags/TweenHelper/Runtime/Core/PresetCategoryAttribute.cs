using System;

namespace LB.TweenHelper
{
    [Flags]
    public enum PresetOverrideFields
    {
        None = 0,
        Ease = 1,
        Strength = 2,
        Overshoot = 4,
        StartScale = 8,
        TargetScale = 16,
        StartAlpha = 32,
        TargetAlpha = 64
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class PresetOverridesAttribute : Attribute
    {
        public PresetOverrideFields Fields { get; }

        public PresetOverridesAttribute(PresetOverrideFields fields) => Fields = fields;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class PresetCategoryAttribute : Attribute
    {
        public string Category { get; }

        public PresetCategoryAttribute(string category) => Category = category;
    }

    /// <summary>Opt in only when preview changes stay within the target hierarchy's supported visual properties.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PresetPreviewSupportedAttribute : Attribute
    {
    }
}
