using Android.Bluetooth;
using Android.Runtime;
using Java.Interop;
using Java.Lang;
using Java.Util.Concurrent;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class HidDeviceApp
    {
        private BluetoothHidDevice _inputHost;
        private IDeviceStateListener _deviceStateListener;

        public BluetoothDevice Device { get; set; }

        public BluetoothHidDevice InputHost => _inputHost;

        public bool Registered { get; set; }

        public bool RegisterApp(IBluetoothProfile inputHost)
        {
            _inputHost = (BluetoothHidDevice)inputHost;
            var callback = new HidDeviceAppCallback(this);
            var executor = Executors.NewFixedThreadPool(1);
            return _inputHost.RegisterApp(Constants.SDP_RECORD_FIDO, null, Constants.QOS_OUT_FIDO, executor, callback);
        }

        public void RegisterDeviceListener(IDeviceStateListener deviceStateListener)
        {
            _deviceStateListener = deviceStateListener;
        }

        public void UnregisterDeviceListener(IDeviceStateListener deviceStateListener)
        {
            deviceStateListener = null;
        }

        public void OnConnectionStateChanged(BluetoothDevice device, int state)
        {
            if(_deviceStateListener != null)
            {
                _deviceStateListener.OnConnectionStateChanged(device, state);
            }
        }

        public void OnAppStatusChanged(bool registered)
        {
            if(_deviceStateListener != null)
            {
                _deviceStateListener.OnStatusChanged(registered);
            }
        }

        public void OnInterruptData(BluetoothDevice device, byte reportId, byte[] data, BluetoothHidDevice inputHost)
        {
            if(_deviceStateListener != null)
            {
                _deviceStateListener.OnInterruptData(device, reportId, data, inputHost);
            }
        }

        public bool ReplyReport(BluetoothDevice dervice, byte type, byte id)
        {
            return false;
        }
    }

    public class HidDeviceAppCallback : BluetoothHidDevice.Callback
    {
        private readonly HidDeviceApp _hidDeviceApp;

        public HidDeviceAppCallback(HidDeviceApp hidDeviceApp)
        {
            _hidDeviceApp = hidDeviceApp;
        }

        public override void OnAppStatusChanged(BluetoothDevice pluggedDevice, bool registered)
        {
            base.OnAppStatusChanged(pluggedDevice, registered);
            _hidDeviceApp.Registered = registered;
            _hidDeviceApp.OnAppStatusChanged(registered);
        }

        public override void OnConnectionStateChanged(BluetoothDevice device, [GeneratedEnum] ProfileState state)
        {
            base.OnConnectionStateChanged(device, state);
            _hidDeviceApp.OnConnectionStateChanged(device, (int)state);
        }

        public override void OnGetReport(BluetoothDevice device, sbyte type, sbyte id, int bufferSize)
        {
            base.OnGetReport(device, type, id, bufferSize);
            if(_hidDeviceApp.InputHost != null)
            {
                if(type != BluetoothHidDevice.ReportTypeInput)
                {
                    _hidDeviceApp.InputHost.ReportError(device, BluetoothHidDevice.ErrorRspUnsupportedReq);
                } 
                else
                {
                    _hidDeviceApp.InputHost.ReportError(device, BluetoothHidDevice.ErrorRspInvalidRptId);
                }
            }
        }

        public override void OnInterruptData(BluetoothDevice device, sbyte reportId, byte[] data)
        {
            base.OnInterruptData(device, reportId, data);
        }
    }
}
