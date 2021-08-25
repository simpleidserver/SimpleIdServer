// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Saml.Idp.Exceptions
{
    public class RelyingPartyNotFoundException : Exception
    {
        public RelyingPartyNotFoundException(string code, string message) : base(message)
        {
            Code = code;
        }


        public string Code { get; set; }
    }
}
