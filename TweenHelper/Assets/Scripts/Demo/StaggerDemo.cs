using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using System.Linq;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates staggered animations across multiple objects using TweenStagger.
    /// Shows coordinated animations with delays between each object's start time.
    /// </summary>
    public class StaggerDemo : MonoBehaviour
    {
        [Header("Stagger Controls")]
        [SerializeField] private Button staggerMoveButton;
        [SerializeField] private Button staggerScaleButton;
        [SerializeField] private Button staggerPresetButton;
        [SerializeField] private Button staggerFadeButton;
        [SerializeField] private Button waveAnimationButton;
        [SerializeField] private Button cascadeButton;
        
        [Header("Stagger Settings")]
        [SerializeField] private float staggerDelay = 0.2f;
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private Vector3 moveOffset = new Vector3(0f, 2f, 0f);
        [SerializeField] private float scaleMultiplier = 1.5f;
        
        private GameObject[] demoObjects;
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            SetupButton(staggerMoveButton, "Stagger Move", DemoStaggerMove);
            SetupButton(staggerScaleButton, "Stagger Scale", DemoStaggerScale);
            SetupButton(staggerPresetButton, "Stagger Preset", DemoStaggerPreset);
            SetupButton(staggerFadeButton, "Stagger Fade", DemoStaggerFade);
            SetupButton(waveAnimationButton, "Wave Animation", DemoWaveAnimation);
            SetupButton(cascadeButton, "Cascade Effect", DemoCascadeEffect);
        }
        
        private void SetupButton(Button button, string text, System.Action action)
        {
            if (button == null) return;
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
            
            var buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }
        
        public void DemoStaggerMove()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Stagger move demo");
            
            var transforms = demoObjects.Where(obj => obj != null).Select(obj => obj.transform).ToArray();
            var targetPositions = transforms.Select(t => t.position + moveOffset);
            
            // Move all objects to offset positions with stagger
            var sequence = TweenHelper.StaggerMoveTo(transforms, transforms[0].position + moveOffset, 
                staggerDelay, animationDuration);
            
            // Return to original positions
            if (sequence != null)
            {
                sequence.OnComplete(() =>
                {
                    var originalPositions = transforms.Select(t => t.position - moveOffset);
                    TweenHelper.StaggerMoveTo(transforms, transforms[0].position - moveOffset, 
                        staggerDelay * 0.5f, animationDuration * 0.5f);
                });
            }
        }
        
        public void DemoStaggerScale()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Stagger scale demo");
            
            var transforms = demoObjects.Where(obj => obj != null).Select(obj => obj.transform).ToArray();
            var originalScales = transforms.Select(t => t.localScale).ToArray();
            var targetScale = Vector3.one * scaleMultiplier;
            
            // Scale up with stagger
            var sequence = TweenHelper.StaggerScaleTo(transforms, targetScale, 
                staggerDelay, animationDuration);
            
            // Scale back down
            if (sequence != null)
            {
                sequence.OnComplete(() =>
                {
                    for (int i = 0; i < transforms.Length; i++)
                    {
                        if (i < originalScales.Length)
                        {
                            TweenHelper.ScaleTo(transforms[i], originalScales[i], animationDuration * 0.5f);
                        }
                    }
                });
            }
        }
        
        public void DemoStaggerPreset()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Stagger preset demo");
            
            // Prepare all objects for PopIn
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    obj.transform.localScale = Vector3.zero;
                }
            }
            
            // Play PopIn preset on all objects with stagger
            TweenHelper.StaggerPreset("PopIn", demoObjects, staggerDelay, animationDuration);
        }
        
        public void DemoStaggerFade()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Stagger fade demo");
            
            // Collect all fadeable components
            var fadeableComponents = new System.Collections.Generic.List<Component>();
            
            foreach (var obj in demoObjects)
            {
                if (obj == null) continue;
                
                var canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    fadeableComponents.Add(canvasGroup);
                    continue;
                }
                
                var image = obj.GetComponent<Image>();
                if (image != null)
                {
                    fadeableComponents.Add(image);
                    continue;
                }
                
                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    fadeableComponents.Add(spriteRenderer);
                }
            }
            
            if (fadeableComponents.Count == 0)
            {
                Debug.LogWarning("No fadeable components found");
                return;
            }
            
            // Fade out with stagger
            var sequence = TweenHelper.StaggerFadeTo(fadeableComponents, 0f, staggerDelay, animationDuration);
            
            // Fade back in
            if (sequence != null)
            {
                sequence.OnComplete(() =>
                {
                    TweenHelper.StaggerFadeTo(fadeableComponents, 1f, staggerDelay * 0.5f, animationDuration * 0.5f);
                });
            }
        }
        
        public void DemoWaveAnimation()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Wave animation demo");
            
            // Create a wave effect by varying the move offset based on object index
            for (int i = 0; i < demoObjects.Length; i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                var originalPos = obj.transform.position;
                var waveHeight = Mathf.Sin(i * 0.5f) * 2f; // Vary height in wave pattern
                var targetPos = originalPos + new Vector3(0f, waveHeight, 0f);
                
                // Stagger the start time
                float delay = i * staggerDelay;
                
                StartCoroutine(AnimateWithDelay(obj.transform, targetPos, originalPos, delay));
            }
        }
        
        private System.Collections.IEnumerator AnimateWithDelay(Transform target, Vector3 targetPos, Vector3 originalPos, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            var tween = TweenHelper.MoveTo(target, targetPos, animationDuration);
            if (tween != null)
            {
                tween.OnComplete(() =>
                {
                    TweenHelper.MoveTo(target, originalPos, animationDuration * 0.5f);
                });
            }
        }
        
        public void DemoCascadeEffect()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Cascade effect demo");
            
            // Create a cascading effect with multiple animation types
            for (int i = 0; i < demoObjects.Length; i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                float delay = i * staggerDelay;
                
                StartCoroutine(CascadeAnimation(obj, delay, i));
            }
        }
        
        private System.Collections.IEnumerator CascadeAnimation(GameObject obj, float delay, int index)
        {
            yield return new WaitForSeconds(delay);
            
            var originalPos = obj.transform.position;
            var originalScale = obj.transform.localScale;
            var originalRotation = obj.transform.eulerAngles;
            
            // Different animation based on index
            switch (index % 3)
            {
                case 0:
                    // Scale and rotate
                    var scaleTween = TweenHelper.ScaleTo(obj.transform, originalScale * 1.5f, animationDuration * 0.5f);
                    TweenHelper.RotateBy(obj.transform, Vector3.up * 180f, animationDuration);
                    scaleTween.OnComplete(() =>
                    {
                        TweenHelper.ScaleTo(obj.transform, originalScale, animationDuration * 0.5f);
                    });
                    break;
                    
                case 1:
                    // Move in arc
                    var midPoint = originalPos + Vector3.up * 3f + Vector3.right * 2f;
                    var moveTween1 = TweenHelper.MoveTo(obj.transform, midPoint, animationDuration * 0.5f);
                    moveTween1.OnComplete(() =>
                    {
                        TweenHelper.MoveTo(obj.transform, originalPos, animationDuration * 0.5f);
                    });
                    break;
                    
                case 2:
                    // Preset animation
                    if (TweenHelper.HasPreset("Bounce"))
                    {
                        TweenHelper.PlayPreset("Bounce", obj, animationDuration);
                    }
                    break;
            }
        }
        
        public void DemoRadialStagger()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Radial stagger demo");
            
            Vector3 center = Vector3.zero;
            if (demoObjects.Length > 0 && demoObjects[0] != null)
            {
                // Use first object position as center
                center = demoObjects[0].transform.position;
            }
            
            for (int i = 0; i < demoObjects.Length; i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                // Calculate radial position
                float angle = (360f / demoObjects.Length) * i * Mathf.Deg2Rad;
                var radialPos = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 4f;
                
                // Stagger based on distance from center
                float distance = Vector3.Distance(obj.transform.position, center);
                float delay = distance * 0.1f;
                
                StartCoroutine(MoveToWithDelay(obj.transform, radialPos, delay));
            }
        }
        
        private System.Collections.IEnumerator MoveToWithDelay(Transform target, Vector3 targetPos, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            var originalPos = target.position;
            var tween = TweenHelper.MoveTo(target, targetPos, animationDuration);
            
            if (tween != null)
            {
                tween.OnComplete(() =>
                {
                    TweenHelper.MoveTo(target, originalPos, animationDuration * 0.5f);
                });
            }
        }
        
        public void DemoCustomStagger()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Custom stagger demo");
            
            // Use TweenStagger.StaggerCustom for complex custom animations
            var transforms = demoObjects.Where(obj => obj != null).Select(obj => obj.transform);
            
            TweenStagger.StaggerCustom(transforms, (transform, index) =>
            {
                var originalPos = transform.position;
                var originalScale = transform.localScale;
                
                // Create different animation based on index
                var sequence = TweenHelper.CreateSequence(transform.gameObject)
                    .Move(transform, originalPos + Vector3.up * (index + 1), animationDuration * 0.5f)
                    .JoinScale(transform, originalScale * (1f + index * 0.2f), animationDuration * 0.5f)
                    .Delay(0.2f)
                    .Move(transform, originalPos, animationDuration * 0.5f)
                    .JoinScale(transform, originalScale, animationDuration * 0.5f)
                    .Build();
                    
                return sequence;
                
            }, staggerDelay);
        }
        
        private void Update()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) DemoStaggerMove();
            if (Input.GetKeyDown(KeyCode.Alpha2)) DemoStaggerScale();
            if (Input.GetKeyDown(KeyCode.Alpha3)) DemoStaggerPreset();
            if (Input.GetKeyDown(KeyCode.Alpha4)) DemoStaggerFade();
            if (Input.GetKeyDown(KeyCode.W)) DemoWaveAnimation();
            if (Input.GetKeyDown(KeyCode.C)) DemoCascadeEffect();
            if (Input.GetKeyDown(KeyCode.R)) DemoRadialStagger();
            if (Input.GetKeyDown(KeyCode.X)) DemoCustomStagger();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 180));
            GUILayout.Label("Stagger Demo Keys:", GUI.skin.box);
            GUILayout.Label("1 : Stagger move");
            GUILayout.Label("2 : Stagger scale");
            GUILayout.Label("3 : Stagger preset");
            GUILayout.Label("4 : Stagger fade");
            GUILayout.Label("W : Wave animation");
            GUILayout.Label("C : Cascade effect");
            GUILayout.Label("R : Radial stagger");
            GUILayout.Label("X : Custom stagger");
            GUILayout.EndArea();
        }
    }
}
