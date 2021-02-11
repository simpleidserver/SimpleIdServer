// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID.Domains
{
    public class OpenIdScopeClaim
    {
        public OpenIdScopeClaim(string claimName, bool isExposed)
        {
            ClaimName = claimName;
            IsExposed = isExposed;
        }

        /// <summary>
        /// Name of the claim.
        /// </summary>
        public string ClaimName { get; set; }
        /// <summary>
        /// Claim is exposed by the configuration endpoitn.
        /// </summary>
        public bool IsExposed { get; set; }
    }
}
