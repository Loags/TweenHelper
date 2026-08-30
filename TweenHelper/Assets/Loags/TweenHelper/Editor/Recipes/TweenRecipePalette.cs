using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    internal sealed class TweenRecipePalette : VisualElement
    {
        private readonly Action<TweenRecipeOperation> _addOperation;
        private readonly List<Entry> _allEntries = new List<Entry>();
        private readonly List<Entry> _filteredEntries = new List<Entry>();
        private readonly ListView _listView;
        private readonly Button _addButton;
        private string _query = string.Empty;

        public TweenRecipePalette(Action<TweenRecipeOperation> addOperation)
        {
            _addOperation = addOperation;
            AddToClassList("recipe-palette");

            var title = new Label("Add Node");
            title.AddToClassList("recipe-section-title");
            Add(title);

            var search = new ToolbarSearchField();
            search.AddToClassList("recipe-search");
            search.RegisterValueChangedCallback(evt =>
            {
                _query = evt.newValue ?? string.Empty;
                RefreshFilter();
            });
            Add(search);

            IReadOnlyList<TweenRecipeOperation> operations = TweenRecipeOperationCatalog.Operations;
            for (int i = 0; i < operations.Count; i++)
            {
                TweenRecipeOperation operation = operations[i];
                _allEntries.Add(new Entry(operation));
            }

            _listView = new ListView(_filteredEntries, 42, MakeItem, BindItem)
            {
                selectionType = SelectionType.Single
            };
            _listView.AddToClassList("recipe-palette-list");
            _listView.selectionChanged += _ => RefreshAddButton();
            _listView.itemsChosen += items =>
            {
                foreach (object item in items)
                {
                    if (item is Entry entry) _addOperation(entry.Operation);
                    break;
                }
            };
            Add(_listView);

            _addButton = new Button(AddSelected) { text = "Add Selected Node" };
            _addButton.AddToClassList("recipe-primary-button");
            Add(_addButton);

            RefreshFilter();
        }

        private VisualElement MakeItem()
        {
            var row = new VisualElement();
            row.AddToClassList("recipe-palette-row");
            var name = new Label { name = "name" };
            name.AddToClassList("recipe-palette-name");
            row.Add(name);
            var category = new Label { name = "category" };
            category.AddToClassList("recipe-muted");
            row.Add(category);
            return row;
        }

        private void BindItem(VisualElement element, int index)
        {
            Entry entry = _filteredEntries[index];
            element.Q<Label>("name").text = entry.Name;
            element.Q<Label>("category").text = entry.Category;
            element.tooltip = entry.Description;
        }

        private void RefreshFilter()
        {
            _filteredEntries.Clear();
            for (int i = 0; i < _allEntries.Count; i++)
            {
                Entry entry = _allEntries[i];
                if (entry.Matches(_query)) _filteredEntries.Add(entry);
            }

            _listView?.Rebuild();
            if (_filteredEntries.Count > 0 && _listView.selectedIndex < 0) _listView.selectedIndex = 0;
            RefreshAddButton();
        }

        private void RefreshAddButton()
        {
            if (_addButton != null) _addButton.SetEnabled(_listView != null && _listView.selectedIndex >= 0);
        }

        private void AddSelected()
        {
            int index = _listView.selectedIndex;
            if (index >= 0 && index < _filteredEntries.Count) _addOperation(_filteredEntries[index].Operation);
        }

        private sealed class Entry
        {
            public TweenRecipeOperation Operation { get; }
            public string Name { get; }
            public string Category { get; }
            public string Description { get; }

            public Entry(TweenRecipeOperation operation)
            {
                Operation = operation;
                Name = TweenRecipeOperationCatalog.GetDisplayName(operation);
                Category = TweenRecipeOperationCatalog.GetCategory(operation);
                Description = TweenRecipeOperationCatalog.GetDescription(operation);
            }

            public bool Matches(string query)
            {
                if (string.IsNullOrWhiteSpace(query)) return true;
                return Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    || Category.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    || Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
    }
}
