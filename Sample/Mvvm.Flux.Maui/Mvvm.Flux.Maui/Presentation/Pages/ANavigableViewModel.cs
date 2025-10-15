using System.Windows.Input;

namespace Mvvm.Flux.Maui.Presentation.Pages
{
    public abstract class ANavigableViewModel : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        private string _title;

        protected ANavigableViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            GoBackCommand = new Command(async () => await NavigationService.GoBackAsync());
        }

        public ICommand GoBackCommand
        {
            get;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        protected INavigationService NavigationService { get; }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {
        }
    }
}
