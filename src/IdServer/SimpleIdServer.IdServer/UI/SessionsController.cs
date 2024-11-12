// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI;

public class SessionsController : Controller
{
    private readonly ISessionManager _sessionManager;
    private readonly IBusControl _busControl;

    public SessionsController(ISessionManager sessionManager, IBusControl busControl)
    {
        _sessionManager = sessionManager;
        _busControl = busControl;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sessions = await GetSessions();
        return View(sessions);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RevokeUserSessionViewModel request, CancellationToken cancellationToken)
    {
        var sessionResult = await _sessionManager.Revoke(HttpContext.Request, request.User, request.Realm, cancellationToken);
        Response.Cookies.Delete(sessionResult.SessionCookieName);
        var items = new Dictionary<string, string>
        {
            { "otherUser", request.User }
        };
        await HttpContext.SignOutAsync(new AuthenticationProperties(items));
        await _busControl.Publish(new UserLogoutSuccessEvent
        {
            UserName = request.User,
            Realm = request.Realm
        });
        var sessions = await GetSessions();
        sessions = sessions.Where(s => s.Realm != request.Realm || s.Name != request.User).ToList();
        return View(sessions);
    }

    private async Task<List<SessionViewModel>> GetSessions()
    {
        var sessions = _sessionManager.FetchTicketsFromAllRealm(HttpContext);
        var accounts = sessions.Select(sess => new SessionViewModel(
            sess.ticket.Principal.Claims.First(cl => cl.Type == ClaimTypes.NameIdentifier).Value,
            sess.realm,
            sess.ticket.Properties.ExpiresUtc,
            sess.ticket.Properties.IssuedUtc)).ToList();
        return accounts;
    }
}