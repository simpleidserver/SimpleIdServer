// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Pwd;

public class IdServerPasswordOptions
{
    /// <summary>
    /// Notification service used to send the RESET link, for example email.
    /// </summary>
    [ConfigurationRecord("Notification mode", null, order: 0)]
    public string NotificationMode { get; set; }
    /// <summary>
    /// Title of the message.
    /// </summary>
    [ConfigurationRecord("Title", null, order: 1)]
    public string ResetPasswordTitle { get; set; }
    /// <summary>
    /// Message sent to the end-user via the selected notification service.
    /// </summary>
    [ConfigurationRecord("Message", null, order: 2)]
    public string ResetPasswordBody { get; set; }
    /// <summary>
    /// Expiration time in seconds of the reset password link.
    /// </summary>
    [ConfigurationRecord("Expiration time", null, order: 3)]
    public int ResetPasswordLinkExpirationInSeconds { get; set; }
}
