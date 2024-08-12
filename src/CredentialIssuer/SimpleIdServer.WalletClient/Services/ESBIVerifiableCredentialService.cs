using SimpleIdServer.Did;
using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.CredentialFormats;
using SimpleIdServer.WalletClient.DTOs;
using SimpleIdServer.WalletClient.DTOs.ESBI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public class ESBIVerifiableCredentialService : BaseGenericVerifiableCredentialService<ESBICredentialOffer>
{
    public ESBIVerifiableCredentialService(
        ICredentialIssuerClient credentialIssuerClient,
        ISidServerClient sidServerClient,
        IEnumerable<IDidResolver> resolvers,
        IEnumerable<IDeferredCredentialIssuer> issuers,
        IEnumerable<ICredentialFormatter> formatters) : base(credentialIssuerClient, sidServerClient, resolvers, issuers, formatters)
    {
    }

    public override string Version => SupportedVcVersions.ESBI;

    protected override async Task<(BaseCredentialDefinitionResult credDef, DTOs.BaseCredentialIssuer credIssuer)?> Extract(ESBICredentialOffer credentialOffer, CancellationToken cancellationToken)
    {
        var credentialIssuer = await CredentialIssuerClient.GetCredentialIssuer<ESBICredentialIssuer>(credentialOffer, cancellationToken);
        var firstCredential = credentialOffer.Credentials.First();
        var credentialDef = credentialIssuer.CredentialsSupported.SingleOrDefault(c => c.Types.All(t => firstCredential.Types.Contains(t)) && c.Format == firstCredential.Format);
        return (credentialDef, credentialIssuer);
    }

    protected override async Task<BaseCredentialResult> GetCredential(DTOs.BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, CredentialProofRequest proofRequest, string accessToken, CancellationToken cancellationToken)
    {
        var credentialRequest = new ESBIGetCredentialRequest
        {
            Format = credentialDefinition.Format,
            Types = credentialDefinition.GetTypes(),
            Proof = proofRequest
        };
        return await CredentialIssuerClient.GetCredential<ESBIGetCredentialRequest, ESBICredentialResult>(credentialIssuer.CredentialEndpoint, credentialRequest, accessToken, cancellationToken);
    }
}
