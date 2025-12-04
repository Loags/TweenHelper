using DG.Tweening;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Singleton ScriptableObject that defines global defaults for tweens and DoTween engine configuration.
    /// This serves as the single source of truth for all tween settings across the project.
    /// </summary>
    [CreateAssetMenu(fileName = "TweenHelperSettings", menuName = "LB/TweenHelper Settings", order = 1)]
    public class TweenHelperSettings : ScriptableObject
    {
        [Header("Global Tween Defaults")]
        [SerializeField, Min(0f)] private float defaultDuration = 1f;
        [SerializeField] private Ease defaultEase = Ease.OutQuart;
        [SerializeField, Min(0f)] private float defaultDelay = 0f;
        [SerializeField] private UpdateType defaultUpdateType = UpdateType.Normal;
        [SerializeField] private bool defaultUnscaledTime = false;
        [SerializeField] private bool defaultSnapping = false;
        
        [Header("DoTween Engine Configuration")]
        [SerializeField] private bool useSafeMode = true;
        [SerializeField] private bool enableAutoPlay = true;
        [SerializeField] private bool enableAutoKill = true;
        [SerializeField] private LogBehaviour logBehaviour = LogBehaviour.ErrorsOnly;
        [SerializeField] private bool enablePrewarm = false;
        
        [Header("Engine Capacities")]
        [SerializeField, Min(0)] private int maxTweenCapacity = 200;
        [SerializeField, Min(0)] private int maxSequenceCapacity = 50;
        [SerializeField, Min(0)] private int maxTweenersCapacity = 200;
        [SerializeField, Min(0)] private int maxSequencesCapacity = 50;
        
        private static TweenHelperSettings _instance;
        
        /// <summary>
        /// Gets the singleton instance of TweenHelperSettings.
        /// Loads from Resources if not already cached.
        /// </summary>
        public static TweenHelperSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<TweenHelperSettings>("TweenHelperSettings");
                    if (_instance == null)
                    {
                        Debug.LogError("TweenHelperSettings not found in Resources folder. Creating default instance.");
                        _instance = CreateInstance<TweenHelperSettings>();
                    }
                }
                return _instance;
            }
        }
        
        #region Global Defaults Properties
        
        /// <summary>
        /// The default duration for tweens when not explicitly specified.
        /// </summary>
        public float DefaultDuration => defaultDuration;
        
        /// <summary>
        /// The default ease for tweens when not explicitly specified.
        /// </summary>
        public Ease DefaultEase => defaultEase;
        
        /// <summary>
        /// The default delay for tweens when not explicitly specified.
        /// </summary>
        public float DefaultDelay => defaultDelay;
        
        /// <summary>
        /// The default update type for tweens when not explicitly specified.
        /// </summary>
        public UpdateType DefaultUpdateType => defaultUpdateType;
        
        /// <summary>
        /// Whether tweens should use unscaled time by default.
        /// </summary>
        public bool DefaultUnscaledTime => defaultUnscaledTime;
        
        /// <summary>
        /// Whether tweens should use snapping by default.
        /// </summary>
        public bool DefaultSnapping => defaultSnapping;
        
        #endregion
        
        #region DoTween Configuration Properties
        
        /// <summary>
        /// Whether DoTween should run in Safe Mode for additional error checking.
        /// </summary>
        public bool UseSafeMode => useSafeMode;
        
        /// <summary>
        /// Whether tweens should auto-play by default.
        /// </summary>
        public bool EnableAutoPlay => enableAutoPlay;
        
        /// <summary>
        /// Whether tweens should auto-kill when complete by default.
        /// </summary>
        public bool EnableAutoKill => enableAutoKill;
        
        /// <summary>
        /// The logging behavior for DoTween.
        /// </summary>
        public LogBehaviour LogBehaviour => logBehaviour;
        
        /// <summary>
        /// Whether to prewarm the DoTween engine by creating and killing small tweens at startup.
        /// </summary>
        public bool EnablePrewarm => enablePrewarm;
        
        /// <summary>
        /// Maximum capacity for active tweens to avoid internal array resizes.
        /// </summary>
        public int MaxTweenCapacity => maxTweenCapacity;
        
        /// <summary>
        /// Maximum capacity for active sequences to avoid internal array resizes.
        /// </summary>
        public int MaxSequenceCapacity => maxSequenceCapacity;
        
        /// <summary>
        /// Maximum capacity for tweeners to avoid internal array resizes.
        /// </summary>
        public int MaxTweenersCapacity => maxTweenersCapacity;
        
        /// <summary>
        /// Maximum capacity for sequences to avoid internal array resizes.
        /// </summary>
        public int MaxSequencesCapacity => maxSequencesCapacity;
        
        #endregion
        
        /// <summary>
        /// Validates the settings and logs warnings for potentially problematic configurations.
        /// </summary>
        public void ValidateSettings()
        {
            if (defaultDuration <= 0f)
            {
                Debug.LogWarning("TweenHelperSettings: Default duration is zero or negative, which may cause issues.");
            }
            
            if (maxTweenCapacity <= 0 || maxSequenceCapacity <= 0)
            {
                Debug.LogWarning("TweenHelperSettings: Capacities should be greater than zero for optimal performance.");
            }
            
            if (enablePrewarm && (maxTweenCapacity < 10 || maxSequenceCapacity < 5))
            {
                Debug.LogWarning("TweenHelperSettings: Prewarm is enabled but capacities are very low, consider increasing them.");
            }
        }
        
        private void OnValidate()
        {
            // Ensure reasonable minimum values
            defaultDuration = Mathf.Max(0.01f, defaultDuration);
            defaultDelay = Mathf.Max(0f, defaultDelay);
            maxTweenCapacity = Mathf.Max(1, maxTweenCapacity);
            maxSequenceCapacity = Mathf.Max(1, maxSequenceCapacity);
            maxTweenersCapacity = Mathf.Max(1, maxTweenersCapacity);
            maxSequencesCapacity = Mathf.Max(1, maxSequencesCapacity);
        }
    }
}

