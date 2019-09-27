// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthUser : ICloneable, IEquatable<OAuthUser>
    {
        public OAuthUser()
        {
            Claims = new Dictionary<string, string>();
            Consents = new List<OAuthConsent>();
            Credentials = new List<OAuthUserCredential>();
        }

        public string Id { get; set; }
        public Dictionary<string, string> Claims { get; set; }
        public ICollection<OAuthConsent> Consents { get; set; }
        public ICollection<OAuthUserCredential> Credentials { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public object Clone()
        {
            return new OAuthUser
            {
                Id = Id,
                Claims = Claims == null ? new Dictionary<string, string>() : Claims.ToDictionary(s => s.Key, s => s.Value),
                Consents = Consents == null ? new List<OAuthConsent>() : Consents.Select(c => (OAuthConsent)c.Clone()).ToList(),
                Credentials = Credentials == null ? new List<OAuthUserCredential>() : Credentials.Select(c => (OAuthUserCredential)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime
            };
        }

        public bool Equals(OAuthUser other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}