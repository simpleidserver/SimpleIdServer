// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Authorization.Validators;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
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
        private readonly IBusControl _busControl;
        private readonly IClientRepository _clientRepository;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IGrantHelper _audienceHelper;
        private readonly IUserRepository _userRepository;
        private readonly IdServerHostOptions _options;

        public PreAuthorizedCodeHandler(
            IBusControl busControl,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IGrantedTokenHelper grantedTokenHelper,
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IClientRepository clientRepository,
            IGrantHelper audienceHelper,
            IUserRepository userRepository,
            IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, grantedTokenHelper, options)
        {
            _busControl = busControl;
            _clientRepository = clientRepository;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
            _audienceHelper = audienceHelper;
            _userRepository = userRepository;
            _options = options.Value;
        }

        public override string GrantType => GRANT_TYPE;
        public static string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:pre-authorized_code";

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = new string[0];
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
            {
                try
                {
                    activity?.SetTag("grant_type", GRANT_TYPE);
                    activity?.SetTag("realm", context.Realm);
                    var validationResult = await Validate(activity);
                    var scopes = ScopeHelper.Validate(context.Request.RequestData.GetStr(TokenRequestParameters.Scope), validationResult.Client.Scopes.Select(s => s.Name));
                    var resources = context.Request.RequestData.GetResourcesFromAuthorizationRequest();
                    var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
                    var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, authDetails, cancellationToken);
                    scopeLst = extractionResult.Scopes;
                    activity?.SetTag("scopes", string.Join(",", extractionResult.Scopes));
                    var result = BuildResult(context, extractionResult.Scopes);
                    var credentialNonce = Guid.NewGuid().ToString();
                    var expiresIn = context.Client.CNonceExpirationTimeInSeconds ?? _options.DefaultCNonceExpirationTimeInSeconds.Value;
                    var parameter = new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Audiences = extractionResult.Audiences, Scopes = extractionResult.Scopes };
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(parameter, context, cancellationToken);

                    _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? _options.DefaultTokenProfile)).Enrich(context);
                    var accessToken = context.Response.Parameters[TokenResponseParameters.AccessToken];
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);

                    await AddCredentialParameters(context, result, cancellationToken);
                    await _busControl.Publish(new TokenIssuedSuccessEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client.Id,
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
                        ClientId = context.Client?.Id,
                        Scopes = scopeLst,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message
                    });
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
                }
            }

            async Task<ValidationResult> Validate(Activity activity)
            {
                string clientId;
                if (!TryGetClientId(context, out clientId)) throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.MISSING_CLIENT_ID);
                activity?.SetTag("client_id", clientId);
                var client = await _clientRepository.Query().AsNoTracking().Include(c => c.Scopes).SingleOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
                if (client == null) throw new OAuthException(ErrorCodes.INVALID_CLIENT, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
                if (!client.GrantTypes.Contains(GRANT_TYPE)) throw new OAuthException(ErrorCodes.INVALID_GRANT, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPE, GRANT_TYPE));
                context.SetClient(client);
                var preAuthorizedCode = context.Request.RequestData.GetPreAuthorizedCode();
                var userPin = context.Request.RequestData.GetUserPin();
                if (string.IsNullOrWhiteSpace(preAuthorizedCode)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.PreAuthorizedCode));
                if (client.UserPinRequired && string.IsNullOrWhiteSpace(userPin)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.UserPin));
                var preAuth = await GrantedTokenHelper.GetPreAuthCode(preAuthorizedCode, cancellationToken);
                if (preAuth == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_PREAUTHORIZEDCODE);
                var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
                OAuthAuthorizationRequestValidator.CheckOpenIdCredential(authDetails);
                var user = await _userRepository.Query().AsNoTracking().SingleAsync(u => u.Id == preAuth.UserId, cancellationToken);
                context.SetUser(user);
                return new ValidationResult(client, user);
            }
        }

        private record ValidationResult
        {
            public ValidationResult(Client client, User user)
            {
                Client = client;
                User = user;
            }

            public Client Client { get; private set; }
            public User User { get; private set; }
        }
    }
}
