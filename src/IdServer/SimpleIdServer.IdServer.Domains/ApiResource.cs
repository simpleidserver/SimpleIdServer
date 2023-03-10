// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ApiResource
    {
        public string Id { get; set; }
        /// <summary>
        /// Target service or resource to which access is being requested.
        /// </summary>
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    }
}
