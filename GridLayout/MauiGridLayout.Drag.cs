namespace Sharpnado.GridLayout;

using System.Diagnostics;
using System.Windows.Input;

using Tasks;
using ScrollView = ScrollView;

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

    private double _lastPanX;

    private double _lastPanY;

    private CancellationTokenSource? _autoScrollCts;

    private bool _isAutoScrolling;

    private double _currentScrollSpeed;

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
        InternalLogger.Debug(Tag, $"Remove( child: {child.GetType().Name}, index: {oldLogicalIndex} )");

        // Skip header view
        if (child == _headerView)
        {
            base.OnChildRemoved(child, oldLogicalIndex);
            return;
        }

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

        InternalLogger.Debug(Tag, $"OnChildAdded( child: {child.GetType().Name} )");

        // Skip header view
        if (child == _headerView)
        {
            return;
        }

        _orderedChildren.Add((View)child);

        if (IsDragAndDropEnabled)
        {
            SubscribeDragAndDropIfNeeded(child);
        }
    }

    private void InitializeDragAndDrop()
    {
        InternalLogger.Debug(Tag, "InitializeDragAndDrop()");

        _scrollView = this.GetFirstParentOfType<ScrollView>();
        _refreshView = this.GetFirstParentOfType<RefreshView>();
        InternalLogger.Debug(Tag, _scrollView != null ? "Parent ScrollView found" : "Warning: No ScrollView found!");
        InternalLogger.Debug(Tag, _refreshView != null ? "Parent RefreshView found" : "No RefreshView found (this is fine if you don't use one).");

        UpdateIsDragAndDropEnabled(IsDragAndDropEnabled);
    }

    private void ClearDragAndDrop()
    {
        InternalLogger.Debug(Tag, "ClearDragAndDrop()");

        foreach (var child in Children)
        {
            // Skip header view
            if (child == _headerView)
            {
                continue;
            }

            UnsubscribeDragAndDropIfNeeded((Element)child);
        }

        _scrollView = null;
        _draggingSessionList.Clear();
        _animationCts?.Dispose();
        _animationCts = null;
        _autoScrollCts?.Cancel();
        _autoScrollCts?.Dispose();
        _autoScrollCts = null;
    }

    private void UpdateIsDragAndDropEnabled(bool isEnabled)
    {
        InternalLogger.Debug(Tag, $"UpdateIsDragAndDropEnabled( isEnabled: {isEnabled} )");

        // On Android, we need to disable scrolling BEFORE enabling gestures
        // Otherwise the ScrollView will intercept all touch events
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            ConfigureScrollViewForDragAndDrop(isEnabled);
        }

        foreach (var child in Children)
        {
            // Skip header view
            if (child == _headerView)
            {
                continue;
            }

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
    }

    private void SubscribeDragAndDropIfNeeded(Element child)
    {
        InternalLogger.Debug(Tag, "SubscribeDragAndDropIfNeeded");

        if (child is not (View view and IDragAndDropView))
        {
            InternalLogger.Warn(Tag, $"Child of type {child.GetType().Name} does not implement IDragAndDropView, skipping drag and drop subscription.");
            return;
        }

        // Add Pan gesture for drag and drop
        var panGesture = new PanGestureRecognizer
        {
            // Setting TouchPoints helps Android recognize the gesture more reliably
            TouchPoints = 1
        };
        panGesture.PanUpdated += OnPanUpdated;
        view.GestureRecognizers.Add(panGesture);
        
        InternalLogger.Debug(Tag, $"Added pan gesture to {view.GetType().Name}, gesture count: {view.GestureRecognizers.Count}");
    }

    private void UnsubscribeDragAndDropIfNeeded(Element child)
    {
        if (child is not View view)
        {
            return;
        }

        InternalLogger.Debug(Tag, "UnsubscribeDragAndDropIfNeeded");

        // Remove all pan gestures
        var panGestures = view.GestureRecognizers.OfType<PanGestureRecognizer>().ToList();
        foreach (var gesture in panGestures)
        {
            gesture.PanUpdated -= OnPanUpdated;
            view.GestureRecognizers.Remove(gesture);
        }
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (sender is not View view)
        {
            return;
        }

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                InternalLogger.Debug(Tag, () => "Pan Started");
                _lastPanX = 0;
                _lastPanY = 0;
                StartDraggingSession(view);
                if (view is IDragAndDropView dragView)
                {
                    dragView.IsDragAndDropping = true;
                }
                break;

            case GestureStatus.Running:
                if (!_isDragging)
                {
                    break;
                }

                // Calculate incremental movement
                double deltaX = e.TotalX - _lastPanX;
                double deltaY = e.TotalY - _lastPanY;
                _lastPanX = e.TotalX;
                _lastPanY = e.TotalY;

                // Update position based on delta
                view.TranslationX += deltaX;
                view.TranslationY += deltaY;

                if (_scrollView != null)
                {
                    HandleScrollViewEdges(_scrollView, view);
                }

                CheckCandidates(view);
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                InternalLogger.Debug(Tag, "Pan Completed/Cancelled");
                StopAutoScroll();
                if (view is IDragAndDropView dragViewEnd)
                {
                    dragViewEnd.IsDragAndDropping = false;
                }
                // view.ZIndex -= 100;
                TaskMonitor.Create(OnViewDroppedAsync(view));
                break;
        }
    }

    private void StartDraggingSession(View view)
    {
        InternalLogger.Debug(Tag, $"StartDraggingSession {view.GetType().Name}");
        _isDragging = true;
        _draggingSessionList = [.._orderedChildren.Cast<View>()];

        ((IDragAndDropView)view).IsDragAndDropping = true;
        // FIXME: this make the layout trigger a new measure and layout pass and then stop the panning
        // view.ZIndex += 100;

        _shouldInvalidate = false;
        _draggingView = view;
    }

    private void ConfigureScrollViewForDragAndDrop(bool dragAndDropEnabled)
    {
        InternalLogger.Debug(Tag, $"ConfigureScrollViewForDragAndDrop( dragAndDropEnabled: {dragAndDropEnabled} )");

        if (_scrollView != null)
        {
#if ANDROID
            // When drag-and-drop is enabled, we disable scrolling so touches reach our gesture recognizers
            var disableScrolling = dragAndDropEnabled;
            InternalLogger.Debug(Tag, $"Setting ScrollView disableScrolling to: {disableScrolling}");
            ((UntouchableScrollviewHandler)_scrollView.Handler!).UpdateDisableScrolling(disableScrolling);
#endif
        }

        if (_refreshView != null)
        {
            // Disable RefreshView when drag-and-drop is enabled
            var refreshEnabled = !dragAndDropEnabled;
            InternalLogger.Debug(Tag, $"Setting RefreshView enabled to: {refreshEnabled}");
            _refreshView.IsEnabled = refreshEnabled;
        }
    }

    private void HandleScrollViewEdges(ScrollView scrollView, View view)
    {
        var viewScreenCoordinates = view.GetScreenCoordinates();
        var scrollScreenCoordinates = scrollView.GetScreenCoordinates();
        var containerScreenCoordinates = this.GetScreenCoordinates();

        double toolbarHeight = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape ? 48 : 56;
        double screenHeight = (DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density) - toolbarHeight;

        InternalLogger.Debug(Tag, $"screenHeight: {screenHeight}, viewScreenCoordinates: {viewScreenCoordinates}, scrollScreenCoordinates: {scrollScreenCoordinates}, containerScreenCoordinates: {containerScreenCoordinates}");

        // Define the edge zone size (larger = easier to trigger auto-scroll)
        const double edgeZoneSize = 120.0;
        const double maxScrollSpeed = 25.0; // pixels per frame

        double startScreenY = scrollView.ScrollY + scrollScreenCoordinates.Y;
        double endScreenY = scrollView.ScrollY + scrollScreenCoordinates.Y + screenHeight;
        double viewCenterY = view.TranslationY + viewScreenCoordinates.Y + (view.Height / 2);

        double containerBottom = containerScreenCoordinates.Y + Height;
        double maxScrollY = Math.Max(0, containerBottom - startScreenY);

        InternalLogger.Debug(Tag, $"viewCenterY: {viewCenterY}, startScreenY: {startScreenY}, endScreenY: {endScreenY}, scrollY: {scrollView.ScrollY}");

        // Check if we're in the top edge zone
        double distanceFromTop = viewCenterY - startScreenY;
        if (distanceFromTop > 0 && distanceFromTop < edgeZoneSize && scrollView.ScrollY > 0)
        {
            // Calculate scroll speed based on proximity to edge (closer = faster)
            double scrollSpeed = maxScrollSpeed * (1 - (distanceFromTop / edgeZoneSize));
            StartAutoScroll(scrollView, -scrollSpeed, maxScrollY);
            return;
        }

        // Check if we're in the bottom edge zone
        double distanceFromBottom = endScreenY - viewCenterY;
        if (distanceFromBottom > 0 && distanceFromBottom < edgeZoneSize && scrollView.ScrollY < maxScrollY)
        {
            // Calculate scroll speed based on proximity to edge (closer = faster)
            double scrollSpeed = maxScrollSpeed * (1 - (distanceFromBottom / edgeZoneSize));
            StartAutoScroll(scrollView, scrollSpeed, maxScrollY);
            return;
        }

        // If not in any edge zone, stop auto-scrolling
        StopAutoScroll();
    }

    private void StartAutoScroll(ScrollView scrollView, double scrollSpeed, double maxScrollY)
    {
        if (_isAutoScrolling)
        {
            // Update speed for existing scroll
            _currentScrollSpeed = scrollSpeed;
            return;
        }

        _isAutoScrolling = true;
        _currentScrollSpeed = scrollSpeed;
        _autoScrollCts?.Cancel();
        _autoScrollCts?.Dispose();
        _autoScrollCts = new CancellationTokenSource();

        InternalLogger.Debug(Tag, $"Starting auto-scroll with speed: {scrollSpeed}");

        Task.Run(async () =>
        {
            try
            {
                while (!_autoScrollCts.Token.IsCancellationRequested && _isDragging)
                {
                    double actualScrollSpeed = _currentScrollSpeed;
                    double scrollBefore = scrollView.ScrollY;
                    
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        if (_autoScrollCts.Token.IsCancellationRequested || !_isDragging)
                        {
                            return;
                        }

                        double newScrollY = Math.Max(0, Math.Min(maxScrollY, scrollView.ScrollY + actualScrollSpeed));
                        await scrollView.ScrollToAsync(0, newScrollY, false);
                    });

                    // Calculate actual scroll that happened
                    double actualScrollDelta = scrollView.ScrollY - scrollBefore;
                    
                    // Compensate dragging view position to keep it visually stable
                    if (_draggingView != null && Math.Abs(actualScrollDelta) > 0.1)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            if (_draggingView != null)
                            {
                                _draggingView.TranslationY += actualScrollDelta;
                            }
                        });
                    }

                    await Task.Delay(16, _autoScrollCts.Token); // ~60 FPS
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping auto-scroll
            }
            finally
            {
                _isAutoScrolling = false;
            }
        }, _autoScrollCts.Token);
    }

    private void StopAutoScroll()
    {
        if (!_isAutoScrolling)
        {
            return;
        }

        InternalLogger.Debug(Tag, "Stopping auto-scroll");
        _autoScrollCts?.Cancel();
        _isAutoScrolling = false;
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
            // Skip header view - it should not be animated or participate in drag and drop
            if (child == _headerView)
            {
                continue;
            }

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
