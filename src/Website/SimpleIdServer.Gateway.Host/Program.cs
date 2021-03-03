// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace SimpleIdServer.Gateway.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults((cfg) =>
                {
                    cfg.UseStartup<Startup>();
                    cfg.UseUrls("http://*:5001");
                    cfg.UseContentRoot(Directory.GetCurrentDirectory());
                    cfg.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                            .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                            .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json");
                    });
                })
                .Build();
            host.Run();
        }
    }
}