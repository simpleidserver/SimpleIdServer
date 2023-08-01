using System.Globalization;
using ZXing.Net.Maui;

namespace SimpleIdServer.Mobile.Converters;

internal class BarcodeDetectionEventArgsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var barcodeDetectionEventArgs = value as BarcodeDetectionEventArgs;
        if (barcodeDetectionEventArgs == null) throw new ArgumentException("Expected value to be of type BarcodeDetectionEventArgs");
        return barcodeDetectionEventArgs;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
