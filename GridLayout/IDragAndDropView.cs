namespace Sharpnado.GridLayout;

public interface IDragAndDropView
{
    bool CanReceiveView { get; }

    bool CanBeDropped { get; }

    bool CanMove { get; }

    bool IsDragAndDropping { get; set; }

    Task OnViewDroppedAsync(IDragAndDropView droppedView);
}
