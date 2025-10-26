<div align="center">
  <img src="https://raw.githubusercontent.com/roubachof/Sharpnado.GridLayout/main/Docs/gridlayout.png" alt="DragDropGridView Logo" width="150"/>
  
  # Sharpnado.Maui.DragDropGridView
  
  A high-performance drag-and-drop grid layout control for .NET MAUI with adaptive sizing, configurable gesture triggers, and flexible item management.
</div>

## ✨ Features

- 🎯 **Drag & Drop**: Built-in support for reordering items with smooth animations
- 📐 **Flexible Layout**: Configurable columns, spacing, and adaptive sizing
- 🎨 **Customizable Animations**: Predefined or custom animations for drag operations
- 🔄 **Data Binding**: Full ItemsSource/DataTemplate support with INotifyCollectionChanged
- 📱 **Cross-Platform**: iOS, Android, Mac Catalyst (grid layout on Windows)

## 🚀 Quick Start

### 1. Installation

```bash
dotnet add package Sharpnado.Maui.DragDropGridView
```

### 2. Initialize in MauiProgram.cs

```csharp
using Sharpnado.Maui.DragDropGridView;

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

### 3. Add to XAML

```xml
xmlns:gridLayout="clr-namespace:Sharpnado.Maui.DragDropGridView;assembly=Sharpnado.Maui.DragDropGridView"

<ScrollView>
    <gridLayout:DragDropGridView
        ColumnCount="2"
        ColumnSpacing="10"
        RowSpacing="10"
        IsDragAndDropEnabled="True"
        DragAndDropTrigger="LongPress"
        ItemsSource="{Binding Items}">
        
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
</ScrollView>
```

### 4. Configure Animations (Optional)

```csharp
using Sharpnado.Maui.DragDropGridView;

// In your view or code-behind
gridView.DragAndDropItemsAnimation = DragDropAnimations.Items.WobbleAsync;
gridView.DragAndDropEndItemsAnimation = DragDropAnimations.EndItems.StopWobbleAsync;
gridView.LongPressedDraggingAnimation = DragDropAnimations.Dragging.ScaleUpBounceAsync;
gridView.LongPressedDroppingAnimation = DragDropAnimations.Dropping.ScaleToBounceAsync;
```

## 📖 Documentation

Full documentation available at: [GitHub Repository](https://github.com/roubachof/Sharpnado.GridLayout)

## 🎯 Key Properties

| Property | Description |
|----------|-------------|
| `ItemsSource` | Collection of items to display |
| `ItemTemplate` | Template for rendering each item |
| `ColumnCount` | Number of columns in the grid |
| `IsDragAndDropEnabled` | Enable/disable drag-and-drop |
| `DragAndDropTrigger` | `Pan` or `LongPress` gesture trigger |
| `OnItemsReorderedCommand` | Command executed when items are reordered |

## 🎨 Predefined Animations

**DragDropAnimations.Dragging**
- `ScaleUpAsync`, `ScaleUpLargeAsync`, `ScaleUpBounceAsync`

**DragDropAnimations.Dropping**
- `ScaleToNormalAsync`, `ScaleToBounceAsync`

**DragDropAnimations.Items**
- `WobbleAsync`, `StopWobbleAsync`

## 🌍 Platform Support

- ✅ iOS 15.0+ (full drag-and-drop)
- ✅ Android API 21+ (full drag-and-drop)
- ✅ Mac Catalyst 15.0+ (full drag-and-drop)
- ⚠️ Windows 10.0.17763.0+ (grid layout only)

## 📝 License

MIT License - see [LICENSE](https://github.com/roubachof/Sharpnado.GridLayout/blob/main/LICENSE) for details

## 👨‍💻 Author

Developed by Jean-Marie Alfonsi (Sharpnado)

## 🐛 Issues & Feedback

Report issues or request features at: [GitHub Issues](https://github.com/roubachof/Sharpnado.GridLayout/issues)
