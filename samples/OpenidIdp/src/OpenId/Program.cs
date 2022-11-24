// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;

namespace OpenId
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults((cfg) =>
                {
                    cfg.UseStartup<OpenIdStartup>();
                    cfg.ConfigureKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(configureOptions =>
                        {
                            configureOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                        });
                    });
                });
    }
}