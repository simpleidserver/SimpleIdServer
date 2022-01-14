using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

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
            var mapping = new Dictionary<string, string>
            {
                { "sub", ClaimTypes.NameIdentifier },
                { "role", ClaimTypes.Role }
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async (ctx) =>
                        {
                            using (var httpClient = new HttpClient())
                            {
                                var authorization = ctx.Request.Headers["Authorization"][0];
                                var bearer = authorization.Split(" ").Last();
                                var requestMessage = new HttpRequestMessage
                                {
                                    RequestUri = new Uri("https://localhost:5001/userinfo"),
                                    Method = HttpMethod.Get
                                };
                                requestMessage.Headers.Add("Authorization", $"Bearer {bearer}");
                                var httpResponse = await httpClient.SendAsync(requestMessage);
                                var json = await httpResponse.Content.ReadAsStringAsync();
                                var jObj = JsonDocument.Parse(json);
                                var identity = new ClaimsIdentity("userInfo");
                                var props = jObj.RootElement.EnumerateObject();
                                while(props.MoveNext())
                                {
                                    var kvp = props.Current;
                                    var key = kvp.Name;
                                    if (mapping.ContainsKey(key))
                                    {
                                        key = mapping[key];
                                    }

                                    identity.AddClaim(new Claim(key, kvp.Value.GetString()));
                                }

                                var principal = new ClaimsPrincipal(identity);
                                ctx.Principal = principal;
                            }
                        }
                    };
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "https://localhost:5001",
                        ValidAudiences = new List<string>
                        {
                            "website"
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = oauthRsaSecurityKey
                    };
                });
            services.AddAuthorization(o => o.AddPolicy("IsAdmin", p => p.RequireClaim(ClaimTypes.Role, "admin")));
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
