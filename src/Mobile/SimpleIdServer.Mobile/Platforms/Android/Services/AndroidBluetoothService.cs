using Android.Bluetooth;
using AndroidApp = Android.App.Application;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class AndroidBluetoothService : IBluetoothService
    {
        private HidDeviceController _hidDeviceController;
        private BluetoothDeviceListing _bluetoothDeviceListing;

        public AndroidBluetoothService()
        {
            _hidDeviceController = new HidDeviceController(new HidDeviceApp(), new HidDeviceProfile());
            _bluetoothDeviceListing = new BluetoothDeviceListing();
        }

        public void Listen()
        {
            var profileListener = new ProfileListener();
            // Get all the devices.
            var result = _bluetoothDeviceListing.GetAvailableDevices();
            var computer = result.Single(r => r.Name == "THABART");
            _hidDeviceController.Disconnect();
            // Register FIDO Device.
            _hidDeviceController.RegisterFido(AndroidApp.Context.ApplicationContext, profileListener);
            // Request connect.
            _hidDeviceController.RequestConnect(computer.Device);
        }
    }

    public class ProfileListener : IProfileListener
    {
        public void OnConnectionStateChanged(BluetoothDevice device, int state)
        {
        }

        public void OnInterruptData(BluetoothDevice device, int reportId, byte[] data, BluetoothHidDevice inputHost)
        {
        }

        public void OnServiceStateChanged(IBluetoothProfile proxy)
        {
        }

        public void OnStatusChanged(bool registered)
        {
        }
    }
}
