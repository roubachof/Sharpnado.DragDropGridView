# Changelog

All notable changes to Sharpnado.Maui.DragDropGridView will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-23

### Initial Release

A high-performance drag-and-drop grid layout control for .NET MAUI with adaptive sizing, configurable gesture triggers, and flexible item management.

#### Features

**Core Grid Layout**
- Automatic grid layout with responsive column calculation
- Configurable column count, spacing, and padding
- Adaptive item width/height based on available space
- Header support with custom DataTemplate
- Full ItemsSource and DataTemplate binding with INotifyCollectionChanged support
- Orientation-aware layout (portrait/landscape)
- Works seamlessly inside ScrollView with explicit ColumnCount

**Drag & Drop** (iOS, Android, Mac Catalyst)
- Built-in drag-and-drop support for reordering items
- Two trigger modes: Pan (immediate) and LongPress (recommended for iOS)
- Automatic ScrollView edge detection and auto-scrolling during drag operations
- Smooth batched shift animations with configurable duration (default 120ms)
- OnItemsReorderedCommand for handling reorder events
- Automatic ItemsSource synchronization for IList collections

**Customizable Animations**
- `ViewStartDraggingAnimation` - Animation when item starts being dragged
- `ViewStopDraggingAnimation` - Animation when item stops being dragged
- `DragAndDropEnabledItemsAnimation` - Continuous animation when D&D is enabled
- `DragAndDropDisabledItemsAnimation` - Cleanup animation when D&D is disabled
- Predefined animations via `DragDropAnimations` static class:
  - Start/Stop: ScaleUp, ScaleUpLarge, ScaleUpBounce, ScaleToBounce
  - Enabled/Disabled: Wobble, StopWobble
- Custom animation functions support

**Integration**
- Pure .NET MAUI implementation with no platform-specific code required
- Includes fork of MR.Gestures for reliable gesture handling
- Full logging support with configurable levels and filtering
- MVVM-friendly with command and data binding support

#### Platform Support

- ✅ iOS 15.0+ (full drag-and-drop support)
- ✅ Android API 21+ (full drag-and-drop support)
- ✅ Mac Catalyst 15.0+ (full drag-and-drop support)
- ⚠️ Windows 10.0.17763.0+ (grid layout only, no drag-and-drop)

#### Known Limitations

- **Windows**: Drag-and-drop not supported due to gesture coordinate system complexities
- **Android**: Dragged items may appear behind other items (ZIndex changes cancel gestures)
- **ItemsSource**: Must implement IList for automatic reordering
- **Draggable items**: Must be wrapped in MR.Gestures-compatible controls (e.g., DragAndDropView)

#### Dependencies

- Microsoft.Maui.Controls 9.0.110+
- .NET 9.0
- Sharpnado.TaskMonitor 1.1.0
- MR.Gestures (included)

#### Installation

```bash
dotnet add package Sharpnado.Maui.DragDropGridView
```

Initialize in MauiProgram.cs:
```csharp
.UseSharpnadoDragDropGridView(enableLogging: false)
```

[1.0.0]: https://github.com/roubachof/Sharpnado.GridLayout/releases/tag/v1.0.0
