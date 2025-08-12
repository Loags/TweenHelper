using UnityEngine;
using UnityEditor;
using System.IO;

namespace LB.TweenHelper.Editor
{
    /// <summary>
    /// Editor utility to create and manage TweenHelperSettings assets.
    /// </summary>
    public static class TweenHelperSettingsCreator
    {
        private const string SETTINGS_PATH = "Assets/Resources/TweenHelperSettings.asset";
        
        /// <summary>
        /// Creates a default TweenHelperSettings asset in the Resources folder if one doesn't exist.
        /// </summary>
        [MenuItem("LB/TweenHelper/Create Settings Asset", false, 1)]
        public static void CreateSettingsAsset()
        {
            if (File.Exists(SETTINGS_PATH))
            {
                Debug.Log("TweenHelperSettings asset already exists at: " + SETTINGS_PATH);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<TweenHelperSettings>(SETTINGS_PATH);
                EditorGUIUtility.PingObject(Selection.activeObject);
                return;
            }
            
            // Ensure Resources directory exists
            string resourcesDir = "Assets/Resources";
            if (!Directory.Exists(resourcesDir))
            {
                Directory.CreateDirectory(resourcesDir);
            }
            
            // Create the settings asset
            var settings = ScriptableObject.CreateInstance<TweenHelperSettings>();
            AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Select and ping the new asset
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
            
            Debug.Log("Created TweenHelperSettings asset at: " + SETTINGS_PATH);
        }
        
        /// <summary>
        /// Validates that a TweenHelperSettings asset exists in Resources.
        /// </summary>
        [MenuItem("LB/TweenHelper/Validate Settings", false, 2)]
        public static void ValidateSettings()
        {
            var settings = Resources.Load<TweenHelperSettings>("TweenHelperSettings");
            if (settings == null)
            {
                EditorUtility.DisplayDialog(
                    "TweenHelper Settings Missing",
                    "No TweenHelperSettings asset found in Resources folder. Would you like to create one?",
                    "Create Now",
                    "Cancel"
                );
                CreateSettingsAsset();
            }
            else
            {
                settings.ValidateSettings();
                EditorUtility.DisplayDialog(
                    "TweenHelper Settings Valid",
                    "TweenHelperSettings asset found and validated successfully.",
                    "OK"
                );
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
        }
        
        /// <summary>
        /// Reinitializes the TweenHelper system with current settings.
        /// Useful for testing settings changes in the editor.
        /// </summary>
        [MenuItem("LB/TweenHelper/Reinitialize System", false, 3)]
        public static void ReinitializeSystem()
        {
            if (Application.isPlaying)
            {
                TweenHelperBootstrapper.Reinitialize();
                Debug.Log("TweenHelper system reinitialized.");
            }
            else
            {
                Debug.LogWarning("TweenHelper system can only be reinitialized during play mode.");
            }
        }
    }
}
