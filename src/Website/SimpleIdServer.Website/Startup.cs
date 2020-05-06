using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace SimpleIdServer.Website
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string baseUrl = _configuration["pathBase"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "/";
            }

            app.UseCors("AllowAllOrigins");
            if (baseUrl != "/")
            {
                app.Map(new PathString(baseUrl), appm =>
                {
                    appm.UseDefaultFiles();
                    appm.UseStaticFiles();
                    appm.Use(async (context, next) =>
                    {
                        var root = env.WebRootPath;
                        var path = Path.Combine(root, "index.html");
                        await context.Response.WriteAsync(File.ReadAllText(path));
                    });
                });
                return;
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                var root = env.WebRootPath;
                var path = Path.Combine(root, "index.html");
                await context.Response.WriteAsync(File.ReadAllText(path));
            });
        }
    }
}
