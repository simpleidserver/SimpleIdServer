// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Domains
{
    public class Consent : ICloneable
    {
        public Consent()
        {
            Scopes = new List<string>();
            Claims = new List<string>();
        }

        public Consent(string id, string clientId, IEnumerable<string> scopes, IEnumerable<string> claims)
        {
            Id = id;
            ClientId = clientId;
            Scopes = scopes;
            Claims = claims;
        }


        public string Id { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
        public IEnumerable<string> Claims { get; set; } = new List<string>();

        public object Clone()
        {
            return new Consent
            {
                Id = Id,
                ClientId = ClientId,
                Scopes = Scopes.ToList(),
                Claims = Claims.ToList()
            };
        }
    }
}
