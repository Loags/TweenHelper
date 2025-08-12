using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Main controller for the TweenHelper demo.
    /// Orchestrates all demo sections and provides UI controls.
    /// </summary>
    public class TweenDemoController : MonoBehaviour
    {
        [Header("Demo Sections")]
        [SerializeField] private BasicAnimationDemo basicAnimationDemo;
        [SerializeField] private PresetDemo presetDemo;
        [SerializeField] private SequenceDemo sequenceDemo;
        [SerializeField] private StaggerDemo staggerDemo;
        [SerializeField] private ControlDemo controlDemo;
        [SerializeField] private AsyncDemo asyncDemo;
        [SerializeField] private OptionsDemo optionsDemo;
        
        [Header("UI Controls")]
        [SerializeField] private Button[] demoButtons;
        [SerializeField] private Text currentDemoText;
        [SerializeField] private Text instructionsText;
        
        [Header("Demo Objects")]
        [SerializeField] private GameObject[] demoObjects;
        
        private int currentDemoIndex = 0;
        private readonly string[] demoNames = {
            "Basic Animations",
            "Preset Animations", 
            "Sequence Composition",
            "Staggered Animations",
            "Control Surface",
            "Async Operations",
            "Options & Settings"
        };
        
        private readonly string[] demoInstructions = {
            "Click buttons to see basic move, rotate, scale, and fade animations",
            "Experience built-in presets: PopIn, PopOut, Bounce, Shake, FadeIn, FadeOut",
            "Watch multi-step sequences with delays, parallel actions, and callbacks",
            "See coordinated animations across multiple objects with stagger delays",
            "Learn to pause, resume, kill, and control animations during playback",
            "Explore async/await integration with timeouts and cancellation",
            "Discover easing, loops, delays, and advanced tween configuration"
        };
        
        private void Start()
        {
            InitializeDemos();
            SetupUI();
            SwitchToDemo(0);
        }
        
        private void InitializeDemos()
        {
            // Ensure all demo components exist
            if (basicAnimationDemo == null) basicAnimationDemo = GetComponent<BasicAnimationDemo>();
            if (presetDemo == null) presetDemo = GetComponent<PresetDemo>();
            if (sequenceDemo == null) sequenceDemo = GetComponent<SequenceDemo>();
            if (staggerDemo == null) staggerDemo = GetComponent<StaggerDemo>();
            if (controlDemo == null) controlDemo = GetComponent<ControlDemo>();
            if (asyncDemo == null) asyncDemo = GetComponent<AsyncDemo>();
            if (optionsDemo == null) optionsDemo = GetComponent<OptionsDemo>();
            
            // Initialize each demo (only if not already initialized by setup script)
            if (demoObjects != null && demoObjects.Length > 0)
            {
                basicAnimationDemo?.Initialize(demoObjects);
                presetDemo?.Initialize(demoObjects);
                sequenceDemo?.Initialize(demoObjects);
                staggerDemo?.Initialize(demoObjects);
                controlDemo?.Initialize(demoObjects);
                asyncDemo?.Initialize(demoObjects);
                optionsDemo?.Initialize(demoObjects);
            }
        }
        
        private void SetupUI()
        {
            if (demoButtons != null)
            {
                for (int i = 0; i < demoButtons.Length && i < demoNames.Length; i++)
                {
                    int index = i; // Capture for closure
                    demoButtons[i].onClick.AddListener(() => SwitchToDemo(index));
                    
                    // Set button text if it has a Text component
                    var buttonText = demoButtons[i].GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = demoNames[i];
                    }
                }
            }
        }
        
        public void SwitchToDemo(int demoIndex)
        {
            if (demoIndex < 0 || demoIndex >= demoNames.Length) return;
            
            // Stop all current animations
            StopAllAnimations();
            
            // Reset demo objects to original positions
            ResetDemoObjects();
            
            currentDemoIndex = demoIndex;
            
            // Update UI
            if (currentDemoText != null)
                currentDemoText.text = $"Demo: {demoNames[currentDemoIndex]}";
            
            if (instructionsText != null)
                instructionsText.text = demoInstructions[currentDemoIndex];
            
            // Activate the selected demo
            ActivateDemo(currentDemoIndex);
        }
        
        private void ActivateDemo(int demoIndex)
        {
            // Disable all demo components first
            if (basicAnimationDemo != null) basicAnimationDemo.enabled = false;
            if (presetDemo != null) presetDemo.enabled = false;
            if (sequenceDemo != null) sequenceDemo.enabled = false;
            if (staggerDemo != null) staggerDemo.enabled = false;
            if (controlDemo != null) controlDemo.enabled = false;
            if (asyncDemo != null) asyncDemo.enabled = false;
            if (optionsDemo != null) optionsDemo.enabled = false;
            
            // Enable selected demo component
            switch (demoIndex)
            {
                case 0: if (basicAnimationDemo != null) basicAnimationDemo.enabled = true; break;
                case 1: if (presetDemo != null) presetDemo.enabled = true; break;
                case 2: if (sequenceDemo != null) sequenceDemo.enabled = true; break;
                case 3: if (staggerDemo != null) staggerDemo.enabled = true; break;
                case 4: if (controlDemo != null) controlDemo.enabled = true; break;
                case 5: if (asyncDemo != null) asyncDemo.enabled = true; break;
                case 6: if (optionsDemo != null) optionsDemo.enabled = true; break;
            }
        }
        
        public void NextDemo()
        {
            int nextIndex = (currentDemoIndex + 1) % demoNames.Length;
            SwitchToDemo(nextIndex);
        }
        
        public void PreviousDemo()
        {
            int prevIndex = (currentDemoIndex - 1 + demoNames.Length) % demoNames.Length;
            SwitchToDemo(prevIndex);
        }
        
        public void StopAllAnimations()
        {
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    TweenHelper.Kill(obj);
                }
            }
        }
        
        public void ResetDemoObjects()
        {
            for (int i = 0; i < demoObjects.Length; i++)
            {
                if (demoObjects[i] != null)
                {
                    var resetPos = GetOriginalPosition(i);
                    var resetRot = GetOriginalRotation(i);
                    var resetScale = GetOriginalScale(i);
                    
                    demoObjects[i].transform.position = resetPos;
                    demoObjects[i].transform.rotation = resetRot;
                    demoObjects[i].transform.localScale = resetScale;
                    
                    // Reset alpha for UI components
                    var canvasGroup = demoObjects[i].GetComponent<CanvasGroup>();
                    if (canvasGroup != null) canvasGroup.alpha = 1f;
                    
                    var image = demoObjects[i].GetComponent<Image>();
                    if (image != null)
                    {
                        var color = image.color;
                        color.a = 1f;
                        image.color = color;
                    }
                    
                    var spriteRenderer = demoObjects[i].GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        var color = spriteRenderer.color;
                        color.a = 1f;
                        spriteRenderer.color = color;
                    }
                }
            }
        }
        
        private Vector3 GetOriginalPosition(int index)
        {
            // Arrange objects in a grid pattern
            int cols = 3;
            float spacing = 3f;
            int row = index / cols;
            int col = index % cols;
            
            return new Vector3(
                (col - 1) * spacing,
                1f,
                (row - 1) * spacing
            );
        }
        
        private Quaternion GetOriginalRotation(int index)
        {
            return Quaternion.identity;
        }
        
        private Vector3 GetOriginalScale(int index)
        {
            return Vector3.one;
        }
        
        private void Update()
        {
            // Handle keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextDemo();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PreviousDemo();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllAnimations();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ResetDemoObjects();
            }
        }
        
        private void OnGUI()
        {
            // Display keyboard shortcuts
            GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 100));
            GUILayout.Label("Keyboard Shortcuts:", GUI.skin.box);
            GUILayout.Label("← → : Switch demos");
            GUILayout.Label("Space : Stop all animations");
            GUILayout.Label("R : Reset objects");
            GUILayout.EndArea();
        }
    }
}
