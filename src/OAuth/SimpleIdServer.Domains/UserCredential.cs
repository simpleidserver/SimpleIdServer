// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Domains
{
    public class UserCredential : ICloneable
    {
        public string CredentialType { get; set; } = null!;
        public string Value { get; set; } = null!;

        public object Clone()
        {
            return new UserCredential
            {
                CredentialType = CredentialType,
                Value = Value
            };
        }
    }
}
