using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Mobile.Clients;

public interface ISidServerClient
{
    Task<string> AddGotifyConnection();
    Task<(string accessToken, string cNonce)?> GetAccessTokenWithPreAuthorizedCode(string authorizationServer, string preAuthorizedCode);
}

public class SidServerClient : ISidServerClient
{
    private readonly Factories.IHttpClientFactory _httpClientFactory;
    private readonly MobileOptions _mobileOptions;

    public SidServerClient(Factories.IHttpClientFactory httpClientFactory, IOptions<MobileOptions> mobileOptions)
    {
        _httpClientFactory = httpClientFactory;
        _mobileOptions = mobileOptions.Value;
    }

    public async Task<string> AddGotifyConnection()
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var msg = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_mobileOptions.IdServerUrl}/gotifyconnections"),
                Content = new StringContent("{}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };
            var httpResult = await httpClient.SendAsync(msg);
            var content = await httpResult.Content.ReadAsStringAsync();
            var jObj = JsonObject.Parse(content);
            var pushToken = jObj["token"].ToString();
            return pushToken;
        }
    }

    public async Task<(string accessToken, string cNonce)?> GetAccessTokenWithPreAuthorizedCode(string authorizationServer, string preAuthorizedCode)
    {
        using (var httpClient = _httpClientFactory.Build())
        {
            var dic = new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:pre-authorized_code" },
                { "client_id", _mobileOptions.ClientId },
                { "client_secret", _mobileOptions.ClientSecret },
                { "pre-authorized_code", preAuthorizedCode }
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(dic),
                RequestUri = new Uri($"{authorizationServer}/token")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            httpResult.EnsureSuccessStatusCode();
            var json = await httpResult.Content.ReadAsStringAsync();
            var jsonObj = JsonObject.Parse(json).AsObject();
            var accessToken = jsonObj["access_token"];
            var cNonce = string.Empty;
            if (jsonObj.ContainsKey("c_nonce")) cNonce = jsonObj["c_nonce"].ToString();
            return (accessToken.ToString(), cNonce);
        }
    }
}
