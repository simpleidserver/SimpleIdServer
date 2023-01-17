// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ApiResource
    {
        /// <summary>
        /// Target service or resource to which access is being requested.
        /// </summary>
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
    }
}
