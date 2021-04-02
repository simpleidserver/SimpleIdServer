using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        private readonly Action<IServiceCollection, CustomWebApplicationFactory<T>> _configureTestServices;

        public CustomWebApplicationFactory(Action<IServiceCollection, CustomWebApplicationFactory<T>> configureTestServices = null)
        {
            _configureTestServices = configureTestServices;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup(typeof(T))
                .UseSetting("https_port", "8080"); ;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseContentRoot(".");
            var certificates = LoadCertificates();
            builder.ConfigureServices(collection =>
            {
                _configureTestServices?.Invoke(collection, this);
                collection.AddSingleton<IStartupFilter>(new CertificateConfiguration(certificates));
            });
        }

        private static Dictionary<string, X509Certificate2> LoadCertificates()
        {
            var directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Certificates");
            var result = new Dictionary<string, X509Certificate2>();
            foreach (var file in Directory.GetFiles(directory, "*.crt"))
            {
                result.Add(Path.GetFileName(file), new X509Certificate2(file));
            }

            return result;
        }
    }
}
