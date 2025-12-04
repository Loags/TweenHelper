using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;

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
        [SerializeField] private TMP_Dropdown animationDropdown;

        [Header("Animation Providers")]
        [SerializeField] private BasicAnimationDemo basicAnimationDemo;
        [SerializeField] private PresetDemo presetDemo;
        [SerializeField] private SequenceDemo sequenceDemo;
        [SerializeField] private StaggerDemo staggerDemo;
        [SerializeField] private ControlDemo controlDemo;
        [SerializeField] private AsyncDemo asyncDemo;
        [SerializeField] private OptionsDemo optionsDemo;

        [Header("Scene Generation")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private Material demoMaterial;

        [Header("Layout Settings")]
        [SerializeField] private int demoObjectsPerRow = 3;
        [SerializeField] private float objectSpacing = 3f;
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

        #endregion

        #region Provider Management

        private void CollectProviders()
        {
            _providers.Clear();

            // Add assigned providers
            if (basicAnimationDemo != null) _providers.Add(basicAnimationDemo);
            if (presetDemo != null) _providers.Add(presetDemo);
            if (sequenceDemo != null) _providers.Add(sequenceDemo);
            if (staggerDemo != null) _providers.Add(staggerDemo);
            if (controlDemo != null) _providers.Add(controlDemo);
            if (asyncDemo != null) _providers.Add(asyncDemo);
            if (optionsDemo != null) _providers.Add(optionsDemo);

            // Find any additional providers on this object or children
            var foundProviders = GetComponentsInChildren<IDemoAnimationProvider>();
            foreach (var provider in foundProviders)
            {
                if (!_providers.Contains(provider))
                {
                    _providers.Add(provider);
                }
            }

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
            _demoObjects = new GameObject[9];
            _demoTransforms = new Transform[9];
            var parentObject = new GameObject("Demo Objects");

            for (int i = 0; i < 9; i++)
            {
                int row = i / demoObjectsPerRow;
                int col = i % demoObjectsPerRow;

                Vector3 position = demoAreaCenter + new Vector3(
                    (col - 1) * objectSpacing,
                    0f,
                    (row - 1) * objectSpacing
                );

                GameObject obj = CreateDemoObject(i, position);
                obj.transform.SetParent(parentObject.transform);
                obj.name = $"DemoObject_{i:00}";

                _demoObjects[i] = obj;
                _demoTransforms[i] = obj.transform;
            }
        }

        private GameObject CreateDemoObject(int index, Vector3 position)
        {
            GameObject obj = (index % 4) switch
            {
                0 => GameObject.CreatePrimitive(PrimitiveType.Cube),
                1 => GameObject.CreatePrimitive(PrimitiveType.Sphere),
                2 => GameObject.CreatePrimitive(PrimitiveType.Cylinder),
                3 => GameObject.CreatePrimitive(PrimitiveType.Capsule),
                _ => GameObject.CreatePrimitive(PrimitiveType.Cube)
            };

            obj.transform.position = position;

            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = demoMaterial != null
                    ? new Material(demoMaterial)
                    : new Material(Shader.Find("Standard"));
                material.color = GetObjectColor(index);
                renderer.material = material;
            }

            obj.AddComponent<CanvasGroup>().alpha = 1f;
            obj.AddComponent<TweenLifecycleTracker>();

            return obj;
        }

        private Color GetObjectColor(int index)
        {
            Color[] colors =
            {
                Color.red, Color.green, Color.blue, Color.yellow,
                Color.cyan, Color.magenta, new Color(1f, 0.5f, 0f),
                new Color(0.5f, 0f, 1f), new Color(0f, 1f, 0.5f)
            };
            return colors[index % colors.Length];
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
