using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LB.TweenHelper.Demo
{
    [AddComponentMenu("Tween Helper/Samples/Gallery Input Module")]
    [DefaultExecutionOrder(-100)]
    public sealed class AnimationGalleryInputModule : StandaloneInputModule
    {
        protected override void OnEnable()
        {
            base.OnEnable();
#if ENABLE_INPUT_SYSTEM
            Type inputSystemModule = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule == null) return;
            enabled = false;
            if (GetComponent(inputSystemModule) == null) gameObject.AddComponent(inputSystemModule);
#endif
        }
    }
}
