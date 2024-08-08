using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs.Latest; 

public class CredentialOffer : BaseCredentialOffer
{
    [JsonPropertyName("credential_configuration_ids")]
    public ICollection<string> CredentialConfigurationIds { get; set; } = new List<string>();

    public override string Version => SupportedVcVersions.LATEST;

    public override bool HasOneCredential()
    {
        return CredentialConfigurationIds != null && CredentialConfigurationIds.Count() == 1;
    }
}
