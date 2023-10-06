using Android.Bluetooth;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public class BluetoothDeviceWrapper
    {
        public BluetoothDeviceWrapper(BluetoothDevice device) : this(device, BluetoothDeviceListing.HID_UNKNOWN_HOST)
        {
            
        }

        public BluetoothDeviceWrapper(BluetoothDevice device, string type)
        {
            Device = device;
            Type = type;
            Name = device.Name;
            Adr = device.Address;
            var majorDeviceClass = device.BluetoothClass.MajorDeviceClass.ToString();
            var deviceClass = device.BluetoothClass.DeviceClass.ToString();
            Value = Name + Adr + majorDeviceClass + deviceClass;
        }

        public string Type { get; set; }
        public bool IsDefault { get; set; }
        public BluetoothDevice Device { get; private set; }
        public string Name { get; private set; }
        public string Adr { get; private set; }
        public string Value { get; private set; }

        public string Hash()
        {
            using (var sha256 = SHA256.Create())
            {
                var payload = Encoding.UTF8.GetBytes(Value);
                var result = sha256.ComputeHash(payload);
                return Convert.ToHexString(result);
            }
        }
    }
}