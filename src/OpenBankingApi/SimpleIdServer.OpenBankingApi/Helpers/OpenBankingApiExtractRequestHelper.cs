// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Helpers;
using System;
using System.Linq;

namespace SimpleIdServer.OpenBankingApi.Helpers
{
    public class OpenBankingApiExtractRequestHelper : ExtractRequestHelper
    {
        public OpenBankingApiExtractRequestHelper(IRequestObjectValidator requestObjectValidator) : base(requestObjectValidator)
        {
        }


        protected override void CheckRequestObject(JwsHeader header, JwsPayload jwsPayload, OpenIdClient openidClient, HandlerContext context)
        {
            base.CheckRequestObject(header, jwsPayload, openidClient, context);
            if (!jwsPayload.ContainsKey(Jwt.Constants.OAuthClaims.ExpirationTime))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, Jwt.Constants.OAuthClaims.ExpirationTime));
            }

            var currentDateTime = DateTime.UtcNow.ConvertToUnixTimestamp();
            var exp = jwsPayload.GetExpirationTime();
            if (currentDateTime > exp)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.REQUEST_OBJECT_IS_EXPIRED);
            }

            var audiences = jwsPayload.GetAudiences();
            if (audiences.Any() && !audiences.Contains(context.Request.IssuerName))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST_OBJECT, ErrorMessages.REQUEST_OBJECT_BAD_AUDIENCE);
            }
        }
    }
}
