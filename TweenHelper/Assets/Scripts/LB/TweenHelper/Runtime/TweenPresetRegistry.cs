using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Registry for managing and discovering tween presets.
    /// Supports both code-defined presets and ScriptableObject assets.
    /// </summary>
    public static class TweenPresetRegistry
    {
        private static readonly Dictionary<string, ITweenPreset> _presets = new Dictionary<string, ITweenPreset>();
        private static bool _hasScannedAssets = false;
        
        /// <summary>
        /// Gets all registered preset names.
        /// </summary>
        public static IEnumerable<string> PresetNames => _presets.Keys;
        
        /// <summary>
        /// Gets all registered presets.
        /// </summary>
        public static IEnumerable<ITweenPreset> Presets => _presets.Values;
        
        /// <summary>
        /// Registers a preset with the registry.
        /// </summary>
        /// <param name="preset">The preset to register.</param>
        public static void RegisterPreset(ITweenPreset preset)
        {
            if (preset == null)
            {
                Debug.LogWarning("TweenPresetRegistry: Cannot register null preset.");
                return;
            }
            
            if (string.IsNullOrEmpty(preset.PresetName))
            {
                Debug.LogWarning("TweenPresetRegistry: Cannot register preset with null or empty name.");
                return;
            }
            
            if (_presets.ContainsKey(preset.PresetName))
            {
                Debug.LogWarning($"TweenPresetRegistry: Preset '{preset.PresetName}' is already registered. Overwriting existing preset.");
            }
            
            _presets[preset.PresetName] = preset;
            Debug.Log($"TweenPresetRegistry: Registered preset '{preset.PresetName}'.");
        }
        
        /// <summary>
        /// Unregisters a preset from the registry.
        /// </summary>
        /// <param name="presetName">The name of the preset to unregister.</param>
        /// <returns>True if the preset was found and removed.</returns>
        public static bool UnregisterPreset(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return false;
            }
            
            bool removed = _presets.Remove(presetName);
            if (removed)
            {
                Debug.Log($"TweenPresetRegistry: Unregistered preset '{presetName}'.");
            }
            
            return removed;
        }
        
        /// <summary>
        /// Gets a preset by name.
        /// </summary>
        /// <param name="presetName">The name of the preset to get.</param>
        /// <returns>The preset if found, null otherwise.</returns>
        public static ITweenPreset GetPreset(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return null;
            }
            
            // Ensure assets have been scanned
            if (!_hasScannedAssets)
            {
                ScanForPresetAssets();
            }
            
            _presets.TryGetValue(presetName, out ITweenPreset preset);
            return preset;
        }
        
        /// <summary>
        /// Checks if a preset with the specified name is registered.
        /// </summary>
        /// <param name="presetName">The name of the preset to check.</param>
        /// <returns>True if the preset exists.</returns>
        public static bool HasPreset(string presetName)
        {
            return GetPreset(presetName) != null;
        }
        
        /// <summary>
        /// Plays a preset by name on the specified target.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween, or null if the preset was not found.</returns>
        public static Tween PlayPreset(string presetName, GameObject target, float? duration = null, TweenOptions options = default)
        {
            var preset = GetPreset(presetName);
            if (preset == null)
            {
                Debug.LogError($"TweenPresetRegistry: Preset '{presetName}' not found. Available presets: {string.Join(", ", PresetNames)}");
                return null;
            }
            
            if (!preset.CanApplyTo(target))
            {
                Debug.LogError($"TweenPresetRegistry: Preset '{presetName}' cannot be applied to target '{target?.name}'. Check target compatibility.");
                return null;
            }
            
            try
            {
                return preset.CreateTween(target, duration, options);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"TweenPresetRegistry: Failed to create tween for preset '{presetName}' on target '{target?.name}'. {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Scans for preset assets in the project and registers them automatically.
        /// </summary>
        public static void ScanForPresetAssets()
        {
            if (_hasScannedAssets) return;
            
            _hasScannedAssets = true;
            
            // Find all TweenPresetBase assets in Resources
            var presetAssets = Resources.LoadAll<TweenPresetBase>("");
            
            foreach (var preset in presetAssets)
            {
                if (preset != null)
                {
                    preset.ValidatePreset();
                    RegisterPreset(preset);
                }
            }
            
            Debug.Log($"TweenPresetRegistry: Scanned and found {presetAssets.Length} preset assets.");
        }
        
        /// <summary>
        /// Clears all registered presets and rescans for assets.
        /// </summary>
        public static void Refresh()
        {
            _presets.Clear();
            _hasScannedAssets = false;
            ScanForPresetAssets();
            RegisterBuiltInPresets();
        }
        
        /// <summary>
        /// Gets presets that can be applied to the specified target.
        /// </summary>
        /// <param name="target">The target to check compatibility for.</param>
        /// <returns>An enumerable of compatible presets.</returns>
        public static IEnumerable<ITweenPreset> GetCompatiblePresets(GameObject target)
        {
            if (target == null) return System.Linq.Enumerable.Empty<ITweenPreset>();
            
            // Ensure assets have been scanned
            if (!_hasScannedAssets)
            {
                ScanForPresetAssets();
            }
            
            return _presets.Values.Where(preset => preset.CanApplyTo(target));
        }
        
        /// <summary>
        /// Registers built-in presets that are always available.
        /// </summary>
        private static void RegisterBuiltInPresets()
        {
            RegisterPreset(new PopInPreset());
            RegisterPreset(new PopOutPreset());
            RegisterPreset(new BouncePreset());
            RegisterPreset(new ShakePreset());
            RegisterPreset(new FadeInPreset());
            RegisterPreset(new FadeOutPreset());
        }
        
        /// <summary>
        /// Initializes the registry with built-in presets.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            RegisterBuiltInPresets();
        }
        
        /// <summary>
        /// Clears the registry when the domain reloads in the editor.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnDomainReload()
        {
            _presets.Clear();
            _hasScannedAssets = false;
        }
    }
}
