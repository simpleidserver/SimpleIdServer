// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

public class DeviceCodeHandler : BaseCredentialsHandler
{
    private readonly IDeviceCodeGrantTypeValidator _validator;
    private readonly IDeviceAuthCodeRepository _deviceAuthCodeRepository;
    private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly IBusControl _busControl;
    private readonly IDPOPProofValidator _dpopProofValidator;
    private readonly ITransactionBuilder _transactionBuilder;

    public DeviceCodeHandler(
        IClientAuthenticationHelper clientAuthenticationHelper,
        IDeviceAuthCodeRepository deviceAuthCodeRepository,
        IEnumerable<ITokenBuilder> tokenBuilders,
        IEnumerable<ITokenProfile> tokenProfiles,
        IOptions<IdServerHostOptions> options,
        IDeviceCodeGrantTypeValidator validator,
        IAuthenticationHelper authenticationHelper,
        IBusControl busControl,
        IDPOPProofValidator dpopProofValidator,
        ITransactionBuilder transactionBuilder) : base(clientAuthenticationHelper, tokenProfiles, options)
    {
        _validator = validator;
        _busControl = busControl;
        _deviceAuthCodeRepository = deviceAuthCodeRepository;
        _tokenBuilders = tokenBuilders;
        _authenticationHelper = authenticationHelper;
        _dpopProofValidator = dpopProofValidator;
        _transactionBuilder = transactionBuilder;
    }

    public override string GrantType => GRANT_TYPE;
    public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:device_code";

    public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<string> scopeLst = null;
        using (var activity = Tracing.BasicActivitySource.StartActivity("DeviceCodeHandler"))
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
                    var deviceAuthCode = await _validator.Validate(context, cancellationToken);
                    scopeLst = deviceAuthCode.Scopes;
                    var user = await _authenticationHelper.GetUserByLogin(deviceAuthCode.UserLogin, context.Realm, cancellationToken);
                    context.SetUser(user, null);
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(new BuildTokenParameter { Scopes = deviceAuthCode.Scopes }, context, cancellationToken);

                    AddTokenProfile(context);
                    var result = BuildResult(context, deviceAuthCode.Scopes);
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);

                    deviceAuthCode.Send();
                    Issue(result, context.Client.ClientId, context.Realm);
                    await _busControl.Publish(new TokenIssuedSuccessEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client.ClientId,
                        Scopes = deviceAuthCode.Scopes,
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
                    Counters.FailToken(context.Client?.ClientId, context.Realm, GrantType);
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
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
