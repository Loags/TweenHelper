using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LB.TweenHelper
{
    /// <summary>
    /// Registry for managing and discovering tween presets.
    /// Supports both code-defined presets (CodePreset, LambdaPreset) and ScriptableObject assets (TweenPresetBase).
    /// </summary>
    public static class TweenPresetRegistry
    {
        private static readonly Dictionary<string, ITweenPreset> _presets = new Dictionary<string, ITweenPreset>();
        private static bool _hasScannedAssets = false;
        private static bool _hasScannedCodePresets = false;

        /// <summary>
        /// Gets all registered preset names.
        /// </summary>
        public static IEnumerable<string> PresetNames => _presets.Keys;

        /// <summary>
        /// Gets all registered presets.
        /// </summary>
        public static IEnumerable<ITweenPreset> Presets => _presets.Values;

        /// <summary>
        /// Gets the number of registered presets.
        /// </summary>
        public static int Count => _presets.Count;

        #region Registration

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
                Debug.LogWarning($"TweenPresetRegistry: Preset '{preset.PresetName}' is already registered. Overwriting.");
            }

            _presets[preset.PresetName] = preset;
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

            return _presets.Remove(presetName);
        }

        #endregion

        #region Lookup

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

            // Ensure scanning has been done
            EnsureInitialized();

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
        /// Gets presets that can be applied to the specified target.
        /// </summary>
        /// <param name="target">The target to check compatibility for.</param>
        /// <returns>An enumerable of compatible presets.</returns>
        public static IEnumerable<ITweenPreset> GetCompatiblePresets(GameObject target)
        {
            if (target == null) return Enumerable.Empty<ITweenPreset>();

            EnsureInitialized();

            return _presets.Values.Where(preset => preset.CanApplyTo(target));
        }

        #endregion

        #region Playback

        /// <summary>
        /// Plays a preset by name on the specified target.
        /// </summary>
        /// <param name="presetName">The name of the preset to play.</param>
        /// <param name="target">The GameObject to animate.</param>
        /// <param name="duration">Duration override (null uses preset default).</param>
        /// <param name="options">Additional options to apply.</param>
        /// <returns>The created tween, or null if the preset was not found.</returns>
        public static Tween Play(string presetName, GameObject target, float? duration = null, TweenOptions options = default)
        {
            var preset = GetPreset(presetName);
            if (preset == null)
            {
                Debug.LogError($"TweenPresetRegistry: Preset '{presetName}' not found. Available: {string.Join(", ", PresetNames)}");
                return null;
            }

            if (!preset.CanApplyTo(target))
            {
                Debug.LogError($"TweenPresetRegistry: Preset '{presetName}' cannot be applied to '{target?.name}'.");
                return null;
            }

            try
            {
                var tween = preset.CreateTween(target, duration, options);
                tween?.Play();
                return tween;
            }
            catch (Exception ex)
            {
                Debug.LogError($"TweenPresetRegistry: Failed to play preset '{presetName}' on '{target?.name}'. {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Scanning

        /// <summary>
        /// Scans for TweenPresetBase ScriptableObject assets in Resources folders.
        /// </summary>
        public static void ScanForAssets()
        {
            if (_hasScannedAssets) return;

            _hasScannedAssets = true;

            var presetAssets = Resources.LoadAll<TweenPresetBase>("");
            int count = 0;

            foreach (var preset in presetAssets)
            {
                if (preset != null)
                {
                    preset.ValidatePreset();
                    RegisterPreset(preset);
                    count++;
                }
            }

            if (count > 0)
            {
                Debug.Log($"TweenPresetRegistry: Found {count} preset assets.");
            }
        }

        /// <summary>
        /// Scans for classes marked with [AutoRegisterPreset] attribute and registers them.
        /// </summary>
        public static void ScanForCodePresets()
        {
            if (_hasScannedCodePresets) return;

            _hasScannedCodePresets = true;
            int count = 0;

            try
            {
                // Get all loaded assemblies
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    // Skip system assemblies for performance
                    var assemblyName = assembly.GetName().Name;
                    if (assemblyName.StartsWith("System") ||
                        assemblyName.StartsWith("Unity") ||
                        assemblyName.StartsWith("mscorlib") ||
                        assemblyName.StartsWith("netstandard"))
                    {
                        continue;
                    }

                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            // Check if type has AutoRegisterPreset attribute
                            if (type.GetCustomAttribute<AutoRegisterPresetAttribute>() != null)
                            {
                                // Must be a concrete class implementing ITweenPreset
                                if (!type.IsAbstract && typeof(ITweenPreset).IsAssignableFrom(type))
                                {
                                    try
                                    {
                                        var instance = Activator.CreateInstance(type) as ITweenPreset;
                                        if (instance != null)
                                        {
                                            RegisterPreset(instance);
                                            count++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.LogWarning($"TweenPresetRegistry: Failed to instantiate code preset '{type.Name}'. {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Skip assemblies that can't be reflected
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"TweenPresetRegistry: Error scanning for code presets. {ex.Message}");
            }

            if (count > 0)
            {
                Debug.Log($"TweenPresetRegistry: Found {count} code presets.");
            }
        }

        /// <summary>
        /// Clears all registered presets and rescans for assets and code presets.
        /// </summary>
        public static void Refresh()
        {
            _presets.Clear();
            _hasScannedAssets = false;
            _hasScannedCodePresets = false;
            ScanForAssets();
            ScanForCodePresets();
        }

        #endregion

        #region Initialization

        private static void EnsureInitialized()
        {
            if (!_hasScannedAssets)
            {
                ScanForAssets();
            }
            if (!_hasScannedCodePresets)
            {
                ScanForCodePresets();
            }
        }

        /// <summary>
        /// Initializes the registry after scene load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            ScanForAssets();
            ScanForCodePresets();
        }

        /// <summary>
        /// Clears the registry when the domain reloads in the editor.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnDomainReload()
        {
            _presets.Clear();
            _hasScannedAssets = false;
            _hasScannedCodePresets = false;
        }

        #endregion
    }
}
