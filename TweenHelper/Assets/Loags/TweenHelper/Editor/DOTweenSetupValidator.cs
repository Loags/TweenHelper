using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    public static class DOTweenSetupValidator
    {
        private static readonly Version MinimumSupportedVersion = new Version(1, 3, 30);

        [MenuItem("Tools/TweenHelper/Validate/DOTween Setup", false, 19)]
        public static void Validate()
        {
            bool isValid = TryValidate(out string message);
            LogResult(isValid, message);
            EditorUtility.DisplayDialog(isValid ? "DOTween Setup Valid" : "DOTween Setup Required", message, "OK");
        }

        [MenuItem("Tools/TweenHelper/Validate/DOTween Setup (Non-Interactive)", false, 119)]
        public static void ValidateNonInteractive()
        {
            bool isValid = TryValidate(out string message);
            LogResult(isValid, message);
        }

        public static bool TryValidate(out string message)
        {
            var errors = new List<string>();
            var notes = new List<string>();

            if (Type.GetType("DG.Tweening.DOTween, DOTween") == null)
            {
                errors.Add("DOTween.dll is not loaded. Install DOTween 1.3.030 or newer before using TweenHelper.");
            }

            if (Type.GetType("DG.Tweening.DOTweenModuleUI, DOTween.Modules") == null)
            {
                errors.Add("DOTween's UI module is unavailable. Open Tools > Demigiant > DOTween Utility Panel and run Setup DOTween.");
            }

            if (AssetDatabase.FindAssets("t:DOTweenSettings").Length == 0)
            {
                notes.Add("No DOTweenSettings asset was found. DOTween can use defaults, but running its Setup panel is recommended.");
            }

            string version = DOTween.Version;
            if (!Version.TryParse(version, out Version parsedVersion))
            {
                errors.Add($"DOTween version '{version}' could not be parsed. Install DOTween {MinimumSupportedVersion} or newer.");
            }
            else if (parsedVersion < MinimumSupportedVersion)
            {
                errors.Add($"DOTween {version} is older than the supported minimum {MinimumSupportedVersion}.");
            }

            if (errors.Count == 0)
            {
                message = $"DOTween {version} is loaded and the required modules are available.";
                if (notes.Count > 0) message += $"\n\n{string.Join("\n", notes)}";
                return true;
            }

            message = string.Join("\n", errors);
            return false;
        }

        private static void LogResult(bool isValid, string message)
        {
            if (isValid)
            {
                Debug.Log($"TweenHelper DOTween validation passed. {message}");
                return;
            }

            Debug.LogError($"TweenHelper DOTween validation failed:\n{message}");
        }
    }
}
