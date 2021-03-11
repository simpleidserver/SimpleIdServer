using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents
{
    [Route(Constants.RouteNames.AccountAccessContents)]
    [ApiController]
    [Authorize(Constants.AuthorizationPolicies.Accounts)]
    public class AccountAccessContentsController : Controller
    {
        private readonly IMediator _mediator;

        public AccountAccessContentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddAccountAccessContentCommand addAccountAccessContent, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var token = Request.Headers["Authorization"].ToString().Split(' ').Last();
            addAccountAccessContent.Token = token;
            addAccountAccessContent.Issuer = issuer;
            var result = await _mediator.Send(addAccountAccessContent, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
