using Mvvm.Flux.Maui.Domain.Lights;

namespace Mvvm.Flux.Maui.Presentation.Pages.Home
{
    public class LightViewModel : BindableBase
    {
        private readonly Light _light;

        private bool _isOn;

        public LightViewModel(Light light)
        {
            _light = light;

            _isOn = _light.IsOn;
        }

        public string Name => _light.Name;

        public string Description => _light.Description;

        public bool IsOn
        {
            get => _isOn;
            set => SetProperty(ref _isOn, value);
        }

        public Light GetEntity()
        {
            return _light with { IsOn = _isOn };
        }
    }
}