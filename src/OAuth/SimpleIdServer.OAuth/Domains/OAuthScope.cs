// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthScope : ICloneable, IEquatable<OAuthScope>, IOAuthScope
    {
        public OAuthScope()
        {
            Claims = new List<OAuthScopeClaim>();
        }

        public OAuthScope(string name) : this()
        {
            Name = name;
        }

        #region Properties

        public string Name { get; set; }
        public bool IsStandardScope { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Array of strings that specifies the claims.
        /// </summary>
        public ICollection<OAuthScopeClaim> Claims { get; set; }
        public ICollection<OAuthConsent> Consents { get; set; }
        public ICollection<OAuthClient> Clients { get; set; }

        #endregion

        public virtual object Clone()
        {
            return new OAuthScope
            {
                Name = Name,
                IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                IsStandardScope = IsStandardScope,
                Claims = Claims.Select(c => (OAuthScopeClaim)c.Clone()).ToList()
            };
        }

        public bool Equals(OAuthScope other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var target = obj as OAuthScope;
            if (target == null)
            {
                return false;
            }

            return Equals(target);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}