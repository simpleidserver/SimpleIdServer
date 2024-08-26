using SimpleIdServer.WalletClient.DTOs;

namespace SimpleIdServer.WalletClient.Clients;

public class DeferredCredentialResult<R> where R : BaseCredentialResult
{
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public R VerifiableCredential { get; set; }
}
