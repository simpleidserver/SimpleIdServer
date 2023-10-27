// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class DeviceCodeHandler : BaseCredentialsHandler
    {
        private readonly IDeviceCodeGrantTypeValidator _validator;
        private readonly IDeviceAuthCodeRepository _deviceAuthCodeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IBusControl _busControl;
        private readonly IDPOPProofValidator _dpopProofValidator;

        public DeviceCodeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IDeviceAuthCodeRepository deviceAuthCodeRepository,
            IUserRepository userRepository,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IEnumerable<ITokenProfile> tokenProfiles,
            IOptions<IdServerHostOptions> options,
            IDeviceCodeGrantTypeValidator validator,
            IAuthenticationHelper authenticationHelper,
            IBusControl busControl,
            IDPOPProofValidator dpopProofValidator) : base(clientAuthenticationHelper, tokenProfiles, options)
        {
            _validator = validator;
            _busControl = busControl;
            _deviceAuthCodeRepository = deviceAuthCodeRepository;
            _userRepository = userRepository;
            _tokenBuilders = tokenBuilders;
            _authenticationHelper = authenticationHelper;
            _dpopProofValidator = dpopProofValidator;
        }

        public override string GrantType => GRANT_TYPE;
        public static string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:device_code";

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = null;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
            {
                try
                {
                    activity?.SetTag("grant_type", GRANT_TYPE);
                    activity?.SetTag("realm", context.Realm);
                    var oauthClient = await AuthenticateClient(context, cancellationToken);
                    context.SetClient(oauthClient);
                    activity?.SetTag("client_id", oauthClient.ClientId);
                    await _dpopProofValidator.Validate(context);
                    var deviceAuthCode = await _validator.Validate(context, cancellationToken);
                    scopeLst = deviceAuthCode.Scopes;
                    activity?.SetTag("scopes", string.Join(",", deviceAuthCode.Scopes));
                    var user = await _authenticationHelper.GetUserByLogin(u => u.Include(u => u.Groups).Include(u => u.OAuthUserClaims).Include(u => u.Realms), deviceAuthCode.UserLogin, context.Realm, cancellationToken);
                    context.SetUser(user);
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(new BuildTokenParameter { Scopes = deviceAuthCode.Scopes }, context, cancellationToken);

                    AddTokenProfile(context);
                    var result = BuildResult(context, deviceAuthCode.Scopes);
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);

                    deviceAuthCode.Send();
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
                catch(OAuthException ex)
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
                finally
                {
                    await _deviceAuthCodeRepository.SaveChanges(cancellationToken);
                }
            }
        }
    }
}
