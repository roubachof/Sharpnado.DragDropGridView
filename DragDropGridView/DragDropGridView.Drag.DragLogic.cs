namespace Sharpnado.Maui.DragDropGridView;

public partial class DragDropGridView
{
    private void StartDraggingSession(View view)
    {
        InternalLogger.Debug(Tag, () => $"StartDraggingSession {view.GetType().Name}");
        _isDragging = true;
        _draggingSessionList = [.._orderedChildren.Cast<View>()];

        ((IDragAndDropView)view).IsDragAndDropping = true;

        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = null;

        _shouldInvalidate = false;
        _draggingView = view;

#if !ANDROID
        // ZIndex cancels gestures on Android, so only elevate on iOS/Mac/Windows
        view.ZIndex += 100;
#endif
    }
}
