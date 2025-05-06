// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class CIBAHandler : BaseCredentialsHandler
    {
        private readonly ILogger<CIBAHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICIBAGrantTypeValidator _cibaGrantTypeValidator;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IBusControl _busControl;
        private readonly IDPOPProofValidator _dpopProofValidator;
        private readonly ITransactionBuilder _transactionBuilder;

        public CIBAHandler(
            ILogger<CIBAHandler> logger,
            IUserRepository userRepository,
            ICIBAGrantTypeValidator cibaGrantTypeValidator,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IEnumerable<ITokenProfile> tokensProfiles,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IOptions<IdServerHostOptions> options,
            IBCAuthorizeRepository bcAuthorizeRepository,
            IBusControl busControl,
            IDPOPProofValidator dpopProofValidator,
            ITransactionBuilder transactionBuilder) : base(clientAuthenticationHelper, tokensProfiles, options)
        {
            _logger = logger;
            _userRepository = userRepository;
            _cibaGrantTypeValidator = cibaGrantTypeValidator;
            _tokenBuilders = tokenBuilders;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _busControl = busControl;
            _dpopProofValidator = dpopProofValidator;
            _transactionBuilder = transactionBuilder;
        }

        public const string GRANT_TYPE = "urn:openid:params:grant-type:ciba";
        public override string GrantType => GRANT_TYPE;

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = new string[0];
            using (var activity = Tracing.BasicActivitySource.StartActivity("CIBAHandler"))
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    try
                    {
                        activity?.SetTag(Tracing.IdserverTagNames.GrantType, GRANT_TYPE);
                        activity?.SetTag(Tracing.CommonTagNames.Realm, context.Realm);
                        var oauthClient = await AuthenticateClient(context, cancellationToken);
                        context.SetClient(oauthClient);
                        await _dpopProofValidator.Validate(context);
                        var authRequest = await _cibaGrantTypeValidator.Validate(context, cancellationToken);
                        scopeLst = authRequest.Scopes;
                        var user = await _userRepository.GetById(authRequest.UserId, context.Realm, cancellationToken);
                        context.SetUser(user, null);
                        foreach (var tokenBuilder in _tokenBuilders)
                            await tokenBuilder.Build(new BuildTokenParameter { Scopes = authRequest.Scopes, AuthorizationDetails = authRequest.AuthorizationDetails }, context, cancellationToken);

                        AddTokenProfile(context);
                        var result = BuildResult(context, authRequest.Scopes);
                        foreach (var kvp in context.Response.Parameters)
                            result.Add(kvp.Key, kvp.Value);

                        authRequest.Send();
                        _bcAuthorizeRepository.Update(authRequest);
                        Issue(result, context.Client.ClientId, context.Realm);
                        await _busControl.Publish(new TokenIssuedSuccessEvent
                        {
                            GrantType = GRANT_TYPE,
                            ClientId = context.Client.ClientId,
                            Scopes = authRequest.Scopes,
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
                        _logger.LogError(ex.ToString());
                        return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
                    }
                    finally
                    {
                        await transaction.Commit(cancellationToken);
                    }
                }
            }
        }
    }
}
