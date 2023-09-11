// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthenticationSchemeProvider
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<AuthenticationSchemeProviderMapper> Mappers { get; set; } = new List<AuthenticationSchemeProviderMapper>();
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
        public AuthenticationSchemeProviderDefinition AuthSchemeProviderDefinition { get; set; } = null!;
    }
}
