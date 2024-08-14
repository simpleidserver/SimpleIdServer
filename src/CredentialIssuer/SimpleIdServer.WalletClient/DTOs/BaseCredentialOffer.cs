using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs;

public abstract class BaseCredentialOffer
{
    [JsonPropertyName("credential_issuer")]
    public string CredentialIssuer { get; set; }
    [JsonPropertyName("grants")]
    public CredentialOfferGrants Grants { get; set; }
    [JsonIgnore]
    public abstract string Version { get; }
    [JsonIgnore]
    public abstract List<string> CredentialIds { get; }

    public abstract bool HasOneCredential();
}


public class CredentialOfferGrants
{
    [JsonPropertyName("urn:ietf:params:oauth:grant-type:pre-authorized_code")]
    public PreAuthorizedCodeGrant PreAuthorizedCodeGrant { get; set; }
    [JsonPropertyName("authorization_code")]
    public AuthorizedCodeGrant AuthorizedCodeGrant { get; set; }
}

public class PreAuthorizedCodeGrant
{
    [JsonPropertyName("pre-authorized_code")]
    public string PreAuthorizedCode { get; set; }
    [JsonPropertyName("tx_code")]
    public PreAuthorizedCodeGrantTransaction Transaction { get; set; }
    [JsonPropertyName("interval")]
    public int? Interval { get; set; }
    [JsonPropertyName("authorization_server")]
    public string AuthorizationServer { get; set; }
}

public class AuthorizedCodeGrant
{
    [JsonPropertyName("issuer_state")]
    public string IssuerState { get; set; } = null;
    [JsonPropertyName("authorization_server")]
    public string AuthorizationServer { get; set; } = null;
}

public class PreAuthorizedCodeGrantTransaction
{
    [JsonPropertyName("input_mode")]
    public string InputMode { get; set; } = "numeric";
    [JsonPropertyName("length")]
    public int? Length { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
}