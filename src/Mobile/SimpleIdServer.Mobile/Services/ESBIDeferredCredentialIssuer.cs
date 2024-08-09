
using SimpleIdServer.Mobile.DTOs;

namespace SimpleIdServer.Mobile.Services;

public class ESBIDeferredCredentialIssuer : ICredentialIssuer
{
    public ESBIDeferredCredentialIssuer()
    {
        
    }

    public string Version => SupportedVcVersions.ESBI;

    public Task<CredentialIssuerResult> Issue(BaseCredentialIssuer credentialIssuer, CancellationToken cancellationToken)
    {
        credentialIssuer.DeferredCredentialEndpoint;

    }
}
