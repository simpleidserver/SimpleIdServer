// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Startup
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(cfg =>
                {
                    cfg.UseUrls("http://*:5002");
                    cfg.UseStartup<Startup>();
                })
                .Build();
            await host.RunAsync();
        }
    }
}