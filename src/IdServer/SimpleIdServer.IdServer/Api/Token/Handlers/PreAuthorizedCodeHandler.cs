// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

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
    private readonly IUserRepository _userRepository;

    public PreAuthorizedCodeHandler(
        IPreAuthorizedCodeValidator validator,
        IBusControl busControl,
        IClientAuthenticationHelper clientAuthenticationHelper,
        IEnumerable<ITokenProfile> tokenProfiles,
        IEnumerable<ITokenBuilder> tokenBuilders,
        IGrantHelper audienceHelper,
        IGrantedTokenHelper grantedTokenHelper,
        IDPOPProofValidator dpopProofValidator,
        IUserRepository userRepository,
        IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, tokenProfiles, options)
    {
        _validator = validator;
        _busControl = busControl;
        _tokenBuilders = tokenBuilders;
        _audienceHelper = audienceHelper;
        _grantedTokenHelper = grantedTokenHelper;
        _dpopProofValidator = dpopProofValidator;
        _userRepository = userRepository;
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
                var clientId = context.Request.RequestData.GetClientId();
                var oauthClient = new Client();
                bool isClientExists = false;
                if(!string.IsNullOrWhiteSpace(clientId))
                {
                    oauthClient = await AuthenticateClient(context, cancellationToken);
                    isClientExists = true;
                }

                context.SetClient(oauthClient);
                var preAuthCode = await _validator.Validate(context, cancellationToken);
                if (!isClientExists)
                    oauthClient.ClientId = preAuthCode.ClientId;
                activity?.SetTag("client_id", oauthClient.Id);
                await _dpopProofValidator.Validate(context);
                var scopes = preAuthCode.Scopes;
                var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, new List<string>(), new List<string>(), new List<AuthorizationData>(), cancellationToken);
                scopeLst = extractionResult.Scopes;
                activity?.SetTag("scopes", string.Join(",", extractionResult.Scopes));
                var result = BuildResult(context, extractionResult.Scopes);
                var credentialNonce = Guid.NewGuid().ToString();
                var parameter = new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Audiences = extractionResult.Audiences, Scopes = extractionResult.Scopes };
                var accessTokenBuilder = _tokenBuilders.Single(t => t.Name == TokenResponseParameters.AccessToken);
                var user = await _userRepository.GetById(preAuthCode.UserId, context.Realm, cancellationToken);
                context.SetUser(user, null);
                await accessTokenBuilder.Build(parameter, context, cancellationToken);
                AddTokenProfile(context);
                foreach (var kvp in context.Response.Parameters)
                    result.Add(kvp.Key, kvp.Value);

                await _grantedTokenHelper.RemovePreAuthCode(preAuthCode.Code, cancellationToken);
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
}
