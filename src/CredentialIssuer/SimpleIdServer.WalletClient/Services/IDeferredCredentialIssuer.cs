using SimpleIdServer.Vc.Models;
using SimpleIdServer.WalletClient.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public interface IDeferredCredentialIssuer
{
    string Version { get; }
    Task<(CredentialIssuerResult credentialIssuer, string errorMessage)> Issue(BaseCredentialIssuer credentialIssuer, string transactionId, CancellationToken cancellationToken);
}

public record CredentialIssuerResult
{
    private CredentialIssuerResult()
    {
        
    }

    public CredentialStatus Status { get; private set; }
    public W3CVerifiableCredential Credential { get; private set; }
    public string ErrorMessage { get; private set; }

    public static CredentialIssuerResult Issue(W3CVerifiableCredential credential) => new CredentialIssuerResult { Credential = credential, Status = CredentialStatus.ISSUED };
    public static CredentialIssuerResult Pending() => new CredentialIssuerResult { Status = CredentialStatus.PENDING };
}

public enum CredentialStatus
{
    ISSUED = 0,
    PENDING = 1
}