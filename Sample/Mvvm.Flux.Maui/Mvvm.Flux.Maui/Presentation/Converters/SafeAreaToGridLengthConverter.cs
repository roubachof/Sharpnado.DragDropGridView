using System.Globalization;

using Mvvm.Flux.Maui.Infrastructure;

namespace Mvvm.Flux.Maui.Presentation.Converters
{
    public class TopSafeAreaToGridLengthConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || !double.TryParse(parameter.ToString(), out var defaultValue))
            {
                defaultValue = 0;
            }

            var result = new GridLength(defaultValue + PlatformService.GetSafeArea().Top);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class BottomSafeAreaToGridLengthConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || !double.TryParse(parameter.ToString(), out var defaultValue))
            {
                defaultValue = 0;
            }

            return new GridLength(defaultValue + PlatformService.GetSafeArea().Bottom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
