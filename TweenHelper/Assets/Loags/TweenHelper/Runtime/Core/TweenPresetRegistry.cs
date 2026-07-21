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
    /// Auto-discovers classes marked with [AutoRegisterPreset] attribute.
    /// </summary>
    public static class TweenPresetRegistry
    {
        private static readonly Dictionary<string, ITweenPreset> _presetsByName = new Dictionary<string, ITweenPreset>();
        private static readonly Dictionary<Type, ITweenPreset> _presetsByType = new Dictionary<Type, ITweenPreset>();
        private static bool _hasScannedCodePresets = false;

        /// <summary>
        /// Gets all registered preset names.
        /// </summary>
        public static IEnumerable<string> PresetNames => _presetsByName.Keys;

        /// <summary>
        /// Gets all registered presets.
        /// </summary>
        public static IEnumerable<ITweenPreset> Presets => _presetsByName.Values;

        /// <summary>
        /// Gets the number of registered presets.
        /// </summary>
        public static int Count => _presetsByName.Count;

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

            Type presetType = preset.GetType();

            if (_presetsByName.TryGetValue(preset.PresetName, out ITweenPreset existingByName))
            {
                Debug.LogWarning($"TweenPresetRegistry: Preset '{preset.PresetName}' is already registered. Overwriting.");
                _presetsByType.Remove(existingByName.GetType());
            }

            if (_presetsByType.TryGetValue(presetType, out ITweenPreset existingByType))
            {
                _presetsByName.Remove(existingByType.PresetName);
            }

            _presetsByName[preset.PresetName] = preset;
            _presetsByType[presetType] = preset;
        }

        /// <summary>
        /// Unregisters a preset from the registry.
        /// </summary>
        /// <param name="presetName">The name of the preset to unregister.</param>
        /// <returns>True if the preset was found and removed.</returns>
        public static bool UnregisterPresetByName(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return false;
            }

            if (!_presetsByName.TryGetValue(presetName, out ITweenPreset preset)) return false;

            _presetsByName.Remove(presetName);
            _presetsByType.Remove(preset.GetType());
            return true;
        }

        /// <summary>
        /// Unregisters a preset by its concrete type.
        /// </summary>
        public static bool UnregisterPreset<TPreset>() where TPreset : class, ITweenPreset
        {
            EnsureInitialized();
            if (!_presetsByType.TryGetValue(typeof(TPreset), out ITweenPreset preset)) return false;
            return UnregisterPresetByName(preset.PresetName);
        }

        /// <summary>
        /// Unregisters a preset by name. Prefer <see cref="UnregisterPreset{TPreset}"/> for statically known presets.
        /// </summary>
        [Obsolete("Use UnregisterPreset<TPreset>() for statically known presets or UnregisterPresetByName() for dynamic names.")]
        public static bool UnregisterPreset(string presetName) => UnregisterPresetByName(presetName);

        #endregion

        #region Lookup

        /// <summary>
        /// Gets a preset by name.
        /// </summary>
        /// <param name="presetName">The name of the preset to get.</param>
        /// <returns>The preset if found, null otherwise.</returns>
        public static ITweenPreset GetPresetByName(string presetName)
        {
            if (string.IsNullOrEmpty(presetName))
            {
                return null;
            }

            // Ensure scanning has been done
            EnsureInitialized();

            _presetsByName.TryGetValue(presetName, out ITweenPreset preset);
            return preset;
        }

        /// <summary>
        /// Gets a registered preset by its concrete type.
        /// </summary>
        public static TPreset GetPreset<TPreset>() where TPreset : class, ITweenPreset
        {
            EnsureInitialized();
            _presetsByType.TryGetValue(typeof(TPreset), out ITweenPreset preset);
            return preset as TPreset;
        }

        /// <summary>
        /// Gets a preset by name. Prefer <see cref="GetPreset{TPreset}"/> for statically known presets.
        /// </summary>
        [Obsolete("Use GetPreset<TPreset>() for statically known presets or GetPresetByName() for dynamic names.")]
        public static ITweenPreset GetPreset(string presetName) => GetPresetByName(presetName);

        /// <summary>
        /// Checks if a preset with the specified name is registered.
        /// </summary>
        /// <param name="presetName">The name of the preset to check.</param>
        /// <returns>True if the preset exists.</returns>
        public static bool HasPresetByName(string presetName) => GetPresetByName(presetName) != null;

        /// <summary>
        /// Checks whether a preset type is registered.
        /// </summary>
        public static bool HasPreset<TPreset>() where TPreset : class, ITweenPreset => GetPreset<TPreset>() != null;

        /// <summary>
        /// Checks for a preset by name. Prefer <see cref="HasPreset{TPreset}"/> for statically known presets.
        /// </summary>
        [Obsolete("Use HasPreset<TPreset>() for statically known presets or HasPresetByName() for dynamic names.")]
        public static bool HasPreset(string presetName) => HasPresetByName(presetName);

        /// <summary>
        /// Gets presets that can be applied to the specified target.
        /// </summary>
        /// <param name="target">The target to check compatibility for.</param>
        /// <returns>An enumerable of compatible presets.</returns>
        public static IEnumerable<ITweenPreset> GetCompatiblePresets(GameObject target)
        {
            if (target == null) return Enumerable.Empty<ITweenPreset>();

            EnsureInitialized();

            return _presetsByName.Values.Where(preset => preset.CanApplyTo(target));
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
        public static Tween Play<TPreset>(GameObject target, float? duration = null, TweenOptions options = default) where TPreset : class, ITweenPreset
        {
            var preset = GetPreset<TPreset>();
            if (preset == null)
            {
                Debug.LogError($"TweenPresetRegistry: Preset type '{typeof(TPreset).Name}' is not registered.");
                return null;
            }

            return Play(preset, target, duration, options);
        }

        /// <summary>
        /// Plays a dynamically selected preset by name on the specified target.
        /// </summary>
        public static Tween PlayByName(string presetName, GameObject target, float? duration = null, TweenOptions options = default)
        {
            var preset = GetPresetByName(presetName);
            if (preset == null)
            {
                Debug.LogError($"TweenPresetRegistry: Preset '{presetName}' not found. Available: {string.Join(", ", PresetNames)}");
                return null;
            }

            return Play(preset, target, duration, options);
        }

        /// <summary>
        /// Plays an already resolved preset on the specified target.
        /// </summary>
        public static Tween Play(ITweenPreset preset, GameObject target, float? duration = null, TweenOptions options = default)
        {
            if (preset == null) throw new ArgumentNullException(nameof(preset));

            if (!preset.CanApplyTo(target))
            {
                Debug.LogError($"TweenPresetRegistry: Preset '{preset.PresetName}' cannot be applied to '{target?.name}'.");
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
                Debug.LogError($"TweenPresetRegistry: Failed to play preset '{preset.PresetName}' on '{target?.name}'. {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Plays a preset by name. Prefer <see cref="Play{TPreset}"/> for statically known presets.
        /// </summary>
        [Obsolete("Use Play<TPreset>() for statically known presets or PlayByName() for dynamic names.")]
        public static Tween Play(string presetName, GameObject target, float? duration = null, TweenOptions options = default)
            => PlayByName(presetName, target, duration, options);

        #endregion

        #region Scanning

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
        /// Clears all registered presets and rescans for code presets.
        /// </summary>
        public static void Refresh()
        {
            _presetsByName.Clear();
            _presetsByType.Clear();
            _hasScannedCodePresets = false;
            ScanForCodePresets();
        }

        #endregion

        #region Initialization

        private static void EnsureInitialized()
        {
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
            ScanForCodePresets();
        }

        /// <summary>
        /// Clears the registry when the domain reloads in the editor.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnDomainReload()
        {
            _presetsByName.Clear();
            _presetsByType.Clear();
            _hasScannedCodePresets = false;
        }

        #endregion
    }
}
