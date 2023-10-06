using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.Interop;
using AndroidContent = Android.Content;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class HidDeviceProfile
    {
        private static ParcelUuid HOGP_UUID = ParcelUuid.FromString("00001812-0000-1000-8000-00805f9b34fb");
        private static ParcelUuid HID_UUID = ParcelUuid.FromString("00001124-0000-1000-8000-00805f9b34fb");

        public BluetoothHidDevice Service { get; set; }
        public IServiceStateListener ServiceStateListener { get; set; }
        public BluetoothAdapter Adapter { get; set; }

        public List<BluetoothDevice> GetConnectedDevices()
        {
            if (Service == null) return new List<BluetoothDevice>();
            return Service.ConnectedDevices.ToList();
        }

        public List<BluetoothDevice> GetDevicesMatchingConnectionStates(ProfileState[] states)
        {
            return Service.GetDevicesMatchingConnectionStates(states).ToList();
        }

        public void Connect(BluetoothDevice device)
        {
            if(Service != null && IsProfileSupported(device))
            {
                Service.Connect(device);
            }
        }

        public void Disconnect(BluetoothDevice device)
        {
            if (Service != null && IsProfileSupported(device))
            {
                Service.Disconnect(device);
            }
        }

        public static bool IsProfileSupported(BluetoothDevice device)
        {
            var uuidLst = device.GetUuids();
            if(uuidLst != null)
            {
                foreach(var uuid in uuidLst)
                {
                    if (HID_UUID.Equals(uuid) || HOGP_UUID.Equals(uuid)) return false;
                }
            }

            return true;
        }

        public void RegisterServiceListener(AndroidContent.Context context, IServiceStateListener listener)
        {
            context = context.ApplicationContext;
            ServiceStateListener = listener;
            if(Adapter == null)
            {
                var bluetoothManager = (BluetoothManager)context.GetSystemService(AndroidContent.Context.BluetoothService);
                Adapter = bluetoothManager.Adapter;
            }

            var serviceListener = new HidDeviceProfileServiceListener(this);
            Adapter.GetProfileProxy(context, serviceListener, ProfileType.HidDevice);
        }

        public void UnregisterServiceListener()
        {
            if(Service != null)
            {
                Adapter.CloseProfileProxy(ProfileType.HidDevice, Service);
                Service = null;
            }

            ServiceStateListener = null;
        }
    }

    public class HidDeviceProfileServiceListener : AndroidContent.BroadcastReceiver, IBluetoothProfileServiceListener
    {
        private readonly HidDeviceProfile _deviceProfile;

        public HidDeviceProfileServiceListener(HidDeviceProfile deviceProfile)
        {
            _deviceProfile = deviceProfile;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string ss = "";
        }

        public void OnServiceConnected([GeneratedEnum] ProfileType profile, IBluetoothProfile proxy)
        {
            _deviceProfile.Service = (BluetoothHidDevice)proxy;
            if (_deviceProfile.ServiceStateListener != null) _deviceProfile.ServiceStateListener.OnServiceStateChanged(_deviceProfile.Service);
            else _deviceProfile.Adapter.CloseProfileProxy(ProfileType.HidDevice, proxy);
        }

        public void OnServiceDisconnected([GeneratedEnum] ProfileType profile)
        {
            _deviceProfile.Service = null;
            if (_deviceProfile.ServiceStateListener != null) _deviceProfile.ServiceStateListener.OnServiceStateChanged(null);
        }
    }
}
