using SimpleIdServer.Did.Crypto;
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
    Task<RequestVerifiableCredentialResult> ResolveByUrl(string credentialOfferUrl, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken);
    Task<RequestVerifiableCredentialResult> Resolve(string credentialOffer, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken);
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

    public async Task<RequestVerifiableCredentialResult> ResolveByUrl(string credentialOfferUrl, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new System.Uri(credentialOfferUrl)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return await Resolve(content, publicDid, privateKey, cancellationToken);
        }
    }

    public Task<RequestVerifiableCredentialResult> Resolve(string credentialOffer, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken)
    {
        var version = SupportedVcVersions.LATEST;
        var jsonObj = JsonObject.Parse(credentialOffer).AsObject();
        if(jsonObj.ContainsKey("credentials")) version = SupportedVcVersions.ESBI;
        var service = _verifiableCredentialsServices.Single(v => v.Version ==  version);
        return service.Request(credentialOffer, publicDid, privateKey, cancellationToken);        
    }
}