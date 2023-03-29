using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Website.Models;
using Website.Stores;

namespace Website.Controllers;

public class HomeController : Controller
{
    private readonly IUserStore _userStore;

    public HomeController(IUserStore userStore)
    {
        _userStore = userStore;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AuthenticateBank()
    {
        const string bankAccountName = "bank";
        const string userId = "userId";
        const string clientId = "fapiGrant";
        const string callbackUrl = "http://localhost:7000/callback";
        const string authorizationDetails = "{ \"type\" : \"account_information\", \"actions\" : [\"read\"] }";
        var userGrant = _userStore.Get(userId).GetGrant(bankAccountName);
        var url = $"https://localhost:5001/master/authorization?client_id={clientId}&redirect_uri={callbackUrl}&response_type=code&scope=openid profile&authorization_details={authorizationDetails}";
        if (userGrant == null)
            url += "&code&grant_management_action=create";
        return Redirect(url);
    }

    [Route("callback")]
    public async Task<IActionResult> Callback()
    {
        const string bankAccountName = "bank";
        const string userId = "userId";
        var user = _userStore.Get(userId);
        var accessToken = await GetAccessToken();
        var accounts = await GetAccounts();
        return View(new CallbackViewModel
        {
            GrantId = user.GetGrant(bankAccountName).GrantId,
            Accounts = accounts
        });

        async Task<string> GetAccessToken()
        {
            var authorizationCode = Request.Query["code"].First();
            using (var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:5001/master/token"),
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", "fapiGrant" },
                    { "client_secret", "password" },
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "redirect_uri", "http://localhost:7000/callback" }
                })
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                var json = await httpResult.Content.ReadAsStringAsync();
                var jsonObj = JsonObject.Parse(json).AsObject();
                if (jsonObj.ContainsKey("grant_id"))
                {
                    var grantId = jsonObj["grant_id"].GetValue<string>();
                    user.Grants.Add(new UserGrant
                    {
                        BankAccount = bankAccountName,
                        GrantId = grantId
                    });
                }
                return jsonObj["access_token"].GetValue<string>();
            }
        }

        async Task<IEnumerable<string>> GetAccounts()
        {
            using(var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://localhost:7001/AccountInfo")
                };
                requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
                var httpResponse = await httpClient.SendAsync(requestMessage);
                var json = await httpResponse.Content.ReadAsStringAsync();
                return JsonArray.Parse(json).AsArray().Select(x => x.GetValue<string>());
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
