// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Fido
{
    public class FidoOptions
    {
        /// <summary>
        /// Expiration time in seconds of the U2F FIDO session identifier.
        /// </summary>
        [ConfigurationRecord("Expiration time in MS of the session", null, order: 0)]
        public TimeSpan U2FExpirationTimeInSeconds { get; set; } = TimeSpan.FromSeconds(5 * 60);
        /// <summary>
        /// Enable or disable the developer mode.
        /// </summary>
        [ConfigurationRecord("Enable the developer mode", null, order: 1)]
        public bool IsDeveloperModeEnabled { get; set; } = false;
    }
}
