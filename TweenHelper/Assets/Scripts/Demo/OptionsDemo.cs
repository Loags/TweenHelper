using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates TweenOptions functionality: easing, delays, loops, unscaled time, snapping, and speed-based tweens.
    /// </summary>
    public class OptionsDemo : MonoBehaviour
    {
        [Header("Options Controls")]
        [SerializeField] private Button easingButton;
        [SerializeField] private Button delayButton;
        [SerializeField] private Button loopsButton;
        [SerializeField] private Button unscaledTimeButton;
        [SerializeField] private Button snappingButton;
        [SerializeField] private Button speedBasedButton;
        [SerializeField] private Button combinedOptionsButton;
        [SerializeField] private Button fluentApiButton;
        
        [Header("Easing Comparison")]
        [SerializeField] private Button easingComparisonButton;
        
        [Header("Settings")]
        [SerializeField] private float baseDuration = 2f;
        [SerializeField] private bool showOptionsLogs = true;
        
        private GameObject[] demoObjects;
        private int currentObjectIndex = 0;
        
        // Easing types for comparison
        private readonly Ease[] easingTypes = {
            Ease.Linear, Ease.InQuad, Ease.OutQuad, Ease.InOutQuad,
            Ease.InCubic, Ease.OutCubic, Ease.InOutCubic,
            Ease.InQuart, Ease.OutQuart, Ease.InOutQuart,
            Ease.InBounce, Ease.OutBounce, Ease.InOutBounce,
            Ease.InElastic, Ease.OutElastic, Ease.InOutElastic,
            Ease.InBack, Ease.OutBack, Ease.InOutBack
        };
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            SetupButton(easingButton, "Easing Demo", DemoEasing);
            SetupButton(delayButton, "Delay Demo", DemoDelay);
            SetupButton(loopsButton, "Loops Demo", DemoLoops);
            SetupButton(unscaledTimeButton, "Unscaled Time", DemoUnscaledTime);
            SetupButton(snappingButton, "Snapping Demo", DemoSnapping);
            SetupButton(speedBasedButton, "Speed Based", DemoSpeedBased);
            SetupButton(combinedOptionsButton, "Combined Options", DemoCombinedOptions);
            SetupButton(fluentApiButton, "Fluent API", DemoFluentApi);
            SetupButton(easingComparisonButton, "Easing Comparison", DemoEasingComparison);
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
        
        public void DemoEasing()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Easing demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.right * 5f;
            
            // Show different easing types
            var easeType = easingTypes[Random.Range(0, easingTypes.Length)];
            var options = TweenOptions.WithEase(easeType);
            
            Log($"Using ease: {easeType}");
            
            var tween = TweenHelper.MoveTo(target.transform, targetPos, baseDuration, options);
            tween.OnComplete(() =>
            {
                // Return with different easing
                var returnEase = easingTypes[Random.Range(0, easingTypes.Length)];
                var returnOptions = TweenOptions.WithEase(returnEase);
                Log($"Return with ease: {returnEase}");
                TweenHelper.MoveTo(target.transform, originalPos, baseDuration * 0.5f, returnOptions);
            });
        }
        
        public void DemoDelay()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Delay demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.up * 3f;
            
            float delay = Random.Range(0.5f, 2f);
            var options = TweenOptions.WithDelay(delay);
            
            Log($"Animation will start after {delay:F1} second delay");
            
            var tween = TweenHelper.MoveTo(target.transform, targetPos, baseDuration, options);
            tween.OnComplete(() =>
            {
                TweenHelper.MoveTo(target.transform, originalPos, baseDuration * 0.5f);
            });
        }
        
        public void DemoLoops()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Loops demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.forward * 3f;
            
            // Different loop types
            var loopTypes = new[] { LoopType.Restart, LoopType.Yoyo, LoopType.Incremental };
            var selectedLoopType = loopTypes[Random.Range(0, loopTypes.Length)];
            int loopCount = Random.Range(2, 5);
            
            var options = TweenOptions.WithLoops(loopCount, selectedLoopType);
            
            Log($"Looping {loopCount} times with {selectedLoopType} type");
            
            TweenHelper.MoveTo(target.transform, targetPos, baseDuration * 0.5f, options);
        }
        
        public void DemoUnscaledTime()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Unscaled time demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.left * 4f;
            
            var options = TweenOptions.WithUnscaledTime();
            
            Log("Animation uses unscaled time - try changing Time.timeScale!");
            
            // Temporarily slow down time to show the difference
            StartCoroutine(TimeScaleDemo());
            
            var tween = TweenHelper.MoveTo(target.transform, targetPos, baseDuration, options);
            tween.OnComplete(() =>
            {
                TweenHelper.MoveTo(target.transform, originalPos, baseDuration * 0.5f, options);
            });
        }
        
        private System.Collections.IEnumerator TimeScaleDemo()
        {
            float originalTimeScale = Time.timeScale;
            Log($"Original time scale: {originalTimeScale}");
            
            yield return new WaitForSeconds(0.5f);
            
            Time.timeScale = 0.1f;
            Log("Time scale set to 0.1 - unscaled time animations should continue normally");
            
            yield return new WaitForSecondsRealtime(3f);
            
            Time.timeScale = originalTimeScale;
            Log($"Time scale restored to {originalTimeScale}");
        }
        
        public void DemoSnapping()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Snapping demo on {target.name}");
            
            var originalPos = target.transform.position;
            // Use fractional target position to demonstrate snapping
            var targetPos = originalPos + new Vector3(3.7f, 2.3f, 1.9f);
            
            var options = TweenOptions.WithSnapping();
            
            Log($"Moving to {targetPos} with snapping enabled - position will be rounded");
            
            var tween = TweenHelper.MoveTo(target.transform, targetPos, baseDuration, options);
            tween.OnComplete(() =>
            {
                Log($"Final position: {target.transform.position}");
                TweenHelper.MoveTo(target.transform, originalPos, baseDuration * 0.5f);
            });
        }
        
        public void DemoSpeedBased()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Speed-based demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.back * 6f;
            
            var options = TweenOptions.WithSpeedBased();
            
            Log("Using speed-based animation - duration represents units per second");
            
            float speed = 3f; // 3 units per second
            var tween = TweenHelper.MoveTo(target.transform, targetPos, speed, options);
            tween.OnComplete(() =>
            {
                TweenHelper.MoveTo(target.transform, originalPos, speed, options);
            });
        }
        
        public void DemoCombinedOptions()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Combined options demo on {target.name}");
            
            var originalPos = target.transform.position;
            var originalScale = target.transform.localScale;
            var targetPos = originalPos + Vector3.up * 2f + Vector3.right * 2f;
            var targetScale = originalScale * 1.5f;
            
            // Combine multiple options
            var options = TweenOptions.WithEase(Ease.OutBounce)
                .SetDelay(0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetId("combined-demo")
                .SetSnapping(true);
            
            Log("Combined options: OutBounce ease, 0.5s delay, 2 yoyo loops, snapping, with ID");
            
            // Apply to multiple animations
            TweenHelper.MoveTo(target.transform, targetPos, baseDuration, options);
            TweenHelper.ScaleTo(target.transform, targetScale, baseDuration, options);
        }
        
        public void DemoFluentApi()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Fluent API demo on {target.name}");
            
            var originalPos = target.transform.position;
            var targetPos = originalPos + Vector3.forward * 3f + Vector3.up * 2f;
            
            // Demonstrate fluent API
            var options = TweenOptions.None
                .SetEase(Ease.InOutElastic)
                .SetDelay(0.3f)
                .SetLoops(3, LoopType.Yoyo)
                .SetUnscaledTime(false)
                .SetSnapping(false)
                .SetId("fluent-api-demo");
            
            Log("Fluent API: None.SetEase().SetDelay().SetLoops().SetUnscaledTime().SetSnapping().SetId()");
            
            TweenHelper.MoveTo(target.transform, targetPos, baseDuration, options);
        }
        
        public void DemoEasingComparison()
        {
            if (demoObjects == null || demoObjects.Length < 4) return;
            
            Log("Easing comparison demo - using first 4 objects");
            
            var selectedEases = new[] { Ease.Linear, Ease.OutQuart, Ease.OutBounce, Ease.OutElastic };
            var easeNames = new[] { "Linear", "OutQuart", "OutBounce", "OutElastic" };
            
            for (int i = 0; i < 4 && i < demoObjects.Length; i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                var originalPos = obj.transform.position;
                var targetPos = originalPos + Vector3.right * 6f;
                
                var options = TweenOptions.WithEase(selectedEases[i]);
                
                Log($"Object {i + 1}: {easeNames[i]} easing");
                
                var tween = TweenHelper.MoveTo(obj.transform, targetPos, baseDuration * 1.5f, options);
                tween.OnComplete(() =>
                {
                    TweenHelper.MoveTo(obj.transform, originalPos, baseDuration * 0.5f);
                });
            }
        }
        
        public void DemoOptionsWithPresets()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Log($"Options with presets demo on {target.name}");
            
            // Prepare for PopIn
            target.transform.localScale = Vector3.zero;
            
            // Apply custom options to preset
            var options = TweenOptions.WithEase(Ease.OutElastic)
                .SetDelay(0.5f)
                .SetLoops(2, LoopType.Yoyo);
            
            Log("Playing PopIn preset with custom options: OutElastic ease, 0.5s delay, 2 yoyo loops");
            
            TweenHelper.PlayPreset("PopIn", target, baseDuration, options);
        }
        
        public void DemoIdBasedOptions()
        {
            Log("ID-based options demo");
            
            // Start multiple animations with same ID
            var groupId = "options-group";
            var options = TweenOptions.WithId(groupId)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            
            foreach (var obj in demoObjects)
            {
                if (obj == null) continue;
                
                var originalPos = obj.transform.position;
                var targetPos = originalPos + Vector3.up * Random.Range(1f, 3f);
                
                TweenHelper.MoveTo(obj.transform, targetPos, baseDuration, options);
            }
            
            Log($"Started looping animations with ID '{groupId}' - use control demo to manage them");
            
            // Stop after 5 seconds
            StartCoroutine(StopGroupAfterDelay(groupId, 5f));
        }
        
        private System.Collections.IEnumerator StopGroupAfterDelay(string id, float delay)
        {
            yield return new WaitForSeconds(delay);
            Log($"Stopping animations with ID '{id}'");
            TweenHelper.KillById(id);
        }
        
        private GameObject GetNextDemoObject()
        {
            if (demoObjects == null || demoObjects.Length == 0) return null;
            
            var target = demoObjects[currentObjectIndex];
            currentObjectIndex = (currentObjectIndex + 1) % demoObjects.Length;
            return target;
        }
        
        private void Log(string message)
        {
            if (showOptionsLogs)
            {
                Debug.Log($"[OptionsDemo] {message}");
            }
        }
        
        private void Update()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) DemoEasing();
            if (Input.GetKeyDown(KeyCode.Alpha2)) DemoDelay();
            if (Input.GetKeyDown(KeyCode.Alpha3)) DemoLoops();
            if (Input.GetKeyDown(KeyCode.Alpha4)) DemoUnscaledTime();
            if (Input.GetKeyDown(KeyCode.Alpha5)) DemoSnapping();
            if (Input.GetKeyDown(KeyCode.Alpha6)) DemoSpeedBased();
            if (Input.GetKeyDown(KeyCode.Alpha7)) DemoCombinedOptions();
            if (Input.GetKeyDown(KeyCode.F)) DemoFluentApi();
            if (Input.GetKeyDown(KeyCode.E)) DemoEasingComparison();
            if (Input.GetKeyDown(KeyCode.P)) DemoOptionsWithPresets();
            if (Input.GetKeyDown(KeyCode.I)) DemoIdBasedOptions();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 220, 10, 210, 250));
            GUILayout.Label("Options Demo Keys:", GUI.skin.box);
            GUILayout.Label("1 : Random easing");
            GUILayout.Label("2 : Delay");
            GUILayout.Label("3 : Loops");
            GUILayout.Label("4 : Unscaled time");
            GUILayout.Label("5 : Snapping");
            GUILayout.Label("6 : Speed based");
            GUILayout.Label("7 : Combined options");
            GUILayout.Label("F : Fluent API");
            GUILayout.Label("E : Easing comparison");
            GUILayout.Label("P : Options with presets");
            GUILayout.Label("I : ID-based options");
            
            GUILayout.Space(10);
            GUILayout.Label($"Current time scale: {Time.timeScale:F1}");
            
            if (GUILayout.Button("Reset Time Scale"))
            {
                Time.timeScale = 1f;
            }
            
            GUILayout.EndArea();
        }
    }
}
