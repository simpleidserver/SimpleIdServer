// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthUser : User, ICloneable
    {
        public OAuthUser() : base()
        {
            Consents = new List<OAuthConsent>();
        }

        public ICollection<OAuthConsent> Consents { get; set; }

        public void RejectConsent(string consentId)
        {
            var consent = Consents.SingleOrDefault(c => c.Id == consentId);
            Consents.Remove(consent);
        }

        public new static OAuthUser Create(string name, string sub = null)
        {
            if (string.IsNullOrWhiteSpace(sub))
            {
                sub = Guid.NewGuid().ToString();
            }

            var user = new OAuthUser
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

        public override object Clone()
        {
            return new OAuthUser
            {
                Id = Id,
                Status = Status,
                Credentials = Credentials == null ? new List<UserCredential>() : Credentials.Select(_ => (UserCredential)_.Clone()).ToList(),
                OAuthUserClaims = Claims == null ? new List<UserClaim>() : OAuthUserClaims.Select(_ => (UserClaim)_.Clone()).ToList(),
                DeviceRegistrationToken = DeviceRegistrationToken,
                Consents = Consents == null ? new List<OAuthConsent>() : Consents.Select(c => (OAuthConsent)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                Sessions = Sessions.Select(s => (UserSession)s.Clone()).ToList(),
                OTPCounter = OTPCounter,
                OTPKey = OTPKey
            };
        }
    }
}