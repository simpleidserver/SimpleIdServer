using SimpleIdServer.Mobile.Models;

namespace SimpleIdServer.Mobile.Stores;

public class DidRecordState
{
    private readonly MobileDatabase _mobileDatabase;

    public DidRecordState()
    {
        _mobileDatabase = App.Database;
    }

    public async Task Load()
    {
        Did = await _mobileDatabase.GetDidRecord();
    }

    public async Task Update(DidRecord did)
    {
        await _mobileDatabase.AddDidRecord(did);
        Did = did;
    }

    public DidRecord Did { get; set; }
}
