using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ShopApi.Controllers;

[ApiController]
[Route("shops")]
public class ShopsController : ControllerBase
{
    [HttpGet]
    [Authorize("Shops")]
    public async Task<IActionResult> Get()
    {
        const string url = "http://localhost:5006/shops";
        var accessToken = await GetAccessToken();
        using (var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var shopNames = JsonSerializer.Deserialize<string[]>(json);
            return new OkObjectResult(shopNames);
        }
    }

    private async Task<string> GetAccessToken()
    {
        const string clientId = "firstDelegationAPi";
        const string clientSecret = "password";
        const string tokenUrl = "https://localhost:5001/master/token";
        var subjectToken = Request.Headers["Authorization"].ElementAt(0).Split("Bearer")[1];
        using (var httpClient = new HttpClient())
        {
            var dic = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:token-exchange"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("subject_token", subjectToken),
                new KeyValuePair<string, string>("subject_token_type", "urn:ietf:params:oauth:token-type:access_token")
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(dic),
                RequestUri = new Uri(tokenUrl)
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var accessToken = (JsonObject.Parse(json) as JsonObject).First(k => k.Key == "access_token").Value.ToString();
            return accessToken;
        }
    }
}
