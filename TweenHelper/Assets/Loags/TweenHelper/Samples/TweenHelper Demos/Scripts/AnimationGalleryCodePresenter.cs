using System.Collections;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    public sealed class AnimationGalleryCodePresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text codeText;
        [SerializeField] private TMP_Text copyStateText;
        [SerializeField] private float copyStateDuration = 1.2f;

        private Coroutine _copyStateRoutine;

        public void Show(AnimationGalleryConfiguration configuration)
        {
            codeText.text = AnimationGalleryCatalog.GetSnippet(configuration);
            copyStateText.gameObject.SetActive(false);
        }

        public void Copy()
        {
            GUIUtility.systemCopyBuffer = codeText.text;
            if (_copyStateRoutine != null) StopCoroutine(_copyStateRoutine);
            _copyStateRoutine = StartCoroutine(ShowCopyState());
        }

        private IEnumerator ShowCopyState()
        {
            copyStateText.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(copyStateDuration);
            copyStateText.gameObject.SetActive(false);
            _copyStateRoutine = null;
        }
    }
}
