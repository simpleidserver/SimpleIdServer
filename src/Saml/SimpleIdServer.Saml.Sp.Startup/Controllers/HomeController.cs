using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.Saml.Sp.Startup.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
