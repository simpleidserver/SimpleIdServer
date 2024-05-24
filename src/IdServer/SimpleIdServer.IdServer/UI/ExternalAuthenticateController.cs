// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public class ExternalAuthenticateController : BaseAuthenticateController
    {
        public const string SCHEME_NAME = "scheme";
        public const string RETURN_URL_NAME = "returnUrl";
        private readonly ILogger<ExternalAuthenticateController> _logger;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IUserTransformer _userTransformer;
        private readonly IAuthenticationSchemeProviderRepository _authenticationSchemeProviderRepository;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IRealmRepository _realmRepository;
        private readonly ITransactionBuilder _transactionBuilder;

        public ExternalAuthenticateController(
            IOptions<IdServerHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            ILogger<ExternalAuthenticateController> logger,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IUserTransformer userTransformer,
            IAuthenticationSchemeProviderRepository authenticationSchemeProviderRepository,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IAuthenticationHelper authenticationHelper,
            IRealmRepository realmRepository,
            ITransactionBuilder transactionBuilder,
            IBusControl busControl) : base(clientRepository, userRepository, userSessionRepository, amrHelper, busControl, userTransformer, dataProtectionProvider, authenticationHelper, tokenRepository, jwtBuilder, options)
        {
            _logger = logger;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _userTransformer = userTransformer;
            _authenticationSchemeProviderRepository = authenticationSchemeProviderRepository;
            _authenticationHelper = authenticationHelper;
            _realmRepository = realmRepository;
            _transactionBuilder = transactionBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromRoute] string prefix, string scheme, string returnUrl, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            if (string.IsNullOrWhiteSpace(scheme))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, nameof(scheme)));

            var result = await HttpContext.AuthenticateAsync(scheme);
            if(result is {  Succeeded : true })
            {
                var user = await JustInTimeProvision(prefix, scheme, result, cancellationToken);
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return await Authenticate(prefix, returnUrl, Constants.Areas.Password, user, cancellationToken, false);
                return await Sign(prefix, "~/", Constants.Areas.Password, user, null, cancellationToken, false);
            }

            var items = new Dictionary<string, string>
            {
                { SCHEME_NAME, scheme }
            };
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                items.Add(RETURN_URL_NAME, returnUrl);
            }
            var props = new AuthenticationProperties(items)
            {
                RedirectUri = Url.Action(nameof(Callback)),
            };
            return Challenge(props, scheme);
        }

        [HttpGet]
        public async Task<IActionResult> Callback([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var result = await HttpContext.AuthenticateAsync(Constants.DefaultExternalCookieAuthenticationScheme);
            if (result == null || !result.Succeeded)
            {
                if (result.Failure != null)
                {
                    _logger.LogError(result.Failure.ToString());
                }

                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.BadExternalAuthentication);
            }

            var scheme = result.Properties.Items[SCHEME_NAME];
            var user = await JustInTimeProvision(prefix, scheme, result, cancellationToken);
            await HttpContext.SignOutAsync(Constants.DefaultExternalCookieAuthenticationScheme);
            if (result.Properties.Items.ContainsKey(RETURN_URL_NAME))
                return await Authenticate(prefix, result.Properties.Items[RETURN_URL_NAME], Constants.Areas.Password, user, cancellationToken, false);     

            return await Sign(prefix, "~/", Constants.Areas.Password, user, null, cancellationToken, false);
        }

        private async Task<User> JustInTimeProvision(string realm, string scheme, AuthenticateResult authResult, CancellationToken cancellationToken)
        {
            var principal = authResult.Principal;
            var idProvider = await _authenticationSchemeProviderRepository.Get(realm, scheme, cancellationToken);
            if(idProvider == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnsupportedSchemeProvider, scheme));
            }


            var sub = UserTransformer.ResolveSubject(idProvider, principal);
            if (string.IsNullOrWhiteSpace(sub))
            {
                _logger.LogError("There is not valid subject");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.BadExternalAuthenticationUser);
            }

            var user = await UserRepository.GetByExternalAuthProvider(scheme, sub, realm, cancellationToken);
            if (user == null)
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    _logger.LogInformation($"Start to provision the user '{sub}'");
                    var existingUser = await _authenticationHelper.GetUserByLogin(sub, realm, cancellationToken);
                    if (existingUser != null)
                    {
                        user = existingUser;
                        user.AddExternalAuthProvider(scheme, sub);
                        UserRepository.Update(user);
                    }
                    else
                    {

                        var r = await _realmRepository.Get(realm, cancellationToken);
                        user = _userTransformer.Transform(r, principal, idProvider);
                        user.AddExternalAuthProvider(scheme, sub);
                        UserRepository.Add(user);
                    }

                    await transaction.Commit(cancellationToken);
                    _logger.LogInformation($"Finish to provision the user '{sub}'");
                }
            }

            return user;
        }
    }
}