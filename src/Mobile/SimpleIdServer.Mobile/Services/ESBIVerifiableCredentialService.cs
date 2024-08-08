using SimpleIdServer.Mobile.Clients;
using SimpleIdServer.Mobile.DTOs;
using SimpleIdServer.Mobile.DTOs.ESBI;

namespace SimpleIdServer.Mobile.Services;

public class ESBIVerifiableCredentialService : BaseGenericVerifiableCredentialService<ESBICredentialOffer>
{
    public ESBIVerifiableCredentialService(
        ICredentialIssuerClient credentialIssuerClient,
        ISidServerClient sidServerClient) : base(credentialIssuerClient, sidServerClient)
    {
    }

    public override string Version => SupportedVcVersions.ESBI;

    protected override async Task<(BaseCredentialDefinitionResult credDef, BaseCredentialIssuer credIssuer)?> Extract(ESBICredentialOffer credentialOffer)
    {
        var credentialIssuer = await CredentialIssuerClient.GetCredentialIssuer<ESBICredentialIssuer>(credentialOffer);
        var firstCredential = credentialOffer.Credentials.First();
        var credentialDef = credentialIssuer.CredentialsSupported.SingleOrDefault(c => c.Types.All(t => firstCredential.Types.Contains(t)));
        return (credentialDef, credentialIssuer);
    }

    protected override async Task<BaseCredentialResult> GetCredential(BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, CredentialProofRequest proofRequest, string accessToken)
    {
        var credentialRequest = new ESBIGetCredentialRequest
        {
            Format = credentialDefinition.Format,
            Types = credentialDefinition.GetTypes(),
            Proof = proofRequest
        };
        var result = await CredentialIssuerClient.GetCredential<ESBIGetCredentialRequest, ESBICredentialResult>(credentialIssuer.CredentialEndpoint, credentialRequest, accessToken);
        return result;
    }
}
