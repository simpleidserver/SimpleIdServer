// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using cl = System.Console;

namespace SimpleIdServer.IdServer.Console;

public interface IUserConsoleNotificationService : IUserNotificationService
{

}

public class ConsoleNotificationService : IUserConsoleNotificationService
{
    public string Name => Constants.AMR;

    public Task Send(string title, string body, Dictionary<string, string> data, User user)
    {
        return Send(title, body, data, string.Empty);
    }

    public Task Send(string title, string body, Dictionary<string, string> data, string destination)
    {
        var before = cl.ForegroundColor;
        cl.ForegroundColor = ConsoleColor.Green;
        cl.WriteLine($"Title : {title}, Body : {body}");
        cl.ForegroundColor = before;
        return Task.CompletedTask;
    }
}
