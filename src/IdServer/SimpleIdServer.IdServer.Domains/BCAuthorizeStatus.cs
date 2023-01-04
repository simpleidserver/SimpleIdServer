// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public enum BCAuthorizeStatus
    {
        Pending = 0,
        Confirmed = 1,
        Pong = 2,
        Sent = 3,
        Rejected = 4
    }
}
