// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthScope : ICloneable, IEquatable<OAuthScope>, IOAuthScope
    {
        public OAuthScope()
        {
        }

        public OAuthScope(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public virtual object Clone()
        {
            return new OAuthScope
            {
                Name = Name,
                IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime
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