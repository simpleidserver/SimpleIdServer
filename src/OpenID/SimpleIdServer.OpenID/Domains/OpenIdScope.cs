// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenID.Domains
{
    public class OpenIdScope : OAuthScope
    {
        public OpenIdScope()
        {
            Claims = new List<OpenIdScopeClaim>();
        }

        public OpenIdScope(string name) : base(name)
        {
            Claims = new List<OpenIdScopeClaim>();
        }

        /// <summary>
        /// Array of strings that specifies the claims.
        /// </summary>
        public ICollection<OpenIdScopeClaim> Claims { get; set; }

        public override object Clone()
        {
            return new OpenIdScope
            {
                Claims = Claims.ToList(),
                CreateDateTime = CreateDateTime,
                IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
                Name = Name,
                UpdateDateTime = UpdateDateTime
            };
        }
    }
}
