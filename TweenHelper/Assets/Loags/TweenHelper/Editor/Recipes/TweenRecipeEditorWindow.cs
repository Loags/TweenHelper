using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    public sealed class TweenRecipeEditorWindow : EditorWindow
    {
        private const string StylePath = "Assets/Loags/TweenHelper/Editor/Recipes/TweenRecipeEditor.uss";

        private readonly List<int> _bindingIndices = new List<int>();
        private TweenRecipe _recipe;
        private SerializedObject _serializedObject;
        private ObjectField _recipeField;
        private ListView _bindingList;
        private Button _removeBindingButton;
        private ToolbarButton _previewButton;
        private ToolbarButton _stopButton;
        private TweenRecipePalette _palette;
        private TweenRecipeTimelineView _timeline;
        private TweenRecipeInspectorView _inspector;
        private VisualElement _validationList;
        private Label _status;
        private Label _previewStatus;
        private string _selectedNodeId;
        private string _selectedBindingId;
        private bool _suppressSelection;
        private bool _refreshScheduled;

        [MenuItem("Tools/Tween Helper/Recipe Editor")]
        public static void Open()
        {
            TweenRecipeEditorWindow window = GetWindow<TweenRecipeEditorWindow>();
            window.titleContent = new GUIContent("Recipe Editor");
            window.minSize = new Vector2(920f, 520f);
            window.Show();
        }

        public static void Open(TweenRecipe recipe)
        {
            Open();
            TweenRecipeEditorWindow window = GetWindow<TweenRecipeEditorWindow>();
            window.SetRecipe(recipe);
            window.Focus();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            EntityId entityId = EntityId.FromULong(unchecked((ulong)(long)instanceId));
            if (EditorUtility.EntityIdToObject(entityId) is not TweenRecipe recipe) return false;
            Open(recipe);
            return true;
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            if (TweenRecipePreviewSession.IsPreviewing) TweenRecipePreviewSession.Stop("Preview stopped because the Recipe Editor closed.");
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            rootVisualElement.AddToClassList("recipe-editor-root");
            rootVisualElement.focusable = true;
            rootVisualElement.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            if (styleSheet != null) rootVisualElement.styleSheets.Add(styleSheet);

            BuildToolbar();
            BuildContent();
            BuildStatus();
            RefreshAll();
            rootVisualElement.schedule.Execute(RefreshPreviewState).Every(200);
        }

        private void BuildToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.AddToClassList("recipe-toolbar");

            _recipeField = new ObjectField("Recipe")
            {
                objectType = typeof(TweenRecipe),
                allowSceneObjects = false
            };
            _recipeField.AddToClassList("recipe-asset-field");
            _recipeField.RegisterValueChangedCallback(evt => SetRecipe(evt.newValue as TweenRecipe));
            toolbar.Add(_recipeField);

            toolbar.Add(new ToolbarButton(CreateRecipe) { text = "Create" });
            toolbar.Add(new ToolbarButton(RefreshAll) { text = "Validate" });
            _previewButton = new ToolbarButton(StartPreview) { text = "Preview" };
            toolbar.Add(_previewButton);
            _stopButton = new ToolbarButton(StopPreview) { text = "Stop" };
            toolbar.Add(_stopButton);
            toolbar.Add(new ToolbarSpacer());
            toolbar.Add(new ToolbarButton(() => _timeline?.FrameAll()) { text = "Frame All" });
            rootVisualElement.Add(toolbar);
        }

        private void BuildContent()
        {
            var content = new VisualElement();
            content.AddToClassList("recipe-editor-content");

            var left = new VisualElement();
            left.AddToClassList("recipe-left-panel");
            var bindingHeading = new VisualElement();
            bindingHeading.AddToClassList("recipe-panel-heading");
            bindingHeading.Add(CreateLabel("Bindings", "recipe-section-title"));
            var bindingActions = new VisualElement();
            bindingActions.AddToClassList("recipe-inline-actions");
            bindingActions.Add(new Button(AddBinding) { text = "+" });
            _removeBindingButton = new Button(RemoveSelectedBinding) { text = "−" };
            bindingActions.Add(_removeBindingButton);
            bindingHeading.Add(bindingActions);
            left.Add(bindingHeading);

            _bindingList = new ListView(_bindingIndices, 38, MakeBindingRow, BindBindingRow)
            {
                selectionType = SelectionType.Single
            };
            _bindingList.AddToClassList("recipe-binding-list");
            _bindingList.selectionChanged += _ => OnBindingSelectionChanged();
            left.Add(_bindingList);

            _palette = new TweenRecipePalette(AddNode);
            left.Add(_palette);
            content.Add(left);

            var center = new VisualElement();
            center.AddToClassList("recipe-center-panel");
            center.Add(CreateLabel("Timeline", "recipe-section-title"));
            _timeline = new TweenRecipeTimelineView(SelectNode, MoveNode);
            center.Add(_timeline);
            content.Add(center);

            var right = new VisualElement();
            right.AddToClassList("recipe-right-panel");
            _inspector = new TweenRecipeInspectorView(ScheduleRefresh, SetNodeOperation, SetNodeBinding, SetNodeSecondaryBinding, DuplicateNode, DeleteNode);
            right.Add(_inspector);
            content.Add(right);

            rootVisualElement.Add(content);
        }

        private void BuildStatus()
        {
            var footer = new VisualElement();
            footer.AddToClassList("recipe-validation-panel");
            _status = new Label();
            _status.AddToClassList("recipe-status");
            footer.Add(_status);
            _previewStatus = new Label();
            _previewStatus.AddToClassList("recipe-preview-status");
            footer.Add(_previewStatus);
            _validationList = new VisualElement();
            _validationList.AddToClassList("recipe-validation-list");
            footer.Add(_validationList);
            rootVisualElement.Add(footer);
        }

        private VisualElement MakeBindingRow()
        {
            var row = new VisualElement();
            row.AddToClassList("recipe-binding-row");
            var name = new Label { name = "name" };
            name.AddToClassList("recipe-binding-name");
            row.Add(name);
            var kind = new Label { name = "kind" };
            kind.AddToClassList("recipe-muted");
            row.Add(kind);
            return row;
        }

        private void BindBindingRow(VisualElement element, int index)
        {
            if (_recipe == null || index < 0 || index >= _recipe.Bindings.Count) return;
            TweenRecipeBindingDefinition binding = _recipe.Bindings[index];
            element.Q<Label>("name").text = binding == null || string.IsNullOrWhiteSpace(binding.DisplayName) ? "(Unnamed)" : binding.DisplayName;
            element.Q<Label>("kind").text = binding == null ? "Missing" : binding.Kind.ToString();
        }

        private void SetRecipe(TweenRecipe recipe)
        {
            if (_recipe == recipe && _serializedObject != null) return;
            _recipe = recipe;
            _serializedObject = recipe != null ? new SerializedObject(recipe) : null;
            _selectedNodeId = null;
            _selectedBindingId = null;
            if (_recipeField != null) _recipeField.SetValueWithoutNotify(recipe);
            RefreshAll();
        }

        private void CreateRecipe()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Tween Recipe", "TweenRecipe", "asset", "Choose where to save the TweenRecipe asset.");
            if (string.IsNullOrEmpty(path)) return;

            path = AssetDatabase.GenerateUniqueAssetPath(path);
            var recipe = CreateInstance<TweenRecipe>();
            AssetDatabase.CreateAsset(recipe, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = recipe;
            SetRecipe(recipe);
        }

        private void RefreshAll()
        {
            _refreshScheduled = false;
            if (_timeline == null) return;
            _serializedObject?.UpdateIfRequiredOrScript();

            TweenRecipeValidationResult validation = TweenRecipeValidator.Validate(_recipe);
            RestoreValidSelection();

            _bindingIndices.Clear();
            if (_recipe != null)
            {
                for (int i = 0; i < _recipe.Bindings.Count; i++) _bindingIndices.Add(i);
            }

            _bindingList?.Rebuild();
            RestoreBindingListSelection();
            if (_removeBindingButton != null) _removeBindingButton.SetEnabled(_recipe != null && !string.IsNullOrEmpty(_selectedBindingId));
            _palette?.SetEnabled(_recipe != null);
            _timeline.Refresh(_recipe, _selectedNodeId, validation);
            _inspector.Show(_recipe, _serializedObject, _selectedNodeId, _selectedBindingId);
            RefreshValidation(validation);
            RefreshPreviewState();
        }

        private void RefreshValidation(TweenRecipeValidationResult validation)
        {
            _validationList.Clear();
            if (_recipe == null)
            {
                _status.text = "Select or create a TweenRecipe.";
                _status.RemoveFromClassList("recipe-status-valid");
                return;
            }

            _status.text = validation.IsValid ? "Valid recipe" : $"{validation.Messages.Count} validation message(s)";
            _status.EnableInClassList("recipe-status-valid", validation.IsValid);

            for (int i = 0; i < validation.Messages.Count; i++)
            {
                TweenRecipeValidationMessage message = validation.Messages[i];
                var button = new Button(() => NavigateTo(message))
                {
                    text = $"{message.Severity}: {message.Text}"
                };
                button.AddToClassList("recipe-validation-message");
                button.AddToClassList($"recipe-validation-{message.Severity.ToString().ToLowerInvariant()}");
                button.SetEnabled(!string.IsNullOrEmpty(message.NodeId) || !string.IsNullOrEmpty(message.BindingId));
                _validationList.Add(button);
            }
        }

        private void NavigateTo(TweenRecipeValidationMessage message)
        {
            if (!string.IsNullOrEmpty(message.NodeId))
            {
                SelectNode(message.NodeId);
            }
            else if (!string.IsNullOrEmpty(message.BindingId))
            {
                SelectBinding(message.BindingId);
            }
        }

        private void AddBinding()
        {
            if (_serializedObject == null) return;
            Undo.RecordObject(_recipe, "Add Tween Recipe Binding");
            _serializedObject.Update();
            SerializedProperty bindings = _serializedObject.FindProperty("bindings");
            int index = bindings.arraySize;
            bindings.InsertArrayElementAtIndex(index);
            SerializedProperty binding = bindings.GetArrayElementAtIndex(index);
            string id = Guid.NewGuid().ToString("N");
            binding.FindPropertyRelative("id").stringValue = id;
            binding.FindPropertyRelative("displayName").stringValue = $"Binding {index + 1}";
            binding.FindPropertyRelative("kind").enumValueIndex = (int)TweenRecipeBindingKind.GameObject;
            Commit();
            _selectedBindingId = id;
            _selectedNodeId = null;
            RefreshAll();
        }

        private void RemoveSelectedBinding()
        {
            int index = FindBindingIndex(_selectedBindingId);
            if (_serializedObject == null || index < 0) return;
            Undo.RecordObject(_recipe, "Remove Tween Recipe Binding");
            _serializedObject.Update();
            _serializedObject.FindProperty("bindings").DeleteArrayElementAtIndex(index);
            Commit();
            _selectedBindingId = null;
            RefreshAll();
        }

        private void AddNode(TweenRecipeOperation operation)
        {
            if (_serializedObject == null) return;
            Undo.RecordObject(_recipe, "Add Tween Recipe Node");
            _serializedObject.Update();
            SerializedProperty nodes = _serializedObject.FindProperty("nodes");
            int index = nodes.arraySize;
            nodes.InsertArrayElementAtIndex(index);
            SerializedProperty node = nodes.GetArrayElementAtIndex(index);
            string id = Guid.NewGuid().ToString("N");
            node.FindPropertyRelative("id").stringValue = id;
            node.FindPropertyRelative("label").stringValue = TweenRecipeOperationCatalog.GetDisplayName(operation);
            node.FindPropertyRelative("placement").enumValueIndex = (int)TweenRecipePlacement.Then;
            node.FindPropertyRelative("operation").enumValueIndex = (int)operation;
            string bindingId = TweenRecipeOperationCatalog.UsesBinding(operation) ? FindFirstBindingId(TweenRecipeOperationCatalog.GetBindingKind(operation)) : string.Empty;
            node.FindPropertyRelative("bindingId").stringValue = bindingId;
            node.FindPropertyRelative("secondaryBindingId").stringValue = TweenRecipeOperationCatalog.UsesSecondaryBinding(operation)
                ? FindFirstBindingId(TweenRecipeOperationCatalog.GetSecondaryBindingKind(operation), bindingId)
                : string.Empty;
            node.FindPropertyRelative("duration").floatValue = TweenRecipeOperationCatalog.GetDefaultDuration(operation);
            node.FindPropertyRelative("delay").floatValue = 0f;
            node.FindPropertyRelative("ease").enumValueIndex = (int)Ease.OutCubic;
            SetParameterDefaults(node.FindPropertyRelative("parameters"), operation);
            Commit();
            _selectedNodeId = id;
            _selectedBindingId = null;
            RefreshAll();
        }

        private void SetNodeOperation(int index, TweenRecipeOperation operation)
        {
            if (_serializedObject == null || index < 0 || index >= _recipe.Nodes.Count) return;
            Undo.RecordObject(_recipe, "Change Tween Recipe Operation");
            _serializedObject.Update();
            SerializedProperty node = _serializedObject.FindProperty("nodes").GetArrayElementAtIndex(index);
            node.FindPropertyRelative("operation").enumValueIndex = (int)operation;
            node.FindPropertyRelative("duration").floatValue = TweenRecipeOperationCatalog.GetDefaultDuration(operation);
            if (!TweenRecipeOperationCatalog.UsesBinding(operation))
            {
                node.FindPropertyRelative("placement").enumValueIndex = (int)TweenRecipePlacement.Then;
                node.FindPropertyRelative("bindingId").stringValue = string.Empty;
            }
            else
            {
                string currentId = node.FindPropertyRelative("bindingId").stringValue;
                if (!BindingMatches(currentId, TweenRecipeOperationCatalog.GetBindingKind(operation)))
                {
                    currentId = FindFirstBindingId(TweenRecipeOperationCatalog.GetBindingKind(operation));
                    node.FindPropertyRelative("bindingId").stringValue = currentId;
                }
            }

            if (TweenRecipeOperationCatalog.UsesSecondaryBinding(operation))
            {
                string primaryId = node.FindPropertyRelative("bindingId").stringValue;
                string secondaryId = node.FindPropertyRelative("secondaryBindingId").stringValue;
                if (secondaryId == primaryId || !BindingMatches(secondaryId, TweenRecipeOperationCatalog.GetSecondaryBindingKind(operation)))
                {
                    node.FindPropertyRelative("secondaryBindingId").stringValue = FindFirstBindingId(TweenRecipeOperationCatalog.GetSecondaryBindingKind(operation), primaryId);
                }
            }
            else
            {
                node.FindPropertyRelative("secondaryBindingId").stringValue = string.Empty;
            }

            SetParameterDefaults(node.FindPropertyRelative("parameters"), operation);
            Commit();
            RefreshAll();
        }

        private void SetNodeBinding(int index, string bindingId)
        {
            if (_serializedObject == null || index < 0 || index >= _recipe.Nodes.Count) return;
            Undo.RecordObject(_recipe, "Change Tween Recipe Binding");
            _serializedObject.Update();
            _serializedObject.FindProperty("nodes").GetArrayElementAtIndex(index).FindPropertyRelative("bindingId").stringValue = bindingId;
            Commit();
            RefreshAll();
        }

        private void SetNodeSecondaryBinding(int index, string bindingId)
        {
            if (_serializedObject == null || index < 0 || index >= _recipe.Nodes.Count) return;
            Undo.RecordObject(_recipe, "Change Tween Recipe Destination Binding");
            _serializedObject.Update();
            _serializedObject.FindProperty("nodes").GetArrayElementAtIndex(index).FindPropertyRelative("secondaryBindingId").stringValue = bindingId;
            Commit();
            RefreshAll();
        }

        private void MoveNode(int sourceIndex, int destinationIndex, TweenRecipePlacement placement)
        {
            if (_serializedObject == null || sourceIndex < 0 || destinationIndex < 0 || sourceIndex >= _recipe.Nodes.Count || destinationIndex >= _recipe.Nodes.Count) return;
            Undo.RecordObject(_recipe, "Reorder Tween Recipe Node");
            _serializedObject.Update();
            SerializedProperty nodes = _serializedObject.FindProperty("nodes");
            nodes.MoveArrayElement(sourceIndex, destinationIndex);
            SerializedProperty movedNode = nodes.GetArrayElementAtIndex(destinationIndex);
            TweenRecipeOperation operation = (TweenRecipeOperation)movedNode.FindPropertyRelative("operation").enumValueIndex;
            movedNode.FindPropertyRelative("placement").enumValueIndex = destinationIndex == 0 || operation == TweenRecipeOperation.Delay
                ? (int)TweenRecipePlacement.Then
                : (int)placement;
            NormalizeFirstNode(nodes);
            Commit();
            RefreshAll();
        }

        private void DuplicateNode(int index)
        {
            if (_serializedObject == null || index < 0 || index >= _recipe.Nodes.Count) return;
            TweenRecipeNode sourceNode = _recipe.Nodes[index];
            Undo.RecordObject(_recipe, "Duplicate Tween Recipe Node");
            _serializedObject.Update();
            SerializedProperty nodes = _serializedObject.FindProperty("nodes");
            int addedIndex = nodes.arraySize;
            nodes.InsertArrayElementAtIndex(addedIndex);
            SerializedProperty duplicate = nodes.GetArrayElementAtIndex(addedIndex);
            CopyNode(sourceNode, duplicate);
            string id = Guid.NewGuid().ToString("N");
            duplicate.FindPropertyRelative("id").stringValue = id;
            duplicate.FindPropertyRelative("label").stringValue = $"{sourceNode.Label} Copy";
            nodes.MoveArrayElement(addedIndex, index + 1);
            Commit();
            _selectedNodeId = id;
            _selectedBindingId = null;
            RefreshAll();
        }

        private void DeleteNode(int index)
        {
            if (_serializedObject == null || index < 0 || index >= _recipe.Nodes.Count) return;
            Undo.RecordObject(_recipe, "Delete Tween Recipe Node");
            _serializedObject.Update();
            SerializedProperty nodes = _serializedObject.FindProperty("nodes");
            nodes.DeleteArrayElementAtIndex(index);
            NormalizeFirstNode(nodes);
            Commit();
            _selectedNodeId = null;
            RefreshAll();
        }

        private void SelectNode(string nodeId)
        {
            _selectedNodeId = nodeId;
            _selectedBindingId = null;
            _suppressSelection = true;
            _bindingList?.ClearSelection();
            _suppressSelection = false;
            RefreshAll();
        }

        private void SelectBinding(string bindingId)
        {
            _selectedBindingId = bindingId;
            _selectedNodeId = null;
            RefreshAll();
        }

        private void OnBindingSelectionChanged()
        {
            if (_suppressSelection || _recipe == null || _bindingList.selectedIndex < 0 || _bindingList.selectedIndex >= _recipe.Bindings.Count) return;
            TweenRecipeBindingDefinition binding = _recipe.Bindings[_bindingList.selectedIndex];
            if (binding != null) SelectBinding(binding.Id);
        }

        private void RestoreBindingListSelection()
        {
            if (_bindingList == null) return;
            int index = FindBindingIndex(_selectedBindingId);
            _suppressSelection = true;
            if (index >= 0) _bindingList.selectedIndex = index;
            else _bindingList.ClearSelection();
            _suppressSelection = false;
        }

        private void RestoreValidSelection()
        {
            if (_recipe == null)
            {
                _selectedNodeId = null;
                _selectedBindingId = null;
                return;
            }

            if (FindNodeIndex(_selectedNodeId) < 0) _selectedNodeId = null;
            if (FindBindingIndex(_selectedBindingId) < 0) _selectedBindingId = null;
        }

        private void ScheduleRefresh()
        {
            if (_refreshScheduled) return;
            _refreshScheduled = true;
            EditorApplication.delayCall += () =>
            {
                if (this != null) RefreshAll();
            };
        }

        private void Commit()
        {
            _serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_recipe);
        }

        private string FindFirstBindingId(TweenRecipeBindingKind kind, string excludedId = null)
        {
            if (_recipe == null) return string.Empty;
            for (int i = 0; i < _recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition binding = _recipe.Bindings[i];
                if (binding != null && binding.Kind == kind && binding.Id != excludedId) return binding.Id;
            }

            return string.Empty;
        }

        private bool BindingMatches(string id, TweenRecipeBindingKind kind)
        {
            int index = FindBindingIndex(id);
            return index >= 0 && _recipe.Bindings[index].Kind == kind;
        }

        private int FindNodeIndex(string id)
        {
            if (_recipe == null || string.IsNullOrEmpty(id)) return -1;
            for (int i = 0; i < _recipe.Nodes.Count; i++)
            {
                if (_recipe.Nodes[i] != null && _recipe.Nodes[i].Id == id) return i;
            }

            return -1;
        }

        private int FindBindingIndex(string id)
        {
            if (_recipe == null || string.IsNullOrEmpty(id)) return -1;
            for (int i = 0; i < _recipe.Bindings.Count; i++)
            {
                if (_recipe.Bindings[i] != null && _recipe.Bindings[i].Id == id) return i;
            }

            return -1;
        }

        private static void NormalizeFirstNode(SerializedProperty nodes)
        {
            if (nodes.arraySize > 0)
            {
                nodes.GetArrayElementAtIndex(0).FindPropertyRelative("placement").enumValueIndex = (int)TweenRecipePlacement.Then;
            }
        }

        private static void SetParameterDefaults(SerializedProperty parameters, TweenRecipeOperation operation)
        {
            parameters.FindPropertyRelative("vector3Value").vector3Value = TweenRecipeOperationCatalog.GetDefaultVector3(operation);
            parameters.FindPropertyRelative("colorValue").colorValue = TweenRecipeOperationCatalog.GetDefaultColor(operation);
            parameters.FindPropertyRelative("floatValue").floatValue = TweenRecipeOperationCatalog.GetDefaultFloat(operation);
            parameters.FindPropertyRelative("intValue").intValue = TweenRecipeOperationCatalog.GetDefaultInteger(operation);
            parameters.FindPropertyRelative("boolValue").boolValue = TweenRecipeOperationCatalog.GetDefaultBoolean(operation);
            parameters.FindPropertyRelative("stringValue").stringValue = TweenRecipeOperationCatalog.GetDefaultString(operation);
        }

        private static void CopyNode(TweenRecipeNode source, SerializedProperty destination)
        {
            destination.FindPropertyRelative("label").stringValue = source.Label;
            destination.FindPropertyRelative("placement").enumValueIndex = (int)source.Placement;
            destination.FindPropertyRelative("operation").enumValueIndex = (int)source.Operation;
            destination.FindPropertyRelative("bindingId").stringValue = source.BindingId;
            destination.FindPropertyRelative("secondaryBindingId").stringValue = source.SecondaryBindingId;
            destination.FindPropertyRelative("duration").floatValue = source.Duration;
            destination.FindPropertyRelative("delay").floatValue = source.Delay;
            destination.FindPropertyRelative("ease").enumValueIndex = (int)source.Ease;
            SerializedProperty parameters = destination.FindPropertyRelative("parameters");
            parameters.FindPropertyRelative("vector3Value").vector3Value = source.Parameters.Vector3Value;
            parameters.FindPropertyRelative("colorValue").colorValue = source.Parameters.ColorValue;
            parameters.FindPropertyRelative("floatValue").floatValue = source.Parameters.FloatValue;
            parameters.FindPropertyRelative("intValue").intValue = source.Parameters.IntValue;
            parameters.FindPropertyRelative("boolValue").boolValue = source.Parameters.BoolValue;
            parameters.FindPropertyRelative("stringValue").stringValue = source.Parameters.StringValue;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape && TweenRecipePreviewSession.IsPreviewing)
            {
                StopPreview();
                evt.StopPropagation();
                return;
            }

            if (_recipe == null || IsEditingField(evt.target as VisualElement)) return;
            bool actionKey = evt.ctrlKey || evt.commandKey;
            if (actionKey && evt.keyCode == KeyCode.D && !string.IsNullOrEmpty(_selectedNodeId))
            {
                DuplicateNode(FindNodeIndex(_selectedNodeId));
            }
            else if ((evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace) && !string.IsNullOrEmpty(_selectedNodeId))
            {
                DeleteNode(FindNodeIndex(_selectedNodeId));
            }
            else if ((evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace) && !string.IsNullOrEmpty(_selectedBindingId))
            {
                RemoveSelectedBinding();
            }
            else if (actionKey && evt.keyCode == KeyCode.S)
            {
                AssetDatabase.SaveAssetIfDirty(_recipe);
            }
            else if (!actionKey && evt.keyCode == KeyCode.F)
            {
                _timeline.FrameAll();
            }
            else
            {
                return;
            }

            evt.StopPropagation();
        }

        private void StartPreview()
        {
            TweenPlayer player = FindPreviewPlayer();
            TweenRecipePreviewSession.Start(player, out _);
            RefreshPreviewState();
        }

        private void StopPreview()
        {
            TweenRecipePreviewSession.Stop();
            RefreshPreviewState();
        }

        private void RefreshPreviewState()
        {
            if (_previewButton == null || _stopButton == null || _previewStatus == null) return;
            TweenPlayer player = FindPreviewPlayer();
            bool canPreview = TweenRecipePreviewSession.CanPreview(player, out string reason);
            _previewButton.SetEnabled(!TweenRecipePreviewSession.IsPreviewing && canPreview);
            _previewButton.tooltip = reason;
            _stopButton.SetEnabled(TweenRecipePreviewSession.IsPreviewing);
            _previewStatus.text = TweenRecipePreviewSession.IsPreviewing ? TweenRecipePreviewSession.StatusMessage : reason;
        }

        private TweenPlayer FindPreviewPlayer()
        {
            TweenPlayer player = Selection.activeObject as TweenPlayer;
            if (player == null && Selection.activeGameObject != null) player = Selection.activeGameObject.GetComponent<TweenPlayer>();
            return player != null && player.Recipe == _recipe ? player : null;
        }

        private static bool IsEditingField(VisualElement element)
        {
            VisualElement current = element;
            while (current != null)
            {
                if (current.ClassListContains("unity-base-field")) return true;
                current = current.parent;
            }

            return false;
        }

        private static Label CreateLabel(string text, string className)
        {
            var label = new Label(text);
            label.AddToClassList(className);
            return label;
        }

        private void OnUndoRedo()
        {
            if (_recipe != null) _serializedObject = new SerializedObject(_recipe);
            RefreshAll();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is TweenRecipe recipe) SetRecipe(recipe);
        }
    }
}
