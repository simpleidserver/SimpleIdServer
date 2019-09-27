// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthConsent : ICloneable
    {
        public OAuthConsent()
        {
            Scopes = new List<OAuthScope>();
            Claims = new List<string>();
        }

        public OAuthConsent(string id, string clientId, IEnumerable<OAuthScope> scopes, IEnumerable<string> claims)
        {
            Id = id;
            ClientId = clientId;
            Scopes = scopes;
            Claims = claims;
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<OAuthScope> Scopes { get; set; }
        public IEnumerable<string> Claims { get; set; }

        public object Clone()
        {
            return new OAuthConsent
            {
                Id = Id,
                ClientId = ClientId,
                Scopes = Scopes == null ? new List<OAuthScope>() : Scopes.Select(s => (OAuthScope)s.Clone()),
                Claims = Claims.ToList()
            };
        }
    }
}