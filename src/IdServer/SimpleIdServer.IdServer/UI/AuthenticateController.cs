// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.AuthProviders;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Area(Constants.Areas.Password)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IPasswordAuthService _passwordAuthService;

        public AuthenticateController(IAuthenticationSchemeProvider authenticationSchemeProvider, IPasswordAuthService passwordAuthService, IOptions<IdServerHostOptions> options, IDataProtectionProvider dataProtectionProvider, IClientRepository clientRepository, IAmrHelper amrHelper, IUserRepository userRepository, IUserTransformer userTransformer, IBusControl busControl) : base(options, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _passwordAuthService = passwordAuthService;
        }


        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            try
            {
                var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
                var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
                if (IsProtected(returnUrl))
                {
                    var query = ExtractQuery(returnUrl);
                    var clientId = query.GetClientIdFromAuthorizationRequest();
                    var str = prefix ?? Constants.DefaultRealm;
                    var client = await ClientRepository.Query().Include(c => c.Realms).Include(c => c.Translations).FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name ==  str), cancellationToken);
                    var loginHint = query.GetLoginHintFromAuthorizationRequest();
                    return View(new AuthenticatePasswordViewModel(loginHint,
                        returnUrl,
                        prefix,
                        client.ClientName,
                        client.LogoUri,
                        client.TosUri,
                        client.PolicyUri,
                        externalIdProviders.Select(e => new ExternalIdProvider
                        {
                            AuthenticationScheme = e.Name,
                            DisplayName = e.DisplayName
                        }).ToList()));
                }

                return View(new AuthenticatePasswordViewModel(
                    returnUrl,
                    prefix,
                    externalIdProviders.Select(e => new ExternalIdProvider
                    {
                        AuthenticationScheme = e.Name,
                        DisplayName = e.DisplayName
                    }).ToList()));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, AuthenticatePasswordViewModel viewModel, CancellationToken token)
        {
            viewModel.Realm = prefix;
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            viewModel.Check(ModelState);
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var user = await _passwordAuthService.Authenticate(prefix, viewModel.Login, viewModel.Password, token);
                return await Authenticate(prefix, viewModel.ReturnUrl, Constants.Areas.Password, user, token, viewModel.RememberLogin);
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("invalid_request", "invalid_request");
                return View(viewModel);
            }
            catch (BaseUIException ex)
            {
                ModelState.AddModelError(ex.Code, ex.Code);
                await Bus.Publish(new UserLoginFailureEvent
                {
                    Realm = prefix,
                    Amr = Constants.Areas.Password,
                    Login = viewModel.Login
                });
                return View(viewModel);
            }
        }
    }
}
