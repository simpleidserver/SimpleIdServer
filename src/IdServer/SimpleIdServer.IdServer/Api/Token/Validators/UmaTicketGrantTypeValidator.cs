// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IUmaTicketGrantTypeValidator
    {
        void Validate(HandlerContext context);
    }

    public class UmaTicketGrantTypeValidator : IUmaTicketGrantTypeValidator
    {
        public void Validate(HandlerContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.RequestData.GetTicket()))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.Ticket));

            var claimToken = context.Request.RequestData.GetClaimToken();
            var claimTokenFormat = context.Request.RequestData.GetClaimTokenFormat();
            if (!string.IsNullOrWhiteSpace(claimToken) && string.IsNullOrWhiteSpace(claimTokenFormat))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.ClaimTokenFormat));

            if (!string.IsNullOrWhiteSpace(claimTokenFormat) && string.IsNullOrWhiteSpace(claimToken))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.ClaimToken));
        }
    }
}
