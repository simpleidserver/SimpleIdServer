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
            if (string.IsNullOrWhiteSpace(handlerContext.Request.HttpBody.GetTicket()))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(UMAErrorMessages.MISSING_PARAMETER, UMATokenRequestParameters.Ticket));
            }

            var claimToken = handlerContext.Request.HttpBody.GetClaimToken();
            var claimTokenFormat = handlerContext.Request.HttpBody.GetClaimTokenFormat();
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
