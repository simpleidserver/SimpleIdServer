using Android.Bluetooth;
using Java.Util;

namespace SimpleIdServer.Mobile.Platforms.Android.Services;

public class FIDO2AuthenticatorService : IGattService
{
    private static UUID SERVICE_U2F_AUTHENTICATOR = UUID.FromString("0000-FFFD-0000-1000-8000-00805F9B34FB");

    public BluetoothGattService Setup()
    {
        var result = new BluetoothGattService(SERVICE_U2F_AUTHENTICATOR, GattServiceType.Primary);
        return result;
    }
}
