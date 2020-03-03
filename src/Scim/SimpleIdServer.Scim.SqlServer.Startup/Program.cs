// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SimpleIdServer.Scim.SqlServer.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                    config.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                })
                .UseKestrel()
                .UseUrls("http://*:60002")
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}