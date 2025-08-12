using UnityEngine;
using UnityEngine.UI;
using LB.TweenHelper;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demonstrates basic TweenHelper animations: Move, Rotate, Scale, and Fade.
    /// </summary>
    public class BasicAnimationDemo : MonoBehaviour
    {
        [Header("Animation Controls")]
        [SerializeField] private Button moveButton;
        [SerializeField] private Button rotateButton;
        [SerializeField] private Button scaleButton;
        [SerializeField] private Button fadeButton;
        [SerializeField] private Button combinedButton;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private Vector3 moveOffset = new Vector3(5f, 0f, 0f);
        [SerializeField] private Vector3 rotationAmount = new Vector3(0f, 180f, 0f);
        [SerializeField] private float scaleMultiplier = 2f;
        
        private GameObject[] demoObjects;
        private int currentObjectIndex = 0;
        
        public void Initialize(GameObject[] objects)
        {
            demoObjects = objects;
            SetupButtons();
        }
        
        private void SetupButtons()
        {
            if (moveButton != null)
            {
                moveButton.onClick.RemoveAllListeners();
                moveButton.onClick.AddListener(DemoMove);
                SetButtonText(moveButton, "Move");
            }
            
            if (rotateButton != null)
            {
                rotateButton.onClick.RemoveAllListeners();
                rotateButton.onClick.AddListener(DemoRotate);
                SetButtonText(rotateButton, "Rotate");
            }
            
            if (scaleButton != null)
            {
                scaleButton.onClick.RemoveAllListeners();
                scaleButton.onClick.AddListener(DemoScale);
                SetButtonText(scaleButton, "Scale");
            }
            
            if (fadeButton != null)
            {
                fadeButton.onClick.RemoveAllListeners();
                fadeButton.onClick.AddListener(DemoFade);
                SetButtonText(fadeButton, "Fade");
            }
            
            if (combinedButton != null)
            {
                combinedButton.onClick.RemoveAllListeners();
                combinedButton.onClick.AddListener(DemoCombined);
                SetButtonText(combinedButton, "Combined");
            }
        }
        
        private void SetButtonText(Button button, string text)
        {
            var buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }
        
        public void DemoMove()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Vector3 currentPos = target.transform.position;
            Vector3 targetPos = currentPos + moveOffset;
            
            Debug.Log($"Moving {target.name} to {targetPos}");
            
            // Demonstrate different move methods
            var tween = TweenHelper.MoveTo(target.transform, targetPos, animationDuration);
            
            // Add a return journey
            tween.OnComplete(() =>
            {
                TweenHelper.MoveTo(target.transform, currentPos, animationDuration * 0.5f);
            });
        }
        
        public void DemoRotate()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Vector3 currentRotation = target.transform.eulerAngles;
            Vector3 targetRotation = currentRotation + rotationAmount;
            
            Debug.Log($"Rotating {target.name} by {rotationAmount}");
            
            // Demonstrate rotation
            TweenHelper.RotateTo(target.transform, targetRotation, animationDuration);
        }
        
        public void DemoScale()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Vector3 originalScale = target.transform.localScale;
            Vector3 targetScale = originalScale * scaleMultiplier;
            
            Debug.Log($"Scaling {target.name} to {targetScale}");
            
            // Scale up then back down
            var tween = TweenHelper.ScaleTo(target.transform, targetScale, animationDuration);
            tween.OnComplete(() =>
            {
                TweenHelper.ScaleTo(target.transform, originalScale, animationDuration * 0.5f);
            });
        }
        
        public void DemoFade()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Fading {target.name}");
            
            // Try different fade targets
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                var tween = TweenHelper.FadeOut(canvasGroup, animationDuration);
                tween.OnComplete(() => TweenHelper.FadeIn(canvasGroup, animationDuration * 0.5f));
                return;
            }
            
            var image = target.GetComponent<Image>();
            if (image != null)
            {
                var tween = TweenHelper.FadeTo(image, 0f, animationDuration);
                tween.OnComplete(() => TweenHelper.FadeTo(image, 1f, animationDuration * 0.5f));
                return;
            }
            
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var tween = TweenHelper.FadeTo(spriteRenderer, 0f, animationDuration);
                tween.OnComplete(() => TweenHelper.FadeTo(spriteRenderer, 1f, animationDuration * 0.5f));
                return;
            }
            
            Debug.LogWarning($"No fadeable component found on {target.name}");
        }
        
        public void DemoCombined()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Debug.Log($"Combined animation on {target.name}");
            
            Vector3 originalPos = target.transform.position;
            Vector3 originalScale = target.transform.localScale;
            
            // Move up while scaling and rotating
            var moveTween = TweenHelper.MoveTo(target.transform, 
                originalPos + Vector3.up * 3f, animationDuration);
            
            var scaleTween = TweenHelper.ScaleTo(target.transform, 
                originalScale * 1.5f, animationDuration);
            
            var rotateTween = TweenHelper.RotateBy(target.transform, 
                new Vector3(0f, 360f, 0f), animationDuration);
            
            // Return to original state after all complete
            moveTween.OnComplete(() =>
            {
                TweenHelper.MoveTo(target.transform, originalPos, animationDuration * 0.5f);
                TweenHelper.ScaleTo(target.transform, originalScale, animationDuration * 0.5f);
            });
        }
        
        public void DemoMoveVariations()
        {
            var target = GetNextDemoObject();
            if (target == null) return;
            
            Vector3 originalPos = target.transform.position;
            
            // Demonstrate different move methods
            Debug.Log("Move variations demo:");
            
            // 1. MoveTo
            var tween1 = TweenHelper.MoveTo(target.transform, originalPos + Vector3.right * 3f, 1f);
            
            // 2. MoveBy (chained)
            tween1.OnComplete(() =>
            {
                var tween2 = TweenHelper.MoveBy(target.transform, Vector3.up * 2f, 1f);
                
                // 3. MoveToLocal (if has parent)
                tween2.OnComplete(() =>
                {
                    if (target.transform.parent != null)
                    {
                        TweenHelper.MoveToLocal(target.transform, Vector3.zero, 1f);
                    }
                    else
                    {
                        TweenHelper.MoveTo(target.transform, originalPos, 1f);
                    }
                });
            });
        }
        
        public void DemoEasingComparison()
        {
            if (demoObjects == null || demoObjects.Length < 3) return;
            
            Debug.Log("Easing comparison demo");
            
            // Use first 3 objects to show different easings
            for (int i = 0; i < 3 && i < demoObjects.Length; i++)
            {
                var target = demoObjects[i];
                if (target == null) continue;
                
                Vector3 startPos = target.transform.position;
                Vector3 endPos = startPos + Vector3.right * 5f;
                
                TweenOptions options = TweenOptions.None;
                switch (i)
                {
                    case 0: options = options.SetEase(Ease.Linear); break;
                    case 1: options = options.SetEase(Ease.OutQuart); break;
                    case 2: options = options.SetEase(Ease.OutBounce); break;
                }
                
                var tween = TweenHelper.MoveTo(target.transform, endPos, 2f, options);
                tween.OnComplete(() => TweenHelper.MoveTo(target.transform, startPos, 1f));
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
        
        // Demo specific controls
        private void Update()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) DemoMove();
            if (Input.GetKeyDown(KeyCode.Alpha2)) DemoRotate();
            if (Input.GetKeyDown(KeyCode.Alpha3)) DemoScale();
            if (Input.GetKeyDown(KeyCode.Alpha4)) DemoFade();
            if (Input.GetKeyDown(KeyCode.Alpha5)) DemoCombined();
            if (Input.GetKeyDown(KeyCode.E)) DemoEasingComparison();
        }
        
        private void OnGUI()
        {
            if (!enabled || !gameObject.activeInHierarchy) return;
            
            GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 150));
            GUILayout.Label("Basic Animation Keys:", GUI.skin.box);
            GUILayout.Label("1 : Move");
            GUILayout.Label("2 : Rotate"); 
            GUILayout.Label("3 : Scale");
            GUILayout.Label("4 : Fade");
            GUILayout.Label("5 : Combined");
            GUILayout.Label("E : Easing comparison");
            GUILayout.EndArea();
        }
    }
}
