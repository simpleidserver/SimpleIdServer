using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.Uma.UI
{
    [Route(UMAConstants.EndPoints.ResourcesUI)]
    public class ResourcesUIController : Controller
    {
        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View();
        }
    }
}
