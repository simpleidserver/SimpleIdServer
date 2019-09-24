// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains.Scopes
{
    public enum ScopeTypes
    {
        ProtectedApi = 0,
        ResourceOwner = 1
    }

    public class OAuthScope : ICloneable, IEquatable<OAuthScope>
    {
        public OAuthScope()
        {
            Claims = new List<string>();
            Descriptions = new List<OAuthTranslation>();
        }

        public OAuthScope(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public ICollection<OAuthTranslation> Descriptions { get; set; }
        public bool IsDisplayedInConsent { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
        public bool IsOpenIdScope { get; set; }
        public bool IsExposed { get; set; }
        public ScopeTypes Type { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<string> Claims { get; set; }

        public object Clone()
        {
            return new OAuthScope
            {
                Name = Name,
                Descriptions = Descriptions.Select(d => (OAuthTranslation)d.Clone()).ToList(),
                IsDisplayedInConsent = IsDisplayedInConsent,
                IsOpenIdScope = IsOpenIdScope,
                IsExposed = IsExposed,
                Type = Type,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
                Claims = Claims == null ? new List<string>() : Claims.ToList()
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