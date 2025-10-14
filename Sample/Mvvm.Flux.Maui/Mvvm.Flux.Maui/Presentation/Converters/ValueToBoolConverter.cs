using System.Globalization;

namespace Mvvm.Flux.Maui.Presentation.Converters
{
    public class ValueToBoolConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null
                || (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
                || (value is IEnumerable<object> enumerableValue && !enumerableValue.Any())
                || (double.TryParse(value.ToString(), out var doubleValue) && doubleValue == 0))
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
