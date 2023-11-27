using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Website.Controllers
{
    public class BackChannelLogoutController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public BackChannelLogoutController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromForm] BackChannelLogoutRequest request)
        {
            if (request == null ||string.IsNullOrWhiteSpace(request.LogoutToken)) return BadRequest();
            var logoutToken = request.LogoutToken;
            var handler = new JsonWebTokenHandler();
            var jwt = handler.ReadJsonWebToken(request.LogoutToken);
            var subject = jwt.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub).Value;
            var sessionId = jwt.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sid).Value;
            await _distributedCache.SetStringAsync(sessionId, "disconnected");
            return Ok();
        }
    }

    public class BackChannelLogoutRequest
    {
        [FromForm(Name =  "logout_token")]
        public string LogoutToken { get; set; }
    }
}
