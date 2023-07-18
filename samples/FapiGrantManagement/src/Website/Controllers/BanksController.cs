using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Website.Stores;
using Website.ViewModels;

namespace Website.Controllers;

public class BanksController : Controller
{
    private readonly IBankInfoStore _bankInfoStore;
    private readonly WebsiteOptions _options;

    public BanksController(IBankInfoStore bankInfoStore, IOptions<WebsiteOptions> options)
    {
        _bankInfoStore = bankInfoStore;
        _options = options.Value;
    }

    public IActionResult Index()
    {
        var bankInfos = _bankInfoStore.GetAll().Select(b => new BankInfoViewModel
        {
            Name = b.Name
        });
        return View(bankInfos);
    }

    public IActionResult Link(string name)
    {
        var bankInfo = _bankInfoStore.GetAll().First(b => b.Name == name);
        const string authorizationDetails = "{ \"type\" : \"account_information\", \"actions\" : [\"read\"] }";
        var url = $"{bankInfo.AuthorizationUrl}?client_id={bankInfo.ClientId}&redirect_uri={_options.CallbackUrl}/{name}&response_type=code&scope=openid profile&authorization_details={authorizationDetails}&grant_management_action=create";
        return Redirect(url);
    }
}