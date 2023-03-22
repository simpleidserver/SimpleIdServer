// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization.Validators
{
    public interface IAuthorizationRequestValidator
    {
        Task<AuthorizationRequestValidationResult> ValidateAuthorizationRequest(HandlerContext context, CancellationToken cancellationToken);
        Task<AuthorizationRequestValidationResult> ValidateAuthorizationRequest(HandlerContext context, string clientId, CancellationToken cancellationToken);
        Task ValidateAuthorizationRequestWhenUserIsAuthenticated(GrantRequest request, HandlerContext context, CancellationToken cancellationToken);
    }

    public class AuthorizationRequestValidationResult
    {
        public AuthorizationRequestValidationResult(GrantRequest grantRequest, IEnumerable<IResponseTypeHandler> responseTypes)
        {
            GrantRequest = grantRequest;
            ResponseTypes = responseTypes;
        }

        public GrantRequest GrantRequest { get; private set; }
        public IEnumerable<IResponseTypeHandler> ResponseTypes { get; private set; }
    }
}