using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.WalletClient.CredentialFormats
{
    public interface ICredentialFormatter
    {
        string Format { get; }
        W3CVerifiableCredential Extract(string content);
        object DeserializeObject(string serializedVc);
    }
}
