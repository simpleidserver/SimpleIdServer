using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;
using Website.Stores;
using Website.ViewModels;

namespace Website.Controllers;

public class AccountsController : Controller
{
    private readonly IBankInfoStore _bankInfoStore;
    private readonly WebsiteOptions _options;

    public AccountsController(IBankInfoStore bankInfoStore, IOptions<WebsiteOptions> options)
    {
        _bankInfoStore = bankInfoStore;
        _options = options.Value;
    }

    public IActionResult Index()
    {
        var result = _bankInfoStore.GetAll().Where(b => !string.IsNullOrWhiteSpace(b.GrantId)).Select(b => new GrantedBankInfoViewModel
        {
            GrantId = b.GrantId,
            BankName = b.Name
        });
        return View(result);
    }

    public async Task<IActionResult> Details(string bankName)
    {
        var accessToken = AccessTokenStore.Instance().GetAccessToken(bankName);
        using (var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_options.AccountInfoUrl)
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResponse = await httpClient.SendAsync(requestMessage);
            var json = await httpResponse.Content.ReadAsStringAsync();
            var viewModel = new BankDetailsViewModel
            {
                Name = bankName,
                Accounts = JsonArray.Parse(json).AsArray().Select(x => x.GetValue<string>())
            };
            return View(viewModel);
        }
    }
}