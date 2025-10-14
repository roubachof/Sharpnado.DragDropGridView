# Migration Notes - Gesture System Refactoring

## Summary

This document describes the changes made to replace MR.Gestures with .NET MAUI's built-in gesture recognizers and replace external logging dependencies with an internal logger.

## Changes Made

### 1. Removed External Dependencies

**Removed:**
- `MR.Gestures` - Third-party gesture library
- `MemoryToolkit.Maui` - Memory management utilities
- Custom `ILogger` interfaces from LakeShore.Log

**Result:** The library now only depends on:
- `Microsoft.Maui.Controls` (v9.0.110)
- `Sharpnado.Tasks` (for TaskMonitor)

### 2. Logging System

**Before:**
```csharp
private readonly ILogger logger;

logger = ContainerLocator.Container.Resolve<ILogger>().AsXip(this, isEnable: true);
logger.LogDebug(() => "Some message");
```

**After:**
```csharp
private const string Tag = nameof(GridLayout);

InternalLogger.Debug(Tag, "Some message");
```

**InternalLogger Features:**
- Static class with no instantiation needed
- Enable/disable via `InternalLogger.EnableLogging` and `InternalLogger.EnableDebug`
- Custom delegate support via `InternalLogger.LoggerDelegate`
- Filtering by tag with `InternalLogger.SetFilter("GridLayout|GridLayoutManager")`
- Automatic timestamped output to Debug/Console

### 3. Gesture System

**Before (MR.Gestures):**
```csharp
// Required IGestureAwareControl interface
if (child is not (IGestureAwareControl gestureAwareControl and IDragAndDropView))
{
    return;
}

// Platform-specific handling
if (DeviceInfo.Platform == DevicePlatform.Android)
{
    gestureAwareControl.LongPressed += OnLongPressed;
    gestureAwareControl.LongPressing += OnLongPressing;
}
else
{
    gestureAwareControl.Panning += OnPanning;
    gestureAwareControl.Panned += OnPanned;
}
```

**After (.NET MAUI Gestures):**
```csharp
// Only requires View + IDragAndDropView
if (child is not (View view and IDragAndDropView))
{
    return;
}

// Standard PanGestureRecognizer - works on all platforms
var panGesture = new PanGestureRecognizer();
panGesture.PanUpdated += OnPanUpdated;
view.GestureRecognizers.Add(panGesture);
```

**Gesture Event Handling:**
```csharp
private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
{
    switch (e.StatusType)
    {
        case GestureStatus.Started:
            // Initialize drag session
            break;
            
        case GestureStatus.Running:
            // Update view position during drag
            // Calculate incremental movement
            double deltaX = e.TotalX - lastPanX;
            double deltaY = e.TotalY - lastPanY;
            view.TranslationX += deltaX;
            view.TranslationY += deltaY;
            break;
            
        case GestureStatus.Completed:
        case GestureStatus.Canceled:
            // Finalize drag operation
            break;
    }
}
```

### 4. IDragAndDropView Interface

**No changes required** - The interface remains the same:

```csharp
public interface IDragAndDropView
{
    bool CanReceiveView { get; }
    bool CanBeDropped { get; }
    bool CanMove { get; }
    bool IsDragAndDropping { get; set; }
    Task OnViewDroppedAsync(IDragAndDropView droppedView);
}
```

However, views no longer need to implement `IGestureAwareControl` - just being a `View` that implements `IDragAndDropView` is sufficient.

### 5. Namespace Updates

All files updated from:
- `LakeShore.Triton.Instrument.Controls.Sharpnado`

To:
- `Sharpnado.GridLayout`

### 6. Helper Method Updates

**GetScreenCoordinates:**
Added non-generic overload that walks up to Page/Window level:

```csharp
// Before (required type parameter)
var coords = view.GetScreenCoordinates<App>();

// After (no type parameter needed)
var coords = view.GetScreenCoordinates();
```

## Breaking Changes for Library Users

### For Views Implementing Drag-and-Drop:

**Before:**
```csharp
public class MyDraggableView : ContentView, IGestureAwareControl, IDragAndDropView
{
    // Had to implement IGestureAwareControl events
}
```

**After:**
```csharp
public class MyDraggableView : ContentView, IDragAndDropView
{
    // Only implement IDragAndDropView interface
}
```

### Namespace Changes:

Update your using statements:
```csharp
// Before
using LakeShore.Triton.Instrument.Controls.Sharpnado;

// After
using Sharpnado.GridLayout;
```

## Migration Guide for Existing Code

1. **Remove MR.Gestures package reference** from your project
2. **Remove MemoryToolkit.Maui package reference** if only used for GridLayout
3. **Update namespace imports** to `Sharpnado.GridLayout`
4. **Update draggable views** to remove `IGestureAwareControl` implementation
5. **Remove event handlers** for LongPressed, LongPressing, Panning, Panned (no longer needed)
6. **Rebuild and test** drag-and-drop functionality

## Benefits

1. ✅ **Fewer Dependencies** - Reduced from 3 external packages to 1
2. ✅ **Better Performance** - Native MAUI gestures with no wrapper overhead
3. ✅ **Simpler API** - No platform-specific code needed
4. ✅ **Better Maintainability** - Less code to maintain
5. ✅ **Future-Proof** - Uses standard .NET MAUI APIs
6. ✅ **Self-Contained Logging** - No external logging framework required

## Testing

The library has been successfully built for:
- ✅ net9.0
- ✅ net9.0-ios
- ✅ net9.0-android
- ✅ net9.0-maccatalyst

All builds succeeded with only nullable reference warnings (not errors).

## Notes

- Pan gesture tracking uses incremental delta calculations to properly handle continuous movement
- The gesture system now works consistently across all platforms without special cases
- InternalLogger can be customized by setting a custom `LoggerDelegate` if needed
- The library maintains full backward compatibility for the `IDragAndDropView` interface
