namespace Sharpnado.GridLayout;

using System.Collections;
using System.Collections.ObjectModel;
using Tasks;

public partial class DragDropGridView
{
    private const bool VerboseLogging = false;

    private bool _isReorderingItemsSource;

    private void CheckCandidates(View draggingView)
    {
        var dragAndDropMoving = (IDragAndDropView)draggingView;

        // We are comparing here the current translated coordinates
        var viewCenter = draggingView.GetTranslatingCenter();
        InternalLogger.DebugIf(VerboseLogging, Tag, () => $"CheckCandidates( draggingView: {draggingView.GetType().Name},  CenterX:{viewCenter.X}, CenterY:{viewCenter.Y}");

        var viewIndex = _draggingSessionList.IndexOf(draggingView);
        double replaceMinXDistance = 0;
        double replaceMaxXDistance = 80;
        double replaceMaxYDistance = 80;

        // 5 pixels radius
        const double groupMaxSquareDistance = 50;

        var candidateIndex = -1;
        foreach (var candidate in _draggingSessionList)
        {
            candidateIndex++;
            if (!(candidate != draggingView && candidate is IDragAndDropView { CanMove: true } dragEndDropCandidate))
            {
                continue;
            }

            // We are comparing here the current translated coordinates
            var candidateCenter = candidate.GetTranslatingCenter();

            InternalLogger.DebugIf(VerboseLogging, Tag, () => $"selected candidate index {candidateIndex}");

            if (dragEndDropCandidate.CanReceiveView && dragAndDropMoving.CanBeDropped)
            {
                var squareDistance = Computation.SquareDistance(
                    viewCenter.X,
                    viewCenter.Y,
                    candidateCenter.X,
                    candidateCenter.Y);

                if (squareDistance <= groupMaxSquareDistance)
                {
                    if (_groupCandidate == null)
                    {
                        _groupCandidate = candidate;
                        TaskMonitor.Create(_groupCandidate.ScaleTo(1.2));
                    }

                    if (_groupCandidate != candidate)
                    {
                        TaskMonitor.Create(_groupCandidate.ScaleTo(1));
                        _groupCandidate = candidate;
                        TaskMonitor.Create(_groupCandidate.ScaleTo(1.2));
                    }

                    break;
                }
            }

            if (_groupCandidate == candidate)
            {
                // view is no more a candidate
                TaskMonitor.Create(_groupCandidate.ScaleTo(1));
                _groupCandidate = null;
            }

            InternalLogger.DebugIf(VerboseLogging, Tag, () => $"candidateX: {candidateCenter.X}, candidateY: {candidateCenter.Y}");
            var xDistanceSigned = viewCenter.X - candidateCenter.X;
            var yDistanceSigned = viewCenter.Y - candidateCenter.Y;
            var xDistance = Math.Abs(xDistanceSigned);
            var yDistance = Math.Abs(viewCenter.Y - candidateCenter.Y);

            InternalLogger.DebugIf(VerboseLogging, Tag, () => $"replaceXDistance: [{replaceMinXDistance}, {replaceMaxXDistance}], replaceMaxXDistance: {replaceMaxYDistance}");
            InternalLogger.DebugIf(VerboseLogging, Tag, () => $"Computed xDistance: {xDistance}, yDistance: {yDistance} ");
            if (xDistance > replaceMinXDistance
                && xDistance < replaceMaxXDistance
                && yDistance < replaceMaxYDistance)
            {
                InternalLogger.DebugIf(VerboseLogging, Tag, () => "MADE IT THROUGH TESTS!!!");
                InternalLogger.DebugIf(VerboseLogging, Tag, () => $"xDistanceSigned: {xDistanceSigned}, viewIndex: {viewIndex}, candidateIndex: {candidateIndex}");

                var isAfter = ColumnCount > 1 ? xDistanceSigned > 0 : yDistanceSigned > 1;
                var mustShiftLeft = candidateIndex > viewIndex && isAfter;

                InternalLogger.DebugIf(VerboseLogging, Tag, () => $"isAfter: {isAfter}, mustShiftLeft: {mustShiftLeft}");
                if (mustShiftLeft && (_currentShiftTask.IsNotStarted || _currentShiftTask.IsCompleted))
                {
                    _currentShiftTask = TaskMonitor.Create(ShiftAsync(Direction.Left, candidateIndex, viewIndex, draggingView));
                    break;
                }

                var isBefore = ColumnCount > 1 ? xDistanceSigned < 0 : yDistanceSigned < 0;
                var mustShiftRight = candidateIndex < viewIndex && isBefore;

                InternalLogger.DebugIf(VerboseLogging, Tag, () => $"isBefore: {isBefore}, mustShiftRight: {mustShiftRight}");
                if (mustShiftRight && (_currentShiftTask.IsNotStarted || _currentShiftTask.IsCompleted))
                {
                    _currentShiftTask = TaskMonitor.Create(ShiftAsync(Direction.Right, candidateIndex, viewIndex, draggingView));
                    break;
                }
            }
        }
    }

    private enum Direction
    {
        Left,
        Right,
    }

    /// <summary>
    /// Shift to the right or the left items after a successful drag between to new items.
    /// </summary>
    /// <param name="direction">Left or Right.</param>
    /// <param name="targetIndex">
    /// The index where the shift will begin.
    /// Also the index where the dragging view will be finally located.
    /// If draggingView is null, the bucket at targetIndex will be removed
    /// (In a Grouping operation the dragging view is merged into the receiving view.)
    /// </param>
    /// <param name="holeIndex">The index towards the views will be shift finally erasing the hole.</param>
    /// <param name="draggingView">The view currently being dragged around.</param>
    /// <returns></returns>
    private Task ShiftAsync(Direction direction, int targetIndex, int holeIndex, View? draggingView = null)
    {
        InternalLogger.DebugIf(VerboseLogging, Tag, () => draggingView == null
            ? $"Shift {direction} => remove view at {targetIndex}, and shift the views from {targetIndex} to {holeIndex}"
            : $"Shift {direction} => inserting {draggingView} to {targetIndex}, and shift to {holeIndex}");

        // Collect shifting views and their target slot positions
        var shifts = new List<(View shifting, View target)>();

        if (direction == Direction.Left)
        {
            for (var index = holeIndex; index < targetIndex; index++)
            {
                shifts.Add(((View)_draggingSessionList[index + 1], (View)_orderedChildren[index]));
                _draggingSessionList[index] = _draggingSessionList[index + 1];
            }
        }
        else
        {
            for (var index = holeIndex; index > targetIndex; index--)
            {
                shifts.Add(((View)_draggingSessionList[index - 1], (View)_orderedChildren[index]));
                _draggingSessionList[index] = _draggingSessionList[index - 1];
            }
        }

        if (draggingView == null)
        {
            // Grouping: also remove the bucket at targetIndex
            _draggingSessionList.RemoveAt(targetIndex);
        }
        else
        {
            // Place dragging view in target slot
            _draggingSessionList[targetIndex] = draggingView;
        }

        // Build a single batched animation instead of per-view TranslateTo calls
        var tcs = new TaskCompletionSource<bool>();
        try
        {
            var parent = new Microsoft.Maui.Controls.Animation();
            var duration = ShiftAnimationDuration; // short, consistent duration helps Android
            var easing = Microsoft.Maui.Easing.Linear;

            foreach (var (shifting, target) in shifts)
            {
                // Cancel any ongoing animations on this view
                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(shifting);

                var startX = shifting.TranslationX;
                var startY = shifting.TranslationY;

                var endX = target.X - shifting.X;
                var endY = target.Y - shifting.Y;

                // Single child animation controlling both axes
                var child = new Microsoft.Maui.Controls.Animation(progress =>
                {
                    // Lerp translations
                    shifting.TranslationX = startX + (endX - startX) * progress;
                    shifting.TranslationY = startY + (endY - startY) * progress;
                });

                parent.Add(0, 1, child);
            }

            // Commit once
            parent.Commit(this, "ShiftBatch", 16, duration, easing, (v, c) =>
            {
                // Ensure final values are set exactly
                foreach (var (shifting, target) in shifts)
                {
                    shifting.TranslationX = target.X - shifting.X;
                    shifting.TranslationY = target.Y - shifting.Y;
                }
                tcs.TrySetResult(true);
            });
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
        }

        var listResult = _draggingSessionList.Aggregate(string.Empty, (i, v) => $"{i}, {v}");
        InternalLogger.DebugIf(VerboseLogging, Tag, () => $"list: {listResult}");

        return tcs.Task;
    }

    private IView? StopDraggingSession(View view)
    {
        InternalLogger.Debug(Tag, () => "StopDraggingSession");

        _isDragging = false;

        ((IDragAndDropView)view).IsDragAndDropping = false;
#if !ANDROID
        view.ZIndex -= 100;
#endif

        SyncItemsSource(view);

        _draggingView = null;
        _orderedChildren.Clear();
        _orderedChildren.AddRange(_draggingSessionList);

        _draggingSessionList.Clear();

        // Find views that should be removed (excluding the header view)
        return Children
            .Except(_orderedChildren)
            .FirstOrDefault(child => child != _headerView);
    }

    private void SyncItemsSource(View view)
    {
        var newIndexOfView = _draggingSessionList.IndexOf(view);

        var itemsSource = ItemsSource;
        var bindingContext = view.BindingContext;
        if (bindingContext != null)
        {
            if (itemsSource is IList list)
            {
                try
                {
                    var oldIndex = list.IndexOf(bindingContext);

                    InternalLogger.Debug(Tag, () => $"ItemsSource item moved from {oldIndex} to {newIndexOfView}");

                    var observableCollectionType = typeof(ObservableCollection<>);
                    var observableOfType = observableCollectionType.MakeGenericType(bindingContext.GetType());

                    var itemsSourceType = itemsSource.GetType();
                    if (itemsSourceType.IsAssignableTo(observableOfType))
                    {
                        _isReorderingItemsSource = true;

                        // call the move method on the observable collection
                        var moveMethod = itemsSourceType.GetMethod("Move");
                        moveMethod?.Invoke(itemsSource, [oldIndex, newIndexOfView]);
                    }
                }
                finally
                {
                    _isReorderingItemsSource = false;
                }
            }
        }
    }

    /// <summary>
    /// The method that will handle the end of the dragging.
    /// It will either:
    /// * Group the dragged view with a receiving view,
    /// then shift the views to the left to fill the hole left by the dragged view,
    /// * Animate the dragged view to its placeholder.
    /// Then invalidate the layout.
    /// </summary>
    /// <param name="view">The view that has been dropped.</param>
    private async Task OnViewDroppedAsync(View view)
    {
        InternalLogger.Debug(Tag, () => $"OnViewDroppedAsync( view: {view.GetType().Name} )");

        // Prevent invalidation during animation of children and binding context changes
        _shouldInvalidate = false;

        if (_groupCandidate != null && _draggingView != null)
        {
            InternalLogger.Debug(Tag, () => $"Grouping started with {_groupCandidate}");

            // Group the 2 views
            await _groupCandidate.ScaleTo(1);

            await ((IDragAndDropView)_groupCandidate).OnViewDroppedAsync((IDragAndDropView)_draggingView);

            var draggingViewIndex = _draggingSessionList.IndexOf(_draggingView);

            await ShiftAsync(Direction.Left, _draggingSessionList.Count - 1, draggingViewIndex);

            _groupCandidate = null;
        }
        else if (_draggingView != null)
        {
            // Animate to final position
            var viewIndex = _draggingSessionList.IndexOf(_draggingView);
            var targetView = (View)_orderedChildren[viewIndex];

            var translationX = targetView.X - _draggingView.X;
            var translationY = targetView.Y - _draggingView.Y;

            InternalLogger.Debug(Tag, () => $"Animating view to final translation: x:{translationX}, y:{translationY}");

            await _draggingView.TranslateTo(translationX, translationY);
        }

        var toBeRemoved = StopDraggingSession(view);

        _shouldInvalidate = true;
        if (toBeRemoved != null)
        {
            Children.Remove(toBeRemoved);
        }
        else
        {
            InvalidateMeasure();
        }

        InternalLogger.Debug(Tag, () => $"Ordered children after invalidation: {_orderedChildren.Aggregate(string.Empty, (acc, view) => $"{acc}, {((View)view).BindingContext}")}");

        OnItemsReorderedCommand?.Execute(
            _orderedChildren.Select(v => ((View)v).BindingContext)
                .ToList());
    }

}
