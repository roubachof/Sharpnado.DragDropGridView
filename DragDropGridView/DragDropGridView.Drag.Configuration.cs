using System.Windows.Input;
using Sharpnado.Tasks;
using ScrollView = Microsoft.Maui.Controls.ScrollView;

namespace Sharpnado.Maui.DragDropGridView;

public partial class DragDropGridView
{
    public static readonly BindableProperty DragAndDropTriggerProperty = BindableProperty.Create(
        nameof(DragAndDropTrigger),
        typeof(DragAndDropTrigger),
        typeof(DragDropGridView),
        DragAndDropTrigger.Pan);

    public static readonly BindableProperty IsDragAndDropEnabledProperty = BindableProperty.Create(
        nameof(IsDragAndDropEnabled),
        typeof(bool),
        typeof(DragDropGridView),
        false,
        propertyChanged: (bindable, _, newValue) =>
        {
            var gridLayout = (DragDropGridView)bindable;
            var isEnabled = (bool)newValue;

            gridLayout.UpdateIsDragAndDropEnabled(isEnabled);
        });

    public static readonly BindableProperty OnItemsReorderedCommandProperty = BindableProperty.Create(
        nameof(OnItemsReorderedCommand),
        typeof(ICommand),
        typeof(DragDropGridView),
        null);

    private readonly List<IView> _orderedChildren = [];

    private List<View> _draggingSessionList = [];

    private ScrollView? _scrollView;
    private RefreshView? _refreshView;

    private bool _isDragging;

#if IOS || MACCATALYST
    // iOS/MacCatalyst: track long-press lifecycle to avoid cancelling recognizers mid-gesture
    private bool _isLongPressActive;
#endif

    private View? _draggingView;

    private View? _groupCandidate;

    private bool _shouldInvalidate = true;

    private ITaskMonitor _currentShiftTask = TaskMonitor.NotStartedTask;

    private CancellationTokenSource? _animationCts;

    public Func<View, Task>? LongPressedDraggingAnimation { get; set; }

    public Func<View, Task>? LongPressedDroppingAnimation { get; set; }

    public Func<View, Task>? DragAndDropItemsAnimation { get; set; }

    public Func<View, Task>? DragAndDropEndItemsAnimation { get; set; }

    public DragAndDropTrigger DragAndDropTrigger
    {
        get => (DragAndDropTrigger)GetValue(DragAndDropTriggerProperty);
        set => SetValue(DragAndDropTriggerProperty, value);
    }

    public bool IsDragAndDropEnabled
    {
        get => (bool)GetValue(IsDragAndDropEnabledProperty);
        set => SetValue(IsDragAndDropEnabledProperty, value);
    }

    /// <summary>
    /// Pass as arguments the newly reordered list of BindingContext (List{object}).
    /// </summary>
    public ICommand OnItemsReorderedCommand
    {
        get => (ICommand)GetValue(OnItemsReorderedCommandProperty);
        set => SetValue(OnItemsReorderedCommandProperty, value);
    }
}
