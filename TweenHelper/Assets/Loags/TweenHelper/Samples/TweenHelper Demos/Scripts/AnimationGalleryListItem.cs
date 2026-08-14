using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public sealed class AnimationGalleryListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text secondaryText;
        [SerializeField] private Image selectedIndicator;
        [SerializeField] private Button button;

        private Action _onSelected;

        public void Configure(AnimationGalleryEntry entry, Action onSelected)
        {
            nameText.text = entry.Name;
            secondaryText.text = entry.Category == AnimationGalleryCategory.Presets ? entry.Family : entry.ApiKind.ToString();
            _onSelected = onSelected;
            button.onClick.RemoveListener(Select);
            button.onClick.AddListener(Select);
        }

        public void SetSelected(bool selected) => selectedIndicator.enabled = selected;

        private void Select() => _onSelected?.Invoke();
    }
}
