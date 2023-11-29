using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.ViewModels;

namespace Website.Controllers
{
    public class ClaimsController : Controller
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return View(new ClaimsViewModel
            {
                AccessToken = accessToken
            });
        }
    }
}
