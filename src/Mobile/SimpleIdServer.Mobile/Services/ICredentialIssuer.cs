using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.Mobile.Services;

public interface ICredentialIssuer
{
    string Version { get; }
    Task<CredentialIssuerResult> Issue(CancellationToken cancellationToken);
}

public record CredentialIssuerResult
{
    private CredentialIssuerResult()
    {
        
    }

    public CredentialStatus Status { get; private set; }
    public W3CVerifiableCredential Credential { get; private set; }

    public static CredentialIssuerResult Issue(W3CVerifiableCredential credential) => new CredentialIssuerResult { Credential = credential, Status = CredentialStatus.ISSUED };
    public static CredentialIssuerResult Pending() => new CredentialIssuerResult { Status = CredentialStatus.PENDING };
}

public enum CredentialStatus
{
    ISSUED = 0,
    PENDING = 1
}
