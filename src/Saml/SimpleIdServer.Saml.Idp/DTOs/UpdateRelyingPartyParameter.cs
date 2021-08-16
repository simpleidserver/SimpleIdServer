// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Saml.Idp.DTOs
{
    public class UpdateRelyingPartyParameter
    {
        public UpdateRelyingPartyParameter()
        {
            ClaimMappings = new List<RelyingPartyClaimMapping>();
        }

        public string MetadataUrl { get; set; }
        public int? AssertionExpirationTimeInSeconds { get; set; }
        public ICollection<RelyingPartyClaimMapping> ClaimMappings { get; set; }
    }
}
