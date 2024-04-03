using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class CredentialOffer
    {
        [JsonPropertyName("credential_issuer")]
        public string CredentialIssuer { get; set; }
        [JsonPropertyName("credential_configuration_ids")]
        public ICollection<string> CredentialConfigurationIds { get; set; } = new List<string>();
        [JsonPropertyName("grants")]
        public CredentialOfferGrants Grants { get; set; } = null;
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
}
