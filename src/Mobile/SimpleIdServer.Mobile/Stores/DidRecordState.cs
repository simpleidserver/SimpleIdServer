using SimpleIdServer.Mobile.Models;
using System.Collections.ObjectModel;

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
        var didRecords = await _mobileDatabase.GetDidRecords();
        foreach(var didRecord in didRecords) Dids.Add(didRecord);
        ActiveDid = didRecords.FirstOrDefault(d => d.IsActive);
    }

    public async Task Add(DidRecord did)
    {
        await _mobileDatabase.AddDidRecord(did);
        Dids.Add(did);
    }

    public async Task Delete(DidRecord did)
    {
        await _mobileDatabase.RemoveDidRecord(did);
        Dids.Remove(did);
    }

    public async Task Update(IEnumerable<DidRecord> didRecords)
    {
        await _mobileDatabase.UpdateDidRecords(didRecords);
    }

    public ObservableCollection<DidRecord> Dids { get; set; } = new ObservableCollection<DidRecord>();
    public DidRecord ActiveDid { get; set; }
}
