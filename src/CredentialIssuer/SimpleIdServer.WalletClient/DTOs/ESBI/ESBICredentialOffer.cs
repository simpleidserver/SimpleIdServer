using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.ESBI;

public class ESBICredentialOffer : BaseCredentialOffer
{
    [JsonPropertyName("credentials")]
    public List<ESBICredential> Credentials { get; set; }

    public override List<string> CredentialIds => Credentials?.Select(c => $"{string.Join('\n', c.Types)}")?.ToList() ?? new List<string>();

    public override string Version => SupportedVcVersions.ESBI;

    public override bool HasOneCredential()
    {
        return Credentials != null && Credentials.Count() == 1;
    }
}

public class ESBICredential
{
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }
}