using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.Uma.Api;

namespace SimpleIdServer.Uma.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
            }).AddApplicationPart(typeof(RegistrationController).Assembly)
            .AddApplicationPart(typeof(PermissionsAPIController).Assembly)
            .AddNewtonsoftJson(o => { });
            services.AddAuthorization(p => p.AddDefaultOAUTHAuthorizationPolicy());
            services.AddSIDUma(o =>
            {
                o.OpenIdJsonWebKeySignature = JwksStore.GetInstance().GetJsonWebKey();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
