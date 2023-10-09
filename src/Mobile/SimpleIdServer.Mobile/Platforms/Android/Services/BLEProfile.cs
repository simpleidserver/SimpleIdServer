using Android.Bluetooth;
using Android.Bluetooth.LE;
using AndroidContent = Android.Content;
namespace SimpleIdServer.Mobile.Platforms.Android.Services;

public class BLEProfile
{
    private readonly IEnumerable<IGattService> _services = new List<IGattService>
    {
        new DeviceInformationService(),
        new FIDO2AuthenticatorService()
    };

    private readonly BluetoothManager _bluetoothManager;

    public BLEProfile(AndroidContent.Context context)
    {
        context = context.ApplicationContext;
        _bluetoothManager = context.GetSystemService(AndroidContent.Context.BluetoothService) as BluetoothManager;
    }

    public BluetoothAdapter Adapter
    {
        get
        {
            return _bluetoothManager.Adapter;
        }
    }

    public BluetoothLeAdvertiser Advertiser
    {
        get
        {
            return Adapter.BluetoothLeAdvertiser;
        }
    }

    public void Start()
    {
        // TODO
    }

    public void StartAdvertising()
    {

    }

    public void StopAdvertising()
    {

    }
}
