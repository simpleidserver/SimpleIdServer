// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;

namespace SimpleIdServer.IdServer.Api.Token.TokenProfiles
{
    public class MacTokenProfile : ITokenProfile
    {
        public string Profile => DEFAULT_NAME;
        public static string DEFAULT_NAME = "mac";

        public void Enrich(HandlerContext context)
        {
            // check response contains access_token
            string accessToken;
            if (context.Response.TryGet(TokenResponseParameters.AccessToken, out accessToken))
            {
                context.Response.Add("mac_key", "mac_key");
                context.Response.Add("kid", "kid");
                context.Response.Add("mac_algorithm", "mac_algorithm");
            }
        }
    }
}