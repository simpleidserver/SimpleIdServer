using SimpleIdServer.Mobile.Models;
using System.Collections.ObjectModel;

namespace SimpleIdServer.Mobile.Stores;

public class CredentialListState
{
    private readonly MobileDatabase _database;

    public CredentialListState()
    {
        _database = App.Database;
    }

    public ObservableCollection<SelectableCredentialRecord> CredentialRecords { get; set; } = new ObservableCollection<SelectableCredentialRecord>();

    public async Task Load()
    {
        var credentialsRecords = await _database.GetCredentialRecords();
        foreach (var credentialRecord in credentialsRecords) CredentialRecords.Add(new SelectableCredentialRecord(credentialRecord));
    }

    public async Task Remove()
    {
        var credentials = CredentialRecords.Where(r => r.IsSelected);
        foreach (var credential in credentials.OrderByDescending(c => c.Credential.Id))
        {
            var index = CredentialRecords.IndexOf(credential);
            CredentialRecords.RemoveAt(index);
            await _database.RemoveCredentialRecord(credential.Credential);
        }
    }

    public async Task AddCredentialRecord(CredentialRecord credentialRecord)
    {
        await _database.AddCredentialRecord(credentialRecord);
        CredentialRecords.Add(new SelectableCredentialRecord(credentialRecord));
    }
}

public class SelectableCredentialRecord
{
    public SelectableCredentialRecord(CredentialRecord credential)
    {
        Credential = credential;
    }

    public bool IsSelected { get; set; }
    public CredentialRecord Credential { get; set; }
}
