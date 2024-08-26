using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.DTOs;
using SimpleIdServer.WalletClient.DTOs.Latest;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public class DeferredCredentialIssuer : GenericDeferredCredentialIssuer<CredentialResult>
{
    private readonly ICredentialIssuerClient _credentialIssuerClient;

    public DeferredCredentialIssuer(ICredentialIssuerClient credentialIssuerClient)
    {
        _credentialIssuerClient = credentialIssuerClient;
    }

    public override string Version => SupportedVcVersions.LATEST;

    protected override bool HasPendingState(DeferredCredentialResult<CredentialResult> credential)
    {
        return credential.ErrorCode == "issuance_pending";
    }

    protected override Task<DeferredCredentialResult<CredentialResult>> GetDeferredCredential(BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken)
        => _credentialIssuerClient.GetDeferredCredential(credentialIssuer.DeferredCredentialEndpoint, transactionId, cancellationToken);
}
