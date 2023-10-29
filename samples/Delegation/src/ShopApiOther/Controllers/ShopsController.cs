using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopApiOther.Controllers;

[ApiController]
[Route("shops")]
public class ShopsController : ControllerBase
{
    [HttpGet]
    [Authorize("Shops")]
    public IActionResult Get()
    {
        return new OkObjectResult(new[] { "shop1", "shop2" });
    }
}