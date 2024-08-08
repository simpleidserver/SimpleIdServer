using SimpleIdServer.Mobile.Clients;
using SimpleIdServer.Mobile.DTOs;
using SimpleIdServer.Mobile.DTOs.Latest;
using System.Text.Json;

namespace SimpleIdServer.Mobile.Services;

public class VerifiableCredentialService : BaseGenericVerifiableCredentialService<CredentialOffer>
{
    public VerifiableCredentialService(
        ICredentialIssuerClient credentialIssuerClient, 
        ISidServerClient sidServerClient) : base(credentialIssuerClient, sidServerClient)
    {
    }

    public override string Version
    {
        get
        {
            return SupportedVcVersions.LATEST;
        }
    }

    protected async override Task<(BaseCredentialDefinitionResult credDef, BaseCredentialIssuer credIssuer)?> Extract(CredentialOffer credentialOffer)
    {
        var credentialIssuer = await CredentialIssuerClient.GetCredentialIssuer<CredentialIssuerResult>(credentialOffer);
        var serializedCredentialDef = credentialIssuer.CredentialsConfigurationsSupported
            .SingleOrDefault(kvp => kvp.Key == credentialOffer.CredentialConfigurationIds.Single()).Value?.ToJsonString();
        if (serializedCredentialDef == null) return null;
        var credentialDef = JsonSerializer.Deserialize<CredentialDefinitionResult>(serializedCredentialDef);
        return (credentialDef, credentialIssuer);
    }

    protected override async Task<BaseCredentialResult> GetCredential(BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, CredentialProofRequest proofRequest, string accessToken)
    {
        var credentialRequest = new GetCredentialRequest
        {
            Format = credentialDefinition.Format,
            CredentialDefinitionRequest = new CredentialDefinitionRequest
            {
                Type = credentialDefinition.GetTypes()
            },
            Proof = proofRequest
        };
        var result = await CredentialIssuerClient.GetCredential<GetCredentialRequest, CredentialResult>(credentialIssuer.CredentialEndpoint, credentialRequest, accessToken);
        return result;
    }
}