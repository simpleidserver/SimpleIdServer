using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            const string modulus = "7jyP7WVsRx9WRj/nvLODxpfWrqtITHtssFc6DC8+FBjwcUAsJE+BOiwbGFoMN6aFgnug3T+EWb4g6UcBrkLlLMNhLLAnE1MvvO5elsaTmIdRNaRKq5W2N1nYZM/Ad17gV5XoXsr82Zl92tHHSbhRTRYIAWUevXA8IOMEw+Q1TeBtIGGAjweclkliNb2T69PitHC4AD1CjuHkrEO7LbmZgfsj+F/RjnD+/6MJ0E9KSiJPJ0RFxzsC72NR2uquDDOBxWluUEgXRFgqd1s/D/t/FehPEgfc5Iy88xOQkD/k3SN8xqeopaZD8OdMwxdGNMjwyD5cw80jlH0lXRLTYK0aiQ==";
            const string exponent = "AQAB";
            var rsaParameters = new RSAParameters
            {
                Modulus = Convert.FromBase64String(modulus),
                Exponent = Convert.FromBase64String(exponent)
            };
            var oauthRsaSecurityKey = new RsaSecurityKey(rsaParameters);
            services.AddAuthentication("Test")
                .AddJwtBearer("Test", cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "http://localhost:5000",
                        ValidAudiences = new List<string>
                        {
                            "console"
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            services.AddAuthorization(o => o.AddPolicy("GetWeather", p => p.RequireClaim("scope", "get_weather")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
