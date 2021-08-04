// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Common.Domains
{
    public class UserExternalAuthProvider : ICloneable
    {
        #region Properties

        public string Scheme { get; set; }
        public string Subject { get; set; }
        public DateTime CreateDateTime { get; set; }

        #endregion

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
