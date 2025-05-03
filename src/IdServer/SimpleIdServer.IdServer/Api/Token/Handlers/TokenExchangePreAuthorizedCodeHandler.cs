// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

public class TokenExchangePreAuthorizedCodeHandler : BaseCredentialsHandler
{
    private readonly ITokenExchangePreAuthorizedCodeValidator _validator;
    private readonly IEnumerable<IUserNotificationService> _userNotificationServices;
    private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;
    private readonly IGrantedTokenHelper _grantedTokenHelper;

    public TokenExchangePreAuthorizedCodeHandler(
        ITokenExchangePreAuthorizedCodeValidator validator,
        IEnumerable<IUserNotificationService> userNotificationServices,
        IEnumerable<IOTPAuthenticator> otpAuthenticators,
        IGrantedTokenHelper grantedTokenHelper,
        IClientAuthenticationHelper clientAuthenticationHelper, 
        IEnumerable<ITokenProfile> tokenProfiles, 
        IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, tokenProfiles, options)
    {
        _validator = validator;
        _userNotificationServices = userNotificationServices;
        _otpAuthenticators = otpAuthenticators;
        _grantedTokenHelper = grantedTokenHelper;
    }

    public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code";
    public override string GrantType => GRANT_TYPE;

    public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        using (var activity = Tracing.TokenActivitySource.StartActivity("Exchange access token against pre-authorized_code"))
        {
            try
            {
                activity?.SetTag("grant_type", GRANT_TYPE);
                activity?.SetTag("realm", context.Realm);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                activity?.SetTag("client_id", oauthClient.ClientId);
                var user = await _validator.Validate(context, cancellationToken);
                var scopes = context.Request.RequestData.GetScopes();
                var preAuthCode = new PreAuthCode
                {
                    ClientId = oauthClient.ClientId,
                    Code = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    Scopes = scopes
                };
                if (oauthClient.IsTransactionCodeRequired)
                {
                    var otpAuthenticator = _otpAuthenticators.Single(o => o.Alg == user.ActiveOTP.OTPAlg);
                    var notificationService = _userNotificationServices.Single(s => s.Name == user.NotificationMode);
                    var otpCode = otpAuthenticator.GenerateOtp(user.ActiveOTP);
                    var transactionCode = otpCode;
                    await notificationService.Send("Transaction code", transactionCode, new Dictionary<string, string>(), user);
                    preAuthCode.TransactionCode = transactionCode;
                }

                await _grantedTokenHelper.AddPreAuthCode(preAuthCode, oauthClient.PreAuthCodeExpirationTimeInSeconds, cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok, "Pre authorized code has been issued");
                return new OkObjectResult(new JsonObject
                {
                    { TokenRequestParameters.PreAuthorizedCode, preAuthCode.Code  }
                });
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
