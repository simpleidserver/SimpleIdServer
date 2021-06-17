// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using System;

namespace SimpleIdServer.Scim.Provisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .Build();
            JobLauncher.Launch(configuration);
            Console.WriteLine("Press Enter to quit the application !");
            Console.ReadLine();
        }
    }
}
