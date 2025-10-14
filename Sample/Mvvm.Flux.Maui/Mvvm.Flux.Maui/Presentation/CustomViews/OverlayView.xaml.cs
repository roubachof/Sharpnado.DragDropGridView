using Mvvm.Flux.Maui.Localization;

using Sharpnado.Tasks;

namespace Mvvm.Flux.Maui.Presentation.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OverlayView : Grid
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(OverlayView),
            defaultValue: GlobalResources.Common_Saving);

        public static readonly BindableProperty IconProperty = BindableProperty.Create(
            nameof(Icon),
            typeof(ImageSource),
            typeof(OverlayView));

        private readonly CancellationTokenSource _cts = new();

        public OverlayView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private void OnLoaded(object? sender, EventArgs e)
        {
            TaskMonitor.Create(AnimateImageAsync(_cts.Token));
        }

        private void OnUnloaded(object? sender, EventArgs e)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private async Task AnimateImageAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Image.Opacity = 1;
                Image.TranslationY = 0;
                await Task.WhenAll(
                    Image.FadeTo(0, 1000, Easing.CubicInOut),
                    Image.TranslateTo(0, -100, 1000, Easing.CubicInOut));
            }
        }
    }
}