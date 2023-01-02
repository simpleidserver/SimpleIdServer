// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains.DTOs;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Register
{
    public interface IRegisterClientRequestValidator
    {
        void Validate(RegisterClientRequest request);
    }

    public class RegisterClientRequestValidator : IRegisterClientRequestValidator
    {
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;

        public RegisterClientRequestValidator(IEnumerable<IGrantTypeHandler> grantTypeHandlers)
        {
            _grantTypeHandlers = grantTypeHandlers;
        }

        public void Validate(RegisterClientRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, OAuthClientParameters.ClientId));
            if (request.GrantTypes != null && request.GrantTypes.Any())
            {
                var invalidGrantTypes = request.GrantTypes.Where(gt => !_grantTypeHandlers.Any(h => h.GrantType != gt));
                if (invalidGrantTypes.Any()) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPES, string.Join(',', invalidGrantTypes)));
            }
        }
    }
}
