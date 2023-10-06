using Android.Bluetooth;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public interface IDeviceStateListener
    {
        void OnConnectionStateChanged(BluetoothDevice device, int state);
        void OnStatusChanged(bool registered);
        void OnInterruptData(BluetoothDevice device, int reportId, byte[] data, BluetoothHidDevice inputHost);
    }
}
