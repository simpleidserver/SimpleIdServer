using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.WsFederation.ClientHost.Controllers
{
    [Route("protectedresources")]
    public class ProtectedResourcesController : Controller
    {
        [Authorize("Authenticated")]
        public IActionResult Index()
        {
            return new ContentResult
            {
                Content = "Hello world"
            };
        }
    }
}
