# Release Notes v1.0.1

## 🐛 Bug Fixes - November 2025

This is a maintenance release focusing on bug fixes and improvements.

## 🔧 Bug Fixes

### Auto-Scrolling Fix
- **Fixed**: Auto-scrolling behavior during drag operations
- **Impact**: Improved reliability when dragging items near ScrollView edges
- **Details**: Refactored drag utilities to handle edge detection and scrolling more accurately
- **Commit**: `8da181c`

### Shadow Effect Fix (Android & iOS)
- **Fixed**: Shadow effect rendering on Android and iOS platforms
- **Impact**: Visual improvements for dragged items with shadow effects
- **Details**: Corrected shadow rendering in the sample HomeSectionView
- **Commit**: `d18706b`

## 📦 Installation

```bash
dotnet add package Sharpnado.Maui.DragDropGridView --version 1.0.1
```

Or update your `.csproj`:

```xml
<PackageReference Include="Sharpnado.Maui.DragDropGridView" Version="1.0.1" />
```

## 🔄 Migration from 1.0.0

Simply update the package version. No breaking changes or API modifications.

## 🌐 Platform Support

No changes from v1.0.0:

| Platform | Grid Layout | Drag & Drop |
|----------|------------|-------------|
| iOS 15.0+ | ✅ | ✅ |
| Android API 21+ | ✅ | ✅ |
| Mac Catalyst 15.0+ | ✅ | ✅ |
| Windows 10.0.17763.0+ | ✅ | ⚠️ Not available |

## 📚 Documentation

- **GitHub Repository**: https://github.com/roubachof/Sharpnado.DragDropGridView
- **NuGet Package**: https://www.nuget.org/packages/Sharpnado.Maui.DragDropGridView
- **Full Changelog**: See commit history for detailed changes

## 🙏 Thanks

Thank you to everyone who reported issues and helped improve this library!
