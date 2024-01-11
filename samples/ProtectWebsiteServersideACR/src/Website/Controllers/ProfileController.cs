using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers;
public class ProfileController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Edit()
    {
        var amrs = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/claims/authnmethodsreferences").Select(c => c.Value);
        var expectedAmrs = new List<string> { "pwd", "console" };
        if(expectedAmrs.Any(a => !amrs.Contains(a)))
        {
            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                { "acr", "pwd-console" }
            });
            return Challenge(props, "sid");
        }

        return View();
    }
}