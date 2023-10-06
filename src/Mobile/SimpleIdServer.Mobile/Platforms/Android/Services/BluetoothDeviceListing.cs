using Android.Bluetooth;
using Android.Content;
using AndroidApp = Android.App.Application;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class BluetoothDeviceListing
    {
        public static string HID_PREFERENCES = "hidbt";
        public static string HID_FIDO_HOST = "FIDO_HID_HOST";
        public static string HID_PREFERRED_HOST = "DEFAULT_HID_HOST";
        public static string HID_UNKNOWN_HOST = "HID_UNKNOWN_HOST";
        private readonly BluetoothAdapter _bluetoothAdapter;
        private readonly ISharedPreferences _sharedPreferences;

        public BluetoothDeviceListing()
        {
            _sharedPreferences = AndroidApp.Context.ApplicationContext.GetSharedPreferences(HID_PREFERENCES, FileCreationMode.Private);
            var bluetoothManager = (BluetoothManager)AndroidApp.Context.GetSystemService(Context.BluetoothService);
            _bluetoothAdapter = bluetoothManager.Adapter;
        }

        public List<BluetoothDeviceWrapper> GetAvailableDevices()
        {
            var pairedDevices = _bluetoothAdapter.BondedDevices;
            var result = new List<BluetoothDeviceWrapper>();
            foreach (var device in pairedDevices)
            {
                if (HidDeviceProfile.IsProfileSupported(device))
                {
                    var wrapper = new BluetoothDeviceWrapper(device);
                    wrapper.Type = _sharedPreferences.GetString(wrapper.Hash(), HID_UNKNOWN_HOST);
                    wrapper.IsDefault = IsHidDefaultDevice(wrapper);
                    result.Add(wrapper);
                }
            }

            return result;
        }
        
        public List<BluetoothDeviceWrapper> GetHidAvailableDevices()
        {
            var availableDevices = GetAvailableDevices();
            return availableDevices.Where(d => _sharedPreferences.Contains(d.Hash())).ToList();
        }

        public bool AddHidDeviceAsFido(BluetoothDeviceWrapper wrapper)
        {
            if (HidDeviceProfile.IsProfileSupported(wrapper.Device)) return SaveHidPreference(wrapper, HID_FIDO_HOST);
            return false;
        }

        private bool SaveHidPreference(BluetoothDeviceWrapper device, string type)
        {
            return _sharedPreferences.Edit().PutString(device.Hash(), type).Commit();
        }

        private bool IsHidDefaultDevice(BluetoothDevice device)
        {
            var wrapper = new BluetoothDeviceWrapper(device);
            return IsHidDefaultDevice(wrapper);
        }

        private bool IsHidDefaultDevice(BluetoothDeviceWrapper device)
        {
            var hidDefaultHost = _sharedPreferences.GetString(HID_PREFERRED_HOST, null);
            var deviceHash = device.Hash();
            return deviceHash.Equals(hidDefaultHost);
        }
    }
}
