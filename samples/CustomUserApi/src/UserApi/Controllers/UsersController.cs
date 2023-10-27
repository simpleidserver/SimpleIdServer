using Microsoft.AspNetCore.Mvc;

namespace UserApi.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private static ICollection<User> _users = new List<User>
    {
        new User { Id = Guid.NewGuid().ToString(), Login = "apiUser", Password = "password", Claims = new List<UserClaim> 
        {
            new UserClaim { Name = "email", Value = "sid@gmail.com" }
        }}
    };

    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] Authenticate request)
    {
        var user = _users.SingleOrDefault(u => u.Login == request.Login && u.Password == request.Password);
        if (user == null) return Unauthorized();
        return Ok(new AuthenticationResult { UserId = user.Id });
    }

    [HttpGet("{id}/claims")]
    public IActionResult GetClaims(string id)
    {
        var user = _users.SingleOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        return Ok(user.Claims);
    }
}

public record User
{
    public string Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public ICollection<UserClaim> Claims { get; set; }
}

public record Authenticate
{
    public string Login { get; set; }
    public string Password { get; set; }
}

public record UserClaim
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public record AuthenticationResult
{
    public string UserId { get; set; }
}