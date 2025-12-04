using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace LB.TweenHelper.Demo
{
    /// <summary>
    /// Automatically sets up the complete TweenHelper demo scene with dropdown-based UI
    /// for selecting and triggering all available tween animations organized by category.
    /// </summary>
    public class TweenDemoSceneSetup : MonoBehaviour
    {
        #region Animation Categories

        /// <summary>
        /// Defines an animation that can be triggered from the demo UI.
        /// </summary>
        public class AnimationEntry
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public System.Action<Transform, float> Execute { get; set; }
        }

        /// <summary>
        /// Defines a category of animations.
        /// </summary>
        public class AnimationCategory
        {
            public string Name { get; set; }
            public List<AnimationEntry> Animations { get; set; } = new List<AnimationEntry>();
        }

        #endregion

        #region Serialized Fields

        [Header("Scene Generation")]
        [SerializeField] private bool autoSetupOnStart = false;
        [SerializeField] private Material demoMaterial;

        [Header("Layout Settings")]
        [SerializeField] private int demoObjectsPerRow = 3;
        [SerializeField] private float objectSpacing = 3f;
        [SerializeField] private Vector3 demoAreaCenter = Vector3.zero;

        [Header("Animation Settings")]
        [SerializeField] private float defaultDuration = 1f;
        [SerializeField] private float staggerDelay = 0.1f;

        #endregion

        #region Private Fields

        private List<AnimationCategory> _categories;
        private GameObject[] _demoObjects;
        private Transform[] _demoTransforms;
        private Vector3[] _originalPositions;
        private Vector3[] _originalScales;
        private Quaternion[] _originalRotations;

        // UI References
        private Dropdown _categoryDropdown;
        private Dropdown _animationDropdown;
        private Button _playButton;
        private Button _resetButton;
        private Button _playAllButton;
        private Toggle _applyToAllToggle;
        private Slider _durationSlider;
        private Text _durationText;
        private Text _statusText;
        private Text _descriptionText;

        private int _selectedObjectIndex = 0;

        #endregion

        #region Setup Methods

        [ContextMenu("Setup Complete Demo Scene")]
        public void SetupCompleteScene()
        {
            Debug.Log("Setting up TweenHelper demo scene...");

            // Initialize animation categories
            InitializeAnimationCategories();

            // 1. Create demo objects
            _demoObjects = CreateDemoObjects();
            CacheOriginalTransforms();

            // 2. Create UI canvas and controls
            var uiCanvas = CreateUICanvas();

            // 3. Setup camera for optimal viewing
            SetupCamera();

            // 4. Add lighting if needed
            SetupLighting();

            // 5. Wire up UI events
            WireUpUIEvents();

            Debug.Log($"Demo scene setup complete! Created {_demoObjects.Length} demo objects with dropdown UI.");
        }

        private void InitializeAnimationCategories()
        {
            _categories = new List<AnimationCategory>
            {
                CreateMovementCategory(),
                CreateRotationCategory(),
                CreateScaleCategory(),
                CreateFadeCategory(),
                CreatePatternsCategory(),
                CreatePresetsCategory(),
                CreateStaggerCategory(),
                CreateSequenceCategory()
            };
        }

        #endregion

        #region Animation Category Definitions

        private AnimationCategory CreateMovementCategory()
        {
            return new AnimationCategory
            {
                Name = "Movement",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "MoveTo (Up)",
                        Description = "Moves the object up by 2 units",
                        Execute = (t, d) => TweenHelper.MoveTo(t, t.position + Vector3.up * 2f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "MoveTo (Forward)",
                        Description = "Moves the object forward by 2 units",
                        Execute = (t, d) => TweenHelper.MoveTo(t, t.position + Vector3.forward * 2f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "MoveTo (Right)",
                        Description = "Moves the object right by 2 units",
                        Execute = (t, d) => TweenHelper.MoveTo(t, t.position + Vector3.right * 2f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "MoveBy (Offset)",
                        Description = "Moves the object by offset (1, 1, 0)",
                        Execute = (t, d) => TweenHelper.MoveBy(t, new Vector3(1f, 1f, 0f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "MoveToLocal",
                        Description = "Moves to local position (0, 2, 0)",
                        Execute = (t, d) => TweenHelper.MoveToLocal(t, new Vector3(0f, 2f, 0f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "MoveTo (Circle)",
                        Description = "Moves in a circular pattern using sequence",
                        Execute = (t, d) => {
                            var start = t.position;
                            TweenHelper.CreateSequence(t)
                                .Move(t, start + Vector3.right * 2f, d * 0.25f)
                                .Move(t, start + Vector3.right * 2f + Vector3.up * 2f, d * 0.25f)
                                .Move(t, start + Vector3.up * 2f, d * 0.25f)
                                .Move(t, start, d * 0.25f)
                                .Play();
                        }
                    }
                }
            };
        }

        private AnimationCategory CreateRotationCategory()
        {
            return new AnimationCategory
            {
                Name = "Rotation",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "RotateTo (90° Y)",
                        Description = "Rotates 90 degrees on Y axis",
                        Execute = (t, d) => TweenHelper.RotateTo(t, new Vector3(0f, 90f, 0f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "RotateTo (180° Y)",
                        Description = "Rotates 180 degrees on Y axis",
                        Execute = (t, d) => TweenHelper.RotateTo(t, new Vector3(0f, 180f, 0f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "RotateTo (360° Y)",
                        Description = "Full rotation on Y axis",
                        Execute = (t, d) => TweenHelper.RotateTo(t, new Vector3(0f, 360f, 0f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "RotateBy (45° X)",
                        Description = "Rotates by 45 degrees on X axis",
                        Execute = (t, d) => TweenHelper.RotateBy(t, new Vector3(45f, 0f, 0f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "RotateBy (45° Z)",
                        Description = "Rotates by 45 degrees on Z axis",
                        Execute = (t, d) => TweenHelper.RotateBy(t, new Vector3(0f, 0f, 45f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "LookAt (Camera)",
                        Description = "Rotates to look at the camera",
                        Execute = (t, d) => {
                            var cam = Camera.main;
                            if (cam != null)
                                TweenHelper.LookAt(t, cam.transform.position, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "LookAt (Origin)",
                        Description = "Rotates to look at world origin",
                        Execute = (t, d) => TweenHelper.LookAt(t, Vector3.zero, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Spin (Continuous)",
                        Description = "Continuous 720° spin with loops",
                        Execute = (t, d) => TweenHelper.RotateTo(t, new Vector3(0f, 720f, 0f), d,
                            TweenOptions.WithLoops(2, LoopType.Restart))
                    }
                }
            };
        }

        private AnimationCategory CreateScaleCategory()
        {
            return new AnimationCategory
            {
                Name = "Scale",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "ScaleTo (2x)",
                        Description = "Scales uniformly to 2x size",
                        Execute = (t, d) => TweenHelper.ScaleTo(t, 2f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "ScaleTo (0.5x)",
                        Description = "Scales uniformly to half size",
                        Execute = (t, d) => TweenHelper.ScaleTo(t, 0.5f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "ScaleTo (Vector3)",
                        Description = "Scales to (2, 1, 0.5)",
                        Execute = (t, d) => TweenHelper.ScaleTo(t, new Vector3(2f, 1f, 0.5f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "ScaleBy (1.5x)",
                        Description = "Multiplies current scale by 1.5",
                        Execute = (t, d) => TweenHelper.ScaleBy(t, 1.5f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "ScaleBy (Vector3)",
                        Description = "Multiplies scale by (2, 1, 1)",
                        Execute = (t, d) => TweenHelper.ScaleBy(t, new Vector3(2f, 1f, 1f), d)
                    },
                    new AnimationEntry
                    {
                        Name = "ScaleTo (Zero)",
                        Description = "Scales down to zero",
                        Execute = (t, d) => TweenHelper.ScaleTo(t, Vector3.zero, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Pulse Scale",
                        Description = "Pulsing scale with Yoyo loop",
                        Execute = (t, d) => TweenHelper.ScaleTo(t, 1.3f, d * 0.5f,
                            TweenOptions.WithLoops(4, LoopType.Yoyo).SetEase(Ease.InOutSine))
                    }
                }
            };
        }

        private AnimationCategory CreateFadeCategory()
        {
            return new AnimationCategory
            {
                Name = "Fade",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "FadeOut (Alpha 0)",
                        Description = "Fades CanvasGroup to fully transparent",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) TweenHelper.FadeTo(cg, 0f, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "FadeIn (Alpha 1)",
                        Description = "Fades CanvasGroup to fully opaque",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) TweenHelper.FadeTo(cg, 1f, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "FadeTo (50%)",
                        Description = "Fades CanvasGroup to 50% alpha",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) TweenHelper.FadeTo(cg, 0.5f, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "FadeIn (Preset)",
                        Description = "Uses FadeIn convenience method",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) { cg.alpha = 0f; TweenHelper.FadeIn(cg, d); }
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "FadeOut (Preset)",
                        Description = "Uses FadeOut convenience method",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) TweenHelper.FadeOut(cg, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Fade Pulse",
                        Description = "Pulsing fade with Yoyo loop",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) TweenHelper.FadeTo(cg, 0.3f, d * 0.5f,
                                TweenOptions.WithLoops(4, LoopType.Yoyo));
                        }
                    }
                }
            };
        }

        private AnimationCategory CreatePatternsCategory()
        {
            return new AnimationCategory
            {
                Name = "Patterns",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "PopIn",
                        Description = "Scales from zero with bounce (OutBack ease)",
                        Execute = (t, d) => {
                            t.localScale = Vector3.zero;
                            TweenHelper.PopIn(t, Vector3.one, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "PopOut",
                        Description = "Scales to zero with anticipation (InBack ease)",
                        Execute = (t, d) => TweenHelper.PopOut(t, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Bounce",
                        Description = "Bounces scale up then back to original",
                        Execute = (t, d) => TweenHelper.Bounce(t, 1.2f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Bounce (Large)",
                        Description = "Large bounce with 1.5x scale",
                        Execute = (t, d) => TweenHelper.Bounce(t, 1.5f, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Shake",
                        Description = "Random position shake effect",
                        Execute = (t, d) => TweenHelper.Shake(t, 0.5f, d, 10)
                    },
                    new AnimationEntry
                    {
                        Name = "Shake (Strong)",
                        Description = "Strong shake with high vibrato",
                        Execute = (t, d) => TweenHelper.Shake(t, 1f, d, 20)
                    },
                    new AnimationEntry
                    {
                        Name = "Shake (Subtle)",
                        Description = "Subtle shake effect",
                        Execute = (t, d) => TweenHelper.Shake(t, 0.2f, d, 5)
                    }
                }
            };
        }

        private AnimationCategory CreatePresetsCategory()
        {
            return new AnimationCategory
            {
                Name = "Presets",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "Preset: PopIn",
                        Description = "Built-in PopIn preset from registry",
                        Execute = (t, d) => {
                            t.localScale = Vector3.zero;
                            TweenHelper.PlayPreset("PopIn", t, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Preset: PopOut",
                        Description = "Built-in PopOut preset from registry",
                        Execute = (t, d) => TweenHelper.PlayPreset("PopOut", t, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Preset: Bounce",
                        Description = "Built-in Bounce preset from registry",
                        Execute = (t, d) => TweenHelper.PlayPreset("Bounce", t, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Preset: Shake",
                        Description = "Built-in Shake preset from registry",
                        Execute = (t, d) => TweenHelper.PlayPreset("Shake", t, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Preset: FadeIn",
                        Description = "Built-in FadeIn preset from registry",
                        Execute = (t, d) => {
                            var cg = t.GetComponent<CanvasGroup>();
                            if (cg != null) cg.alpha = 0f;
                            TweenHelper.PlayPreset("FadeIn", t, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Preset: FadeOut",
                        Description = "Built-in FadeOut preset from registry",
                        Execute = (t, d) => TweenHelper.PlayPreset("FadeOut", t, d)
                    }
                }
            };
        }

        private AnimationCategory CreateStaggerCategory()
        {
            return new AnimationCategory
            {
                Name = "Stagger",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "Stagger Move (Up)",
                        Description = "All objects move up with staggered delay",
                        Execute = (t, d) => TweenHelper.StaggerMoveTo(_demoTransforms,
                            Vector3.up * 2f, staggerDelay, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Move (Center)",
                        Description = "All objects move to center with stagger",
                        Execute = (t, d) => TweenHelper.StaggerMoveTo(_demoTransforms,
                            demoAreaCenter, staggerDelay, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Scale (2x)",
                        Description = "All objects scale up with staggered delay",
                        Execute = (t, d) => TweenHelper.StaggerScaleTo(_demoTransforms,
                            2f, staggerDelay, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Scale (0.5x)",
                        Description = "All objects scale down with staggered delay",
                        Execute = (t, d) => TweenHelper.StaggerScaleTo(_demoTransforms,
                            0.5f, staggerDelay, d)
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Preset (PopIn)",
                        Description = "PopIn preset on all objects with stagger",
                        Execute = (t, d) => {
                            foreach (var obj in _demoObjects)
                                obj.transform.localScale = Vector3.zero;
                            var gameObjects = new List<GameObject>(_demoObjects);
                            TweenHelper.StaggerPreset("PopIn", gameObjects, staggerDelay, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Preset (Bounce)",
                        Description = "Bounce preset on all objects with stagger",
                        Execute = (t, d) => {
                            var gameObjects = new List<GameObject>(_demoObjects);
                            TweenHelper.StaggerPreset("Bounce", gameObjects, staggerDelay, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Fade (Out)",
                        Description = "All objects fade out with stagger",
                        Execute = (t, d) => {
                            var canvasGroups = new List<Component>();
                            foreach (var obj in _demoObjects)
                            {
                                var cg = obj.GetComponent<CanvasGroup>();
                                if (cg != null) canvasGroups.Add(cg);
                            }
                            TweenHelper.StaggerFadeTo(canvasGroups, 0f, staggerDelay, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Stagger Fade (In)",
                        Description = "All objects fade in with stagger",
                        Execute = (t, d) => {
                            var canvasGroups = new List<Component>();
                            foreach (var obj in _demoObjects)
                            {
                                var cg = obj.GetComponent<CanvasGroup>();
                                if (cg != null) { cg.alpha = 0f; canvasGroups.Add(cg); }
                            }
                            TweenHelper.StaggerFadeTo(canvasGroups, 1f, staggerDelay, d);
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Wave Effect",
                        Description = "Wave-like movement across all objects",
                        Execute = (t, d) => {
                            var seq = DOTween.Sequence();
                            for (int i = 0; i < _demoTransforms.Length; i++)
                            {
                                var tr = _demoTransforms[i];
                                var originalY = _originalPositions[i].y;
                                var moveTween = tr.DOMoveY(originalY + 1.5f, d * 0.5f)
                                    .SetEase(Ease.InOutSine)
                                    .SetLoops(2, LoopType.Yoyo);
                                seq.Insert(i * staggerDelay, moveTween);
                            }
                        }
                    }
                }
            };
        }

        private AnimationCategory CreateSequenceCategory()
        {
            return new AnimationCategory
            {
                Name = "Sequences",
                Animations = new List<AnimationEntry>
                {
                    new AnimationEntry
                    {
                        Name = "Simple Sequence",
                        Description = "Move → Scale → Rotate in sequence",
                        Execute = (t, d) => TweenHelper.CreateSequence(t)
                            .Move(t, t.position + Vector3.up * 2f, d * 0.33f)
                            .Scale(t, 1.5f, d * 0.33f)
                            .Rotate(t, new Vector3(0f, 180f, 0f), d * 0.33f)
                            .Play()
                    },
                    new AnimationEntry
                    {
                        Name = "Parallel Sequence",
                        Description = "Move + Scale + Rotate simultaneously",
                        Execute = (t, d) => TweenHelper.CreateSequence(t)
                            .Move(t, t.position + Vector3.up * 2f, d)
                            .JoinScale(t, new Vector3(1.5f,1.5f,1.5f), d)
                            .JoinRotate(t, new Vector3(0f, 360f, 0f), d)
                            .Play()
                    },
                    new AnimationEntry
                    {
                        Name = "Complex Sequence",
                        Description = "Multi-step animation with mixed timing",
                        Execute = (t, d) => {
                            var startPos = t.position;
                            TweenHelper.CreateSequence(t)
                                .Scale(t, 0.5f, d * 0.2f)
                                .Move(t, startPos + Vector3.up * 3f, d * 0.3f)
                                .JoinRotate(t, new Vector3(0f, 360f, 0f), d * 0.3f)
                                .Scale(t, 1.5f, d * 0.2f)
                                .Delay(d * 0.1f)
                                .Move(t, startPos, d * 0.2f)
                                .Play();
                        }
                    },
                    new AnimationEntry
                    {
                        Name = "Looped Sequence",
                        Description = "Sequence with loop options",
                        Execute = (t, d) => TweenHelper.CreateSequence(t)
                            .Move(t, t.position + Vector3.right * 2f, d * 0.5f)
                            .Move(t, t.position, d * 0.5f)
                            .Build(TweenOptions.WithLoops(3, LoopType.Restart))
                    },
                    new AnimationEntry
                    {
                        Name = "Yoyo Sequence",
                        Description = "Sequence that plays forward then backward",
                        Execute = (t, d) => TweenHelper.CreateSequence(t)
                            .Move(t, t.position + Vector3.up * 2f, d * 0.5f)
                            .Scale(t, 1.5f, d * 0.5f)
                            .Build(TweenOptions.WithLoops(2, LoopType.Yoyo))
                    },
                    new AnimationEntry
                    {
                        Name = "Callback Sequence",
                        Description = "Sequence with callback actions",
                        Execute = (t, d) => TweenHelper.CreateSequence(t)
                            .Call(() => Debug.Log("Starting animation..."))
                            .Move(t, t.position + Vector3.up * 2f, d * 0.4f)
                            .Call(() => Debug.Log("Move complete!"))
                            .Scale(t, 1.5f, d * 0.4f)
                            .Call(() => Debug.Log("Scale complete!"))
                            .Delay(d * 0.2f)
                            .Call(() => Debug.Log("Animation finished!"))
                            .Play()
                    },
                    new AnimationEntry
                    {
                        Name = "Preset Sequence",
                        Description = "Sequence using presets",
                        Execute = (t, d) => {
                            t.localScale = Vector3.zero;
                            TweenHelper.CreateSequence(t)
                                .Preset("PopIn", t.gameObject, d * 0.4f)
                                .Delay(d * 0.2f)
                                .Preset("Bounce", t.gameObject, d * 0.4f)
                                .Play();
                        }
                    }
                }
            };
        }

        #endregion

        #region Demo Object Creation

        private GameObject[] CreateDemoObjects()
        {
            var demoObjects = new GameObject[9]; // 3x3 grid
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

                demoObjects[i] = obj;
                _demoTransforms[i] = obj.transform;
            }

            return demoObjects;
        }

        private GameObject CreateDemoObject(int index, Vector3 position)
        {
            GameObject obj;

            switch (index % 4)
            {
                case 0:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case 1:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                case 2:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;
                case 3:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    break;
                default:
                    obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
            }

            obj.transform.position = position;

            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material;
                if (demoMaterial != null)
                {
                    material = new Material(demoMaterial);
                }
                else
                {
                    material = new Material(Shader.Find("Standard"));
                }
                material.color = GetObjectColor(index);
                renderer.material = material;
            }

            // Add CanvasGroup for fade demonstrations
            var canvasGroup = obj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;

            // Add TweenLifecycleTracker for safety
            obj.AddComponent<TweenLifecycleTracker>();

            return obj;
        }

        private Color GetObjectColor(int index)
        {
            Color[] colors = {
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

        #region UI Creation

        private GameObject CreateUICanvas()
        {
            var canvasObj = new GameObject("Demo UI Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Create main control panel
            CreateControlPanel(canvasObj.transform);

            // Create status panel
            CreateStatusPanel(canvasObj.transform);

            return canvasObj;
        }

        private void CreateControlPanel(Transform canvasTransform)
        {
            // Main panel
            var panelObj = CreateUIPanel("Control Panel", canvasTransform);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.65f);
            panelRect.anchorMax = new Vector2(0.35f, 1f);
            panelRect.offsetMin = new Vector2(10, 10);
            panelRect.offsetMax = new Vector2(-10, -10);

            var verticalLayout = panelObj.AddComponent<VerticalLayoutGroup>();
            verticalLayout.padding = new RectOffset(15, 15, 15, 15);
            verticalLayout.spacing = 10f;
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.childForceExpandHeight = false;

            // Title
            var titleText = CreateUIText("Title", "TweenHelper Demo", panelObj.transform, 24);
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(0, 35);
            var titleTextComp = titleText.GetComponent<Text>();
            titleTextComp.alignment = TextAnchor.MiddleCenter;
            titleTextComp.fontStyle = FontStyle.Bold;

            // Category dropdown row
            var categoryRow = CreateUIPanel("Category Row", panelObj.transform);
            categoryRow.GetComponent<Image>().color = Color.clear;
            var categoryRowRect = categoryRow.GetComponent<RectTransform>();
            categoryRowRect.sizeDelta = new Vector2(0, 35);

            var categoryLabel = CreateUIText("Category Label", "Category:", categoryRow.transform, 14);
            var categoryLabelRect = categoryLabel.GetComponent<RectTransform>();
            categoryLabelRect.anchorMin = new Vector2(0, 0);
            categoryLabelRect.anchorMax = new Vector2(0.3f, 1);
            categoryLabelRect.offsetMin = Vector2.zero;
            categoryLabelRect.offsetMax = Vector2.zero;

            _categoryDropdown = CreateDropdown("Category Dropdown", categoryRow.transform);
            var categoryDropdownRect = _categoryDropdown.GetComponent<RectTransform>();
            categoryDropdownRect.anchorMin = new Vector2(0.32f, 0);
            categoryDropdownRect.anchorMax = new Vector2(1, 1);
            categoryDropdownRect.offsetMin = Vector2.zero;
            categoryDropdownRect.offsetMax = Vector2.zero;

            // Animation dropdown row
            var animationRow = CreateUIPanel("Animation Row", panelObj.transform);
            animationRow.GetComponent<Image>().color = Color.clear;
            var animationRowRect = animationRow.GetComponent<RectTransform>();
            animationRowRect.sizeDelta = new Vector2(0, 35);

            var animationLabel = CreateUIText("Animation Label", "Animation:", animationRow.transform, 14);
            var animationLabelRect = animationLabel.GetComponent<RectTransform>();
            animationLabelRect.anchorMin = new Vector2(0, 0);
            animationLabelRect.anchorMax = new Vector2(0.3f, 1);
            animationLabelRect.offsetMin = Vector2.zero;
            animationLabelRect.offsetMax = Vector2.zero;

            _animationDropdown = CreateDropdown("Animation Dropdown", animationRow.transform);
            var animationDropdownRect = _animationDropdown.GetComponent<RectTransform>();
            animationDropdownRect.anchorMin = new Vector2(0.32f, 0);
            animationDropdownRect.anchorMax = new Vector2(1, 1);
            animationDropdownRect.offsetMin = Vector2.zero;
            animationDropdownRect.offsetMax = Vector2.zero;

            // Description text
            _descriptionText = CreateUIText("Description", "Select an animation to see its description.", panelObj.transform, 12).GetComponent<Text>();
            _descriptionText.fontStyle = FontStyle.Italic;
            _descriptionText.color = new Color(0.8f, 0.8f, 0.8f);
            var descRect = _descriptionText.GetComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(0, 40);

            // Duration slider row
            var durationRow = CreateUIPanel("Duration Row", panelObj.transform);
            durationRow.GetComponent<Image>().color = Color.clear;
            var durationRowRect = durationRow.GetComponent<RectTransform>();
            durationRowRect.sizeDelta = new Vector2(0, 30);

            _durationText = CreateUIText("Duration Label", $"Duration: {defaultDuration:F1}s", durationRow.transform, 14).GetComponent<Text>();
            var durationLabelRect = _durationText.GetComponent<RectTransform>();
            durationLabelRect.anchorMin = new Vector2(0, 0);
            durationLabelRect.anchorMax = new Vector2(0.35f, 1);
            durationLabelRect.offsetMin = Vector2.zero;
            durationLabelRect.offsetMax = Vector2.zero;

            _durationSlider = CreateSlider("Duration Slider", durationRow.transform);
            var durationSliderRect = _durationSlider.GetComponent<RectTransform>();
            durationSliderRect.anchorMin = new Vector2(0.37f, 0.2f);
            durationSliderRect.anchorMax = new Vector2(1, 0.8f);
            durationSliderRect.offsetMin = Vector2.zero;
            durationSliderRect.offsetMax = Vector2.zero;
            _durationSlider.minValue = 0.1f;
            _durationSlider.maxValue = 3f;
            _durationSlider.value = defaultDuration;

            // Apply to all toggle row
            var toggleRow = CreateUIPanel("Toggle Row", panelObj.transform);
            toggleRow.GetComponent<Image>().color = Color.clear;
            var toggleRowRect = toggleRow.GetComponent<RectTransform>();
            toggleRowRect.sizeDelta = new Vector2(0, 30);

            _applyToAllToggle = CreateToggle("Apply To All", toggleRow.transform);
            var toggleText = _applyToAllToggle.GetComponentInChildren<Text>();
            if (toggleText != null) toggleText.text = "Apply to all objects";

            // Buttons row
            var buttonsRow = CreateUIPanel("Buttons Row", panelObj.transform);
            buttonsRow.GetComponent<Image>().color = Color.clear;
            var buttonsRowRect = buttonsRow.GetComponent<RectTransform>();
            buttonsRowRect.sizeDelta = new Vector2(0, 40);

            var buttonsLayout = buttonsRow.AddComponent<HorizontalLayoutGroup>();
            buttonsLayout.spacing = 10f;
            buttonsLayout.childControlWidth = true;
            buttonsLayout.childControlHeight = true;
            buttonsLayout.childForceExpandWidth = true;

            _playButton = CreateUIButton("Play Button", "Play", buttonsRow.transform).GetComponent<Button>();
            _resetButton = CreateUIButton("Reset Button", "Reset", buttonsRow.transform).GetComponent<Button>();
            _playAllButton = CreateUIButton("Play All Button", "Play All", buttonsRow.transform).GetComponent<Button>();
        }

        private void CreateStatusPanel(Transform canvasTransform)
        {
            var panelObj = CreateUIPanel("Status Panel", canvasTransform);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(1f, 0.08f);
            panelRect.offsetMin = new Vector2(10, 10);
            panelRect.offsetMax = new Vector2(-10, -5);

            _statusText = CreateUIText("Status", "Ready. Select a category and animation, then click Play.", panelObj.transform, 14).GetComponent<Text>();
            var statusRect = _statusText.GetComponent<RectTransform>();
            statusRect.anchorMin = Vector2.zero;
            statusRect.anchorMax = Vector2.one;
            statusRect.offsetMin = new Vector2(15, 5);
            statusRect.offsetMax = new Vector2(-15, -5);
            _statusText.alignment = TextAnchor.MiddleLeft;
        }

        #endregion

        #region UI Helper Methods

        private GameObject CreateUIPanel(string name, Transform parent)
        {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent);

            var rectTransform = panelObj.AddComponent<RectTransform>();
            var image = panelObj.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

            return panelObj;
        }

        private GameObject CreateUIText(string name, string textContent, Transform parent, int fontSize = 16)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent);

            var rectTransform = textObj.AddComponent<RectTransform>();
            var text = textObj.AddComponent<Text>();
            text.text = textContent;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;

            return textObj;
        }

        private GameObject CreateUIButton(string name, string buttonText, Transform parent)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            var rectTransform = buttonObj.AddComponent<RectTransform>();
            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.4f, 0.6f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.highlightedColor = new Color(0.3f, 0.5f, 0.7f, 1f);
            colors.pressedColor = new Color(0.15f, 0.3f, 0.45f, 1f);
            button.colors = colors;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textObj.AddComponent<Text>();
            text.text = buttonText;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;

            return buttonObj;
        }

        private Dropdown CreateDropdown(string name, Transform parent)
        {
            var dropdownObj = new GameObject(name);
            dropdownObj.transform.SetParent(parent);

            var rectTransform = dropdownObj.AddComponent<RectTransform>();
            var image = dropdownObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            var dropdown = dropdownObj.AddComponent<Dropdown>();

            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(dropdownObj.transform);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10, 0);
            labelRect.offsetMax = new Vector2(-25, 0);
            var labelText = labelObj.AddComponent<Text>();
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAnchor.MiddleLeft;
            dropdown.captionText = labelText;

            // Arrow
            var arrowObj = new GameObject("Arrow");
            arrowObj.transform.SetParent(dropdownObj.transform);
            var arrowRect = arrowObj.AddComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.sizeDelta = new Vector2(20, 20);
            arrowRect.anchoredPosition = new Vector2(-15, 0);
            var arrowImage = arrowObj.AddComponent<Image>();
            arrowImage.color = Color.white;

            // Template
            var templateObj = new GameObject("Template");
            templateObj.transform.SetParent(dropdownObj.transform);
            var templateRect = templateObj.AddComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0, 0);
            templateRect.anchorMax = new Vector2(1, 0);
            templateRect.pivot = new Vector2(0.5f, 1);
            templateRect.anchoredPosition = Vector2.zero;
            templateRect.sizeDelta = new Vector2(0, 150);
            var templateImage = templateObj.AddComponent<Image>();
            templateImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            var scrollRect = templateObj.AddComponent<ScrollRect>();
            templateObj.SetActive(false);

            // Viewport
            var viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(templateObj.transform);
            var viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;
            viewportObj.AddComponent<Image>();
            scrollRect.viewport = viewportRect;

            // Content
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 28);
            scrollRect.content = contentRect;

            // Item
            var itemObj = new GameObject("Item");
            itemObj.transform.SetParent(contentObj.transform);
            var itemRect = itemObj.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 0.5f);
            itemRect.anchorMax = new Vector2(1, 0.5f);
            itemRect.sizeDelta = new Vector2(0, 28);
            var itemToggle = itemObj.AddComponent<Toggle>();

            // Item Background
            var itemBgObj = new GameObject("Item Background");
            itemBgObj.transform.SetParent(itemObj.transform);
            var itemBgRect = itemBgObj.AddComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.offsetMin = Vector2.zero;
            itemBgRect.offsetMax = Vector2.zero;
            var itemBgImage = itemBgObj.AddComponent<Image>();
            itemBgImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
            itemToggle.targetGraphic = itemBgImage;

            // Item Checkmark
            var checkmarkObj = new GameObject("Item Checkmark");
            checkmarkObj.transform.SetParent(itemObj.transform);
            var checkmarkRect = checkmarkObj.AddComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0, 0.5f);
            checkmarkRect.sizeDelta = new Vector2(20, 20);
            checkmarkRect.anchoredPosition = new Vector2(10, 0);
            var checkmarkImage = checkmarkObj.AddComponent<Image>();
            checkmarkImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            itemToggle.graphic = checkmarkImage;

            // Item Label
            var itemLabelObj = new GameObject("Item Label");
            itemLabelObj.transform.SetParent(itemObj.transform);
            var itemLabelRect = itemLabelObj.AddComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.offsetMin = new Vector2(25, 0);
            itemLabelRect.offsetMax = new Vector2(-10, 0);
            var itemLabelText = itemLabelObj.AddComponent<Text>();
            itemLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            itemLabelText.fontSize = 14;
            itemLabelText.color = Color.white;
            itemLabelText.alignment = TextAnchor.MiddleLeft;
            dropdown.itemText = itemLabelText;

            dropdown.template = templateRect;

            return dropdown;
        }

        private Slider CreateSlider(string name, Transform parent)
        {
            var sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent);

            var rectTransform = sliderObj.AddComponent<RectTransform>();
            var slider = sliderObj.AddComponent<Slider>();

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.25f);
            bgRect.anchorMax = new Vector2(1, 0.75f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // Fill Area
            var fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform);
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            // Fill
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.3f, 0.5f, 0.7f, 1f);
            slider.fillRect = fillRect;

            // Handle Area
            var handleAreaObj = new GameObject("Handle Slide Area");
            handleAreaObj.transform.SetParent(sliderObj.transform);
            var handleAreaRect = handleAreaObj.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = Vector2.zero;
            handleAreaRect.offsetMax = Vector2.zero;

            // Handle
            var handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleAreaObj.transform);
            var handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);
            var handleImage = handleObj.AddComponent<Image>();
            handleImage.color = Color.white;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;

            return slider;
        }

        private Toggle CreateToggle(string name, Transform parent)
        {
            var toggleObj = new GameObject(name);
            toggleObj.transform.SetParent(parent);

            var rectTransform = toggleObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var toggle = toggleObj.AddComponent<Toggle>();

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(toggleObj.transform);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.5f);
            bgRect.anchorMax = new Vector2(0, 0.5f);
            bgRect.sizeDelta = new Vector2(20, 20);
            bgRect.anchoredPosition = new Vector2(10, 0);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            toggle.targetGraphic = bgImage;

            // Checkmark
            var checkObj = new GameObject("Checkmark");
            checkObj.transform.SetParent(bgObj.transform);
            var checkRect = checkObj.AddComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = new Vector2(3, 3);
            checkRect.offsetMax = new Vector2(-3, -3);
            var checkImage = checkObj.AddComponent<Image>();
            checkImage.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            toggle.graphic = checkImage;

            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(toggleObj.transform);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(35, 0);
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelObj.AddComponent<Text>();
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAnchor.MiddleLeft;
            labelText.text = name;

            return toggle;
        }

        #endregion

        #region UI Event Wiring

        private void WireUpUIEvents()
        {
            // Populate category dropdown
            _categoryDropdown.ClearOptions();
            var categoryOptions = new List<string>();
            foreach (var category in _categories)
            {
                categoryOptions.Add(category.Name);
            }
            _categoryDropdown.AddOptions(categoryOptions);
            _categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);

            // Initial population of animation dropdown
            OnCategoryChanged(0);

            // Animation dropdown change
            _animationDropdown.onValueChanged.AddListener(OnAnimationChanged);

            // Duration slider
            _durationSlider.onValueChanged.AddListener(OnDurationChanged);

            // Buttons
            _playButton.onClick.AddListener(OnPlayClicked);
            _resetButton.onClick.AddListener(OnResetClicked);
            _playAllButton.onClick.AddListener(OnPlayAllClicked);
        }

        private void OnCategoryChanged(int index)
        {
            if (index < 0 || index >= _categories.Count) return;

            var category = _categories[index];
            _animationDropdown.ClearOptions();

            var animationOptions = new List<string>();
            foreach (var anim in category.Animations)
            {
                animationOptions.Add(anim.Name);
            }
            _animationDropdown.AddOptions(animationOptions);

            // Update description
            if (category.Animations.Count > 0)
            {
                _descriptionText.text = category.Animations[0].Description;
            }

            UpdateStatus($"Category: {category.Name} ({category.Animations.Count} animations)");
        }

        private void OnAnimationChanged(int index)
        {
            int categoryIndex = _categoryDropdown.value;
            if (categoryIndex < 0 || categoryIndex >= _categories.Count) return;

            var category = _categories[categoryIndex];
            if (index < 0 || index >= category.Animations.Count) return;

            var animation = category.Animations[index];
            _descriptionText.text = animation.Description;
        }

        private void OnDurationChanged(float value)
        {
            defaultDuration = value;
            _durationText.text = $"Duration: {value:F1}s";
        }

        private void OnPlayClicked()
        {
            int categoryIndex = _categoryDropdown.value;
            int animationIndex = _animationDropdown.value;

            if (categoryIndex < 0 || categoryIndex >= _categories.Count) return;
            var category = _categories[categoryIndex];

            if (animationIndex < 0 || animationIndex >= category.Animations.Count) return;
            var animation = category.Animations[animationIndex];

            if (_applyToAllToggle.isOn)
            {
                // Apply to all objects
                foreach (var obj in _demoObjects)
                {
                    if (obj != null)
                    {
                        animation.Execute(obj.transform, defaultDuration);
                    }
                }
                UpdateStatus($"Playing '{animation.Name}' on all objects");
            }
            else
            {
                // Apply to selected object
                if (_selectedObjectIndex >= 0 && _selectedObjectIndex < _demoObjects.Length)
                {
                    var selectedObj = _demoObjects[_selectedObjectIndex];
                    if (selectedObj != null)
                    {
                        animation.Execute(selectedObj.transform, defaultDuration);
                        UpdateStatus($"Playing '{animation.Name}' on {selectedObj.name}");
                    }
                }
                else
                {
                    // Default to first object
                    if (_demoObjects.Length > 0 && _demoObjects[0] != null)
                    {
                        animation.Execute(_demoObjects[0].transform, defaultDuration);
                        UpdateStatus($"Playing '{animation.Name}' on {_demoObjects[0].name}");
                    }
                }
            }
        }

        private void OnResetClicked()
        {
            // Kill all tweens
            TweenController.KillAll();

            // Reset all objects to original transforms
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

            UpdateStatus("All objects reset to original state");
        }

        private void OnPlayAllClicked()
        {
            int categoryIndex = _categoryDropdown.value;
            if (categoryIndex < 0 || categoryIndex >= _categories.Count) return;

            var category = _categories[categoryIndex];

            // Play all animations in the current category sequentially
            float totalDelay = 0f;
            foreach (var animation in category.Animations)
            {
                float delay = totalDelay;
                var anim = animation; // Capture for closure
                DOVirtual.DelayedCall(delay, () =>
                {
                    OnResetClicked();
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        foreach (var obj in _demoObjects)
                        {
                            if (obj != null)
                            {
                                anim.Execute(obj.transform, defaultDuration);
                            }
                        }
                        UpdateStatus($"Playing: {anim.Name}");
                    });
                });
                totalDelay += defaultDuration + 0.5f;
            }

            UpdateStatus($"Playing all {category.Animations.Count} animations in '{category.Name}'");
        }

        private void UpdateStatus(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
            }
            Debug.Log($"[TweenDemo] {message}");
        }

        #endregion

        #region Scene Setup Helpers

        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObj = new GameObject("Main Camera");
                camera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
                cameraObj.tag = "MainCamera";
            }

            camera.transform.position = new Vector3(0, 8, -12);
            camera.transform.rotation = Quaternion.Euler(30, 0, 0);
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        private void SetupLighting()
        {
            var existingLight = FindFirstObjectByType<Light>();
            if (existingLight == null)
            {
                var lightObj = new GameObject("Directional Light");
                var light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                light.color = Color.white;
                light.intensity = 1f;
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupCompleteScene();
            }
        }

        private void Update()
        {
            // Object selection with number keys
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    _selectedObjectIndex = i;
                    UpdateStatus($"Selected object {i + 1}: {_demoObjects[i]?.name ?? "null"}");
                }
            }
        }

        [ContextMenu("Clear Demo Scene")]
        public void ClearDemoScene()
        {
            var demoObjectsParent = GameObject.Find("Demo Objects");
            if (demoObjectsParent != null) DestroyImmediate(demoObjectsParent);

            var uiCanvas = GameObject.Find("Demo UI Canvas");
            if (uiCanvas != null) DestroyImmediate(uiCanvas);

            Debug.Log("Demo scene cleared!");
        }

        #endregion
    }
}
