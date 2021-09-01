// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning
{
    class Program
    {
        private static readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{environmentName}.json", true, true)
              .Build();
            Task.Run(() =>
            {
                JobLauncher.Launch(configuration);
                var random = new Random(10);
                while (true)
                {
                    Thread.Sleep(1000);
                }
            });
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Exit");
                waitHandle.Set();
            };

            waitHandle.WaitOne();
        }
    }
}
