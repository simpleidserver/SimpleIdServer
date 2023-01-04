// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class AuthorizationResponseParameters
    {
        public const string State = "state";
        public const string Code = "code";
        public const string RefreshToken = "refresh_token";
        public const string AccessToken = "access_token";
        public const string IdToken = "id_token";
    }
}
