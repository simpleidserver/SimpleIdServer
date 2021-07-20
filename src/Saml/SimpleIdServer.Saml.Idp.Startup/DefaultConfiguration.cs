// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Saml.Idp.Startup
{
    public class DefaultConfiguration
    {
        public static ICollection<RelyingPartyAggregate> RelyingParties = new List<RelyingPartyAggregate>
        {
            new RelyingPartyAggregate
            {
                Id = "urn:rp",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            }
        };
    }
}
