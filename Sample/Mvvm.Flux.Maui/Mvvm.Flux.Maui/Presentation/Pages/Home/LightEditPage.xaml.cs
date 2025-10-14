using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

using Mvvm.Flux.Maui.Infrastructure;

namespace Mvvm.Flux.Maui.Presentation.Pages.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LightEditPage : ContentPage
    {
        public LightEditPage()
        {
            InitializeComponent();

            On<iOS>().SetUseSafeArea(false);
        }
    }
}