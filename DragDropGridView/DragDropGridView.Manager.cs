namespace Sharpnado.Maui.DragDropGridView;

using Microsoft.Maui.Layouts;

using static Double;

public partial class DragDropGridView
{
    protected class DragDropGridViewManager : LayoutManager
    {
        private const string Tag = nameof(DragDropGridViewManager);

        public DragDropGridViewManager(DragDropGridView dragDropGridView)
            : base(dragDropGridView)
        {
        }

        private DragDropGridView DragDropGridView => (DragDropGridView)Layout;

        private readonly struct LayoutData(int visibleChildCount, Size cellSize, int rows, int columns)
        {
            public int VisibleChildCount { get; } = visibleChildCount;

            public Size CellSize { get; } = cellSize;

            public int Rows { get; } = rows;

            public int Columns { get; } = columns;

            public override string ToString()
            {
                return "{VisibleChildCount=" + VisibleChildCount + ", CellSize=" + CellSize + ", Rows=" + Rows + ", Columns=" + Columns + "}";
            }
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var headerHeight = 0.0;
            
            // Measure header if present
            if (DragDropGridView._headerView is VisualElement headerElement && headerElement.IsVisible)
            {
                var headerSize = DragDropGridView._headerView.Measure(widthConstraint, heightConstraint);
                headerHeight = headerSize.Height;
            }

            var layoutData = GetLayoutData(widthConstraint, heightConstraint - headerHeight);

            if (layoutData.VisibleChildCount == 0 && headerHeight == 0)
            {
                return default;
            }

            var computedWidth = (layoutData.CellSize.Width * layoutData.Columns) +
                                (DragDropGridView.ColumnSpacing * (layoutData.Columns - 1)) +
                                DragDropGridView.GridPadding.HorizontalThickness;

            var computedHeight = headerHeight +
                                 (layoutData.CellSize.Height * layoutData.Rows) +
                                 (DragDropGridView.RowSpacing * (layoutData.Rows - 1)) +
                                 DragDropGridView.GridPadding.VerticalThickness;

            var finalWidth = ResolveConstraints(
                widthConstraint,
                Layout.Width,
                computedWidth,
                Layout.MinimumWidth,
                Layout.MaximumWidth);

            var finalHeight = ResolveConstraints(
                heightConstraint,
                Layout.Height,
                computedHeight,
                Layout.MinimumHeight,
                Layout.MaximumHeight);

            var totalSize = new Size(finalWidth, finalHeight);
            return totalSize;
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            var headerHeight = 0.0;
            
            // Arrange header if present
            if (DragDropGridView._headerView is VisualElement headerElement && headerElement.IsVisible)
            {
                var headerSize = DragDropGridView._headerView.Measure(bounds.Width, bounds.Height);
                var headerBounds = new Rect(bounds.X, bounds.Y, bounds.Width, headerSize.Height);
                DragDropGridView._headerView.Arrange(headerBounds);
                headerHeight = headerSize.Height;
            }

            var layoutData = GetLayoutData(bounds.Width, bounds.Height - headerHeight);

            var padding = DragDropGridView.GridPadding;
            var cellSize = layoutData.CellSize;
            var width = bounds.Width - padding.HorizontalThickness;
            var height = bounds.Height - padding.VerticalThickness - headerHeight;

            if (!DragDropGridView._shouldInvalidate)
            {
                InternalLogger.Debug(Tag, "!shouldInvalidate => skipping arrange children");
                return new Size(width, height);
            }

            if (layoutData.VisibleChildCount == 0)
            {
                return bounds.Size;
            }

            var xChild = bounds.X + padding.Left;
            var yChild = bounds.Y + headerHeight + padding.Top;
            var column = 0;

            InternalLogger.Debug(Tag, $"{nameof(ArrangeChildren)}(): Number of children => {DragDropGridView._orderedChildren.Count}");

            foreach (var child in DragDropGridView._orderedChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                if (child != DragDropGridView._draggingView)
                {
                    ((View)child).TranslationX = 0;
                    ((View)child).TranslationY = 0;

                    var destination = new Rect(new Point(xChild, yChild), cellSize);

                    child.Arrange(destination);
                }

                if (++column == layoutData.Columns)
                {
                    column = 0;
                    xChild = bounds.X + padding.Left;
                    yChild += DragDropGridView.RowSpacing + cellSize.Height;
                }
                else
                {
                    xChild += DragDropGridView.ColumnSpacing + cellSize.Width;
                }
            }

            return new Size(width, height);
        }

        private LayoutData GetLayoutData(double width, double height)
        {
            width -= DragDropGridView.GridPadding.HorizontalThickness;
            height -= DragDropGridView.GridPadding.VerticalThickness;

            if (DragDropGridView._orderedChildren.Count == 0)
            {
                return default;
            }

            var visibleChildCount = 0;
            Size maxChildSize = default;
            LayoutData layoutData = default;

            InternalLogger.Debug(Tag, $"{nameof(GetLayoutData)}(): Number of children => {DragDropGridView._orderedChildren.Count}");

            foreach (var child in DragDropGridView._orderedChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                visibleChildCount++;

                var childSize = child.Measure(PositiveInfinity, PositiveInfinity);

                maxChildSize.Width = Math.Max(maxChildSize.Width, childSize.Width);
                maxChildSize.Height = Math.Max(maxChildSize.Height, childSize.Height);
            }

            if (visibleChildCount != 0)
            {
                int rows;
                int columns;
                if (IsPositiveInfinity(width))
                {
                    // Use the explicitly set ColumnCount if available
                    if (DragDropGridView.ColumnCount > 0)
                    {
                        columns = DragDropGridView.ColumnCount;
                        rows = (visibleChildCount + columns - 1) / columns;
                    }
                    else
                    {
                        columns = visibleChildCount;
                        rows = 1;
                    }
                }
                else
                {
                    // If ColumnCount is explicitly set, use it; otherwise calculate based on width
                    if (DragDropGridView.ColumnCount > 0)
                    {
                        columns = DragDropGridView.ColumnCount;
                    }
                    else
                    {
                        columns = (int)((width + DragDropGridView.ColumnSpacing) / (maxChildSize.Width + DragDropGridView.ColumnSpacing));
                        columns = Math.Max(1, columns);
                    }
                    rows = (visibleChildCount + columns - 1) / columns;
                }

                // Now maximize the cell size based on the layout size.
                Size cellSize = default;

                if (IsPositiveInfinity(width) || !DragDropGridView.AdaptItemWidth)
                {
                    cellSize.Width = maxChildSize.Width;
                }
                else
                {
                    cellSize.Width = (width - (DragDropGridView.ColumnSpacing * (columns - 1))) / columns;
                }

                if (IsPositiveInfinity(height) || !DragDropGridView.AdaptItemHeight)
                {
                    cellSize.Height = maxChildSize.Height;
                }
                else
                {
                    cellSize.Height = (height - (DragDropGridView.RowSpacing * (rows - 1))) / rows;
                }

                layoutData = new LayoutData(visibleChildCount, cellSize, rows, columns);
            }

            DragDropGridView.ColumnCount = layoutData.Columns;

            return layoutData;
        }
    }
}
