using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public sealed class AnimationGalleryOptionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private Dropdown dropdown;

        private Action<int> _onChanged;

        public void Configure(AnimationGalleryOptionDescriptor descriptor, int selectedIndex, Action<int> onChanged)
        {
            gameObject.SetActive(true);
            labelText.text = descriptor.Label;
            dropdown.onValueChanged.RemoveListener(Changed);
            dropdown.ClearOptions();
            dropdown.AddOptions(new System.Collections.Generic.List<string>(descriptor.Values));
            dropdown.SetValueWithoutNotify(selectedIndex);
            _onChanged = onChanged;
            dropdown.onValueChanged.AddListener(Changed);
        }

        public void Hide()
        {
            dropdown.onValueChanged.RemoveListener(Changed);
            _onChanged = null;
            gameObject.SetActive(false);
        }

        private void Changed(int index) => _onChanged?.Invoke(index);
    }
}
