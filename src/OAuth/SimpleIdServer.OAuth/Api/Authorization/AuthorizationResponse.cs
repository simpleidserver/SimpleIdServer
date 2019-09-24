// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Api.Authorization
{
    public class AuthorizationResponse
    {
        protected AuthorizationResponse(AuthorizationResponseTypes type)
        {
            Type = type;
        }

        public AuthorizationResponseTypes Type { get; private set; }
    }
}