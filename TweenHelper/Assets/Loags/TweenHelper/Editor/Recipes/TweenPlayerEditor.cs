using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    [CustomEditor(typeof(TweenPlayer))]
    [CanEditMultipleObjects]
    public sealed class TweenPlayerEditor : UnityEditor.Editor
    {
        private VisualElement _root;
        private VisualElement _configuration;
        private bool _isUndoRedo;
        private Label _validationLabel;
        private VisualElement _runtimeControls;
        private Button _previewButton;
        private Button _stopPreviewButton;
        private string _category = "All";
        private string _search = string.Empty;
        private string _bindingSignature;
        private int _catalogVersion;

        private TweenPlayer Player => (TweenPlayer)target;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
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

        private void OnUndoRedo()
        {
            _isUndoRedo = true;
            try
            {
                Rebuild();
            }
            finally
            {
                _isUndoRedo = false;
            }
        }

        private void Rebuild()
        {
            if (_root == null || target == null) return;
            if (TweenRecipePreviewSession.Player == Player) TweenRecipePreviewSession.Stop();
            _runtimeControls = null;
            _validationLabel = null;
            _root.Unbind();
            _root.Clear();
            _configuration = new VisualElement();
            _root.Add(_configuration);
            serializedObject.UpdateIfRequiredOrScript();

            var modeField = new PropertyField(serializedObject.FindProperty("mode"));
            modeField.RegisterValueChangeCallback(_ => _root.schedule.Execute(Rebuild));
            _configuration.Add(modeField);
            if (serializedObject.FindProperty("mode").hasMultipleDifferentValues)
            {
                _configuration.Add(CreateHint("Choose a common mode to edit the selected players together."));
                _root.Bind(serializedObject);
                return;
            }

            if (Player.Mode == TweenPlayerMode.Preset)
            {
                AddPresetFields();
            }
            else
            {
                if (!_isUndoRedo) TweenPlayerBindingSynchronization.Synchronize(targets.Cast<TweenPlayer>());
                serializedObject.UpdateIfRequiredOrScript();

                SerializedProperty recipeProperty = serializedObject.FindProperty("recipe");
                var recipeField = new ObjectField("Recipe")
                {
                    objectType = typeof(TweenRecipe),
                    allowSceneObjects = false,
                    showMixedValue = recipeProperty.hasMultipleDifferentValues,
                    value = recipeProperty.objectReferenceValue
                };
                recipeField.RegisterValueChangedCallback(evt =>
                {
                    recipeProperty.objectReferenceValue = evt.newValue;
                    serializedObject.ApplyModifiedProperties();
                    TweenPlayerBindingSynchronization.Synchronize(targets.Cast<TweenPlayer>());
                    Rebuild();
                });
                _configuration.Add(recipeField);

                TweenRecipe recipe = recipeProperty.objectReferenceValue as TweenRecipe;
                var recipeActions = new VisualElement();
                recipeActions.AddToClassList("recipe-inspector-actions");
                var openButton = new Button(() =>
                {
                    if (recipe != null) TweenRecipeEditorWindow.Open(recipe);
                }) { text = "Open Recipe" };
                openButton.SetEnabled(recipe != null && !recipeProperty.hasMultipleDifferentValues);
                recipeActions.Add(openButton);
                var removeDeletedButton = new Button(RemoveDeletedBindings) { text = "Remove Deleted" };
                removeDeletedButton.SetEnabled(targets.Length == 1 && recipe != null && HasDeletedBindings(recipe));
                recipeActions.Add(removeDeletedButton);
                _configuration.Add(recipeActions);

                if (targets.Length == 1) AddBindingFields(recipe);
                else _configuration.Add(CreateHint("Recipe selection is shared. Select one player to assign its individual targets."));
            }
            _bindingSignature = GetBindingSignature();
            _catalogVersion = TweenPresetRegistry.Version;
            _configuration.Add(new PropertyField(serializedObject.FindProperty("playOnStart")));
            _configuration.Add(new PropertyField(serializedObject.FindProperty("useUnscaledTime")));
            _configuration.Add(new PropertyField(serializedObject.FindProperty("motionPreference")));
            var events = new Foldout { text = "Events", value = false };
            events.Add(new PropertyField(serializedObject.FindProperty("onStarted")));
            events.Add(new PropertyField(serializedObject.FindProperty("onCompleted")));
            events.Add(new PropertyField(serializedObject.FindProperty("onKilled")));
            _configuration.Add(events);

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

        private void AddPresetFields()
        {
            IReadOnlyList<ITweenPreset> presets = PresetSelectionCatalog.Presets;
            var categories = new List<string> { "All" };
            categories.AddRange(presets.Select(PresetSelectionCatalog.GetCategory).Distinct().OrderBy(category => category, StringComparer.Ordinal));
            if (!categories.Contains(_category)) _category = "All";
            var categoryField = new PopupField<string>("Category", categories, _category);
            var searchField = new ToolbarSearchField { value = _search };
            searchField.tooltip = "Search registered animation names and descriptions";
            var animationField = new PopupField<string>("Animation", new List<string> { "Select an animation" }, 0);
            var description = new Label();
            description.style.whiteSpace = WhiteSpace.Normal;

            void RefreshChoices()
            {
                string selected = serializedObject.FindProperty("presetName").stringValue;
                var choices = presets.Where(preset => (_category == "All" || PresetSelectionCatalog.GetCategory(preset) == _category) && (string.IsNullOrWhiteSpace(_search) || preset.PresetName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0 || (preset.Description ?? string.Empty).IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0)).Select(preset => preset.PresetName).ToList();
                if (!string.IsNullOrEmpty(selected) && !choices.Contains(selected)) choices.Insert(0, selected);
                choices.Insert(0, "Select an animation");
                animationField.choices = choices;
                animationField.SetValueWithoutNotify(string.IsNullOrEmpty(selected) ? choices[0] : selected);
                animationField.showMixedValue = serializedObject.FindProperty("presetName").hasMultipleDifferentValues;
                ITweenPreset current = TweenPresetRegistry.GetPresetByName(selected);
                description.text = current == null ? "Choose a registered preset. An unresolved saved name is retained until you replace it." : $"{current.Description}\nDefault duration: {current.DefaultDuration:0.###}s · {PresetSelectionCatalog.GetCategory(current)}";
            }

            categoryField.RegisterValueChangedCallback(evt => { _category = evt.newValue; RefreshChoices(); });
            searchField.RegisterValueChangedCallback(evt => { _search = evt.newValue; RefreshChoices(); });
            animationField.RegisterValueChangedCallback(evt =>
            {
                TweenRecipePreviewSession.Stop();
                serializedObject.FindProperty("presetName").stringValue = evt.newValue == "Select an animation" ? string.Empty : evt.newValue;
                serializedObject.ApplyModifiedProperties();
                RefreshChoices();
                _root.schedule.Execute(Rebuild);
            });
            _configuration.Add(categoryField);
            _configuration.Add(searchField);
            _configuration.Add(animationField);
            _configuration.Add(description);
            _configuration.Add(new PropertyField(serializedObject.FindProperty("targetOverride"), "Target Override (optional)"));
            _configuration.Add(CreateHint("Uses this GameObject when Target Override is empty."));
            var overrideField = new PropertyField(serializedObject.FindProperty("overrideDuration"));
            overrideField.RegisterValueChangeCallback(_ => _root.schedule.Execute(Rebuild));
            _configuration.Add(overrideField);
            if (serializedObject.FindProperty("overrideDuration").boolValue || serializedObject.FindProperty("overrideDuration").hasMultipleDifferentValues) _configuration.Add(new PropertyField(serializedObject.FindProperty("duration")));
            if (!serializedObject.FindProperty("presetName").hasMultipleDifferentValues)
            {
                PresetOverrideFields supported = TweenPlayerOverrides.GetSupportedFields(TweenPresetRegistry.GetPresetByName(Player.PresetName));
                foreach (PresetOverrideFields field in Enum.GetValues(typeof(PresetOverrideFields)))
                {
                    if (field == PresetOverrideFields.None || !supported.HasFlag(field)) continue;
                    string name = field.ToString();
                    SerializedProperty values = serializedObject.FindProperty("presetOverrides");
                    SerializedProperty toggle = values.FindPropertyRelative("override" + name);
                    var toggleField = new PropertyField(toggle);
                    toggleField.RegisterValueChangeCallback(_ => _root.schedule.Execute(Rebuild));
                    _configuration.Add(toggleField);
                    if (toggle.boolValue || toggle.hasMultipleDifferentValues) _configuration.Add(new PropertyField(values.FindPropertyRelative(char.ToLowerInvariant(name[0]) + name.Substring(1))));
                }
            }
            RefreshChoices();
        }

        private string GetBindingSignature() => string.Join("|", targets.Cast<TweenPlayer>().Select(player => player.Mode + ":" + player.PresetName + ":" + (player.Recipe == null ? "none" : AssetDatabase.GetAssetPath(player.Recipe) + ":" + string.Join(",", player.Recipe.Bindings.Select(binding => binding == null ? "null" : binding.Id + ":" + binding.DisplayName + ":" + binding.Kind)))));

        private void AddBindingFields(TweenRecipe recipe)
        {
            var heading = new VisualElement();
            heading.AddToClassList("recipe-player-bindings-heading");
            heading.Add(CreateStyledLabel("Bindings", "recipe-section-title"));
            _configuration.Add(heading);

            if (recipe == null)
            {
                _configuration.Add(CreateHint("Assign a recipe to create its target fields automatically."));
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
                    _configuration.Add(CreateHint($"{definition.DisplayName}: waiting for binding synchronization."));
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
                _configuration.Add(card);
            }

            for (int i = 0; i < playerBindings.arraySize; i++)
            {
                SerializedProperty playerBinding = playerBindings.GetArrayElementAtIndex(i);
                string id = playerBinding.FindPropertyRelative("bindingId").stringValue;
                if (RecipeHasBinding(recipe, id)) continue;
                var stale = CreateHint($"Deleted recipe binding retained: {ShortId(id)}");
                stale.AddToClassList("recipe-warning");
                _configuration.Add(stale);
            }
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
            if (GetBindingSignature() != _bindingSignature || TweenPresetRegistry.Version != _catalogVersion)
            {
                if (TweenRecipePreviewSession.Player == Player) TweenRecipePreviewSession.Stop();
                Rebuild();
                return;
            }
            _runtimeControls.SetEnabled(EditorApplication.isPlaying && targets.Length == 1);
            _configuration.SetEnabled(!TweenRecipePreviewSession.IsPreviewing);
            bool canPreview = TweenRecipePreviewSession.CanPreview(Player, out string previewReason);
            _previewButton.SetEnabled(targets.Length == 1 && !EditorApplication.isPlaying && !TweenRecipePreviewSession.IsPreviewing && canPreview);
            _previewButton.tooltip = previewReason;
            _stopPreviewButton.SetEnabled(TweenRecipePreviewSession.Player == Player);
            TweenRecipeValidationResult validation = Player.Validate();
            _validationLabel.text = validation.IsValid && Player.Mode == TweenPlayerMode.Preset ? "Animation is ready." : validation.GetSummary();
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
