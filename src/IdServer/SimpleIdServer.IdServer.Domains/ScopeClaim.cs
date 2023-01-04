// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ScopeClaim : ICloneable
    {
        public ScopeClaim(string claimName, bool isExposed)
        {
            ClaimName = claimName;
            IsExposed = isExposed;
        }

        /// <summary>
        /// Name of the claim.
        /// </summary>
        public string ClaimName { get; set; } = null!;
        /// <summary>
        /// Claim is exposed by the configuration endpoitn.
        /// </summary>
        public bool IsExposed { get; set; }

        public object Clone()
        {
            return new ScopeClaim(ClaimName, IsExposed);
        }
    }
}
