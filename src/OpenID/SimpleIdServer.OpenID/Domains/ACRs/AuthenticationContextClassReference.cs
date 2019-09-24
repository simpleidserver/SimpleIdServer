// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OpenID.Domains.ACRs
{
    public class AuthenticationContextClassReference : ICloneable, IEquatable<AuthenticationContextClassReference>
    {
        public AuthenticationContextClassReference()
        {
            AuthenticationMethodReferences = new List<string>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<string> AuthenticationMethodReferences { get; set; }

        public object Clone()
        {
            return new AuthenticationContextClassReference
            {
                Name = Name,
                DisplayName = DisplayName,
                AuthenticationMethodReferences = AuthenticationMethodReferences.ToList()
            };
        }

        public bool Equals(AuthenticationContextClassReference other)
        {
            if(other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}