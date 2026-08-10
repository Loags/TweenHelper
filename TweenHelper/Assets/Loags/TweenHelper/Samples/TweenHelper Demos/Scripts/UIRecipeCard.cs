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

        private Action _onSelected;

        public void Configure(UIRecipeKind recipe, string description, Action<UIRecipeKind> onSelected)
        {
            Configure(recipe.ToString(), description, () => onSelected?.Invoke(recipe));
        }

        public void Configure(string recipeName, string description, Action onSelected)
        {
            _onSelected = onSelected;
            nameText.text = recipeName;
            descriptionText.text = description;
            playButton.onClick.RemoveListener(Select);
            playButton.onClick.AddListener(Select);
        }

        private void Select() => _onSelected?.Invoke();
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
