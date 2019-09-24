// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.UI.Infrastructures
{
    public interface ISessionManager
    {
        IEnumerable<AuthenticationTicket> FetchTickets(HttpContext context);
        AuthenticationTicket FetchTicket(HttpContext context, string name);
    }
}
