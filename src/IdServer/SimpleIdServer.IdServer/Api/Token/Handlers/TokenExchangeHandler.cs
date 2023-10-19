// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

public class TokenExchangeHandler : BaseCredentialsHandler
{
    private readonly ITokenExchangeValidator _tokenExchangeValidator;
    private readonly IBusControl _busControl;
    private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
    private static Dictionary<string, string> _mappingTokenTypeToTokenResponse = new Dictionary<string, string>
    {
        { IdTokenTypeParser.NAME, TokenResponseParameters.IdToken },
        { AccessTokenTypeParser.NAME, TokenResponseParameters.AccessToken }
    };

    public TokenExchangeHandler(ITokenExchangeValidator tokenExchangeValidator, IBusControl busControl, IEnumerable<ITokenBuilder> tokenBuilders, IClientAuthenticationHelper clientAuthenticationHelper, IEnumerable<ITokenProfile> tokenProfiles, IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, tokenProfiles, options)
    {
        _tokenExchangeValidator = tokenExchangeValidator;
        _busControl = busControl;
        _tokenBuilders = tokenBuilders;
    }

    public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:token-exchange";
    public override string GrantType => GRANT_TYPE;

    public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
        {
            activity?.SetTag("grant_type", GRANT_TYPE);
            activity?.SetTag("realm", context.Realm);
            var oauthClient = await AuthenticateClient(context, cancellationToken);
            context.SetClient(oauthClient);
            var validationResult = await _tokenExchangeValidator.Validate(context, cancellationToken);
            activity?.SetTag("scopes", string.Join(",", validationResult.Scopes));

            // BUILD ACCESS TOKEN OR ID_TOKEN.
            var parameter = new BuildTokenParameter { Audiences = validationResult.Audiences, Scopes = validationResult.Scopes };
            var tokenResponseParameter = _mappingTokenTypeToTokenResponse[validationResult.TokenType];
            var result = BuildResult(context, validationResult.Scopes);
            await _tokenBuilders.First(t => t.Name == tokenResponseParameter).Build(parameter, context, cancellationToken);

            await _busControl.Publish(new TokenIssuedSuccessEvent
            {
                GrantType = GRANT_TYPE,
                ClientId = context.Client.ClientId,
                Scopes = validationResult.Scopes,
                Realm = context.Realm
            });

            throw new NotImplementedException();
        }
    }
}
