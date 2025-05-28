using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Authzen.Dtos;

namespace SimpleIdServer.Authzen.Controllers
{
    [Route("access/v1")]
    public class AccessEvaluationController : ControllerBase
    {
        [HttpPost("evaluation")]
        public IActionResult Evaluate([FromBody] AccessEvaluationRequest request)
        {
            return Ok(new AccessEvaluationResponse
            {
                Decision = true
            });
        }
    }
}