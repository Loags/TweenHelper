using System;
using System.IO;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LB.TweenHelper.Editor
{
    public static class PublishingDomainReloadValidation
    {
        private static bool _previousEnabled;
        private static EnterPlayModeOptions _previousOptions;
        private static TweenMotionPreference _previousMotion;
        private static Task _pending;
        private static Tween _editorTween;
        private static int _cycle;
        private static bool _running;

        [MenuItem("Tools/Tween Helper Dev/Publishing/Validate Domain Reload Disabled")]
        public static void Validate()
        {
            if (_running || Application.isPlaying || SceneManager.GetActiveScene().isDirty) throw new InvalidOperationException("Stop playback and save the active scene before validation.");
            _previousEnabled = EditorSettings.enterPlayModeOptionsEnabled;
            _previousOptions = EditorSettings.enterPlayModeOptions;
            _previousMotion = TweenMotion.Preference;
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            _cycle = 0;
            _running = true;
            EditorApplication.playModeStateChanged += OnStateChanged;
            StartCycle();
        }

        private static void StartCycle()
        {
            TweenMotion.Preference = TweenMotionPreference.Reduced;
            _editorTween = DOVirtual.Float(0f, 1f, 60f, _ => { }).Pause();
            _pending = TweenAsync.AwaitCompletion(_editorTween);
            Schedule(() => EditorApplication.isPlaying = true);
        }

        private static void OnStateChanged(PlayModeStateChange state)
        {
            try
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    if (!_pending.IsCanceled) throw new Exception("Entering Play Mode did not settle the Editor observation.");
                    if (TweenMotion.Preference != TweenMotionPreference.UseProjectDefault) throw new Exception("Motion preference leaked across sessions.");
                    TweenPresetRegistry.Refresh();
                    if (TweenPresetRegistry.Count != 300) throw new Exception("Registry changed across play sessions.");
                    new TweenHandle(_editorTween).Kill();
                    DOTween.Init();
                    _pending = TweenAsync.AwaitCompletion(DOVirtual.Float(0f, 1f, 60f, _ => { }));
                    Schedule(() => EditorApplication.isPlaying = false);
                }
                else if (state == PlayModeStateChange.EnteredEditMode)
                {
                    if (!_pending.IsCanceled) throw new Exception("Exiting Play Mode left a pending observation.");
                    if (++_cycle < 2) StartCycle();
                    else Finish("PASS: Two sessions with domain reload disabled. Editor and runtime waits canceled at transitions; motion preference reset; all 300 names registered; original Editor options restored.");
                }
            }
            catch (Exception exception)
            {
                Finish("FAIL: " + exception);
                if (Application.isPlaying) Schedule(() => EditorApplication.isPlaying = false);
                Debug.LogException(exception);
            }
        }

        private static void Finish(string result)
        {
            EditorApplication.playModeStateChanged -= OnStateChanged;
            EditorSettings.enterPlayModeOptionsEnabled = _previousEnabled;
            EditorSettings.enterPlayModeOptions = _previousOptions;
            TweenMotion.Preference = _previousMotion;
            _running = false;
            Directory.CreateDirectory("ReleaseArtifacts/RoadmapValidation");
            File.WriteAllText("ReleaseArtifacts/RoadmapValidation/DomainReloadDisabled.txt", result);
        }

        private static void Schedule(Action action)
        {
            void Update()
            {
                EditorApplication.update -= Update;
                action();
            }
            EditorApplication.update += Update;
        }
    }
}
