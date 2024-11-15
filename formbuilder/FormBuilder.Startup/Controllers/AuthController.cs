using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class AuthController : Controller
{
    [Route("authenticate")]
    public IActionResult Index()
    {
        return NoContent();
    }

    [Route("authenticate")]
    [HttpPost]
    public IActionResult Confirm(AuthViewModel viewModel)
    {
        return NoContent();
    }
}
