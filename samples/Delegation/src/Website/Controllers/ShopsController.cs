using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Website.ViewModels;

namespace Website.Controllers;

public class ShopsController : Controller
{
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        const string url = "http://localhost:5005/shops";
        using(var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var shopNames = JsonSerializer.Deserialize<string[]>(json);
            return View(new ShopsViewModel { Shops = shopNames });
        }
    }
}
