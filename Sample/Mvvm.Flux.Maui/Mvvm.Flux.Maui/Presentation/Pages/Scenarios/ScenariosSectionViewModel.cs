using System.Collections.ObjectModel;
using System.Windows.Input;

using MetroLog;

using Mvvm.Flux.Maui.Domain.Lights;
using Mvvm.Flux.Maui.Infrastructure;
using Mvvm.Flux.Maui.Localization;
using Sharpnado.Maui.DragDropGridView;
using Sharpnado.TaskLoaderView;

namespace Mvvm.Flux.Maui.Presentation.Pages.Scenarios
{
    public class ScenariosSectionViewModel : ANavigableViewModel
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(ScenariosSectionViewModel));

        private readonly ILightService _lightService;
        private int _gridRowCount = 2;
        private bool _isDragAndDropEnabled = true;
        private DragAndDropTrigger _dragTrigger = DragAndDropTrigger.LongPress;

        public ScenariosSectionViewModel(
            INavigationService navigationService,
            ILightService lightService)
            : base(navigationService)
        {
            Log.Info("Building ScenariosSectionViewModel");

            Title = GlobalResources.Section_Scenarios_Title;

            _lightService = lightService;

            _lightService.LightUpdated += OnLightUpdated;

            Loader = new TaskLoaderNotifier<ObservableCollection<Light>>();

            NavigateToLightEditCommand = new TaskLoaderCommand<Light>(NavigateToLightEditAsync);
            ToggleDragAndDropCommand = new Command(ToggleDragAndDrop);
        }

        public TaskLoaderNotifier<ObservableCollection<Light>> Loader { get; }

        public ICommand NavigateToLightEditCommand { get; }

        public ICommand ToggleDragAndDropCommand { get; }

        public int GridRowCount
        {
            get => _gridRowCount;
            set => SetProperty(ref _gridRowCount, value);
        }

        public bool IsDragAndDropEnabled
        {
            get => _isDragAndDropEnabled;
            set
            {
                if (SetProperty(ref _isDragAndDropEnabled, value))
                {
                    Log.Info($"Drag and drop {(value ? "enabled" : "disabled")}");
                }
            }
        }

        public DragAndDropTrigger DragTrigger
        {
            get => _dragTrigger;
            set => SetProperty(ref _dragTrigger, value);
        }

        public override void Destroy()
        {
            Log.Info($"Destroy()");
            _lightService.LightUpdated -= OnLightUpdated;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Log.Info("OnNavigatedTo()");
            AnalyticsHelper.TrackScreenDisplayed("ScenariosSection");

            if (Loader.IsNotStarted)
            {
                Loader.Load(_ => LoadAsync());
            }
        }

        private async Task<ObservableCollection<Light>> LoadAsync()
        {
            Log.Info("LoadAsync()");

            List<Light> domainResult = await _lightService.GetLightsAsync();
            var result = new ObservableCollection<Light>(domainResult);
            Log.Info($"{result.Count} lights loaded");

            return result;
        }

        private void ToggleDragAndDrop(object obj)
        {
            IsDragAndDropEnabled = !IsDragAndDropEnabled;
        }

        private void OnLightUpdated(object? sender, Light light)
        {
            Log.Info($"OnLightUpdated( lightId: {light.Id} )");

            ObservableCollection<Light>? itemList = Loader.Result;

            Light? matchingViewModel = itemList?
                .FirstOrDefault(item => item.Id == light.Id);
            if (matchingViewModel == null)
            {
                return;
            }

            int matchingViewModelIndex = itemList!.IndexOf(matchingViewModel);
            itemList[matchingViewModelIndex] = light;
        }

        private async Task<INavigationResult> NavigateToLightEditAsync(Light item)
        {
            Log.Info($"NavigateToLightEditAsync( id: {item.Id} )");

            var parameters = new NavigationParameters
                {
                    { nameof(Light.Id), item.Id },
                    { nameof(Title), item.Name },
                };

            var result = await NavigationService.NavigateAsync("LightEditPage", parameters);
            return result.Exception != null ? throw result.Exception : result;
        }
    }
}
