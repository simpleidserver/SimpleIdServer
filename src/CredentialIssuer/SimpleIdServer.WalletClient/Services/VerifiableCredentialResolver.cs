using SimpleIdServer.Did.Crypto;
using SimpleIdServer.WalletClient.DTOs.Latest;
using SimpleIdServer.WalletClient.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Services;

public interface IVerifiableCredentialResolver
{
    Task<RequestVerifiableCredentialResult> Resolve(Dictionary<string, string> parameters, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken);
    Task<RequestVerifiableCredentialResult> Resolve(Dictionary<string, string> parameters, string publicDid, IAsymmetricKey privateKey, string pin, CancellationToken cancellationToken);
    Task<(IVerifiableCredentialsService service, string credentialOffer)> BuildService(Dictionary<string, string> parameters, CancellationToken cancellationToken);
}

public class VerifiableCredentialResolver : IVerifiableCredentialResolver
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<IVerifiableCredentialsService> _verifiableCredentialsServices;
    
    public VerifiableCredentialResolver(IHttpClientFactory httpClientFactory, IEnumerable<IVerifiableCredentialsService> verifiableCredentialsServices)
    {
        _httpClientFactory = httpClientFactory;
        _verifiableCredentialsServices = verifiableCredentialsServices;
    }

    public Task<RequestVerifiableCredentialResult> Resolve(Dictionary<string, string> parameters, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken)
        => Resolve(parameters, publicDid, privateKey, null, cancellationToken);

    public async Task<RequestVerifiableCredentialResult> Resolve(Dictionary<string, string> parameters, string publicDid, IAsymmetricKey privateKey, string pin, CancellationToken cancellationToken)
    {
        var record = await BuildService(parameters, cancellationToken);
        return await record.service.Request(record.credentialOffer, publicDid, privateKey, pin, cancellationToken);
    }

    public async Task<(IVerifiableCredentialsService service, string credentialOffer)> BuildService(Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var credentialOffer = string.Empty;
        if (parameters.ContainsKey("credential_offer_uri"))
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new System.Uri(parameters["credential_offer_uri"])
                };
                var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
                credentialOffer = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            }
        }
        else credentialOffer = parameters["credential_offer"];
        var version = SupportedVcVersions.LATEST;
        var jsonObj = JsonObject.Parse(credentialOffer).AsObject();
        if (jsonObj.ContainsKey("credentials")) version = SupportedVcVersions.ESBI;
        var service = _verifiableCredentialsServices.Single(v => v.Version == version);
        return (service, credentialOffer);
    }
}