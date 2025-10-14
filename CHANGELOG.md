# Changelog

All notable changes to the Sharpnado.Maui.GridLayout project will be documented in this file.

## [Unreleased]

### Added
- **MAUI Extension Builder**: Added `UseSharpnadoGridLayout()` extension method for `MauiAppBuilder`
  - Enables easy configuration of the GridLayout library in `MauiProgram.cs`
  - Supports logging configuration with `enableLogging`, `enableDebugLogging`, `loggerDelegate`, and `logFilter` parameters
  - Similar pattern to `UseSharpnadoShadows()` from Sharpnado.Shadows library
  - See `MauiAppBuilderExtensions.cs` for implementation details

- **Header Support**: Added `HeaderTemplate` and `Header` properties to GridLayout
  - Allows displaying a custom header at the top of the grid
  - Uses `DataTemplate` for flexible header customization
  - Header is automatically laid out above grid items
  - See sample app `HomeSectionView.xaml` for usage example

- **Internal Logging**: Added `InternalLogger` class for diagnostic logging
  - Configurable logging levels (Info, Debug, Error)
  - Optional custom logger delegate
  - Tag-based filtering with pipe-separated syntax
  - Default console output for development scenarios

- **Comprehensive Documentation**: Added README.md with:
  - Getting started guide
  - Usage examples with XAML code
  - Advanced scenarios (headers, drag-and-drop)
  - Property reference table
  - Architecture overview
  - Platform support information

### Changed
- **Sample App Updated**: Modified `HomeSectionView.xaml` to use GridLayout instead of CollectionView
  - Demonstrates 2-column grid layout
  - Shows header implementation with title and image
  - Includes light bulb items with tap gesture handling
  - Integrated with TaskLoaderView for async data loading

- **Sample App Configuration**: Updated `MauiProgram.cs` to call `UseSharpnadoGridLayout()`
  - Shows proper initialization pattern
  - Demonstrates logging configuration options

### Fixed
- **ScrollView Compatibility**: Fixed GridLayout to respect `ColumnCount` property when inside a ScrollView
  - Previously, when GridLayout was in a ScrollView with infinite width, it would ignore `ColumnCount` and show all items in one row
  - Now, when `ColumnCount` is explicitly set, it's always respected regardless of available width
  - Also respects `ColumnCount` when width is constrained but `ColumnCount` is set
  - Enables proper grid layout in scrollable containers

- **Visibility Check**: Fixed `Visibility` property access on header view
  - Changed to use `VisualElement.IsVisible` property
  - Safe casting from `View` to `VisualElement`
  - Prevents runtime errors on non-visual elements

### Technical Details
- Framework: .NET 9.0
- Target Platforms: iOS 15.0+, Android 21+, Mac Catalyst 15.0+, Windows 10.0.17763.0+
- Build Status: ✅ All platforms compile successfully with 0 errors
- Dependencies: Microsoft.Maui.Controls 9.0+

### Developer Notes
- Follow the pattern established by Sharpnado.Shadows for consistency
- Extension method pattern simplifies library initialization
- Logging configuration is optional but recommended for debugging
- Header support enables richer UI layouts without custom controls
