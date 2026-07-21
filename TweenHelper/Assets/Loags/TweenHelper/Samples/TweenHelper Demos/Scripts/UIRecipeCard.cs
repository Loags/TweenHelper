using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public class UIRecipeCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button playButton;

        private UIRecipeKind _recipe;
        private Action<UIRecipeKind> _onSelected;

        public void Configure(UIRecipeKind recipe, string description, Action<UIRecipeKind> onSelected)
        {
            _recipe = recipe;
            _onSelected = onSelected;
            nameText.text = recipe.ToString();
            descriptionText.text = description;
            playButton.onClick.RemoveListener(Select);
            playButton.onClick.AddListener(Select);
        }

        private void Select() => _onSelected?.Invoke(_recipe);
    }

    public enum UIRecipeKind
    {
        UIAppear,
        UIAppearSoft,
        UIDisappear,
        UIDisappearSoft,
        UIHover,
        UIHoverSoft,
        UIPress,
        UIPressHard,
        UIAttention,
        UIAttentionSoft,
        UIAttentionHard,
        UIDisabled,
        UIEnabled
    }
}
