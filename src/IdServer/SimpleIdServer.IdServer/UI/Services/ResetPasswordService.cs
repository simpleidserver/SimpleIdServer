// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services;

public interface IResetPasswordService
{

}

public class ResetPasswordService
{
    private readonly IEnumerable<IUserNotificationService> _notificationServices;
    private readonly IConfiguration _configuration;

    public ResetPasswordService(
        IEnumerable<IUserNotificationService> notificationServices,
        IConfiguration configuration)
    {
        _notificationServices = notificationServices;
        _configuration = configuration;
    }

    public async Task SendLink(User user, CancellationToken cancellationToken)
    {
        // Check user has the claim.

    }

    public async Task SendLink(string destination, CancellationToken cancellationToken)
    {
        var options = GetOptions();
        var notificationService = _notificationServices.Single(n => n.Name == options.NotificationService);
        // Build the URL.
        var link = "";
        var body = options.ResetPasswordBody.Replace("{link}", link);
        await notificationService.Send(options.ResetPasswordTitle, body, new Dictionary<string, string>(), destination);
    }

    private IdServerPasswordOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(IdServerPasswordOptions).Name);
        return section.Get<IdServerPasswordOptions>();
    }
}
