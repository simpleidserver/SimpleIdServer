namespace SimpleIdServer.Mobile.Helpers;

public interface IPlatformHelpers
{
    Task<PermissionStatus> CheckAndRequestBluetoothPermissions();
}
