using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    internal sealed class TweenRecipeInspectorView : VisualElement
    {
        private readonly Action _changed;
        private readonly Action<int, TweenRecipeOperation> _setOperation;
        private readonly Action<int, string> _setNodeBinding;
        private readonly Action<int, string> _setNodeSecondaryBinding;
        private readonly Action<int> _duplicateNode;
        private readonly Action<int> _deleteNode;
        private SerializedObject _serializedObject;

        public TweenRecipeInspectorView(Action changed, Action<int, TweenRecipeOperation> setOperation, Action<int, string> setNodeBinding,
            Action<int, string> setNodeSecondaryBinding, Action<int> duplicateNode, Action<int> deleteNode)
        {
            _changed = changed;
            _setOperation = setOperation;
            _setNodeBinding = setNodeBinding;
            _setNodeSecondaryBinding = setNodeSecondaryBinding;
            _duplicateNode = duplicateNode;
            _deleteNode = deleteNode;
            AddToClassList("recipe-inspector");
            RegisterCallback<SerializedPropertyChangeEvent>(_ =>
            {
                if (_serializedObject == null) return;
                _serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_serializedObject.targetObject);
                _changed();
            });
        }

        public void Show(TweenRecipe recipe, SerializedObject serializedObject, string nodeId, string bindingId)
        {
            this.Unbind();
            Clear();
            _serializedObject = serializedObject;

            if (recipe == null || serializedObject == null)
            {
                ShowEmpty("Select or create a TweenRecipe.");
                return;
            }

            int nodeIndex = FindNodeIndex(recipe, nodeId);
            if (nodeIndex >= 0)
            {
                ShowNode(recipe, serializedObject, nodeIndex);
                return;
            }

            int bindingIndex = FindBindingIndex(recipe, bindingId);
            if (bindingIndex >= 0)
            {
                ShowBinding(serializedObject, bindingIndex);
                return;
            }

            ShowEmpty("Select a binding or node to edit its properties.");
        }

        private void ShowNode(TweenRecipe recipe, SerializedObject serializedObject, int nodeIndex)
        {
            TweenRecipeNode node = recipe.Nodes[nodeIndex];
            SerializedProperty nodeProperty = serializedObject.FindProperty("nodes").GetArrayElementAtIndex(nodeIndex);
            Add(CreateLabel("Node Inspector", "recipe-section-title"));
            Add(CreateLabel($"ID  {ShortId(node.Id)}", "recipe-muted"));
            Add(new PropertyField(nodeProperty.FindPropertyRelative("label"), "Label"));

            var placement = new EnumField("Placement", node.Placement);
            placement.BindProperty(nodeProperty.FindPropertyRelative("placement"));
            Add(placement);

            var operation = new EnumField("Operation", node.Operation);
            operation.RegisterValueChangedCallback(evt => _setOperation(nodeIndex, (TweenRecipeOperation)evt.newValue));
            Add(operation);

            if (TweenRecipeOperationCatalog.UsesBinding(node.Operation))
            {
                Add(CreateBindingPopup(recipe, node, nodeIndex, false));
            }

            if (TweenRecipeOperationCatalog.UsesSecondaryBinding(node.Operation))
            {
                Add(CreateBindingPopup(recipe, node, nodeIndex, true));
            }

            Add(new PropertyField(nodeProperty.FindPropertyRelative("duration"), node.Operation == TweenRecipeOperation.Delay ? "Delay Duration" : "Duration"));
            if (node.Operation != TweenRecipeOperation.Delay)
            {
                Add(new PropertyField(nodeProperty.FindPropertyRelative("delay"), "Start Delay"));
                Add(new PropertyField(nodeProperty.FindPropertyRelative("ease"), "Ease"));
            }

            SerializedProperty parameters = nodeProperty.FindPropertyRelative("parameters");
            TweenRecipeParameterFields fields = TweenRecipeOperationCatalog.GetVisibleFields(node.Operation);
            AddParameterField(parameters, node.Operation, fields, TweenRecipeParameterFields.Vector3, "vector3Value");
            AddParameterField(parameters, node.Operation, fields, TweenRecipeParameterFields.Color, "colorValue");
            AddParameterField(parameters, node.Operation, fields, TweenRecipeParameterFields.Float, "floatValue");
            AddParameterField(parameters, node.Operation, fields, TweenRecipeParameterFields.Integer, "intValue");
            AddParameterField(parameters, node.Operation, fields, TweenRecipeParameterFields.Boolean, "boolValue");
            AddParameterField(parameters, node.Operation, fields, TweenRecipeParameterFields.String, "stringValue");

            var actions = new VisualElement();
            actions.AddToClassList("recipe-inspector-actions");
            actions.Add(new Button(() => _duplicateNode(nodeIndex)) { text = "Duplicate" });
            actions.Add(new Button(() => _deleteNode(nodeIndex)) { text = "Delete" });
            Add(actions);
            this.Bind(serializedObject);
        }

        private void ShowBinding(SerializedObject serializedObject, int bindingIndex)
        {
            SerializedProperty binding = serializedObject.FindProperty("bindings").GetArrayElementAtIndex(bindingIndex);
            Add(CreateLabel("Binding Inspector", "recipe-section-title"));
            Add(CreateLabel($"ID  {ShortId(binding.FindPropertyRelative("id").stringValue)}", "recipe-muted"));
            Add(new PropertyField(binding.FindPropertyRelative("displayName"), "Name"));
            Add(new PropertyField(binding.FindPropertyRelative("kind"), "Kind"));
            this.Bind(serializedObject);
        }

        private VisualElement CreateBindingPopup(TweenRecipe recipe, TweenRecipeNode node, int nodeIndex, bool secondary)
        {
            var labels = new List<string>();
            var ids = new List<string>();
            int selectedIndex = -1;
            string currentId = secondary ? node.SecondaryBindingId : node.BindingId;
            TweenRecipeBindingKind requiredKind = secondary
                ? TweenRecipeOperationCatalog.GetSecondaryBindingKind(node.Operation)
                : TweenRecipeOperationCatalog.GetBindingKind(node.Operation);
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition binding = recipe.Bindings[i];
                if (binding == null || binding.Kind != requiredKind) continue;
                ids.Add(binding.Id);
                labels.Add(string.IsNullOrWhiteSpace(binding.DisplayName) ? "(Unnamed)" : binding.DisplayName);
                if (binding.Id == currentId) selectedIndex = ids.Count - 1;
            }

            if (labels.Count == 0)
            {
                var missing = new Label("Add a compatible binding before assigning this node.");
                missing.AddToClassList("recipe-warning");
                return missing;
            }

            if (selectedIndex < 0)
            {
                ids.Insert(0, currentId);
                labels.Insert(0, string.IsNullOrEmpty(currentId) ? "(Select binding)" : "(Missing binding)");
                selectedIndex = 0;
            }

            var popup = new PopupField<string>(secondary ? "Destination Binding" : "Binding", labels, selectedIndex);
            popup.RegisterValueChangedCallback(evt =>
            {
                int index = labels.IndexOf(evt.newValue);
                if (index < 0) return;
                if (secondary) _setNodeSecondaryBinding(nodeIndex, ids[index]);
                else _setNodeBinding(nodeIndex, ids[index]);
            });
            return popup;
        }

        private void AddParameterField(SerializedProperty parameters, TweenRecipeOperation operation, TweenRecipeParameterFields visible, TweenRecipeParameterFields field, string propertyName)
        {
            if ((visible & field) == 0) return;
            Add(new PropertyField(parameters.FindPropertyRelative(propertyName), TweenRecipeOperationCatalog.GetParameterLabel(operation, field)));
        }

        private void ShowEmpty(string text)
        {
            var label = new Label(text);
            label.AddToClassList("recipe-empty-state");
            Add(label);
        }

        private static int FindNodeIndex(TweenRecipe recipe, string id)
        {
            if (string.IsNullOrEmpty(id)) return -1;
            for (int i = 0; i < recipe.Nodes.Count; i++)
            {
                if (recipe.Nodes[i] != null && recipe.Nodes[i].Id == id) return i;
            }

            return -1;
        }

        private static int FindBindingIndex(TweenRecipe recipe, string id)
        {
            if (string.IsNullOrEmpty(id)) return -1;
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                if (recipe.Bindings[i] != null && recipe.Bindings[i].Id == id) return i;
            }

            return -1;
        }

        private static Label CreateLabel(string text, string className)
        {
            var label = new Label(text);
            label.AddToClassList(className);
            return label;
        }

        private static string ShortId(string id) => string.IsNullOrEmpty(id) ? "Missing" : id.Substring(0, Math.Min(8, id.Length));
    }
}
