// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Fido;

public class MobileOptions : IFidoOptions
{
    /// <summary>
    /// Expiration time in seconds of the U2F FIDO session identifier.
    /// </summary>
    [ConfigurationRecord("Duration of the authentication/registration process in seconds", null, order: 0)]
    public int U2FExpirationTimeInSeconds { get; set; } = 5 * 60;
    /// <summary>
    /// Enable or disable the developer mode.
    /// </summary>
    [ConfigurationRecord("Enable the developer mode", null, order: 1)]
    public bool IsDeveloperModeEnabled { get; set; } = false;
}
