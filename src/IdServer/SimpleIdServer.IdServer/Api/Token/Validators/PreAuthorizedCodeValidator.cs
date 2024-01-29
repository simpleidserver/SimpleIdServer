// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IPreAuthorizedCodeValidator
    {
        Task<PreAuthCode> Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class PreAuthorizedCodeValidator : IPreAuthorizedCodeValidator
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IUserRepository _userRepository;

        public PreAuthorizedCodeValidator(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IClientRepository clientRepository,
            IGrantedTokenHelper grantedTokenHelper,
            IUserRepository userRepository)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _clientRepository = clientRepository;
            _grantedTokenHelper = grantedTokenHelper;
            _userRepository = userRepository;
        }

        public async virtual Task<PreAuthCode> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var preAuthorizedCode = context.Request.RequestData.GetPreAuthorizedCode();
            var transactionCode = context.Request.RequestData.GetTransactionCode();
            if (string.IsNullOrWhiteSpace(preAuthorizedCode)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.PreAuthorizedCode));
            if (context.Client.IsTransactionCodeRequired && string.IsNullOrWhiteSpace(transactionCode)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.TransactionCode));
            var preAuthCode = await _grantedTokenHelper.GetPreAuthCode(preAuthorizedCode, cancellationToken);
            if (preAuthCode == null) throw new OAuthException(ErrorCodes.INVALID_GRANT, ErrorMessages.INVALID_PREAUTHORIZEDCODE);
            if (context.Client.IsTransactionCodeRequired && preAuthCode.TransactionCode != transactionCode) throw new OAuthException(ErrorCodes.INVALID_GRANT, ErrorMessages.INVALID_TRANSACTION_CODE);
            return preAuthCode;
        }
    }
}
