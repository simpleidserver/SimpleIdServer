// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Domains
{
    public class User : IEquatable<User>
    {
        private static Dictionary<string, KeyValuePair<Action<User, string>, Func<User, object>>> _userClaims = new Dictionary<string, KeyValuePair<Action<User, string>, Func<User, object>>>
        {
            {  JwtRegisteredClaimNames.Sub, new KeyValuePair<Action<User, string>, Func<User, object>>((u, str) => u.Name = str, (u) => u.Name) },
            {  JwtRegisteredClaimNames.Name, new KeyValuePair<Action<User, string>, Func<User, object>>((u, str) => u.Firstname = str, (u) => u.Firstname) },
            {  JwtRegisteredClaimNames.FamilyName, new KeyValuePair<Action<User, string>, Func<User, object>>((u, str) => u.Lastname = str, (u) => u.Lastname) },
            {  JwtRegisteredClaimNames.Email, new KeyValuePair<Action<User, string>, Func<User, object>>((u, str) => u.Email = str, (u) => u.Email) },
            { "email_verified", new KeyValuePair<Action<User, string>, Func<User, object>>((u, str) => u.EmailVerified = bool.Parse(str), (u) => u.EmailVerified) }
        };

        public User()
        {
            Sessions = new List<UserSession>();
            OAuthUserClaims = new List<UserClaim>();
            Credentials = new List<UserCredential>();
            ExternalAuthProviders = new List<UserExternalAuthProvider>();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Firstname { get; set; } = null;
        public string? Lastname { get; set; } = null;
        public string? Email { get; set; } = null;
        public bool EmailVerified { get; set; } = false;
        public string? DeviceRegistrationToken { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<Claim> Claims
        {
            get
            {
                var properties = OAuthUserClaims.Select(c => new Claim(c.Name, c.Value, c.Type)).ToList();
                foreach (var kvp in _userClaims)
                {
                    var val = kvp.Value.Value(this);
                    if (val == null) continue;
                    properties.Add(new Claim(kvp.Key, val.ToString()));
                }

                return properties;
            }
        }
        public UserSession? ActiveSession
        {
            get
            {
                return Sessions.FirstOrDefault(s => s.State == UserSessionStates.Active && DateTime.UtcNow < s.ExpirationDateTime);
            }
        }
        public UserCredential? ActiveOTP
        {
            get
            {
                return Credentials.FirstOrDefault(c => c.CredentialType == UserCredential.OTP && c.IsActive);
            }
        }
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public ICollection<UserClaim> OAuthUserClaims { get; set; } = new List<UserClaim>();
        public ICollection<UserCredential> Credentials { get; set; } = new List<UserCredential>();
        public ICollection<UserExternalAuthProvider> ExternalAuthProviders { get; set; } = new List<UserExternalAuthProvider>();
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        public ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();

        #region User claims

        public bool TryGetUserClaim(string key, out object result)
        {
            result = null;
            if (!_userClaims.ContainsKey(key))
                return false;

            result = _userClaims[key].Value(this);
            if (result == null)
                return false;

            return true;
        }

        #endregion

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

        public void UpdateEmail(string value) => UpdateClaim(JwtRegisteredClaimNames.Email, value);

        public void UpdateName(string value) => UpdateClaim(JwtRegisteredClaimNames.Name, value);

        public void UpdateLastname(string value) => UpdateClaim(JwtRegisteredClaimNames.FamilyName, value);

        public void UpdateClaim(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (_userClaims.ContainsKey(key))
            {
                _userClaims[key].Key(this, value);
                return;
            }

            var claim = OAuthUserClaims.FirstOrDefault(c => c.Name == key);
            if (claim != null)
                claim.Value = value;
            else
            {
                OAuthUserClaims.Add(new UserClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = key,
                    Value = value
                });
            }
        }

        public void AddClaim(string key, string value)
        {
            OAuthUserClaims.Add(new UserClaim
            {
                Id = Guid.NewGuid().ToString(),
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

        public void GenerateHOTP()
        {
            foreach (var cred in Credentials.Where(c => c.CredentialType == UserCredential.OTP))
                cred.IsActive = false;
            var key = KeyGeneration.GenerateRandomKey(20);
            Credentials.Add(new UserCredential
            {
                CredentialType = UserCredential.OTP,
                IsActive = true,
                OTPAlg = OTPAlgs.HOTP,
                Value = key.ConvertFromBase32()
            });
        }

        public void GenerateTOTP()
        {
            foreach (var cred in Credentials.Where(c => c.CredentialType == UserCredential.OTP))
                cred.IsActive = false;
            var key = KeyGeneration.GenerateRandomKey(20);
            Credentials.Add(new UserCredential
            {
                CredentialType = UserCredential.OTP,
                IsActive = true,
                OTPAlg = OTPAlgs.TOTP,
                Value = key.ConvertFromBase32()
            });
        }

        public new static User Create(string sub)
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = sub,
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
