using static Microsoft.Maui.ApplicationModel.Permissions;

namespace SimpleIdServer.Mobile.Helpers;

public class DroidPlatformHelpers : IPlatformHelpers
{
    public async Task<PermissionStatus> CheckAndRequestBluetoothPermissions()
    {
        PermissionStatus status;
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
        {
            status = await Permissions.CheckStatusAsync<BluetoothSPermission>();

            if (status == PermissionStatus.Granted)
                return status;

            status = await Permissions.RequestAsync<BluetoothSPermission>();
        }
        else
        {
            status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;


            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await Application.Current.MainPage.DisplayAlert("Permission required", "Location permission is required for bluetooth scanning. We do not store or use your location at all.", "OK");
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        return status;
    }

    public class BluetoothSPermission : BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.Bluetooth, true),
            (Android.Manifest.Permission.BluetoothAdmin, true),
            (Android.Manifest.Permission.AccessFineLocation, true),
            (Android.Manifest.Permission.AccessBackgroundLocation, true)
        }.ToArray();
    }
}
