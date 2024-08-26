using Microsoft.Maui.Graphics.Converters;
using System.Globalization;

namespace SimpleIdServer.Mobile.Converters;

public class StringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ColorTypeConverter converter = new ColorTypeConverter();
        if (value == null) value = "#000000";
        Color color = (Color)(converter.ConvertFromInvariantString((string)value));
        return color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
