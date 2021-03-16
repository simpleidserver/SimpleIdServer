// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OpenID.Domains
{
    public class BCAuthorize : ICloneable
    {
        public string Id { get; set; }
        public DateTime ExpirationDateTime { get; set; }

        public object Clone()
        {
            return new BCAuthorize
            {
                ExpirationDateTime = ExpirationDateTime,
                Id = Id
            };
        }
    }
}
