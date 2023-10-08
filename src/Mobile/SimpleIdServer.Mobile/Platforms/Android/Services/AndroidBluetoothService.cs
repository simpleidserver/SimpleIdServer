using Android.Bluetooth;
using AndroidApp = Android.App.Application;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class AndroidBluetoothService : IBluetoothService
    {
        private HidDeviceController _hidDeviceController;
        private BluetoothDeviceListing _bluetoothDeviceListing;
        private ProfileListener _profileListener;

        public AndroidBluetoothService()
        {
            _hidDeviceController = new HidDeviceController(new HidDeviceApp(), new HidDeviceProfile());
            _bluetoothDeviceListing = new BluetoothDeviceListing();
            _profileListener = new ProfileListener();
        }

        public void Listen()
        {
            // Task.Run(() =>
            {
                // FAIRE ADVERTISTING...
                // Get all the devices.
                var result = _bluetoothDeviceListing.GetAvailableDevices();
                var computer = result.Single(r => r.Name == "THABART");
                // _hidDeviceController.Disconnect();
                // Register FIDO Device.
                _hidDeviceController.RegisterFido(AndroidApp.Context.ApplicationContext, _profileListener);
                _hidDeviceController.RequestConnect(computer.Device);
                // Thread.Sleep(10000);

            }
            //);
            // Request connect.
            // _hidDeviceController.RequestConnect(computer.Device);
        }
    }

    public class ProfileListener : IProfileListener
    {
        public void OnStatusChanged(bool registered)
        {

        }

        public void OnConnectionStateChanged(BluetoothDevice device, int state)
        {
            if(device.BondState == Bond.Bonded && state == 0)
            {

            }
        }

        public void OnInterruptData(BluetoothDevice device, int reportId, byte[] data, BluetoothHidDevice inputHost)
        {
        }

        public void OnServiceStateChanged(IBluetoothProfile proxy)
        {
        }
    }
}
