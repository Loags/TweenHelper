using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates the built-in preset system with PopIn, PopOut, Bounce, Shake, FadeIn, FadeOut.
    /// </summary>
    public class PresetDemo : MonoBehaviour
    {
        [Header("Preset Controls")]
        [SerializeField] private Button popInButton;
        [SerializeField] private Button popOutButton;
        [SerializeField] private Button bounceButton;
        [SerializeField] private Button shakeButton;
        [SerializeField] private Button fadeInButton;
        [SerializeField] private Button fadeOutButton;
        [SerializeField] private Button allPresetsButton;
        [SerializeField] private Button randomPresetButton;
        
        [Header("Settings")]
        [SerializeField] private float presetDuration = 0.5f;
        [SerializeField] private bool showAvailablePresets = true;
        
        private GameObject[] demoObjects;
        private int currentObjectIndex = 0;
        
        // Built-in preset names
        private readonly string[] builtInPresets = {
            "PopIn", "PopOut", "Bounce", "Shake", "FadeIn", "FadeOut"
        };
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
            
            if (showAvailablePresets)
            {
                LogAvailablePresets();
            }
        }
        
        private void SetupButtons()
        {
            SetupButton(popInButton, "PopIn", () => DemoPreset("PopIn"));
            SetupButton(popOutButton, "PopOut", () => DemoPreset("PopOut"));
            SetupButton(bounceButton, "Bounce", () => DemoPreset("Bounce"));
            SetupButton(shakeButton, "Shake", () => DemoPreset("Shake"));
            SetupButton(fadeInButton, "FadeIn", () => DemoPreset("FadeIn"));
            SetupButton(fadeOutButton, "FadeOut", () => DemoPreset("FadeOut"));
            SetupButton(allPresetsButton, "All Presets", DemoAllPresets);
            SetupButton(randomPresetButton, "Random", DemoRandomPreset);
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
        
        public void DemoPreset(string presetName)
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Playing preset '{presetName}' on {target.name}");
            
            // Check if preset exists
            if (!TweenHelper.HasPreset(presetName))
            {
                Debug.LogError($"Preset '{presetName}' not found!");
                return;
            }
            
            // Special setup for certain presets
            PrepareObjectForPreset(target, presetName);
            
            // Play the preset with optional duration override
            var tween = TweenHelper.PlayPreset(presetName, target, presetDuration);
            
            if (tween != null)
            {
                Debug.Log($"Successfully started preset '{presetName}' on {target.name}");
                
                // Add completion callback for some presets
                if (presetName == "PopOut")
                {
                    tween.OnComplete(() =>
                    {
                        // Reset scale after PopOut
                        target.transform.localScale = Vector3.one;
                    });
                }
            }
            else
            {
                Debug.LogWarning($"Failed to play preset '{presetName}' on {target.name}");
            }
        }
        
        private void PrepareObjectForPreset(GameObject target, string presetName)
        {
            switch (presetName)
            {
                case "PopIn":
                    // PopIn starts from zero scale
                    target.transform.localScale = Vector3.zero;
                    break;
                    
                case "FadeIn":
                    // FadeIn starts from alpha 0
                    SetObjectAlpha(target, 0f);
                    break;
                    
                case "FadeOut":
                    // FadeOut starts from alpha 1
                    SetObjectAlpha(target, 1f);
                    break;
            }
        }
        
        private void SetObjectAlpha(GameObject target, float alpha)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                return;
            }
            
            var image = target.GetComponent<Image>();
            if (image != null)
            {
                var color = image.color;
                color.a = alpha;
                image.color = color;
                return;
            }
            
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
                return;
            }
        }
        
        public void DemoAllPresets()
        {
            if (demoObjects == null) return;
            
            Debug.Log("Playing all presets in sequence");
            
            int presetIndex = 0;
            float delay = 0f;
            
            foreach (string presetName in builtInPresets)
            {
                if (presetIndex >= demoObjects.Length) break;
                
                var target = demoObjects[presetIndex];
                if (target == null) continue;
                
                // Schedule preset with delay
                StartCoroutine(PlayPresetWithDelay(target, presetName, delay));
                
                delay += 0.8f; // Stagger the presets
                presetIndex++;
            }
        }
        
        private System.Collections.IEnumerator PlayPresetWithDelay(GameObject target, string presetName, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            PrepareObjectForPreset(target, presetName);
            TweenHelper.PlayPreset(presetName, target, presetDuration);
        }
        
        public void DemoRandomPreset()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            string randomPreset = builtInPresets[Random.Range(0, builtInPresets.Length)];
            Debug.Log($"Playing random preset: {randomPreset}");
            
            DemoPreset(randomPreset);
        }
        
        public void DemoPresetCompatibility()
        {
            Debug.Log("=== Preset Compatibility Test ===");
            
            foreach (var obj in demoObjects)
            {
                if (obj == null) continue;
                
                var compatiblePresets = TweenHelper.GetCompatiblePresets(obj);
                Debug.Log($"{obj.name} is compatible with presets: {string.Join(", ", compatiblePresets)}");
            }
        }
        
        public void DemoPresetWithOptions()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            // Play PopIn with custom options
            target.transform.localScale = Vector3.zero;
            
            var options = TweenOptions.WithEase(DG.Tweening.Ease.OutElastic)
                .SetDelay(0.2f)
                .SetLoops(2, DG.Tweening.LoopType.Yoyo);
            
            Debug.Log($"Playing PopIn with custom options on {target.name}");
            TweenHelper.PlayPreset("PopIn", target, 1f, options);
        }
        
        public void DemoCustomPresetChain()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Chaining presets on {target.name}");
            
            // Chain multiple presets
            target.transform.localScale = Vector3.zero;
            
            var popIn = TweenHelper.PlayPreset("PopIn", target, 0.5f);
            if (popIn != null)
            {
                popIn.OnComplete(() =>
                {
                    var bounce = TweenHelper.PlayPreset("Bounce", target, 0.5f);
                    if (bounce != null)
                    {
                        bounce.OnComplete(() =>
                        {
                            var shake = TweenHelper.PlayPreset("Shake", target, 0.3f);
                            if (shake != null)
                            {
                                shake.OnComplete(() =>
                                {
                                    TweenHelper.PlayPreset("PopOut", target, 0.5f);
                                });
                            }
                        });
                    }
                });
            }
        }
        
        private void LogAvailablePresets()
        {
            var availablePresets = TweenHelper.GetAvailablePresets();
            Debug.Log($"Available presets: {string.Join(", ", availablePresets)}");
            
            // Log compatibility for each demo object
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    var compatible = TweenHelper.GetCompatiblePresets(obj);
                    Debug.Log($"{obj.name} compatible presets: {string.Join(", ", compatible)}");
                }
            }
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
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) DemoPreset("PopIn");
            if (Input.GetKeyDown(KeyCode.Alpha2)) DemoPreset("PopOut");
            if (Input.GetKeyDown(KeyCode.Alpha3)) DemoPreset("Bounce");
            if (Input.GetKeyDown(KeyCode.Alpha4)) DemoPreset("Shake");
            if (Input.GetKeyDown(KeyCode.Alpha5)) DemoPreset("FadeIn");
            if (Input.GetKeyDown(KeyCode.Alpha6)) DemoPreset("FadeOut");
            if (Input.GetKeyDown(KeyCode.A)) DemoAllPresets();
            if (Input.GetKeyDown(KeyCode.Q)) DemoRandomPreset();
            if (Input.GetKeyDown(KeyCode.C)) DemoPresetCompatibility();
            if (Input.GetKeyDown(KeyCode.O)) DemoPresetWithOptions();
            if (Input.GetKeyDown(KeyCode.P)) DemoCustomPresetChain();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 200));
            GUILayout.Label("Preset Demo Keys:", GUI.skin.box);
            GUILayout.Label("1 : PopIn");
            GUILayout.Label("2 : PopOut");
            GUILayout.Label("3 : Bounce");
            GUILayout.Label("4 : Shake");
            GUILayout.Label("5 : FadeIn");
            GUILayout.Label("6 : FadeOut");
            GUILayout.Label("A : All presets");
            GUILayout.Label("Q : Random preset");
            GUILayout.Label("C : Check compatibility");
            GUILayout.Label("O : Preset with options");
            GUILayout.Label("P : Preset chain");
            GUILayout.EndArea();
        }
    }
}
