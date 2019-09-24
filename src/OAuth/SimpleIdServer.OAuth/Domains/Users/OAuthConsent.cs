// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains.Users
{
    public class OAuthConsent : ICloneable
    {
        public OAuthConsent()
        {
            Scopes = new List<OAuthScope>();
            Claims = new List<OAuthClaim>();
        }

        public OAuthConsent(string id, string clientId, IEnumerable<OAuthScope> scopes, IEnumerable<OAuthClaim> claims)
        {
            Id = id;
            ClientId = clientId;
            Scopes = scopes;
            Claims = claims;
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<OAuthScope> Scopes { get; set; }
        public IEnumerable<OAuthClaim> Claims { get; set; }

        public object Clone()
        {
            return new OAuthConsent
            {
                Id = Id,
                ClientId = ClientId,
                Scopes = Scopes == null ? new List<OAuthScope>() : Scopes.Select(s => (OAuthScope)s.Clone()),
                Claims = Claims == null ? new List<OAuthClaim>() : Claims.Select(c => (OAuthClaim)c.Clone())
            };
        }
    }
}