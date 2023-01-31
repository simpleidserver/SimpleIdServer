// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.WsFederation
{
    public class IdServerWsFederationOptions
    {
        /// <summary>
        /// Default JSON Web Key used to sign the response.
        /// </summary>
        public string? DefaultKid { get; set; } = null;
    }
}
