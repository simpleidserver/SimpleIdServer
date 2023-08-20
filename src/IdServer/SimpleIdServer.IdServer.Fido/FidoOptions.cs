// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Fido
{
    public class FidoOptions
    {
        /// <summary>
        /// Expiration time in seconds of the U2F FIDO session identifier.
        /// </summary>
        public TimeSpan U2FExpirationTimeInSeconds { get; set; } = TimeSpan.FromSeconds(5 * 60);
        /// <summary>
        /// Enable or disable the developer mode.
        /// </summary>
        public bool IsDeveloperModeEnabled { get; set; } = false;
    }
}
