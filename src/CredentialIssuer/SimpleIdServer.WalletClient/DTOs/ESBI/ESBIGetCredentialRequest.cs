using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.ESBI;

public class ESBIGetCredentialRequest : BaseCredentialRequest
{
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }
}