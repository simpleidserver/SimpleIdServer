// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    /// <summary>
    /// The code representing the Credential Issuer's authorization for the Wallet to obtain Credentials of a certain type.
    /// </summary>
    public class PreAuthorizedCodeHandler : BaseCredentialsHandler
    {
        private readonly IPreAuthorizedCodeValidator _validator;
        private readonly IBusControl _busControl;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IGrantHelper _audienceHelper;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IDPOPProofValidator _dpopProofValidator;

        public PreAuthorizedCodeHandler(
            IPreAuthorizedCodeValidator validator,
            IBusControl busControl,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IGrantHelper audienceHelper,
            IGrantedTokenHelper grantedTokenHelper,
            IDPOPProofValidator dpopProofValidator,
            IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, tokenProfiles, options)
        {
            _validator = validator;
            _busControl = busControl;
            _tokenBuilders = tokenBuilders;
            _audienceHelper = audienceHelper;
            _grantedTokenHelper = grantedTokenHelper;
            _dpopProofValidator = dpopProofValidator;
        }

        public override string GrantType => GRANT_TYPE;
        public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:pre-authorized_code";

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = new string[0];
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
            {
                try
                {
                    activity?.SetTag("grant_type", GRANT_TYPE);
                    activity?.SetTag("realm", context.Realm);
                    var validationResult = await _validator.Validate(context, cancellationToken);
                    activity?.SetTag("client_id", validationResult.Client.Id);
                    await _dpopProofValidator.Validate(context);
                    var scopes = ScopeHelper.Validate(context.Request.RequestData.GetStr(TokenRequestParameters.Scope), validationResult.Client.Scopes.Select(s => s.Name));
                    var resources = context.Request.RequestData.GetResourcesFromAuthorizationRequest();
                    var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
                    var preAuthorizedCode = context.Request.RequestData.GetPreAuthorizedCode();
                    var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, new List<string>(), authDetails, cancellationToken);
                    scopeLst = extractionResult.Scopes;
                    activity?.SetTag("scopes", string.Join(",", extractionResult.Scopes));
                    var result = BuildResult(context, extractionResult.Scopes);
                    var credentialNonce = Guid.NewGuid().ToString();
                    var parameter = new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Audiences = extractionResult.Audiences, Scopes = extractionResult.Scopes };
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(parameter, context, cancellationToken);

                    AddTokenProfile(context);
                    var accessToken = context.Response.Parameters[TokenResponseParameters.AccessToken];
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);

                    await Enrich(context, result, cancellationToken);
                    await _grantedTokenHelper.RemovePreAuthCode(preAuthorizedCode, cancellationToken);
                    await _busControl.Publish(new TokenIssuedSuccessEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client.ClientId,
                        Realm = context.Realm
                    });
                    activity?.SetStatus(ActivityStatusCode.Ok, "Token has been issued");
                    return new OkObjectResult(result);
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
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
                }
            }
        }

        protected virtual Task Enrich(HandlerContext handlerContext, JsonObject result, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
