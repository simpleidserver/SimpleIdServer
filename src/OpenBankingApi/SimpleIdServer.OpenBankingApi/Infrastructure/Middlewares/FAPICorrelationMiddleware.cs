using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Infrastructure.Middlewares
{
    /// <summary>
    /// An RFC4122 UID used as a correlation Id.
    /// The ASPSP must set the response header x-fapi-interaction-id to the value received from the corresponding fapi client request header or to a RFC4122 UUID value if the request header was not provided to track the interaction. 
    /// The header must be returned for both successful and error responses.
    /// </summary>
    public class FAPICorrelationMiddleware
    {
        private const string HEADER_NAME = "x-fapi-interaction-id";
        private readonly RequestDelegate _next;

        public FAPICorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HEADER_NAME, out StringValues values))
            {
                context.Response.Headers.Add(HEADER_NAME, values);
            }
            else
            {
                context.Response.Headers.Add(HEADER_NAME, Guid.NewGuid().ToString());
            }

            await _next.Invoke(context);
        }
    }
}
