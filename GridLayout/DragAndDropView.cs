namespace Sharpnado.GridLayout;

/// <summary>
/// A lightweight, headless wrapper view that implements IDragAndDropView.
/// </summary>
public partial class DragAndDropView : MR.Gestures.ContentView, IDragAndDropView
{
    public static readonly BindableProperty CanReceiveViewProperty = BindableProperty.Create(
        nameof(CanReceiveView),
        typeof(bool),
        typeof(DragAndDropView),
        false);

    public static readonly BindableProperty CanBeDroppedProperty = BindableProperty.Create(
        nameof(CanBeDropped),
        typeof(bool),
        typeof(DragAndDropView),
        true);

    public static readonly BindableProperty CanMoveProperty = BindableProperty.Create(
        nameof(CanMove),
        typeof(bool),
        typeof(DragAndDropView),
        true);

    public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
        nameof(IsDragAndDropping),
        typeof(bool),
        typeof(DragAndDropView),
        false);

    public static readonly BindableProperty OnViewDroppedCommandProperty = BindableProperty.Create(
        nameof(OnViewDroppedCommand),
        typeof(System.Windows.Input.ICommand),
        typeof(DragAndDropView),
        null);

    public DragAndDropView()
    {
        // Make this view headless by default to avoid adding layout overhead
        CompressedLayout.SetIsHeadless(this, true);
        
        // Ensure this view can receive touch input (critical for Android)
        InputTransparent = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this view can receive dropped views.
    /// </summary>
    public bool CanReceiveView
    {
        get => (bool)GetValue(CanReceiveViewProperty);
        set => SetValue(CanReceiveViewProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this view can be dropped onto other views.
    /// </summary>
    public bool CanBeDropped
    {
        get => (bool)GetValue(CanBeDroppedProperty);
        set => SetValue(CanBeDroppedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this view can be moved/dragged.
    /// </summary>
    public bool CanMove
    {
        get => (bool)GetValue(CanMoveProperty);
        set => SetValue(CanMoveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this view is currently being dragged or dropped.
    /// </summary>
    public bool IsDragAndDropping
    {
        get => (bool)GetValue(IsDragAndDroppingProperty);
        set => SetValue(IsDragAndDroppingProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute when a view is dropped onto this view.
    /// The command parameter will be the dropped IDragAndDropView.
    /// </summary>
    public System.Windows.Input.ICommand? OnViewDroppedCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(OnViewDroppedCommandProperty);
        set => SetValue(OnViewDroppedCommandProperty, value);
    }

    /// <summary>
    /// Called when another view is dropped onto this view.
    /// </summary>
    /// <param name="droppedView">The view that was dropped.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnViewDroppedAsync(IDragAndDropView droppedView)
    {
        if (OnViewDroppedCommand?.CanExecute(droppedView) == true)
        {
            OnViewDroppedCommand.Execute(droppedView);
        }

        await Task.CompletedTask;
    }
}
