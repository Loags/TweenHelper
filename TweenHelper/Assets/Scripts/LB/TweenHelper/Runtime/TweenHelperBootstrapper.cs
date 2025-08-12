using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Handles runtime initialization of the TweenHelper system.
    /// Applies settings at startup, initializes DoTween, sets capacities, and optionally prewarms the engine.
    /// </summary>
    public static class TweenHelperBootstrapper
    {
        private static bool _isInitialized = false;
        
        /// <summary>
        /// Initializes the TweenHelper system with the current settings.
        /// This should be called once at application startup.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("TweenHelperBootstrapper: Already initialized. Skipping duplicate initialization.");
                return;
            }
            
            var settings = TweenHelperSettings.Instance;
            if (settings == null)
            {
                Debug.LogError("TweenHelperBootstrapper: Cannot initialize without TweenHelperSettings.");
                return;
            }
            
            // Validate settings before applying
            settings.ValidateSettings();
            
            try
            {
                InitializeDoTween(settings);
                ConfigureDoTween(settings);
                SetCapacities(settings);
                
                if (settings.EnablePrewarm)
                {
                    PrewarmEngine();
                }
                
                _isInitialized = true;
                Debug.Log("TweenHelper initialized successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"TweenHelperBootstrapper: Failed to initialize. {ex.Message}");
            }
        }
        
        /// <summary>
        /// Gets whether the TweenHelper system has been initialized.
        /// </summary>
        public static bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// Forces reinitialization of the TweenHelper system.
        /// Useful for testing or when settings change at runtime.
        /// </summary>
        public static void Reinitialize()
        {
            _isInitialized = false;
            Initialize();
        }
        
        private static void InitializeDoTween(TweenHelperSettings settings)
        {
            // Initialize DoTween with basic configuration
            DOTween.Init(
                recycleAllByDefault: true,
                useSafeMode: settings.UseSafeMode,
                logBehaviour: settings.LogBehaviour
            );
            
            // Set global defaults
            DOTween.defaultAutoPlay = settings.EnableAutoPlay ? AutoPlay.All : AutoPlay.None;
            DOTween.defaultAutoKill = settings.EnableAutoKill;
            DOTween.defaultUpdateType = settings.DefaultUpdateType;
            DOTween.defaultTimeScaleIndependent = settings.DefaultUnscaledTime;
        }
        
        private static void ConfigureDoTween(TweenHelperSettings settings)
        {
            // Apply additional DoTween configuration
            DOTween.useSafeMode = settings.UseSafeMode;
            DOTween.logBehaviour = settings.LogBehaviour;
            
            // Set default easing
            DOTween.defaultEaseType = settings.DefaultEase;
        }
        
        private static void SetCapacities(TweenHelperSettings settings)
        {
            try
            {
                // Set capacities to avoid internal array resizes during gameplay
                DOTween.SetTweensCapacity(
                    tweenersCapacity: settings.MaxTweenersCapacity,
                    sequencesCapacity: settings.MaxSequencesCapacity
                );
                
                Debug.Log($"DoTween capacities set - Tweeners: {settings.MaxTweenersCapacity}, Sequences: {settings.MaxSequencesCapacity}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"TweenHelperBootstrapper: Failed to set capacities. {ex.Message}");
            }
        }
        
        private static void PrewarmEngine()
        {
            try
            {
                // Create and immediately kill a few small tweens to prewarm the engine
                // This helps avoid allocation spikes during the first real tweens
                var tempGO = new GameObject("TweenHelper_PrewarmTemp");
                tempGO.SetActive(false);
                
                // Prewarm with different tween types
                var transform = tempGO.transform;
                
                // Position tween
                transform.DOMove(Vector3.zero, 0.01f).Kill();
                
                // Rotation tween  
                transform.DORotate(Vector3.zero, 0.01f).Kill();
                
                // Scale tween
                transform.DOScale(Vector3.one, 0.01f).Kill();
                
                // Sequence
                var sequence = DOTween.Sequence();
                sequence.Append(transform.DOMove(Vector3.zero, 0.01f));
                sequence.Kill();
                
                // Clean up
                if (Application.isPlaying)
                {
                    Object.Destroy(tempGO);
                }
                else
                {
                    Object.DestroyImmediate(tempGO);
                }
                
                Debug.Log("DoTween engine prewarmed successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"TweenHelperBootstrapper: Prewarm failed. {ex.Message}");
            }
        }
        
        /// <summary>
        /// Cleans up the DoTween engine. Called automatically on application quit.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            // Reset static state for domain reloading in editor
            _isInitialized = false;
        }
        
        /// <summary>
        /// Ensures proper cleanup when the application is quitting.
        /// </summary>
        static TweenHelperBootstrapper()
        {
            Application.quitting += OnApplicationQuitting;
        }
        
        private static void OnApplicationQuitting()
        {
            if (_isInitialized)
            {
                try
                {
                    DOTween.KillAll();
                    DOTween.Clear();
                    _isInitialized = false;
                    Debug.Log("TweenHelper cleaned up on application quit.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"TweenHelperBootstrapper: Cleanup failed. {ex.Message}");
                }
            }
        }
    }
}
