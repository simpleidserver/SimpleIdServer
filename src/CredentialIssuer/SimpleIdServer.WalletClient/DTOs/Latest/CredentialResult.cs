using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.Latest;

public class CredentialResult : BaseCredentialResult
{
    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; }

    public override string GetTransactionId()
        => TransactionId;
}
