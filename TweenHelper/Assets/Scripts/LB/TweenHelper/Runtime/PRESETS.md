# How to use Presets

Direct call (auto-plays):

    transform.PopIn();
    transform.Shake(0.3f);
    gameObject.FadeOut();

Builder chain:

    transform.Tween().PopIn().Play();
    transform.Tween().PopIn().Then().FadeOut().Play();

With options:

    transform.PopIn(0.5f, TweenOptions.WithEase(Ease.OutBounce));


# Creating Custom Presets

## Create a Preset

BuiltInPresets.cs:

    [AutoRegisterPreset]
    public class MyPreset : CodePreset
    {
        public override string PresetName => "MyPreset";
        public override string Description => "My custom animation";
        public override float DefaultDuration => 0.5f;

        public override Tween CreateTween(GameObject target, float? duration = null, TweenOptions options = default)
        {
            return target.transform.DOScale(1.2f, GetDuration(duration))
                .SetEase(Ease.OutBack)
                .WithDefaults(MergeWithDefaultEase(options, Ease.OutBack), target);
        }
    }

## Add Direct Methods

PresetExtensions.cs:

    public static TweenHandle MyPreset(this Transform t, float? duration = null, TweenOptions options = default)
        => new TweenBuilder(t).Preset("MyPreset", duration).WithOptions(options).Play();

TweenBuilder.cs:

    public TweenBuilder MyPreset(float? duration = null) => Preset("MyPreset", duration);

## Helper Methods

    GetDuration(duration)                         // Duration with fallback
    MergeWithDefaultEase(options, Ease.OutBack)   // Merge ease options
    CreateFadeTween(target, alpha, duration)      // Auto-detect fade component
    SetAlpha(target, 0f)                          // Set alpha directly
    CanFade(target)                               // Check fade support
