// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.UI;

public class IdServerPasswordOptions
{
    /// <summary>
    /// Notification service used to send the RESET link, for example email.
    /// </summary>
    public string NotificationService { get; set; }
    /// <summary>
    /// Title of the message.
    /// </summary>
    public string ResetPasswordTitle { get; set; }
    /// <summary>
    /// Message sent to the end-user via the selected notification service.
    /// </summary>
    public string ResetPasswordBody { get; set; }
    /// <summary>
    /// Expiration time in seconds of the reset password link.
    /// </summary>
    public int ResetPasswordLinkExpirationInSeconds { get; set; }
}
