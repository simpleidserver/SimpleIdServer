// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OpenID.UI.Infrastructures;
using SimpleIdServer.OpenID.UI.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI
{
    public class AccountsController : Controller
    {
        private readonly ISessionManager _sessionManager;
        private readonly IDataProtector _dataProtector;

        public AccountsController(ISessionManager sessionManager, IDataProtectionProvider dataProtectionProvider)
        {
            _sessionManager = sessionManager;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        }

        public IActionResult Index(string returnUrl)
        {
            var sessions = _sessionManager.FetchTickets(HttpContext);
            var accounts = sessions.Select(sess => new AccountViewModel(sess.Principal.Claims.First(cl => cl.Type == ClaimTypes.NameIdentifier).Value, sess.Properties.ExpiresUtc, sess.Properties.IssuedUtc));
            return View(new AccountsIndexViewModel(returnUrl, accounts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ChooseSessionViewModel chooseSessionViewModel)
        {
            try
            {
                var unprotectedUrl = _dataProtector.Unprotect(chooseSessionViewModel.ReturnUrl);
                var ticket = _sessionManager.FetchTicket(HttpContext, chooseSessionViewModel.AccountName);
                if (ticket == null)
                {
                    return new UnauthorizedResult();
                }

                await HttpContext.SignInAsync(ticket.Principal, new AuthenticationProperties());
                return Redirect(unprotectedUrl);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}" });
            }
        }
    }
}