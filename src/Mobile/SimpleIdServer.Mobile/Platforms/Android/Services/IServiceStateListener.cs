using Android.Bluetooth;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public interface IServiceStateListener
    {
        void OnServiceStateChanged(IBluetoothProfile proxy);
    }
}
