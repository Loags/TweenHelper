using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Spawns objects for each registered animation preset in a grid layout.
    /// Attach to any GameObject and use context menu or run at Start.
    /// </summary>
    public class PresetShowcaseSpawner : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private int columns = 5;
        [SerializeField] private float spacingX = 4f;
        [SerializeField] private float spacingZ = 4f;
        [SerializeField] private Vector3 startPosition = new Vector3(-8, 0, 0);

        [Header("Object Settings")]
        [SerializeField] private PrimitiveType objectType = PrimitiveType.Cube;
        [SerializeField] private Vector3 objectScale = Vector3.one;
        [SerializeField] private Material objectMaterial;
        [SerializeField] private Color objectColor = new Color(0.3f, 0.6f, 1f, 1f);
        [Tooltip("Creates a transparent-capable material at runtime if no material is assigned")]
        [SerializeField] private bool useTransparentMaterial = true;

        [Header("Canvas Settings")]
        [SerializeField] private Vector3 labelOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] private float canvasScale = 0.01f;

        [Header("Camera Setup")]
        [SerializeField] private bool setupCamera = true;
        [SerializeField] private Vector3 cameraPosition = new Vector3(0, 5, -10);
        [SerializeField] private Vector3 cameraRotation = new Vector3(25, 0, 0);

        [Header("Options")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool clearExistingOnSpawn = true;

        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();
        private Material _transparentMaterial;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnAll();
            }

            if (setupCamera)
            {
                SetupFlyCamera();
            }

            SetupResetManager();
        }

        private void SetupResetManager()
        {
            if (AnimationResetManager.Instance == null)
            {
                var managerGO = new GameObject("AnimationResetManager");
                managerGO.AddComponent<AnimationResetManager>();
            }
        }

        [ContextMenu("Spawn All Preset Objects")]
        public void SpawnAll()
        {
            if (clearExistingOnSpawn)
            {
                ClearSpawned();
            }

            // Get all registered presets and sort by category
            var presets = TweenPresetRegistry.Presets.ToList();
            if (presets.Count == 0)
            {
                Debug.LogWarning("PresetShowcaseSpawner: No presets registered. Make sure presets are auto-registered.");
                return;
            }

            // Sort presets by category for logical grouping
            var sortedPresets = SortPresetsByCategory(presets);

            Debug.Log($"PresetShowcaseSpawner: Spawning {sortedPresets.Count} preset objects...");

            // Create parent container
            var container = new GameObject("PresetShowcase");
            container.transform.SetParent(transform);
            container.transform.localPosition = Vector3.zero;

            // Spawn objects in grid
            for (int i = 0; i < sortedPresets.Count; i++)
            {
                var preset = sortedPresets[i];
                int row = i / columns;
                int col = i % columns;

                var position = startPosition + new Vector3(col * spacingX, 0, row * spacingZ);
                var obj = SpawnPresetObject(preset, position, container.transform);
                _spawnedObjects.Add(obj);
            }

            Debug.Log($"PresetShowcaseSpawner: Spawned {_spawnedObjects.Count} objects.");
        }

        /// <summary>
        /// Sorts presets by category for logical visual grouping.
        /// Order: Scale → Position → Fade → Rotation → Combined
        /// </summary>
        private List<ITweenPreset> SortPresetsByCategory(List<ITweenPreset> presets)
        {
            // Define category order and patterns
            var categoryOrder = new (string Category, string[] Patterns)[]
            {
                ("Scale", new[] { "PopIn", "PopOut", "Punch", "Bounce" }),
                ("Position", new[] { "Shake", "SlideIn" }),
                ("Fade", new[] { "FadeIn", "FadeOut" }),
                ("Spin", new[] { "SpinX", "SpinY", "SpinZ" }),
                ("Wobble", new[] { "WobbleX", "WobbleY", "WobbleZ" }),
                ("Combined", new[] { "PopInFade", "Attention" }),
            };

            var sorted = new List<ITweenPreset>();
            var remaining = new List<ITweenPreset>(presets);

            foreach (var (category, patterns) in categoryOrder)
            {
                foreach (var pattern in patterns)
                {
                    var matches = remaining
                        .Where(p => p.PresetName.StartsWith(pattern, System.StringComparison.OrdinalIgnoreCase))
                        .OrderBy(p => p.PresetName)
                        .ToList();

                    foreach (var match in matches)
                    {
                        sorted.Add(match);
                        remaining.Remove(match);
                    }
                }
            }

            // Add any remaining presets at the end (custom user presets)
            sorted.AddRange(remaining.OrderBy(p => p.PresetName));

            return sorted;
        }

        private GameObject SpawnPresetObject(ITweenPreset preset, Vector3 position, Transform parent)
        {
            // Create primitive
            var obj = GameObject.CreatePrimitive(objectType);
            obj.name = $"Preset_{preset.PresetName}";
            obj.transform.SetParent(parent);
            obj.transform.localPosition = position;
            obj.transform.localScale = objectScale;

            // Apply material
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (objectMaterial != null)
                {
                    renderer.material = new Material(objectMaterial);
                }
                else if (useTransparentMaterial)
                {
                    renderer.material = CreateTransparentMaterial();
                }

                // Set color
                if (renderer.material != null)
                {
                    renderer.material.color = objectColor;
                }
            }

            // Ensure collider exists for clicking
            var collider = obj.GetComponent<Collider>();
            if (collider == null)
            {
                obj.AddComponent<BoxCollider>();
            }

            // Add display component
            var display = obj.AddComponent<AnimationPresetDisplay>();
            display.Setup(preset.PresetName, preset.Description);

            return obj;
        }

        /// <summary>
        /// Creates a material that supports transparency for fade animations.
        /// Uses URP Lit shader if available, falls back to Standard shader.
        /// </summary>
        private Material CreateTransparentMaterial()
        {
            Material mat = null;

            // Try URP Lit shader first (Unity 6 default)
            var urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit != null)
            {
                mat = new Material(urpLit);
                // Set to transparent mode
                mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
                mat.SetFloat("_Blend", 0);   // 0 = Alpha, 1 = Premultiply, 2 = Additive, 3 = Multiply
                mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)RenderQueue.Transparent;
            }
            else
            {
                // Fallback to Standard shader (Built-in RP)
                var standard = Shader.Find("Standard");
                if (standard != null)
                {
                    mat = new Material(standard);
                    // Set to transparent mode
                    mat.SetFloat("_Mode", 3); // Transparent
                    mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = (int)RenderQueue.Transparent;
                }
            }

            return mat;
        }

        [ContextMenu("Clear Spawned Objects")]
        public void ClearSpawned()
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(obj);
                    }
                    else
                    {
                        DestroyImmediate(obj);
                    }
                }
            }
            _spawnedObjects.Clear();

            // Also destroy the container
            var container = transform.Find("PresetShowcase");
            if (container != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(container.gameObject);
                }
                else
                {
                    DestroyImmediate(container.gameObject);
                }
            }
        }

        [ContextMenu("Setup Fly Camera")]
        public void SetupFlyCamera()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("PresetShowcaseSpawner: No main camera found.");
                return;
            }

            // Position camera
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.eulerAngles = cameraRotation;

            // Add FlyCamera if not present
            if (mainCamera.GetComponent<FlyCamera>() == null)
            {
                mainCamera.gameObject.AddComponent<FlyCamera>();
                Debug.Log("PresetShowcaseSpawner: Added FlyCamera to main camera.");
            }
        }

        [ContextMenu("List All Presets")]
        public void ListAllPresets()
        {
            var presets = TweenPresetRegistry.Presets.ToList();
            Debug.Log($"Registered Presets ({presets.Count}):");
            foreach (var preset in presets)
            {
                Debug.Log($"  - {preset.PresetName}: {preset.Description}");
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw grid preview
            Gizmos.color = new Color(0, 1, 0, 0.3f);

            int presetCount = 20; // Approximate count for preview (includes axis variants)
            for (int i = 0; i < presetCount; i++)
            {
                int row = i / columns;
                int col = i % columns;
                var position = transform.position + startPosition + new Vector3(col * spacingX, 0, row * spacingZ);
                Gizmos.DrawWireCube(position, objectScale);
            }

            // Draw camera position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraPosition, 0.5f);
            Gizmos.DrawLine(cameraPosition, cameraPosition + Quaternion.Euler(cameraRotation) * Vector3.forward * 3f);
        }
    }
}
