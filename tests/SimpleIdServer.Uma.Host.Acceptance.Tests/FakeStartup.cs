using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Uma.Host.Acceptance.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSIDUma();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSID();
        }
    }
}
