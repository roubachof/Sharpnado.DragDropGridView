using System.Diagnostics;
using MR.Gestures;
using Sharpnado.Tasks;

namespace Sharpnado.Maui.DragDropGridView;

public partial class DragDropGridView
{
    private void SubscribeDragAndDropIfNeeded(Element child)
    {
        if (child is not (IGestureAwareControl gestureAwareControl and IDragAndDropView))
        {
            return;
        }

        InternalLogger.Debug(Tag, () => "SubscribeDragAndDropIfNeeded");

        if (DragAndDropTrigger == DragAndDropTrigger.LongPress)
        {
#if IOS || MACCATALYST
            // Always keep panning subscribed so iOS doesn't rebuild gesture recognizers mid-gesture
            SubscribeToPanning(gestureAwareControl);
#endif
            gestureAwareControl.LongPressed -= OnLongPressed;
            gestureAwareControl.LongPressing -= OnLongPressing;

            gestureAwareControl.LongPressed += OnLongPressed;
            gestureAwareControl.LongPressing += OnLongPressing;
        }
        else
        {
            SubscribeToPanning(gestureAwareControl);
        }
    }

    private void UnsubscribeDragAndDropIfNeeded(Element child)
    {
        if (child is not (IGestureAwareControl gestureAwareControl and IDragAndDropView))
        {
            return;
        }

        InternalLogger.Debug(Tag, () => "UnsubscribeDragAndDropIfNeeded");

        if (DragAndDropTrigger == DragAndDropTrigger.LongPress)
        {
            gestureAwareControl.LongPressed -= OnLongPressed;
            gestureAwareControl.LongPressing -= OnLongPressing;
        }

        UnsubscribeToPanning(gestureAwareControl);
    }

    private void OnLongPressing(object? sender, LongPressEventArgs e)
    {
        InternalLogger.Debug(Tag, () => "OnLongPressing()");

        var gestureAwareControl = (IGestureAwareControl)sender!;
        var view = (View)gestureAwareControl;

#if IOS || MACCATALYST
        _isLongPressActive = true;
#endif
        // Apply start dragging animation
        if (LongPressedDraggingAnimation != null)
        {
            TaskMonitor.Create(LongPressedDraggingAnimation(view));
        }
        else
        {
            TaskMonitor.Create(DragDropAnimations.Dragging.ScaleUpAsync(view));
        }

        ((IDragAndDropView)sender!).IsDragAndDropping = true;

        UpdateDisableScrollView(true);

#if !(IOS || MACCATALYST)
        // On Android/Windows we subscribe panning when long-press starts
        SubscribeToPanning(gestureAwareControl);
#endif
    }

    private void OnLongPressed(object? sender, LongPressEventArgs e)
    {
        InternalLogger.Debug(Tag, () => "OnLongPressed()");

        TaskMonitor.Create(
            async () =>
            {
                // Need a little delay: sometimes long pressed event occurs just before panning
                await Task.Delay(50);

#if IOS || MACCATALYST
                _isLongPressActive = false;
#endif

                if (_isDragging)
                {
                    InternalLogger.Debug(Tag, () => "It is dragging => discard long pressed");
                    return;
                }

                InternalLogger.Debug(Tag, () => "OnLongPressed() => !isDragging");

                var gestureAwareControl = (IGestureAwareControl)sender!;
                var view = (View)gestureAwareControl;

                // Apply stop dragging animation
                if (LongPressedDroppingAnimation != null)
                {
                    TaskMonitor.Create(LongPressedDroppingAnimation(view));
                }
                else
                {
                    TaskMonitor.Create(DragDropAnimations.Dropping.ScaleToNormalAsync(view));
                }

                ((IDragAndDropView)sender!).IsDragAndDropping = false;
                UpdateDisableScrollView(false);
#if !(IOS || MACCATALYST)
                UnsubscribeToPanning(gestureAwareControl);
#endif
            });
    }

    private void SubscribeToPanning(IGestureAwareControl gestureAwareControl)
    {
        InternalLogger.Debug(Tag, () => "SubscribeToPanning()");

        gestureAwareControl.Panning -= OnPanning;
        gestureAwareControl.Panning += OnPanning;

        gestureAwareControl.Panned -= OnPanned;
        gestureAwareControl.Panned += OnPanned;
    }

    private void UnsubscribeToPanning(IGestureAwareControl gestureAwareControl)
    {
        gestureAwareControl.Panning -= OnPanning;
        gestureAwareControl.Panned -= OnPanned;
    }

    private void OnPanning(object sender, PanEventArgs e)
    {
        InternalLogger.Debug(Tag, () => "OnPanning");
        var view = (View)sender;

#if IOS || MACCATALYST
        // In LongPress mode on iOS/Mac, only process panning while long-press is active
        if (DragAndDropTrigger == DragAndDropTrigger.LongPress && !_isLongPressActive)
        {
            return;
        }
#endif
        if (!_isDragging)
        {
            StartDraggingSession(view);
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (e.Cancelled)
        {
            InternalLogger.Debug(Tag, () => "OnPanning cancelled");
            TaskMonitor.Create(OnViewDroppedAsync(view));
            return;
        }

        view.TranslationX += e.TotalDistance.X;
        view.TranslationY += e.TotalDistance.Y;

        if (_scrollView != null)
        {
            HandleScrollViewEdges(_scrollView, view);
        }

        CheckCandidates(view);

        stopwatch.Stop();
    }

    private void OnPanned(object sender, PanEventArgs e)
    {
        InternalLogger.Debug(Tag, () => "OnPanned()");

        if (DragAndDropTrigger == DragAndDropTrigger.LongPress)
        {
            var gestureAwareControl = (IGestureAwareControl)sender!;
#if !(IOS || MACCATALYST)
            // On non-Apple platforms we can safely unsubscribe here
            UnsubscribeToPanning(gestureAwareControl);
#else
            if (!_isLongPressActive)
            {
                // On iOS/Mac panning is always subscribed in LongPress mode, so we just exit if long-press is not active
                return;
            }
#endif
            var view = (View)gestureAwareControl;

            // Apply stop dragging animation
            if (LongPressedDroppingAnimation != null)
            {
                TaskMonitor.Create(LongPressedDroppingAnimation(view));
            }
            else
            {
                TaskMonitor.Create(DragDropAnimations.Dropping.ScaleToNormalAsync(view));
            }

            UpdateDisableScrollView(false);
        }

        TaskMonitor.Create(OnViewDroppedAsync((View)sender));
    }
}
