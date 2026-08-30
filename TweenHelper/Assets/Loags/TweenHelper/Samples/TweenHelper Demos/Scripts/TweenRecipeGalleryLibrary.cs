using UnityEngine;

namespace LB.TweenHelper.Demo
{
    public sealed class TweenRecipeGalleryLibrary : ScriptableObject
    {
        [SerializeField] private TweenRecipe panelMoveFade;
        [SerializeField] private TweenRecipe iconScalePreset;
        [SerializeField] private TweenRecipe multiBindingPopup;
        [SerializeField] private TweenRecipe delayedNotification;

        public TweenRecipe Resolve(AnimationGalleryOperation operation)
        {
            switch (operation)
            {
                case AnimationGalleryOperation.RecipePanelMoveFade: return panelMoveFade;
                case AnimationGalleryOperation.RecipeIconScalePreset: return iconScalePreset;
                case AnimationGalleryOperation.RecipeMultiBindingPopup: return multiBindingPopup;
                case AnimationGalleryOperation.RecipeDelayedNotification: return delayedNotification;
                default: return null;
            }
        }
    }
}
