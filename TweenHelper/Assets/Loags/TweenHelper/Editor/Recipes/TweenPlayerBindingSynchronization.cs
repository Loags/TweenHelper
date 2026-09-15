using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace LB.TweenHelper.Editor
{
    [InitializeOnLoad]
    internal static class TweenPlayerBindingSynchronization
    {
        static TweenPlayerBindingSynchronization()
        {
            EditorApplication.projectChanged += SynchronizeLoadedPlayers;
            EditorApplication.delayCall += SynchronizeLoadedPlayers;
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode) SynchronizeLoadedPlayers();
            };
        }

        private static void SynchronizeLoadedPlayers() => Synchronize(Resources.FindObjectsOfTypeAll<TweenPlayer>().Where(player => !EditorUtility.IsPersistent(player) && player.gameObject.scene.IsValid()));

        public static void Synchronize(IEnumerable<TweenPlayer> players)
        {
            if (EditorApplication.isPlaying || TweenRecipePreviewSession.IsPreviewing) return;
            foreach (TweenPlayer player in players)
            {
                if (player.Mode != TweenPlayerMode.Recipe || player.Recipe == null) continue;
                var serialized = new SerializedObject(player);
                SerializedProperty bindings = serialized.FindProperty("bindings");
                var existing = new HashSet<string>();
                for (int i = 0; i < bindings.arraySize; i++) existing.Add(bindings.GetArrayElementAtIndex(i).FindPropertyRelative("bindingId").stringValue);
                foreach (TweenRecipeBindingDefinition definition in player.Recipe.Bindings)
                {
                    if (definition == null || string.IsNullOrEmpty(definition.Id) || !existing.Add(definition.Id)) continue;
                    int index = bindings.arraySize;
                    bindings.InsertArrayElementAtIndex(index);
                    SerializedProperty binding = bindings.GetArrayElementAtIndex(index);
                    binding.FindPropertyRelative("bindingId").stringValue = definition.Id;
                    binding.FindPropertyRelative("target").objectReferenceValue = null;
                    binding.FindPropertyRelative("targets").arraySize = 0;
                }
                serialized.ApplyModifiedProperties();
            }
        }
    }
}
