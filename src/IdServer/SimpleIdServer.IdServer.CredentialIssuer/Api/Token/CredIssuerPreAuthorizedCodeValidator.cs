// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.CredentialIssuer.Validators;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Token
{
    public class CredIssuerPreAuthorizedCodeValidator : PreAuthorizedCodeValidator
    {
        private readonly IAuthorizationDetailsValidator _authorizationDetailsValidator;

        public CredIssuerPreAuthorizedCodeValidator(IAuthorizationDetailsValidator authorizationDetailsValidator, IClientAuthenticationHelper clientAuthenticationHelper, IClientRepository clientRepository, IGrantedTokenHelper grantedTokenHelper, IUserRepository userRepository) : base(clientAuthenticationHelper, clientRepository, grantedTokenHelper, userRepository)
        {
            _authorizationDetailsValidator = authorizationDetailsValidator;
        }

        public async override Task<ValidationResult> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var result = await base.Validate(context, cancellationToken);
            var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
            _authorizationDetailsValidator.Validate(authDetails);
            return result;
        }
    }
}
