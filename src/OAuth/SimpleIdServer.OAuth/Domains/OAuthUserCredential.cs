// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthUserCredential : ICloneable
    {
        public OAuthUserCredential() { }
        public string CredentialType { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            return new OAuthUserCredential
            {
                CredentialType = CredentialType,
                Value = Value
            };
        }
    }
}
