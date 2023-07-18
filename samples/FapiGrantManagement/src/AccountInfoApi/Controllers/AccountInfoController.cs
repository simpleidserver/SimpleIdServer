using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountInfoApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize("account_information")]
public class AccountInfoController : ControllerBase
{
    [HttpGet(Name = "Accounts")]
    public IEnumerable<string> Get()
    {
        return new List<string> { "BE91798829733676", "BE90321175762332", "BE56631811788388" };
    }
}