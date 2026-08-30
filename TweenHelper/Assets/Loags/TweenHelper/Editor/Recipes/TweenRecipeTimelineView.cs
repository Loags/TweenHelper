using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LB.TweenHelper.Editor
{
    internal sealed class TweenRecipeTimelineView : VisualElement
    {
        private const float DragThreshold = 5f;

        private readonly Action<string> _selectNode;
        private readonly Action<int, int, TweenRecipePlacement> _moveNode;
        private readonly ScrollView _scrollView;
        private readonly VisualElement _content;
        private readonly List<VisualElement> _nodeCards = new List<VisualElement>();
        private readonly List<TweenRecipePlacement> _nodePlacements = new List<TweenRecipePlacement>();
        private readonly List<TweenRecipeOperation> _nodeOperations = new List<TweenRecipeOperation>();

        private VisualElement _dragCard;
        private int _dragSourceIndex = -1;
        private int _dragPointerId = -1;
        private Vector2 _dragStart;
        private bool _isDragging;

        public TweenRecipeTimelineView(Action<string> selectNode, Action<int, int, TweenRecipePlacement> moveNode)
        {
            _selectNode = selectNode;
            _moveNode = moveNode;
            AddToClassList("recipe-timeline");

            _scrollView = new ScrollView(ScrollViewMode.Horizontal);
            _scrollView.AddToClassList("recipe-timeline-scroll");
            _content = new VisualElement();
            _content.AddToClassList("recipe-timeline-content");
            _scrollView.Add(_content);
            Add(_scrollView);
        }

        public void Refresh(TweenRecipe recipe, string selectedNodeId, TweenRecipeValidationResult validation)
        {
            CancelDrag();
            _nodeCards.Clear();
            _nodePlacements.Clear();
            _nodeOperations.Clear();
            _content.Clear();
            _content.Add(CreateStartCard());

            if (recipe == null || recipe.Nodes.Count == 0)
            {
                var empty = new Label("Add a node from the palette to start the timeline.");
                empty.AddToClassList("recipe-empty-state");
                _content.Add(empty);
                return;
            }

            var invalidNodeIds = new HashSet<string>(StringComparer.Ordinal);
            if (validation != null)
            {
                for (int i = 0; i < validation.Messages.Count; i++)
                {
                    TweenRecipeValidationMessage message = validation.Messages[i];
                    if (message.Severity == TweenRecipeValidationSeverity.Error && !string.IsNullOrEmpty(message.NodeId))
                    {
                        invalidNodeIds.Add(message.NodeId);
                    }
                }
            }

            var bindingNames = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < recipe.Bindings.Count; i++)
            {
                TweenRecipeBindingDefinition binding = recipe.Bindings[i];
                if (binding != null && !string.IsNullOrEmpty(binding.Id) && !bindingNames.ContainsKey(binding.Id))
                {
                    bindingNames.Add(binding.Id, binding.DisplayName);
                }
            }

            VisualElement currentGroup = null;
            for (int i = 0; i < recipe.Nodes.Count; i++)
            {
                TweenRecipeNode node = recipe.Nodes[i];
                if (node == null) continue;

                if (i == 0 || node.Placement == TweenRecipePlacement.Then || currentGroup == null)
                {
                    _content.Add(CreateConnector());
                    currentGroup = new VisualElement();
                    currentGroup.AddToClassList("recipe-node-group");
                    _content.Add(currentGroup);
                }

                string bindingName = string.Empty;
                if (!string.IsNullOrEmpty(node.BindingId) && bindingNames.TryGetValue(node.BindingId, out string resolvedName))
                {
                    bindingName = resolvedName;
                }
                else if (TweenRecipeOperationCatalog.UsesBinding(node.Operation))
                {
                    bindingName = "Missing binding";
                }

                VisualElement card = CreateNodeCard(node, i, recipe.Nodes.Count, bindingName, node.Id == selectedNodeId, invalidNodeIds.Contains(node.Id));
                _nodeCards.Add(card);
                _nodePlacements.Add(node.Placement);
                _nodeOperations.Add(node.Operation);
                currentGroup.Add(card);
            }
        }

        public void FrameAll() => _scrollView.scrollOffset = Vector2.zero;

        private VisualElement CreateStartCard()
        {
            var card = new VisualElement();
            card.AddToClassList("recipe-start-card");
            card.Add(new Label("START"));
            return card;
        }

        private VisualElement CreateConnector()
        {
            var connector = new Label("›");
            connector.AddToClassList("recipe-connector");
            return connector;
        }

        private VisualElement CreateNodeCard(TweenRecipeNode node, int index, int count, string bindingName, bool selected, bool invalid)
        {
            var card = new VisualElement();
            card.AddToClassList("recipe-node-card");
            if (node.Placement == TweenRecipePlacement.With) card.AddToClassList("recipe-node-with");
            if (selected) card.AddToClassList("recipe-node-selected");
            if (invalid) card.AddToClassList("recipe-node-invalid");
            card.tooltip = "Click to select. Drag horizontally to reorder; drop on a parallel lane to use With.";
            card.RegisterCallback<ClickEvent>(_ => _selectNode(node.Id));
            card.RegisterCallback<PointerDownEvent>(evt => BeginDrag(evt, card, index));
            card.RegisterCallback<PointerMoveEvent>(UpdateDrag);
            card.RegisterCallback<PointerUpEvent>(EndDrag);
            card.RegisterCallback<PointerCaptureOutEvent>(_ => CancelDrag());

            var heading = new VisualElement();
            heading.AddToClassList("recipe-node-heading");
            var placement = new Label(node.Placement == TweenRecipePlacement.With ? "WITH" : "THEN");
            placement.AddToClassList("recipe-node-placement");
            heading.Add(placement);
            if (invalid)
            {
                var badge = new Label("!");
                badge.AddToClassList("recipe-error-badge");
                heading.Add(badge);
            }
            card.Add(heading);

            string label = string.IsNullOrWhiteSpace(node.Label) ? TweenRecipeOperationCatalog.GetDisplayName(node.Operation) : node.Label;
            card.Add(CreateLabel(label, "recipe-node-title"));
            card.Add(CreateLabel(TweenRecipeOperationCatalog.GetDisplayName(node.Operation), "recipe-muted"));
            if (!string.IsNullOrEmpty(bindingName)) card.Add(CreateLabel(bindingName, "recipe-node-binding"));
            card.Add(CreateLabel($"{node.Duration:0.###}s · {node.Ease}", "recipe-muted"));

            var controls = new VisualElement();
            controls.AddToClassList("recipe-node-order-controls");
            var previous = new Button(() => _moveNode(index, index - 1, node.Placement)) { text = "←", tooltip = "Move earlier" };
            previous.SetEnabled(index > 0);
            controls.Add(previous);
            var next = new Button(() => _moveNode(index, index + 1, node.Placement)) { text = "→", tooltip = "Move later" };
            next.SetEnabled(index < count - 1);
            controls.Add(next);
            card.Add(controls);
            return card;
        }

        private void BeginDrag(PointerDownEvent evt, VisualElement card, int index)
        {
            if (evt.button != 0 || IsButtonTarget(evt.target as VisualElement, card)) return;
            _dragCard = card;
            _dragSourceIndex = index;
            _dragPointerId = evt.pointerId;
            _dragStart = evt.position;
            _isDragging = false;
            card.CapturePointer(evt.pointerId);
        }

        private void UpdateDrag(PointerMoveEvent evt)
        {
            if (_dragCard == null || evt.pointerId != _dragPointerId) return;
            if (!_isDragging && Vector2.Distance(_dragStart, evt.position) >= DragThreshold)
            {
                _isDragging = true;
                _dragCard.AddToClassList("recipe-node-dragging");
            }

            if (_isDragging) evt.StopPropagation();
        }

        private void EndDrag(PointerUpEvent evt)
        {
            if (_dragCard == null || evt.pointerId != _dragPointerId) return;
            if (_dragCard.HasPointerCapture(evt.pointerId)) _dragCard.ReleasePointer(evt.pointerId);

            if (_isDragging)
            {
                int destinationIndex = FindNearestCard(evt.position);
                TweenRecipePlacement placement = ResolvePlacement(destinationIndex, evt.position);
                _moveNode(_dragSourceIndex, destinationIndex, placement);
                evt.StopPropagation();
            }

            CancelDrag();
        }

        private int FindNearestCard(Vector2 position)
        {
            int nearestIndex = Mathf.Clamp(_dragSourceIndex, 0, _nodeCards.Count - 1);
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < _nodeCards.Count; i++)
            {
                Vector2 center = _nodeCards[i].worldBound.center;
                float distance = (center - position).sqrMagnitude;
                if (distance >= nearestDistance) continue;
                nearestDistance = distance;
                nearestIndex = i;
            }

            return nearestIndex;
        }

        private TweenRecipePlacement ResolvePlacement(int destinationIndex, Vector2 position)
        {
            if (destinationIndex <= 0 || _nodeOperations[_dragSourceIndex] == TweenRecipeOperation.Delay) return TweenRecipePlacement.Then;
            VisualElement destination = _nodeCards[destinationIndex];
            bool parallelLane = _nodePlacements[destinationIndex] == TweenRecipePlacement.With
                || position.y > destination.worldBound.center.y + destination.worldBound.height * 0.15f;
            return parallelLane ? TweenRecipePlacement.With : TweenRecipePlacement.Then;
        }

        private void CancelDrag()
        {
            if (_dragCard != null)
            {
                _dragCard.RemoveFromClassList("recipe-node-dragging");
                if (_dragPointerId >= 0 && _dragCard.HasPointerCapture(_dragPointerId)) _dragCard.ReleasePointer(_dragPointerId);
            }

            _dragCard = null;
            _dragSourceIndex = -1;
            _dragPointerId = -1;
            _isDragging = false;
        }

        private static bool IsButtonTarget(VisualElement target, VisualElement card)
        {
            VisualElement current = target;
            while (current != null && current != card)
            {
                if (current is Button) return true;
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
    }
}
