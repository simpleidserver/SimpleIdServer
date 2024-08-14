using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.Latest; 

public class CredentialOffer : BaseCredentialOffer
{
    [JsonPropertyName("credential_configuration_ids")]
    public ICollection<string> CredentialConfigurationIds { get; set; } = new List<string>();

    public override List<string> CredentialIds => CredentialConfigurationIds?.ToList() ?? new List<string>();

    public override string Version => SupportedVcVersions.LATEST;

    public override bool HasOneCredential()
    {
        return CredentialConfigurationIds != null && CredentialConfigurationIds.Count() == 1;
    }
}
