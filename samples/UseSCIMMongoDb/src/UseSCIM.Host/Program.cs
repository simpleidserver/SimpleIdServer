// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace UseSCIM.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults((cfg) =>
                {
                    cfg.UseUrls("http://*:60002");
                    cfg.UseStartup<Startup>();
                    cfg.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.json");
                        config.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                    });
                }).Build();
            host.Run();
        }
    }
}