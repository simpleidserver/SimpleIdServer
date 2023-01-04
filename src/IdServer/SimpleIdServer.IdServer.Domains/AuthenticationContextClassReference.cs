// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthenticationContextClassReference : IEquatable<AuthenticationContextClassReference>
    {
        /// <summary>
        /// Name of the Authentication Context Reference.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Human-readable Authentication Context Reference.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Array of strings that specifies the authentication methods.
        /// </summary>
        public IEnumerable<string> AuthenticationMethodReferences { get; set; } = new List<string>();

        public bool Equals(AuthenticationContextClassReference other)
        {
            if (other == null)
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
