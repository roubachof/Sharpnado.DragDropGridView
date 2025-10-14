using MetroLog;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Mvvm.Flux.Maui.Presentation.Pages
{
    public partial class MainPage : ContentPage
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(MainPage));

        public MainPage()
        {
            InitializeComponent();

            On<iOS>().SetUseSafeArea(false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}