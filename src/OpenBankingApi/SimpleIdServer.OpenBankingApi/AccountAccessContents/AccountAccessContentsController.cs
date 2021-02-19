using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents
{
    [Route("account-requests")]
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
        public async Task<IActionResult> Add([FromBody] AddAccountAccessContentCommand addAccountAccessContent, CancellationToken token)
        {
            var result = await _mediator.Send(addAccountAccessContent, token);
            return new OkObjectResult(result);
        }
    }
}
