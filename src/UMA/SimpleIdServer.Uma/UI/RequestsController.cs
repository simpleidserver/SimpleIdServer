using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.Uma.UI
{
    public class RequestsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
