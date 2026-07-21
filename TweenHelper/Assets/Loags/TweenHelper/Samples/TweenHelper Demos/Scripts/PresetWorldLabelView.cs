using TMPro;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    public class PresetWorldLabelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text presetNameText;
        [SerializeField] private TMP_Text descriptionText;

        public void SetContent(string presetName, string description)
        {
            presetNameText.text = presetName;
            descriptionText.text = description;
        }
    }
}
