using System.Globalization;

namespace Mvvm.Flux.Maui.Presentation.Converters
{
    public class BoolToStaticResourceConverter : IValueConverter, IMarkupExtension
    {
        public string IfTrue { get; set; }

        public string IfFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool test = (bool)value;

            var trueResource = GetResource(IfTrue);
            var falseResource = GetResource(IfFalse);
            return test ? trueResource : falseResource;
        }

        private object? GetResource(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (Application.Current!.Resources.TryGetValue(key, out var resource))
            {
                return resource;
            }

            throw new InvalidOperationException("Resource not found");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // One-Way converter only
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}