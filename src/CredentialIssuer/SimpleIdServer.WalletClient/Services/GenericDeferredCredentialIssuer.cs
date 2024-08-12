using SimpleIdServer.Vc.Models;
using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.DTOs;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public abstract class GenericDeferredCredentialIssuer<T> : IDeferredCredentialIssuer where T : BaseCredentialResult
{
    public abstract string Version { get; }

    public async Task<(CredentialIssuerResult credentialIssuer, string errorMessage)> Issue(BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken)
    {
        var result = await GetDeferredCredential(credentialIssuer, transactionId, cancellationToken);
        if (result.ErrorCode == "issuance_pending") return (CredentialIssuerResult.Pending(), null);
        if (!string.IsNullOrWhiteSpace(result.ErrorMessage)) return (null, result.ErrorMessage);
        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(result.VerifiableCredential.Credential);
        return (CredentialIssuerResult.Issue(credential), null);
    }

    protected abstract Task<DeferredCredentialResult<T>> GetDeferredCredential(BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken);
}
