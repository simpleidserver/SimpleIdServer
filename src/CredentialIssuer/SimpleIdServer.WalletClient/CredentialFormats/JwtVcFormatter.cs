using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Text.Json;

namespace SimpleIdServer.WalletClient.CredentialFormats;

public class JwtVcFormatter : ICredentialFormatter
{
    public string Format => "jwt_vc";

    public W3CVerifiableCredential Extract(string content)
    {
        var handler = new JsonWebTokenHandler();
        var jsonWebToken = handler.ReadJsonWebToken(content);
        var vcJson = jsonWebToken.Claims.Single(c => c.Type == "vc").Value;
        return JsonSerializer.Deserialize<W3CVerifiableCredential>(vcJson);
    }
}
