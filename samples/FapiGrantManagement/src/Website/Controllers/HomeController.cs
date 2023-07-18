using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Authentication;
using System.Text.Json.Nodes;
using Website.Models;
using Website.Stores;

namespace Website.Controllers;

public class HomeController : Controller
{
    private readonly IBankInfoStore _bankInfoStore;
    private readonly WebsiteOptions _options;

    public HomeController(IBankInfoStore bankInfoStore, IOptions<WebsiteOptions> options)
    {
        _bankInfoStore = bankInfoStore;
        _options = options.Value;
    }

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    [Route("callback/{bankName}")]
    public async Task<IActionResult> Callback(string bankName)
    {
        var bankInfo = _bankInfoStore.GetAll().First(b => b.Name == bankName);
        var accessToken = await GetAccessToken();
        AccessTokenStore.Instance().Add(bankName, accessToken);
        await _bankInfoStore.SaveChanges(CancellationToken.None);
        return RedirectToAction("Index");

        async Task<string> GetAccessToken()
        {
            var authorizationCode = Request.Query["code"].First();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                CheckCertificateRevocationList = false,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };
            handler.ClientCertificates.Add(_options.MTLSCertificate);
            using (var httpClient = new HttpClient(handler))
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(bankInfo.TokenUrl),
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", bankInfo.ClientId },
                        { "grant_type", "authorization_code" },
                        { "code", authorizationCode },
                        { "redirect_uri", $"{_options.CallbackUrl}/{bankName}" }
                    })
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                var json = await httpResult.Content.ReadAsStringAsync();
                var jsonObj = JsonObject.Parse(json).AsObject();
                if (jsonObj.ContainsKey("grant_id"))
                {
                    var grantId = jsonObj["grant_id"].GetValue<string>();
                    bankInfo.GrantId = grantId;
                }

                return jsonObj["access_token"].GetValue<string>();
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}