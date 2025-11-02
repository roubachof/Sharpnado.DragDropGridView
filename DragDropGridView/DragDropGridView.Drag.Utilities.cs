using Microsoft.Maui.Devices;
using Sharpnado.Tasks;
using ScrollView = Microsoft.Maui.Controls.ScrollView;

namespace Sharpnado.Maui.DragDropGridView;

public partial class DragDropGridView
{
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
#else
        if (_scrollView != null)
        {
            _scrollView.IsEnabled = !isDisabled;
        }
#endif
    }

    private void HandleScrollViewEdges(ScrollView scrollView, View view)
    {
        // Configuration
        const double EdgeZone = 150.0;
        const double MaxScrollSpeed = 20.0;
        const double MinScrollSpeed = 3.0;

        // Determine scroll orientation and boundaries
        bool isVertical = scrollView.Orientation == ScrollOrientation.Vertical;
        double currentScroll = isVertical ? scrollView.ScrollY : scrollView.ScrollX;
        double scrollViewSize = isVertical ? scrollView.Height : scrollView.Width;
        double contentSize = isVertical ? Height : Width;
        double maxScroll = Math.Max(0, contentSize - scrollViewSize);

        // Get dragged view position relative to scroll viewport (accounting for translation)
        double viewPosition = isVertical 
            ? (view.Y + view.TranslationY) - currentScroll
            : (view.X + view.TranslationX) - currentScroll;
        double viewSize = isVertical ? view.Height : view.Width;
        double viewEnd = viewPosition + viewSize;

        InternalLogger.DebugIf(VerboseLogging, Tag, () => 
            $"HandleScrollViewEdges: orientation={scrollView.Orientation}, viewPosition={viewPosition:F1}, viewEnd={viewEnd:F1}, scrollViewSize={scrollViewSize:F1}, currentScroll={currentScroll:F1}, maxScroll={maxScroll:F1}");

        // Calculate distance from edges
        double distanceFromStart = viewPosition;
        double distanceFromEnd = scrollViewSize - viewEnd;

        // Scroll backwards (up/left) if near start edge
        if (distanceFromStart < EdgeZone && currentScroll > 0)
        {
            double scrollIntensity = 1.0 - Math.Max(0, distanceFromStart) / EdgeZone;
            double scrollSpeed = MinScrollSpeed + (MaxScrollSpeed - MinScrollSpeed) * scrollIntensity;
            double targetScroll = Math.Max(0, currentScroll - scrollSpeed);
            double scrollDelta = currentScroll - targetScroll;

            InternalLogger.DebugIf(VerboseLogging, Tag, () => 
                $"Scrolling BACK: intensity={scrollIntensity:F2}, speed={scrollSpeed:F2}, delta={scrollDelta:F2}");

            // Compensate view translation to keep it under finger
            if (isVertical)
            {
                view.TranslationY += scrollDelta;
                TaskMonitor.Create(scrollView.ScrollToAsync(0, targetScroll, false));
            }
            else
            {
                view.TranslationX += scrollDelta;
                TaskMonitor.Create(scrollView.ScrollToAsync(targetScroll, 0, false));
            }
        }
        // Scroll forwards (down/right) if near end edge
        else if (distanceFromEnd < EdgeZone && currentScroll < maxScroll)
        {
            double scrollIntensity = 1.0 - Math.Max(0, distanceFromEnd) / EdgeZone;
            double scrollSpeed = MinScrollSpeed + (MaxScrollSpeed - MinScrollSpeed) * scrollIntensity;
            double targetScroll = Math.Min(maxScroll, currentScroll + scrollSpeed);
            double scrollDelta = targetScroll - currentScroll;

            InternalLogger.DebugIf(VerboseLogging, Tag, () => 
                $"Scrolling FORWARD: intensity={scrollIntensity:F2}, speed={scrollSpeed:F2}, delta={scrollDelta:F2}");

            // Compensate view translation to keep it under finger
            if (isVertical)
            {
                view.TranslationY -= scrollDelta;
                TaskMonitor.Create(scrollView.ScrollToAsync(0, targetScroll, false));
            }
            else
            {
                view.TranslationX -= scrollDelta;
                TaskMonitor.Create(scrollView.ScrollToAsync(targetScroll, 0, false));
            }
        }
    }
}
