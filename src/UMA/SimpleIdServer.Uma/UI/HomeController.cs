using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.Uma.UI
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
