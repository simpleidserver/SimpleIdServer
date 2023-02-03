// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class UserSession : ICloneable
    {
        public string SessionId { get; set; } = null!;
        public DateTime AuthenticationDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public UserSessionStates State { get; set; }
        public User User { get; set; }

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
