using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    [CustomEditor(typeof(TweenPlayer))]
    public sealed class TweenPlayerEditor : UnityEditor.Editor
    {
        private VisualElement _root;
        private Label _validationLabel;
        private VisualElement _runtimeControls;
        private Button _previewButton;
        private Button _stopPreviewButton;

        private TweenPlayer Player => (TweenPlayer)target;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Rebuild;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= Rebuild;
            if (target != null && TweenRecipePreviewSession.Player == Player) TweenRecipePreviewSession.Stop("Preview stopped because the TweenPlayer Inspector closed.");
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement();
            _root.AddToClassList("tween-player-inspector");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Loags/TweenHelper/Editor/Recipes/TweenRecipeEditor.uss");
            if (styleSheet != null) _root.styleSheets.Add(styleSheet);
            Rebuild();
            _root.schedule.Execute(RefreshState).Every(250);
            return _root;
        }

        private void Rebuild()
        {
            if (_root == null || target == null) return;
            _root.Unbind();
            _root.Clear();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty recipeProperty = serializedObject.FindProperty("recipe");
            var recipeField = new ObjectField("Recipe")
            {
                objectType = typeof(TweenRecipe),
                allowSceneObjects = false,
                value = recipeProperty.objectReferenceValue
            };
            recipeField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(Player, "Change Tween Player Recipe");
                recipeProperty.objectReferenceValue = evt.newValue;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(Player);
                Rebuild();
            });
            _root.Add(recipeField);

            TweenRecipe recipe = recipeProperty.objectReferenceValue as TweenRecipe;
            var recipeActions = new VisualElement();
            recipeActions.AddToClassList("recipe-inspector-actions");
            var openButton = new Button(() =>
            {
                if (recipe != null) TweenRecipeEditorWindow.Open(recipe);
            }) { text = "Open Recipe" };
            openButton.SetEnabled(recipe != null);
            recipeActions.Add(openButton);
            var syncButton = new Button(SyncBindings) { text = "Sync Bindings" };
            syncButton.SetEnabled(recipe != null);
            recipeActions.Add(syncButton);
            var removeDeletedButton = new Button(RemoveDeletedBindings) { text = "Remove Deleted" };
            removeDeletedButton.SetEnabled(recipe != null && HasDeletedBindings(recipe));
            recipeActions.Add(removeDeletedButton);
            _root.Add(recipeActions);

            AddBindingFields(recipe);
            _root.Add(new PropertyField(serializedObject.FindProperty("playOnStart")));
            _root.Add(new PropertyField(serializedObject.FindProperty("useUnscaledTime")));
            _root.Add(new PropertyField(serializedObject.FindProperty("motionPreference")));
            _root.Add(new PropertyField(serializedObject.FindProperty("onStarted")));
            _root.Add(new PropertyField(serializedObject.FindProperty("onCompleted")));
            _root.Add(new PropertyField(serializedObject.FindProperty("onKilled")));

            _validationLabel = new Label();
            _validationLabel.AddToClassList("recipe-player-validation");
            _root.Add(_validationLabel);

            var previewActions = new VisualElement();
            previewActions.AddToClassList("recipe-inspector-actions");
            _previewButton = new Button(StartPreview) { text = "Preview" };
            previewActions.Add(_previewButton);
            _stopPreviewButton = new Button(() => TweenRecipePreviewSession.Stop()) { text = "Stop" };
            previewActions.Add(_stopPreviewButton);
            _root.Add(previewActions);

            _runtimeControls = new VisualElement();
            _runtimeControls.AddToClassList("recipe-runtime-controls");
            AddRuntimeButton("Play", () => Player.Play());
            AddRuntimeButton("Pause", () => Player.Pause());
            AddRuntimeButton("Resume", () => Player.Resume());
            AddRuntimeButton("Restart", () => Player.Restart());
            AddRuntimeButton("Rewind", () => Player.Rewind());
            AddRuntimeButton("Complete", () => Player.Complete());
            AddRuntimeButton("Kill", () => Player.Kill());
            _root.Add(_runtimeControls);

            _root.Bind(serializedObject);
            RefreshState();
        }

        private void AddBindingFields(TweenRecipe recipe)
        {
            var heading = new VisualElement();
            heading.AddToClassList("recipe-player-bindings-heading");
            heading.Add(CreateStyledLabel("Bindings", "recipe-section-title"));
            _root.Add(heading);

            if (recipe == null)
            {
                _root.Add(CreateHint("Assign a recipe, then use Sync Bindings."));
                return;
            }

            SerializedProperty playerBindings = serializedObject.FindProperty("bindings");
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition definition = recipe.Bindings[i];
                if (definition == null) continue;
                int playerIndex = FindPlayerBindingIndex(definition.Id);
                if (playerIndex < 0)
                {
                    _root.Add(CreateHint($"{definition.DisplayName}: missing — use Sync Bindings."));
                    continue;
                }

                SerializedProperty playerBinding = playerBindings.GetArrayElementAtIndex(playerIndex);
                var card = new VisualElement();
                card.AddToClassList("recipe-player-binding-card");
                card.Add(CreateStyledLabel(definition.DisplayName, "recipe-binding-name"));
                card.Add(CreateStyledLabel($"{definition.Kind} · {ShortId(definition.Id)}", "recipe-muted"));
                if (definition.Kind == TweenRecipeBindingKind.GameObject)
                {
                    card.Add(new PropertyField(playerBinding.FindPropertyRelative("target"), "Target"));
                }
                else
                {
                    card.Add(new PropertyField(playerBinding.FindPropertyRelative("targets"), "Ordered Targets"));
                }
                _root.Add(card);
            }

            for (int i = 0; i < playerBindings.arraySize; i++)
            {
                SerializedProperty playerBinding = playerBindings.GetArrayElementAtIndex(i);
                string id = playerBinding.FindPropertyRelative("bindingId").stringValue;
                if (RecipeHasBinding(recipe, id)) continue;
                var stale = CreateHint($"Deleted recipe binding retained: {ShortId(id)}");
                stale.AddToClassList("recipe-warning");
                _root.Add(stale);
            }
        }

        private void SyncBindings()
        {
            TweenRecipe recipe = serializedObject.FindProperty("recipe").objectReferenceValue as TweenRecipe;
            if (recipe == null) return;

            Undo.RecordObject(Player, "Sync Tween Player Bindings");
            serializedObject.Update();
            SerializedProperty playerBindings = serializedObject.FindProperty("bindings");
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition definition = recipe.Bindings[i];
                if (definition == null || FindPlayerBindingIndex(definition.Id) >= 0) continue;
                int index = playerBindings.arraySize;
                playerBindings.InsertArrayElementAtIndex(index);
                SerializedProperty binding = playerBindings.GetArrayElementAtIndex(index);
                binding.FindPropertyRelative("bindingId").stringValue = definition.Id;
                binding.FindPropertyRelative("target").objectReferenceValue = null;
                binding.FindPropertyRelative("targets").arraySize = 0;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(Player);
            Rebuild();
        }

        private void RemoveDeletedBindings()
        {
            TweenRecipe recipe = serializedObject.FindProperty("recipe").objectReferenceValue as TweenRecipe;
            if (recipe == null) return;

            Undo.RecordObject(Player, "Remove Deleted Tween Player Bindings");
            serializedObject.Update();
            SerializedProperty playerBindings = serializedObject.FindProperty("bindings");
            for (int i = playerBindings.arraySize - 1; i >= 0; i--)
            {
                string id = playerBindings.GetArrayElementAtIndex(i).FindPropertyRelative("bindingId").stringValue;
                if (!RecipeHasBinding(recipe, id)) playerBindings.DeleteArrayElementAtIndex(i);
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(Player);
            Rebuild();
        }

        private void AddRuntimeButton(string label, Action action)
        {
            var button = new Button(action) { text = label };
            _runtimeControls.Add(button);
        }

        private void RefreshState()
        {
            if (_runtimeControls == null || _validationLabel == null || target == null) return;
            _runtimeControls.SetEnabled(EditorApplication.isPlaying);
            bool canPreview = TweenRecipePreviewSession.CanPreview(Player, out string previewReason);
            _previewButton.SetEnabled(!EditorApplication.isPlaying && !TweenRecipePreviewSession.IsPreviewing && canPreview);
            _previewButton.tooltip = previewReason;
            _stopPreviewButton.SetEnabled(TweenRecipePreviewSession.Player == Player);
            TweenRecipeValidationResult validation = Player.Validate();
            _validationLabel.text = validation.GetSummary();
            _validationLabel.EnableInClassList("recipe-player-validation-valid", validation.IsValid);
        }

        private void StartPreview()
        {
            TweenRecipePreviewSession.Start(Player, out _);
            RefreshState();
        }

        private int FindPlayerBindingIndex(string id)
        {
            SerializedProperty bindings = serializedObject.FindProperty("bindings");
            for (int i = 0; i < bindings.arraySize; i++)
            {
                if (bindings.GetArrayElementAtIndex(i).FindPropertyRelative("bindingId").stringValue == id) return i;
            }

            return -1;
        }

        private bool HasDeletedBindings(TweenRecipe recipe)
        {
            SerializedProperty bindings = serializedObject.FindProperty("bindings");
            for (int i = 0; i < bindings.arraySize; i++)
            {
                string id = bindings.GetArrayElementAtIndex(i).FindPropertyRelative("bindingId").stringValue;
                if (!RecipeHasBinding(recipe, id)) return true;
            }

            return false;
        }

        private static bool RecipeHasBinding(TweenRecipe recipe, string id)
        {
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                if (recipe.Bindings[i] != null && recipe.Bindings[i].Id == id) return true;
            }

            return false;
        }

        private static Label CreateStyledLabel(string text, string className)
        {
            var label = new Label(text);
            label.AddToClassList(className);
            return label;
        }

        private static Label CreateHint(string text)
        {
            var label = new Label(text);
            label.AddToClassList("recipe-empty-state");
            return label;
        }

        private static string ShortId(string id) => string.IsNullOrEmpty(id) ? "Missing ID" : id.Substring(0, Math.Min(8, id.Length));
    }
}
