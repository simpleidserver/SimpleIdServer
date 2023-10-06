using Android.Bluetooth;

namespace SimpleIdServer.Mobile.Platforms.Android.Services
{
    public static class Constants
    {
        public static int HID_REPORT_SIZE = 64;

        public static byte[] HID_REPORT_DESC_FIDO = 
        {
            // FIDO
            (byte)0x06, (byte)0xD0, (byte)0xF1,   // Usage Page (FIDO_USAGE_PAGE, 2 bytes)
            (byte)0x09, (byte)0x01,               // Usage (FIDO_USAGE_U2FHID)
            (byte)0xA1, (byte)0x01,               // Collection (Application)
            (byte)0x09, (byte)0x20,               // Usage (FIDO_USAGE_DATA_IN)
            (byte)0x15, (byte)0x00,               // Logical Minimum (0)
            (byte)0x26, (byte)0xFF, (byte)0x00,   // Logical Maximum (255, 2 bytes)
            (byte)0x75, (byte)0x08,               // Report Size (8)
            (byte)0x95, (byte)HID_REPORT_SIZE,    // Report Count (variable)
            (byte)0x81, (byte)0x02,               // Input (Data, Absolute, Variable)
            (byte)0x09, (byte)0x21,               // Usage (FIDO_USAGE_DATA_OUT)
            (byte)0x15, (byte)0x00,               // Logical Minimum (0)
            (byte)0x26, (byte)0xFF, (byte)0x00,   // Logical Maximum (255, 2 bytes)
            (byte)0x75, (byte)0x08,               // Report Size (8)
            (byte)0x95, (byte)HID_REPORT_SIZE,    // Report Count (variable)
            (byte)0x91, (byte)0x02,               // Output (Data, Absolute, Variable)
            (byte)0xC0                            // End Collection
        };


        public static string SDP_NAME_FIDO = "Sid Security Key";
        public static string SDP_DESCRIPTION_FIDO = "FIDO2/U2F Android OS Security Key";
        public static string SDP_PROVIDER_FIDO = "https://github.com/simpleidserver";
        public static int QOS_TOKEN_RATE_FIDO = 1000;
        public static int QOS_TOKEN_BUCKET_SIZE_FIDO = HID_REPORT_SIZE + 1;
        public static int QOS_PEAK_BANDWIDTH_FIDO = 2000;
        public static int QOS_LATENCY_FIDO = 5000;

        public static BluetoothHidDeviceAppSdpSettings SDP_RECORD_FIDO =
            new BluetoothHidDeviceAppSdpSettings(
                    Constants.SDP_NAME_FIDO,
                    Constants.SDP_DESCRIPTION_FIDO,
                    Constants.SDP_PROVIDER_FIDO,
                    BluetoothHidDevice.Subclass2Uncategorized,
                    Constants.HID_REPORT_DESC_FIDO);

        public static BluetoothHidDeviceAppQosSettings QOS_OUT_FIDO =
            new BluetoothHidDeviceAppQosSettings(
                    HidDeviceAppQosSettingsServiceType.BestEffort,
                    Constants.QOS_TOKEN_RATE_FIDO,
                    Constants.QOS_TOKEN_BUCKET_SIZE_FIDO,
                    Constants.QOS_PEAK_BANDWIDTH_FIDO,
                    Constants.QOS_LATENCY_FIDO,
                    BluetoothHidDeviceAppQosSettings.Max);
    }
}
