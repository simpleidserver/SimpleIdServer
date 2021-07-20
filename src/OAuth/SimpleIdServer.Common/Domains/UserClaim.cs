// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Common.Domains
{
    public class UserClaim : ICloneable
    {
        public UserClaim() { }

        public UserClaim(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public UserClaim(string name, string value, string type) : this(name, value)
        {
            Type = type;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public object Clone()
        {
            return new UserClaim
            {
                Name = Name,
                Value = Value
            };
        }
    }
}
