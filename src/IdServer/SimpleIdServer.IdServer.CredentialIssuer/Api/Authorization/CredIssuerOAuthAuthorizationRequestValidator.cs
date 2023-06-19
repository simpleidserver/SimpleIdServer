// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Authorization.Validators;
using SimpleIdServer.IdServer.CredentialIssuer.Validators;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Authorization
{
    public class CredIssuerOAuthAuthorizationRequestValidator : OAuthAuthorizationRequestValidator
    {
        private readonly IAuthorizationDetailsValidator _authDetailsValidator;

        public CredIssuerOAuthAuthorizationRequestValidator(IAuthorizationDetailsValidator authDetailsValidator, IEnumerable<IResponseTypeHandler> responseTypeHandlers, IUserHelper userHelper, IClientRepository clientRepository, IGrantHelper grantHelper, IAmrHelper amrHelper, IExtractRequestHelper extractRequestHelper, IEnumerable<IOAuthResponseMode> oauthResponseModes, IClientHelper clientHelper, IJwtBuilder jwtBuilder, IOptions<IdServerHostOptions> options) : base(responseTypeHandlers, userHelper, clientRepository, grantHelper, amrHelper, extractRequestHelper, oauthResponseModes, clientHelper, jwtBuilder, options)
        {
            _authDetailsValidator = authDetailsValidator;
        }

        public override async Task<AuthorizationRequestValidationResult> ValidateAuthorizationRequest(HandlerContext context, string clientId, CancellationToken cancellationToken)
        {
            var result = await base.ValidateAuthorizationRequest(context, clientId, cancellationToken);
            var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
            _authDetailsValidator.Validate(authDetails);
            return result;
        }
    }
}
