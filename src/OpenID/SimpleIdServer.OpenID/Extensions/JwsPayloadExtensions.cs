// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class JwsPayloadExtensions
    {
        public static string GetSub(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetClaimValue(UserClaims.Subject);
        }

        public static string GetName(this JwsPayload jwsPayload)
        {
            return jwsPayload.GetClaimValue(UserClaims.Name);
        }

        public static IEnumerable<AuthorizationRequestClaimParameter> GetClaims(this JwsPayload jObj)
        {
            if (!jObj.ContainsKey(AuthorizationRequestParameters.Claims))
            {
                return new AuthorizationRequestClaimParameter[0];
            }

            return ((JObject)JsonConvert.DeserializeObject(jObj[AuthorizationRequestParameters.Claims].ToString())).GetClaims();
        }
    }
}
