using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.Uma.UI
{
    public class ResourcesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View();
        }
    }
}
