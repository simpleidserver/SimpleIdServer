// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResourcePermissionClaim : ICloneable
    {
        public string ClaimType { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            return new UMAResourcePermissionClaim
            {
                ClaimType = ClaimType,
                FriendlyName = FriendlyName,
                Name = Name,
                Value = Value
            };
        }
    }
}
