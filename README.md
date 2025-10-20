# Sharpnado.Maui.DragDropGridView

A high-performance drag-and-drop grid layout control for .NET MAUI with adaptive sizing, configurable gesture triggers, and flexible item management.

## Features

- ✨ **High Performance**: Optimized for smooth scrolling and rendering
- 🎯 **Drag & Drop**: Built-in drag-and-drop support for reordering items
- 📐 **Flexible Layout**: Configurable column count, spacing, and item sizing
- 🎨 **Header Support**: Optional header with custom template
- 🔄 **Data Binding**: Full ItemsSource and DataTemplate support
- 📱 **Cross-Platform**: Works on iOS, Android, Mac Catalyst, and Windows

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

The DragDropGridView supports drag-and-drop reordering with two trigger modes:

- Pan (default): drag starts as soon as the user pans the item.
- LongPress: hold the item, then drag after the long press begins. Recommended on iOS when you want to avoid accidental drags.

Important requirements:
- Draggable items must be MR.Gestures-based controls that implement `IGestureAwareControl`. The easiest is to wrap your template content inside `<gridLayout:DragAndDropView>...</gridLayout:DragAndDropView>` which already inherits from `MR.Gestures.ContentView`.
- The ItemsSource will be automatically updated on reorder if it implements `IList`.

MR.Gestures integration:
- This package includes a fork of MR.Gestures to enable reliable LongPress-based drag-and-drop on iOS/Mac Catalyst. No extra setup is required beyond calling `.UseSharpnadoDragDropGridView(...)`, which configures MR.Gestures for you.

### Using DragDropGridView in a ScrollView

The DragDropGridView works correctly inside a ScrollView and includes automatic edge scrolling during drag operations. When `ColumnCount` is explicitly set, it will be respected even when the ScrollView provides infinite width:

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

## Platform Support

- ✅ iOS 15.0+
- ✅ Android 21+ (API Level 21)
- ✅ Mac Catalyst 15.0+
- ✅ Windows 10.0.17763.0+

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
