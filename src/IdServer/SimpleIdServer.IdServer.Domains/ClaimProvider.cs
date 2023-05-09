// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class ClaimProvider
    {
        public string Id { get; set; } = null!;
        /// <summary>
        /// Types of provider.
        /// Valid values : AD, API, DB etc...
        /// </summary>
        public string ProviderType { get; set; } = null!;
        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString { get; set; } = null!;
        public ClaimType ClaimType { get; set; } = ClaimType.AGGREGATED;
    }

    public enum ClaimType
    {
        DISTRIBUTED = 0,
        AGGREGATED = 1
    }
}
