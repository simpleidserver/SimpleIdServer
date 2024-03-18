using SimpleIdServer.Mobile.Models;
using System.Collections.ObjectModel;

namespace SimpleIdServer.Mobile.Stores;

public class VerifiableCredentialListState
{
    private readonly MobileDatabase _database;

    public VerifiableCredentialListState()
    {
        _database = App.Database;
    }

    public ObservableCollection<VerifiableCredentialRecord> VerifiableCredentialRecords { get; set; } = new ObservableCollection<VerifiableCredentialRecord>();

    public async Task Load()
    {
        var vcLst = await _database.GetVerifiableCredentials();
        foreach (var vc in vcLst) VerifiableCredentialRecords.Add(vc);
    }

    public async Task AddVerifiableCredentialRecord(VerifiableCredentialRecord verifiableCredential)
    {
        await _database.AddVerifiableCredential(verifiableCredential);
        VerifiableCredentialRecords.Add(verifiableCredential);
    }
}