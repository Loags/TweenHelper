using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LB.TweenHelper.Editor
{
    [Serializable]
    internal sealed class PresetBrowserPreferences
    {
        [SerializeField] private List<string> favorites = new List<string>();
        [SerializeField] private List<string> recent = new List<string>();

        private static string Key => $"LB.TweenHelper.Browser.{Hash128.Compute(Application.dataPath)}";

        internal static PresetBrowserPreferences Load()
        {
            try
            {
                var preferences = JsonUtility.FromJson<PresetBrowserPreferences>(EditorPrefs.GetString(Key, "{}")) ?? new PresetBrowserPreferences();
                preferences.favorites ??= new List<string>();
                preferences.recent ??= new List<string>();
                return preferences;
            }
            catch (ArgumentException)
            {
                return new PresetBrowserPreferences();
            }
        }

        internal bool IsFavorite(string id) => favorites.Contains(id);
        internal int RecentIndex(string id) => recent.IndexOf(id);

        internal void ToggleFavorite(string id)
        {
            if (!favorites.Remove(id)) favorites.Add(id);
            Save();
        }

        internal void Visit(string id)
        {
            if (recent.Count > 0 && recent[0] == id) return;
            recent.Remove(id);
            recent.Insert(0, id);
            if (recent.Count > 20) recent.RemoveRange(20, recent.Count - 20);
            Save();
        }

        private void Save() => EditorPrefs.SetString(Key, JsonUtility.ToJson(this));

        internal static bool MatchesUseCase(PresetBrowserEntry entry, string useCase)
        {
            switch (useCase)
            {
                case "Menus": return entry.PreviewKind == PresetBrowserPreviewKind.UiSequence || entry.PreviewKind == PresetBrowserPreviewKind.UiTarget;
                case "Inventory": return entry.IsCollection || entry.PreviewKind == PresetBrowserPreviewKind.CollectionLayout;
                case "Rewards": return entry.PreviewKind == PresetBrowserPreviewKind.WorldToUi || entry.Name.IndexOf("Reward", StringComparison.OrdinalIgnoreCase) >= 0 || entry.Name.IndexOf("Pickup", StringComparison.OrdinalIgnoreCase) >= 0;
                case "Text": return entry.PreviewKind == PresetBrowserPreviewKind.Text;
                case "Progress": return entry.PreviewKind == PresetBrowserPreviewKind.ProgressImage || entry.PreviewKind == PresetBrowserPreviewKind.ProgressSlider;
                case "Camera": return entry.PreviewKind == PresetBrowserPreviewKind.Camera;
                default: return true;
            }
        }
    }
}
