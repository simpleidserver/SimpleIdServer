// See https://aka.ms/new-console-template for more information
using SimpleIdServer.CredentialIssuer.Api.CredentialIssuer;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using static QRCoder.PayloadGenerator;

using (var httpClient = new HttpClient())
{
    var openidCredentialIssuer = GetOpenidCredentialIssuer(httpClient).Result;
    var authorizationEndpoint = GetAuthorizationEndpoint(httpClient, openidCredentialIssuer.AuthorizationServer).Result;
    ExecuteAuthorizationRequest(httpClient, authorizationEndpoint).Wait();
}

async Task<ESBICredentialIssuerResult> GetOpenidCredentialIssuer(HttpClient httpClient)
{
    var url = "https://api-conformance.ebsi.eu/conformance/v3/issuer-mock/.well-known/openid-credential-issuer";
    var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
    var httpResult = await httpClient.SendAsync(requestMessage);
    var json = await httpResult.Content.ReadAsStringAsync();
    var openidCredentialIssuer = JsonSerializer.Deserialize<ESBICredentialIssuerResult>(json);
    return openidCredentialIssuer;
}

async Task<string> GetAuthorizationEndpoint(HttpClient httpClient, string authUrl)
{
    var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{authUrl}/.well-known/openid-configuration");
    var httpResult = await httpClient.SendAsync(requestMessage);
    var json = await httpResult.Content.ReadAsStringAsync();
    return JsonObject.Parse(json)["authorization_endpoint"].ToString();
}

async Task ExecuteAuthorizationRequest(HttpClient httpClient, string url)
{
    var uriBuilder = new UriBuilder(url);
    var dic = new Dictionary<string, string>
    {
        { "client_id", "did:key:z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9KbpMAoXtZtunruYnM4gCV65AKAUX2AwEReRhEaf3BRQNJArZPwQdmf9ENZcF8VT13a58WsHeVjJtvAKKPYEibaEfdUxvU7sgxEUTJpjEkq6BJKrRV1JQ1CqhYvGbmJ1WyoUQ" },
        { "redirect_uri", "http://localhost:5005" },
        { "scope", "openid" },
        { "response_type", "code" }
    };
    uriBuilder.Query = string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    var requestMessage = new HttpRequestMessage
    {
        RequestUri = uriBuilder.Uri
    };
    var httpResult = await httpClient.SendAsync(requestMessage);
    var result = httpResult.Headers.Location.AbsoluteUri;
    var json = await httpResult.Content.ReadAsStringAsync();

}