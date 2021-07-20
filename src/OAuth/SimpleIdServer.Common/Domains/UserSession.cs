// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Common.Domains
{
    public class UserSession : ICloneable
    {
        public string SessionId { get; set; }
        public DateTime AuthenticationDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public UserSessionStates State { get; set; }

        public object Clone()
        {
            return new UserSession
            {
                SessionId = SessionId,
                AuthenticationDateTime = AuthenticationDateTime,
                ExpirationDateTime = ExpirationDateTime,
                State = State
            };
        }
    }
}
