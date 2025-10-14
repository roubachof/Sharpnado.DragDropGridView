using System.Globalization;

namespace Mvvm.Flux.Maui.Presentation.Converters
{
    public class BoolToObjectConverter : IValueConverter, IMarkupExtension
    {
        public object IfTrue { get; set; }

        public object IfFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool test = (bool)value;
            return test ? IfTrue : IfFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}