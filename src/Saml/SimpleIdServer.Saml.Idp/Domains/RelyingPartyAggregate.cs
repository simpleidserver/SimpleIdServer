// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Idp.Domains
{
    public class RelyingPartyAggregate : ICloneable
    {
        public RelyingPartyAggregate()
        {
            ClaimMappings = new List<RelyingPartyClaimMapping>();
        }

        public string Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public int AssertionExpirationTimeInSeconds { get; set; }
        public ICollection<RelyingPartyClaimMapping> ClaimMappings { get; set; }

        public string GetClaim(string userAttribute)
        {
            var claimMapping = ClaimMappings.FirstOrDefault(c => c.UserAttribute == userAttribute);
            if (claimMapping == null)
            {
                return null;
            }

            return claimMapping.ClaimName;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
