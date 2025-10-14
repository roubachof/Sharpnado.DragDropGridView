using System.Globalization;

namespace Mvvm.Flux.Maui.Presentation.Converters
{
    public class HeightToRoundCornerConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double doubleValue) || doubleValue <= 0)
            {
                return 0;
            }

            return (int)(doubleValue / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}