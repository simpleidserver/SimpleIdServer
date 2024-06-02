// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Resources;
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
        private readonly IGrantedTokenHelper _grantedTokenHelper;

        public PreAuthorizedCodeValidator(
            IGrantedTokenHelper grantedTokenHelper)
        {
            _grantedTokenHelper = grantedTokenHelper;
        }

        public async virtual Task<PreAuthCode> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var preAuthorizedCode = context.Request.RequestData.GetPreAuthorizedCode();
            var transactionCode = context.Request.RequestData.GetTransactionCode();
            if (string.IsNullOrWhiteSpace(preAuthorizedCode)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.PreAuthorizedCode));
            if (context.Client.IsTransactionCodeRequired && string.IsNullOrWhiteSpace(transactionCode)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.TransactionCode));
            var preAuthCode = await _grantedTokenHelper.GetPreAuthCode(preAuthorizedCode, cancellationToken);
            if (preAuthCode == null) throw new OAuthException(ErrorCodes.INVALID_GRANT, Global.InvalidPreAuthorizedCode);
            if (context.Client.IsTransactionCodeRequired && preAuthCode.TransactionCode != transactionCode) throw new OAuthException(ErrorCodes.INVALID_GRANT, Global.InvalidTransactionCode);
            return preAuthCode;
        }
    }
}
