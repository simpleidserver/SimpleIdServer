using System.Globalization;

namespace SimpleIdServer.Mobile.Converters;

public class ToggledEventArgsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var toggleEventArgs = value as ToggledEventArgs;
        if (toggleEventArgs == null) throw new ArgumentException("Expected value to be of type BarcodeDetectionEventArgs");
        return toggleEventArgs;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}