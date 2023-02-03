// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UserClaim
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

        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Type { get; set; } = null;
        public User User { get; set; }
    }
}
