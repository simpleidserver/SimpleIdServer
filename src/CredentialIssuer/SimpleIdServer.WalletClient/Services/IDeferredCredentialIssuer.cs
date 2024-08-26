using Org.BouncyCastle.Bcpg;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.WalletClient.CredentialFormats;
using SimpleIdServer.WalletClient.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public interface IDeferredCredentialIssuer
{
    string Version { get; }
    Task<(CredentialIssuerResult credentialIssuer, string errorMessage)> Issue(ICredentialFormatter formatter, BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, string transactionId, CancellationToken cancellationToken);
}

public record CredentialIssuerResult
{
    private CredentialIssuerResult()
    {
        
    }

    public CredentialStatus Status { get; private set; } 
    public BaseCredentialResult Credential { get; private set; }
    public W3CVerifiableCredential W3CCredential { get; private set; }
    public BaseCredentialDefinitionResult CredentialDef { get; private set; }
    public string SerializedVc { get; private set; }
    public string ResponseType { get; private set; }

    public static CredentialIssuerResult Issue(BaseCredentialResult credential, W3CVerifiableCredential w3cCredential, BaseCredentialDefinitionResult credDef, string serializedVc) => new CredentialIssuerResult { Credential = credential, W3CCredential = w3cCredential, Status = CredentialStatus.ISSUED, CredentialDef = credDef, SerializedVc = serializedVc };
    public static CredentialIssuerResult Pending() => new CredentialIssuerResult { Status = CredentialStatus.PENDING };
}

public enum CredentialStatus
{
    ISSUED = 0,
    PENDING = 1,
    ERROR = 2,
    VP_PRESENTED = 3
}