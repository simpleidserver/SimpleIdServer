using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.UI
{
    public class HomeController : Controller
    {
        private readonly UMAHostOptions _options;

        public HomeController(IOptions<UMAHostOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            var redirectUrl = Url.Action("LoginCallback", "Home", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["LoginProvider"] = _options.ChallengeAuthenticationScheme;
            return new ChallengeResult(_options.ChallengeAuthenticationScheme, properties);
        }

        [HttpGet]
        public async Task<IActionResult> Disconnect()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult LoginCallback(string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
    }
}
