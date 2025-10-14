using System.Diagnostics;
using System.Windows.Input;

using Sharpnado.Tasks;

using MR.Gestures;
using ScrollView = Microsoft.Maui.Controls.ScrollView;

namespace Sharpnado.GridLayout;

public partial class GridLayout
{
    public static readonly BindableProperty IsDragAndDropEnabledProperty = BindableProperty.Create(
        nameof(IsDragAndDropEnabled),
        typeof(bool),
        typeof(GridLayout),
        false,
        propertyChanged: (bindable, _, newValue) =>
        {
            var gridLayout = (GridLayout)bindable;
            var isEnabled = (bool)newValue;

            gridLayout.UpdateIsDragAndDropEnabled(isEnabled);
        });

    public static readonly BindableProperty OnItemsReorderedCommandProperty = BindableProperty.Create(
        nameof(OnItemsReorderedCommand),
        typeof(ICommand),
        typeof(GridLayout),
        null);

    private readonly List<IView> _orderedChildren = [];

    private List<View> _draggingSessionList = [];

    private ScrollView? _scrollView;
    private RefreshView? _refreshView;

    private bool _isDragging;

    private View? _draggingView;

    private View? _groupCandidate;

    private bool _shouldInvalidate = true;

    private ITaskMonitor _currentShiftTask = TaskMonitor.NotStartedTask;

    private CancellationTokenSource? _animationCts;

    public Func<View, Task>? DragAndDropEnabledItemsAnimation { get; set; }

    public Func<View, Task>? DragAndDropDisabledItemsAnimation { get; set; }

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
            if (DragAndDropEnabledItemsAnimation != null)
            {
                _animationCts?.Cancel();
                _animationCts?.Dispose();
                _animationCts = new CancellationTokenSource();
                TaskMonitor.Create(ApplyAnimationToChildrenAsync(_animationCts.Token));
            }
        }
        else
        {
            _animationCts?.Cancel();
            _animationCts?.Dispose();
            _animationCts = null;
        }

        UpdateDisableScrollView(isEnabled);
    }

    private void SubscribeDragAndDropIfNeeded(Element child)
    {
        if (child is not (IGestureAwareControl gestureAwareControl and IDragAndDropView))
        {
            return;
        }

        InternalLogger.Debug(Tag, () => "SubscribeDragAndDropIfNeeded");

        if (DeviceInfo.Platform == DevicePlatform.tvOS)
        {
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

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            gestureAwareControl.LongPressed -= OnLongPressed;
            gestureAwareControl.LongPressing -= OnLongPressing;
        }

        UnsubscribeToPanning(gestureAwareControl);
    }

    private void OnLongPressing(object sender, LongPressEventArgs e)
    {
        InternalLogger.Debug(Tag, () => "OnLongPressing()");

        var gestureAwareControl = (IGestureAwareControl)sender;

        TaskMonitor.Create(((View)gestureAwareControl).ScaleTo(1.05, 100));
        ((IDragAndDropView)sender).IsDragAndDropping = true;

        SubscribeToPanning(gestureAwareControl);
    }

    private void OnLongPressed(object sender, LongPressEventArgs e)
    {
        InternalLogger.Debug(Tag, () => "OnLongPressed()");

        TaskMonitor.Create(
            async () =>
            {
                // Need a little delay: sometimes long pressed event occurs just before panning
                await Task.Delay(50);
                if (_isDragging)
                {
                    return;
                }

                InternalLogger.Debug(Tag, () => "OnLongPressed() => !isDragging");

                var gestureAwareControl = (IGestureAwareControl)sender;
                TaskMonitor.Create(((View)gestureAwareControl).ScaleTo(1, 100));

                ((IDragAndDropView)sender).IsDragAndDropping = false;

                UnsubscribeToPanning(gestureAwareControl);
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

        TaskMonitor.Create(OnViewDroppedAsync((View)sender));
    }

    private void StartDraggingSession(View view)
    {
        InternalLogger.Debug(Tag, () => $"StartDraggingSession {view.GetType().Name}");
        _isDragging = true;
        _draggingSessionList = [.._orderedChildren.Cast<View>()];

        ((IDragAndDropView)view).IsDragAndDropping = true;
        // FIXME: this make the layout trigger a new measure and layout pass and then stop the panning
        // ((View)view).ZIndex += 100;

        _shouldInvalidate = false;
        _draggingView = view;
    }

    private void UpdateDisableScrollView(bool isDisabled)
    {
#if ANDROID
        if (_scrollView != null)
        {
            InternalLogger.Debug(Tag, () => $"UpdateScrollView( disabled: {isDisabled} )");
            ((UntouchableScrollviewHandler)_scrollView.Handler!).UpdateDisableScrolling(isDisabled);
        }

        if (_refreshView != null)
        {
            InternalLogger.Debug(Tag, () => $"UpdateRefreshView( disabled: {isDisabled} )");
            ((UntouchableRefreshViewHandler)_refreshView.Handler!).UpdateDisableScrolling(isDisabled);
        }
#endif
    }

    private void HandleScrollViewEdges(ScrollView scrollView, View view)
    {
        var viewScreenCoordinates = view.GetScreenCoordinates<Application>();
        var scrollScreenCoordinates = scrollView.GetScreenCoordinates<Application>();
        var containerScreenCoordinates = this.GetScreenCoordinates<Application>();

        double toolbarHeight = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape ? 48 : 56;
        double screenHeight = (DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density) - toolbarHeight;

        InternalLogger.Debug(Tag, () => $"screenHeight: {screenHeight}, viewScreenCoordinates: {viewScreenCoordinates}, scrollScreenCoordinates: {scrollScreenCoordinates}, containerScreenCoordinates: {containerScreenCoordinates}");

        double endScreenY = scrollView.ScrollY + scrollScreenCoordinates.Y + screenHeight;

        double viewBottom = view.TranslationY + viewScreenCoordinates.Y + view.Height;

        double exceedingBottom = viewBottom - endScreenY;

        double targetY = scrollView.ScrollY + exceedingBottom;

        double containerBottom = containerScreenCoordinates.Y + Height;
        double exceedingContainerBottom = containerBottom - endScreenY;

        InternalLogger.Debug(Tag, () => $"exceedingContainerBottom: {exceedingContainerBottom}");

        if (exceedingBottom > 0 && exceedingContainerBottom > 0)
        {
            InternalLogger.Debug(Tag, () => $"Scrolling to exceeding bottom: {exceedingBottom}, scrollTargetY: {targetY}");
            TaskMonitor.Create(scrollView.ScrollToAsync(0, targetY, false));
        }

        double startScreenY = scrollView.ScrollY + scrollScreenCoordinates.Y;
        double viewTop = view.TranslationY + viewScreenCoordinates.Y;
        double exceedingTop = viewTop - startScreenY;
        double targetYTop = scrollView.ScrollY + exceedingTop;

        InternalLogger.Debug(Tag, () => $"viewTop: {viewTop}, scrollY: {scrollView.ScrollY}, exceedingTop: {exceedingTop}");

        if (exceedingTop < 10)
        {
            InternalLogger.Debug(Tag, () => $"Scrolling to exceeding top: {exceedingTop}, scrollTargetY: {targetYTop}");
            TaskMonitor.Create(scrollView.ScrollToAsync(0, targetYTop, false));
        }
    }

    private static Task WobbleAnimationAsync(View view)
    {
        return view.Wobble();
    }

    private static Task PostWobbleAnimationAsync(View view)
    {
        return view.RotateTo(0);
    }

    private Task ApplyAnimationToChildrenAsync(CancellationToken token)
    {
        List<Task> animationTasks = new List<Task>();
        foreach (var child in Children)
        {
            animationTasks.Add(ApplyAnimationAsync((View)child, token));
        }

        return Task.WhenAll(animationTasks);
    }

    private async Task ApplyAnimationAsync(View view, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await DragAndDropEnabledItemsAnimation(view);
        }

        if (DragAndDropDisabledItemsAnimation != null)
        {
            await DragAndDropDisabledItemsAnimation(view);
        }
    }
}
