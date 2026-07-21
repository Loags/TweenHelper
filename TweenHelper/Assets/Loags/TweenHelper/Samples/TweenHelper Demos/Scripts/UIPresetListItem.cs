using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Demo
{
    public class UIPresetListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text presetNameText;
        [SerializeField] private TMP_Text familyText;
        [SerializeField] private Button selectButton;

        private ITweenPreset _preset;
        private Action<ITweenPreset> _onSelected;

        public ITweenPreset Preset => _preset;

        public void Configure(ITweenPreset preset, Action<ITweenPreset> onSelected)
        {
            _preset = preset;
            _onSelected = onSelected;
            presetNameText.text = preset.PresetName;
            familyText.text = PresetFamilyClassifier.GetFamilyName(preset.PresetName);
            selectButton.onClick.RemoveListener(Select);
            selectButton.onClick.AddListener(Select);
        }

        private void Select() => _onSelected?.Invoke(_preset);
    }
}
