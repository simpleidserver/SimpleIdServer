// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.OpenBankingApi.EF.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Run();
        }

        public static IHost CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(cfg =>
                {
                    cfg.UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 60010, listenOptions =>
                        {
                            var serverCertificate = LoadCertificate();
                            listenOptions.UseHttps(serverCertificate, configureOptions =>
                            {
                                configureOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                                configureOptions.SslProtocols = SslProtocols.Tls12;
                                configureOptions.ClientCertificateValidation = (certificate2, chain, args) =>
                                {
                                    return true;
                                };
                            });
                        });
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();
                })
                .Build();                
        }

        private static X509Certificate2 LoadCertificate()
        {
            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            var embeddedFileProvider = new EmbeddedFileProvider(assembly, "SimpleIdServer.OpenBankingApi.EF.Startup");
            var certificateFileInfo = embeddedFileProvider.GetFileInfo("localhost.pfx");
            using (var certificateStream = certificateFileInfo.CreateReadStream())
            {
                byte[] certificatePayload;
                using (var memoryStream = new MemoryStream())
                {
                    certificateStream.CopyTo(memoryStream);
                    certificatePayload = memoryStream.ToArray();
                }

                return new X509Certificate2(certificatePayload, "pass");
            }
        }
    }
}