using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSIDScim();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSID();
        }
    }
}
