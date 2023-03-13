// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Auth;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.UI.Infrastructures
{
    public interface ISessionManager
    {
        AuthenticationTicket FetchTicket(HttpContext context, string name);
        IEnumerable<AuthenticationTicket> FetchTickets(HttpContext context);
    }

    public class SessionManager : ISessionManager
    {
        private static string COOKIE_NAME = CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme;
        private readonly IdServerHostOptions _options;
        private readonly TicketDataFormat _ticketDataFormat;

        public SessionManager(IOptionsMonitor<IdServerHostOptions> options, IDataProtectionProvider dataProtectionProvider)
        {
            _options = options.CurrentValue;
            _ticketDataFormat = new TicketDataFormat(dataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2"));
        }

        public AuthenticationTicket FetchTicket(HttpContext context, string name)
        {
            var cookie = context.Request.Cookies.FirstOrDefault(c => c.Key.StartsWith($"{COOKIE_NAME}-{name}"));
            if (cookie.Equals(default(KeyValuePair<string, string>))) return null;

            return _ticketDataFormat.Unprotect(cookie.Value);
        }

        public IEnumerable<AuthenticationTicket> FetchTickets(HttpContext context)
        {
            var filteredCookies = context.Request.Cookies.Where(c => c.Key.StartsWith($"{IdServerCookieAuthenticationHandler.GetCookieName(COOKIE_NAME)}-"));
            var result = new List<AuthenticationTicket>();
            foreach (var filterCookie in filteredCookies)
            {
                try
                {
                    var ticket = _ticketDataFormat.Unprotect(filterCookie.Value);
                    result.Add(ticket);
                }
                catch (Exception) { }
            }

            return result;
        }
    }
}
