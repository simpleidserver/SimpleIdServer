// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Pwd;

public class IdServerPasswordOptions
{
    /// <summary>
    /// Notification service used to send the RESET link, for example email.
    /// </summary>
    [ConfigurationRecord("Notification mode", "Notification mode used to send the reset link", 0, null, CustomConfigurationRecordType.NOTIFICATIONMODE)]
    public string NotificationMode { get; set; } = Constants.DefaultNotificationMode;
    /// <summary>
    /// Title of the message.
    /// </summary>
    [ConfigurationRecord("Title", null, order: 1)]
    public string ResetPasswordTitle { get; set; } = "Reset your password";
    /// <summary>
    /// Message sent to the end-user via the selected notification service.
    /// </summary>
    [ConfigurationRecord("Message", null, order: 2)]
    public string ResetPasswordBody { get; set; } = "Link to reset your password {0}";
    /// <summary>
    /// Expiration time in seconds of the reset password link.
    /// </summary>
    [ConfigurationRecord("Expiration time", "Expiration time in seconds of the reset link", order: 3)]
    public int ResetPasswordLinkExpirationInSeconds { get; set; } = 30;

    /// <summary>
    /// Enable password validation.
    /// </summary>
    [ConfigurationRecord("Password validation", "Enable or disable the password validation", order: 4)]
    public bool EnableValidation { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum length a password must be. Defaults to 6.
    /// </summary>
    [ConfigurationRecord("Required length", "Gets or sets the minimum length a password must be", order: 5, displayCondition: "EnableValidation=true")]
    public int RequiredLength { get; set; } = 6;

    /// <summary>
    /// Gets or sets the minimum number of unique characters which a password must contain. Defaults to 1.
    /// </summary>
    [ConfigurationRecord("Required unique chars", "Gets or sets the minimum number of unique characters which a password must contain", order: 6, displayCondition: "EnableValidation=true")]
    public int RequiredUniqueChars { get; set; } = 1;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a non-alphanumeric character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a non-alphanumeric character, otherwise false.</value>
    [ConfigurationRecord("Require non alpha numeric", "Gets or sets a flag indicating if passwords must contain a non-alphanumeric character", order: 7, displayCondition: "EnableValidation=true")]
    public bool RequireNonAlphanumeric { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a lower case ASCII character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a lower case ASCII character.</value>
    [ConfigurationRecord("Require lower case", "Gets or sets a flag indicating if passwords must contain a lower case ASCII character.", order: 8, displayCondition: "EnableValidation=true")]
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a upper case ASCII character. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a upper case ASCII character.</value>
    [ConfigurationRecord("Require upper case", "Gets or sets a flag indicating if passwords must contain a upper case ASCII character.", order: 9, displayCondition: "EnableValidation=true")]
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a flag indicating if passwords must contain a digit. Defaults to true.
    /// </summary>
    /// <value>True if passwords must contain a digit.</value>
    [ConfigurationRecord("Require digit", "Gets or sets a flag indicating if passwords must contain a digit.", order: 10, displayCondition: "EnableValidation=true")]
    public bool RequireDigit { get; set; } = true;
}
