using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.ESBI;

public class ESBICredentialResult  : BaseCredentialResult
{
    [JsonPropertyName("acceptance_token")]
    public string AcceptanceToken { get; set; }

    public override string GetTransactionId()
        => AcceptanceToken;
}