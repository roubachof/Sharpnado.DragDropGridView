# Mvvm.Flux.Maui

A sample .NET MAUI application demonstrating the **Mvvm.Flux** architecture pattern - a state orchestration approach that combines MVVM with functional programming principles for building coherent, maintainable applications.

## What is Mvvm.Flux?

Mvvm.Flux is a pragmatic architectural pattern that addresses common challenges in MVVM applications:

- **Coherent UI**: The UI always reflects the current application state
- **Coherent Updates**: The application state is always consistent
- **Single Source of Truth**: Domain layer is authoritative
- **Predictable State Flow**: One-way data flow from domain to view

## Core Principles

### 1. Composition over Inheritance

Instead of complex base class hierarchies with `IsBusy` flags and error handling logic, use **TaskLoaderNotifier** components:

```csharp
// ❌ Old way: IsBusy pattern with inheritance
private async void Load(bool isRefreshing = false)
{
    IsBusy = !isRefreshing;
    IsRefreshing = isRefreshing;
    HasError = false;
    ErrorMessage = string.Empty;
    
    try
    {
        Games = await GetGamesAsync();
    }
    catch (NetworkException)
    {
        ErrorMessage = "Network error";
    }
    finally
    {
        IsBusy = false;
        IsRefreshing = false;
        HasError = ErrorMessage != string.Empty;
    }
}

// ✅ Mvvm.Flux way: Composition with TaskLoaderNotifier
public TaskLoaderNotifier<List<Game>> Loader { get; }

public void OnNavigatedTo()
{
    Loader.Load(_ => GetGamesAsync());
}
```

### 2. Single Source of Truth

The **domain layer** (services) is the authoritative source for all data. ViewModels never hold or mutate the source data directly.

```csharp
// Domain service is the single source of truth
public interface ILightService
{
    event EventHandler<Light> LightUpdated;  // Notifies subscribers of changes
    
    Task<List<Light>> GetLightsAsync();
    Task<Light> GetLightAsync(int lightId);
    Task UpdateLightAsync(Light light);
}
```

### 3. Immutability

Use **C# records** for domain entities. Never mutate entities directly; always create new instances with the `with` syntax:

```csharp
// Domain entity as immutable record
public record Light(int Id, string Name, bool IsOn);

// ✅ Correct: Create new instance
var updatedLight = light with { IsOn = true };
await _lightService.UpdateLightAsync(updatedLight);

// ❌ Wrong: Mutating shared reference
light.IsOn = true;  // Records are immutable, this won't compile
```

### 4. One-Way Data Flow

Updates flow in a single direction: **Domain → ViewModel → View**

```
1. User Action (View)
   ↓
2. ViewModel calls Domain Service
   ↓
3. Service updates data and raises event
   ↓
4. ViewModel listens to event and updates collection
   ↓
5. View reflects new state (via data binding)
```

## Key Components

### TaskLoaderNotifier<T>

Manages asynchronous loading operations with built-in state tracking:

- **NotStarted**: Initial state
- **Loading**: First load in progress
- **Success**: Data loaded successfully
- **Error**: Load failed
- **Refreshing**: Reloading existing data

```csharp
public class HomeSectionViewModel : ANavigableViewModel
{
    private readonly ILightService _lightService;
    
    public TaskLoaderNotifier<ObservableCollection<Light>> Loader { get; }
    
    public HomeSectionViewModel(ILightService lightService)
    {
        _lightService = lightService;
        
        // Subscribe to domain events (single source of truth)
        _lightService.LightUpdated += OnLightUpdated;
        
        Loader = new TaskLoaderNotifier<ObservableCollection<Light>>();
    }
    
    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        if (Loader.IsNotStarted)
        {
            Loader.Load(_ => LoadAsync());
        }
    }
    
    private async Task<ObservableCollection<Light>> LoadAsync()
    {
        List<Light> domainResult = await _lightService.GetLightsAsync();
        return new ObservableCollection<Light>(domainResult);
    }
    
    // One-way data flow: Domain service notifies of changes
    private void OnLightUpdated(object? sender, Light light)
    {
        ObservableCollection<Light>? itemList = Loader.Result;
        Light? matchingItem = itemList?.FirstOrDefault(item => item.Id == light.Id);
        
        if (matchingItem == null) return;
        
        int index = itemList!.IndexOf(matchingItem);
        itemList[index] = light;  // Replace with new immutable record
    }
    
    public override void Destroy()
    {
        // Always clean up event subscriptions
        _lightService.LightUpdated -= OnLightUpdated;
    }
}
```

### TaskLoaderCommand

Wraps commands with loading state for UI feedback during operations:

```csharp
public TaskLoaderCommand SaveCommand { get; }

public LightEditPageViewModel(ILightService lightService)
{
    SaveCommand = new TaskLoaderCommand(
        SaveAsync, 
        autoRaiseCanExecuteChange: true);
}

private async Task SaveAsync()
{
    await Task.Delay(2000);  // Simulated work
    await _lightService.UpdateLightAsync(Loader.Result.GetEntity());
    await NavigationService.GoBackAsync();
}
```

### CompositeTaskLoaderNotifier

Combines multiple loaders to track aggregate loading state (useful for disabling UI during multiple operations):

```csharp
public CompositeTaskLoaderNotifier CompositeCommandLoader { get; }

public LightEditPageViewModel()
{
    SaveCommand = new TaskLoaderCommand(SaveAsync);
    ActionOneCommand = new TaskLoaderCommand(ActionOneAsync);
    ActionTwoCommand = new TaskLoaderCommand(ActionTwoAsync);
    
    // Composite tracks if ANY command is running
    CompositeCommandLoader = new CompositeTaskLoaderNotifier(
        SaveCommand.Notifier,
        ActionOneCommand.Notifier,
        ActionTwoCommand.Notifier);
}
```

## Architecture Layers

### Domain Layer

**Location**: `Domain/`

Contains business logic and data contracts:

- **Entities**: Immutable records representing domain concepts (e.g., `Light`)
- **Service Interfaces**: Contracts for data operations (e.g., `ILightService`)
- **Service Implementations**: Actual business logic (e.g., `LightServiceMock`)
- **Domain Events**: Services raise events when data changes

```csharp
// Domain/Lights/Light.cs
public record Light(int Id, string Name, bool IsOn);

// Domain/Lights/ILightService.cs
public interface ILightService
{
    event EventHandler<Light> LightUpdated;
    
    Task<List<Light>> GetLightsAsync();
    Task<Light> GetLightAsync(int lightId);
    Task UpdateLightAsync(Light light);
}
```

### Presentation Layer

**Location**: `Presentation/`

Contains UI-related code:

- **Pages**: XAML views and their ViewModels
- **ViewModels**: Presentation logic, no business logic
- **Navigation**: Navigation services and parameters
- **Converters**: Value converters for data binding
- **Behaviors**: Reusable XAML behaviors
- **CustomViews**: Reusable UI components

**ViewModel Pattern**:
```csharp
public class LightViewModel : BindableBase
{
    private readonly Light _light;  // Immutable domain entity
    private bool _isOn;
    
    public LightViewModel(Light light)
    {
        _light = light;
        _isOn = _light.IsOn;
    }
    
    public string Name => _light.Name;
    
    public bool IsOn
    {
        get => _isOn;
        set => SetProperty(ref _isOn, value);
    }
    
    // Create new entity with updated state
    public Light GetEntity()
    {
        return _light with { IsOn = _isOn };
    }
}
```

### Infrastructure Layer

**Location**: `Infrastructure/`

Cross-cutting concerns:

- **Helpers**: Utility classes (e.g., `AnalyticsHelper`)
- **Extensions**: Extension methods
- **Logging**: Logging infrastructure (MetroLog)
- **Validation**: Validation logic
- **Mocking**: Mock data generators

## Update Flow Example

Here's how updating a light's state works in Mvvm.Flux:

```csharp
// 1. USER ACTION: User toggles light in LightEditPage
private void ToggleLight()
{
    Loader.Result.IsOn = !Loader.Result.IsOn;
}

// 2. USER SAVES: User clicks save button
private async Task SaveAsync()
{
    // Get updated entity from ViewModel
    Light updatedLight = Loader.Result.GetEntity();
    
    // 3. CALL DOMAIN: Update via service
    await _lightService.UpdateLightAsync(updatedLight);
    
    // 4. SERVICE RAISES EVENT
    // (Inside LightService.UpdateLightAsync)
    LightUpdated?.Invoke(this, updatedLight);
    
    await NavigationService.GoBackAsync();
}

// 5. ALL SUBSCRIBERS NOTIFIED: HomeSectionViewModel receives update
private void OnLightUpdated(object? sender, Light light)
{
    // Find and replace the old record with new one
    ObservableCollection<Light>? itemList = Loader.Result;
    Light? matchingItem = itemList?.FirstOrDefault(item => item.Id == light.Id);
    
    if (matchingItem != null)
    {
        int index = itemList!.IndexOf(matchingItem);
        itemList[index] = light;  // Immutable update
    }
}

// 6. UI UPDATES: View reflects new state via data binding
```

## Why This Matters

### Problems Mvvm.Flux Solves

❌ **Without Mvvm.Flux**:
- Shared entity references lead to unexpected mutations
- Multiple ViewModels might have stale data
- Complex error handling in every ViewModel
- `IsBusy` boilerplate everywhere
- Difficult to reason about state changes

✅ **With Mvvm.Flux**:
- Immutable entities prevent unexpected mutations
- Domain events ensure all ViewModels stay in sync
- TaskLoaderView handles loading states consistently
- Clear, predictable data flow
- Single source of truth eliminates inconsistencies

## Key Dependencies

- **[Sharpnado.TaskLoaderView](https://github.com/roubachof/Sharpnado.TaskLoaderView)** (v2.5.1) - Async state management
- **Prism.DryIoc.Maui** (v9.0.271-pre) - DI container and navigation
- **MetroLog.Maui** (v2.1.0) - Logging infrastructure
- **Mopups** (v1.3.0) - Popup pages
- **Sharpnado.Tabs.Maui** (v3.2.1) - Tab control

## Building and Running

```bash
# Build the solution
dotnet build Mvvm.Flux.Maui.sln

# Run on iOS Simulator
dotnet build -t:Run -f net9.0-ios Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj

# Run on Android Emulator
dotnet build -t:Run -f net9.0-android Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj

# Run on Mac Catalyst
dotnet build -t:Run -f net9.0-maccatalyst Mvvm.Flux.Maui/Mvvm.Flux.Maui.csproj
```

## Coding Conventions

- ViewModels inherit from `ANavigableViewModel` or `BindableBase`
- Use Prism's `INavigationService` with `NavigationParameters`
- Use MetroLog for logging: `LoggerFactory.GetLogger(nameof(MyClass))`
- Always use records for domain entities
- Update entities via `with` syntax
- Subscribe to domain events in constructor
- Unsubscribe in `Destroy()` override
- Use `TaskLoaderNotifier` for all async operations
- Use `TaskLoaderCommand` for commands with loading feedback
- Track screen views with `AnalyticsHelper.TrackScreenDisplayed()`

## Learn More

See `mvvm.flux/mvvm-flux.md` for presentation slides explaining the architecture in detail.

## Platform Support

- .NET 9.0 (Android, iOS, MacCatalyst)
- .NET 8.0 (Windows)
- iOS 15.0+
- Android API 21+
- MacCatalyst 15.0+
- Windows 10.0.17763.0+
