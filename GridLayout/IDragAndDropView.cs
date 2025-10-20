using MR.Gestures;

namespace Sharpnado.GridLayout;

public interface IDragAndDropView : IGestureAwareControl
{
    bool CanReceiveView { get; }

    bool CanBeDropped { get; }

    bool CanMove { get; }

    bool IsDragAndDropping { get; set; }

    Task OnViewDroppedAsync(IDragAndDropView droppedView);
}
