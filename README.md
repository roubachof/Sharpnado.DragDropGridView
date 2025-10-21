# Sharpnado.Maui.DragDropGridView

A high-performance drag-and-drop grid layout control for .NET MAUI with adaptive sizing, configurable gesture triggers, and flexible item management.

## Features

- ✨ **High Performance**: Optimized for smooth scrolling and rendering
- 🎯 **Drag & Drop**: Built-in drag-and-drop support for reordering items (iOS, Android, Mac Catalyst)
- 📐 **Flexible Layout**: Configurable column count, spacing, and item sizing
- 🎨 **Header Support**: Optional header with custom template
- 🔄 **Data Binding**: Full ItemsSource and DataTemplate support
- 📱 **Cross-Platform**: Works on iOS, Android, Mac Catalyst, and Windows (drag-and-drop available on iOS, Android, Mac Catalyst only)

## Installation

```xml
<PackageReference Include="Sharpnado.Maui.DragDropGridView" Version="x.x.x" />
```

## Getting Started

### 1. Initialize in MauiProgram.cs

Add the DragDropGridView initialization to your `MauiProgram.cs`:

```csharp
using Sharpnado.GridLayout;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSharpnadoDragDropGridView(enableLogging: false);
            // ... other configurations
            
        return builder.Build();
    }
}
```

#### Configuration Options

The `UseSharpnadoDragDropGridView` extension method accepts the following parameters:

- `enableLogging` (default: `false`): Enable or disable logging
- `enableDebugLogging` (default: `false`): Enable debug-level logging
- `loggerDelegate`: Custom logger implementation (optional)
- `logFilter`: Filter logs by tags separated by `|` (e.g., `"DragDropGridView|Drag|Drop"`)

Example with logging enabled:

```csharp
.UseSharpnadoDragDropGridView(
    enableLogging: true,
    enableDebugLogging: true,
    logFilter: "DragDropGridView|Drag")
```

### 2. Add Namespace to XAML

```xml
xmlns:gridLayout="clr-namespace:Sharpnado.GridLayout;assembly=Sharpnado.Maui.DragDropGridView"
```

### 3. Use the DragDropGridView

```xml
<gridLayout:DragDropGridView
    ColumnCount="2"
    ColumnSpacing="10"
    RowSpacing="10"
    IsDragAndDropEnabled="True"
    DragAndDropTrigger="Pan"
    ItemsSource="{Binding Items}"
    OnItemsReorderedCommand="{Binding ItemsReorderedCommand}">
    
    <gridLayout:DragDropGridView.ItemTemplate>
        <DataTemplate>
            <gridLayout:DragAndDropView>
                <Border Padding="10" Background="LightGray">
                    <Label Text="{Binding Name}" />
                </Border>
            </gridLayout:DragAndDropView>
        </DataTemplate>
    </gridLayout:DragDropGridView.ItemTemplate>
</gridLayout:DragDropGridView>
```

## Advanced Usage

### With Header

```xml
<gridLayout:DragDropGridView
    ColumnCount="2"
    ColumnSpacing="10"
    RowSpacing="10"
    IsDragAndDropEnabled="True"
    ItemsSource="{Binding Items}">
    
    <gridLayout:DragDropGridView.HeaderTemplate>
        <DataTemplate>
            <StackLayout Padding="15">
                <Label Text="My Items" FontSize="24" FontAttributes="Bold" />
                <Label Text="Drag to reorder" FontSize="12" TextColor="Gray" />
            </StackLayout>
        </DataTemplate>
    </gridLayout:DragDropGridView.HeaderTemplate>
    
    <gridLayout:DragDropGridView.ItemTemplate>
        <DataTemplate>
            <gridLayout:DragAndDropView>
                <Border Padding="10" Background="LightGray">
                    <Label Text="{Binding Name}" />
                </Border>
            </gridLayout:DragAndDropView>
        </DataTemplate>
    </gridLayout:DragDropGridView.ItemTemplate>
</gridLayout:DragDropGridView>
```

### Drag and Drop

**Platform Availability**: Drag-and-drop functionality is currently available on iOS, Android, and Mac Catalyst. It is **not available on Windows** due to gesture coordinate system complexities.

The DragDropGridView supports drag-and-drop reordering with two trigger modes:

- Pan (default): drag starts as soon as the user pans the item.
- LongPress: hold the item, then drag after the long press begins. Recommended on iOS when you want to avoid accidental drags.

Important requirements:
- Draggable items must be MR.Gestures-based controls that implement `IGestureAwareControl`. The easiest is to wrap your template content inside `<gridLayout:DragAndDropView>...</gridLayout:DragAndDropView>` which already inherits from `MR.Gestures.ContentView`.
- The ItemsSource will be automatically updated on reorder if it implements `IList`.
- On Windows, the grid will function as a read-only layout without drag-and-drop capabilities.

MR.Gestures integration:
- This package includes a fork of MR.Gestures to enable reliable LongPress-based drag-and-drop on iOS/Mac Catalyst. No extra setup is required beyond calling `.UseSharpnadoDragDropGridView(...)`, which configures MR.Gestures for you.

### Using DragDropGridView in a ScrollView

✨ **The DragDropGridView is designed to work seamlessly inside a ScrollView with intelligent gesture handling:**

- **Automatic Scrolling**: When dragging items near the edges, the ScrollView will automatically scroll to reveal more content
- **Gesture Coordination**: The control automatically manages gesture conflicts between dragging and scrolling
- **Edge Detection**: Smart edge detection triggers scrolling during drag operations for smooth user experience
- **Layout Respect**: When `ColumnCount` is explicitly set, it will be respected even when the ScrollView provides infinite width

```xml
<ScrollView>
    <gridLayout:DragDropGridView
        ColumnCount="2"
        ColumnSpacing="10"
        RowSpacing="10"
        IsDragAndDropEnabled="True"
        ItemsSource="{Binding Items}">
        <!-- Templates -->
    </gridLayout:DragDropGridView>
</ScrollView>
```

**Important**: Always set an explicit `ColumnCount` when using DragDropGridView inside a ScrollView. Without it, the layout cannot determine how many columns to display.

**Note**: ScrollView integration and automatic gesture handling is available on all supported drag-and-drop platforms (iOS, Android, Mac Catalyst).

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ItemsSource` | `IEnumerable` | The collection of items to display |
| `ItemTemplate` | `DataTemplate` | Template for rendering each item |
| `HeaderTemplate` | `DataTemplate` | Optional template for the header |
| `ColumnCount` | `int` | Number of columns in the grid |
| `ColumnSpacing` | `double` | Spacing between columns |
| `RowSpacing` | `double` | Spacing between rows |
| `GridPadding` | `Thickness` | Padding around the entire grid |
| `IsDragAndDropEnabled` | `bool` | Enable or disable drag-and-drop functionality |
| `DragAndDropTrigger` | `DragAndDropTrigger` | Gesture trigger: `Pan` or `LongPress` |
| `OnItemsReorderedCommand` | `ICommand` | Command executed when items are reordered |
| `AdaptItemWidth` | `bool` | Whether items should adapt their width (default: true) |
| `AdaptItemHeight` | `bool` | Whether items should adapt their height (default: false) |
| `AnimateTransitions` | `bool` | Enable layout change animations (default: true) |

## Sample Application

Check out the sample application in the `Sample/Mvvm.Flux.Maui` directory for a complete working example demonstrating:

- Grid layout with 2 columns
- Custom item templates with data binding
- Header with title and image
- Drag-and-drop reordering
- Integration with MVVM pattern

## Architecture

The DragDropGridView is built using the Mvvm.Flux architecture pattern, which emphasizes:

- **Composition over inheritance**: Small, focused components
- **Immutability**: Using C# records for data models
- **Single source of truth**: Centralized state management
- **One-way data flow**: Clear data flow from services to views

For more information on the architecture, see the [Mvvm.Flux README](Sample/Mvvm.Flux.Maui/README.md).

## Limitations

- **Windows**: Drag-and-drop is not supported on Windows due to gesture coordinate system complexities. The grid functions as a read-only layout.
- **Android**: ZIndex changes during drag operations cancel gestures, which means dragged items may appear behind other items instead of on top.
- **ItemsSource Requirements**: For automatic reordering to work, the ItemsSource must implement `IList`. Collections that are read-only or implement only `IEnumerable` will not be automatically updated during drag-and-drop operations.
- **Gesture Dependencies**: Draggable items must be wrapped in MR.Gestures-compatible controls (such as `DragAndDropView`) that implement `IGestureAwareControl`. Standard MAUI controls without gesture support cannot be dragged.

## Platform Support

- ✅ iOS 15.0+ (full drag-and-drop support)
- ✅ Android 21+ (full drag-and-drop support)
- ✅ Mac Catalyst 15.0+ (full drag-and-drop support)
- ⚠️ Windows 10.0.17763.0+ (grid layout only, no drag-and-drop)

## Dependencies

- Microsoft.Maui.Controls 9.0+
- .NET 9.0

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.

## Credits

Developed by Jean-Marie Alfonsi (Sharpnado)

## Support

For issues, questions, or feature requests, please open an issue on GitHub.
