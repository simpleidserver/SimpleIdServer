// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Auth;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Infrastructures
{
    public interface ISessionManager
    {
        AuthenticationTicket FetchTicket(HttpContext context, string name);
        IEnumerable<AuthenticationTicket> FetchTickets(HttpContext context);
        IEnumerable<(AuthenticationTicket ticket, string realm)> FetchTicketsFromAllRealm(HttpContext context);
        Task<RevokeSessionResult> Revoke(HttpRequest request, string user, string realm, CancellationToken cancellationToken);
    }

    public class RevokeSessionResult
    {
        public IEnumerable<string> FrontChannelLogouts { get; set; }
        public string SessionCookieName { get; set;}
    }

    public class SessionManager : ISessionManager
    {
        private static string COOKIE_NAME = CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme;
        private readonly IdServerHostOptions _options;
        private readonly TicketDataFormat _ticketDataFormat;
        private readonly IClientRepository _clientRepository;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IRealmStore _realmStore;

        public SessionManager(
            IOptionsMonitor<IdServerHostOptions> options, 
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IUserSessionResitory userSessionRepository,
            ITransactionBuilder transactionBuilder,
            IRecurringJobManager recurringJobManager,
            IRealmStore realmStore)
        {
            _options = options.CurrentValue;
            _ticketDataFormat = new TicketDataFormat(dataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2"));
            _clientRepository = clientRepository;
            _userSessionRepository = userSessionRepository;
            _transactionBuilder = transactionBuilder;
            _recurringJobManager = recurringJobManager;
            _realmStore = realmStore;
        }

        public AuthenticationTicket FetchTicket(HttpContext context, string name)
        {
            var cookie = context.Request.Cookies.FirstOrDefault(c => c.Key.StartsWith($"{COOKIE_NAME}-{name.SanitizeNameIdentifier()}"));
            if (cookie.Equals(default(KeyValuePair<string, string>))) return null;
            return _ticketDataFormat.Unprotect(cookie.Value);
        }

        public IEnumerable<AuthenticationTicket> FetchTickets(HttpContext context)
        {
            var filteredCookies = context.Request.Cookies.Where(c => c.Key.StartsWith($"{IdServerCookieAuthenticationHandler.GetCookieName(_realmStore.Realm, COOKIE_NAME)}-"));
            var result = new List<AuthenticationTicket>();
            foreach (var filterCookie in filteredCookies)
            {
                try
                {
                    var ticket = _ticketDataFormat.Unprotect(filterCookie.Value);
                    if(ticket != null) result.Add(ticket);
                }
                catch (Exception) { }
            }

            return result;
        }

        public IEnumerable<(AuthenticationTicket ticket, string realm)> FetchTicketsFromAllRealm(HttpContext context)
        {
            var regex = new Regex(_options.UseRealm ? $"{COOKIE_NAME}\\.\\w*\\-\\w*" : $"{COOKIE_NAME}\\-\\w*");
            var filteredCookies = context.Request.Cookies.Where(c => regex.IsMatch(c.Key));
            var result = new List<(AuthenticationTicket ticket, string realm)>();
            foreach (var filterCookie in filteredCookies)
            {
                try
                {
                    var ticket = _ticketDataFormat.Unprotect(filterCookie.Value);
                    if (ticket == null) continue;
                    var realm = filterCookie.Key.Split('-').First().Split('.').Last();
                    result.Add((ticket, realm));
                }
                catch (Exception) { }
            }

            return result;
        }

        public async Task<RevokeSessionResult> Revoke(HttpRequest request, string user, string realm, CancellationToken cancellationToken)
        {
            realm = realm ?? Constants.DefaultRealm;
            var sessionCookieName = _options.GetSessionCookieName(_realmStore.Realm, user);
            var kvp = request.Cookies.SingleOrDefault(c => c.Key == sessionCookieName);
            IEnumerable<string> frontChannelLogouts = new List<string>();
            if (!string.IsNullOrWhiteSpace(kvp.Key))
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    var activeSession = await _userSessionRepository.GetById(kvp.Value, realm, cancellationToken);
                    var targetedClients = await _clientRepository.GetByClientIdsAndExistingFrontchannelLogoutUri(realm, activeSession.ClientIds, cancellationToken);
                    frontChannelLogouts = targetedClients.Select(c => BuildFrontChannelLogoutUrl(request, c, kvp.Value));
                    if (activeSession != null && !activeSession.IsClientsNotified)
                    {
                        activeSession.State = UserSessionStates.Rejected;
                        _userSessionRepository.Update(activeSession);
                        await transaction.Commit(cancellationToken);
                        _recurringJobManager.Trigger(nameof(UserSessionJob));
                    }
                }
            }

            return new RevokeSessionResult { FrontChannelLogouts = frontChannelLogouts, SessionCookieName = sessionCookieName };
        }

        private string BuildFrontChannelLogoutUrl(HttpRequest request, Client client, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(client.FrontChannelLogoutUri))
                return null;

            var url = client.FrontChannelLogoutUri;
            if (client.FrontChannelLogoutSessionRequired)
            {
                var issuer = HandlerContext.GetIssuer(_realmStore.Realm, request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
                url = QueryHelpers.AddQueryString(url, new Dictionary<string, string>
                {
                    { JwtRegisteredClaimNames.Iss, issuer },
                    { JwtRegisteredClaimNames.Sid, sessionId }
                });
            }

            return url;
        }
    }
}
