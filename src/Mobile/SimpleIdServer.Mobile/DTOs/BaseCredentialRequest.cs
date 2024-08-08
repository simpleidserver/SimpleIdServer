using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class BaseCredentialRequest
{
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("proof")]
    public CredentialProofRequest Proof { get; set; }
}

public class CredentialProofRequest
{
    [JsonPropertyName("proof_type")]
    public string ProofType { get; set; }
    [JsonPropertyName("jwt")]
    public string Jwt { get; set; }
}
