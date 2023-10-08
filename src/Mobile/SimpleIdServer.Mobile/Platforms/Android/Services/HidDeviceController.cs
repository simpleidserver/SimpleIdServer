using Android.Bluetooth;
using AndroidContent = Android.Content;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class HidDeviceController
    {
        private readonly HidDeviceProfile _hidDeviceProfile;
        private readonly HidDeviceApp _hidDeviceApp;
        private readonly List<IProfileListener> _listeners = new List<IProfileListener>();
        private IProfileListener _profileListener;
        private BluetoothDevice _waitingForDevice;
        private BluetoothDevice _connectedDevice;

        public HidDeviceController(HidDeviceApp hidDeviceApp, HidDeviceProfile hidDeviceProfile)
        {
            _hidDeviceApp = hidDeviceApp;
            _hidDeviceProfile = hidDeviceProfile;
            _profileListener = new HidDeviceProfileListener(this);
        }

        public bool IsAppRegistered { get; set; }
        public bool IsConnected => _connectedDevice != null;
        public BluetoothDevice WaitingForDevice => _waitingForDevice;
        public HidDeviceApp HidDeviceApp => _hidDeviceApp;
        public List<IProfileListener> Listeners => _listeners;

        public void RequestConnect(BluetoothDevice device)
        {
            _waitingForDevice = device;
            _connectedDevice = null;
            if(!IsAppRegistered)
            {
                return;
            }

            UpdateDevices();
            if (device != null && device.Equals(_connectedDevice))
            {
                foreach(var listener in _listeners)
                {
                    listener.OnConnectionStateChanged(device, 0);
                }
            }
        }
        
        public HidDeviceProfile RegisterFido(AndroidContent.Context context, IProfileListener listener)
        {
            _listeners.Add(listener);
            context = context.ApplicationContext;
            _hidDeviceProfile.RegisterServiceListener(context, _profileListener);
            _hidDeviceApp.RegisterDeviceListener(_profileListener);
            return _hidDeviceProfile;
        }

        public void Disconnect()
        {
            if (IsConnected) RequestConnect(null);
        }

        private void UpdateDevices()
        {
            BluetoothDevice connected = null;
            var connectedDevices = _hidDeviceProfile.GetConnectedDevices();
            foreach(var connectedDevice in connectedDevices)
            {
                if(connectedDevice == _waitingForDevice || connectedDevice == _connectedDevice)
                {
                    connected = connectedDevice;
                }
                else
                {
                    _hidDeviceProfile.Disconnect(connectedDevice);
                }
            }

            var connectionStateDevices = _hidDeviceProfile.GetDevicesMatchingConnectionStates(new[]
            {
                ProfileState.Connected,
                ProfileState.Connecting,
                ProfileState.Disconnecting
            });
            if(!connectionStateDevices.Any() && _waitingForDevice != null)
            {
                _hidDeviceProfile.Connect(_waitingForDevice);
                _connectedDevice = connected;
                _waitingForDevice = null;
            }

            _hidDeviceApp.Device = _connectedDevice;
        }
    }

    public class HidDeviceProfileListener : Java.Lang.Throwable, IProfileListener
    {
        private readonly HidDeviceController _hidDeviceController;

        public HidDeviceProfileListener(HidDeviceController hidDeviceController)
        {
            _hidDeviceController = hidDeviceController;
        }

        public void OnConnectionStateChanged(BluetoothDevice device, int state)
        {
            // throw new NotImplementedException();
        }

        public void OnInterruptData(BluetoothDevice device, int reportId, byte[] data, BluetoothHidDevice inputHost)
        {
            // throw new NotImplementedException();
        }

        public void OnServiceStateChanged(IBluetoothProfile proxy)
        {
            if(proxy == null)
            {
                if(_hidDeviceController.IsAppRegistered)
                {
                    OnStatusChanged(false);
                }
            }
            else
            {
                _hidDeviceController.HidDeviceApp.RegisterApp(proxy);
            }
        }

        public void OnStatusChanged(bool registered)
        {
            if (_hidDeviceController.IsAppRegistered == registered) return;
            _hidDeviceController.IsAppRegistered = registered;
            foreach(var listener in _hidDeviceController.Listeners)
            {
                listener.OnStatusChanged(registered);
            }

            if(registered && _hidDeviceController.WaitingForDevice != null)
            {
                (_hidDeviceController.RequestConnect(_hidDeviceController.WaitingForDevice);
            }
        }
    }
}
