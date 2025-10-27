using MetroLog;

using Mvvm.Flux.Maui.Domain.Lights;
using Mvvm.Flux.Maui.Presentation.Navigation;
using Mvvm.Flux.Maui.Presentation.Pages.Activities;
using Mvvm.Flux.Maui.Presentation.Pages.EShop;
using Mvvm.Flux.Maui.Presentation.Pages.Home;
using Mvvm.Flux.Maui.Presentation.Pages.Scenarios;

namespace Mvvm.Flux.Maui.Presentation.Pages
{
    public class MainPageViewModel : ANavigableViewModel
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(MainPageViewModel));

        private readonly ANavigableViewModel[] _childrenViewModels = new ANavigableViewModel[4];

        private int _selectedViewModelIndex;

        public MainPageViewModel(INavigationService navigationService, ILightService lightService)
            : base(navigationService)
        {
            Log.Info("Building MainPageViewModel");

            _childrenViewModels[(int)MainSections.Home] =
                new HomeSectionViewModel(navigationService, lightService);

            _childrenViewModels[(int)MainSections.Scenarios] =
                new ScenariosSectionViewModel(navigationService, lightService);

            _childrenViewModels[(int)MainSections.EShop] =
                new EShopSectionViewModel(navigationService);

            _childrenViewModels[(int)MainSections.Activities] =
                new ActivitiesSectionViewModel(navigationService);
        }

        public int SelectedViewModelIndex
        {
            get => _selectedViewModelIndex;
            set
            {
                SetProperty(ref _selectedViewModelIndex, value);
                OnChildrenSelection(_selectedViewModelIndex);
            }
        }

        public HomeSectionViewModel HomeSection => (HomeSectionViewModel)_childrenViewModels[(int)MainSections.Home];

        public ScenariosSectionViewModel ScenariosSection => (ScenariosSectionViewModel)_childrenViewModels[(int)MainSections.Scenarios];

        public EShopSectionViewModel EShopSection => (EShopSectionViewModel)_childrenViewModels[(int)MainSections.EShop];

        public ActivitiesSectionViewModel ActivitiesSection => (ActivitiesSectionViewModel)_childrenViewModels[(int)MainSections.Activities];

        public override void Initialize(INavigationParameters parameters)
        {
            Log.Info($"Initialize( parameters: {parameters} )");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Log.Info($"OnNavigatedTo( parameters: {parameters} )");

            parameters.TryGetValue<MainSections>(NavigationKeys.SectionKey, out var targetSection);

            SelectedViewModelIndex = (int)targetSection;
        }

        public override void Destroy()
        {
            Log.Info("Destroy()");
            foreach (var child in _childrenViewModels)
            {
                child.Destroy();
            }
        }

        private void OnChildrenSelection(int childrenIndex, INavigationParameters parameters = null)
        {
            Log.Info($"OnChildrenSelection( childrenIndex: {childrenIndex} )");

            _childrenViewModels[childrenIndex].OnNavigatedTo(parameters);

            Title = _childrenViewModels[childrenIndex].Title;
        }
    }
}
