using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UseUMAToProtectAPI.Portal.Controllers
{
    public class HomeController : Controller
    {
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
            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, properties);
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

        [HttpGet]
        public async Task<IActionResult> Disconnect()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
