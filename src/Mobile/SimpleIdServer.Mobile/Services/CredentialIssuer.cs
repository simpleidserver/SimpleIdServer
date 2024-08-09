
using SimpleIdServer.Mobile.DTOs;

namespace SimpleIdServer.Mobile.Services;

public class CredentialIssuer : ICredentialIssuer
{
    public List<string> Versions => throw new NotImplementedException();

    public Task<CredentialIssuerResult> Issue(BaseCredentialIssuer credentialIssuer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
