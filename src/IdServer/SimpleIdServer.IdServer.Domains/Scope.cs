// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class Scope : ICloneable, IEquatable<Scope>
    {
        public Scope()
        {
            Claims = new List<ScopeClaim>();
        }

        public Scope(string name) : this()
        {
            Name = name;
        }

        public string Name { get; set; } = null!;
        public bool IsStandardScope { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Array of strings that specifies the claims.
        /// </summary>
        public ICollection<ScopeClaim> Claims { get; set; }= new List<ScopeClaim>();
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public static Scope Create(string scopeName)
        {
            return new Scope
            {
                Name = scopeName,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
        }

        public virtual object Clone()
        {
            return new Scope
            {
                Name = Name,
                IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                IsStandardScope = IsStandardScope,
                Claims = Claims.Select(c => (ScopeClaim)c.Clone()).ToList()
            };
        }

        public bool Equals(Scope other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var target = obj as Scope;
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
