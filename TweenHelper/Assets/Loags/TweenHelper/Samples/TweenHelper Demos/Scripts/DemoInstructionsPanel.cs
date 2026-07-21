using TMPro;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    public class DemoInstructionsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text instructionsText;

        public void SetContent(string title, string instructions)
        {
            titleText.text = title;
            instructionsText.text = instructions;
        }
    }
}
