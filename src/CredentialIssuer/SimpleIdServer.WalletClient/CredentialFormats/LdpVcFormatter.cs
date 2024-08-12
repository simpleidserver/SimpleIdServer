using SimpleIdServer.Vc.Models;
using System.Text.Json;

namespace SimpleIdServer.WalletClient.CredentialFormats;

public class LdpVcFormatter : ICredentialFormatter
{
    public string Format => "ldp_vc";

    public W3CVerifiableCredential Extract(string content)
    {
        return JsonSerializer.Deserialize<W3CVerifiableCredential>(content);
    }
}
