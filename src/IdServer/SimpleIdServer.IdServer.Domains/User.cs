// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Domains
{
    public class User : IEquatable<User>
    {
        public User()
        {
            Sessions = new List<UserSession>();
            OAuthUserClaims = new List<UserClaim>();
            Credentials = new List<UserCredential>();
            ExternalAuthProviders = new List<UserExternalAuthProvider>();
        }

        public string Id { get; set; } = null!;
        public string? DeviceRegistrationToken { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string? OTPKey { get; set; }
        public int OTPCounter { get; set; }
        public ICollection<Claim> Claims
        {
            get
            {
                return OAuthUserClaims.Select(c => new Claim(c.Name, c.Value, c.Type)).ToList();
            }
        }
        public UserSession? ActiveSession
        {
            get
            {
                return Sessions.FirstOrDefault(s => s.State == UserSessionStates.Active);
            }
        }
        public string? Email
        {
            get
            {
                return OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.Email)?.Value;
            }
        }
        public string? Name
        {
            get
            {
                return OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.Name)?.Value;
            }
        }
        public string? FamilyName
        {
            get
            {
                return OAuthUserClaims.FirstOrDefault(c => c.Name == JwtRegisteredClaimNames.FamilyName)?.Value;
            }
        }
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public ICollection<UserClaim> OAuthUserClaims { get; set; } = new List<UserClaim>();
        public ICollection<UserCredential> Credentials { get; set; } = new List<UserCredential>();
        public ICollection<UserExternalAuthProvider> ExternalAuthProviders { get; set; } = new List<UserExternalAuthProvider>();
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        public ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();

        public void RejectConsent(string consentId)
        {
            var consent = Consents.Single(c => c.Id == consentId);
            Consents.Remove(consent);
        }

        public bool AddSession(DateTime expirationDateTime)
        {
            foreach (var session in Sessions)
                session.State = UserSessionStates.Rejected;

            Sessions.Add(new UserSession { SessionId = Guid.NewGuid().ToString(), AuthenticationDateTime = DateTime.UtcNow, ExpirationDateTime = expirationDateTime, State = UserSessionStates.Active });
            return true;
        }

        public void UpdateClaims(ICollection<UserClaim> claims)
        {
            OAuthUserClaims.Clear();
            foreach (var claim in claims)
                OAuthUserClaims.Add(claim);
        }

        public void UpdateEmail(string value) => UpdateClaim(JwtRegisteredClaimNames.Email, value);

        public void UpdateName(string value) => UpdateClaim(JwtRegisteredClaimNames.Name, value);

        public void UpdateLastname(string value) => UpdateClaim(JwtRegisteredClaimNames.FamilyName, value);

        public void UpdateClaim(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var claim = OAuthUserClaims.FirstOrDefault(c => c.Name == key);
            if (claim != null)
                claim.Value = value;
            else
            {
                OAuthUserClaims.Add(new UserClaim
                {
                    Name = key,
                    Value = value
                });
            }
        }

        public void AddClaim(string key, string value)
        {
            OAuthUserClaims.Add(new UserClaim
            {
                Name = key,
                Value = value
            });
        }

        public void AddExternalAuthProvider(string scheme, string subject)
        {
            ExternalAuthProviders.Add(new UserExternalAuthProvider
            {
                CreateDateTime = DateTime.UtcNow,
                Scheme = scheme,
                Subject = subject
            });
        }

        public new static User Create(string sub)
        {
            return new User
            {
                Id = sub,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
        }

        public bool Equals(User other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var u = obj as User;
            if (u == null)
            {
                return false;
            }

            return GetHashCode() == u.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
