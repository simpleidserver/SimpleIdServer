using SimpleIdServer.Mobile.Models;

namespace SimpleIdServer.Mobile.Stores;

public class MobileSettingsState
{
    private readonly MobileDatabase _mobileDatabase;

    public MobileSettingsState()
    {
        _mobileDatabase = App.Database;
    }

    public async Task Load()
    {
        Settings = await _mobileDatabase.GetMobileSettings();
    }

    public async Task Update(MobileSettings mobileSettings)
    {
        await _mobileDatabase.UpdateMobileSettings(mobileSettings);
        Settings = mobileSettings;
    }

    public MobileSettings Settings { get; set; }
}
