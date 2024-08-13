using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Mobile.Clients;

public interface ISidServerClient
{
    Task<string> AddGotifyConnection();
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
}
