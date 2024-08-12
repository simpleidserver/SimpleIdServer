using SimpleIdServer.WalletClient.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.WalletClient.Clients;

public interface ISidServerClient
{
    Task<OpenidConfigurationResult> GetOpenidConfiguration(string url, CancellationToken cancellationToken);
    Task<Dictionary<string, string>> GetAuthorization(string authEdp, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    Task<Dictionary<string, string>> PostAuthorizationRequest(string url, string idToken, string state, CancellationToken cancellationToken);
    Task<TokenResult> GetAccessTokenWithPreAuthorizedCode(string clientId, string tokenEndpoint, string preAuthorizedCode, CancellationToken cancellationToken);
    Task<TokenResult> GetAccessTokenWithAuthorizationCode(string url, string clientId, string code, string codeVerifier, CancellationToken cancellationToken);
}

public class SidServerClient : ISidServerClient
{
    private readonly Factories.IHttpClientFactory _httpClientFactory;

    public SidServerClient(Factories.IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<OpenidConfigurationResult> GetOpenidConfiguration(string url, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var result = await httpClient.GetFromJsonAsync<OpenidConfigurationResult>($"{url}/.well-known/openid-configuration", cancellationToken);
            return result;
        }
    }

    public async Task<Dictionary<string, string>> GetAuthorization(string authEdp, Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var uriBuilder = new UriBuilder(authEdp);
        uriBuilder.Query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = uriBuilder.Uri
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            return ExtractQueryParameters(httpResult);
        }
    }

    public async Task<Dictionary<string, string>> PostAuthorizationRequest(string url, string idToken, string state, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(HttpUtility.UrlDecode(url)),
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("id_token", idToken),
                    new KeyValuePair<string, string>("state", state)
                }),
                Method = HttpMethod.Post
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            return ExtractQueryParameters(httpResult);
        }
    }

    public async Task<TokenResult> GetAccessTokenWithPreAuthorizedCode(string clientId, string tokenEndpoint, string preAuthorizedCode, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var dic = new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:pre-authorized_code" },
                { "client_id", clientId },
                { "pre-authorized_code", preAuthorizedCode }
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(dic),
                RequestUri = new Uri(tokenEndpoint)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            if (!httpResult.IsSuccessStatusCode) return null;
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TokenResult>(json);
        }
    }

    public async Task<TokenResult> GetAccessTokenWithAuthorizationCode(string url, string clientId, string code, string codeVerifier, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("code_verifier", codeVerifier)
                }),
                Method = HttpMethod.Post
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            if (!httpResult.IsSuccessStatusCode) return null;
            var content = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<TokenResult>(content);
        }
    }

    private Dictionary<string, string> ExtractQueryParameters(HttpResponseMessage response)
    {
        var builder = new UriBuilder(response.Headers.Location.AbsoluteUri);
        return builder.Query.Trim('?').Split('&').Select(s => s.Split('=')).ToDictionary(arr => arr[0], arr => arr[1]);
    }
}
