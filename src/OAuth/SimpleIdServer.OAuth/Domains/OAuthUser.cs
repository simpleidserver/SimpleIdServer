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
            Sessions = new List<OAuthUserSession>();
            OAuthUserClaims = new List<OAuthUserClaim>();
            Credentials = new List<OAuthUserCredential>();
        }

        public string Id { get; set; }
        public string DeviceRegistrationToken { get; set; }
        public OAuthUserStatus Status { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<Claim> Claims
        {
            get
            {
                return OAuthUserClaims.Select(c => new Claim(c.Name, c.Value, c.Type)).ToList();
            }
        }
        public ICollection<OAuthConsent> Consents { get; set; }
        public ICollection<OAuthUserSession> Sessions { get; set; }
        public ICollection<OAuthUserClaim> OAuthUserClaims { get; set; }
        public ICollection<OAuthUserCredential> Credentials { get; set; }

        public void RejectConsent(string consentId)
        {
            var consent = Consents.SingleOrDefault(c => c.Id == consentId);
            Consents.Remove(consent);
        }

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

        public void UpdateClaim(string key, string value)
        {
            var claim = OAuthUserClaims.FirstOrDefault(c => c.Name == key);
            if (claim != null)
            {
                claim.Value = value;
            }
            else
            {
                OAuthUserClaims.Add(new OAuthUserClaim
                {
                    Name = key,
                    Value = value
                });
            }
        }

        public static OAuthUser Create(string sub = null)
        {
            if (string.IsNullOrWhiteSpace(sub))
            {
                sub = Guid.NewGuid().ToString();
            }

            var user = new OAuthUser
            {
                Id = sub,
                OAuthUserClaims = new List<OAuthUserClaim>
                {
                    new OAuthUserClaim
                    {
                        Name =SimpleIdServer.Jwt.Constants.UserClaims.Subject,
                        Value = sub
                    }
                }
            };
            return user;
        }

        public object Clone()
        {
            return new OAuthUser
            {
                Id = Id,
                Status = Status,
                Credentials = Credentials == null ? new List<OAuthUserCredential>() : Credentials.Select(_ => (OAuthUserCredential)_.Clone()).ToList(),
                OAuthUserClaims = Claims == null ? new List<OAuthUserClaim>() : OAuthUserClaims.Select(_ => (OAuthUserClaim)_.Clone()).ToList(),
                DeviceRegistrationToken = DeviceRegistrationToken,
                Consents = Consents == null ? new List<OAuthConsent>() : Consents.Select(c => (OAuthConsent)c.Clone()).ToList(),
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