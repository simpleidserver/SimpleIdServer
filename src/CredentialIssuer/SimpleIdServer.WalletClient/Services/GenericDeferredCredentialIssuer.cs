using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.CredentialFormats;
using SimpleIdServer.WalletClient.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public abstract class GenericDeferredCredentialIssuer<T> : IDeferredCredentialIssuer where T : BaseCredentialResult
{
    public abstract string Version { get; }

    public async Task<(CredentialIssuerResult credentialIssuer, string errorMessage)> Issue(ICredentialFormatter formatter, BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken)
    {
        var result = await GetDeferredCredential(credentialIssuer, transactionId, cancellationToken);
        if (HasPendingState(result)) return (CredentialIssuerResult.Pending(), null);
        if (!string.IsNullOrWhiteSpace(result.ErrorMessage)) return (null, result.ErrorMessage);
        var credential = formatter.Extract(result.VerifiableCredential.Credential.ToString());
        return (CredentialIssuerResult.Issue(credential), null);
    }

    protected abstract bool HasPendingState(DeferredCredentialResult<T> credential);

    protected abstract Task<DeferredCredentialResult<T>> GetDeferredCredential(BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken);
}
