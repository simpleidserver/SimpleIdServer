
using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.DTOs;
using SimpleIdServer.WalletClient.DTOs.ESBI;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public class ESBIDeferredCredentialIssuer : GenericDeferredCredentialIssuer<ESBICredentialResult>
{
    private readonly ICredentialIssuerClient _credentialIssuerClient;

    public ESBIDeferredCredentialIssuer(ICredentialIssuerClient credentialIssuerClient)
    {
        _credentialIssuerClient = credentialIssuerClient;
    }
    public override string Version => SupportedVcVersions.ESBI;

    protected override Task<DeferredCredentialResult<ESBICredentialResult>> GetDeferredCredential(BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken)
        => _credentialIssuerClient.GetEsbiDeferredCredential(credentialIssuer.DeferredCredentialEndpoint, transactionId, cancellationToken);
}
