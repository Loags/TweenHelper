using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LB.TweenHelper.Editor
{
    [InitializeOnLoad]
    internal static class UISequencePhaseValidation
    {
        private const string SessionKey = "TweenHelper.UISequencePhaseValidation.Pending";
        private const string ResultPath = "Temp/UISequencePhase4Validation.txt";
        private static ValidationSession _session;

        static UISequencePhaseValidation()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.update += Update;
        }

        [MenuItem("Tools/Tween Helper Dev/Run UI Sequence Phase Validation %#u", false, 100)]
        private static void Run()
        {
            string resultPath = Path.GetFullPath(ResultPath);
            if (File.Exists(resultPath)) File.Delete(resultPath);
            SessionState.SetBool(SessionKey, true);
            if (EditorApplication.isPlaying) BeginSession();
            else EditorApplication.EnterPlaymode();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(SessionKey, false)) BeginSession();
        }

        private static void BeginSession()
        {
            SessionState.SetBool(SessionKey, false);
            _session = new ValidationSession(Path.GetFullPath(ResultPath));
        }

        private static void Update() => _session?.Tick();

        private sealed class ValidationSession
        {
            private readonly string _resultPath;
            private readonly List<string> _results = new List<string>();
            private readonly GameObject _root;
            private int _step;
            private double _deadline;
            private TweenHandle _handle;
            private GameObject _toast;
            private GameObject _modalBackdrop;
            private GameObject _modalPanel;
            private GameObject[] _modalControls;
            private GameObject _tooltip;
            private GameObject _dropdown;
            private GameObject[] _dropdownEntries;
            private GameObject _tabOutgoing;
            private GameObject _tabIncoming;
            private Pose _baseline;
            private Pose _interrupted;

            public ValidationSession(string resultPath)
            {
                _resultPath = resultPath;
                _root = new GameObject("UI Sequence Phase Validation", typeof(RectTransform), typeof(Canvas));
                _root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }

            public void Tick()
            {
                if (EditorApplication.timeSinceStartup < _deadline) return;

                try
                {
                    switch (_step)
                    {
                        case 0: StartToastShow(); break;
                        case 1: CompleteToastShow(); break;
                        case 2: CompleteToastHide(); break;
                        case 3: StartModalOpen(); break;
                        case 4: CompleteModalOpen(); break;
                        case 5: CompleteModalClose(); break;
                        case 6: StartKillPreservation(); break;
                        case 7: CaptureAndKill(); break;
                        case 8: CompleteKillPreservation(); break;
                        case 9: StartRewind(); break;
                        case 10: CompleteRewind(); break;
                        case 11: StartTabSwitch(); break;
                        case 12: CompleteTabSwitch(); break;
                        case 13: StartRestart(); break;
                        case 14: RestartSecondPass(); break;
                        case 15: CompleteRestart(); break;
                        case 16: StartUnscaled(); break;
                        case 17: CompleteUnscaled(); break;
                        default: Finish(); break;
                    }
                }
                catch (Exception exception)
                {
                    _results.Add("FAIL | Validator exception | " + exception);
                    Finish();
                }
            }

            private void StartToastShow()
            {
                _toast = CreatePanel("Toast", new Vector3(37f, -22f, 3f), new Vector3(0.9f, 1.1f, 1f), 0.73f);
                _baseline = Pose.Capture(_toast);
                _handle = _toast.ToastShow(UISequenceDirection.Up, 60f, 0.12f);
                Wait(0.24f);
            }

            private void CompleteToastShow()
            {
                Record("ToastShow exact custom baseline", Pose.Capture(_toast).Matches(_baseline));
                _handle = _toast.ToastHide(UISequenceDirection.Right, 45f, 0.12f);
                Wait(0.24f);
            }

            private void CompleteToastHide()
            {
                Pose hidden = Pose.Capture(_toast);
                bool position = Vector3.Distance(hidden.Position, _baseline.Position + Vector3.right * 45f) <= 0.01f;
                bool scale = Vector3.Distance(hidden.Scale, _baseline.Scale * 0.97f) <= 0.001f;
                Record("ToastHide semantic endpoint", position && scale && Mathf.Abs(hidden.Alpha) <= 0.001f);
                Next();
            }

            private void StartModalOpen()
            {
                _modalBackdrop = CreatePanel("Backdrop", Vector3.zero, Vector3.one, 0.66f);
                _modalPanel = CreatePanel("Modal", new Vector3(0f, 12f, 0f), new Vector3(1.08f, 0.94f, 1f), 0.82f);
                _modalControls = new[]
                {
                    CreatePanel("Control A", new Vector3(-80f, -45f, 0f), Vector3.one, 0.88f, _modalPanel.transform),
                    CreatePanel("Control B", new Vector3(0f, -45f, 0f), Vector3.one, 0.91f, _modalPanel.transform),
                    CreatePanel("Control C", new Vector3(80f, -45f, 0f), Vector3.one, 0.95f, _modalPanel.transform)
                };
                _handle = _modalPanel.ModalOpen(_modalBackdrop, _modalControls, 0.18f, 0.025f);
                Wait(0.36f);
            }

            private void CompleteModalOpen()
            {
                bool shown = Mathf.Abs(_modalBackdrop.GetComponent<CanvasGroup>().alpha - 0.66f) <= 0.001f && Mathf.Abs(_modalPanel.GetComponent<CanvasGroup>().alpha - 0.82f) <= 0.001f;
                for (int i = 0; i < _modalControls.Length; i++) shown &= _modalControls[i].GetComponent<CanvasGroup>().alpha >= 0.87f;
                Record("ModalOpen restores all authored endpoints", shown);
                _handle = _modalPanel.ModalClose(_modalBackdrop, _modalControls, 0.18f, 0.025f);
                Wait(0.36f);
            }

            private void CompleteModalClose()
            {
                bool hidden = Mathf.Abs(_modalBackdrop.GetComponent<CanvasGroup>().alpha) <= 0.001f && Mathf.Abs(_modalPanel.GetComponent<CanvasGroup>().alpha) <= 0.001f;
                for (int i = 0; i < _modalControls.Length; i++) hidden &= Mathf.Abs(_modalControls[i].GetComponent<CanvasGroup>().alpha) <= 0.001f;
                Record("ModalClose hides panel, backdrop, and controls", hidden);
                Next();
            }

            private void StartKillPreservation()
            {
                _tooltip = CreatePanel("Tooltip", new Vector3(-14f, 33f, 2f), new Vector3(1.02f, 0.97f, 1f), 0.77f);
                _tooltip.RefreshUIAnimationState();
                _handle = _tooltip.TooltipHide(UISequenceDirection.Down, 35f, 0.4f);
                Wait(0.13f);
            }

            private void CaptureAndKill()
            {
                _interrupted = Pose.Capture(_tooltip);
                _handle.Kill();
                Wait(0.08f);
            }

            private void CompleteKillPreservation()
            {
                Record("Kill preserves interrupted state", Pose.Capture(_tooltip).Matches(_interrupted));
                Next();
            }

            private void StartRewind()
            {
                _dropdown = CreatePanel("Dropdown", new Vector3(18f, 90f, 1f), new Vector3(0.92f, 1.08f, 1f), 0.84f);
                ((RectTransform)_dropdown.transform).pivot = new Vector2(0.5f, 1f);
                _dropdownEntries = new[]
                {
                    CreatePanel("Entry A", new Vector3(0f, 20f, 0f), Vector3.one, 0.81f, _dropdown.transform),
                    CreatePanel("Entry B", new Vector3(0f, -20f, 0f), Vector3.one, 0.86f, _dropdown.transform)
                };
                _baseline = Pose.Capture(_dropdown);
                _handle = _dropdown.DropdownClose(_dropdownEntries, 0.35f, 0.03f);
                Wait(0.12f);
            }

            private void CompleteRewind()
            {
                _handle.Rewind();
                bool restored = Pose.Capture(_dropdown).Matches(_baseline);
                for (int i = 0; i < _dropdownEntries.Length; i++) restored &= _dropdownEntries[i].GetComponent<CanvasGroup>().alpha > 0.8f;
                Record("Rewind restores every invocation state", restored);
                Next();
            }

            private void StartTabSwitch()
            {
                _tabOutgoing = CreatePanel("Outgoing", new Vector3(11f, -9f, 2f), new Vector3(1.04f, 0.96f, 1f), 0.79f);
                _tabIncoming = CreatePanel("Incoming", new Vector3(-15f, 14f, 4f), new Vector3(0.93f, 1.07f, 1f), 0.87f);
                Pose incomingBaseline = Pose.Capture(_tabIncoming);
                _baseline = incomingBaseline;
                _handle = _tabOutgoing.TabSwitchTo(_tabIncoming, UISequenceDirection.Left, 80f, 0.18f);
                Wait(0.32f);
            }

            private void CompleteTabSwitch()
            {
                Pose outgoing = Pose.Capture(_tabOutgoing);
                bool outgoingHidden = Mathf.Abs(outgoing.Alpha) <= 0.001f && Vector3.Distance(outgoing.Position, new Vector3(11f, -9f, 2f) + Vector3.left * 80f) <= 0.01f;
                Record("TabSwitch coordinates exact outgoing/incoming endpoints", outgoingHidden && Pose.Capture(_tabIncoming).Matches(_baseline));
                Next();
            }

            private void StartRestart()
            {
                GameObject restartTarget = CreatePanel("Restart", new Vector3(9f, 21f, 5f), new Vector3(0.95f, 1.03f, 1f), 0.74f);
                _toast = restartTarget;
                _baseline = Pose.Capture(restartTarget);
                _handle = restartTarget.Tween().ToastHide(UISequenceDirection.Up, 48f, 0.14f).Build();
                _handle.Tween.SetAutoKill(false);
                _handle.Tween.Play();
                Wait(0.24f);
            }

            private void RestartSecondPass()
            {
                _interrupted = Pose.Capture(_toast);
                _handle.Restart();
                Wait(0.24f);
            }

            private void CompleteRestart()
            {
                Record("Restart repeats without endpoint drift", Pose.Capture(_toast).Matches(_interrupted));
                _handle.Kill();
                Next();
            }

            private void StartUnscaled()
            {
                Time.timeScale = 0f;
                _tooltip = CreatePanel("Unscaled", new Vector3(-8f, 12f, 6f), new Vector3(1.06f, 0.94f, 1f), 0.69f);
                _baseline = Pose.Capture(_tooltip);
                _handle = _tooltip.TooltipShow(duration: 0.12f, options: TweenOptions.WithUnscaledTime());
                Wait(0.25f);
            }

            private void CompleteUnscaled()
            {
                Record("Unscaled-time sequence completes at timeScale zero", Pose.Capture(_tooltip).Matches(_baseline));
                Time.timeScale = 1f;
                Next();
            }

            private GameObject CreatePanel(string name, Vector3 position, Vector3 scale, float alpha, Transform parent = null)
            {
                var target = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
                target.transform.SetParent(parent == null ? _root.transform : parent, false);
                var rect = (RectTransform)target.transform;
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition3D = position;
                rect.localScale = scale;
                rect.sizeDelta = new Vector2(120f, 60f);
                target.GetComponent<CanvasGroup>().alpha = alpha;
                return target;
            }

            private void Record(string name, bool passed) => _results.Add((passed ? "PASS" : "FAIL") + " | " + name);

            private void Wait(float seconds)
            {
                _step++;
                _deadline = EditorApplication.timeSinceStartup + seconds;
            }

            private void Next()
            {
                _step++;
                _deadline = 0d;
            }

            private void Finish()
            {
                Time.timeScale = 1f;
                _handle?.Kill();
                int failures = 0;
                for (int i = 0; i < _results.Count; i++)
                {
                    if (_results[i].StartsWith("FAIL", StringComparison.Ordinal)) failures++;
                }

                _results.Insert(0, failures == 0 ? "RESULT: PASS" : $"RESULT: FAIL ({failures})");
                File.WriteAllLines(_resultPath, _results);
                UnityEngine.Object.Destroy(_root);
                _session = null;
                EditorApplication.ExitPlaymode();
            }
        }

        private readonly struct Pose
        {
            public readonly Vector3 Position;
            public readonly Vector3 Scale;
            public readonly Quaternion Rotation;
            public readonly float Alpha;

            private Pose(Vector3 position, Vector3 scale, Quaternion rotation, float alpha)
            {
                Position = position;
                Scale = scale;
                Rotation = rotation;
                Alpha = alpha;
            }

            public static Pose Capture(GameObject target)
            {
                var rect = (RectTransform)target.transform;
                return new Pose(rect.anchoredPosition3D, rect.localScale, rect.localRotation, target.GetComponent<CanvasGroup>().alpha);
            }

            public bool Matches(Pose other)
            {
                return Vector3.Distance(Position, other.Position) <= 0.01f &&
                       Vector3.Distance(Scale, other.Scale) <= 0.001f &&
                       Quaternion.Angle(Rotation, other.Rotation) <= 0.01f &&
                       Mathf.Abs(Alpha - other.Alpha) <= 0.001f;
            }
        }
    }
}
