// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Common;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.ExternalEvents;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI
{
    public class ExternalAuthenticateController : BaseAuthenticateController
    {
        private const string SCHEME_NAME = "scheme";
        private const string RETURN_URL_NAME = "returnUrl";
        private readonly ILogger<ExternalAuthenticateController> _logger;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBusControl _busControl;

        public ExternalAuthenticateController(
            IOptions<OpenIDHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IOAuthClientRepository oauthClientRepository,
            IAmrHelper amrHelper,
            IOAuthUserRepository oauthUserRepository,
            ILogger<ExternalAuthenticateController> logger,
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            IServiceProvider serviceProvider,
            IBusControl busControl) : base(options, dataProtectionProvider, oauthClientRepository, amrHelper, oauthUserRepository)
        {
            _logger = logger;
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _serviceProvider = serviceProvider;
            _busControl = busControl;
        }

        [HttpGet]
        public IActionResult Login(string scheme, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, nameof(scheme)));
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
        public async Task<IActionResult> Callback(CancellationToken cancellationToken)
        {
            var result = await HttpContext.AuthenticateAsync(Options.ExternalAuthenticationScheme);
            if (result == null || !result.Succeeded)
            {
                if (result.Failure != null)
                {
                    _logger.LogError(result.Failure.ToString());
                }

                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_EXTERNAL_AUTHENTICATION);
            }

            var user = await JustInTimeProvision(result, cancellationToken);
            await HttpContext.SignOutAsync(Options.ExternalAuthenticationScheme);
            var returnUrl = "~/";
            if (result.Properties.Items.ContainsKey(RETURN_URL_NAME))
            {
                returnUrl = result.Properties.Items[RETURN_URL_NAME];
                returnUrl = Unprotect(returnUrl);
            }

            return await Sign(returnUrl, "externalAuth", user, cancellationToken, true);
        }

        private async Task<OAuthUser> JustInTimeProvision(AuthenticateResult authResult, CancellationToken cancellationToken)
        {
            var scheme = authResult.Properties.Items[SCHEME_NAME];
            var principal = authResult.Principal;
            var sub = GetClaim(principal, Jwt.Constants.UserClaims.Subject);
            if (string.IsNullOrWhiteSpace(sub))
            {
                sub = GetClaim(principal, ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(sub))
                {
                    _logger.LogError("There is not valid subject");
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_EXTERNAL_AUTHENTICATION_USER);
                }
            }

            var user = await OauthUserRepository.FindOAuthUserByExternalAuthProvider(scheme, sub, cancellationToken);
            if (user == null)
            {
                _logger.LogInformation($"Start to provision the user '{sub}'");
                user = principal.BuildOAuthUser(scheme);
                user.UpdateClaim(Jwt.Constants.UserClaims.Role, "visitor");
                await OauthUserRepository.Add(user, cancellationToken);
                await OauthUserRepository.SaveChanges(cancellationToken);
                await _busControl.Publish(new UserAddedEvent(user.Id, 0, CommonConstants.ResourceTypes.OpenIdUser, user.OAuthUserClaims.ToDictionary(c => c.Name, c => c.Value)));
                _logger.LogInformation($"Finish to provision the user '{sub}'");
            }

            return user;
        }

        private static string GetClaim(ClaimsPrincipal principal, string claimType)
        {
            var claim = principal.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
            {
                return null;
            }

            return claim.Value;
        }
    }
}
