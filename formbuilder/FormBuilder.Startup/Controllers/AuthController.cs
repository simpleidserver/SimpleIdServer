using FormBuilder.Startup.Controllers.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Startup.Controllers;

public class AuthController : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly FormBuilderOptions _options;

    public AuthController(IAntiforgery antiforgery, IOptions<FormBuilderOptions> options)
    {
        _antiforgery = antiforgery;
        _options = options.Value;
    }

    public IActionResult Index()
    {
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var viewModel = new AuthViewModel
        {
            ReturnUrl = "http://localhost:5000",
            ExternalIdProviders = new List<ExternalIdProviderViewModel>
            {
                new ExternalIdProviderViewModel { AuthenticationScheme = "facebook", DisplayName = "Facebook" }
            }
        };
        return View(new IndexAuthViewModel
        {
            Form = Constants.LoginPwdAuthForm,
            Input = JsonObject.Parse(JsonSerializer.Serialize(viewModel)).AsObject(),
            AntiforgeryToken = new AntiforgeryTokenRecord
            {
                FormValue = tokenSet.RequestToken,
                FormField = tokenSet.FormFieldName,
                CookieName = _options.AntiforgeryCookieName,
                CookieValue = tokenSet.CookieToken
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirm(AuthViewModel viewModel)
    {
        return NoContent();
    }

    [HttpGet]
    public IActionResult Callback(string scheme)
    {
        return NoContent();
    }
}
