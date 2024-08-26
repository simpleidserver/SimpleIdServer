using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs;

public class GetDeferredCredentialRequest
{
    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; }
}
