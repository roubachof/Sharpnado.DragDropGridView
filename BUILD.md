# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

**Sharpnado.GridLayout** is a .NET MAUI library providing a high-performance grid layout control with drag-and-drop reordering capabilities. The repository includes both the core library and a sample application demonstrating the Mvvm.Flux architecture pattern.

### Structure

- **GridLayout/** - The core library (NuGet package: `Sharpnado.Maui.GridLayout`)
- **Sample/Mvvm.Flux.Maui/** - Sample application demonstrating Mvvm.Flux architecture and GridLayout usage

## Build Commands

### Building the Library

```bash
# Build the GridLayout library for all platforms
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/GridLayout/GridLayout.csproj

# Build for specific target framework
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/GridLayout/GridLayout.csproj -f net9.0-ios
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/GridLayout/GridLayout.csproj -f net9.0-android
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/GridLayout/GridLayout.csproj -f net9.0-maccatalyst

# Create NuGet package
dotnet pack /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/GridLayout/GridLayout.csproj -c Release
```

### Building the Sample App

```bash
# Build the sample application
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui.sln

# Build for specific platform
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj -f net9.0-ios
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj -f net9.0-android
dotnet build /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj -f net9.0-maccatalyst
```

### Running on Devices

```bash
# iOS Simulator
dotnet build -t:Run -f net9.0-ios /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj

# Android Emulator
dotnet build -t:Run -f net9.0-android /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj

# Mac Catalyst
dotnet build -t:Run -f net9.0-maccatalyst /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/Sample/Mvvm.Flux.Maui/Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj
```

## GridLayout Library Architecture

### Core Components

The GridLayout control is split across multiple partial class files for maintainability:

1. **MauiGridLayout.cs** - Base control with layout properties (ColumnCount, GridPadding, ColumnSpacing, RowSpacing, AdaptItemWidth, AdaptItemHeight)
2. **MauiGridLayout.Manager.cs** - Custom GridLayoutManager handling measure and arrange logic
3. **MauiGridLayout.ItemsSource.cs** - ItemsSource/DataTemplate binding with INotifyCollectionChanged support
4. **MauiGridLayout.Drag.cs** - Drag gesture handling and drag state management
5. **MauiGridLayout.Drop.cs** - Drop zone detection, item reordering, and animations

### Key Features

- **Automatic Layout**: Calculates column count and arranges items in a grid automatically
- **Adaptive Sizing**: Items adapt their width/height based on available space (configurable)
- **ItemsSource Binding**: Supports data binding with `ItemsSource` and `ItemTemplate`
- **Collection Changes**: Automatically updates UI when underlying collection changes (via `INotifyCollectionChanged`)
- **Drag-and-Drop**: Enable via `IsDragAndDropEnabled` property; items must implement `IDragAndDropView`
- **Reorder Events**: Fires `OnItemsReorderedCommand` when items are reordered
- **Custom Animations**: Support for custom drag/drop animations via delegates
- **Orientation Handling**: Responds to device orientation changes and recalculates layout

### Drag-and-Drop Implementation

Views that want to participate in drag-and-drop must implement the `IDragAndDropView` interface:

- `CanReceiveView` - Whether this view can receive dropped items
- `CanBeDropped` - Whether this view can be dropped
- `CanMove` - Whether this view can be moved
- `IsDragAndDropping` - Current drag state
- `OnViewDroppedAsync()` - Called when another view is dropped on this view

The GridLayout uses standard .NET MAUI `PanGestureRecognizer` for drag-and-drop interactions. When `IsDragAndDropEnabled` is true, a pan gesture is automatically added to child views that implement `IDragAndDropView`.

The GridLayout maintains an `orderedChildren` list that tracks the logical order of items, which may differ from visual positions during drag operations.

## Mvvm.Flux Architecture (Sample App)

The sample application demonstrates the **Mvvm.Flux** pattern, a state orchestration approach that combines MVVM with functional programming principles.

### Core Principles

1. **Composition over Inheritance** - Use `TaskLoaderNotifier` components instead of base class patterns
2. **Single Source of Truth** - Domain layer is the authoritative data source
3. **Immutability** - Use C# records for entities (e.g., `Light` record)
4. **One-Way Data Flow** - Updates flow from domain → viewmodel → view via events/messages

### Layer Structure

```
Domain/                      # Business logic and data
├── Lights/
│   ├── Light.cs            # Record entities (immutable)
│   ├── ILightService.cs    # Service contracts
│   └── Mock/               # Mock implementations

Presentation/               # UI layer
├── Pages/                  # ViewModels and Views
│   ├── Home/
│   │   ├── HomeSectionViewModel.cs
│   │   ├── LightEditPageViewModel.cs
│   │   └── LightViewModel.cs
│   └── ...
├── Navigation/             # Navigation services
├── Converters/             # Value converters
├── Behaviors/              # XAML behaviors
└── CustomViews/            # Reusable views

Infrastructure/             # Cross-cutting concerns
├── Helpers/
├── Extensions/
├── Logging/
└── Validation/
```

### TaskLoaderView Integration

The sample heavily uses [Sharpnado.TaskLoaderView](https://github.com/roubachof/Sharpnado.TaskLoaderView) for async state management:

- **TaskLoaderNotifier<T>** - Manages async loading state (NotStarted, Loading, Success, Error, Refreshing)
- **TaskLoaderCommand** - Wraps commands with loading state for UI feedback
- **CompositeTaskLoaderNotifier** - Combines multiple loaders to track aggregate state

### State Update Pattern

1. **Loading Data**:
   ```csharp
   Loader = new TaskLoaderNotifier<ObservableCollection<Light>>();
   Loader.Load(_ => LoadAsync());  // Triggers loading state
   ```

2. **Updating Entities** (using records and `with` syntax):
   ```csharp
   var updatedLight = light with { IsOn = true };
   await _lightService.UpdateLightAsync(updatedLight);
   ```

3. **Domain Events** (single source of truth):
   ```csharp
   // Service raises event after successful update
   _lightService.LightUpdated += OnLightUpdated;
   
   // ViewModel receives update and replaces record in collection
   private void OnLightUpdated(object? sender, Light light)
   {
       int index = itemList.IndexOf(matchingViewModel);
       itemList[index] = light;  // Replace entire record
   }
   ```

### Key Dependencies

- **Prism.DryIoc.Maui** (v9.0.271-pre) - DI container and navigation
- **Sharpnado.Maui.TaskLoaderView** (v2.5.1) - Async state management
- **MetroLog.Maui** (v2.1.0) - Logging infrastructure
- **Mopups** (v1.3.0) - Popup pages
- **Sharpnado.Tabs.Maui** (v3.2.1) - Tab control
- **SkiaSharp.Extended.UI.Maui** (v2.0.0-preview.86) - Lottie animations

### Initialization

The `MauiProgram.cs` initializes:
- MetroLog logging with different targets for Debug/Release and Emulator/Device
- TaskLoaderView with error handling
- Prism DI container registration (Domain services, Navigation)
- Sharpnado.Tabs configuration

## Coding Guidelines

### GridLayout Library

- **Namespace**: Uses `Sharpnado.GridLayout` namespace
- **Logging**: Uses internal `InternalLogger` static class for diagnostic logging (can be enabled via `InternalLogger.EnableLogging` and `InternalLogger.EnableDebug`)
- **Platform Detection**: Check `DeviceInfo.Platform` for platform-specific behavior
- **Invalidation Control**: The `shouldInvalidate` flag controls whether measure invalidation occurs (used during drag operations to prevent layout thrashing)
- **Gestures**: Uses standard .NET MAUI `PanGestureRecognizer` for drag-and-drop (no external gesture library dependencies)
- **No External Dependencies**: The library only depends on Microsoft.Maui.Controls and Sharpnado.Tasks

### Sample Application

- **ViewModels**: Inherit from `ANavigableViewModel` (Prism base) or `BindableBase`
- **Navigation**: Use Prism's `INavigationService` with `NavigationParameters`
- **Logging**: Use MetroLog's `LoggerFactory.GetLogger()`
- **Analytics**: Track screens with `AnalyticsHelper.TrackScreenDisplayed()`
- **Records**: Always use records for domain entities and update via `with` syntax
- **Event Cleanup**: Unsubscribe from domain events in `Destroy()` override

## Platform Support

### GridLayout Library
- .NET 9.0
- iOS 12.2+
- Android API 21+
- Windows 10.0.17763.0+
- MacCatalyst 15.0+

### Sample App
- .NET 9.0 (Android, iOS, MacCatalyst)
- .NET 8.0 (Windows) - Note the different framework for Windows
- iOS 15.0+
- Android API 21+
- MacCatalyst 15.0+
- Windows 10.0.17763.0+

## StyleCop Configuration

The sample application uses StyleCop.Analyzers (v1.2.0-beta.556) with `EnforceCodeStyleInBuild` enabled. Follow StyleCop rules when making code changes to the sample.
