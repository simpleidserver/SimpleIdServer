// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthScopeClaim : ICloneable
    {
        public OAuthScopeClaim(string claimName, bool isExposed)
        {
            ClaimName = claimName;
            IsExposed = isExposed;
        }

        #region Properties

        /// <summary>
        /// Name of the claim.
        /// </summary>
        public string ClaimName { get; set; }
        /// <summary>
        /// Claim is exposed by the configuration endpoitn.
        /// </summary>
        public bool IsExposed { get; set; }

        #endregion

        public object Clone()
        {
            return new OAuthScopeClaim(ClaimName, IsExposed);
        }
    }
}
