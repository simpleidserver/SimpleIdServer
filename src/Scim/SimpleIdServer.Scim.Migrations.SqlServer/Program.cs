using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SimpleIdServer.Scim.Migrations.SqlServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            SeedData.EnsureSeedData(host.Services);
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}