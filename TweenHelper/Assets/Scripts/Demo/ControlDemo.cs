using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates the control surface functionality: pause, resume, kill, complete, rewind operations.
    /// Shows both target-based and ID-based control methods.
    /// </summary>
    public class ControlDemo : MonoBehaviour
    {
        [Header("Control Buttons")]
        [SerializeField] private Button startAnimationsButton;
        [SerializeField] private Button pauseAllButton;
        [SerializeField] private Button resumeAllButton;
        [SerializeField] private Button killAllButton;
        [SerializeField] private Button completeAllButton;
        [SerializeField] private Button idControlButton;
        [SerializeField] private Button diagnosticsButton;
        
        [Header("Individual Object Controls")]
        [SerializeField] private Button pauseObjectButton;
        [SerializeField] private Button resumeObjectButton;
        [SerializeField] private Button killObjectButton;
        
        [Header("Animation Settings")]
        [SerializeField] private float longAnimationDuration = 5f;
        [SerializeField] private bool showDebugInfo = true;
        
        private GameObject[] demoObjects;
        private int selectedObjectIndex = 0;
        private bool animationsRunning = false;
        
        // Animation IDs for ID-based control
        private readonly string[] animationIds = {
            "group-a", "group-b", "group-c", "special-animation"
        };
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            SetupButton(startAnimationsButton, "Start Animations", StartLongAnimations);
            SetupButton(pauseAllButton, "Pause All", PauseAllAnimations);
            SetupButton(resumeAllButton, "Resume All", ResumeAllAnimations);
            SetupButton(killAllButton, "Kill All", KillAllAnimations);
            SetupButton(completeAllButton, "Complete All", CompleteAllAnimations);
            SetupButton(idControlButton, "ID Control", DemoIdControl);
            SetupButton(diagnosticsButton, "Diagnostics", ShowDiagnostics);
            
            SetupButton(pauseObjectButton, "Pause Object", PauseSelectedObject);
            SetupButton(resumeObjectButton, "Resume Object", ResumeSelectedObject);
            SetupButton(killObjectButton, "Kill Object", KillSelectedObject);
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
        
        public void StartLongAnimations()
        {
            if (demoObjects == null || demoObjects.Length == 0) return;
            
            Debug.Log("Starting long animations for control demo");
            animationsRunning = true;
            
            for (int i = 0; i < demoObjects.Length; i++)
            {
                var obj = demoObjects[i];
                if (obj == null) continue;
                
                var originalPos = obj.transform.position;
                var originalScale = obj.transform.localScale;
                
                // Assign different animation IDs to different groups
                string animationId = animationIds[i % animationIds.Length];
                var options = TweenOptions.WithId(animationId);
                
                // Create different types of long animations
                switch (i % 4)
                {
                    case 0:
                        // Looping movement
                        var moveSequence = TweenHelper.CreateSequence(obj)
                            .Move(obj.transform, originalPos + Vector3.right * 3f, longAnimationDuration * 0.25f)
                            .Move(obj.transform, originalPos + Vector3.right * 3f + Vector3.up * 2f, longAnimationDuration * 0.25f)
                            .Move(obj.transform, originalPos + Vector3.up * 2f, longAnimationDuration * 0.25f)
                            .Move(obj.transform, originalPos, longAnimationDuration * 0.25f)
                            .Build(options.SetLoops(-1)); // Infinite loop
                        break;
                        
                    case 1:
                        // Continuous rotation
                        TweenHelper.RotateBy(obj.transform, Vector3.up * 360f, longAnimationDuration, 
                            options.SetLoops(-1));
                        break;
                        
                    case 2:
                        // Pulsing scale
                        TweenHelper.ScaleTo(obj.transform, originalScale * 1.5f, longAnimationDuration * 0.5f,
                            options.SetLoops(-1, LoopType.Yoyo));
                        break;
                        
                    case 3:
                        // Complex sequence
                        var complexSequence = TweenHelper.CreateSequence(obj)
                            .Move(obj.transform, originalPos + Vector3.up * 3f, longAnimationDuration * 0.3f)
                            .JoinScale(obj.transform, originalScale * 2f, longAnimationDuration * 0.3f)
                            .Delay(longAnimationDuration * 0.2f)
                            .Move(obj.transform, originalPos + Vector3.back * 2f, longAnimationDuration * 0.25f)
                            .Move(obj.transform, originalPos, longAnimationDuration * 0.25f)
                            .JoinScale(obj.transform, originalScale, longAnimationDuration * 0.25f)
                            .Build(options.SetLoops(-1));
                        break;
                }
                
                if (showDebugInfo)
                {
                    Debug.Log($"Started animation '{animationId}' on {obj.name}");
                }
            }
        }
        
        public void PauseAllAnimations()
        {
            Debug.Log("Pausing all animations");
            
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    TweenHelper.Pause(obj);
                }
            }
        }
        
        public void ResumeAllAnimations()
        {
            Debug.Log("Resuming all animations");
            
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    TweenHelper.Resume(obj);
                }
            }
        }
        
        public void KillAllAnimations()
        {
            Debug.Log("Killing all animations");
            animationsRunning = false;
            
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    TweenHelper.Kill(obj);
                }
            }
        }
        
        public void CompleteAllAnimations()
        {
            Debug.Log("Completing all animations");
            animationsRunning = false;
            
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    TweenHelper.Complete(obj);
                }
            }
        }
        
        public void DemoIdControl()
        {
            Debug.Log("Demonstrating ID-based control");
            
            // Start some animations with specific IDs
            if (demoObjects.Length > 0)
            {
                var obj = demoObjects[0];
                if (obj != null)
                {
                    var originalPos = obj.transform.position;
                    
                    // Start multiple animations with same ID
                    var options = TweenOptions.WithId("controllable-group");
                    
                    TweenHelper.MoveTo(obj.transform, originalPos + Vector3.right * 3f, 3f, options);
                    TweenHelper.RotateBy(obj.transform, Vector3.up * 360f, 3f, options);
                    TweenHelper.ScaleTo(obj.transform, Vector3.one * 1.5f, 3f, options);
                    
                    Debug.Log("Started animations with ID 'controllable-group'");
                    
                    // Demonstrate ID-based control after delay
                    StartCoroutine(DemoIdControlSequence());
                }
            }
        }
        
        private System.Collections.IEnumerator DemoIdControlSequence()
        {
            yield return new WaitForSeconds(1f);
            
            Debug.Log("Pausing by ID: controllable-group");
            TweenHelper.PauseById("controllable-group");
            
            yield return new WaitForSeconds(1f);
            
            Debug.Log("Resuming by ID: controllable-group");
            TweenHelper.ResumeById("controllable-group");
            
            yield return new WaitForSeconds(1f);
            
            Debug.Log("Killing by ID: controllable-group");
            TweenHelper.KillById("controllable-group");
        }
        
        public void PauseSelectedObject()
        {
            var selectedObj = GetSelectedObject();
            if (selectedObj == null) return;
            
            Debug.Log($"Pausing animations on {selectedObj.name}");
            TweenHelper.Pause(selectedObj);
        }
        
        public void ResumeSelectedObject()
        {
            var selectedObj = GetSelectedObject();
            if (selectedObj == null) return;
            
            Debug.Log($"Resuming animations on {selectedObj.name}");
            TweenHelper.Resume(selectedObj);
        }
        
        public void KillSelectedObject()
        {
            var selectedObj = GetSelectedObject();
            if (selectedObj == null) return;
            
            Debug.Log($"Killing animations on {selectedObj.name}");
            TweenHelper.Kill(selectedObj);
        }
        
        public void ShowDiagnostics()
        {
            Debug.Log("=== Animation Diagnostics ===");
            
            // Global diagnostics
            int totalActive = TweenHelper.GetTotalActiveTweenCount();
            Debug.Log($"Total active tweens globally: {totalActive}");
            
            // Per-object diagnostics
            foreach (var obj in demoObjects)
            {
                if (obj == null) continue;
                
                int objectTweens = TweenHelper.GetActiveTweenCount(obj);
                bool hasActive = TweenHelper.HasActiveTweens(obj);
                
                Debug.Log($"{obj.name}: {objectTweens} active tweens (hasActive: {hasActive})");
            }
            
            // ID-based diagnostics
            foreach (string id in animationIds)
            {
                bool hasById = TweenController.HasActiveTweensById(id);
                int countById = TweenController.GetActiveTweenCountById(id);
                
                if (hasById || countById > 0)
                {
                    Debug.Log($"ID '{id}': {countById} active tweens");
                }
            }
        }
        
        public void DemoRewindFunctionality()
        {
            var selectedObj = GetSelectedObject();
            if (selectedObj == null) return;
            
            Debug.Log($"Demonstrating rewind on {selectedObj.name}");
            
            var originalPos = selectedObj.transform.position;
            var originalScale = selectedObj.transform.localScale;
            
            // Start a long animation
            var moveTween = TweenHelper.MoveTo(selectedObj.transform, originalPos + Vector3.up * 3f, 3f);
            var scaleTween = TweenHelper.ScaleTo(selectedObj.transform, originalScale * 2f, 3f);
            
            // Rewind after 1.5 seconds
            StartCoroutine(RewindAfterDelay(selectedObj, 1.5f));
        }
        
        private System.Collections.IEnumerator RewindAfterDelay(GameObject target, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            Debug.Log($"Rewinding animations on {target.name}");
            TweenHelper.Rewind(target);
        }
        
        public void DemoControlByComponent()
        {
            Debug.Log("Demonstrating component-specific control");
            
            foreach (var obj in demoObjects)
            {
                if (obj == null) continue;
                
                // Start different animations on different components
                TweenHelper.MoveTo(obj.transform, obj.transform.position + Vector3.up * 2f, 3f);
                
                var canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    TweenHelper.FadeTo(canvasGroup, 0.3f, 3f);
                }
            }
            
            // Demonstrate selective control
            StartCoroutine(SelectiveControlDemo());
        }
        
        private System.Collections.IEnumerator SelectiveControlDemo()
        {
            yield return new WaitForSeconds(1f);
            
            Debug.Log("Pausing only transform animations");
            // This would pause all transform-based animations
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    // Pause specific to transform
                    TweenHelper.Pause(obj.transform);
                }
            }
            
            yield return new WaitForSeconds(1f);
            
            Debug.Log("Resuming all animations");
            foreach (var obj in demoObjects)
            {
                if (obj != null)
                {
                    TweenHelper.Resume(obj);
                }
            }
        }
        
        private GameObject GetSelectedObject()
        {
            if (demoObjects == null || demoObjects.Length == 0) return null;
            return demoObjects[selectedObjectIndex % demoObjects.Length];
        }
        
        public void NextSelectedObject()
        {
            selectedObjectIndex = (selectedObjectIndex + 1) % demoObjects.Length;
            Debug.Log($"Selected object: {GetSelectedObject()?.name}");
        }
        
        public void PreviousSelectedObject()
        {
            selectedObjectIndex = (selectedObjectIndex - 1 + demoObjects.Length) % demoObjects.Length;
            Debug.Log($"Selected object: {GetSelectedObject()?.name}");
        }
        
        private void Update()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            if (Input.GetKeyDown(KeyCode.S)) StartLongAnimations();
            if (Input.GetKeyDown(KeyCode.P)) PauseAllAnimations();
            if (Input.GetKeyDown(KeyCode.R)) ResumeAllAnimations();
            if (Input.GetKeyDown(KeyCode.K)) KillAllAnimations();
            if (Input.GetKeyDown(KeyCode.C)) CompleteAllAnimations();
            if (Input.GetKeyDown(KeyCode.I)) DemoIdControl();
            if (Input.GetKeyDown(KeyCode.D)) ShowDiagnostics();
            if (Input.GetKeyDown(KeyCode.W)) DemoRewindFunctionality();
            if (Input.GetKeyDown(KeyCode.T)) DemoControlByComponent();
            
            // Individual object control
            if (Input.GetKeyDown(KeyCode.Tab)) NextSelectedObject();
            if (Input.GetKeyDown(KeyCode.Alpha1)) PauseSelectedObject();
            if (Input.GetKeyDown(KeyCode.Alpha2)) ResumeSelectedObject();
            if (Input.GetKeyDown(KeyCode.Alpha3)) KillSelectedObject();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 220, 10, 210, 250));
            GUILayout.Label("Control Demo Keys:", GUI.skin.box);
            GUILayout.Label("S : Start animations");
            GUILayout.Label("P : Pause all");
            GUILayout.Label("R : Resume all");
            GUILayout.Label("K : Kill all");
            GUILayout.Label("C : Complete all");
            GUILayout.Label("I : ID control demo");
            GUILayout.Label("D : Show diagnostics");
            GUILayout.Label("W : Rewind demo");
            GUILayout.Label("T : Component control");
            GUILayout.Label("");
            GUILayout.Label("Individual Object:");
            GUILayout.Label("Tab : Select next object");
            GUILayout.Label("1 : Pause selected");
            GUILayout.Label("2 : Resume selected");
            GUILayout.Label("3 : Kill selected");
            
            if (GetSelectedObject() != null)
            {
                GUILayout.Label($"Selected: {GetSelectedObject().name}");
            }
            
            GUILayout.Label($"Animations running: {animationsRunning}");
            GUILayout.EndArea();
        }
    }
}
