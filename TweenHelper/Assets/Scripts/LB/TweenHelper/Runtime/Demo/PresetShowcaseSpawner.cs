using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Spawns objects for each registered animation preset in a family-cluster layout.
    /// Related presets (e.g., all "Pop*" presets) form small visual clusters arranged in a grid.
    /// Attach to any GameObject and use context menu or run at Start.
    /// </summary>
    public class PresetShowcaseSpawner : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance for access by PresetConsole.
        /// </summary>
        public static PresetShowcaseSpawner Instance { get; private set; }
        [Header("Cluster Layout")]
        [SerializeField] private int familiesPerRow = 4;
        [SerializeField] private float intraClusterSpacing = 6f;
        [SerializeField] private float interClusterGapX = 15f;
        [SerializeField] private float interClusterGapZ = 12f;
        [SerializeField] private Vector3 startPosition = new Vector3(-8, 0, 0);

        [Header("Object Settings")]
        [SerializeField] private PrimitiveType objectType = PrimitiveType.Cube;
        [SerializeField] private Vector3 objectScale = Vector3.one;
        [SerializeField] private Material objectMaterial;
        [SerializeField] private Color objectColor = new Color(0.3f, 0.6f, 1f, 1f);
        [Tooltip("Creates a transparent-capable material at runtime if no material is assigned")]
        [SerializeField] private bool useTransparentMaterial = true;

        [Header("Ground")]
        [SerializeField] private bool spawnGround = true;
        [SerializeField] private Material groundMaterial;
        [SerializeField] private float groundPadding = 120f;
        [SerializeField] private float groundYOffset = -0.5f;
        [SerializeField] private float groundTiling = 4f;

        [Header("Camera Setup")]
        [SerializeField] private bool setupCamera = true;
        [SerializeField] private Vector3 cameraPosition = new Vector3(0, 5, -10);
        [SerializeField] private Vector3 cameraRotation = new Vector3(25, 0, 0);

        [Header("Options")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool clearExistingOnSpawn = true;

        private readonly List<GameObject> _spawnedObjects = new List<GameObject>();
        private readonly Dictionary<string, GameObject> _presetObjects = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, string> _presetFamilies = new Dictionary<string, string>();
        private List<ITweenPreset> _allPresets;
        private Transform _container;
        private GameObject _groundObject;

        /// <summary>
        /// Order in which families appear. Families not listed sort alphabetically after these.
        /// </summary>
        private static readonly string[] FamilyOrder =
        {
            "Pop",
            "Slide",
            "Wobble",
            "Spin",
            "Bounce",
            "Shake",
            "Nudge",
            "Pulse",
            "Punch",
            "Sway",
            "Orbit",
            "Pendulum",
            "Nod",
            "Jitter",
            "Launch",
            "LaunchUp",
            "LaunchDown",
            "LaunchLeft",
            "LaunchRight",
            "Fade",
            "Recoil",
            "RecoilForward",
            "RecoilBack",
            "Drop",
            "Flip",
            "Spiral",
            "Breathe",
            "Squash",
            "Heartbeat",
            "Float",
            "Tilt",
            "ZigZag",
            "Misc",
        };

        private class PresetFamilyCluster
        {
            public string Name;
            public List<List<ITweenPreset>> Rows;
            public int MaxColumnsInRow;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
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
            SetupConsole();
        }

        private void SetupResetManager()
        {
            if (AnimationResetManager.Instance == null)
            {
                var managerGO = new GameObject("AnimationResetManager");
                managerGO.AddComponent<AnimationResetManager>();
            }
        }

        private void SetupConsole()
        {
            var consoleGO = new GameObject("PresetConsole");
            consoleGO.transform.SetParent(transform);
            consoleGO.AddComponent<PresetConsole>();
        }

        [ContextMenu("Spawn All Preset Objects")]
        public void SpawnAll()
        {
            if (clearExistingOnSpawn)
            {
                ClearSpawned();
            }

            TweenPresetRegistry.ScanForCodePresets();

            _allPresets = TweenPresetRegistry.Presets.ToList();
            if (_allPresets.Count == 0)
            {
                Debug.LogWarning("PresetShowcaseSpawner: No presets registered. Make sure presets are auto-registered.");
                return;
            }

            // Build family lookup
            _presetFamilies.Clear();
            foreach (var preset in _allPresets)
            {
                _presetFamilies[preset.PresetName] = GetPresetFamily(preset);
            }

            var families = GroupPresetsByFamily(_allPresets);

            Debug.Log($"PresetShowcaseSpawner: Spawning {_allPresets.Count} preset objects across {families.Count} families...");

            var container = new GameObject("PresetShowcase");
            container.transform.SetParent(transform);
            container.transform.localPosition = Vector3.zero;
            _container = container.transform;

            // Track layout bounds for ground plane
            float maxX = 0f;
            float maxZ = 0f;

            // Arrange families in a grid-of-clusters
            int familyCol = 0;
            float rowOffsetZ = 0f;
            float rowMaxDepth = 0f;
            float currentX = 0f;

            for (int f = 0; f < families.Count; f++)
            {
                var family = families[f];

                float clusterWidth = (family.MaxColumnsInRow - 1) * intraClusterSpacing;
                float clusterDepth = (family.Rows.Count - 1) * intraClusterSpacing;

                var clusterOrigin = startPosition + new Vector3(currentX, 0f, rowOffsetZ);

                // Spawn preset objects in sub-row layout
                for (int r = 0; r < family.Rows.Count; r++)
                {
                    var subRow = family.Rows[r];
                    for (int c = 0; c < subRow.Count; c++)
                    {
                        var position = clusterOrigin + new Vector3(c * intraClusterSpacing, 0f, r * intraClusterSpacing);
                        var obj = SpawnPresetObject(subRow[c], position, container.transform);
                        _spawnedObjects.Add(obj);
                        _presetObjects[subRow[c].PresetName] = obj;

                        maxX = Mathf.Max(maxX, position.x);
                        maxZ = Mathf.Max(maxZ, position.z);
                    }
                }

                rowMaxDepth = Mathf.Max(rowMaxDepth, clusterDepth);

                familyCol++;
                currentX += clusterWidth + interClusterGapX;

                // Wrap to next row of clusters
                if (familyCol >= familiesPerRow)
                {
                    familyCol = 0;
                    currentX = 0f;
                    rowOffsetZ += rowMaxDepth + interClusterGapZ;
                    rowMaxDepth = 0f;
                }
            }

            // Account for the last partial row
            float totalDepth = rowOffsetZ + rowMaxDepth;

            if (spawnGround)
            {
                _groundObject = SpawnGroundPlane(container.transform, maxX - startPosition.x, totalDepth);
            }

            Debug.Log($"PresetShowcaseSpawner: Spawned {_spawnedObjects.Count} objects.");
        }

        private string GetPresetFamily(ITweenPreset preset)
        {
            return LB.TweenHelper.PresetFamilyClassifier.GetFamilyName(preset.PresetName);
        }

        private List<PresetFamilyCluster> GroupPresetsByFamily(List<ITweenPreset> presets)
        {
            var groups = presets
                .GroupBy(GetPresetFamily)
                .Select(g =>
                {
                    var rows = GroupIntoSubRows(g.Key, g.ToList());
                    int maxCols = rows.Max(r => r.Count);
                    return new PresetFamilyCluster
                    {
                        Name = g.Key,
                        Rows = rows,
                        MaxColumnsInRow = maxCols
                    };
                })
                .OrderByDescending(c => c.Rows.Sum(r => r.Count))
                .ThenBy(c => c.Name)
                .ToList();

            return groups;
        }

        private int GetFamilyOrderIndex(string family)
        {
            var index = Array.IndexOf(FamilyOrder, family);
            return index >= 0 ? index : int.MaxValue;
        }

        private List<List<ITweenPreset>> GroupIntoSubRows(string family, List<ITweenPreset> presets)
        {
            var grouped = presets
                .GroupBy(p => GetSubRowKey(family, p.PresetName))
                .OrderBy(g => GetSubRowOrder(g.Key))
                .ThenBy(g => g.Key)
                .Select(g => g.OrderBy(p => GetColumnOrder(family, p.PresetName)).ThenBy(p => p.PresetName).ToList())
                .ToList();

            // Post-process: if all rows are singletons and count > 6, re-chunk into rows of 3
            if (grouped.Count > 6 && grouped.All(r => r.Count == 1))
            {
                var flat = grouped.SelectMany(r => r).ToList();
                grouped = new List<List<ITweenPreset>>();
                for (int i = 0; i < flat.Count; i += 3)
                {
                    grouped.Add(flat.Skip(i).Take(3).ToList());
                }
            }

            return grouped;
        }

        private static string GetSubRowKey(string family, string presetName)
        {
            return LB.TweenHelper.PresetVariantParser.Parse(presetName).SubRowKey;
        }

        private static int GetSubRowOrder(string rowKey)
        {
            return LB.TweenHelper.PresetVariantParser.GetSubRowSortScore(rowKey);
        }

        private static int GetColumnOrder(string family, string presetName)
        {
            return LB.TweenHelper.PresetVariantParser.GetColumnSortScore(presetName);
        }

        private GameObject SpawnPresetObject(ITweenPreset preset, Vector3 position, Transform parent)
        {
            var obj = GameObject.CreatePrimitive(objectType);
            obj.name = $"Preset_{preset.PresetName}";
            obj.transform.SetParent(parent);
            obj.transform.localPosition = position;
            obj.transform.localScale = objectScale;

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

                if (renderer.material != null)
                {
                    renderer.material.color = objectColor;
                }
            }

            var collider = obj.GetComponent<Collider>();
            if (collider == null)
            {
                obj.AddComponent<BoxCollider>();
            }

            var display = obj.AddComponent<AnimationPresetDisplay>();
            display.Setup(preset.PresetName, preset.Description);

            return obj;
        }

        private GameObject SpawnGroundPlane(Transform parent, float totalWidth, float totalDepth)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
            ground.name = "Ground";
            ground.transform.SetParent(parent);

            ground.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            float width = totalWidth + groundPadding * 2f;
            float depth = totalDepth + groundPadding * 2f;
            ground.transform.localScale = new Vector3(width, depth, 1f);

            float centerX = startPosition.x + totalWidth * 0.5f;
            float centerZ = startPosition.z + totalDepth * 0.5f;
            ground.transform.localPosition = new Vector3(centerX, groundYOffset, centerZ);

            var renderer = ground.GetComponent<Renderer>();
            if (renderer != null && groundMaterial != null)
            {
                renderer.material = new Material(groundMaterial);
                renderer.material.mainTextureScale = new Vector2(width / groundTiling, depth / groundTiling);
            }

            _spawnedObjects.Add(ground);
            return ground;
        }

        /// <summary>
        /// Creates a material that supports transparency for fade animations.
        /// Uses URP Lit shader if available, falls back to Standard shader.
        /// </summary>
        private Material CreateTransparentMaterial()
        {
            Material mat = null;

            var urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit != null)
            {
                mat = new Material(urpLit);
                mat.SetFloat("_Surface", 1);
                mat.SetFloat("_Blend", 0);
                mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)RenderQueue.Transparent;
            }
            else
            {
                var standard = Shader.Find("Standard");
                if (standard != null)
                {
                    mat = new Material(standard);
                    mat.SetFloat("_Mode", 3);
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

        /// <summary>
        /// Returns the family name for a given preset name, or null if unknown.
        /// </summary>
        public string GetPresetFamilyName(string presetName)
        {
            return _presetFamilies.TryGetValue(presetName, out var family) ? family : null;
        }

        /// <summary>
        /// Returns all distinct family names, sorted alphabetically.
        /// </summary>
        public List<string> GetAllFamilyNames()
        {
            return _presetFamilies.Values.Distinct().OrderBy(f => f).ToList();
        }

        /// <summary>
        /// Returns the number of currently visible (active) preset objects.
        /// </summary>
        public int GetVisibleCount()
        {
            int count = 0;
            foreach (var kvp in _presetObjects)
            {
                if (kvp.Value != null && kvp.Value.activeSelf)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the total number of preset objects.
        /// </summary>
        public int GetTotalCount()
        {
            return _presetObjects.Count;
        }

        /// <summary>
        /// Returns all preset names, sorted alphabetically.
        /// </summary>
        public List<string> GetAllPresetNames()
        {
            return _presetObjects.Keys.OrderBy(n => n).ToList();
        }

        /// <summary>
        /// Filters visible presets and re-lays out the remaining ones with no gaps.
        /// </summary>
        public void RelayoutWithFilter(Func<string, bool> visibilityFilter)
        {
            if (_allPresets == null) return;

            // Apply visibility
            var visiblePresets = new List<ITweenPreset>();
            foreach (var preset in _allPresets)
            {
                var isVisible = visibilityFilter(preset.PresetName);
                if (_presetObjects.TryGetValue(preset.PresetName, out var obj) && obj != null)
                {
                    obj.SetActive(isVisible);
                    if (isVisible) visiblePresets.Add(preset);
                }
            }

            // Re-layout visible presets
            var families = GroupPresetsByFamily(visiblePresets);

            float maxX = 0f;
            float maxZ = 0f;
            int familyCol = 0;
            float rowOffsetZ = 0f;
            float rowMaxDepth = 0f;
            float currentX = 0f;

            for (int f = 0; f < families.Count; f++)
            {
                var family = families[f];
                float clusterWidth = (family.MaxColumnsInRow - 1) * intraClusterSpacing;
                float clusterDepth = (family.Rows.Count - 1) * intraClusterSpacing;
                var clusterOrigin = startPosition + new Vector3(currentX, 0f, rowOffsetZ);

                for (int r = 0; r < family.Rows.Count; r++)
                {
                    var subRow = family.Rows[r];
                    for (int c = 0; c < subRow.Count; c++)
                    {
                        var position = clusterOrigin + new Vector3(c * intraClusterSpacing, 0f, r * intraClusterSpacing);
                        if (_presetObjects.TryGetValue(subRow[c].PresetName, out var obj) && obj != null)
                        {
                            var display = obj.GetComponent<AnimationPresetDisplay>();
                            if (display != null) display.ResetAnimation();

                            obj.transform.localPosition = position;
                            obj.transform.localScale = objectScale;
                            obj.transform.localRotation = Quaternion.identity;

                            if (display != null) display.SaveOriginalState();
                        }

                        maxX = Mathf.Max(maxX, position.x);
                        maxZ = Mathf.Max(maxZ, position.z);
                    }
                }

                rowMaxDepth = Mathf.Max(rowMaxDepth, clusterDepth);
                familyCol++;
                currentX += clusterWidth + interClusterGapX;

                if (familyCol >= familiesPerRow)
                {
                    familyCol = 0;
                    currentX = 0f;
                    rowOffsetZ += rowMaxDepth + interClusterGapZ;
                    rowMaxDepth = 0f;
                }
            }

            // Resize ground plane
            float totalDepth = rowOffsetZ + rowMaxDepth;
            if (_groundObject != null)
            {
                float totalWidth = maxX - startPosition.x;
                float width = totalWidth + groundPadding * 2f;
                float depth = totalDepth + groundPadding * 2f;
                _groundObject.transform.localScale = new Vector3(width, depth, 1f);
                float centerX = startPosition.x + totalWidth * 0.5f;
                float centerZ = startPosition.z + totalDepth * 0.5f;
                _groundObject.transform.localPosition = new Vector3(centerX, groundYOffset, centerZ);

                var renderer = _groundObject.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.mainTextureScale = new Vector2(width / groundTiling, depth / groundTiling);
                }
            }
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
            _presetObjects.Clear();
            _presetFamilies.Clear();
            _groundObject = null;

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

            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.eulerAngles = cameraRotation;

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
                int presetCount = 20;
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                for (int i = 0; i < presetCount; i++)
                {
                    int row = i / 3;
                    int col = i % 3;
                    var position = transform.position + startPosition + new Vector3(col * intraClusterSpacing, 0, row * intraClusterSpacing);
                    Gizmos.DrawWireCube(position, objectScale);
                }
            }
            else
            {
                var families = GroupPresetsByFamily(presets);

                int familyCol = 0;
                float rowOffsetZ = 0f;
                float rowMaxDepth = 0f;
                float currentX = 0f;

                Gizmos.color = new Color(0, 1, 0, 0.3f);

                for (int f = 0; f < families.Count; f++)
                {
                    var family = families[f];

                    float clusterWidth = (family.MaxColumnsInRow - 1) * intraClusterSpacing;
                    float clusterDepth = (family.Rows.Count - 1) * intraClusterSpacing;

                    var clusterOrigin = transform.position + startPosition + new Vector3(currentX, 0f, rowOffsetZ);

                    for (int r = 0; r < family.Rows.Count; r++)
                    {
                        var subRow = family.Rows[r];
                        for (int c = 0; c < subRow.Count; c++)
                        {
                            var position = clusterOrigin + new Vector3(c * intraClusterSpacing, 0f, r * intraClusterSpacing);
                            Gizmos.DrawWireCube(position, objectScale);
                        }
                    }

                    // Draw cluster bounding box
                    Gizmos.color = new Color(1f, 1f, 0f, 0.15f);
                    var clusterCenter = clusterOrigin + new Vector3(clusterWidth * 0.5f, 0f, clusterDepth * 0.5f);
                    Gizmos.DrawWireCube(clusterCenter, new Vector3(clusterWidth + 1f, 0.1f, clusterDepth + 1f));
                    Gizmos.color = new Color(0, 1, 0, 0.3f);

                    rowMaxDepth = Mathf.Max(rowMaxDepth, clusterDepth);

                    familyCol++;
                    currentX += clusterWidth + interClusterGapX;

                    if (familyCol >= familiesPerRow)
                    {
                        familyCol = 0;
                        currentX = 0f;
                        rowOffsetZ += rowMaxDepth + interClusterGapZ;
                        rowMaxDepth = 0f;
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
