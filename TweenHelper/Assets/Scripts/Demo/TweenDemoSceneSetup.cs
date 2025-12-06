using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Demo scene setup with two-level dropdown system:
    /// 1. Provider dropdown (MoveAnimationDemo, etc.)
    /// 2. Sub-category dropdowns spawned based on selected provider
    /// </summary>
    public class TweenDemoSceneSetup : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Reference")]
        [SerializeField] private GameObject dropdownPrefab;
        [SerializeField] private Transform providerDropdownContainer;
        [SerializeField] private Transform subCategoryDropdownContainer;
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

        private readonly List<IDemoAnimationProvider> _providers = new();
        private readonly List<SubCategoryDropdown> _subCategoryDropdowns = new();
        private TMP_Dropdown _providerDropdown;
        private SubCategoryDropdown _activeSubCategory;
        private IDemoAnimationProvider _activeProvider;
        private GameObject[] _demoObjects;
        private Transform[] _demoTransforms;
        private Vector3[] _originalPositions;
        private Vector3[] _originalScales;
        private Quaternion[] _originalRotations;
        private bool _autoResetAfterAnimation = true;

        #endregion

        #region Nested Types

        private class SubCategoryDropdown
        {
            public string SubCategoryName;
            public TMP_Dropdown Dropdown;
            public List<DemoAnimation> Animations = new List<DemoAnimation>();
        }

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            Application.targetFrameRate = 60;

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
            CreateProviderDropdown();
            UpdateInfoText();

            Debug.Log($"[TweenDemo] Demo ready! {_demoObjects.Length} objects, {_providers.Count} providers.");
        }

        [ContextMenu("Clear Demo")]
        public void ClearDemo()
        {
            ClearSubCategoryDropdowns();
            ClearProviderDropdown();

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
            if (_activeSubCategory == null || _demoObjects == null) return;

            int index = _activeSubCategory.Dropdown.value;
            if (index < 0 || index >= _activeSubCategory.Animations.Count) return;

            var animation = _activeSubCategory.Animations[index];
            if (animation == null || animation.Execute == null) return;

            if (_autoResetAfterAnimation)
            {
                ResetAllObjects();
            }

            animation.Execute(_demoTransforms, defaultDuration);
            Debug.Log($"[TweenDemo] Playing: [{_activeSubCategory.SubCategoryName}] {animation.Name}");
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

        #region Provider Dropdown

        private void CreateProviderDropdown()
        {
            if (dropdownPrefab == null || providerDropdownContainer == null)
            {
                Debug.LogError("[TweenDemo] Dropdown prefab or provider container is not assigned!");
                return;
            }

            ClearProviderDropdown();

            // Instantiate provider dropdown
            var dropdownObj = Instantiate(dropdownPrefab, providerDropdownContainer);
            dropdownObj.name = "Dropdown_Providers";
            dropdownObj.SetActive(true);

            _providerDropdown = dropdownObj.GetComponent<TMP_Dropdown>();
            if (_providerDropdown == null)
            {
                Debug.LogError("[TweenDemo] Dropdown prefab missing TMP_Dropdown component!");
                return;
            }

            // Populate with provider names
            _providerDropdown.ClearOptions();
            var options = _providers.Select(p => new TMP_Dropdown.OptionData(p.CategoryName)).ToList();
            _providerDropdown.AddOptions(options);
            
            // Subscribe to changes
            _providerDropdown.onValueChanged.AddListener(OnProviderChanged);

            // Select first provider
            if (_providers.Count > 0)
            {
                _providerDropdown.value = 0;
                _providerDropdown.RefreshShownValue();
                
                OnProviderChanged(0);
            }
        }

        private void ClearProviderDropdown()
        {
            if (_providerDropdown != null)
            {
                Destroy(_providerDropdown.gameObject);
                _providerDropdown = null;
            }
            _activeProvider = null;
        }

        private void OnProviderChanged(int index)
        {
            if (index < 0 || index >= _providers.Count) return;

            _activeProvider = _providers[index];
            CreateSubCategoryDropdowns(_activeProvider);

            Debug.Log($"[TweenDemo] Selected provider: {_activeProvider.CategoryName}");
        }

        #endregion

        #region SubCategory Dropdowns

        private void CreateSubCategoryDropdowns(IDemoAnimationProvider provider)
        {
            Debug.Log($"[TweenDemo] CreateSubCategoryDropdowns called for: {provider.CategoryName}");

            if (dropdownPrefab == null)
            {
                Debug.LogError("[TweenDemo] dropdownPrefab is not assigned!");
                return;
            }

            if (subCategoryDropdownContainer == null)
            {
                Debug.LogError("[TweenDemo] subCategoryDropdownContainer is not assigned!");
                return;
            }

            ClearSubCategoryDropdowns();

            // Group animations by SubCategory
            var animations = provider.GetAnimations().ToList();
            Debug.Log($"[TweenDemo] Found {animations.Count} animations from provider");

            var subCategories = animations
                .GroupBy(a => a.SubCategory ?? "Other")
                .OrderBy(g => animations.FindIndex(a => a.SubCategory == g.Key))
                .ToList();

            Debug.Log($"[TweenDemo] Grouped into {subCategories.Count} sub-categories");

            foreach (var group in subCategories)
            {
                var subCategory = new SubCategoryDropdown
                {
                    SubCategoryName = group.Key,
                    Animations = group.ToList()
                };

                // Instantiate dropdown
                var dropdownObj = Instantiate(dropdownPrefab, subCategoryDropdownContainer);
                dropdownObj.name = $"Dropdown_{group.Key}";
                dropdownObj.SetActive(true);

                subCategory.Dropdown = dropdownObj.GetComponent<TMP_Dropdown>();
                if (subCategory.Dropdown == null)
                {
                    Debug.LogError("[TweenDemo] Dropdown prefab missing TMP_Dropdown component!");
                    continue;
                }

                // Populate dropdown options
                subCategory.Dropdown.ClearOptions();
                var options = subCategory.Animations.Select(a => new TMP_Dropdown.OptionData(a.Name)).ToList();
                subCategory.Dropdown.AddOptions(options);

                // Set initial value to show first animation name
                subCategory.Dropdown.value = 0;
                subCategory.Dropdown.RefreshShownValue();

                // Subscribe to selection changes
                var captured = subCategory;
                subCategory.Dropdown.onValueChanged.AddListener(_ => OnSubCategoryDropdownChanged(captured));

                _subCategoryDropdowns.Add(subCategory);
            }

            // Set first sub-category as active
            if (_subCategoryDropdowns.Count > 0)
            {
                _activeSubCategory = _subCategoryDropdowns[0];
            }

            int totalAnimations = _subCategoryDropdowns.Sum(s => s.Animations.Count);
            Debug.Log($"[TweenDemo] Created {_subCategoryDropdowns.Count} sub-category dropdowns with {totalAnimations} animations.");
        }

        private void ClearSubCategoryDropdowns()
        {
            foreach (var subCategory in _subCategoryDropdowns)
            {
                if (subCategory.Dropdown != null)
                {
                    Destroy(subCategory.Dropdown.gameObject);
                }
            }
            _subCategoryDropdowns.Clear();
            _activeSubCategory = null;
        }

        private void OnSubCategoryDropdownChanged(SubCategoryDropdown subCategory)
        {
            _activeSubCategory = subCategory;
            subCategory.Dropdown.RefreshShownValue();
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
