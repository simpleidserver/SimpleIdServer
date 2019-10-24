// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.Uma.DTOs;
using SimpleIdServer.Uma.Extensions;

namespace SimpleIdServer.Uma.Api.Token.Validators
{
    public interface IUmaTicketGrantTypeValidator
    {
        void Validate(HandlerContext handlerContext);
    }

    public class UmaTicketGrantTypeValidator : IUmaTicketGrantTypeValidator
    {
        public void Validate(HandlerContext handlerContext)
        {
            if (string.IsNullOrWhiteSpace(handlerContext.Request.Data.GetTicket()))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(UMAErrorMessages.MISSING_PARAMETER, UMATokenRequestParameters.Ticket));
            }

            var claimToken = handlerContext.Request.Data.GetClaimToken();
            var claimTokenFormat = handlerContext.Request.Data.GetClaimTokenFormat();
            if (!string.IsNullOrWhiteSpace(claimToken) && string.IsNullOrWhiteSpace(claimTokenFormat))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(UMAErrorMessages.MISSING_PARAMETER, UMATokenRequestParameters.ClaimTokenFormat));
            }

            if (!string.IsNullOrWhiteSpace(claimTokenFormat) && string.IsNullOrWhiteSpace(claimToken))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(UMAErrorMessages.MISSING_PARAMETER, UMATokenRequestParameters.ClaimToken));
            }
        }
    }
}
