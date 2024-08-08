using SimpleIdServer.Mobile.DTOs;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.Mobile.Clients;

public interface ICredentialIssuerClient
{
    Task<T> GetCredentialOffer<T>(string url) where T : BaseCredentialOffer;
    Task<T> GetCredentialIssuer<T>(BaseCredentialOffer credentialOffer) where T : BaseCredentialIssuer;
    Task<T> GetCredentialIssuer<T>(string url) where T : BaseCredentialIssuer;
    Task<R> GetCredential<T, R>(string url, T request, string accessToken) where T : BaseCredentialRequest where R : BaseCredentialResult;
}

public class CredentialIssuerClient : ICredentialIssuerClient
{
    private readonly Factories.IHttpClientFactory _httpClientFactory;

    public CredentialIssuerClient(Factories.IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T> GetCredentialOffer<T>(string url) where T : BaseCredentialOffer
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var content = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }
    }

    public Task<T> GetCredentialIssuer<T>(BaseCredentialOffer credentialOffer) where T : BaseCredentialIssuer
        => GetCredentialIssuer<T>($"{credentialOffer.CredentialIssuer}/.well-known/openid-credential-issuer");

    public async Task<T> GetCredentialIssuer<T>(string url) where T : BaseCredentialIssuer
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var content = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }
    }

    public async Task<R> GetCredential<T, R>(string url, T request, string accessToken) where T : BaseCredentialRequest where R : BaseCredentialResult
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
            var httpResult = await httpClient.SendAsync(requestMessage);
            var content = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<R>(content);
        }
    }
}