// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Console.Services;
using SimpleIdServer.IdServer.UI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddConsoleNotification(this  IdServerBuilder builder)
    {
        builder.Services.AddTransient<IUserAuthenticationService, UserConsoleAuthenticationService>();
        builder.Services.AddTransient<IUserConsoleAuthenticationService, UserConsoleAuthenticationService>();
        builder.Services.AddTransient<IResetPasswordService, UserConsolePasswordResetService>();
        builder.Services.AddTransient<IAuthenticationMethodService, ConsoleAuthenticationService>();
        builder.Services.AddTransient<IUserConsoleNotificationService, ConsoleNotificationService>();
        builder.Services.AddTransient<IUserNotificationService, ConsoleNotificationService>();
        return builder;
    }
}
