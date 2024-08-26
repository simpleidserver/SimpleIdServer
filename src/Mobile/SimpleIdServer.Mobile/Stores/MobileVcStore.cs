using SimpleIdServer.WalletClient;
using SimpleIdServer.WalletClient.Stores;

namespace SimpleIdServer.Mobile.Stores;

public class MobileVcStore : IVcStore
{
    private readonly VerifiableCredentialListState _verifiableCredentialListState;

    public MobileVcStore(VerifiableCredentialListState verifiableCredentialListState)
    {
        _verifiableCredentialListState = verifiableCredentialListState;
    }

    public Task<List<StoredVcRecord>> GetAll(CancellationToken cancellationToken)
    {
        var result = _verifiableCredentialListState.VerifiableCredentialRecords.Select(r => new StoredVcRecord
        {
            Format = r.Format,
            SerializedVc = r.SerializedVc
        }).ToList();
        return Task.FromResult(result);
    }
}
