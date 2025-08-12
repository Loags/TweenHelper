using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates sequence composition with TweenSequenceBuilder.
    /// Shows step-by-step animations, parallel compositions, and complex multi-step flows.
    /// </summary>
    public class SequenceDemo : MonoBehaviour
    {
        [Header("Sequence Controls")]
        [SerializeField] private Button simpleSequenceButton;
        [SerializeField] private Button parallelSequenceButton;
        [SerializeField] private Button complexSequenceButton;
        [SerializeField] private Button loopSequenceButton;
        [SerializeField] private Button callbackSequenceButton;
        [SerializeField] private Button presetSequenceButton;
        
        [Header("Sequence Settings")]
        [SerializeField] private float stepDuration = 0.8f;
        [SerializeField] private float delayBetweenSteps = 0.3f;
        
        private GameObject[] demoObjects;
        private int currentObjectIndex = 0;
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            SetupButton(simpleSequenceButton, "Simple Sequence", DemoSimpleSequence);
            SetupButton(parallelSequenceButton, "Parallel Actions", DemoParallelSequence);
            SetupButton(complexSequenceButton, "Complex Flow", DemoComplexSequence);
            SetupButton(loopSequenceButton, "Looped Sequence", DemoLoopSequence);
            SetupButton(callbackSequenceButton, "With Callbacks", DemoCallbackSequence);
            SetupButton(presetSequenceButton, "Preset Sequence", DemoPresetSequence);
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
        
        public void DemoSimpleSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Simple sequence on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            Vector3 originalScale = target.transform.localScale;
            
            // Simple step-by-step sequence
            TweenHelper.CreateSequence(target)
                .Move(target.transform, originalPos + Vector3.up * 2f, stepDuration)
                .Delay(delayBetweenSteps)
                .Rotate(target.transform, target.transform.eulerAngles + Vector3.up * 180f, stepDuration)
                .Delay(delayBetweenSteps)
                .Scale(target.transform, originalScale * 1.5f, stepDuration)
                .Delay(delayBetweenSteps)
                .Move(target.transform, originalPos, stepDuration)
                .Scale(target.transform, originalScale, stepDuration)
                .Play();
        }
        
        public void DemoParallelSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Parallel sequence on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            Vector3 originalScale = target.transform.localScale;
            
            // Sequence with parallel actions using Join
            TweenHelper.CreateSequence(target)
                .Move(target.transform, originalPos + Vector3.right * 3f, stepDuration)
                .JoinMove(target.transform, originalPos + Vector3.up * 2f, stepDuration) // Parallel move
                .JoinScale(target.transform, originalScale * 2f, stepDuration) // Parallel scale
                .Delay(delayBetweenSteps)
                .Move(target.transform, originalPos, stepDuration)
                .JoinScale(target.transform, originalScale, stepDuration)
                .Play();
        }
        
        public void DemoComplexSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Complex sequence on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            Vector3 originalRot = target.transform.eulerAngles;
            Vector3 originalScale = target.transform.localScale;
            
            // Complex multi-step flow
            var sequence = TweenHelper.CreateSequence(target)
                // Phase 1: Rise and rotate
                .Move(target.transform, originalPos + Vector3.up * 3f, stepDuration)
                .JoinRotate(target.transform, originalRot + Vector3.up * 360f, stepDuration)
                .Delay(0.2f)
                
                // Phase 2: Move in circle while scaling
                .Move(target.transform, originalPos + Vector3.right * 2f + Vector3.up * 3f, stepDuration * 0.5f)
                .JoinScale(target.transform, originalScale * 1.5f, stepDuration * 0.5f)
                .Move(target.transform, originalPos + Vector3.back * 2f + Vector3.up * 3f, stepDuration * 0.5f)
                .Move(target.transform, originalPos + Vector3.left * 2f + Vector3.up * 3f, stepDuration * 0.5f)
                .Move(target.transform, originalPos + Vector3.forward * 2f + Vector3.up * 3f, stepDuration * 0.5f)
                .JoinScale(target.transform, originalScale, stepDuration * 0.5f)
                
                // Phase 3: Return home
                .Delay(0.3f)
                .Move(target.transform, originalPos, stepDuration)
                .Play();
                
            // Add some fade effects to components if available
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                sequence.OnPlay(() => TweenHelper.FadeOut(canvasGroup, 0.2f));
                sequence.OnComplete(() => TweenHelper.FadeIn(canvasGroup, 0.5f));
            }
        }
        
        public void DemoLoopSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Looped sequence on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            Vector3 originalScale = target.transform.localScale;
            
            // Create a looping sequence using TweenOptions
            var loopOptions = TweenOptions.WithLoops(3, LoopType.Yoyo);
            
            TweenHelper.CreateSequence(target)
                .Move(target.transform, originalPos + Vector3.up * 2f, stepDuration)
                .Scale(target.transform, originalScale * 1.3f, stepDuration * 0.5f)
                .JoinRotate(target.transform, target.transform.eulerAngles + Vector3.up * 90f, stepDuration * 0.5f)
                .Delay(0.2f)
                .Scale(target.transform, originalScale, stepDuration * 0.5f)
                .Move(target.transform, originalPos, stepDuration)
                .Play(loopOptions);
        }
        
        public void DemoCallbackSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Callback sequence on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            
            int stepCounter = 0;
            
            TweenHelper.CreateSequence(target)
                .Call(() => {
                    stepCounter++;
                    Debug.Log($"Step {stepCounter}: Starting animation on {target.name}");
                })
                .Move(target.transform, originalPos + Vector3.right * 3f, stepDuration)
                .Call(() => {
                    stepCounter++;
                    Debug.Log($"Step {stepCounter}: Reached first position");
                })
                .Delay(0.5f)
                .Call(() => {
                    stepCounter++;
                    Debug.Log($"Step {stepCounter}: Starting return journey");
                })
                .Move(target.transform, originalPos + Vector3.up * 2f, stepDuration)
                .Move(target.transform, originalPos, stepDuration)
                .Call(() => {
                    stepCounter++;
                    Debug.Log($"Step {stepCounter}: Animation complete!");
                })
                .Play();
        }
        
        public void DemoPresetSequence()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Preset sequence on {target.name}");
            
            // Prepare for PopIn
            target.transform.localScale = Vector3.zero;
            
            TweenHelper.CreateSequence(target)
                .Preset("PopIn", target, stepDuration)
                .Delay(delayBetweenSteps)
                .Preset("Bounce", target, stepDuration)
                .Delay(delayBetweenSteps)
                .Preset("Shake", target, stepDuration * 0.5f)
                .Delay(delayBetweenSteps)
                .Preset("PopOut", target, stepDuration)
                .Call(() => {
                    // Reset after PopOut
                    target.transform.localScale = Vector3.one;
                })
                .Play();
        }
        
        public void DemoSequenceWithFade()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            var canvasGroup = target.GetComponent<CanvasGroup>();
            var image = target.GetComponent<Image>();
            
            if (canvasGroup == null && image == null)
            {
                Debug.LogWarning($"No fadeable component on {target.name}");
                return;
            }
            
            Debug.Log($"Fade sequence on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            
            var builder = TweenHelper.CreateSequence(target);
            
            // Fade out first
            if (canvasGroup != null)
            {
                builder.Fade(canvasGroup, 0f, stepDuration * 0.5f);
            }
            else if (image != null)
            {
                builder.Fade(image, 0f, stepDuration * 0.5f);
            }
            
            // Move while invisible
            builder.Move(target.transform, originalPos + Vector3.up * 3f, stepDuration);
            
            // Fade back in
            if (canvasGroup != null)
            {
                builder.Fade(canvasGroup, 1f, stepDuration * 0.5f);
            }
            else if (image != null)
            {
                builder.Fade(image, 1f, stepDuration * 0.5f);
            }
            
            // Return to original position
            builder.Delay(delayBetweenSteps)
                .Move(target.transform, originalPos, stepDuration)
                .Play();
        }
        
        public void DemoMultiObjectSequence()
        {
            if (demoObjects == null || demoObjects.Length < 2) return;
            
            Debug.Log("Multi-object sequence");
            
            var obj1 = demoObjects[0];
            var obj2 = demoObjects[1 % demoObjects.Length];
            
            if (obj1 == null || obj2 == null) return;
            
            Vector3 pos1 = obj1.transform.position;
            Vector3 pos2 = obj2.transform.position;
            
            // Create a sequence that animates multiple objects
            TweenHelper.CreateSequence()
                .Move(obj1.transform, pos2, stepDuration)
                .JoinMove(obj2.transform, pos1, stepDuration) // Swap positions
                .Delay(delayBetweenSteps)
                .Move(obj1.transform, pos1, stepDuration)
                .JoinMove(obj2.transform, pos2, stepDuration) // Swap back
                .Play();
        }
        
        private GameObject GetNextDemoObject()
        {
            if (demoObjects == null || demoObjects.Length == 0) return null;
            
            var target = demoObjects[currentObjectIndex];
            currentObjectIndex = (currentObjectIndex + 1) % demoObjects.Length;
            return target;
        }
        
        private void OnEnable()
        {
            currentObjectIndex = 0;
        }
        
        private void Update()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) DemoSimpleSequence();
            if (Input.GetKeyDown(KeyCode.Alpha2)) DemoParallelSequence();
            if (Input.GetKeyDown(KeyCode.Alpha3)) DemoComplexSequence();
            if (Input.GetKeyDown(KeyCode.Alpha4)) DemoLoopSequence();
            if (Input.GetKeyDown(KeyCode.Alpha5)) DemoCallbackSequence();
            if (Input.GetKeyDown(KeyCode.Alpha6)) DemoPresetSequence();
            if (Input.GetKeyDown(KeyCode.F)) DemoSequenceWithFade();
            if (Input.GetKeyDown(KeyCode.M)) DemoMultiObjectSequence();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 180));
            GUILayout.Label("Sequence Demo Keys:", GUI.skin.box);
            GUILayout.Label("1 : Simple sequence");
            GUILayout.Label("2 : Parallel actions");
            GUILayout.Label("3 : Complex flow");
            GUILayout.Label("4 : Looped sequence");
            GUILayout.Label("5 : With callbacks");
            GUILayout.Label("6 : Preset sequence");
            GUILayout.Label("F : Fade sequence");
            GUILayout.Label("M : Multi-object");
            GUILayout.EndArea();
        }
    }
}
