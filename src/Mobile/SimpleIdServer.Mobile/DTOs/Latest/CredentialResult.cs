using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs.Latest;

public class CredentialResult : BaseCredentialResult
{
    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; }
}
