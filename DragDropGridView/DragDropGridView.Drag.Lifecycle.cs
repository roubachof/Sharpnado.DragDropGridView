using MR.Gestures;
using Sharpnado.Tasks;
using ScrollView = Microsoft.Maui.Controls.ScrollView;

namespace Sharpnado.Maui.DragDropGridView;

public partial class DragDropGridView
{
    protected override void OnChildRemoved(Element child, int oldLogicalIndex)
    {
        InternalLogger.Debug(Tag, () => $"Remove( child: {child.GetType().Name}, index: {oldLogicalIndex} )");

        if (IsDragAndDropEnabled)
        {
            UnsubscribeDragAndDropIfNeeded(child);
        }

        _orderedChildren.Remove((View)child);

        base.OnChildRemoved(child, oldLogicalIndex);
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);

        InternalLogger.Debug(Tag, () => $"OnChildAdded( child: {child.GetType().Name} )");

        _orderedChildren.Add((View)child);

        if (IsDragAndDropEnabled)
        {
            SubscribeDragAndDropIfNeeded(child);
        }
    }

    private void InitializeDragAndDrop()
    {
        InternalLogger.Debug(Tag, () => $"InitializeDragAndDrop()");

        Settings.MinimumDeltaDistance = 2;

        _scrollView = this.GetFirstParentOfType<ScrollView>();
        _refreshView = this.GetFirstParentOfType<RefreshView>();
        InternalLogger.Debug(Tag, () => _scrollView != null ? $"Parent ScrollView found" : "Warning: No ScrollView found!");
        UpdateIsDragAndDropEnabled(IsDragAndDropEnabled);
    }

    private void ClearDragAndDrop()
    {
        InternalLogger.Debug(Tag, () => "ClearDragAndDrop()");

        foreach (var child in Children)
        {
            UnsubscribeDragAndDropIfNeeded((Element)child);
        }

        _scrollView = null;
        _draggingSessionList.Clear();
        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = null;
    }

    private void UpdateIsDragAndDropEnabled(bool isEnabled)
    {
        InternalLogger.Debug(Tag, () => $"UpdateIsDragAndDropEnabled( isEnabled: {isEnabled} )");

        foreach (var child in Children)
        {
            if (isEnabled)
            {
                SubscribeDragAndDropIfNeeded((Element)child);
            }
            else
            {
                UnsubscribeDragAndDropIfNeeded((Element)child);
            }
        }

        if (isEnabled)
        {
            if (DragAndDropItemsAnimation != null)
            {
                TaskMonitor.Create(ApplyAnimationToChildrenAsync());
            }
        }
        else
        {
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = null;
        }

        if (DragAndDropTrigger == DragAndDropTrigger.Pan)
        {
            UpdateDisableScrollView(isEnabled);
        }
    }
}
