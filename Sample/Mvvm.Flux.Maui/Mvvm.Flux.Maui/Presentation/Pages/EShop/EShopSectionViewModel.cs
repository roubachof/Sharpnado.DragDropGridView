using MetroLog;

using Mvvm.Flux.Maui.Infrastructure.Logging;
using Mvvm.Flux.Maui.Localization;

namespace Mvvm.Flux.Maui.Presentation.Pages.EShop
{
    public class EShopSectionViewModel : ANavigableViewModel
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(EShopSectionViewModel));

        public EShopSectionViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = GlobalResources.Section_EShop_Title;
        }
    }
}