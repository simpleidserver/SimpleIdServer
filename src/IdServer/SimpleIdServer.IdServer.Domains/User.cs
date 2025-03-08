// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class User : IEquatable<User>, ICloneable
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

        [JsonPropertyName(UserNames.Id)]
        [UserProperty(true)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(UserNames.Name)]
        [UserProperty(true)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(UserNames.Firstname)]
        [UserProperty(true)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Firstname { get; set; } = null;
        [JsonPropertyName(UserNames.Lastname)]
        [UserProperty(true)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Lastname { get; set; } = null;
        [JsonPropertyName(UserNames.Email)]
        [UserProperty(true)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Email { get; set; } = null;
        [UserProperty(true)]
        [JsonPropertyName(UserNames.EmailVerified)]
        public bool EmailVerified { get; set; } = false;
        [JsonPropertyName(UserNames.DeviceRegistrationToken)]
        public string? DeviceRegistrationToken { get; set; }
        [JsonPropertyName(UserNames.Status)]
        public UserStatus Status { get; set; }
        [JsonPropertyName(UserNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(UserNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(UserNames.Source)]
        public string? Source { get; set; } = null;
        [JsonPropertyName(UserNames.UnblockDateTime)]
        public DateTime? UnblockDateTime { get; set; }
        [JsonPropertyName(UserNames.NbLoginAttempt)]
        public int NbLoginAttempt { get; set; }
        [JsonIgnore]
        public string? IdentityProvisioningId { get; set; } = null;
        [JsonIgnore]
        public string? EncodedPicture { get; set; } = null;
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
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
        [JsonIgnore]
        public UserCredential? ActiveOTP
        {
            get
            {
                return Credentials.FirstOrDefault(c => c.CredentialType == UserCredential.OTP && c.IsActive);
            }
        }
        [JsonIgnore]
        public UserCredential? ActivePassword
        {
            get
            {
                return Credentials.FirstOrDefault(c => c.CredentialType == UserCredential.PWD && c.IsActive);
            }
        }
        [JsonPropertyName(UserNames.NotificationMode)]
        public string NotificationMode { get; set; } = "console";
        [JsonPropertyName(UserNames.Sessions)]
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        [JsonPropertyName(UserNames.Claims)]
        public ICollection<UserClaim> OAuthUserClaims { get; set; } = new List<UserClaim>();
        [JsonPropertyName(UserNames.Credentials)]
        public ICollection<UserCredential> Credentials { get; set; } = new List<UserCredential>();
        [JsonPropertyName(UserNames.ExternalAuthProviders)]
        public ICollection<UserExternalAuthProvider> ExternalAuthProviders { get; set; } = new List<UserExternalAuthProvider>();
        [JsonPropertyName(UserNames.Consents)]
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        [JsonPropertyName(UserNames.Devices)]
        public ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
        [JsonPropertyName(UserNames.Groups)]
        public ICollection<GroupUser> Groups { get; set; } = new List<GroupUser>();
        [JsonIgnore]
        public ICollection<RealmUser> Realms { get; set; } = new List<RealmUser>();
        [JsonIgnore]
        public IdentityProvisioning? IdentityProvisioning { get; set; } = null;


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

        public bool IsBlocked()
        {
            if (Status == UserStatus.BLOCKED && UnblockDateTime >= DateTime.UtcNow) return true;
            if (Status == UserStatus.BLOCKED && UnblockDateTime < DateTime.UtcNow)
            {
                ResetLoginAttempt();
            }

            return false;
        }

        public void Block(int lockTimeInSeconds)
        {
            UnblockDateTime = DateTime.UtcNow.AddSeconds(lockTimeInSeconds);
            Status = UserStatus.BLOCKED;
        }

        public void Unblock()
        {
            ResetLoginAttempt();
        }

        public void LoginAttempt(int maxAttempt, int lockTimeInSeconds)
        {
            NbLoginAttempt++;
            if (NbLoginAttempt >= maxAttempt)
            {
                UnblockDateTime = DateTime.UtcNow.AddSeconds(lockTimeInSeconds);
                Status = UserStatus.BLOCKED;
            }
        }

        public void ResetLoginAttempt()
        {
            NbLoginAttempt = 0;
            UnblockDateTime = null;
            Status = UserStatus.ACTIVATED;
        }

        public Consent AddConsent(string realm, string clientId, ICollection<string> claims, ICollection<AuthorizedScope> scopes, ICollection<AuthorizationData> authorizationDetails)
        {
            var result = new Consent
            {
                Id = Guid.NewGuid().ToString(),
                Realm = realm,
                ClientId = clientId,
                Scopes = scopes,
                Claims = claims,
                AuthorizationDetails = authorizationDetails,
                Status = ConsentStatus.PENDING,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            Consents.Add(result);
            return result;
        }

        public void RejectConsent(string consentId)
        {
            var consent = Consents.Single(c => c.Id == consentId);
            Consents.Remove(consent);
        }

        public void UpdateEmail(string value) => UpdateClaim(JwtRegisteredClaimNames.Email, value);

        public void UpdateName(string value) => UpdateClaim(JwtRegisteredClaimNames.Name, value);

        public void UpdateLastname(string value) => UpdateClaim(JwtRegisteredClaimNames.FamilyName, value);

        public void UpdateClaim(string key, string value)
        {
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
                Id = Guid.NewGuid().ToString(),
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
                Id = Guid.NewGuid().ToString(),
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

        public object Clone()
        {
            return new User
            {
                Name = Name,
                Email = Email,
                EmailVerified = EmailVerified,
                Firstname = Firstname,
                Lastname = Lastname,
                CreateDateTime = CreateDateTime,
                UpdateDateTime= UpdateDateTime,
                Source = Source,
                Id = Id,
                OAuthUserClaims = OAuthUserClaims.Select(c => new UserClaim
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Value = c.Value                    
                }).ToList(),
                Groups = Groups.Select(g => new GroupUser
                {
                    GroupsId = g.GroupsId,
                    UsersId = Id,
                    Group = new Group
                    {
                        CreateDateTime = g.Group.CreateDateTime,
                        Name = g.Group.Name,
                        FullPath = g.Group.FullPath,
                        ParentGroupId = g.Group.ParentGroupId,
                        UpdateDateTime = g.Group.UpdateDateTime,
                        Description = g.Group.Description,
                        Id = g.Group.Id
                    }
                }).ToList()
            };
        }
    }
}
