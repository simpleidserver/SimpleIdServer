// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Saml.Idp.Domains
{
    public class RelyingPartyClaimMapping : ICloneable
    {
        public string UserAttribute { get; set; }
        public string ClaimName { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
