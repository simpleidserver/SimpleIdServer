// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Extensions;
using SimpleIdServer.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SimpleIdServer.Common.Domains
{
    public class User : ICloneable
    {
        public User()
        {
            Sessions = new List<UserSession>();
            OAuthUserClaims = new List<UserClaim>();
            Credentials = new List<UserCredential>();
        }

        public string Id { get; set; }
        public string DeviceRegistrationToken { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string OTPKey { get; set; }
        public int OTPCounter { get; set; }
        public ICollection<Claim> Claims
        {
            get
            {
                return OAuthUserClaims.Select(c => new Claim(c.Name, c.Value, c.Type)).ToList();
            }
        }
        public ICollection<UserSession> Sessions { get; set; }
        public ICollection<UserClaim> OAuthUserClaims { get; set; }
        public ICollection<UserCredential> Credentials { get; set; }

        public bool AddSession(DateTime expirationDateTime)
        {
            foreach (var session in Sessions)
            {
                session.State = UserSessionStates.Rejected;
            }

            Sessions.Add(new UserSession { SessionId = Guid.NewGuid().ToString(), AuthenticationDateTime = DateTime.UtcNow, ExpirationDateTime = expirationDateTime, State = UserSessionStates.Active });
            return true;
        }

        public byte[] GetOTPKey()
        {
            return OTPKey.ConvertToBase32();
        }

        public void ResetOtp()
        {
            byte[] key = new byte[20];
            using (var rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(key);
                OTPKey = key.ConvertFromBase32();
                OTPCounter = 0;
            }
        }

        public void ResetOtp(byte[] secret)
        {
            ResetOtp(secret, 0);
        }

        public void ResetOtp(byte[] secret, int counter)
        {
            OTPKey = secret.ConvertFromBase32();
            OTPCounter = counter;
        }

        public void IncrementCounter()
        {
            OTPCounter++;
        }

        public UserSession GetActiveSession()
        {
            var currentDateTime = DateTime.UtcNow;
            return Sessions.FirstOrDefault(s => currentDateTime < s.ExpirationDateTime && s.State == UserSessionStates.Active);
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
                OAuthUserClaims.Add(new UserClaim
                {
                    Name = key,
                    Value = value
                });
            }
        }

        public void UpdatePassword(string newPassword)
        {
            var credential = Credentials.First(c => c.CredentialType == "pwd");
            credential.Value = PasswordHelper.ComputeHash(newPassword);
            UpdateDateTime = DateTime.UtcNow;
        }

        public static User Create(string name, string sub = null)
        {
            if (string.IsNullOrWhiteSpace(sub))
            {
                sub = Guid.NewGuid().ToString();
            }

            var user = new User
            {
                Id = sub,
                OAuthUserClaims = new List<UserClaim>
                {
                    new UserClaim
                    {
                        Name = name,
                        Value = sub
                    }
                }
            };
            return user;
        }

        public object Clone()
        {
            return new User
            {
                Id = Id,
                Status = Status,
                Credentials = Credentials == null ? new List<UserCredential>() : Credentials.Select(_ => (UserCredential)_.Clone()).ToList(),
                OAuthUserClaims = Claims == null ? new List<UserClaim>() : OAuthUserClaims.Select(_ => (UserClaim)_.Clone()).ToList(),
                DeviceRegistrationToken = DeviceRegistrationToken,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Sessions = Sessions.Select(s => (UserSession)s.Clone()).ToList(),
                OTPCounter = OTPCounter,
                OTPKey = OTPKey
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

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
