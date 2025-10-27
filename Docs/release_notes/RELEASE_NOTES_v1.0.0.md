# Release Notes v1.0.0

## 🎉 Initial Release - October 2025

Sharpnado.Maui.DragDropGridView is now available on NuGet!

## ✨ Core Features

### Automatic Grid Layout
- **Responsive column calculation** based on available space
- **Full ItemsSource/DataTemplate binding** with INotifyCollectionChanged support
- **Header support** with custom DataTemplate
- **Adaptive item width/height** based on available space
- **Configurable column/row spacing** and grid padding
- **Orientation-aware layout** (portrait/landscape)
- Works seamlessly inside ScrollView with explicit ColumnCount

### Data Binding
- Full support for `ItemsSource` with automatic updates
- `DataTemplate` support for custom item rendering
- Header template support
- MVVM-friendly command binding

## 🎯 Drag & Drop Support

**Available on: iOS, Android, Mac Catalyst**

### Reordering Features
- Built-in drag-and-drop reordering with smooth animations
- Two trigger modes:
  - **Pan** (default): Drag starts immediately on pan gesture
  - **LongPress**: Hold item, then drag after long press (recommended for iOS)
- Automatic ScrollView edge detection and auto-scrolling
- Batched shift animations with configurable duration (default 120ms)
- `OnItemsReorderedCommand` for handling reorder events
- Automatic ItemsSource synchronization for IList collections

### Visual Feedback
- Customizable animations during drag operations
- Smooth item shifting during reorder
- Visual feedback for drag start/stop

## 🎨 Customizable Animations

### Built-in Animation Properties
- `ViewStartDraggingAnimation` - Animation when item starts dragging
- `ViewStopDraggingAnimation` - Animation when item stops dragging
- `DragAndDropEnabledItemsAnimation` - Animation for other items when drag starts
- `DragAndDropDisabledItemsAnimation` - Animation for other items when drag ends

### Predefined Animations
- `ScaleUp` - Simple scale animation
- `ScaleUpBounce` - Scale with bounce effect
- `Wobble` - Wobble animation
- Custom animation functions support

## 🔧 Integration

### Setup
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSharpnadoDragDropGridView(enableLogging: false);
            
        return builder.Build();
    }
}
```

### Features
- Pure .NET MAUI implementation
- Includes MR.Gestures fork for reliable gesture handling
- Full logging support with configurable levels
- MVVM-friendly with command and data binding support
- No platform-specific code required

## 🌐 Platform Support

| Platform | Grid Layout | Drag & Drop |
|----------|------------|-------------|
| iOS 15.0+ | ✅ | ✅ |
| Android API 21+ | ✅ | ✅ |
| Mac Catalyst 15.0+ | ✅ | ✅ |
| Windows 10.0.17763.0+ | ✅ | ⚠️ Not available |

**Note**: Windows platform supports grid layout only. Drag-and-drop is not available due to gesture coordinate system complexities.

## 📦 Installation

```bash
dotnet add package Sharpnado.Maui.DragDropGridView --version 1.0.0
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Sharpnado.Maui.DragDropGridView" Version="1.0.0" />
```

## 🙏 Credits

**Special thanks to Michael Rumpler** for creating and maintaining [MR.Gestures](https://www.mrgestures.com/). This library uses a fork of MR.Gestures to enable reliable cross-platform gesture handling, particularly for LongPress-based drag-and-drop on iOS and Mac Catalyst.

## 📚 Documentation

- **GitHub Repository**: https://github.com/roubachof/Sharpnado.DragDropGridView
- **NuGet Package**: https://www.nuget.org/packages/Sharpnado.Maui.DragDropGridView
- **Full README**: See the repository for detailed usage instructions and examples

## 🐛 Known Issues

- Drag-and-drop is not available on Windows (grid layout works normally)
- Requires explicit `ColumnCount` when used inside ScrollView

## 📝 License

MIT License - see LICENSE file in the repository
