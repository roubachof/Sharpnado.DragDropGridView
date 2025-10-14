namespace Sharpnado.GridLayout;

using Microsoft.Maui.Layouts;

using static Double;

public partial class GridLayout
{
    protected class GridLayoutManager : LayoutManager
    {
        private const string Tag = nameof(GridLayoutManager);

        public GridLayoutManager(GridLayout gridLayout)
            : base(gridLayout)
        {
        }

        private GridLayout GridLayout => (GridLayout)Layout;

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
            if (GridLayout._headerView is VisualElement headerElement && headerElement.IsVisible)
            {
                var headerSize = GridLayout._headerView.Measure(widthConstraint, heightConstraint);
                headerHeight = headerSize.Height;
            }

            var layoutData = GetLayoutData(widthConstraint, heightConstraint - headerHeight);

            if (layoutData.VisibleChildCount == 0 && headerHeight == 0)
            {
                return default;
            }

            var computedWidth = (layoutData.CellSize.Width * layoutData.Columns) +
                                (GridLayout.ColumnSpacing * (layoutData.Columns - 1)) +
                                GridLayout.GridPadding.HorizontalThickness;

            var computedHeight = headerHeight +
                                 (layoutData.CellSize.Height * layoutData.Rows) +
                                 (GridLayout.RowSpacing * (layoutData.Rows - 1)) +
                                 GridLayout.GridPadding.VerticalThickness;

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
            if (GridLayout._headerView is VisualElement headerElement && headerElement.IsVisible)
            {
                var headerSize = GridLayout._headerView.Measure(bounds.Width, bounds.Height);
                var headerBounds = new Rect(bounds.X, bounds.Y, bounds.Width, headerSize.Height);
                GridLayout._headerView.Arrange(headerBounds);
                headerHeight = headerSize.Height;
            }

            var layoutData = GetLayoutData(bounds.Width, bounds.Height - headerHeight);

            var padding = GridLayout.GridPadding;
            var cellSize = layoutData.CellSize;
            var width = bounds.Width - padding.HorizontalThickness;
            var height = bounds.Height - padding.VerticalThickness - headerHeight;

            if (!GridLayout._shouldInvalidate)
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

            InternalLogger.Debug(Tag, $"{nameof(ArrangeChildren)}(): Number of children => {GridLayout._orderedChildren.Count}");

            foreach (var child in GridLayout._orderedChildren)
            {
                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                if (child != GridLayout._draggingView)
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
                    yChild += GridLayout.RowSpacing + cellSize.Height;
                }
                else
                {
                    xChild += GridLayout.ColumnSpacing + cellSize.Width;
                }
            }

            return new Size(width, height);
        }

        private LayoutData GetLayoutData(double width, double height)
        {
            width -= GridLayout.GridPadding.HorizontalThickness;
            height -= GridLayout.GridPadding.VerticalThickness;

            if (GridLayout._orderedChildren.Count == 0)
            {
                return default;
            }

            var visibleChildCount = 0;
            Size maxChildSize = default;
            LayoutData layoutData = default;

            InternalLogger.Debug(Tag, $"{nameof(GetLayoutData)}(): Number of children => {GridLayout._orderedChildren.Count}");

            foreach (var child in GridLayout._orderedChildren)
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
                    if (GridLayout.ColumnCount > 0)
                    {
                        columns = GridLayout.ColumnCount;
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
                    if (GridLayout.ColumnCount > 0)
                    {
                        columns = GridLayout.ColumnCount;
                    }
                    else
                    {
                        columns = (int)((width + GridLayout.ColumnSpacing) / (maxChildSize.Width + GridLayout.ColumnSpacing));
                        columns = Math.Max(1, columns);
                    }
                    rows = (visibleChildCount + columns - 1) / columns;
                }

                // Now maximize the cell size based on the layout size.
                Size cellSize = default;

                if (IsPositiveInfinity(width) || !GridLayout.AdaptItemWidth)
                {
                    cellSize.Width = maxChildSize.Width;
                }
                else
                {
                    cellSize.Width = (width - (GridLayout.ColumnSpacing * (columns - 1))) / columns;
                }

                if (IsPositiveInfinity(height) || !GridLayout.AdaptItemHeight)
                {
                    cellSize.Height = maxChildSize.Height;
                }
                else
                {
                    cellSize.Height = (height - (GridLayout.RowSpacing * (rows - 1))) / rows;
                }

                layoutData = new LayoutData(visibleChildCount, cellSize, rows, columns);
            }

            GridLayout.ColumnCount = layoutData.Columns;

            return layoutData;
        }
    }
}
