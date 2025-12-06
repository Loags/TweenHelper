using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demo scene setup that creates demo objects and uses a TMP_Dropdown for animation selection.
    /// Integrates with IDemoAnimationProvider classes for animation data.
    /// </summary>
    public class TweenDemoSceneSetup : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Reference")]
        [SerializeField] private GameObject animationDropdownObject;
        [SerializeField] private Transform dropdownContainerTransform;
        [SerializeField] private TextMeshProUGUI infoText;

        [Header("Animation Providers")]
        [SerializeField] private MoveAnimationDemo moveAnimationDemo;

        [Header("Scene Generation")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private Material demoMaterial;

        [Header("Layout Settings")]
        [SerializeField] private Vector3 demoAreaCenter = Vector3.zero;

        [Header("Animation Settings")]
        [SerializeField] private float defaultDuration = 1f;

        #endregion

        #region Private Fields

        private readonly List<IDemoAnimationProvider> _providers = new List<IDemoAnimationProvider>();
        private readonly List<DemoAnimation> _animations = new List<DemoAnimation>();
        private GameObject[] _demoObjects;
        private Transform[] _demoTransforms;
        private Vector3[] _originalPositions;
        private Vector3[] _originalScales;
        private Quaternion[] _originalRotations;
        private bool _autoResetAfterAnimation = true;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupDemo();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetAllObjects();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlaySelectedAnimation();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleAutoReset();
            }
        }

        #endregion

        #region Public Methods

        [ContextMenu("Setup Demo")]
        public void SetupDemo()
        {
            Debug.Log("[TweenDemo] Setting up demo scene...");

            CollectProviders();
            CreateDemoObjects();
            InitializeProviders();
            CacheOriginalTransforms();
            PopulateDropdown();
            UpdateInfoText();

            Debug.Log($"[TweenDemo] Demo ready! {_demoObjects.Length} objects, {_animations.Count} animations.");
        }

        [ContextMenu("Clear Demo")]
        public void ClearDemo()
        {
            var demoParent = GameObject.Find("Demo Objects");
            if (demoParent != null)
            {
                DestroyImmediate(demoParent);
            }
            _demoObjects = null;
            _demoTransforms = null;
            Debug.Log("[TweenDemo] Demo cleared.");
        }

        public void PlaySelectedAnimation()
        {
            if (animationDropdown == null || _demoObjects == null) return;

            int index = animationDropdown.value;
            if (index < 0 || index >= _animations.Count) return;

            var animation = _animations[index];
            if (animation == null || animation.Execute == null) return;

            // Skip headers
            if (animation.Name.StartsWith("──")) return;

            if (_autoResetAfterAnimation)
            {
                ResetAllObjects();
            }

            animation.Execute(_demoTransforms, defaultDuration);
            Debug.Log($"[TweenDemo] Playing: {animation.Name.Trim()}");
        }

        public void ResetAllObjects()
        {
            if (_demoObjects == null) return;

            TweenController.KillAll();

            for (int i = 0; i < _demoObjects.Length; i++)
            {
                if (_demoObjects[i] != null)
                {
                    _demoObjects[i].transform.position = _originalPositions[i];
                    _demoObjects[i].transform.localScale = _originalScales[i];
                    _demoObjects[i].transform.rotation = _originalRotations[i];

                    var cg = _demoObjects[i].GetComponent<CanvasGroup>();
                    if (cg != null) cg.alpha = 1f;
                }
            }

            Debug.Log("[TweenDemo] All objects reset.");
        }

        public void ToggleAutoReset()
        {
            _autoResetAfterAnimation = !_autoResetAfterAnimation;
            UpdateInfoText();
            Debug.Log($"[TweenDemo] Auto-reset: {(_autoResetAfterAnimation ? "ON" : "OFF")}");
        }

        private void UpdateInfoText()
        {
            if (infoText == null) return;

            string autoResetStatus = _autoResetAfterAnimation ? "<color=green>ON</color>" : "<color=red>OFF</color>";
            infoText.text = "KEYBOARD CONTROLS\n\n" +
                           "<b>SPACE</b>  —  Play Selected Animation\n" +
                           "<b>R</b>  —  Reset All Objects\n" +
                           $"<b>T</b>  —  Toggle Auto-Reset [{autoResetStatus}]";
        }

        #endregion

        #region Provider Management

        private void CollectProviders()
        {
            _providers.Clear();

            if (moveAnimationDemo != null) _providers.Add(moveAnimationDemo);

            Debug.Log($"[TweenDemo] Found {_providers.Count} animation providers.");
        }

        private void InitializeProviders()
        {
            foreach (var provider in _providers)
            {
                provider.Initialize(_demoObjects);
            }
        }

        #endregion

        #region Dropdown Population

        private void PopulateDropdown()
        {
            if (animationDropdown == null)
            {
                Debug.LogError("[TweenDemo] AnimationDropdown is not assigned!");
                return;
            }

            _animations.Clear();
            animationDropdown.ClearOptions();

            // Collect animations from all providers, grouped by category
            foreach (var provider in _providers)
            {
                // Add category header
                _animations.Add(new DemoAnimation
                {
                    Name = $"── {provider.CategoryName.ToUpper()} ──",
                    Category = provider.CategoryName,
                    Execute = null
                });

                // Add animations from this provider
                foreach (var animation in provider.GetAnimations())
                {
                    _animations.Add(new DemoAnimation
                    {
                        Name = $"   {animation.Name}",
                        Category = animation.Category,
                        Execute = animation.Execute,
                        RequiresMultipleObjects = animation.RequiresMultipleObjects
                    });
                }
            }

            // Convert to dropdown options
            var options = _animations.Select(a => new TMP_Dropdown.OptionData(a.Name)).ToList();
            animationDropdown.AddOptions(options);
            animationDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            // Select first non-header item
            for (int i = 0; i < _animations.Count; i++)
            {
                if (!_animations[i].Name.StartsWith("──"))
                {
                    animationDropdown.value = i;
                    break;
                }
            }
        }

        private void OnDropdownValueChanged(int index)
        {
            if (index < 0 || index >= _animations.Count) return;

            // Skip to next non-header if header selected
            if (_animations[index].Name.StartsWith("──"))
            {
                for (int i = index + 1; i < _animations.Count; i++)
                {
                    if (!_animations[i].Name.StartsWith("──"))
                    {
                        animationDropdown.value = i;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Demo Objects

        private void CreateDemoObjects()
        {
            _demoObjects = new GameObject[1];
            _demoTransforms = new Transform[1];
            var parentObject = new GameObject("Demo Objects");

            GameObject obj = CreateDemoObject(demoAreaCenter);
            obj.transform.SetParent(parentObject.transform);
            obj.name = "DemoObject_00";

            _demoObjects[0] = obj;
            _demoTransforms[0] = obj.transform;
        }

        private GameObject CreateDemoObject(Vector3 position)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = position;

            if (obj.TryGetComponent<Renderer>(out var renderer))
                renderer.material = demoMaterial;

            obj.AddComponent<CanvasGroup>().alpha = 1f;
            obj.AddComponent<TweenLifecycleTracker>();

            return obj;
        }

        private void CacheOriginalTransforms()
        {
            _originalPositions = new Vector3[_demoObjects.Length];
            _originalScales = new Vector3[_demoObjects.Length];
            _originalRotations = new Quaternion[_demoObjects.Length];

            for (int i = 0; i < _demoObjects.Length; i++)
            {
                _originalPositions[i] = _demoObjects[i].transform.position;
                _originalScales[i] = _demoObjects[i].transform.localScale;
                _originalRotations[i] = _demoObjects[i].transform.rotation;
            }
        }

        #endregion
    }
}
