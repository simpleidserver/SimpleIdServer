using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OpenBankingApi.Accounts.Queries;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Accounts
{
    [Route("v3.1/accounts")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _mediator.Send(new GetAccountQuery(id));
            return new OkObjectResult(result);
        }
    }
}
