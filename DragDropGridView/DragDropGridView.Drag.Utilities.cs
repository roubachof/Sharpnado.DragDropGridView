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
        const double EdgeZone = 150.0;  // Larger edge detection zone for earlier response
        const double MaxScrollSpeed = 20.0;  // Maximum pixels per frame
        const double MinScrollSpeed = 3.0;   // Minimum scroll speed to avoid stalling

        var viewScreenCoordinates = view.GetScreenCoordinates<Application>();
        var scrollScreenCoordinates = scrollView.GetScreenCoordinates<Application>();
        var containerScreenCoordinates = this.GetScreenCoordinates<Application>();

        double toolbarHeight = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape ? 48 : 56;
        double screenHeight = (DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density) - toolbarHeight;

        InternalLogger.DebugIf(VerboseLogging, Tag, () => $"screenHeight: {screenHeight}, viewScreenCoordinates: {viewScreenCoordinates}, scrollScreenCoordinates: {scrollScreenCoordinates}, containerScreenCoordinates: {containerScreenCoordinates}");

        // Calculate scroll viewport boundaries
        double scrollViewportTop = scrollView.ScrollY + scrollScreenCoordinates.Y;
        double scrollViewportBottom = scrollView.ScrollY + scrollScreenCoordinates.Y + screenHeight;

        // Calculate dragged view boundaries
        double viewTop = view.TranslationY + viewScreenCoordinates.Y;
        double viewBottom = viewTop + view.Height;

        // Calculate container boundaries
        double containerBottom = containerScreenCoordinates.Y + Height;

        // Calculate distance from edges (negative = inside edge zone)
        double distanceFromTop = viewTop - scrollViewportTop;
        double distanceFromBottom = scrollViewportBottom - viewBottom;

        InternalLogger.DebugIf(VerboseLogging, Tag, () => $"distanceFromTop: {distanceFromTop}, distanceFromBottom: {distanceFromBottom}");

        // Check if we need to scroll up (near top edge)
        if (distanceFromTop < EdgeZone && scrollView.ScrollY > 0)
        {
            // Calculate scroll speed based on distance (closer = faster, with easing)
            double normalizedDistance = Math.Max(0, distanceFromTop) / EdgeZone; // 0 = at edge, 1 = far from edge
            double easedDistance = 1.0 - Math.Pow(normalizedDistance, 2); // Quadratic easing
            double scrollSpeed = MinScrollSpeed + (MaxScrollSpeed - MinScrollSpeed) * easedDistance;

            double targetY = Math.Max(0, scrollView.ScrollY - scrollSpeed);
            double actualScrollDelta = scrollView.ScrollY - targetY;

            InternalLogger.DebugIf(VerboseLogging, Tag, () => $"Scrolling UP: speed={scrollSpeed:F2}, targetY={targetY:F2}, delta={actualScrollDelta:F2}");

            // Compensate view position for scroll movement to keep it under the finger
            view.TranslationY += actualScrollDelta;

            TaskMonitor.Create(scrollView.ScrollToAsync(0, targetY, false));
        }
        // Check if we need to scroll down (near bottom edge)
        else if (distanceFromBottom < EdgeZone && containerBottom > scrollViewportBottom)
        {
            // Calculate scroll speed based on distance (closer = faster, with easing)
            double normalizedDistance = Math.Max(0, distanceFromBottom) / EdgeZone;
            double easedDistance = 1.0 - Math.Pow(normalizedDistance, 2); // Quadratic easing
            double scrollSpeed = MinScrollSpeed + (MaxScrollSpeed - MinScrollSpeed) * easedDistance;

            // Calculate max scroll position: content height minus visible viewport height
            // This ensures we can scroll enough to show the full last row
            double scrollViewHeight = scrollView.Height;
            double contentHeight = Height; // GridLayout's full content height
            double maxScrollY = Math.Max(0, contentHeight - scrollViewHeight);
            
            double targetY = Math.Min(maxScrollY, scrollView.ScrollY + scrollSpeed);
            double actualScrollDelta = targetY - scrollView.ScrollY;

            InternalLogger.DebugIf(VerboseLogging, Tag, () => $"Scrolling DOWN: speed={scrollSpeed:F2}, targetY={targetY:F2}, maxScrollY={maxScrollY:F2}, delta={actualScrollDelta:F2}");

            // Compensate view position for scroll movement to keep it under the finger
            view.TranslationY -= actualScrollDelta;

            TaskMonitor.Create(scrollView.ScrollToAsync(0, targetY, false));
        }
    }
}
