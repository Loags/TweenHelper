using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LB.TweenHelper
{
    public enum TweenRecipeBindingKind
    {
        GameObject,
        Collection
    }

    public enum TweenRecipePlacement
    {
        Then,
        With
    }

    public enum TweenRecipeOperation
    {
        Delay,
        RegisteredPreset,
        MoveLocalTo,
        MoveLocalBy,
        RotateLocalTo,
        RotateLocalBy,
        ScaleTo,
        FadeTo,
        ColorTo,
        MoveWorldTo,
        MoveWorldBy,
        RotateWorldTo,
        RotateWorldBy,
        ArcWorldTo,
        HopWorldTo,
        ErrorReject,
        DamageHit,
        SuccessConfirm,
        RewardReveal,
        PageCrossFadeTo,
        TypewriterReveal,
        NumberCountTo,
        CollectionPreset,
        CameraFieldOfViewTo,
        LightIntensityTo,
        AudioVolumeTo,
        ParticleEmissionRateTo,
        ProgressFillTo
    }

    [Flags]
    public enum TweenRecipeParameterFields
    {
        None = 0,
        Vector3 = 1 << 0,
        Color = 1 << 1,
        Float = 1 << 2,
        Integer = 1 << 3,
        Boolean = 1 << 4,
        String = 1 << 5
    }

    [Serializable]
    public sealed class TweenRecipeBindingDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private TweenRecipeBindingKind kind;

        public string Id => id;
        public string DisplayName => displayName;
        public TweenRecipeBindingKind Kind => kind;
    }

    [Serializable]
    public sealed class TweenRecipeParameters
    {
        [SerializeField] private Vector3 vector3Value;
        [SerializeField] private Color colorValue = Color.white;
        [SerializeField] private float floatValue;
        [SerializeField] private int intValue;
        [SerializeField] private bool boolValue;
        [SerializeField] private string stringValue;

        public Vector3 Vector3Value => vector3Value;
        public Color ColorValue => colorValue;
        public float FloatValue => floatValue;
        public int IntValue => intValue;
        public bool BoolValue => boolValue;
        public string StringValue => stringValue;
    }

    [Serializable]
    public sealed class TweenRecipeNode
    {
        [SerializeField] private string id;
        [SerializeField] private string label;
        [SerializeField] private TweenRecipePlacement placement;
        [SerializeField] private TweenRecipeOperation operation;
        [SerializeField] private string bindingId;
        [SerializeField] private string secondaryBindingId;
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private float delay;
        [SerializeField] private Ease ease = Ease.OutCubic;
        [SerializeField] private TweenRecipeParameters parameters = new TweenRecipeParameters();

        public string Id => id;
        public string Label => label;
        public TweenRecipePlacement Placement => placement;
        public TweenRecipeOperation Operation => operation;
        public string BindingId => bindingId;
        public string SecondaryBindingId => secondaryBindingId;
        public float Duration => duration;
        public float Delay => delay;
        public Ease Ease => ease;
        public TweenRecipeParameters Parameters => parameters;
    }

    [CreateAssetMenu(fileName = "TweenRecipe", menuName = "Tween Helper/Tween Recipe")]
    public sealed class TweenRecipe : ScriptableObject
    {
        [SerializeField] private List<TweenRecipeBindingDefinition> bindings = new List<TweenRecipeBindingDefinition>();
        [SerializeField] private List<TweenRecipeNode> nodes = new List<TweenRecipeNode>();

        public IReadOnlyList<TweenRecipeBindingDefinition> Bindings => bindings;
        public IReadOnlyList<TweenRecipeNode> Nodes => nodes;
    }
}
