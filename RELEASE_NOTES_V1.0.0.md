# Release v1.0.0 - Sharpnado.Maui.DragDropGridView

## 📦 Package Information

- **Package Name**: Sharpnado.Maui.DragDropGridView
- **Version**: 1.0.0
- **Release Date**: January 23, 2025
- **Target Framework**: .NET 9.0
- **License**: MIT

## ✅ Pre-Release Checklist

### Code & Build
- [x] All tests passing
- [x] Release build succeeds
- [x] Version updated to 1.0.0 in DragDropGridView.csproj
- [x] No critical warnings in release build
- [x] Sample application builds and runs

### Documentation
- [x] README.md updated with comprehensive documentation
- [x] CHANGELOG.md created with v1.0.0 release notes
- [x] PACKAGE_README.md created for NuGet package page
- [x] Package release notes updated in csproj
- [x] API documentation complete
- [x] Usage examples included

### Package Metadata
- [x] PackageId: Sharpnado.Maui.DragDropGridView
- [x] Version: 1.0.0
- [x] Authors: Jean-Marie Alfonsi
- [x] Description: Updated and comprehensive
- [x] Tags: maui, android, ios, windows, maccatalyst, grid, dragdrop, drag-and-drop, reorder
- [x] Project URL: https://github.com/roubachof/Sharpnado.GridLayout
- [x] License: MIT
- [ ] Package Icon: gridlayout.png (create Docs/gridlayout.png before release)

## 🚀 Release Steps

### 1. Create NuGet Package

```bash
cd /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/DragDropGridView
dotnet pack -c Release
```

The package will be created at:
`DragDropGridView/bin/Release/Sharpnado.Maui.DragDropGridView.1.0.0.nupkg`

### 2. Test Package Locally (Optional)

```bash
# Add local package source
dotnet nuget add source /Users/roubachof/Dev/Sharpnado/src/Sharpnado.GridLayout/DragDropGridView/bin/Release --name LocalPackages

# Test in a sample project
dotnet add package Sharpnado.Maui.DragDropGridView --version 1.0.0 --source LocalPackages
```

### 3. Publish to NuGet.org

```bash
# Publish to NuGet (requires API key)
dotnet nuget push DragDropGridView/bin/Release/Sharpnado.Maui.DragDropGridView.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### 4. Create GitHub Release

1. Commit all changes:
```bash
git add .
git commit -m "Release v1.0.0 - Initial release of Sharpnado.Maui.DragDropGridView"
```

2. Create and push tag:
```bash
git tag -a v1.0.0 -m "Version 1.0.0 - Initial Release"
git push origin v1.0.0
git push origin main
```

3. Create GitHub Release:
   - Go to https://github.com/roubachof/Sharpnado.GridLayout/releases/new
   - Tag: v1.0.0
   - Title: v1.0.0 - Initial Release
   - Description: Copy from CHANGELOG.md
   - Attach: Sharpnado.Maui.DragDropGridView.1.0.0.nupkg

### 5. Post-Release

- [ ] Verify package appears on NuGet.org
- [ ] Test installation from NuGet in a fresh project
- [ ] Announce release (Twitter, blog, etc.)
- [ ] Update documentation site (if applicable)

## 📋 Release Highlights

### Core Features
- Automatic grid layout with responsive column calculation
- Full ItemsSource/DataTemplate binding
- Header support with custom DataTemplate
- Adaptive item sizing
- Orientation-aware layout
- ScrollView integration

### Drag & Drop (iOS, Android, Mac Catalyst)
- Built-in drag-and-drop reordering
- Two trigger modes: Pan and LongPress
- Automatic ScrollView edge detection
- Smooth batched shift animations
- OnItemsReorderedCommand support
- Automatic ItemsSource synchronization

### Customizable Animations
- ViewStartDraggingAnimation / ViewStopDraggingAnimation
- DragAndDropEnabledItemsAnimation / DragAndDropDisabledItemsAnimation
- Predefined animations via DragDropAnimations static class
- Custom animation functions support

### Integration
- Pure .NET MAUI implementation
- Includes MR.Gestures fork for gesture handling
- Full logging support
- MVVM-friendly

## 🌍 Platform Support

- ✅ iOS 15.0+ (full drag-and-drop)
- ✅ Android API 21+ (full drag-and-drop)
- ✅ Mac Catalyst 15.0+ (full drag-and-drop)
- ⚠️ Windows 10.0.17763.0+ (grid layout only, no drag-and-drop)

## ⚠️ Known Limitations

- **Windows**: Drag-and-drop not supported due to gesture coordinate system complexities
- **Android**: Dragged items may appear behind other items (ZIndex changes cancel gestures)
- **ItemsSource**: Must implement IList for automatic reordering
- **Draggable items**: Must be wrapped in MR.Gestures-compatible controls

## 📦 Dependencies

- Microsoft.Maui.Controls 9.0.110+
- .NET 9.0
- Sharpnado.TaskMonitor 1.1.0
- MR.Gestures (included)

## 🔗 Links

- **NuGet Package**: https://www.nuget.org/packages/Sharpnado.Maui.DragDropGridView
- **GitHub Repository**: https://github.com/roubachof/Sharpnado.GridLayout
- **Documentation**: https://github.com/roubachof/Sharpnado.GridLayout#readme
- **Issues**: https://github.com/roubachof/Sharpnado.GridLayout/issues
- **License**: https://github.com/roubachof/Sharpnado.GridLayout/blob/main/LICENSE

## 📝 Notes

- This is the initial stable release for .NET 9 MAUI
- The package replaces any previous beta or preview versions
- Feedback and contributions are welcome via GitHub issues and pull requests
- Consider creating a package icon (128x128 PNG) at Docs/gridlayout.png before final publish

---

**Status**: ✅ Ready for release (pending package icon creation)
