using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

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

        [Header("Category Layout")]
        [SerializeField] private float categoryGapZ = 6f;
        [SerializeField] private bool spawnCategoryLabels = true;
        [SerializeField] private Vector3 categoryLabelOffset = new Vector3(0f, 1.6f, 0f);
        [SerializeField] private float categoryLabelLeftPadding = 2f;
        [SerializeField] private Color categoryLabelColor = new Color(1f, 1f, 1f, 0.9f);

        [Header("Object Settings")]
        [SerializeField] private PrimitiveType objectType = PrimitiveType.Cube;
        [SerializeField] private Vector3 objectScale = Vector3.one;
        [SerializeField] private Material objectMaterial;
        [SerializeField] private Color objectColor = new Color(0.3f, 0.6f, 1f, 1f);
        [Tooltip("Creates a transparent-capable material at runtime if no material is assigned")]
        [SerializeField] private bool useTransparentMaterial = true;

        [Header("Camera Setup")]
        [SerializeField] private bool setupCamera = true;
        [SerializeField] private Vector3 cameraPosition = new Vector3(0, 5, -10);
        [SerializeField] private Vector3 cameraRotation = new Vector3(25, 0, 0);

        [Header("Options")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool clearExistingOnSpawn = true;

        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();
        private static readonly string[] CategoryOrder =
        {
            PresetCategories.Base,
            PresetCategories.Scale,
            PresetCategories.Movement,
            PresetCategories.Fade,
            PresetCategories.Rotation,
            PresetCategories.Combined
        };

        private class PresetCategoryGroup
        {
            public string Name { get; }
            public List<ITweenPreset> Presets { get; }

            public PresetCategoryGroup(string name, List<ITweenPreset> presets)
            {
                Name = name;
                Presets = presets;
            }
        }

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

            TweenPresetRegistry.ScanForCodePresets();

            // Get all registered presets and sort by category
            var presets = TweenPresetRegistry.Presets.ToList();
            if (presets.Count == 0)
            {
                Debug.LogWarning("PresetShowcaseSpawner: No presets registered. Make sure presets are auto-registered.");
                return;
            }

            // Group presets by category for spatial separation
            var groupedPresets = GroupPresetsByCategory(presets);

            Debug.Log($"PresetShowcaseSpawner: Spawning {presets.Count} preset objects across {groupedPresets.Count} categories...");

            // Create parent container
            var container = new GameObject("PresetShowcase");
            container.transform.SetParent(transform);
            container.transform.localPosition = Vector3.zero;

            // Spawn per-category blocks offset in Z so groups stay visually separated
            float categoryOffsetZ = 0f;
            foreach (var group in groupedPresets)
            {
                var groupStart = startPosition + new Vector3(0f, 0f, categoryOffsetZ);

                var rows = Mathf.Max(1, Mathf.CeilToInt(group.Presets.Count / (float)columns));
                var categoryCenterOffset = new Vector3(0f, 0f, (rows - 1) * spacingZ * 0.5f);

                if (spawnCategoryLabels)
                {
                    var labelPosition = GetCategoryLabelPosition(groupStart, rows, group.Presets.Count);
                    SpawnCategoryLabel(group.Name, labelPosition + categoryCenterOffset, container.transform);
                }

                for (int i = 0; i < group.Presets.Count; i++)
                {
                    var preset = group.Presets[i];
                    int row = i / columns;
                    int col = i % columns;

                    var position = groupStart + new Vector3(col * spacingX, 0, row * spacingZ);
                    var obj = SpawnPresetObject(preset, position, container.transform);
                    _spawnedObjects.Add(obj);
                }

                // Advance the offset based on the depth of this category block
                categoryOffsetZ += rows * spacingZ + categoryGapZ;
            }

            Debug.Log($"PresetShowcaseSpawner: Spawned {_spawnedObjects.Count} objects.");
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

        private List<PresetCategoryGroup> GroupPresetsByCategory(List<ITweenPreset> presets)
        {
            return presets
                .GroupBy(GetCategoryName)
                .Select(group => new PresetCategoryGroup(group.Key, group.OrderBy(p => p.PresetName).ToList()))
                .OrderBy(g => GetCategoryOrderIndex(g.Name))
                .ThenBy(g => g.Name)
                .ToList();
        }

        private string GetCategoryName(ITweenPreset preset)
        {
            if (preset is ICategorizedTweenPreset categorized && !string.IsNullOrWhiteSpace(categorized.Category))
            {
                return categorized.Category;
            }

            return PresetCategories.Base;
        }

        private int GetCategoryOrderIndex(string category)
        {
            var index = Array.IndexOf(CategoryOrder, category);
            return index >= 0 ? index : int.MaxValue;
        }

        private void SpawnCategoryLabel(string category, Vector3 position, Transform parent)
        {
            var labelGO = new GameObject($"Category_{category}_Label");
            labelGO.transform.SetParent(parent);
            labelGO.transform.localPosition = position;
            labelGO.transform.localRotation = Quaternion.identity;

            var canvas = labelGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var scaler = labelGO.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10f;

            var rect = labelGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(320f, 80f);
            rect.localScale = Vector3.one * 0.01f;

            // Background panel similar to animation labels
            var panelGO = new GameObject("Panel");
            panelGO.transform.SetParent(labelGO.transform, false);
            var panelRect = panelGO.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            var panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.7f);

            var layout = panelGO.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 0;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            var textGO = new GameObject("CategoryText");
            textGO.transform.SetParent(panelGO.transform, false);
            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = category;
            text.fontSize = 28;
            text.alignment = TextAlignmentOptions.Left;
            text.color = categoryLabelColor;
            text.fontStyle = FontStyles.Bold;

            var textLayout = textGO.AddComponent<LayoutElement>();
            textLayout.preferredHeight = 40;

            // Billboard so it faces the camera
            labelGO.AddComponent<BillboardCanvas>();
        }

        private Vector3 GetCategoryLabelPosition(Vector3 groupStart, int rows, int presetCount)
        {
            var colsInGroup = Mathf.Max(1, Mathf.Min(columns, presetCount));
            var groupWidth = (colsInGroup - 1) * spacingX + objectScale.x;
            var leftOfGroup = groupStart.x - (groupWidth * 0.5f) - categoryLabelLeftPadding;
            var zCenter = (rows - 1) * spacingZ * 0.5f;
            return new Vector3(leftOfGroup, groupStart.y, groupStart.z + zCenter) + categoryLabelOffset;
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
            TweenPresetRegistry.ScanForCodePresets();
            var presets = TweenPresetRegistry.Presets.ToList();

            if (presets.Count == 0)
            {
                int presetCount = 20; // Approximate fallback preview
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                for (int i = 0; i < presetCount; i++)
                {
                    int row = i / columns;
                    int col = i % columns;
                    var position = transform.position + startPosition + new Vector3(col * spacingX, 0, row * spacingZ);
                    Gizmos.DrawWireCube(position, objectScale);
                }
            }
            else
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                var grouped = GroupPresetsByCategory(presets);
                float categoryOffsetZ = 0f;

                foreach (var group in grouped)
                {
                    for (int i = 0; i < group.Presets.Count; i++)
                    {
                        int row = i / columns;
                        int col = i % columns;
                        var position = transform.position + startPosition + new Vector3(col * spacingX, 0, row * spacingZ + categoryOffsetZ);
                        Gizmos.DrawWireCube(position, objectScale);
                    }

                    var rows = Mathf.Max(1, Mathf.CeilToInt(group.Presets.Count / (float)columns));
                    categoryOffsetZ += rows * spacingZ + categoryGapZ;
                }

                if (spawnCategoryLabels)
                {
                    categoryOffsetZ = 0f;
                    Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
                    foreach (var group in grouped)
                    {
                        var rows = Mathf.Max(1, Mathf.CeilToInt(group.Presets.Count / (float)columns));
                        var categoryCenterOffset = new Vector3(0f, 0f, (rows - 1) * spacingZ * 0.5f);
                        var basePos = transform.position + startPosition + new Vector3(0f, 0f, categoryOffsetZ);
                        var position = GetCategoryLabelPosition(basePos, rows, group.Presets.Count) + categoryCenterOffset;
                        Gizmos.DrawWireCube(position, new Vector3(1f, 1f, 0.1f));
                        categoryOffsetZ += rows * spacingZ + categoryGapZ;
                    }
                }
            }

            // Draw camera position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraPosition, 0.5f);
            Gizmos.DrawLine(cameraPosition, cameraPosition + Quaternion.Euler(cameraRotation) * Vector3.forward * 3f);
        }
    }
}
