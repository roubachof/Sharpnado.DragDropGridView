# Sharpnado.Maui.GridLayout

A high-performance grid layout control for .NET MAUI with drag-and-drop reordering, adaptive sizing, and flexible item management.

## Features

- ✨ **High Performance**: Optimized for smooth scrolling and rendering
- 🎯 **Drag & Drop**: Built-in drag-and-drop support for reordering items
- 📐 **Flexible Layout**: Configurable column count, spacing, and item sizing
- 🎨 **Header Support**: Optional header with custom template
- 🔄 **Data Binding**: Full ItemsSource and DataTemplate support
- 📱 **Cross-Platform**: Works on iOS, Android, Mac Catalyst, and Windows

## Installation

```xml
<PackageReference Include="Sharpnado.Maui.GridLayout" Version="x.x.x" />
```

## Getting Started

### 1. Initialize in MauiProgram.cs

Add the GridLayout initialization to your `MauiProgram.cs`:

```csharp
using Sharpnado.GridLayout;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSharpnadoGridLayout(enableLogging: false);
            // ... other configurations
            
        return builder.Build();
    }
}
```

#### Configuration Options

The `UseSharpnadoGridLayout` extension method accepts the following parameters:

- `enableLogging` (default: `false`): Enable or disable logging
- `enableDebugLogging` (default: `false`): Enable debug-level logging
- `loggerDelegate`: Custom logger implementation (optional)
- `logFilter`: Filter logs by tags separated by `|` (e.g., `"GridLayout|Drag|Drop"`)

Example with logging enabled:

```csharp
.UseSharpnadoGridLayout(
    enableLogging: true,
    enableDebugLogging: true,
    logFilter: "GridLayout|Drag")
```

### 2. Add Namespace to XAML

```xml
xmlns:gridLayout="clr-namespace:Sharpnado.GridLayout;assembly=Sharpnado.Maui.GridLayout"
```

### 3. Use the GridLayout

```xml
<gridLayout:GridLayout
    ColumnCount="2"
    ColumnSpacing="10"
    RowSpacing="10"
    ItemsSource="{Binding Items}">
    
    <gridLayout:GridLayout.ItemTemplate>
        <DataTemplate>
            <Border Padding="10" Background="LightGray">
                <Label Text="{Binding Name}" />
            </Border>
        </DataTemplate>
    </gridLayout:GridLayout.ItemTemplate>
</gridLayout:GridLayout>
```

## Advanced Usage

### With Header

```xml
<gridLayout:GridLayout
    ColumnCount="2"
    ColumnSpacing="10"
    RowSpacing="10"
    ItemsSource="{Binding Items}">
    
    <gridLayout:GridLayout.HeaderTemplate>
        <DataTemplate>
            <StackLayout Padding="15">
                <Label Text="My Items" FontSize="24" FontAttributes="Bold" />
                <Label Text="Drag to reorder" FontSize="12" TextColor="Gray" />
            </StackLayout>
        </DataTemplate>
    </gridLayout:GridLayout.HeaderTemplate>
    
    <gridLayout:GridLayout.ItemTemplate>
        <DataTemplate>
            <Border Padding="10" Background="LightGray">
                <Label Text="{Binding Name}" />
            </Border>
        </DataTemplate>
    </gridLayout:GridLayout.ItemTemplate>
</gridLayout:GridLayout>
```

### Drag and Drop

The GridLayout supports drag-and-drop reordering out of the box. Items can be dragged to new positions, and the ItemsSource will be automatically updated if it implements `IList`.

### Using GridLayout in a ScrollView

The GridLayout works correctly inside a ScrollView. When `ColumnCount` is explicitly set, it will be respected even when the ScrollView provides infinite width:

```xml
<ScrollView>
    <gridLayout:GridLayout
        ColumnCount="2"
        ColumnSpacing="10"
        RowSpacing="10"
        ItemsSource="{Binding Items}">
        <!-- Templates -->
    </gridLayout:GridLayout>
</ScrollView>
```

**Important**: Always set an explicit `ColumnCount` when using GridLayout inside a ScrollView. Without it, the layout cannot determine how many columns to display.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `ItemsSource` | `IEnumerable` | The collection of items to display |
| `ItemTemplate` | `DataTemplate` | Template for rendering each item |
| `HeaderTemplate` | `DataTemplate` | Optional template for the header |
| `ColumnCount` | `int` | Number of columns in the grid |
| `ColumnSpacing` | `double` | Spacing between columns |
| `RowSpacing` | `double` | Spacing between rows |
| `ItemHeight` | `double` | Fixed height for all items (optional) |
| `ItemWidth` | `double` | Fixed width for all items (optional) |

## Sample Application

Check out the sample application in the `Sample/Mvvm.Flux.Maui` directory for a complete working example demonstrating:

- Grid layout with 2 columns
- Custom item templates with data binding
- Header with title and image
- Drag-and-drop reordering
- Integration with MVVM pattern

## Architecture

The GridLayout is built using the Mvvm.Flux architecture pattern, which emphasizes:

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
