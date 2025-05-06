// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class PasswordHandler : BaseCredentialsHandler
    {
        private readonly IPasswordGrantTypeValidator _passwordGrantTypeValidator;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IGrantHelper _audienceHelper;
        private readonly IBusControl _busControl;
        private readonly IDPOPProofValidator _dpopProofValidator;
        private readonly IEnumerable<IUserAuthenticationService> _userAuthenticationServices;

        public PasswordHandler(
            IPasswordGrantTypeValidator passwordGrantTypeValidator,
            IEnumerable<ITokenProfile> tokenProfiles,
            IOptions<IdServerHostOptions> options,
            IEnumerable<ITokenBuilder> tokenBuilders, 
            IClientAuthenticationHelper clientAuthenticationHelper,
            IGrantHelper audienceHelper,
            IBusControl busControl,
            IDPOPProofValidator dpopProofValidator,
            IEnumerable<IUserAuthenticationService> userAuthenticationServices) : base(clientAuthenticationHelper, tokenProfiles, options)
        {
            _passwordGrantTypeValidator = passwordGrantTypeValidator;
            _tokenBuilders = tokenBuilders;
            _audienceHelper = audienceHelper;
            _busControl = busControl;
            _dpopProofValidator = dpopProofValidator;
            _userAuthenticationServices = userAuthenticationServices;
        }

        public const string GRANT_TYPE = "password";
        public override string GrantType => GRANT_TYPE;

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = new string[0];
            using (var activity = Tracing.BasicActivitySource.StartActivity("PasswordHandler"))
            {
                try
                {
                    activity?.SetTag(Tracing.IdserverTagNames.GrantType, GRANT_TYPE);
                    activity?.SetTag(Tracing.CommonTagNames.Realm, context.Realm);
                    _passwordGrantTypeValidator.Validate(context);
                    var oauthClient = await AuthenticateClient(context, cancellationToken);
                    context.SetClient(oauthClient);
                    await _dpopProofValidator.Validate(context);
                    var scopes = ScopeHelper.Validate(context.Request.RequestData.GetStr(TokenRequestParameters.Scope), oauthClient.Scopes.Select(s => s.Name));
                    var resources = context.Request.RequestData.GetResourcesFromAuthorizationRequest();
                    var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
                    var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, new List<string>(), authDetails, cancellationToken);
                    scopeLst = extractionResult.Scopes;
                    var userName = context.Request.RequestData.GetStr(TokenRequestParameters.Username);
                    var password = context.Request.RequestData.GetStr(TokenRequestParameters.Password);
                    var authenticationService = _userAuthenticationServices.Single(s => s.Amr == Constants.AreaPwd);
                    var userAuthenticationResult = await authenticationService.Validate(context.Realm ?? Constants.DefaultRealm, string.Empty, new AuthenticatePasswordViewModel
                    {
                        Login = userName,
                        Password = password
                    }, cancellationToken);
                    if (userAuthenticationResult.Status != ValidationStatus.AUTHENTICATE) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, Global.BadUserCredential);
                    var user = userAuthenticationResult.AuthenticatedUser;
                    context.SetUser(user, null);
                    var result = BuildResult(context, extractionResult.Scopes);
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Scopes = extractionResult.Scopes, Audiences = extractionResult.Audiences }, context, cancellationToken);

                    AddTokenProfile(context);
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);
                    Issue(result, context.Client.ClientId, context.Realm);
                    await _busControl.Publish(new TokenIssuedSuccessEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client.ClientId,
                        Scopes = extractionResult.Scopes,
                        Realm = context.Realm
                    });
                    activity?.SetStatus(ActivityStatusCode.Ok, "Token has been issued");
                    return new OkObjectResult(result);
                }
                catch (OAuthUnauthorizedException ex)
                {
                    await _busControl.Publish(new TokenIssuedFailureEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client?.ClientId,
                        Scopes = scopeLst,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message
                    });
                    Counters.FailToken(context.Client?.ClientId, context.Realm, GrantType);
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
                }
                catch (OAuthException ex)
                {
                    await _busControl.Publish(new TokenIssuedFailureEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client?.ClientId,
                        Scopes = scopeLst,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message
                    });
                    Counters.FailToken(context.Client?.ClientId, context.Realm, GrantType);
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
                }
            }
        }
    }
}