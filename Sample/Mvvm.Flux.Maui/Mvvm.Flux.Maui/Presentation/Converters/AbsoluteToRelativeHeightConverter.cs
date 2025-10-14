using System.Globalization;

namespace Mvvm.Flux.Maui.Presentation.Converters
{
    public class AbsoluteToRelativeHeightConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double parentHeight) || parentHeight < 1)
            {
                return 1;
            }

            if (!(parameter is string) || !double.TryParse((string)parameter, out var absoluteHeight))
            {
                return 1;
            }

            return absoluteHeight / parentHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}