namespace Sharpnado.Maui.DragDropGridView;

using Microsoft.Maui.Layouts;

public partial class DragDropGridView : Layout
{
    public static readonly BindableProperty ColumnCountProperty = BindableProperty.Create(
        nameof(ColumnCount),
        typeof(int),
        typeof(DragDropGridView),
        2,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var gridLayout = (DragDropGridView)bindable;
            InternalLogger.Info(Tag, $"ColumnCount changed from {oldValue} to {newValue}");
            
            // Only animate if this is a real change (not initial setup)
            if ((int)oldValue != 0 && (int)oldValue != (int)newValue)
            {
                gridLayout.AnimateLayoutChange();
                return;
            }
            
            // Force invalidation by temporarily setting _shouldInvalidate to true
            var oldShouldInvalidate = gridLayout._shouldInvalidate;
            gridLayout._shouldInvalidate = true;
            gridLayout.InvalidateMeasure();
            gridLayout._shouldInvalidate = oldShouldInvalidate;
        });

    public static readonly BindableProperty RowCountProperty = BindableProperty.Create(
        nameof(RowCount),
        typeof(int),
        typeof(DragDropGridView),
        0,
        BindingMode.TwoWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var gridLayout = (DragDropGridView)bindable;
            InternalLogger.Info(Tag, $"RowCount changed from {oldValue} to {newValue}");
            
            // Only animate if this is a real change (not initial setup)
            if ((int)oldValue != 0 && (int)oldValue != (int)newValue)
            {
                gridLayout.AnimateLayoutChange();
                return;
            }
            
            // Force invalidation by temporarily setting _shouldInvalidate to true
            var oldShouldInvalidate = gridLayout._shouldInvalidate;
            gridLayout._shouldInvalidate = true;
            gridLayout.InvalidateMeasure();
            gridLayout._shouldInvalidate = oldShouldInvalidate;
        });

    public static readonly BindableProperty GridPaddingProperty = BindableProperty.Create(
        nameof(GridPadding),
        typeof(Thickness),
        typeof(DragDropGridView),
        default(Thickness),
        propertyChanged: (bindable, _, _) =>
            {
                ((DragDropGridView)bindable).InvalidateMeasure();
            });

    public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(
        nameof(ColumnSpacing),
        typeof(double),
        typeof(DragDropGridView),
        5.0,
        propertyChanged: (bindable, _, _) =>
            {
                ((DragDropGridView)bindable).InvalidateMeasure();
            });

    public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(
        nameof(RowSpacing),
        typeof(double),
        typeof(DragDropGridView),
        5.0,
        propertyChanged: (bindable, _, _) =>
            {
                ((DragDropGridView)bindable).InvalidateMeasure();
            });

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(object),
        typeof(DragDropGridView),
        null,
        propertyChanged: OnHeaderChanged);

    public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(
        nameof(HeaderTemplate),
        typeof(DataTemplate),
        typeof(DragDropGridView),
        null,
        propertyChanged: OnHeaderTemplateChanged);

    public static readonly BindableProperty AnimateTransitionsProperty = BindableProperty.Create(
        nameof(AnimateTransitions),
        typeof(bool),
        typeof(DragDropGridView),
        true);

    private const string Tag = nameof(DragDropGridView);

    private View? _headerView;

    private DisplayOrientation _currentOrientation;

    public DragDropGridView()
    {

        _currentOrientation = DeviceDisplay.MainDisplayInfo.Orientation;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public int ColumnCount
    {
        get => (int)GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    public int RowCount
    {
        get => (int)GetValue(RowCountProperty);
        set => SetValue(RowCountProperty, value);
    }

    public Thickness GridPadding
    {
        get => (Thickness)GetValue(GridPaddingProperty);
        set => SetValue(GridPaddingProperty, value);
    }

    public double ColumnSpacing
    {
        get => (double)GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public double RowSpacing
    {
        get => (double)GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    public bool AdaptItemWidth { get; set; } = true;

    public bool AdaptItemHeight { get; set; }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public bool AnimateTransitions
    {
        get => (bool)GetValue(AnimateTransitionsProperty);
        set => SetValue(AnimateTransitionsProperty, value);
    }

    // Duration (in ms) used for batched shift animations during reordering.
    // Not bindable on purpose to avoid overhead; tweak per-instance if needed.
    public uint ShiftAnimationDuration { get; set; } = 250;

    protected DragDropGridViewManager? LayoutManager => _layoutManager as DragDropGridViewManager;

    /// <summary>
    /// Clears all children.
    /// </summary>
    public new void Clear()
    {
        // Remove all children except header
        var childrenToRemove = Children.Where(c => c != _headerView).ToList();
        foreach (var child in childrenToRemove)
        {
            Children.Remove(child);
        }
        _orderedChildren.Clear();
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return new DragDropGridViewManager(this);
    }

    private void MainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        if (e.DisplayInfo.Orientation != _currentOrientation)
        {
            _currentOrientation = e.DisplayInfo.Orientation;
            InvalidateMeasure();
        }
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        InternalLogger.Debug(Tag, ">>>> DragDropGridView Loaded");
        MainDisplayInfoChanged(this, new DisplayInfoChangedEventArgs(DeviceDisplay.MainDisplayInfo));
        DeviceDisplay.MainDisplayInfoChanged += MainDisplayInfoChanged;
        InitializeDragAndDrop();
    }

    private void OnUnloaded(object? sender, EventArgs args)
    {
        InternalLogger.Debug(Tag, "DragDropGridView Unloaded <<<<");
        DeviceDisplay.MainDisplayInfoChanged -= MainDisplayInfoChanged;
        ClearDragAndDrop();
    }

    protected override void InvalidateMeasure()
    {
        if (!_shouldInvalidate)
        {
            InternalLogger.Debug(Tag, "DragDropGridView skipping InvalidateMeasure!");
            return;
        }

        base.InvalidateMeasure();
    }

    protected override void InvalidateMeasureOverride()
    {
        if (!_shouldInvalidate)
        {
            InternalLogger.Debug(Tag, "GridLayout skipping InvalidateMeasureOverride!");
            return;
        }

        base.InvalidateMeasureOverride();
    }

    private static void OnHeaderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DragDropGridView gridLayout)
        {
            gridLayout.UpdateHeader();
        }
    }

    private static void OnHeaderTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DragDropGridView gridLayout)
        {
            gridLayout.UpdateHeader();
        }
    }

    private void UpdateHeader()
    {
        // Remove existing header view
        if (_headerView != null)
        {
            Children.Remove(_headerView);
            _headerView = null;
        }

        // Create new header view
        if (HeaderTemplate != null)
        {
            // Use template to create view
            if (HeaderTemplate.CreateContent() is View headerView)
            {
                // Set binding context if Header is provided
                if (Header != null)
                {
                    headerView.BindingContext = Header;
                }
                _headerView = headerView;
            }
        }
        else if (Header != null)
        {
            if (Header is View view)
            {
                // Header is already a view
                _headerView = view;
            }
            else
            {
                // Header is a simple object, wrap it in a label
                _headerView = new Label
                {
                    Text = Header?.ToString(),
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                };
            }
        }

        if (_headerView != null)
        {
            // Insert header at the beginning
            Children.Insert(0, _headerView);
            InvalidateMeasure();
        }
    }

    private void AnimateLayoutChange()
    {
        InternalLogger.Debug(Tag, "AnimateLayoutChange()");

        // If animations are disabled, just invalidate the measure and return
        if (!AnimateTransitions)
        {
            InvalidateMeasure();
            return;
        }

        // Store current positions of all visible children
        var childrenInfo = new Dictionary<View, Rect>();
        
        foreach (var child in _orderedChildren)
        {
            if (child is not View view || view == _headerView)
            {
                continue;
            }

            if (!view.IsVisible)
            {
                continue;
            }

            // Store current bounds
            childrenInfo[view] = view.Bounds;
            ((View)child).Opacity = 0;
        }

        // Trigger layout update
        InvalidateMeasure();

        // After layout invalidation completes, animate to new positions
        Dispatcher.Dispatch(async () =>
        {
            // Wait for layout to update
            await Task.Delay(16); // One frame
            _shouldInvalidate = false;
            try
            {
                // Animate each child from old position to new position
                var tasks = new List<Task>();

                foreach (var kvp in childrenInfo)
                {
                    var view = kvp.Key;
                    var oldBounds = kvp.Value;
                    var newBounds = view.Bounds;

                    // Skip if view was removed or bounds didn't change significantly
                    if (!_orderedChildren.Contains(view) ||
                        (Math.Abs(oldBounds.X - newBounds.X) < 1 &&
                         Math.Abs(oldBounds.Y - newBounds.Y) < 1 &&
                         Math.Abs(oldBounds.Width - newBounds.Width) < 1 &&
                         Math.Abs(oldBounds.Height - newBounds.Height) < 1))
                    {
                        continue;
                    }

                    // Reset any previous animation properties to avoid cumulative effects
                    view.ScaleX = 1.0;
                    view.ScaleY = 1.0;
                    view.Scale = 1.0;

                    // Calculate translation to move from old position to new position
                    var deltaX = oldBounds.X - newBounds.X;
                    var deltaY = oldBounds.Y - newBounds.Y;

                    // Set initial state (at old position)
                    view.TranslationX = deltaX;
                    view.TranslationY = deltaY;
                    view.Opacity = 0.8;

                    // Animate to new position
                    var animationTask = Task.WhenAll(
                        view.TranslateTo(0, 0, 300, Easing.CubicOut),
                        view.FadeTo(1.0, 200, Easing.SinOut)
                    );

                    tasks.Add(animationTask);
                }

                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }
            finally
            {
                _shouldInvalidate = true;
            }
        });
    }
}
