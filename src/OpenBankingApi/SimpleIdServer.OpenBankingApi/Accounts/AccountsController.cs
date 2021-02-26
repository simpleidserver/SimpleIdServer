using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenBankingApi.Accounts.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Accounts
{
    [Route(Constants.RouteNames.Accounts)]
    [ApiController]
    [Authorize(Constants.AuthorizationPolicies.Accounts)]
    public class AccountsController : Controller
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var token = Request.Headers["Authorization"].ToString().Split(' ').Last();
            var result = await _mediator.Send(new GetAccountsQuery(token, issuer));
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = await _mediator.Send(new GetAccountQuery(id, issuer));
            return new OkObjectResult(result);
        }
    }
}
