using System.Collections.ObjectModel;
using System.Windows.Input;

using MetroLog;

using Mvvm.Flux.Maui.Domain.Lights;
using Mvvm.Flux.Maui.Infrastructure;
using Mvvm.Flux.Maui.Localization;
using Sharpnado.GridLayout;
using Sharpnado.TaskLoaderView;

namespace Mvvm.Flux.Maui.Presentation.Pages.Home
{
    public class HomeSectionViewModel : ANavigableViewModel
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(HomeSectionViewModel));

        private readonly ILightService _lightService;
        private int _gridColumnCount = 2;
        private bool _isDragAndDropEnabled = true;
        private DragAndDropTrigger _dragTrigger = DragAndDropTrigger.Pan;

        public HomeSectionViewModel(
            INavigationService navigationService,
            ILightService lightService)
            : base(navigationService)
        {
            Log.Info("Building HomeSectionViewModel");

            Title = GlobalResources.Section_Home_Title;

            _lightService = lightService;

            _lightService.LightUpdated += OnLightUpdated;

            Loader = new TaskLoaderNotifier<ObservableCollection<Light>>();

            NavigateToLightEditCommand = new TaskLoaderCommand<Light>(NavigateToLightEditAsync);
            ToggleViewModeCommand = new Command(ToggleViewMode);
            ToggleDragAndDropCommand = new Command(ToggleDragAndDrop);
            SetThreeColumnsCommand = new Command(() => GridColumnCount = 3);

            AddGarageCommand = new Command(TakePhoto);
            AddBedroomCommand = new Command(TakePhoto);
            AddKitchenCommand = new Command(ChoosePhoto);
            AddLivingRoomCommand = new Command(TakeVideo);
            AddBathroomCommand = new Command(ChooseVideo);
        }

        public TaskLoaderNotifier<ObservableCollection<Light>> Loader { get; }

        public ICommand NavigateToLightEditCommand { get; }

        public ICommand ToggleDragAndDropCommand { get; }

        public ICommand ToggleViewModeCommand { get; }

        public ICommand SetThreeColumnsCommand { get; }

        public int GridColumnCount
        {
            get => _gridColumnCount;
            set => SetProperty(ref _gridColumnCount, value);
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

        public ICommand AddGarageCommand { get; }

        public ICommand AddBedroomCommand { get; }

        public ICommand AddKitchenCommand { get; }

        public ICommand AddLivingRoomCommand { get; }

        public ICommand AddBathroomCommand { get; }

        public override void Destroy()
        {
            Log.Info($"Destroy()");
            _lightService.LightUpdated -= OnLightUpdated;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Log.Info("OnNavigatedTo()");
            AnalyticsHelper.TrackScreenDisplayed("HomeSection");

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

        private void ToggleViewMode()
        {
            GridColumnCount = GridColumnCount == 2 ? 1 : 2;
            Log.Info($"View mode toggled to {(GridColumnCount == 2 ? "Grid" : "List")}");
        }

        private void ChooseVideo(object obj)
        {
        }

        private void TakeVideo(object obj)
        {
        }

        private void ChoosePhoto(object obj)
        {
        }

        private void TakePhoto(object obj)
        {
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
