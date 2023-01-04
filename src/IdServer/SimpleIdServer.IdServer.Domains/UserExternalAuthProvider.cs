// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UserExternalAuthProvider : ICloneable
    {
        public string Scheme { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }

        public object Clone()
        {
            return new UserExternalAuthProvider
            {
                Scheme = Scheme,
                Subject = Subject,
                CreateDateTime = CreateDateTime
            };
        }
    }
}
