// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api
{
    public class ConsoleNotificationService : IUserNotificationService
    {
        public string Name => Constants.DefaultNotificationMode;

        public Task Send(string title, string body, Dictionary<string, string> data, User user) => Send(title, body, data, string.Empty);

        public Task Send(string title, string body, Dictionary<string, string> data, string destination)
        {
            var before = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(body);
            Console.ForegroundColor = before;
            return Task.CompletedTask;
        }
    }
}
