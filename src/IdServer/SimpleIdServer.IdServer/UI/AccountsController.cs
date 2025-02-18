// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Infrastructures;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public class AccountsController : Controller
    {
        private readonly ISessionManager _sessionManager;
        private readonly ILanguageRepository _languageRepository;
        private readonly IDataProtector _dataProtector;
        private readonly IBusControl _busControl;
        private readonly IRealmStore _realmStore;

        public AccountsController(ISessionManager sessionManager, ILanguageRepository languageRepository, IDataProtectionProvider dataProtectionProvider, IBusControl busControl, IRealmStore realmStore)
        {
            _sessionManager = sessionManager;
            _languageRepository = languageRepository;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _busControl = busControl;
            _realmStore = realmStore;
        }

        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            var sessions = _sessionManager.FetchTickets(HttpContext);
            var result = await GetAccounts(returnUrl, cancellationToken);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ChooseSessionViewModel chooseSessionViewModel, CancellationToken cancellationToken)
        {
            var realm = _realmStore.Realm ?? Constants.DefaultRealm;
            AccountsIndexViewModel accounts = null;
            try
            {
                switch(chooseSessionViewModel.Action)
                {
                    case "Reject":
                        var sessionResult = await _sessionManager.Revoke(HttpContext.Request, chooseSessionViewModel.AccountName, realm, cancellationToken);
                        Response.Cookies.Delete(sessionResult.SessionCookieName);
                        var items = new Dictionary<string, string>
                        {
                            { Constants.LogoutUserKey, chooseSessionViewModel.AccountName }
                        };
                        await HttpContext.SignOutAsync(new AuthenticationProperties(items));
                        await _busControl.Publish(new UserLogoutSuccessEvent
                        {
                            UserName = chooseSessionViewModel.AccountName,
                            Realm = realm
                        });
                        accounts = await GetAccounts(string.Empty, cancellationToken);
                        ViewBag.IsSessionRejected = "true";
                        accounts.Accounts = accounts.Accounts.Where(s => s.Name != chooseSessionViewModel.AccountName).ToList();
                        break;
                    default:
                        var ticket = _sessionManager.FetchTicket(HttpContext, chooseSessionViewModel.AccountName);
                        if (ticket == null) return new UnauthorizedResult();
                        await HttpContext.SignInAsync(ticket.Principal, new AuthenticationProperties());
                        if (!string.IsNullOrWhiteSpace(chooseSessionViewModel.ReturnUrl))
                        {
                            var unprotectedUrl = _dataProtector.Unprotect(chooseSessionViewModel.ReturnUrl);
                            if (ticket == null)
                            {
                                return new UnauthorizedResult();
                            }

                            return Redirect(unprotectedUrl);
                        }

                        accounts = await GetAccounts(string.Empty, cancellationToken);
                        ViewBag.IsUserAccountSwitched = "true";
                        break;
                }
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }

            return View(accounts);
        }

        private async Task<AccountsIndexViewModel> GetAccounts(string returnUrl, CancellationToken cancellationToken)
        {
            var sessions = _sessionManager.FetchTickets(HttpContext);
            var accounts = sessions.Select(sess => new AccountViewModel(sess.Principal.Claims.First(cl => cl.Type == ClaimTypes.NameIdentifier).Value, sess.Properties.ExpiresUtc, sess.Properties.IssuedUtc));
            accounts = accounts.Where(a => a.ExpiresUct >= DateTime.UtcNow);
            var languages = await _languageRepository.GetAll(cancellationToken);
            return new AccountsIndexViewModel(returnUrl, accounts, languages);
        }
    }
}