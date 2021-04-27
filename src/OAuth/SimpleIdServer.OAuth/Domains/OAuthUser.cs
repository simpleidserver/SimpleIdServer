// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthUser : ICloneable, IEquatable<OAuthUser>
    {
        public OAuthUser()
        {
            Consents = new List<OAuthConsent>();
            Credentials = new List<OAuthUserCredential>();
            Claims = new List<Claim>();
            Sessions = new List<OAuthUserSession>();
        }

        public string Id { get; set; }
        public List<Claim> Claims { get; set; }
        public string DeviceRegistrationToken { get; set; }
        public ICollection<OAuthConsent> Consents { get; set; }
        public ICollection<OAuthUserCredential> Credentials { get; set; }
        public ICollection<OAuthUserSession> Sessions { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public bool AddSession(DateTime expirationDateTime)
        {
            foreach(var session in Sessions)
            {
                session.State = OAuthUserSessionStates.Rejected;
            }

            Sessions.Add(new OAuthUserSession { SessionId = Guid.NewGuid().ToString(), AuthenticationDateTime = DateTime.UtcNow, ExpirationDateTime = expirationDateTime, State = OAuthUserSessionStates.Active });
            return true;
        }

        public OAuthUserSession GetActiveSession()
        {
            var currentDateTime = DateTime.UtcNow;
            return Sessions.FirstOrDefault(s => currentDateTime < s.ExpirationDateTime && s.State == OAuthUserSessionStates.Active);
        }

        public object Clone()
        {
            return new OAuthUser
            {
                Id = Id,
                Claims = Claims == null ? new List<Claim>() : Claims.Select(_ => new Claim(_.Type, _.Value, _.ValueType)).ToList(),
                DeviceRegistrationToken = DeviceRegistrationToken,
                Consents = Consents == null ? new List<OAuthConsent>() : Consents.Select(c => (OAuthConsent)c.Clone()).ToList(),
                Credentials = Credentials == null ? new List<OAuthUserCredential>() : Credentials.Select(c => (OAuthUserCredential)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Sessions = Sessions.Select(s => (OAuthUserSession)s.Clone()).ToList()
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