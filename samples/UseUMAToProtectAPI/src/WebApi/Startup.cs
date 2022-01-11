using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
            const string modulus = "3JsFC7E93xAShgnNp9dDWJPOHjJYLGPX464AfKW9gOB5CGD2uIYiP9m6yuZd73Z334RhQw616IMYijAvtpK25Nkk91KoAvrRoUGv2bl6pmX2JwUjwqe+lbmop4Rj9tzC2UBrGPcWSbIMNLaHkUrqR15DwVdFkG19QBwo9X6gOjCgSDvV0OY7vmwq1M3j2YmDwWnyTXh92wnUn2Hg57mVNZCX8RgdhdaWR6tiFP3QtgEYzZEulOGP6PKilqSr7E6Smg7mUNy6JTRkMGm1KZHTAY6HuNG5PPq0DUmsg8YMmsGEQPHMjw7IdaPxO0qy0aC1fiLj8NgWBOJ6bgrck55vfQ==";
            const string exponent = "AQAB";
            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters
            {
                Modulus = Convert.FromBase64String(modulus),
                Exponent = Convert.FromBase64String(exponent)
            };
            rsa.ImportParameters(rsaParameters);
            var issuerSigningKey = new RsaSecurityKey(rsa);
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = issuerSigningKey,
                        ValidAudiences = new List<string>
                        {
                            "client"
                        },
                        ValidIssuers = new List<string>
                        {
                "http://localhost:60003"
                        }
                    };
                });
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
