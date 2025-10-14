using MetroLog;

using Mvvm.Flux.Maui.Infrastructure.Logging;
using Mvvm.Flux.Maui.Localization;

namespace Mvvm.Flux.Maui.Presentation.Pages.Scenarios
{
    public class ScenariosSectionViewModel : ANavigableViewModel
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(ScenariosSectionViewModel));

        public ScenariosSectionViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = GlobalResources.Section_Scenarios_Title;
        }
    }
}