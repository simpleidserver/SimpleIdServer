// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Saml.Idp.Domains
{
    public class RelyingPartyAggregate
    {
        public string Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
