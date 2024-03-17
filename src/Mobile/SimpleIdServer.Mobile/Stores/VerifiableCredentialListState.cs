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

    public ObservableCollection<SelectableVerifiableCredentialRecord> VerifiableCredentialRecords { get; set; } = new ObservableCollection<SelectableVerifiableCredentialRecord>();

    public async Task AddVerifiableCredentialRecord(VerifiableCredentialRecord verifiableCredential)
    {
        await _database.AddVerifiableCredential(verifiableCredential);
        VerifiableCredentialRecords.Add(new SelectableVerifiableCredentialRecord(verifiableCredential));
    }
}

public class SelectableVerifiableCredentialRecord
{
    public SelectableVerifiableCredentialRecord(VerifiableCredentialRecord verifiableCredential)
    {
        VerifiableCredential = verifiableCredential;
    }

    public VerifiableCredentialRecord VerifiableCredential { get; set; }
    public bool IsSelected { get; set; }
}