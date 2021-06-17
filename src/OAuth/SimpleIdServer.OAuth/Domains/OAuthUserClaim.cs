// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Security.Claims;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthUserClaim : ICloneable
    {
        public OAuthUserClaim() { }

        public OAuthUserClaim(string name, string value) 
        {
            Name = name;
            Value = value;
        }

        public OAuthUserClaim(string name, string value, string type) : this(name, value)
        {
            Type = type;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public object Clone()
        {
            return new OAuthUserClaim
            {
                Name = Name,
                Value =Value
            };
        }
    }
}
