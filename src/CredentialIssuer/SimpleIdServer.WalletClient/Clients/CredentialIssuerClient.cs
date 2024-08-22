using SimpleIdServer.Vp.Models;
using SimpleIdServer.WalletClient.DTOs;
using SimpleIdServer.WalletClient.DTOs.ESBI;
using SimpleIdServer.WalletClient.DTOs.Latest;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.WalletClient.Clients;

public interface ICredentialIssuerClient
{
    Task<T> GetCredentialOffer<T>(string url, CancellationToken cancellationToken) where T : BaseCredentialOffer;
    Task<T> GetCredentialIssuer<T>(BaseCredentialOffer credentialOffer, CancellationToken cancellationToken) where T : BaseCredentialIssuer;
    Task<T> GetCredentialIssuer<T>(string url, CancellationToken cancellationToken) where T : BaseCredentialIssuer;
    Task<R> GetCredential<T, R>(string url, T request, string accessToken, CancellationToken cancellationToken) where T : BaseCredentialRequest where R : BaseCredentialResult;
    Task<DeferredCredentialResult<ESBICredentialResult>> GetEsbiDeferredCredential(string url, string acceptanceToken, CancellationToken cancellationToken);
    Task<DeferredCredentialResult<CredentialResult>> GetDeferredCredential(string url, string transactionId, CancellationToken cancellationToken);
    Task<VerifiablePresentationDefinition> GetVerifiablePresentationDefinition(string url, CancellationToken cancellationToken);
}

public class CredentialIssuerClient : ICredentialIssuerClient
{
    private readonly Factories.IHttpClientFactory _httpClientFactory;

    public CredentialIssuerClient(Factories.IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T> GetCredentialOffer<T>(string url, CancellationToken cancellationToken) where T : BaseCredentialOffer
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content);
        }
    }

    public Task<T> GetCredentialIssuer<T>(BaseCredentialOffer credentialOffer, CancellationToken cancellationToken) where T : BaseCredentialIssuer
        => GetCredentialIssuer<T>($"{credentialOffer.CredentialIssuer}/.well-known/openid-credential-issuer", cancellationToken);

    public async Task<T> GetCredentialIssuer<T>(string url, CancellationToken cancellationToken) where T : BaseCredentialIssuer
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(content);
        }
    }

    public async Task<R> GetCredential<T, R>(string url, T request, string accessToken, CancellationToken cancellationToken) where T : BaseCredentialRequest where R : BaseCredentialResult
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<R>(content);
        }
    }

    public async Task<DeferredCredentialResult<ESBICredentialResult>> GetEsbiDeferredCredential(string url, string acceptanceToken, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url)
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {acceptanceToken}");
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            if(!httpResult.IsSuccessStatusCode)
            {
                var errorResult = JsonSerializer.Deserialize<ErrorResult>(content);
                return new DeferredCredentialResult<ESBICredentialResult> { ErrorMessage = errorResult?.ErrorDescription };
            }

            return new DeferredCredentialResult<ESBICredentialResult> { VerifiableCredential = JsonSerializer.Deserialize<ESBICredentialResult>(content) };
        }
    }

    public async Task<DeferredCredentialResult<CredentialResult>> GetDeferredCredential(string url, string transactionId, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonSerializer.Serialize(new GetDeferredCredentialRequest { TransactionId = transactionId }), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            if (!httpResult.IsSuccessStatusCode)
            {
                var errorResult = JsonSerializer.Deserialize<ErrorResult>(content);
                return new DeferredCredentialResult<CredentialResult> { ErrorMessage = errorResult?.ErrorDescription, ErrorCode = errorResult?.Error };
            }

            return new DeferredCredentialResult<CredentialResult> { VerifiableCredential = JsonSerializer.Deserialize<CredentialResult>(content) };
        }
    }

    public async Task<VerifiablePresentationDefinition> GetVerifiablePresentationDefinition(string url, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<VerifiablePresentationDefinition>(content);
        }
    }
}