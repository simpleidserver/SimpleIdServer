using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleIdServer.Scim.Startup
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBranchWithServices(this IApplicationBuilder app, PathString path, Action<IServiceCollection> servicesConfiguration, Action<IApplicationBuilder> appBuilderConfiguration)
        {
            var webHost = new WebHostBuilder().UseKestrel().ConfigureServices(servicesConfiguration).UseStartup<EmptyStartup>().Build();
            var serviceProvider = webHost.Services;
            var serverFeatures = webHost.ServerFeatures;
            var appBuilderFactory = serviceProvider.GetRequiredService<IApplicationBuilderFactory>();
            var branchBuilder = appBuilderFactory.CreateBuilder(serverFeatures);
            var factory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            branchBuilder.Use(async (context, next) =>
            {
                using (var scope = factory.CreateScope())
                {
                    context.RequestServices = scope.ServiceProvider;
                    await next();
                }
            });

            appBuilderConfiguration(branchBuilder);
            var branchDelegate = branchBuilder.Build();
            return app.Map(path, builder =>
            {
                builder.Use(async (context, next) =>
                {
                    await branchDelegate(context);
                });
            });
        }

        private class EmptyStartup
        {
            public void ConfigureServices(IServiceCollection services) { }

            public void Configure(IApplicationBuilder app) { }
        }
    }
}
