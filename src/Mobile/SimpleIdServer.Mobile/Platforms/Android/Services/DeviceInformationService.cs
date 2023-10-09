using Android.Bluetooth;
using Java.Util;

namespace SimpleIdServer.Mobile.Platforms.Android.Services;

public class DeviceInformationService : IGattService
{
    private static UUID SERVICE_DEVICE_INFORMATION = UUID.FromString("0000-180A-0000-1000-8000-00805F9B34FB");

    public BluetoothGattService Setup()
    {
        var result = new BluetoothGattService(SERVICE_DEVICE_INFORMATION, GattServiceType.Primary);
        return result;
    }
}
