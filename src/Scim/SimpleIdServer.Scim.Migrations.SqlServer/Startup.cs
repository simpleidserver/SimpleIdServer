using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Persistence.EF;

namespace SimpleIdServer.Scim.Migrations.SqlServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SCIMDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("db"), o => o.MigrationsAssembly((typeof(Startup)).Namespace)));
        }

        public void Configure(IApplicationBuilder app) { }
    }
}
