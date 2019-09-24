// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains.Scopes
{
    public class OAuthClaim : ICloneable
    {
        public OAuthClaim()
        {
            Descriptions = new List<OAuthTranslation>();
        }

        public OAuthClaim(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public ICollection<OAuthTranslation> Descriptions { get; set; }

        public object Clone()
        {
            return new OAuthClaim
            {
                Name = Name,
                Descriptions = Descriptions.Select(d => (OAuthTranslation)d.Clone()).ToList()
            };
        }
    }
}
