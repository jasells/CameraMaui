using System.Globalization;

namespace Video_Demo.Converters
{
    public class BoolToColorConverter : IValueConverter, IMarkupExtension
    {
        public Color TrueColor { get; set; } = Colors.Green;
        public Color FalseColor { get; set; } = Colors.Red;

        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is bool bVal)
                return bVal ? TrueColor : FalseColor;

            return FalseColor;
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is Color colorVal)
                return colorVal.Equals(TrueColor);

            return false;
        }

        public object ProvideValue(IServiceProvider? serviceProvider)
        {
            return this;
        }
    }
}