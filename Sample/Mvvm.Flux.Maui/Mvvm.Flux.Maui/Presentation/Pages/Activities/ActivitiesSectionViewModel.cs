using Mvvm.Flux.Maui.Localization;

namespace Mvvm.Flux.Maui.Presentation.Pages.Activities
{
    public class ActivitiesSectionViewModel : ANavigableViewModel
    {
        public ActivitiesSectionViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = GlobalResources.Section_Activities_Title;
        }
    }
}