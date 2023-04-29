using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.IdServer.Website.Middlewares;

public class SignOutMiddleware
{
    private readonly RequestDelegate _next;

    public SignOutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/SignOut", System.StringComparison.OrdinalIgnoreCase))
        {
            await context.SignOutAsync("oidc");
        }

        await _next(context);
    }
}
