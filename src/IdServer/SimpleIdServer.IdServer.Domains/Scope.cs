// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class Scope : IEquatable<Scope>
    {
        public Scope()
        {
            ClaimMappers = new List<ScopeClaimMapper>();
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
        public ICollection<ScopeClaimMapper> ClaimMappers { get; set; }= new List<ScopeClaimMapper>();
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<ApiResource> ApiResources { get; set; } = new List<ApiResource>();

        public static Scope Create(string scopeName)
        {
            return new Scope
            {
                Name = scopeName,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
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
