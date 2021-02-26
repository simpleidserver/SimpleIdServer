using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Infrastructure.Middlewares
{
    /// <summary>
    /// Always redirect 403 error to 401.
    /// </summary>
    public class RedirectHttpStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectHttpStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next.Invoke(context);
            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
