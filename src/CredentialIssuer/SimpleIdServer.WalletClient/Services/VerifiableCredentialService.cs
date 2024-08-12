using SimpleIdServer.Did;
using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.DTOs;
using SimpleIdServer.WalletClient.DTOs.Latest;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public class VerifiableCredentialService : BaseGenericVerifiableCredentialService<CredentialOffer>
{
    public VerifiableCredentialService(
        ICredentialIssuerClient credentialIssuerClient, 
        ISidServerClient sidServerClient,
        IEnumerable<IDidResolver> resolvers) : base(credentialIssuerClient, sidServerClient, resolvers)
    {
    }

    public override string Version
    {
        get
        {
            return SupportedVcVersions.LATEST;
        }
    }

    protected async override Task<(BaseCredentialDefinitionResult credDef, DTOs.BaseCredentialIssuer credIssuer)?> Extract(CredentialOffer credentialOffer, CancellationToken cancellationToken)
    {
        var credentialIssuer = await CredentialIssuerClient.GetCredentialIssuer<DTOs.Latest.CredentialIssuerResult>(credentialOffer, cancellationToken);
        var serializedCredentialDef = credentialIssuer.CredentialsConfigurationsSupported
            .SingleOrDefault(kvp => kvp.Key == credentialOffer.CredentialConfigurationIds.Single()).Value?.ToJsonString();
        if (serializedCredentialDef == null) return null;
        var credentialDef = JsonSerializer.Deserialize<CredentialDefinitionResult>(serializedCredentialDef);
        return (credentialDef, credentialIssuer);
    }

    protected override async Task<BaseCredentialResult> GetCredential(DTOs.BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, CredentialProofRequest proofRequest, string accessToken, CancellationToken cancellationToken)
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
        var result = await CredentialIssuerClient.GetCredential<GetCredentialRequest, CredentialResult>(credentialIssuer.CredentialEndpoint, credentialRequest, accessToken, cancellationToken);
        return result;
    }
}