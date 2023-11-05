// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;

namespace SimpleIdServer.IdServer.Notification.Fcm;

public class FcmUserNotificationService : IUserNotificationService
{
    private readonly IConfiguration _configuration;

    public FcmUserNotificationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Name => Constants.NotificationName;

    public async Task Send(string title, string body, Dictionary<string, string> data, User user)
    {
        if (user.Devices == null || !user.Devices.Any(d => d.DeviceType == Constants.NotificationName)) throw new OAuthException(ErrorCodes.UNEXPECTED_ERROR, ErrorMessages.MISSING_REGISTERED_USER_DEVICE);
        var userDevice = user.Devices.First(d => d.DeviceType == Constants.NotificationName);
        await Send(title, body, data, userDevice.PushToken);
    }

    public async Task Send(string title, string body, Dictionary<string, string> data, string destination)
    {
        var options = GetOptions();
        FirebaseApp.Create(new AppOptions
        {
            Credential = string.IsNullOrWhiteSpace(options.ServiceAccountFilePath) ? GoogleCredential.GetApplicationDefault() : GoogleCredential.FromFile(options.ServiceAccountFilePath)
        });
        var fcmMessage = new Message
        {
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Token = destination,
            Data = data
        };
        await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);
    }

    private FcmOptions GetOptions()
    {
        var section = _configuration.GetSection(typeof(FcmOptions).Name);
        return section.Get<FcmOptions>();
    }
}
