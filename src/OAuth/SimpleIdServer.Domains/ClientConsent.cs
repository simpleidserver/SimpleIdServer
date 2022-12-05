// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Domains
{
    public class ClientConsent : ICloneable
    {
        public ClientConsent()
        {
            Scopes = new List<Scope>();
            Claims = new List<string>();
        }

        public ClientConsent(string id, string clientId, IEnumerable<Scope> scopes, IEnumerable<string> claims)
        {
            Id = id;
            ClientId = clientId;
            Scopes = scopes;
            Claims = claims;
        }


        public string Id { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public IEnumerable<Scope> Scopes { get; set; } = new List<Scope>();
        public IEnumerable<string> Claims { get; set; } = new List<string>();

        public object Clone()
        {
            return new ClientConsent
            {
                Id = Id,
                ClientId = ClientId,
                Scopes = Scopes == null ? new List<Scope>() : Scopes.Select(s => (Scope)s.Clone()),
                Claims = Claims.ToList()
            };
        }
    }
}
