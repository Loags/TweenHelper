using UnityEngine;

namespace LB.TweenHelper.Demo
{
    public sealed class AnimationGalleryPreviewRouter : MonoBehaviour
    {
        [SerializeField] private GameObject uiTarget;
        [SerializeField] private GameObject worldTargetRoot;
        [SerializeField] private GameObject worldTarget;
        [SerializeField] private GameObject listRoot;
        [SerializeField] private GameObject gridRoot;
        [SerializeField] private GameObject loadingDotsRoot;
        [SerializeField] private GameObject destinationUiRoot;
        [SerializeField] private GameObject destinationWorldRoot;
        [SerializeField] private GameObject uiSequenceRoot;
        [SerializeField] private GameObject textValueRoot;
        [SerializeField] private GameObject worldTextValueRoot;
        [SerializeField] private GameObject cameraRoot;

        public GameObject UiTarget => uiTarget;
        public GameObject WorldTarget => worldTarget;

        public void Show(AnimationGalleryConfiguration configuration)
        {
            HideAll();
            AnimationGalleryEntry entry = configuration.Entry;
            switch (entry.Fixture)
            {
                case AnimationGalleryFixture.PresetAuto:
                    GameObject target = ResolvePresetTarget(entry);
                    (target == uiTarget ? uiTarget : worldTargetRoot).SetActive(true);
                    break;
                case AnimationGalleryFixture.UiTarget:
                    uiTarget.SetActive(true);
                    break;
                case AnimationGalleryFixture.List:
                    listRoot.SetActive(true);
                    break;
                case AnimationGalleryFixture.Grid:
                    gridRoot.SetActive(true);
                    break;
                case AnimationGalleryFixture.LoadingDots:
                    loadingDotsRoot.SetActive(true);
                    break;
                case AnimationGalleryFixture.Destination:
                case AnimationGalleryFixture.Feedback:
                    bool useWorld = configuration.GetValue(AnimationGalleryOptionKind.TargetContext) == "World";
                    (useWorld ? destinationWorldRoot : destinationUiRoot).SetActive(true);
                    break;
                case AnimationGalleryFixture.UISequence:
                    uiSequenceRoot.SetActive(true);
                    break;
                case AnimationGalleryFixture.TextValue:
                    bool worldText = configuration.GetValue(AnimationGalleryOptionKind.TargetContext) == "World";
                    (worldText ? worldTextValueRoot : textValueRoot).SetActive(true);
                    break;
                case AnimationGalleryFixture.WorldTextValue:
                    worldTextValueRoot.SetActive(true);
                    break;
                case AnimationGalleryFixture.Camera:
                    cameraRoot.SetActive(true);
                    break;
            }
        }

        public void HideAll()
        {
            uiTarget.SetActive(false);
            worldTargetRoot.SetActive(false);
            listRoot.SetActive(false);
            gridRoot.SetActive(false);
            loadingDotsRoot.SetActive(false);
            destinationUiRoot.SetActive(false);
            destinationWorldRoot.SetActive(false);
            uiSequenceRoot.SetActive(false);
            textValueRoot.SetActive(false);
            worldTextValueRoot.SetActive(false);
            cameraRoot.SetActive(false);
        }

        public GameObject ResolvePresetTarget(AnimationGalleryEntry entry)
        {
            if (entry.Preset.CanApplyTo(uiTarget)) return uiTarget;
            return worldTarget;
        }
    }
}
