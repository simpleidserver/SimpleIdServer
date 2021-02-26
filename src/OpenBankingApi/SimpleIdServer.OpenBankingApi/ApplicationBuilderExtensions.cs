using Microsoft.AspNetCore.Builder;
using SimpleIdServer.OpenBankingApi.Infrastructure.Middlewares;

namespace SimpleIdServer.OpenBankingApi
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseOpenBankingAPI(this IApplicationBuilder app)
        {
            app.UseMiddleware<FAPICorrelationMiddleware>();
            app.UseMiddleware<RedirectHttpStatusMiddleware>();
            return app;
        }
    }
}
